using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpCore.Logging;

namespace SharpCore.Logging.Testing
{
	/// <summary>
	/// Verifies the functionality found in SharpCore.Logging.
	/// </summary>
	[TestClass]
	public class UnitTest
	{
		[TestMethod]
		public void LoggingUnitTestMethod()
		{
			Logger.LogEntry("SharpCore.Logging.Testing.UnitTest", "Test Message", LoggingLevel.Information);
		}
	}
}
