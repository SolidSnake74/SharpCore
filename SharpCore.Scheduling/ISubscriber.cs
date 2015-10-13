using System;

namespace SharpCore.Scheduling
{
	/// <summary>
	/// Defines the functionality for all subscribers, which are registered to be notified when a job completes.
	/// </summary>
	public interface ISubscriber
	{
		/// <summary>
		/// Provides an entry point for the Scheduler to notify the subscriber that a job has completed.
		/// </summary>
		/// <param name="job">The job that completed.</param>
		void OnComplete(JobBase job);

		/// <summary>
		/// Provides an entry point for the Scheduler to notify the subscriber that a job has failed.
		/// </summary>
		/// <param name="job">The job that failed.</param>
		/// <param name="e">The exception that was thrown during the job execution.</param>
		void OnError(JobBase job, Exception e);
	}
}
