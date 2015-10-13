using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpCore.Extensions.Testing
{
	[TestClass]
	public class NameValueCollectionExtensionsUnitTest
	{
		[TestMethod]
		public void GetBooleanTest()
		{
			NameValueCollection collection = new NameValueCollection();
			collection.Add("True", Boolean.TrueString);
			collection.Add("False", Boolean.FalseString);
			
			Assert.IsTrue(collection.GetBoolean("True"));
			Assert.IsFalse(collection.GetBoolean("False"));
			Assert.IsFalse(collection.GetBoolean("DoesNotExist"));			
		}
		
		[TestMethod]
		public void GetGuidTest()
		{
			Guid guid = Guid.NewGuid();
			NameValueCollection collection = new NameValueCollection();
			collection.Add("Guid", guid.ToString());

			Assert.AreEqual(guid, collection.GetGuid("Guid"));
			Assert.AreEqual<Guid>(guid, collection.GetGuid("Guid"));

			Assert.AreEqual(Guid.Empty, collection.GetGuid("DoesNotExist"));
			Assert.AreEqual<Guid>(Guid.Empty, collection.GetGuid("DoesNotExist"));
		}
		
		[TestMethod]
		public void GetStringTest()
		{
			string value = "String";
			NameValueCollection collection = new NameValueCollection();
			collection.Add("String", value);

			Assert.AreEqual(value, collection.GetString("String"));
			Assert.AreEqual<string>(value, collection.GetString("String"));
			Assert.IsNotNull(collection.GetString("String"));
			Assert.IsNull(collection.GetString("DoesNotExist"));
		}
		
		[TestMethod]
		public void GetStringCollectionTest()
		{
			NameValueCollection collection = new NameValueCollection();
			collection.Add("Collection", "Value1, Value2, Value3");
			
			List<string> values = collection.GetStringCollection("Collection", ',');
			Assert.AreEqual(3, values.Count);
			Assert.AreEqual<int>(3, values.Count);
			Assert.AreEqual("Value1", values[0]);
			Assert.AreEqual<string>("Value1", values[0]);
			Assert.AreEqual("Value2", values[1]);
			Assert.AreEqual<string>("Value2", values[1]);
			Assert.AreEqual("Value3", values[2]);
			Assert.AreEqual<string>("Value3", values[2]);

			List<string> doNotExistValues = collection.GetStringCollection("DoesNotExist", ',');
			Assert.AreEqual(0, doNotExistValues.Count);
			Assert.AreEqual<int>(0, doNotExistValues.Count);
		}
		
		[TestMethod]
		public void GetTimeSpan()
		{
			TimeSpan timeSpan = new TimeSpan(1, 2, 3, 4, 5);
			NameValueCollection collection = new NameValueCollection();
			collection.Add("TimeSpan", timeSpan.ToString());
			
			Assert.AreEqual(timeSpan, collection.GetTimeSpan("TimeSpan"));
			Assert.AreEqual<TimeSpan>(timeSpan, collection.GetTimeSpan("TimeSpan"));

			Assert.AreEqual(TimeSpan.Zero, collection.GetTimeSpan("DoesNotExist"));
			Assert.AreEqual<TimeSpan>(TimeSpan.Zero, collection.GetTimeSpan("DoesNotExist"));
		}
	}
}
