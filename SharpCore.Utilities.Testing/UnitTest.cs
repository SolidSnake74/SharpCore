using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpCore.Utilities;

namespace SharpCore.Utilities.Testing
{
	/// <summary>
	/// Provides unit tests for the SharpCore.Utilities functionality.
	/// </summary>
	[TestClass]
	public class UnitTest
	{
		[TestMethod]
		public void EncryptionUnitTest()
		{
			string value = DateTime.Now.ToLongDateString();

			string encryptedValue = EncryptionUtility.Encrypt(value);
			Assert.AreNotEqual<string>(value, encryptedValue);
			Assert.AreNotSame(value, encryptedValue);

			string decryptedValue = EncryptionUtility.Decrypt(encryptedValue);
			Assert.AreEqual<string>(value, decryptedValue);
			Assert.AreNotSame(value, decryptedValue);
		}
		
		[TestMethod]
		public void IsNullOrEmptyTest()
		{
			// ArrayList
			ArrayList arrayList = null;
			Assert.IsTrue(ValidationUtility.IsNullOrEmpty(arrayList));
			arrayList = new ArrayList();
			Assert.IsTrue(ValidationUtility.IsNullOrEmpty(arrayList));
			arrayList.Add("abc");
			Assert.IsFalse(ValidationUtility.IsNullOrEmpty(arrayList));
			
			// Hashtable
			Hashtable hashtable = null;
			Assert.IsTrue(ValidationUtility.IsNullOrEmpty(hashtable));
			hashtable = new Hashtable();
			Assert.IsTrue(ValidationUtility.IsNullOrEmpty(hashtable));
			hashtable.Add(123, "abc");
			Assert.IsFalse(ValidationUtility.IsNullOrEmpty(hashtable));
			
			// List<>
			List<string> list = null;
			Assert.IsTrue(ValidationUtility.IsNullOrEmpty(list));
			list = new List<string>();
			Assert.IsTrue(ValidationUtility.IsNullOrEmpty(list));
			list.Add("abc");
			Assert.IsFalse(ValidationUtility.IsNullOrEmpty(list));

			// Dictionary<>
			Dictionary<int, string> dictionary = null;
			Assert.IsTrue(ValidationUtility.IsNullOrEmpty(dictionary));
			dictionary = new Dictionary<int, string>();
			Assert.IsTrue(ValidationUtility.IsNullOrEmpty(dictionary));
			dictionary.Add(123, "abc");
			Assert.IsFalse(ValidationUtility.IsNullOrEmpty(dictionary));
		}
	}
}
