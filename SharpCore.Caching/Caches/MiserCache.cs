// TODO: Determine if the MiserTimerCallback should prioritize the items moved to the persistent cache based on size (if it's possible)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;

namespace SharpCore.Caching.Caches
{
	/// <summary>
	/// Represents a cache in which items are stored in a volatile cache until the configured memory limit has been reached.
	/// When the configured memory limit has been reached, items are moved to the persistent cache until the consumed memory is no longer over the configured limit.
	/// </summary>
	internal sealed class MiserCache : CacheBase
	{
		private float availableMemoryLimit;
		private TimeSpan miserInterval;
		private MethodInfo getCachedItemMethod;
		private MethodInfo purgeMethod;
		private CacheBase volatileCache;
		private CacheBase persistentCache;
		private Timer miserTimer;

		public MiserCache()
		{
		}

		public override void Configure(System.Xml.XmlElement element)
		{
			base.Configure(element);

			if (element.HasAttribute("availableMemoryLimit"))
			{
				availableMemoryLimit = Single.Parse(element.GetAttribute("availableMemoryLimit"));
			}
			else
			{
				availableMemoryLimit = 96.0f;
			}

			if (element.HasAttribute("miserInterval"))
			{
				miserInterval = TimeSpan.Parse(element.GetAttribute("miserInterval"));
			}
			else
			{
				miserInterval = TimeSpan.FromMinutes(1);
			}

			XmlElement volatileElement = (XmlElement) element.SelectSingleNode("volatileCache");
			if (volatileElement.HasAttribute("type"))
			{
				Type type = Type.GetType(volatileElement.GetAttribute("type"));
				volatileCache = (CacheBase) type.Assembly.CreateInstance(type.FullName);
				volatileCache.Configure(volatileElement);
			}

			XmlElement persistentElement = (XmlElement) element.SelectSingleNode("persistentCache");
			if (persistentElement.HasAttribute("type"))
			{
				Type type = Type.GetType(persistentElement.GetAttribute("type"));
				persistentCache = (CacheBase) type.Assembly.CreateInstance(type.FullName);
				persistentCache.Configure(persistentElement);
			}

			Type cacheBaseType = typeof(CacheBase);
			getCachedItemMethod = cacheBaseType.GetMethod("GetCachedItem", BindingFlags.Instance | BindingFlags.NonPublic);
			purgeMethod = cacheBaseType.GetMethod("Purge", BindingFlags.Instance | BindingFlags.NonPublic);

			miserTimer = new Timer(new TimerCallback(MiserTimerCallback), null, miserInterval, miserInterval);
		}

		public override void Add(string key, object item, DateTime absoluteExpiration, TimeSpan slidingExpiration)
		{
			volatileCache.Add(key, item, absoluteExpiration, slidingExpiration);
		}

		public override bool Exists(string key)
		{
			if (volatileCache.Exists(key) || persistentCache.Exists(key))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public override object Get(string key)
		{
			CachedItem cachedItem = (CachedItem) volatileCache.Get(key);
			if (cachedItem == null)
			{
				cachedItem = (CachedItem) persistentCache.Get(key);
				if (cachedItem != null)
				{
					volatileCache.Add(key, cachedItem.Item, cachedItem.AbsoluteExpiration, cachedItem.SlidingExpiration);
					persistentCache.Remove(key);
				}
			}

			return cachedItem.Item;
		}

		protected override CachedItem GetCachedItem(string key)
		{
			CachedItem cachedItem = null;

			cachedItem = (CachedItem) getCachedItemMethod.Invoke(volatileCache, new object[] { key });
			if (cachedItem == null)
			{
				cachedItem = (CachedItem) getCachedItemMethod.Invoke(persistentCache, new object[] { key });
				if (cachedItem != null)
				{
					volatileCache.Add(key, cachedItem.Item, cachedItem.AbsoluteExpiration, cachedItem.SlidingExpiration);
					persistentCache.Remove(key);
				}
			}

			return cachedItem;
		}

		protected override void Purge()
		{
			purgeMethod.Invoke(volatileCache, null);
			purgeMethod.Invoke(persistentCache, null);
		}

		public override void Remove(string key)
		{
			volatileCache.Remove(key);
			persistentCache.Remove(key);
		}

		public override void Update(string key, object item)
		{
			if (volatileCache.Exists(key))
			{
				volatileCache.Update(key, item);
			}
			else
			{
				CachedItem cachedItem = (CachedItem) getCachedItemMethod.Invoke(persistentCache, new object[] { key });
				if (cachedItem != null)
				{
					volatileCache.Add(key, item, cachedItem.AbsoluteExpiration, cachedItem.SlidingExpiration);
					persistentCache.Remove(key);
				}
			}
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			List<string> keys = new List<string>();

			AcquireReaderLock();
			try
			{
				foreach (string key in volatileCache)
				{
					keys.Add(key);
				}

				foreach (string key in persistentCache)
				{
					keys.Add(key);
				}
			}
			finally
			{
				ReleaseReaderLock();
			}

			return keys.GetEnumerator();
		}

		private void MiserTimerCallback(object state)
		{
			if (GetAvailableMemory() < availableMemoryLimit)
			{
				List<string> keys = new List<string>();

				AcquireWriterLock();
				try
				{
					// Copy the keys, because an exception will be thrown if the original key collectionis changed while it is iterated through.
					foreach (string key in volatileCache)
					{
						keys.Add(key);
					}

					foreach (string key in keys)
					{
						if (GetAvailableMemory() < availableMemoryLimit)
						{
							CachedItem cachedItem = (CachedItem) getCachedItemMethod.Invoke(volatileCache, new object[] { key });

							// Only remove items that aren't about to expire; this will prioritize older objects first
							if (cachedItem.AbsoluteExpiration.Subtract(DateTime.Now) > TimeSpan.FromMinutes(1))
							{
								persistentCache.Add(key, cachedItem.Item, cachedItem.AbsoluteExpiration, cachedItem.SlidingExpiration);
								volatileCache.Remove(key);
							}

							System.GC.Collect();
						}
						else
						{
							break;
						}
					}
				}
				finally
				{
					ReleaseWriterLock();
				}
			}
		}

		private long GetAvailableMemory()
		{
			PerformanceCounter performanceCounter = new PerformanceCounter("Memory", "Available MBytes");
			return performanceCounter.RawValue;
		}
	}
}
