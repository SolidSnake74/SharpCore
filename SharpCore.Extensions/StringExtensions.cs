using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using SharpCore.Utilities;

namespace SharpCore.Extensions
{
	/// <summary>
	/// Extends the <see ref="System.String"/> class to include functions for determining its contents.
	/// </summary>
	public static class StringExtensions
	{
		// Contains a corresponding compiled regular expressions
		private static Dictionary<ResourceKey, Regex> regexes = new Dictionary<ResourceKey, Regex>();
		
		/// <summary>
		/// Determines if the specified string only contains alpha characters.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><b>true</b> if the value only contains alpha characters; otherwise, <b>false</b>.</returns>
		public static bool IsAlpha(this string value)
		{
			return IsMatch(value, Resources.Alpha);
		}

		/// <summary>
		/// Determines if the specified string only contains alpha or numeric characters.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><b>true</b> if the value only contains alpha or numeric characters; otherwise, <b>false</b>.</returns>
		public static bool IsAlphaNumeric(this string value)
		{
			return IsMatch(value, Resources.AlphaNumeric);
		}

		/// <summary>
		/// Determines if the specified string is formatted as a date.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><b>true</b> if the value is formatted as a date; otherwise, <b>false</b>.</returns>
		public static bool IsDate(this string value)
		{
			return IsMatch(value, Resources.Date);
		}

		/// <summary>
		/// Determines if the specified string is formatted as an email address.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><b>true</b> if the value is an email address; otherwise, <b>false</b>.</returns>
		public static bool IsEmailAddress(this string value)
		{
			return IsMatch(value, Resources.EmailAddress);
		}

		/// <summary>
		/// Determines if the specified string is formatted as a GUID.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><b>true</b> if the value is formatted as a GUID; otherwise, <b>false</b>.</returns>
		public static bool IsGuid(this string value)
		{
			return IsMatch(value, Resources.Guid);
		}

		/// <summary>
		/// Determines if the specified string is formatted as an integer.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><b>true</b> if the value is formatted as an integer; otherwise, <b>false</b>.</returns>
		public static bool IsInteger(this string value)
		{
			return IsMatch(value, Resources.Integer);
		}

		/// <summary>
		/// Determines if the specified string is formatted as a non-zero integer.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><b>true</b> if the value is formatted as a non-zero integer; otherwise, <b>false</b>.</returns>
		public static bool IsNonZeroInteger(this string value)
		{
			return IsMatch(value, Resources.NonZeroInteger);
		}

		/// <summary>
		/// Determines if the specified string is formatted as a phone number.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><b>true</b> if the value is formatted as a phone number; otherwise, <b>false</b>.</returns>
		public static bool IsPhoneNumber(this string value)
		{
			return IsMatch(value, Resources.PhoneNumber);
		}

		/// <summary>
		/// Determines if the specified string is formatted as a real (floating-point number).
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><b>true</b> if the value is formatted as a real; otherwise, <b>false</b>.</returns>
		public static bool IsReal(this string value)
		{
			return IsMatch(value, Resources.Real);
		}

		/// <summary>
		/// Determines if the specified string is formatted as a social security number.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><b>true</b> if the value is formatted as a social security number; otherwise, <b>false</b>.</returns>
		public static bool IsSocialSecurityNumber(this string value)
		{
			return IsMatch(value, Resources.SocialSecurityNumber);
		}

		/// <summary>
		/// Determines if the specified string is formatted as a time.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><b>true</b> if the value is formatted as a time; otherwise, <b>false</b>.</returns>
		public static bool IsTime(this string value)
		{
			return IsMatch(value, Resources.Time);
		}

		/// <summary>
		/// Determines if the specified string is formatted as a URL.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><b>true</b> if the value is formatted as a URL; otherwise, <b>false</b>.</returns>
		public static bool IsUrl(this string value)
		{
			return IsMatch(value, Resources.Url);
		}

		/// <summary>
		/// Determines if the specified string is formatted as a zipcode.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><b>true</b> if the value is formatted as a zipcode; otherwise, <b>false</b>.</returns>
		public static bool IsZipcode(this string value)
		{
			return IsMatch(value, Resources.Zipcode);
		}
		
		/// <summary>
		/// Creates an MD5 hash for the string.
		/// </summary>
		/// <param name="value">The value to create a hash for.</param>
		/// <returns>The MD5 hash of the string.</returns>
		public static string ToMD5(this string value)
		{
			ValidationUtility.ValidateArgument("value", value, false);
			
			using (HashAlgorithm algorithm = new MD5CryptoServiceProvider())
			{
				byte[] bytes = Encoding.UTF8.GetBytes(value);
				bytes = algorithm.ComputeHash(bytes);
				string hashString = BitConverter.ToString(bytes);
				return hashString;
			}
		}

		/// <summary>
		/// Creates an SHA1 hash for the string.
		/// </summary>
		/// <param name="value">The value to create a hash for.</param>
		/// <returns>The MD5 hash of the string.</returns>
		public static string ToSHA1(this string value)
		{
			ValidationUtility.ValidateArgument("value", value, false);

			using (HashAlgorithm algorithm = new SHA1CryptoServiceProvider())
			{
				byte[] bytes = Encoding.UTF8.GetBytes(value);
				bytes = algorithm.ComputeHash(bytes);
				string hashString = BitConverter.ToString(bytes);
				return hashString;
			}
		}

		/// <summary>
		/// Creates an SHA256 hash for the string.
		/// </summary>
		/// <param name="value">The value to create a hash for.</param>
		/// <returns>The MD5 hash of the string.</returns>
		public static string ToSHA256(this string value)
		{
			ValidationUtility.ValidateArgument("value", value, false);

			using (HashAlgorithm algorithm = new SHA256CryptoServiceProvider())
			{
				byte[] bytes = Encoding.UTF8.GetBytes(value);
				bytes = algorithm.ComputeHash(bytes);
				string hashString = BitConverter.ToString(bytes);
				return hashString;
			}
		}
		
		/// <summary>
		/// Determines if the specified value matches the specified Regex.
		/// </summary>
		/// <param name="value">The value to match.</param>
		/// <param name="resourceName">The name of the Regex to match against.</param>
		/// <returns><b>true</b> if the value matches the Regex; otherwise, <b>false</b>.</returns>
		private static bool IsMatch(string value, string pattern)
		{
			ValidationUtility.ValidateArgument("value", value, false);
			
			ResourceKey resourceKey = new ResourceKey(pattern, CultureInfo.CurrentUICulture);
			Regex regex = GetRegex(resourceKey);
			return regex.IsMatch(value);
		}

		/// <summary>
		/// Retrieves a Regex identified by pattern.
		/// </summary>
		/// <param name="resourceKey">The resource key of the Regex.</param>
		/// <returns>The Regex for the specified resource key.</returns>
		private static Regex GetRegex(ResourceKey resourceKey)
		{
			Regex regex;
			
			if (regexes.TryGetValue(resourceKey, out regex) == false)
			{
				regex = new Regex(resourceKey.ResourceName, RegexOptions.Compiled);
				regexes.Add(resourceKey, regex);
			}
			
			return regex;
		}
	}
}
