using System;
using System.Collections;
using System.Configuration;
using System.Messaging;
using System.Reflection;
using System.Threading;

using SharpCore.Caching;

namespace SharpCore.Caching.Caches
{
	/// <summary>
	/// Represents a cache in which items are stored locally, and changes and additions are propogated through one or more MS Message Queue queues.
	/// </summary>
	internal sealed class MessageQueueCache : CacheBase
	{
		private MessageQueue queue;
		private SimpleCache cache;
		private MethodInfo getCachedItemMethod;
		private MethodInfo purgeMethod;
		private bool listen;
		private Thread listenThread;

		~MessageQueueCache()
		{
			if (queue != null)
			{
				listen = false;
				listenThread.Join();
				queue.Close();
			}
		}

		public override void Configure(System.Xml.XmlElement element)
		{
			// Reflect some of the necessary protected methods on the SimpleCache class that are needed
			Type type = typeof(SimpleCache);
			getCachedItemMethod = type.GetMethod("GetCachedItem", BindingFlags.Instance | BindingFlags.NonPublic);
			purgeMethod = type.GetMethod("Purge", BindingFlags.Instance | BindingFlags.NonPublic);

			// Instantiate and configure the SimpleCache instance to be used for the local cache
			cache = new SimpleCache();
			cache.Configure(element);

			// Allow the base CacheBase class to configure itself
			base.Configure(element);

			// Check for and configure the queue
			if (element.HasAttribute("path"))
			{
				string path = element.GetAttribute("path");
				if (MessageQueue.Exists(path))
				{
					queue = new MessageQueue(path);
					queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(CachedItem) });

					// Start the listener thread
					listen = true;
					listenThread = new Thread(new ThreadStart(Listen));
					listenThread.IsBackground = true;
					listenThread.Start();
				}
				else
				{
					throw new ConfigurationErrorsException("The specified queue path (" + path + ") does not exist.");
				}
			}
		}

		public override void Add(string key, object item, DateTime absoluteExpiration, TimeSpan slidingExpiration)
		{
			cache.Add(key, item, absoluteExpiration, slidingExpiration);

			if (queue != null)
			{
				CachedItem cachedItem = (CachedItem) getCachedItemMethod.Invoke(cache, new object[] { key });
				queue.Send(cachedItem);
			}
		}

		public override bool Exists(string key)
		{
			return cache.Exists(key);
		}

		public override object Get(string key)
		{
			return cache.Get(key);
		}

		protected override CachedItem GetCachedItem(string key)
		{
			return (CachedItem) getCachedItemMethod.Invoke(cache, new object[] { key });
		}

		protected override void Purge()
		{
			purgeMethod.Invoke(cache, null);
		}

		public override void Remove(string key)
		{
			cache.Remove(key);
		}

		public override void Update(string key, object item)
		{
			cache.Update(key, item);

			if (queue != null)
			{
				CachedItem cachedItem = (CachedItem) getCachedItemMethod.Invoke(cache, new object[] { key });
				queue.Send(cachedItem);
			}
		}

		public override IEnumerator GetEnumerator()
		{
			return cache.GetEnumerator();
		}

		/// <summary>
		/// Method wrapped in a ThreadStart delegate to allow for asynchronous queue updates
		/// </summary>
		private void Listen()
		{
			while (listen)
			{
				try
				{
					// The Receive() method will throw a MessageQueueException if no messages are present
					Message message = queue.Receive(new TimeSpan(TimeSpan.TicksPerSecond));

					// Take the new message and update the local cache
					CachedItem cachedItem = (CachedItem) message.Body;
					if (cache.Exists(cachedItem.Key))
					{
						cache.Update(cachedItem.Key, cachedItem.Item);
					}
					else
					{
						cache.Add(cachedItem.Key, cachedItem.Item, cachedItem.AbsoluteExpiration, cachedItem.SlidingExpiration);
					}

					// Yield to other threads
					Thread.Sleep(TimeSpan.Zero);
				}
				catch (MessageQueueException e)
				{
					// Ignore the IOTimeout exceptions; they are only thrown if there are no messages in the queue when Receive() is called
					if (e.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
					{
						throw;
					}
				}
			}
		}
	}
}
