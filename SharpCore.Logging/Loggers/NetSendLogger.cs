using System;
using System.Runtime.InteropServices;
using System.Xml;

namespace SharpCore.Logging.Loggers
{
	/// <summary>
	/// Provides functionality for logging to a NET SEND recipient.
	/// </summary>
	internal class NetSendLogger : LoggerBase
	{
		private string format;
		private string recipient;

		public NetSendLogger() : base()
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

			NetMessageBufferSend(null, recipient, Environment.MachineName, entry, entry.Length * 2);
		}

		public override void Configure(XmlElement element)
		{
			base.Configure(element);

			format = GetAttributeValue(element, "format", "%source %loggingLevel %datetime %message");
			recipient = GetAttributeValue(element, "recipient", "NetSendLogger", true);
		}

		[DllImport("netapi32.dll", SetLastError = true)]
		private static extern int NetMessageBufferSend(
			[MarshalAs(UnmanagedType.LPWStr)] string serverName,
			[MarshalAs(UnmanagedType.LPWStr)] string msgName,
			[MarshalAs(UnmanagedType.LPWStr)] string fromName,
			[MarshalAs(UnmanagedType.LPWStr)] string buffer,
			int bufferSize
		);
	}
}
