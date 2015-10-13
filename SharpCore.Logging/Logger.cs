using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

using SharpCore.Logging.Loggers;

namespace SharpCore.Logging
{
	/// <summary>
	/// Provides logging functionality for an application.
	/// </summary>
	public static class Logger
	{
		/// <summary>
		/// The collection of configured loggers to use.
		/// </summary>
		private static List<LoggerBase> loggers;

		/// <summary>
		/// The configured level at which loggers can log events.
		/// </summary>
		private static LoggingLevel loggingLevel;

		/// <summary>
		/// Initializes the static members of the Logger class.
		/// </summary>
		static Logger()
		{
			LoggingSectionHandler sectionHandler = (LoggingSectionHandler) ConfigurationManager.GetSection("sharpCore/logging");
			if (sectionHandler == null)
			{
				throw new TypeInitializationException(typeof(Logger).FullName, new ConfigurationErrorsException("The sharpCore/logging configuration section has not been defined."));
			}
			else
			{
				loggingLevel = sectionHandler.LoggingLevel;
				loggers = sectionHandler.Loggers;
			}
		}

		/// <summary>
		/// The configured level at which this logger can log events.
		/// </summary>
		public static LoggingLevel LoggingLevel
		{
			get { return loggingLevel; }
		}

		/// <summary>
		/// Logs an entry to all logs configured for the specified <see cref="SharpCore.Logging.LoggingLevel"/>.
		/// </summary>
		/// <param name="source">The source of the entry to be logged.</param>
		/// <param name="message">The message of the entry to be logged.</param>
		/// <param name="loggingLevel">The logging level to be used when writing to all configured logs.</param>
		public static void LogEntry(string source, string message, LoggingLevel loggingLevel)
		{
			if (Logger.LoggingLevel > LoggingLevel.Off)
			{
				foreach (LoggerBase logger in loggers)
				{
					if (Logger.LoggingLevel > loggingLevel && logger.LoggingLevel != LoggingLevel.Off && logger.LoggingLevel >= loggingLevel)
					{
						logger.LogEntry(source, message, loggingLevel);
					}
				}
			}
		}
	}
}
