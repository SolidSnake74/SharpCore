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
				connectionDictionary = new Dictionary<string, SqlConnection>();
				Thread.SetData(connectionDictionarySlot, connectionDictionary);
			}

			SqlConnection connection = null;
			
			if (connectionDictionary.ContainsKey(connectionStringName))
			{
				connection = connectionDictionary[connectionStringName];
			}
			else
			{
				string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
				connection = new SqlConnection(connectionString);
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
					if (connection != null && connection.State != ConnectionState.Closed)
					{
						connection.Close();
					}
				}
				
				connectionDictionary.Clear();
			}
			
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
			if (connection != null && connection.State == ConnectionState.Closed)
			{
				connection.Open();
			}

			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandText = commandText;
			command.CommandType = commandType;
			return command;
		}

		/// <summary>
		/// Creates, initializes, and returns a <see cref="SqlCommand"/> instance.
		/// </summary>
		/// <param name="connection">The <see cref="SqlConnection"/> the <see cref="SqlCommand"/> should be executed on.</param>
		/// <param name="commandType">The command type.</param>
		/// <param name="commandText">The name of the stored procedure to execute.</param>
		/// <param name="parameters">The parameters of the stored procedure.</param>
		/// <returns>An initialized <see cref="SqlCommand"/> instance.</returns>
		private static SqlCommand CreateCommand(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] parameters)
		{
			if (connection != null && connection.State == ConnectionState.Closed)
			{
				connection.Open();
			}

			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandText = commandText;
			command.CommandType = commandType;
			
			if (parameters != null)
			{
				// Append each parameter to the command
				foreach (SqlParameter parameter in parameters)
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
	}
}
