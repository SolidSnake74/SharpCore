using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpCore.Objects;

namespace SharpCore.Objects.Testing
{
	/// <summary>
	/// Provides unit tests for the SharpCore.Objects functionality.
	/// </summary>
	[TestClass]
	public class UnitTest
	{
		[TestMethod]
		public void SimpleObjectUnitTest()
		{
			SimpleObject simpleObject = (SimpleObject) ObjectFactory.GetObject("simpleObject");

			Assert.IsNotNull(simpleObject);
			Assert.IsInstanceOfType(simpleObject, typeof(SimpleObject));
		}

		[TestMethod]
		public void SimpleObjectWithPropertyUnitTest()
		{
			SimpleObject simpleObject = (SimpleObject) ObjectFactory.GetObject("simpleObjectWithProperty");

			Assert.IsNotNull(simpleObject);
			Assert.IsInstanceOfType(simpleObject, typeof(SimpleObject));

			Assert.IsTrue(simpleObject.Name.Length > 0);
		}

		[TestMethod]
		public void SimpleObjectWithConstructorUnitTest()
		{
			SimpleObject simpleObject = (SimpleObject) ObjectFactory.GetObject("simpleObjectWithConstructor");

			Assert.IsNotNull(simpleObject);
			Assert.IsInstanceOfType(simpleObject, typeof(SimpleObject));

			Assert.IsTrue(simpleObject.Name.Length > 0);
		}

		[TestMethod]
		public void ComplextObjectUnitTest()
		{
			ComplexObject complexObject = (ComplexObject) ObjectFactory.GetObject("complexObject");

			Assert.IsNotNull(complexObject);
			Assert.IsInstanceOfType(complexObject, typeof(ComplexObject));
		}

		[TestMethod]
		public void ComplexObjectWithPropertyUnitTest()
		{
			ComplexObject complexObject = (ComplexObject) ObjectFactory.GetObject("complexObjectWithProperty");

			Assert.IsNotNull(complexObject);
			Assert.IsInstanceOfType(complexObject, typeof(ComplexObject));

			Assert.IsTrue(complexObject.Name.Length > 0);

			Assert.IsNotNull(complexObject.SimpleObject);
			Assert.IsInstanceOfType(complexObject.SimpleObject, typeof(SimpleObject));
		}

		[TestMethod]
		public void ComplexObjectWithConstructorUnitTest()
		{
			ComplexObject complexObject = (ComplexObject) ObjectFactory.GetObject("complexObjectWithConstructor");

			Assert.IsNotNull(complexObject);
			Assert.IsInstanceOfType(complexObject, typeof(ComplexObject));

			Assert.IsTrue(complexObject.Name.Length > 0);

			Assert.IsNotNull(complexObject.SimpleObject);
			Assert.IsInstanceOfType(complexObject.SimpleObject, typeof(SimpleObject));
		}

		[TestMethod]
		public void SingletonUnitTest()
		{
			SimpleObject simpleObject = (SimpleObject) ObjectFactory.GetObject("singleton");

			Assert.IsNotNull(simpleObject);
			Assert.IsInstanceOfType(simpleObject, typeof(SimpleObject));

			Assert.AreEqual(simpleObject, ObjectFactory.GetObject("singleton"));
			Assert.AreSame(simpleObject, ObjectFactory.GetObject("singleton"));
		}

		[TestMethod]
		public void NonSingletonUnitTest()
		{
			SimpleObject simpleObject = (SimpleObject) ObjectFactory.GetObject("nonSingleton");

			Assert.IsNotNull(simpleObject);
			Assert.IsInstanceOfType(simpleObject, typeof(SimpleObject));

			Assert.AreNotEqual(simpleObject, ObjectFactory.GetObject("nonSingleton"));
			Assert.AreNotSame(simpleObject, ObjectFactory.GetObject("nonSingleton"));
		}
	}
}
