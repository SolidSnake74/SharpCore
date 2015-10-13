using System;

namespace SharpCore.Scheduling
{
	/// <summary>
	/// Defines the functionality for all trigger classes, which are registered to be notified when a job completes.
	/// </summary>
	public abstract class TriggerBase
	{
		/// <summary>
		/// When overriden in a derived class, provides an entry point for the Scheduler to notify the trigger that a job has completed.
		/// </summary>
		/// <param name="job">The job that has completed.</param>
		/// <param name="e">The exception that was thrown during the job execution, if any; otherwise, <code>null</code>.</param>
		public abstract void JobCompleted(JobBase job, Exception e);
	}
}
