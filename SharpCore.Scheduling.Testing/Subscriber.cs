using System;

using SharpCore.Scheduling;

namespace SharpCore.Scheduling.Testing
{
	/// <summary>
	/// Example class that implements the ISubscriber interface.
	/// </summary>
	public sealed class Subscriber : ISubscriber
	{
		private bool isComplete = false;
		private bool encounteredError = false;
		private bool isDone = false;

		/// <summary>
		/// Gets a value indicating if the job has executed yet.
		/// </summary>
		public bool IsComplete
		{
			get { return isComplete; }
		}

		/// <summary>
		/// Gets a value indicating if the job encountered an error during execution.
		/// </summary>
		public bool EncounteredError
		{
			get { return encounteredError; }
		}

		/// <summary>
		/// Gets a value indicating if the job executed, regardless of errors.
		/// </summary>
		public bool IsDone
		{
			get { return isDone; }
		}

		#region ISubscriber Methods
		/// <summary>
		/// Method to be called after the job executes successfully.
		/// </summary>
		/// <param name="job">The job that was executed.</param>
		public void OnComplete(JobBase job)
		{
			isComplete = true;
			isDone = true;
		}

		/// <summary>
		/// Method to be called after an execption occurs during job execution.
		/// </summary>
		/// <param name="job">The job that was executed.</param>
		/// <param name="e">The exception that occurred.</param>
		public void OnError(JobBase job, Exception e)
		{
			encounteredError = true;
			isDone = true;
		}
		#endregion
	}
}
