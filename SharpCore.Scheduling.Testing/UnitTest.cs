using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpCore.Scheduling;

namespace SharpCore.Scheduling.Testing
{
	/// <summary>
	/// Provides unit tests for the SharpCore.Scheduling functionality.
	/// </summary>
	[TestClass]
	public sealed class UnitTest
	{
		[TestMethod]
		public void SchedulingUnitTest()
		{
			Scheduler.Start();
			try
			{
				// Get a reference to the configured subscriber
				Subscriber subscriber = (Subscriber) Scheduler.Jobs["SimpleJob"].Subscribers[0];

				// Wait for the job to execute at least once
				while (subscriber.IsDone == false)
				{
					Thread.Sleep(1);
				}

				// Check the job's execution status
				Assert.IsTrue(subscriber.IsComplete);
				Assert.IsFalse(subscriber.EncounteredError);
			}
			finally
			{
				Scheduler.Stop();
			}
		}
	}
}
