using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

using SharpCore.Data;
using SharpCore.Logging;

namespace SharpCore.Logging.Loggers
{
	/// <summary>
	/// Provides functionality for logging to a SQL Server database.
	/// </summary>
	internal sealed class SqlLogger : LoggerBase
	{
		private string connectionStringName;
		private string commandText;

		public SqlLogger() : base()
		{
		}

		public override void LogEntry(string source, string message, LoggingLevel loggingLevel)
		{
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Source", source),
				new SqlParameter("@Message", message),
				new SqlParameter("@LoggingLevel", loggingLevel)
			};
			
			SqlClientUtility.ExecuteNonQuery(connectionStringName, CommandType.StoredProcedure, commandText, parameters);
		}

		public override void Configure(System.Xml.XmlElement element)
		{
			base.Configure(element);

			connectionStringName = GetAttributeValue(element, "connectionStringName", "SqlLogger", true);
			commandText = GetAttributeValue(element, "insertSql", "SqlLogger", true);
		}

	}
}
