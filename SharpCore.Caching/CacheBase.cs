using System;
using System.Collections;
using System.Threading;
using System.Xml;

namespace SharpCore.Caching
{
	/// <summary>
	/// Defines the methods shared by all cache classes.
	/// </summary>
	public abstract class CacheBase : IEnumerable
	{
		/// <summary>
		/// The locking mechanism used to aid in synchronization.
		/// </summary>
		private static ReaderWriterLock rwLock;
		
		/// <summary>
		/// The lock that implements single-writer/multiple-reader semantics specific to the current thread of execution (denoted by the ThreadStatic attribute).
		/// </summary>
		[ThreadStatic]
		private static object lockCookie;
		
		/// <summary>
		/// The interval at which the cache should be inspected and purged.
		/// </summary>
		protected TimeSpan purgeInterval;

		/// <summary>
		/// The context assigned to the cache.
		/// </summary>
		protected string context;

		/// <summary>
		/// The <see cref="System.Threading.Timer"/> used to intiate cache inspection and purging.
		/// </summary>
		protected Timer timer;

		/// <summary>
		/// Reads the configuration and configures the cache instance.
		/// </summary>
		/// <param name="element">An <b>XmlElement</b> that contains the configuration information for the cache instance.</param>
		public virtual void Configure(XmlElement element)
		{
			if (element.HasAttribute("context"))
			{
				context = element.GetAttribute("context");
			}
			else
			{
				context = "default";
			}

			if (element.HasAttribute("purgeInterval"))
			{
				purgeInterval = TimeSpan.Parse(element.GetAttribute("purgeInterval"));
			}
			else
			{
				purgeInterval = TimeSpan.FromMinutes(1);
			}

			rwLock = new ReaderWriterLock();
			timer = new Timer(new TimerCallback(TimerCallback), null, purgeInterval, purgeInterval);
		}

		/// <summary>
		/// Adds an item to the cache.
		/// </summary>
		/// <param name="key">The key used to identify the item in the cache.</param>
		/// <param name="item">The item to be stored in the cache.</param>
		/// <param name="absoluteExpiration">The time at which the item expires and can be removed from the cache.</param>
		/// <param name="slidingExpiration">The interval between the time the item is last accessed and when that item expires and can be removed from the cache.</param>
		public abstract void Add(string key, object item, DateTime absoluteExpiration, TimeSpan slidingExpiration);

		/// <summary>
		/// Determines whether the specified key refers to an existing item in the cache.
		/// </summary>
		/// <param name="key">The identifier for the cache item to check for.</param>
		/// <returns><b>true</b> if path refers to an existing cached item; otherwise, <b>false</b>.</returns>
		public abstract bool Exists(string key);

		/// <summary>
		/// Gets the specified item from the cache.
		/// </summary>
		/// <param name="key">The identifier for the cache item to retrieve.</param>
		/// <returns>The retrieved cache item, or a null reference if the key is not found.</returns>
		public abstract object Get(string key);

		/// <summary>
		/// Get a <b>CachedItem</b> contained from the cache.
		/// </summary>
		/// <param name="key">The identifier for the cache item to retrieve.</param>
		/// <returns>The retrieved CachedItem instance, or a null reference if the key is not found.</returns>
		protected abstract CachedItem GetCachedItem(string key);

		/// <summary>
		/// Deletes all items contained in the cache.
		/// </summary>
		protected abstract void Purge();

		/// <summary>
		/// Removes the specified item from the cache.
		/// </summary>
		/// <param name="key">The identifier for the cache item to remove.</param>
		public abstract void Remove(string key);

		/// <summary>
		/// Updates the cache item identified by the specified key.
		/// </summary>
		/// <param name="key">The identifier for the cache item to update.</param>
		/// <param name="item">The value for the item to be updated.</param>
		public abstract void Update(string key, object item);

		/// <summary>
		/// Retrieves an enumerator used to iterate through the keys contained in the cache.
		/// </summary>
		/// <returns>An enumerator to iterate through the <see cref="SharpCore.Caching.CacheBase"/> object.</returns>
		public abstract IEnumerator GetEnumerator();

		/// <summary>
		/// Handles calls from the purge timer.
		/// </summary>
		/// <param name="state">An object containing application-specific information relevant to the method invoked by this delegate, or a null reference.</param>
		protected virtual void TimerCallback(object state)
		{
			Purge();
		}

		/// <summary>
		/// Acquires a writer lock.
		/// </summary>
		protected void AcquireWriterLock()
		{
			if (rwLock.IsReaderLockHeld)
			{
				if (rwLock.IsWriterLockHeld == false)
				{
					lockCookie = rwLock.UpgradeToWriterLock(Timeout.Infinite);
				}
			}
			else if (rwLock.IsWriterLockHeld == false)
			{
				rwLock.AcquireWriterLock(Timeout.Infinite);
			}
		}

		/// <summary>
		/// Releases a writer lock.
		/// </summary>
		protected void ReleaseWriterLock()
		{
			if (lockCookie != null)
			{
				LockCookie cookie = (LockCookie) lockCookie;
				rwLock.DowngradeFromWriterLock(ref cookie);
				lockCookie = null;
			}
			else
			{
				rwLock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Acquires a reader lock.
		/// </summary>
		protected void AcquireReaderLock()
		{
			if (rwLock.IsReaderLockHeld == false && rwLock.IsWriterLockHeld == false)
			{
				rwLock.AcquireReaderLock(Timeout.Infinite);
			}
		}

		/// <summary>
		///  Releases a reader lock.
		/// </summary>
		protected void ReleaseReaderLock()
		{
			if (rwLock.IsReaderLockHeld && rwLock.IsWriterLockHeld == false)
			{
				rwLock.ReleaseReaderLock();
			}
		}
	}
}
