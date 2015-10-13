using System;
using System.Configuration;
using System.IO;
using System.Xml;

using SharpCore.Logging;

namespace SharpCore.Logging.Loggers
{
	/// <summary>
	/// Provides functionality for logging to a file.  New files are created on daily basis.
	/// </summary>
	internal sealed class RollingFileLogger : LoggerBase
	{
		private string fileName;
		private string currentFileName;
		private string format;
		private DateTime dateTime;

		public RollingFileLogger()
			: base()
		{
			dateTime = DateTime.Now.Date;
		}

		public override void LogEntry(string source, string message, LoggingLevel loggingLevel)
		{
			if (dateTime != DateTime.Now.Date)
			{
				dateTime = DateTime.Now.Date;
				currentFileName = GetNewFileName();
			}

			using (StreamWriter writer = new StreamWriter(currentFileName, true))
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
			fileName = GetAttributeValue(element, "fileName", "RollingFileLogger", true);

			if (fileName.IndexOf("{0}") >= 0)
			{
				currentFileName = GetNewFileName();
			}
			else
			{
				StringWriter writer = new StringWriter();
				writer.WriteLine("The fileName attribute of the RollingFileLogger configuration must contain a placeholder for the date (example: MyLog {0}.log).");
				writer.WriteLine();
				writer.WriteLine(element.OuterXml);

				throw new ConfigurationErrorsException(writer.ToString());
			}
		}

		private string GetNewFileName()
		{
			return String.Format(fileName, dateTime.Date.ToShortDateString().Replace("/", "-"));
		}
	}
}
