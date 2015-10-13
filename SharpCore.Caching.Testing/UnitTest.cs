using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpCore.Caching;

namespace SharpCore.Caching.Testing
{
	/// <summary>
	/// Verifies the functionality found in SharpCore.Caching.
	/// </summary>
	[TestClass]
	public class UnitTest
	{
		[TestMethod]
		public void CachingUnitTest()
		{
			Cache.Add("UnitTest", "Key", null);

			object value = Cache.Get("UnitTest", "Key");
			Assert.IsNull(value);

			Cache.Update("UnitTest", "Key", new Uri("http://www.sourceforge.net"));
			Uri uri = (Uri) Cache.Get("UnitTest", "Key");
			Assert.IsNotNull(uri);
			Assert.IsTrue(uri.AbsolutePath.Length > 0);

			Cache.Remove("UnitTest", "Key");
			Assert.IsNull(Cache.Get("UnitTest", "Key"));
		}
	}
}
