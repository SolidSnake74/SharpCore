using System;
using System.Globalization;

namespace SharpCore.Extensions
{
	/// <summary>
	/// Represents a composite key used to uniquely identify a resource by name and culture.
	/// </summary>
	internal sealed class ResourceKey
	{
		private string resourceName;
		private CultureInfo cultureInfo;
		
		/// <summary>
		/// Initializes a new instance of the ResourceKey class.
		/// </summary>
		/// <param name="resourceName">The name of the resource this key is used to access.</param>
		/// <param name="cultureInfo">The culture of the resource this key is used to assess.</param>
		public ResourceKey(string resourceName, CultureInfo cultureInfo)
		{
			this.resourceName = resourceName;
			this.cultureInfo = cultureInfo;
		}
		
		/// <summary>
		/// Gets or sets the name of the resource this key is used to access.
		/// </summary>
		public string ResourceName
		{
			get { return resourceName; }
			set { resourceName = value; }
		}
		
		/// <summary>
		/// Gets or sets the culture of the resource this key is used to access.
		/// </summary>
		public CultureInfo CultureInfo
		{
			get { return cultureInfo; }
			set { cultureInfo = value; }
		}
		
		/// <summary>
		/// Returns a value indicating whether this instance is equal to the specified object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns><b>true</b> if obj is an instance of ResourceKey and equals the value of this instance; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			ResourceKey resourceKey = obj as ResourceKey;
			return Equals(resourceKey);
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to the specified ResourceKey.
		/// </summary>
		/// <param name="resourceKey">A ResourceKey to compare with this instance.</param>
		/// <returns><b>true</b> if resourceKey equals the value of this instance; otherwise, <b>false</b>.</returns>
		public bool Equals(ResourceKey resourceKey)
		{
			if (resourceKey == null)
			{
				return false;
			}
			
			if (resourceKey.resourceName != resourceName)
			{
				return false;
			}

			if (resourceKey.cultureInfo.LCID != cultureInfo.LCID)
			{
				return false;
			}

			return true;
		}
		
		/// <summary>
		/// Returns a hash code for this ResourceKey.
		/// </summary>
		/// <returns>A hash code for this ResourceKey.</returns>
		public override int GetHashCode()
		{
			return resourceName.GetHashCode() ^ cultureInfo.LCID.GetHashCode();
		}
	}
}
