using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Xml;

namespace SharpCore.Scheduling
{
	/// <summary>
	/// Defines the functionality shared between all scheduled job classes.
	/// </summary>
	public abstract class JobBase
	{
		/// <summary>
		/// The name of the job.
		/// </summary>
		protected string name;

		/// <summary>
		/// The description of the job.
		/// </summary>
		protected string description;

		/// <summary>
		/// The list of subscribers that should be notified when this job completes.
		/// </summary>
		protected List<ISubscriber> subscribers;

		/// <summary>
		/// The date and time the job was last run.
		/// </summary>
		protected DateTime lastExecuted;

		/// <summary>
		/// The time of day that the scheduler should execute the job.
		/// </summary>
		protected TimeSpan time;

		/// <summary>
		/// The time interval in which the scheduler should periodically check so see if this job is ready to be executed.
		/// </summary>
		protected TimeSpan interval;

		/// <summary>
		/// The day of the month that the job should execute on
		/// </summary>
		protected int day;

		/// <summary>
		/// Indicates if this job is currently executing.
		/// </summary>
		protected bool executing;

		/// <summary>
		/// Initializes a new instance of the JobBase class.
		/// </summary>
		public JobBase()
		{
			this.subscribers = new List<ISubscriber>();
			this.lastExecuted = DateTime.Now;
			this.interval = TimeSpan.Zero;
			this.time = TimeSpan.Zero;
			this.day = 0;
		}

		/// <summary>
		/// Reads the configuration and configures the job instance.
		/// </summary>
		/// <param name="section">An <b>XmlElement</b> that contains the configuration information for the job instance.</param>
		public virtual void Configure(XmlElement section)
		{
			// Configure the name property
			if (section.HasAttribute("name"))
			{
				name = section.GetAttribute("name");
			}
			else
			{
				throw BuildConfigurationErrorsException("The name attribute is required for a job element.", section.OuterXml);
			}

			// Configure the description property
			if (section.HasAttribute("description"))
			{
				description = section.GetAttribute("description");
			}

			// Configure the time property
			if (section.HasAttribute("time"))
			{
				this.time = TimeSpan.Parse(section.GetAttribute("time"));
			}

			// Configure the interval property
			if (section.HasAttribute("interval"))
			{
				this.interval = TimeSpan.Parse(section.GetAttribute("interval"));
			}

			// Configure the day property
			if (section.HasAttribute("day"))
			{
				this.day = Int32.Parse(section.GetAttribute("day"));
			}

			// Validate the schedule information; a day, interval, or time is required
			if (day == 0 && interval == TimeSpan.Zero && time == TimeSpan.Zero)
			{
				throw BuildConfigurationErrorsException("The either the day, interval, or time attribute must be specified for a job element.", section.OuterXml);
			}

			// Configure any specified subscribers
			foreach (XmlElement element in section.SelectNodes("subscribers/subscriber"))
			{
				string typeName = element.GetAttribute("type");

				// Use reflection to create an instance of the configured subscriber
				Type type = Type.GetType(typeName);
				ISubscriber subscriber = (ISubscriber) type.Assembly.CreateInstance(type.FullName);
				subscribers.Add(subscriber);
			}
		}

		/// <summary>
		/// When overridden in a derived class, executes the scheduled job.
		/// </summary>
		public abstract void Execute();

		/// <summary>
		/// The name of the job.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// The description of the job.
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		/// <summary>
		/// Indicates if this job is currently executing.
		/// </summary>
		public bool Executing
		{
			get { return executing; }
			set { executing = value; }
		}

		/// <summary>
		/// The list of subscribers that should be notified when this job completes.
		/// </summary>
		public List<ISubscriber> Subscribers
		{
			get { return subscribers; }
		}

		/// <summary>
		/// The time interval in which the scheduler should periodically check so see if this job is ready to be executed.
		/// </summary>
		public TimeSpan Interval
		{
			get { return interval; }
		}

		/// <summary>
		/// The time of day that the scheduler should execute the job.
		/// </summary>
		public TimeSpan Time
		{
			get { return time; }
		}

		/// <summary>
		/// The day of the month that the job should execute on.
		/// </summary>
		public int Day
		{
			get { return day; }
		}

		/// <summary>
		/// The date and time the job was last run.
		/// </summary>
		public DateTime LastExecuted
		{
			get { return lastExecuted; }
			set { lastExecuted = value; }
		}

		/// <summary>
		/// Creates a ConfigurationErrorsException representing incorrect/invalid configuration XML.
		/// </summary>
		/// <param name="message">The message indicating the configuration error.</param>
		/// <param name="xml">The XML that contains the incorrect/invalid configuration information.</param>
		/// <returns>An ConfigurationErrorsException containing the error message and the incorrect/invalid configuration XML.</returns>
		protected ConfigurationErrorsException BuildConfigurationErrorsException(string message, string xml)
		{
			StringBuilder builder = new StringBuilder(128);
			builder.Append(message);
			builder.Append(Environment.NewLine);
			builder.Append(Environment.NewLine);
			builder.Append(xml);

			return new ConfigurationErrorsException(builder.ToString());
		}
	}
}
