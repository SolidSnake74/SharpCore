using System;
using System.Collections;
using System.Configuration;

using SharpCore.Collections.Generic;

namespace SharpCore.Caching
{
	/// <summary>
	/// Provides caching functionality for one or more contexts of an application.
	/// </summary>
	public static class Cache
	{
		/// <summary>
		/// Used to disable item time based expirations.
		/// </summary>
		public static DateTime NoAbsoluteExpiration = DateTime.MaxValue;

		/// <summary>
		/// Used to disable sliding expirations.
		/// </summary>
		public static TimeSpan NoSlidingExpiration = TimeSpan.Zero;

		/// <summary>
		/// The collection of configured caches to be used.
		/// </summary>
		private static SynchronizedDictionary<string, CacheBase> caches;

		/// <summary>
		/// Initializes the static members of the Cache class.
		/// </summary>
		static Cache()
		{
			CachingSectionHandler sectionHandler = (CachingSectionHandler) ConfigurationManager.GetSection("sharpCore/caching");
			if (sectionHandler == null)
			{
				throw new TypeInitializationException(typeof(Cache).FullName, new ConfigurationErrorsException("The sharpCore/caching configuration section has not been defined."));
			}
			else
			{
				caches = sectionHandler.Caches;
			}
		}

		/// <summary>
		/// Adds an item with no absolute expiration and no sliding expiration to each cache configured for the specified context.
		/// </summary>
		/// <param name="context">The caching context in which the item should be stored.</param>
		/// <param name="key">The key used to identify the item in the caching context.</param>
		/// <param name="item">The item to be stored in the caching context.</param>
		public static void Add(string context, string key, object item)
		{
			Add(context, key, item, Cache.NoAbsoluteExpiration);
		}

		/// <summary>
		/// Adds an item with an absolute expiration policy to each cache configured for the specified context.
		/// </summary>
		/// <param name="context">The caching context in which the item should be stored.</param>
		/// <param name="key">The key used to identify the item in the caching context.</param>
		/// <param name="item">The item to be stored in the caching context.</param>
		/// <param name="absoluteExpiration">The time at which the item expires and can be removed from the caching context.</param>
		public static void Add(string context, string key, object item, DateTime absoluteExpiration)
		{
			CacheBase cache = (CacheBase) caches[context];
			cache.Add(key, item, absoluteExpiration, Cache.NoSlidingExpiration);
		}

		/// <summary>
		/// Adds an item with a sliding expiration policy to each cache configured for the specified context.
		/// </summary>
		/// <param name="context">The caching context in which the item should be stored.</param>
		/// <param name="key">The key used to identify the item in the caching context.</param>
		/// <param name="item">The item to be stored in the caching context.</param>
		/// <param name="slidingExpiration">The interval between the time the item is last accessed and when that item expires and can be removed from the caching context.</param>
		public static void Add(string context, string key, object item, TimeSpan slidingExpiration)
		{
			CacheBase cache = (CacheBase) caches[context];
			cache.Add(key, item, DateTime.Now.Add(slidingExpiration), slidingExpiration);
		}

		/// <summary>
		/// Retrieves the specified item from the specified caching context.
		/// </summary>
		/// <param name="context">The context for the item to be retrieved.</param>
		/// <param name="key">The identifier for the item to be retrieved.</param>
		/// <returns>The retrieved cached item, or a null reference if the key is not found.</returns>
		public static object Get(string context, string key)
		{
			CacheBase cache = (CacheBase) caches[context];
			return cache.Get(key);
		}

		/// <summary>
		/// Retrieves an enumerator used to iterate through the keys contained in the caching context.
		/// </summary>
		/// <param name="context">The context that the enumerator should be retrieved for.</param>
		/// <returns>An enumerator to iterate through the <see cref="SharpCore.Caching.CacheBase"/> object.</returns>
		public static IEnumerator GetEnumerator(string context)
		{
			CacheBase cache = (CacheBase) caches[context];
			return cache.GetEnumerator();
		}

		/// <summary>
		/// Removes the specified item from the specified caching context.
		/// </summary>
		/// <param name="context">The caching context for the item to remove.</param>
		/// <param name="key">The identifier for the item to remove.</param>
		public static void Remove(string context, string key)
		{
			CacheBase cache = (CacheBase) caches[context];
			cache.Remove(key);
		}

		/// <summary>
		/// Updates a cache item identified by the specified key in the specified caching context.
		/// </summary>
		/// <param name="context">The caching context for the item to be updated.</param>
		/// <param name="key">The identifier for the item to be updated.</param>
		/// <param name="item">The value for the item to be updated.</param>
		public static void Update(string context, string key, object item)
		{
			CacheBase cache = (CacheBase) caches[context];
			cache.Update(key, item);
		}
	}
}
