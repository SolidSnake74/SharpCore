using System;
using System.Web;
using System.Xml;

using SharpCore.Logging;

namespace SharpCore.Logging.Loggers
{
	/// <summary>
	/// Provides functionality for logging to all configured ASP.NET trace listeners.
	/// </summary>
	internal sealed class AspNetTraceLogger : LoggerBase
	{
		private string format;

		public AspNetTraceLogger() : base()
		{
		}

		public override void LogEntry(string source, string message, LoggingLevel loggingLevel)
		{
			string entry = format;
			entry = entry.Replace("%source", source);
			entry = entry.Replace("%message", message);
			entry = entry.Replace("%loggingLevel", loggingLevel.ToString());
			entry = entry.Replace("%datetime", DateTime.Now.ToString());
			entry = entry.Replace("%newline", Environment.NewLine);
			entry = entry.Replace("%tab", "\t");

			if (loggingLevel == LoggingLevel.Error || loggingLevel == LoggingLevel.Warning)
			{
				HttpContext.Current.Trace.Warn(entry);
			}
			else
			{
				HttpContext.Current.Trace.Write(entry);
			}
		}

		public override void Configure(XmlElement element)
		{
			base.Configure(element);

			format = GetAttributeValue(element, "format", "%source %loggingLevel %datetime %message");
		}
	}
}
