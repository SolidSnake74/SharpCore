using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Xml;

namespace SharpCore.Scheduling
{
	/// <summary>
	/// Provides scheduling configuration information from a configuration section.
	/// </summary>
	internal sealed class SchedulingSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Represents the list of configured jobs.
		/// </summary>
		private Dictionary<string, JobBase> jobs;

		/// <summary>
		/// Represents the time interval in which the scheduler should periodically check so see if jobs are ready to be executed.
		/// </summary>
		private TimeSpan interval;

		/// <summary>
		/// Initializes a new instance of the SchedulingSectionHandler class.
		/// </summary>
		public SchedulingSectionHandler()
		{
			jobs = new Dictionary<string, JobBase>();
			interval = TimeSpan.FromMinutes(1);
		}

		/// <summary>
		/// Parses the XML of the configuration section.
		/// </summary>
		/// <param name="parent">The configuration settings in a corresponding parent configuration section.</param>
		/// <param name="configContext">An HttpConfigurationContext when Create is called from the ASP.NET configuration system. Otherwise, this parameter is reserved and is a null reference.</param>
		/// <param name="section">The XmlNode that contains the configuration information from the configuration file. Provides direct access to the XML contents of the configuration section.</param>
		/// <returns>A configuration object.</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			XmlElement sectionElement = (XmlElement) section;
			if (sectionElement.HasAttribute("interval"))
			{
				interval = TimeSpan.Parse(sectionElement.GetAttribute("interval"));
			}

			foreach (XmlElement element in section.ChildNodes)
			{
				string typeName = element.GetAttribute("type");

				// Use reflection to create an instance of the configured Job instance
				Type type = Type.GetType(typeName);
				JobBase job = (JobBase) type.Assembly.CreateInstance(type.FullName);
				job.Configure(element);

				// Make sure that the job is not configured to run on shorter intervals than the scheduler
				if (job.Interval.Subtract(interval) > TimeSpan.Zero)
				{
					jobs.Add(job.Name, job);
				}
				else
				{
					StringBuilder builder = new StringBuilder(128);
					builder.Append("A job interval must be less than the scheduling interval.");
					builder.Append(Environment.NewLine);
					builder.Append(Environment.NewLine);
					builder.Append(element.OuterXml);

					throw new ConfigurationErrorsException(builder.ToString());
				}
			}

			return this;
		}

		/// <summary>
		/// Returns the list of scheduled jobs.
		/// </summary>
		public Dictionary<string, JobBase> Jobs
		{
			get { return jobs; }
		}

		/// <summary>
		/// Returns the time interval in which the scheduler should periodically check so see if jobs are ready to be executed.
		/// </summary>
		public TimeSpan Interval
		{
			get { return interval; }
		}
	}
}
