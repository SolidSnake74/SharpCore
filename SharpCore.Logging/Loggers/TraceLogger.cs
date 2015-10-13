using System;
using System.Diagnostics;
using System.Xml;

using SharpCore.Logging;

namespace SharpCore.Logging.Loggers
{
	/// <summary>
	/// Provides functionality for logging to all configured trace listeners.
	/// </summary>
	internal sealed class TraceLogger : LoggerBase
	{
		private string format;

		public TraceLogger() : base() { }

		public override void LogEntry(string source, string message, LoggingLevel loggingLevel)
		{
			string entry = format;
			entry = entry.Replace("%source", source);
			entry = entry.Replace("%message", message);
			entry = entry.Replace("%loggingLevel", loggingLevel.ToString());
			entry = entry.Replace("%datetime", DateTime.Now.ToString());
			entry = entry.Replace("%newline", Environment.NewLine);
			entry = entry.Replace("%tab", "\t");

			Trace.Write(entry);
		}

		public override void Configure(XmlElement element)
		{
			base.Configure(element);

			format = GetAttributeValue(element, "format", "%source %loggingLevel %datetime %message%newline");
		}
	}
}
