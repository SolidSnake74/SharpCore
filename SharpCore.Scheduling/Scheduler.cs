using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

using SharpCore.Collections.Generic;

namespace SharpCore.Scheduling
{
	/// <summary>
	/// Provides job scheduling functionality for an application.
	/// </summary>
	public static class Scheduler
	{
		/// <summary>
		/// Represents the list of configured jobs.
		/// </summary>
		private static Dictionary<string, JobBase> jobs;

		/// <summary>
		/// The timer used to periodically check for jobs scheduled for execution.
		/// </summary>
		private static Timer timer;

		/// <summary>
		/// Provides a placeholder for data between the scheduler and the threads that it starts.
		/// </summary>
		private static SynchronizedQueue<JobBase> queue;

		/// <summary>
		/// Represents the time interval in which the scheduler should periodically check so see if jobs are ready to be executed.
		/// </summary>
		private static TimeSpan interval;

		/// <summary>
		/// Initializes the members of the Scheduler class and starts the execution timer.
		/// </summary>
		static Scheduler()
		{
			SchedulingSectionHandler config = (SchedulingSectionHandler) ConfigurationManager.GetSection("sharpCore/scheduling");
			jobs = config.Jobs;
			interval = config.Interval;

			queue = new SynchronizedQueue<JobBase>();
		}

		/// <summary>
		/// Starts the scheduler.
		/// </summary>
		public static void Start()
		{
			timer = new Timer(new TimerCallback(TimerCallback), null, interval, interval);
		}

		/// <summary>
		/// Stops the scheduler.
		/// </summary>
		public static void Stop()
		{
			timer.Dispose();
		}

		/// <summary>
		/// Handles the callbacks from the Timer class.
		/// Checks to see if any scheduled jobs should be executed, queues them up, and starts a new thread for them to be executed on.
		/// </summary>
		/// <param name="state">An object containing application-specific information relevant to the method invoked by this delegate, or a null reference.</param>
		private static void TimerCallback(object state)
		{
			DateTime currentDate = DateTime.Now;

			foreach (JobBase job in jobs.Values)
			{
				// Is this a monthly job?
				if (job.Day == currentDate.Day && job.LastExecuted.Month != currentDate.Month)
				{
					if (job.Time > TimeSpan.Zero)
					{
						if (job.Time > currentDate.TimeOfDay)
						{
							ExecuteJob(job);
						}
					}
					else
					{
						ExecuteJob(job);
					}
				}
				// Is this a daily job?
				else if (job.Time > TimeSpan.Zero && job.LastExecuted.Day != currentDate.Day)
				{
					if (job.Time > currentDate.TimeOfDay)
					{
						ExecuteJob(job);
					}
				}
				// Is this a job that executes on an interval?
				else if (job.Interval > TimeSpan.Zero)
				{
					if (currentDate.Subtract(job.LastExecuted) > job.Interval)
					{
						ExecuteJob(job);
					}
				}
			}
		}

		/// <summary>
		/// Represents the list of configured jobs.
		/// </summary>
		public static Dictionary<string, JobBase> Jobs
		{
			get { return jobs; }
		}

		/// <summary>
		/// Queues a job to be executed on a new thread.
		/// </summary>
		/// <param name="job">The job to be executed.</param>
		private static void QueueJob(JobBase job)
		{
			if (queue.Contains(job) == false)
			{
				queue.Enqueue(job);

				Thread thread = new Thread(new ThreadStart(ExecuteJob));
				thread.IsBackground = true;
				thread.Start();
			}
		}

		/// <summary>
		/// Executes a job on demand, rather than waiting for its regularly scheduled time.
		/// </summary>
		/// <param name="job">The job to be executed.</param>
		public static void ExecuteJob(JobBase job)
		{
			ReaderWriterLock rwLock = new ReaderWriterLock();
			try
			{
				rwLock.AcquireReaderLock(Timeout.Infinite);
				if (job.Executing == false)
				{
					LockCookie lockCookie = rwLock.UpgradeToWriterLock(Timeout.Infinite);
					try
					{
						if (job.Executing == false)
						{
							job.Executing = true;
							QueueJob(job);
						}
					}
					finally
					{
						rwLock.DowngradeFromWriterLock(ref lockCookie);
					}
				}
			}
			finally
			{
				rwLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// Executes a job on demand, rather than waiting for its regularly scheduled time.
		/// </summary>
		/// <param name="name">The name of the job to be executed.</param>
		public static void ExecuteJob(string name)
		{
			JobBase job = (JobBase) jobs[name];
			ExecuteJob(job);
		}

		/// <summary>
		/// Executes the most recently queued job on a seperate thread and notifies all registered triggers when the job has completed.
		/// </summary>
		private static void ExecuteJob()
		{
			JobBase job = (JobBase) queue.Dequeue();
			try
			{
				job.Execute();
				job.LastExecuted = DateTime.Now;
				job.Executing = false;

				foreach (ISubscriber subscriber in job.Subscribers)
				{
					subscriber.OnComplete(job);
				}
			}
			catch (Exception e)
			{
				job.Executing = false;

				foreach (ISubscriber subscriber in job.Subscribers)
				{
					subscriber.OnError(job, e);
				}
			}
		}
	}
}
