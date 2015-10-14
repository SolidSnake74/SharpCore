using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Transactions;

namespace SharpCore.Data
{
	/// <summary>
	/// Provides helper functions for working with the <b>System.Data.SqlClient</b> namespace.
	/// </summary>
	public static class SqlClientUtility
	{
		/// <summary>
		/// Executes the stored procedure with the specified parameters.
		/// </summary>
		/// <param name="connectionStringName">The name of the connection string to use.</param>
		/// <param name="commandType">The command type.</param>
		/// <param name="commandText">The stored procedure to execute.</param>
		/// <param name="parameters">The parameters of the stored procedure.</param>
		public static void ExecuteNonQuery(string connectionStringName, CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            
			if (Transaction.Current == null)
			{
                TraceLog.LogEntry("ExecuteNonQuery", "Transaction.Current == null");

				string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
					{
						command.ExecuteNonQuery();
					}
				}
			}
			else
			{                
				SqlConnection connection = GetTransactedSqlConnection(connectionStringName);
				using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
				{
					command.ExecuteNonQuery();
				}
			}
		}
		
		/// <summary>
		/// Executes the stored procedure with the specified parameters and returns the result as a <see cref="SqlDataReader"/>.
		/// </summary>
		/// <param name="connectionStringName">The name of the connection string to use.</param>
		/// <param name="commandType">The command type.</param>
		/// <param name="commandText">The stored procedure to execute.</param>
		/// <param name="parameters">The parameters of the stored procedure.</param>
		/// <returns>A <see cref="SqlDataReader"/> containing the results of the stored procedure execution.</returns>
		public static SqlDataReader ExecuteReader(string connectionStringName, CommandType commandType, string commandText, params SqlParameter[] parameters)
		{
			if (Transaction.Current == null)
			{
				string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
				SqlConnection connection = new SqlConnection(connectionString);
				
				using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
				{
					return command.ExecuteReader(CommandBehavior.CloseConnection);
				}
			}
			else
			{
				SqlConnection connection = GetTransactedSqlConnection(connectionStringName);
				using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
				{
					return command.ExecuteReader();
				}
			}
		}
		
		/// <summary>
		/// Executes the stored procedure with the specified parameters, and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="connectionStringName">The name of the connection string to use.</param>
		/// <param name="commandType">The command type.</param>
		/// <param name="commandText">The stored procedure to execute.</param>
		/// <param name="parameters">The parameters of the stored procedure.</param>
		/// <returns>The first column of the first row in the result set, or a null reference if the result set is empty.</returns>
		public static object ExecuteScalar(string connectionStringName, CommandType commandType, string commandText, params SqlParameter[] parameters)
		{
			if (Transaction.Current == null)
			{
				string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
				SqlConnection connection = new SqlConnection(connectionString);

				using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
				{
					return command.ExecuteScalar();
				}
			}
			else
			{
				SqlConnection connection = GetTransactedSqlConnection(connectionStringName);
				using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
				{
					return command.ExecuteScalar();
				}
			}
		}
		
		/// <summary>
		/// Executes the stored procedure and returns the result as a <see cref="DataSet"/>.
		/// </summary>
		/// <param name="connectionStringName">The name of the connection string to use.</param>
		/// <param name="commandType">The command type.</param>
		/// <param name="commandText">The stored procedure to execute.</param>
		/// <param name="parameters">The parameters of the stored procedure.</param>
		/// <returns>A <see cref="DataSet"/> containing the results of the stored procedure execution.</returns>
		public static DataSet ExecuteDataSet(string connectionStringName, CommandType commandType, string commandText, params SqlParameter[] parameters)
		{
			if (Transaction.Current == null)
			{
				string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
					{
						return CreateDataSet(command);
					}
				}
			}
			else
			{
				SqlConnection connection = GetTransactedSqlConnection(connectionStringName);
				using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
				{
					return CreateDataSet(command);
				}
			}
		}

		/// <summary>
		/// Executes the stored procedure and returns the result as a <see cref="DataTable"/>.
		/// </summary>
		/// <param name="connectionStringName">The name of the connection string to use.</param>
		/// <param name="commandType">The command type.</param>
		/// <param name="commandText">The stored procedure to execute.</param>
		/// <param name="parameters">The parameters of the stored procedure.</param>
		/// <returns>A <see cref="DataTable"/> containing the results of the stored procedure execution.</returns>
		public static DataTable ExecuteDataTable(string connectionStringName, CommandType commandType, string commandText, params SqlParameter[] parameters)
		{
			if (Transaction.Current == null)
			{
				string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
					{
						return CreateDataTable(command);
					}
				}
			}
			else
			{
				SqlConnection connection = GetTransactedSqlConnection(connectionStringName);
				using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
				{
					return CreateDataTable(command);
				}
			}
		}
		
		/// <summary>
		/// Gets the SqlConnection to be used as part of the current transaction.
		/// </summary>
		/// <param name="connectionStringName">The name of the connection string to use.</param>
		/// <returns>The SqlConnection associated with the current transaction.</returns>
		/// <remarks>If no SqlConnection exists, one will be created.</remarks>
		private static SqlConnection GetTransactedSqlConnection(string connectionStringName)
		{
			LocalDataStoreSlot connectionDictionarySlot = Thread.GetNamedDataSlot("ConnectionDictionary");
			Dictionary<string, SqlConnection> connectionDictionary = (Dictionary<string, SqlConnection>) Thread.GetData(connectionDictionarySlot);

			if (connectionDictionary == null)
			{
                TraceLog.LogEntry("GetTransactedSqlConnection()", "connectionDictionary == null");
				connectionDictionary = new Dictionary<string, SqlConnection>();
				Thread.SetData(connectionDictionarySlot, connectionDictionary);
			}

			SqlConnection connection = null;
			
			if (connectionDictionary.ContainsKey(connectionStringName))
			{
				connection = connectionDictionary[connectionStringName];
                TraceLog.LogEntry("GetTransactedSqlConnection()", "Cached in ThreadSlot: cnnString: {0} DB: {1}",connectionStringName,connection.Database);
			}
			else
			{
				string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
				connection = new SqlConnection(connectionString);
                TraceLog.LogEntry("GetTransactedSqlConnection()", "NOT CACHED in ThreadSlot: cnnString: {0} DB: {1}", connectionStringName, connection.Database);

				connectionDictionary.Add(connectionStringName, connection);                
				Transaction.Current.TransactionCompleted += new TransactionCompletedEventHandler(Current_TransactionCompleted);
			}
			
			return connection;
		}
		
		/// <summary>
		/// Event handler for when any transaction that have been enlisted are completed.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The TransactionEventArgs that contains the event data.</param>
		/// <remarks>This event handler will attempt to close any participating SqlConnections in the transaction.</remarks>
		private static void Current_TransactionCompleted(object sender, TransactionEventArgs e)
		{
			LocalDataStoreSlot connectionDictionarySlot = Thread.GetNamedDataSlot("ConnectionDictionary");
			Dictionary<string, SqlConnection> connectionDictionary = (Dictionary<string, SqlConnection>) Thread.GetData(connectionDictionarySlot);
            
			if (connectionDictionary != null)
			{
				foreach (SqlConnection connection in connectionDictionary.Values)
				{
                    TraceLog.LogEntry("Current_TransactionCompleted(): (Connection, State) = (0x{0:X}, {1}) Transaction ({2}, Status: {3})", 
                        connection == null ? -1 : connection.GetHashCode(), 
                        connection == null ? "NULL" : connection.State.ToString(),
                        e.Transaction.TransactionInformation.LocalIdentifier,
                        e.Transaction.TransactionInformation.Status);

					if (connection != null && connection.State != ConnectionState.Closed)
					{
						connection.Close();
					}
				}
				
				connectionDictionary.Clear();
			}
			else
                TraceLog.LogEntry("Current_TransactionCompleted(): connectionDictionary is NULL.");

			Thread.FreeNamedDataSlot("ConnectionDictionary");
		}

		#region Utility functions
		
		/// <summary>
		/// Converts the specified value to <code>DBNull.Value</code> if it is <code>null</code>.
		/// </summary>
		/// <param name="value">The value that should be checked for <code>null</code>.</param>
		/// <returns>The original value if it is not null, otherwise <code>DBNull.Value</code>.</returns>
		private static object CheckValue(object value)
		{
			if (value == null)
			{
				return DBNull.Value;
			}
			else
			{
				return value;
			}
		}

		#region CreateCommand
		
		/// <summary>
		/// Creates, initializes, and returns a <see cref="SqlCommand"/> instance.
		/// </summary>
		/// <param name="connection">The <see cref="SqlConnection"/> the <see cref="SqlCommand"/> should be executed on.</param>
		/// <param name="commandType">The command type.</param>
		/// <param name="commandText">The name of the stored procedure to execute.</param>
		/// <returns>An initialized <see cref="SqlCommand"/> instance.</returns>
		private static SqlCommand CreateCommand(SqlConnection connection, CommandType commandType, string commandText)
		{
            if(connection != null && connection.State == ConnectionState.Closed) connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;

			return command;
		}
		
        /// <summary>
        /// Creates, initializes, and returns a <see cref="IDbCommand"/> instance, same as above, but into a Session context.
        /// </summary>
        /// <param name="p_cnnMgr">Session factory owner of current session.</param>
        /// <param name="commandType">sic</param>
        /// <param name="commandText">SP command name</param>
        /// <param name="parameters">parameters to build command.</param>
        /// <returns>Valid IDbCommand instance.</returns>
        private static SqlCommand CreateCommand(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            SqlCommand command= SqlClientUtility.CreateCommand(connection, commandType, commandText);

            if (parameters != null)
            {
                command.Parameters.Clear();

                foreach (SqlParameter parameter in parameters)       // Append each parameter to the command
                {
                    parameter.Value = CheckValue(parameter.Value);
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

		#endregion CreateCommand

		private static DataSet CreateDataSet(SqlCommand command)
		{
			using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
			{
				DataSet dataSet = new DataSet();
				dataAdapter.Fill(dataSet);
				return dataSet;
			}
		}

		private static DataTable CreateDataTable(SqlCommand command)
		{
			using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
			{
				DataTable dataTable = new DataTable();
				dataAdapter.Fill(dataTable);
				return dataTable;
			}
		}
		
		#endregion

		#region Exception functions
		
		/// <summary>
		/// Determines if the specified exception is the result of a foreign key violation.
		/// </summary>
		/// <param name="e">The exception to check.</param>
		/// <returns><code>true</code> if the exception is a foreign key violation, otherwise <code>false</code>.</returns>
		public static bool IsForeignKeyContraintException(Exception e)
		{
			SqlException sqlex = e as SqlException;
			if (sqlex != null && sqlex.Number == 547)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Determines if the specified exception is the result of a unique constraint violation.
		/// </summary>
		/// <param name="e">The exception to check.</param>
		/// <returns><code>true</code> if the exception is a unique constraint violation, otherwise <code>false</code>.</returns>
		public static bool IsUniqueConstraintException(Exception e)
		{
			SqlException sqlex = e as SqlException;
			if (sqlex != null && (sqlex.Number == 2627 || sqlex.Number == 2601))
			{
				return true;
			}

			return false;
		}
		
		#endregion


        #region Session oriented operations:

        #region newest CreateCommand & ExecuteNonQuery & ExecuteReader

       

        public static void ExecuteNonQuery(ICnnManager p_cnnMgr, string commandText, params SqlParameter[] parameters)
        {            
            SqlConnection connection = null;

            string strInfoExcep = String.Empty;

            TraceLog.LogEntry("ExecuteNonQuery(): commandText= {0} parameters= {1}", commandText, parameters.Length);

            int res = -1;
            bool m_mustCloseSqlCnn = true;
            string m_tx = "Transact: NONE";
            
            long ticks_t0 = DateTime.Now.Ticks;

            if (Transaction.Current == null)            
                connection = p_cnnMgr.GetConnection() as SqlConnection;     // Root session connection will be used...                            
            else
            {
                TraceLog.LogEntry("Transaction.Current: LocalId {0}\t - Status {1}", Transaction.Current.TransactionInformation.LocalIdentifier, Transaction.Current.TransactionInformation.Status);
                connection = SqlClientUtility.GetTransactedSqlConnection(p_cnnMgr);
                m_mustCloseSqlCnn = false;
            }
           
            SqlCommand sqlc = null;
            p_cnnMgr.Transaction.Enlist(sqlc = SqlClientUtility.CreateCommand(connection, CommandType.StoredProcedure, commandText, parameters));

            if (p_cnnMgr.IsInActiveTransaction)
            {
                if (sqlc.Transaction != null)
                {
                    m_tx = String.Format("Transact 0x{0:X}", sqlc.Transaction.GetHashCode());
                    m_mustCloseSqlCnn = false;
                }
            }
            


            res = sqlc.ExecuteNonQuery();

            if (m_mustCloseSqlCnn && sqlc.Connection.State == ConnectionState.Open) sqlc.Connection.Close();

            double mseg = TimeSpan.FromTicks(DateTime.Now.Ticks - ticks_t0).TotalMilliseconds;
            TraceLog.LogEntry("ExecuteNonQuery(): Ejecutado SqlCmd bajo {0} sobre SqlConn 0x{1:X}. Tiempo {2} mseg.!", m_tx, sqlc.Connection.GetHashCode(), mseg);            
        }

        public static void ExecuteNonQuery(ICnnManager p_cnnMgr, List<SqlCommand> p_lstSqlCmd)
        {

            int res = -1;
            string m_tx = String.Empty;
            int n = 0;

            //if ((n = p_lstSqlCmd.Count) > 0)
            //    SharpLogger.Nfo("p_lstSqlCmd.Count= {0} {1} [{2}..{3}]", n, p_lstSqlCmd[0].CommandText, Param2Str(p_lstSqlCmd[0].Parameters), Param2Str(p_lstSqlCmd[n - 1].Parameters));
            //else
            //    SharpLogger.Nfo("p_lstSqlCmd.Count= {0} [...]", n);

            long ticks_t0 = DateTime.Now.Ticks;
            IDbConnection m_sqlCnn = null;

            if (Transaction.Current == null)
                m_sqlCnn = (p_cnnMgr.GetConnection());
            else
                m_sqlCnn = GetTransactedSqlConnection(p_cnnMgr);

            SqlCommand sqlCmd = null;
            m_tx = "SqlCmd Tran NULL";

            for (int i = 0; i < n; i++)
            {
                sqlCmd = p_lstSqlCmd[i];

                if (m_sqlCnn != sqlCmd.Connection) sqlCmd.Connection = m_sqlCnn as SqlConnection;
                p_cnnMgr.Transaction.Enlist(sqlCmd);

                if (sqlCmd.Transaction != null) m_tx = String.Format("SqlCmd Tran 0x{0:X}", sqlCmd.Transaction.GetHashCode());

                res = sqlCmd.ExecuteNonQuery();

                TraceLog.LogEntry("ExecuteNonQuery(): {0} SqlCnn Hash 0x{1:X}. ExecNonQuery: {2} ({3}). Filas: {4}", m_tx, sqlCmd.Connection.GetHashCode(), sqlCmd.CommandText, "sqlCmd.Parameters", res);
            }

            TraceLog.LogEntry("ExecuteNonQuery(): Ejecutado lote. Tiempo: {0} mseg.", TimeSpan.FromTicks(DateTime.Now.Ticks - ticks_t0).TotalMilliseconds);
            ////SharpLogger.CallerOut();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_cnnMgr">Name for connection manager on current session.</param> 
        /// <param name="p_sqlCmd">Built instance of SqlCommand to execute expecting a reader.</param>        
        /// <returns>A <see cref="SqlDataReader"/>Containing the results of the stored procedure execution.</returns>
        public static SqlDataReader ExecuteReader(ICnnManager p_cnnMgr, string commandText, params SqlParameter[] parameters)
        {           
           
            CommandBehavior beh = CommandBehavior.Default;           
            SqlConnection connection = null;

            string strInfoExcep = String.Empty;

            TraceLog.LogEntry("ExecuteReader({0} par: {1}) ", commandText, parameters.Length);
            
            try
            {               
                if (Transaction.Current == null)
                {                                        
                    connection = p_cnnMgr.GetConnection() as SqlConnection;                    
                    beh = CommandBehavior.CloseConnection;                    
                }
                else
                {
                    TraceLog.LogEntry("ExecuteReader(): Transaction.Current: LocalId {0} \t - Status {1} CurrentTime: {2}",
                                        Transaction.Current.TransactionInformation.LocalIdentifier,
                                        Transaction.Current.TransactionInformation.Status,
                                        Transaction.Current.TransactionInformation.CreationTime);

                    connection = SqlClientUtility.GetTransactedSqlConnection(p_cnnMgr);                                       
                }
               
                return SqlClientUtility.CreateCommand(connection, CommandType.StoredProcedure, commandText, parameters).ExecuteReader(beh);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    strInfoExcep = ex.ToString() + "/" + ex.InnerException.ToString();
                else
                    strInfoExcep = ex.ToString() + "/" + "-";

                TraceLog.LogEntry("Error:", strInfoExcep);

                return null;
            }                       
        }

        #endregion

        /// <summary>
        /// Gets the SqlConnection to be used as part of the current transaction.
        /// </summary>
        /// <param name="p_cnnMgr">The name of the connection string to use.</param>
        /// <returns>The SqlConnection associated with the current transaction.</returns>
        /// <remarks>If no SqlConnection exists, one will be created.</remarks>
        private static SqlConnection GetTransactedSqlConnection(ICnnManager p_cnnMgr)
        {
            LocalDataStoreSlot connectionDictionarySlot = Thread.GetNamedDataSlot("ConnectionDictionary");
            Dictionary<string, SqlConnection> connectionDictionary = (Dictionary<string, SqlConnection>)Thread.GetData(connectionDictionarySlot);

            if (connectionDictionary == null)
            {
                Thread.SetData(connectionDictionarySlot, connectionDictionary = new Dictionary<string, SqlConnection>());
                TraceLog.LogEntry("GetTransactedSqlConnection()"," ThreadSlotCache era NULL, se instanció y luego se cacheará par ("+ p_cnnMgr.CnnStrName+",SqlConn) válido.");
            }

            IDbConnection cnn = null;

            if (connectionDictionary.ContainsKey(p_cnnMgr.CnnStrName))
            {
                cnn = connectionDictionary[p_cnnMgr.CnnStrName];

                if (cnn.State != ConnectionState.Open)
                    TraceLog.LogEntry("GetTransactedSqlConnection(): Key '{0}' CACHED in ThreadSlot, Maps -> valid SqlConn Hash 0x{1:X} State: {2}", p_cnnMgr.CnnStrName, cnn.GetHashCode(), cnn.State);                    
            }
            else
            {
                

                if ((cnn = p_cnnMgr.GetConnection()) != null)  // Si es necesario, el manager la instancia y además la conecta...
                {
                    connectionDictionary.Add(p_cnnMgr.CnnStrName, (cnn as SqlConnection));

                    TraceLog.LogEntry("GetTransactedSqlConnection(): Key '{0}' NOT CACHED in ThreadSlot: Added SqlConn Hash 0x{1:X} State: {2}", p_cnnMgr.CnnStrName, cnn.GetHashCode(), cnn.State);                    
                    Transaction.Current.TransactionCompleted += new TransactionCompletedEventHandler(Current_TransactionCompleted);
                }
            }

            return cnn as SqlConnection;
        }


        #endregion

    }
}
