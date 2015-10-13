using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml;

using SharpCore.Caching;
using SharpCore.Communications;

namespace SharpCore.Caching.Caches
{
	/// <summary>
	/// Represents a cache in which items are stored locally, and changes and additions are distributed to other configured caches, typically on other servers.
	/// </summary>
	internal sealed class DistributedCache : CacheBase
	{
		private CacheBase cache;
		private SessionServer server;
		private List<SessionClient> clients;
		MethodInfo getCachedItemMethod;
		MethodInfo purgeMethod;

		public override void Configure(XmlElement element)
		{
			base.Configure(element);

			XmlElement localCacheElement = (XmlElement) element.SelectSingleNode("localCache");

			string typeName;
			if (localCacheElement.HasAttribute("type"))
			{
				typeName = localCacheElement.GetAttribute("type");
			}
			else
			{
				typeName = typeof(SimpleCache).FullName;
			}

			Type type = Type.GetType(typeName);
			getCachedItemMethod = type.GetMethod("GetCachedItem", BindingFlags.Instance | BindingFlags.NonPublic);
			purgeMethod = type.GetMethod("Purge", BindingFlags.Instance | BindingFlags.NonPublic);

			cache = (CacheBase) type.Assembly.CreateInstance(type.FullName);
			cache.Configure(localCacheElement);

			int port;
			if (element.HasAttribute("port"))
			{
				port = Int32.Parse(element.GetAttribute("port"));

				server = new SessionServer(IPAddress.Any, port);
				server.CommuniqueReceived += new CommuniqueReceivedEventHandler(CommuniqueReceived);
			}
			else
			{
				throw new ConfigurationErrorsException("The port attribute is required for a distributedCache element.");
			}

			clients = new List<SessionClient>();
			foreach (XmlElement serverElement in element.SelectNodes("server"))
			{
				if (serverElement.HasAttribute("port"))
				{
					SessionClient client;

					port = Int32.Parse(serverElement.GetAttribute("port"));

					if (serverElement.HasAttribute("address"))
					{
						client = new SessionClient(IPAddress.Parse(serverElement.GetAttribute("address")), port);
					}
					else if (serverElement.HasAttribute("hostName"))
					{
						client = new SessionClient(serverElement.GetAttribute("hostName"), port);
					}
					else
					{
						throw new ConfigurationErrorsException("The address or hostName attribute is required for a distributedCache server element.");
					}

					clients.Add(client);
				}
				else
				{
					throw new ConfigurationErrorsException("The port attribute is required for a distributedCache server element.");
				}
			}
		}

		public override void Add(string key, object item, DateTime absoluteExpiration, TimeSpan slidingExpiration)
		{
			cache.Add(key, item, absoluteExpiration, slidingExpiration);
			CachedItem cachedItem = GetCachedItem(key);

			foreach (SessionClient client in clients)
			{
				SendCommunique(client, cachedItem);
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
			CachedItem cachedItem = GetCachedItem(key);

			foreach (SessionClient client in clients)
			{
				SendCommunique(client, cachedItem);
			}
		}

		public override IEnumerator GetEnumerator()
		{
			return cache.GetEnumerator();
		}

		/// <summary>
		/// Handles the CommiqueReceived event raised by the configured SessionServer.  The specified CachedItem is added or updated within the local cache.
		/// </summary>
		/// <param name="sender">The SessionServer instance that fired the event.</param>
		/// <param name="e">A CommuniqueReceviedEventArgs instance that contains the CachedItem to be modified.</param>
		private void CommuniqueReceived(object sender, CommuniqueReceviedEventArgs e)
		{
			CachedItem item = (CachedItem) e.Communique.Body;

			if (cache.Exists(item.Key))
			{
				cache.Update(item.Key, item.Item);
			}
			else
			{
				cache.Add(item.Key, item.Item, item.AbsoluteExpiration, item.SlidingExpiration);
			}
		}

		/// <summary>
		/// Sends an update for the specified CachedItem to all configured servers.
		/// </summary>
		/// <param name="client">The SessionClient used to send the update.</param>
		/// <param name="item">The CachedItem to be sent to all configured servers.</param>
		private void SendCommunique(SessionClient client, CachedItem item)
		{
			Communique communique = new Communique(item);
			client.SendCommunique(communique);
		}
	}
}
