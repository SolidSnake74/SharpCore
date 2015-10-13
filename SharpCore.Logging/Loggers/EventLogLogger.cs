using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

using SharpCore.Logging;

namespace SharpCore.Logging.Loggers
{
	/// <summary>
	/// Provides functionality for logging to the Windows Event Log.
	/// </summary>
	internal sealed class EventLogLogger : LoggerBase
	{
		private string logName;

		public EventLogLogger() : base()
		{
		}

		public override void LogEntry(string source, string message, LoggingLevel loggingLevel)
		{
			if (EventLog.Exists(logName))
			{
				StringWriter writer = new StringWriter();
				writer.WriteLine("Source: " + source);
				writer.WriteLine("Message: " + message);
				message = writer.ToString();

				switch (loggingLevel)
				{
					case LoggingLevel.Error:
						EventLog.WriteEntry(logName, message, EventLogEntryType.Error);
						break;
					case LoggingLevel.Warning:
						EventLog.WriteEntry(logName, message, EventLogEntryType.Warning);
						break;
					case LoggingLevel.Information:
					case LoggingLevel.Verbose:
						EventLog.WriteEntry(logName, message, EventLogEntryType.Information);
						break;
					default:
						break;
				}
			}
			else
			{
				StringWriter writer = new StringWriter();
				writer.WriteLine("Entries cannot be written to the " + logName + " log because it does not exist.");
				writer.WriteLine("Source: " + source);
				writer.WriteLine("Message: " + message);

				throw new InvalidOperationException(writer.ToString());
			}
		}

		public override void Configure(XmlElement element)
		{
			base.Configure(element);

			logName = GetAttributeValue(element, "logName", "EventLogLogger.", true);
		}
	}
}
