using System;
using System.IO;
using System.Xml;

using SharpCore.Logging;

namespace SharpCore.Logging.Loggers
{
	/// <summary>
	/// Provides functionality for logging to the console.
	/// </summary>
	internal sealed class ConsoleLogger : LoggerBase
	{
		private string format;

		public ConsoleLogger() : base()
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

			Console.WriteLine(entry);
		}

		public override void Configure(XmlElement element)
		{
			base.Configure(element);

			format = GetAttributeValue(element, "format", "%source %loggingLevel %datetime %message");
		}
	}
}
