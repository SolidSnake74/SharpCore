using System;

namespace SharpCore.Caching
{
	/// <summary>
	/// Represents an item stored in a cache.
	/// </summary>
	[Serializable]
	public sealed class CachedItem
	{
		/// <summary>
		/// The identifier of the item.
		/// </summary>
		private string key;

		/// <summary>
		/// The value of the item.
		/// </summary>
		private object item;

		/// <summary>
		/// The time at which the item expires and can be removed from the cache.
		/// </summary>
		private DateTime absoluteExpiration;

		/// <summary>
		/// The interval between the time the item is last accessed and when that item expires.
		/// </summary>
		private TimeSpan slidingExpiration;

		/// <summary>
		/// Initializes a new instance of the <b>CachedItem</b> class.
		/// </summary>
		/// <remarks>This contructor is intended for serialization purposes only.</remarks>
		public CachedItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <b>CachedItem</b> class.
		/// </summary>
		/// <param name="key">The identifier of the item.</param>
		/// <param name="item">The value of the item.</param>
		/// <param name="absoluteExpiration">The time at which the item expires and can be removed from the cache.</param>
		/// <param name="slidingExpiration">The interval between the time the item is last accessed and when that item expires.</param>
		public CachedItem(string key, object item, DateTime absoluteExpiration, TimeSpan slidingExpiration)
		{
			this.key = key;
			this.item = item;
			this.absoluteExpiration = absoluteExpiration;
			this.slidingExpiration = slidingExpiration;
		}

		/// <summary>
		/// The identifier of the item.
		/// </summary>
		public string Key
		{
			get { return key; }
			set { key = value; }
		}

		/// <summary>
		/// The value of the item.
		/// </summary>
		public object Item
		{
			get { return item; }
			set { item = value; }
		}

		/// <summary>
		/// The time at which the item expires and can be removed from the cache.
		/// </summary>
		public DateTime AbsoluteExpiration
		{
			get { return absoluteExpiration; }
			set { absoluteExpiration = value; }
		}

		/// <summary>
		/// The interval between the time the item is last accessed and when that item expires.
		/// </summary>
		public TimeSpan SlidingExpiration
		{
			get { return slidingExpiration; }
			set { slidingExpiration = value; }
		}
	}
}
