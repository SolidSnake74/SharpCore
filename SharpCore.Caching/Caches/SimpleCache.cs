using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using SharpCore.Caching;

namespace SharpCore.Caching.Caches
{
	/// <summary>
	/// Represents a cache in which items are stored in memory.
	/// </summary>
	internal sealed class SimpleCache : CacheBase
	{
		private Dictionary<string, CachedItem> dictionary;

		public SimpleCache()
		{
			dictionary = new Dictionary<string, CachedItem>();
		}

		public override void Configure(XmlElement element)
		{
			base.Configure(element);
		}

		public override void Add(string key, object item, DateTime absoluteExpiration, TimeSpan slidingExpiration)
		{
			AcquireWriterLock();
			try
			{
				CachedItem cachedItem = new CachedItem(key, item, absoluteExpiration, slidingExpiration);
				dictionary.Add(cachedItem.Key, cachedItem);
			}
			finally
			{
				ReleaseWriterLock();
			}
		}

		public override bool Exists(string key)
		{
			AcquireReaderLock();
			try
			{
				return dictionary.ContainsKey(key);
			}
			finally
			{
				ReleaseReaderLock();
			}
		}

		public override object Get(string key)
		{
			AcquireReaderLock();
			try
			{
				CachedItem cachedItem;
				if (dictionary.TryGetValue(key, out cachedItem))
				{
					if (cachedItem.SlidingExpiration != Cache.NoSlidingExpiration)
					{
						cachedItem.AbsoluteExpiration = DateTime.Now.Add(cachedItem.SlidingExpiration);
					}

					return cachedItem.Item;
				}
				else
				{
					return null;
				}
			}
			finally
			{
				ReleaseReaderLock();
			}
		}

		protected override CachedItem GetCachedItem(string key)
		{
			AcquireReaderLock();
			try
			{
				CachedItem cachedItem;
				if (dictionary.TryGetValue(key, out cachedItem))
				{
					return cachedItem;
				}
				else
				{
					return null;
				}
			}
			finally
			{
				ReleaseReaderLock();
			}
		}

		protected override void Purge()
		{
			AcquireReaderLock();
			try
			{
				List<string> keys = new List<string>();
				foreach (CachedItem cachedItem in dictionary.Values)
				{
					if (cachedItem.AbsoluteExpiration <= DateTime.Now)
					{
						keys.Add(cachedItem.Key);
					}
				}
				
				AcquireWriterLock();
				try
				{
					foreach (string key in keys)
					{
						RemoveCore(key);
					}
				}
				finally
				{
					ReleaseWriterLock();
				}
			}
			finally
			{
				ReleaseReaderLock();
			}
		}

		public override void Remove(string key)
		{
			AcquireWriterLock();
			try
			{
				RemoveCore(key);
			}
			finally
			{
				ReleaseWriterLock();
			}
		}

		public override void Update(string key, object item)
		{
			AcquireWriterLock();
			try
			{
				CachedItem cachedItem;
				if (dictionary.TryGetValue(key, out cachedItem))
				{
					cachedItem.Item = item;

					if (cachedItem.SlidingExpiration != Cache.NoSlidingExpiration)
					{
						cachedItem.AbsoluteExpiration.Add(cachedItem.SlidingExpiration);
					}
				}
			}
			finally
			{
				ReleaseWriterLock();
			}
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			AcquireReaderLock();
			try
			{
				List<string> keyList = new List<string>(dictionary.Keys.Count);
				foreach (string key in dictionary.Keys)
				{
					keyList.Add(key);
				}
				
				return keyList.GetEnumerator();
			}
			finally
			{
				ReleaseReaderLock();
			}
		}
		
		private void RemoveCore(string key)
		{
			CachedItem cachedItem;
			if (dictionary.TryGetValue(key, out cachedItem))
			{
				IDisposable disposable = cachedItem.Item as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}

				dictionary.Remove(key);
			}
		}
	}
}
