using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SharpCore.Data
{
    /// <summary>
    /// Provides logging functionality for SharpCore.Data class.
    /// </summary>
    internal static class TraceLog
    {
        private static string format = " %datetime %source %message%newline"; 

        /// <summary>
        /// Initializes the static members of the Logger class.
        /// </summary>
        static TraceLog()
        {          
        }

        /// <summary>
        /// Logs an entry to all logs configured for the specified <see cref="SharpCore.Logging.LoggingLevel"/>.
        /// </summary>
        /// <param name="source">The source of the entry to be logged.</param>
        /// <param name="message">The message of the entry to be logged.</param>
        /// <param name="loggingLevel">The logging level to be used when writing to all configured logs.</param>
        internal static void LogEntry(object source, string message)//, LoggingLevel loggingLevel)
        {
            //if (Logger.LoggingLevel > LoggingLevel.Off)
            {
                string entry = format;
                entry = entry.Replace("%datetime", DateTime.Now.ToString());
                entry = entry.Replace("%source", source.GetType().FullName);
                entry = entry.Replace("%message", message);
                entry = entry.Replace("%newline", Environment.NewLine);
                entry = entry.Replace("%tab", "\t");
                Trace.Write(entry);
            }
        }

         /// <summary>
        /// Logs an entry to all logs configured for the specified <see cref="SharpCore.Logging.LoggingLevel"/>.
        /// </summary>
        /// <param name="source">The source of the entry to be logged.</param>
        /// <param name="message">The message of the entry to be logged.</param>
        /// <param name="loggingLevel">The logging level to be used when writing to all configured logs.</param>
        internal static void LogEntry(string source, string message)
        {
            string entry = format;
            entry = entry.Replace("%datetime", DateTime.Now.ToString());
            entry = entry.Replace("%source", source);
            entry = entry.Replace("%message", message);                               
            entry = entry.Replace("%newline", Environment.NewLine);
            entry = entry.Replace("%tab", "\t");
            Trace.Write(entry);
        }

        internal static void LogEntry(string message, params object[] args)
        {
            LogEntry(string.Empty, String.Format(message, args));
        }

        //internal static void LogEntry(string message, params object[] args)
        //{
        //    LogEntry(string.Empty, String.Format(message, args));                            
        //}

    }
}
