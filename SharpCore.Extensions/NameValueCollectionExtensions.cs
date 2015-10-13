using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SharpCore.Extensions
{
	/// <summary>
	/// Extends the <see ref="System.Collections.Specialized.NameValueCollection"/> class to include functions for retrieving its contents with string types.
	/// </summary>
	public static class NameValueCollectionExtensions
	{
		/// <summary>
		/// Gets a specified entry from the NamveValueCollection as a <see cref="System.Boolean"/>.
		/// </summary>
		/// <param name="collection">The collection to retrive the value from.</param>
		/// <param name="key">The <see cref="System.String"/> key of the entry to locate.</param>
		/// <returns>A <see cref="System.String"/> that contains the comma-separated list of values associated with the specified key, if found; otherwise, <b>null</b>.</returns>
		public static bool GetBoolean(this NameValueCollection collection, string key)
		{
			string stringValue = collection[key];
			if (String.IsNullOrEmpty(stringValue) == false)
			{
				bool boolValue = Boolean.Parse(stringValue);
				return boolValue;
			}

			return false;
		}

		/// <summary>
		/// Gets a specified entry from the NamveValueCollection as a <see cref="System.Guid"/>.
		/// </summary>
		/// <param name="collection">The collection to retrive the value from.</param>
		/// <param name="key">The <see cref="System.String"/> key of the entry to locate.</param>
		/// <returns>A <see cref="System.Guid"/> associated with the specified key, if found; otherwise, <b>Guid.Empty</b>.</returns>
		public static Guid GetGuid(this NameValueCollection collection, string key)
		{
			string stringValue = collection[key];
			if (String.IsNullOrEmpty(stringValue) == false)
			{
				Guid guidValue = new Guid(stringValue);
				return guidValue;
			}

			return Guid.Empty;
		}

		/// <summary>
		/// Gets a specified entry from the NamveValueCollection as a <see cref="System.Int32"/>.
		/// </summary>
		/// <param name="collection">The collection to retrive the value from.</param>
		/// <param name="key">The <see cref="System.String"/> key of the entry to locate.</param>
		/// <returns>A <see cref="System.Int32"/> associated with the specified key, if found; otherwise, <b>0</b>.</returns>
		public static int GetInt32(this NameValueCollection collection, string key)
		{
			string stringValue = collection[key];
			if (String.IsNullOrEmpty(stringValue) == false)
			{
				int intValue = Int32.Parse(stringValue);
				return intValue;
			}
			
			return 0;
		}

		/// <summary>
		/// Gets a specified entry from the NamveValueCollection as a <see cref="System.String"/>.
		/// </summary>
		/// <param name="collection">The collection to retrive the value from.</param>
		/// <param name="key">The <see cref="System.String"/> key of the entry to locate.</param>
		/// <returns>A <see cref="System.String"/> that contains the comma-separated list of values associated with the specified key, if found; otherwise, <b>null</b>.</returns>
		/// <remarks>This method doesn't improve upon the built-in functionality; instead, it just provides a method for consistency.</remarks>
		public static string GetString(this NameValueCollection collection, string key)
		{
			string stringValue = collection[key];
			return stringValue;
		}

		/// <summary>
		/// Gets a specified entry from the NamveValueCollection as a <see cref="System.Collections.Generic.List"/> of <see cref="System.String"/>.
		/// </summary>
		/// <param name="collection">The collection to retrive the value from.</param>
		/// <param name="key">The <see cref="System.String"/> key of the entry to locate.</param>
		/// <returns>A <see cref="System.Collections.Generic.List"/> of <see cref="System.String"/> associated with the specified key.</returns>
		public static List<string> GetStringCollection(this NameValueCollection collection, string key, char delimiter)
		{
			List<string> stringList = new List<string>();

			string stringListValue = collection[key];
			if (String.IsNullOrEmpty(stringListValue) == false)
			{
				string[] stringListArray = stringListValue.Split(delimiter);
				foreach (string value in stringListArray)
				{
					string trimmedValue = value.Trim();
					stringList.Add(trimmedValue);
				}
			}

			return stringList;
		}

		/// <summary>
		/// Gets a specified entry from the NamveValueCollection as a <see cref="System.TimeSpan"/>.
		/// </summary>
		/// <param name="collection">The collection to retrive the value from.</param>
		/// <param name="key">The <see cref="System.String"/> key of the entry to locate.</param>
		/// <returns>A <see cref="System.TimeSpan"/> associated with the specified key, if found; otherwise, <b>TimeSpan.Zero</b>.</returns>
		public static TimeSpan GetTimeSpan(this NameValueCollection collection, string key)
		{
			string value = collection[key];
			if (String.IsNullOrEmpty(value) == false)
			{
				TimeSpan timeSpan = TimeSpan.Parse(value);
				return timeSpan;
			}

			return TimeSpan.Zero;
		}
	}
}
