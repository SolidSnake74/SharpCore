using System;
using System.Messaging;
using System.Net.Mail;
using System.Xml;

using SharpCore.Logging;

namespace SharpCore.Logging.Loggers
{
	/// <summary>
	/// Provides functionality for logging to an SMTP recipient through emails.
	/// </summary>
	internal sealed class SmtpLogger : LoggerBase
	{
		private string host;
		private string from;
		private string to;

		public SmtpLogger() : base() { }

		public override void LogEntry(string source, string message, SharpCore.Logging.LoggingLevel loggingLevel)
		{
			MailMessage mailMessage = new MailMessage(from, to);
			mailMessage.Subject = source + " (" + loggingLevel.ToString() + ")";
			mailMessage.Body = message;

			SmtpClient smtpClient = new SmtpClient(host);
			smtpClient.Send(mailMessage);
		}

		public override void Configure(XmlElement element)
		{
			base.Configure(element);

			host = GetAttributeValue(element, "host", "EmailLogger", true);
			from = GetAttributeValue(element, "from", "EmailLogger", true);
			to = GetAttributeValue(element, "to", "EmailLogger", true);
		}

	}
}
