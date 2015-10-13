using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpCore.Extensions;

namespace SharpCore.Extensions.Testing
{
	/// <summary>
	/// Summary description for UnitTest
	/// </summary>
	[TestClass]
	public class StringExtensionsUnitTest
	{
		[TestMethod]
		public void IsAlphaTest()
		{
			string value;
			
			value = "abc";
			Assert.IsTrue(value.IsAlpha());
			
			value = "123";
			Assert.IsFalse(value.IsAlpha());
			
			Assert.IsFalse(String.Empty.IsAlpha());
		}
		
		[TestMethod]
		public void IsAlphaNumericTest()
		{
			string value;
			
			value = "abc123";
			Assert.IsTrue(value.IsAlphaNumeric());
			
			value = "!@#";
			Assert.IsFalse(value.IsAlphaNumeric());
			
			Assert.IsFalse(String.Empty.IsAlphaNumeric());
		}
		
		[TestMethod]
		public void IsDateTest()
		{
			string value;
			
			value = "1/1/2001";
			Assert.IsTrue(value.IsDate());
			
			value = "123";
			Assert.IsFalse(value.IsDate());
			
			Assert.IsFalse(String.Empty.IsDate());
		}

		[TestMethod]
		public void IsEmailTest()
		{
			string value;
			
			value = "someone@somewhere.com";
			Assert.IsTrue(value.IsEmailAddress());
			
			value = "abc";
			Assert.IsFalse(value.IsEmailAddress());

			Assert.IsFalse(String.Empty.IsEmailAddress());
		}

		[TestMethod]
		public void IsGuidTest()
		{
			string value;
			
			value = Guid.Empty.ToString();
			Assert.IsTrue(value.IsGuid());
			
			value = "123";
			Assert.IsFalse(value.IsGuid());
			
			Assert.IsFalse(String.Empty.IsGuid());
		}

		[TestMethod]
		public void IsIntegerTest()
		{
			string value;
			
			value = "123";
			Assert.IsTrue(value.IsInteger());
			
			value = "-123";
			Assert.IsTrue(value.IsInteger());
			
			value = "123.4";
			Assert.IsFalse(value.IsInteger());
			
			Assert.IsFalse(String.Empty.IsInteger());
		}

		[TestMethod]
		public void Test()
		{
			string value;

			value = "123";
			Assert.IsTrue(value.IsNonZeroInteger());

			value = "-123";
			Assert.IsFalse(value.IsNonZeroInteger());

			value = "123.4";
			Assert.IsFalse(value.IsNonZeroInteger());

			Assert.IsFalse(String.Empty.IsNonZeroInteger());
		}

		[TestMethod]
		public void IsPhoneNumberTest()
		{
			string value;
			
			value = "123-456-7890";
			Assert.IsTrue(value.IsPhoneNumber());
			
			value = "(123) 456-7890";
			Assert.IsTrue(value.IsPhoneNumber());
			
			value = "abc";
			Assert.IsFalse(value.IsPhoneNumber());
			
			Assert.IsFalse(String.Empty.IsPhoneNumber());
		}

		[TestMethod]
		public void IsRealTest()
		{
			string value;

			value = "123";
			Assert.IsTrue(value.IsReal());

			value = "123.4";
			Assert.IsTrue(value.IsReal());
			
			value = "abc";
			Assert.IsFalse(value.IsReal());
			
			Assert.IsFalse(String.Empty.IsReal());
		}

		[TestMethod]
		public void IsSocialSecurityNumberTest()
		{
			string value;
			
			value = "123-45-6789";
			Assert.IsTrue(value.IsSocialSecurityNumber());
			
			value = "123";
			Assert.IsFalse(value.IsSocialSecurityNumber());

			Assert.IsFalse(String.Empty.IsSocialSecurityNumber());
		}

		[TestMethod]
		public void IsTimeTest()
		{
			string value;
			
			value = "01:01:0001";
			Assert.IsTrue(value.IsTime());
			
			value = "1:1:0001";
			Assert.IsTrue(value.IsTime());
			
			value = "123";
			Assert.IsFalse(value.IsTime());

			Assert.IsFalse(String.Empty.IsTime());
		}

		[TestMethod]
		public void IsUrlTest()
		{
			string value;
			
			value = "http://www.codeplex.com";
			Assert.IsTrue(value.IsUrl());
			
			value = "abc";
			Assert.IsFalse(value.IsUrl());

			Assert.IsFalse(String.Empty.IsUrl());
		}
		
		[TestMethod]
		public void IsZipcodeTest()
		{
			string value;
			
			value = "12345-6789";
			Assert.IsTrue(value.IsZipcode());
			
			value = "12345";
			Assert.IsTrue(value.IsZipcode());
			
			value = "123";
			Assert.IsFalse(value.IsZipcode());
			
			Assert.IsFalse(String.Empty.IsZipcode());
		}
		
		[TestMethod]
		public void ToMD5Test()
		{
			string value = "MD5";
			value = value.ToMD5();
			
			Assert.IsNotNull(value);
			Assert.IsTrue(value.Length > 0);
		}

		[TestMethod]
		public void ToSHA1Test()
		{
			string value = "SHA1";
			value = value.ToSHA1();

			Assert.IsNotNull(value);
			Assert.IsTrue(value.Length > 0);
		}

		[TestMethod]
		public void ToSHA256Test()
		{
			string value = "SHA256";
			value = value.ToSHA256();

			Assert.IsNotNull(value);
			Assert.IsTrue(value.Length > 0);
		}
	}
}
