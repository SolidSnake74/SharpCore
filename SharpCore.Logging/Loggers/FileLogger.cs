using System;
using System.IO;
using System.Xml;

using SharpCore.Logging;

namespace SharpCore.Logging.Loggers
{
	/// <summary>
	/// Provides functionality for logging to a file.
	/// </summary>
	internal sealed class FileLogger : LoggerBase
	{
		private string fileName;
		private string format;

		public FileLogger() : base()
		{
		}

		public override void LogEntry(string source, string message, SharpCore.Logging.LoggingLevel loggingLevel)
		{
			using (StreamWriter writer = new StreamWriter(fileName, true))
			{
				string entry = format;
				entry = entry.Replace("%source", source);
				entry = entry.Replace("%message", message);
				entry = entry.Replace("%loggingLevel", loggingLevel.ToString());
				entry = entry.Replace("%datetime", DateTime.Now.ToString());
				entry = entry.Replace("%newline", Environment.NewLine);
				entry = entry.Replace("%tab", "\t");

				writer.Write(entry);
			}
		}

		public override void Configure(XmlElement element)
		{
			base.Configure(element);

			format = GetAttributeValue(element, "format", "%source %loggingLevel %datetime %message%newline");
			fileName = GetAttributeValue(element, "fileName", "", true);
		}
	}
}
