using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Xml;

namespace SharpCore.Logging
{
	/// <summary>
	/// Provides logging configuration information from a configuration section.
	/// </summary>
	internal sealed class LoggingSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Represents the list of configured loggers.
		/// </summary>
		private List<LoggerBase> loggers;

		/// <summary>
		/// The configured level at which loggers can log events.
		/// </summary>
		private LoggingLevel loggingLevel;

		/// <summary>
		/// Initializes a new instance of the LoggingSectionHandler class.
		/// </summary>
		public LoggingSectionHandler()
		{
			loggers = new List<LoggerBase>();
		}

		/// <summary>
		/// Parses the XML of the configuration section.
		/// </summary>
		/// <param name="parent">The configuration settings in a corresponding parent configuration section.</param>
		/// <param name="configContext">An HttpConfigurationContext when Create is called from the ASP.NET configuration system. Otherwise, this parameter is reserved and is a null reference.</param>
		/// <param name="section">The XmlNode that contains the configuration information from the configuration file. Provides direct access to the XML contents of the configuration section.</param>
		/// <returns>A configuration object.</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			XmlElement loggingElement = (XmlElement) section;
			if (loggingElement.HasAttribute("LoggingLevel"))
			{
				loggingLevel = (LoggingLevel) Enum.Parse(typeof(LoggingLevel), loggingElement.GetAttribute("LoggingLevel"), true);
			}
			else
			{
				loggingLevel = LoggingLevel.Off;
			}

			if (loggingElement.HasChildNodes)
			{
				foreach (XmlElement element in loggingElement.SelectNodes("logger"))
				{
					string typeName = element.GetAttribute("type");

					// Use reflection to create an instance of the configured Logger instance
					Type type = Type.GetType(typeName);
					LoggerBase logger = (LoggerBase) type.Assembly.CreateInstance(type.FullName);
					logger.Configure(element);
					loggers.Add(logger);
				}
			}

			return this;
		}

		/// <summary>
		/// The configured level at which this logger can log events.
		/// </summary>
		public LoggingLevel LoggingLevel
		{
			get { return loggingLevel; }
		}

		/// <summary>
		/// Returns the list of configured logger instances.
		/// </summary>
		public List<LoggerBase> Loggers
		{
			get { return loggers; }
		}
	}
}
