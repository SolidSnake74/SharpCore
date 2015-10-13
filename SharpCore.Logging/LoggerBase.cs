using System;
using System.Configuration;
using System.IO;
using System.Xml;

namespace SharpCore.Logging
{
	/// <summary>
	/// Defines the methods and properties shared by all logger classes.
	/// </summary>
	public abstract class LoggerBase
	{
		/// <summary>
		/// The configured level at which this logger can log events.
		/// </summary>
		protected LoggingLevel loggingLevel;

		/// <summary>
		/// Initializes a new instance of the LoggerBase class.
		/// </summary>
		public LoggerBase()
		{
			loggingLevel = LoggingLevel.Off;
		}

		/// <summary>
		/// The configured level at which this logger can log events.
		/// </summary>
		public virtual LoggingLevel LoggingLevel
		{
			get { return loggingLevel; }
		}

		/// <summary>
		/// Reads the configuration and configures the logger instance.
		/// </summary>
		/// <param name="element">An <b>XmlElement</b> that contains the configuration information for the logger instance.</param>
		public virtual void Configure(XmlElement element)
		{
			string attributeValue = GetAttributeValue(element, "loggingLevel");

			if (attributeValue.Length > 0)
			{
				loggingLevel = (LoggingLevel) Enum.Parse(typeof(LoggingLevel), attributeValue, true);
			}
			else
			{
				loggingLevel = LoggingLevel.Off;
			}
		}

		/// <summary>
		/// Logs an entry to all logs configured for the specified <see cref="SharpCore.Logging.LoggingLevel"/>.
		/// </summary>
		/// <param name="source">The source of the entry to be logged.</param>
		/// <param name="message">The message of the entry to be logged.</param>
		/// <param name="loggingLevel"></param>
		public abstract void LogEntry(string source, string message, LoggingLevel loggingLevel);

		/// <summary>
		/// Reads the value of the specified attribute from the specified element.
		/// </summary>
		/// <param name="element">The <see cref="System.Xml.XmlElement"/> that contains the attribute to be read.</param>
		/// <param name="name">The name of the attribute that the value should be retrived for.</param>
		/// <returns>The value of the specified attribute for the specified element.</returns>
		protected string GetAttributeValue(XmlElement element, string name)
		{
			return GetAttributeValue(element, name, "", false);
		}

		/// <summary>
		/// Reads the value of the specified attribute from the specified element.
		/// </summary>
		/// <param name="element">The <see cref="System.Xml.XmlElement"/> that contains the attribute to be read.</param>
		/// <param name="name">The name of the attribute that the value should be retrived for.</param>
		/// <param name="defaultValue">The default value to be used if the atribute cannot be found on the element.</param>
		/// <returns>The value of the specified attribute for the specified element.</returns>
		protected string GetAttributeValue(XmlElement element, string name, string defaultValue)
		{
			string returnValue = GetAttributeValue(element, name, "", false);

			if (returnValue.Length > 0)
			{
				return returnValue;
			}
			else
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Reads the value of the specified attribute from the specified element.
		/// </summary>
		/// <param name="element">The <see cref="System.Xml.XmlElement"/> that contains the attribute to be read.</param>
		/// <param name="attributeName">The name of the attribute that the value should be retrived for.</param>
		/// <param name="loggerName">The name of the logger associated with the element/attribute pair.</param>
		/// <param name="generateException">Indicates if an exception should be thrown if the attribute cannot be found on the element.</param>
		/// <returns>The value of the specified attribute for the specified element.</returns>
		protected string GetAttributeValue(XmlElement element, string attributeName, string loggerName, bool generateException)
		{
			if (element.HasAttribute(attributeName))
			{
				return element.GetAttribute(attributeName);
			}
			else if (generateException)
			{
				StringWriter writer = new StringWriter();
				writer.WriteLine("The " + attributeName + " attribute is required in a logger element representing a " + loggerName + " configuration");
				writer.WriteLine();
				writer.WriteLine(element.OuterXml);

				throw new ConfigurationErrorsException(writer.ToString());
			}
			else
			{
				return "";
			}
		}
	}
}
