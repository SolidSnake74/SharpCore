using System;
using System.IO;
using System.Messaging;

using SharpCore.Logging;

namespace SharpCore.Logging.Loggers
{
	/// <summary>
	/// Provides functionality for logging to message queue.
	/// </summary>
	internal sealed class MessageQueueLogger : LoggerBase
	{
		private string path;

		public MessageQueueLogger() : base()
		{
		}

		public override void LogEntry(string source, string message, SharpCore.Logging.LoggingLevel loggingLevel)
		{
			StringWriter writer = new StringWriter();
			writer.WriteLine("Source: " + source);
			writer.WriteLine("Message: " + message);
			writer.WriteLine("Log Level: " + loggingLevel.ToString());

			Message msg = new Message(writer.ToString());

			using (MessageQueue queue = new MessageQueue(path))
			{
				queue.Send(msg);
			}
		}

		public override void Configure(System.Xml.XmlElement element)
		{
			base.Configure(element);

			path = GetAttributeValue(element, "path", "MessageQueueLogger", true);
		}
	}
}
