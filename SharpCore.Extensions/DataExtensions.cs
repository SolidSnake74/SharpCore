using System;
using System.Data;
using System.Data.SqlClient;

using SharpCore.Utilities;

namespace SharpCore.Extensions
{
	/// <summary>
	/// Provides added functionality to classes in the <see cref="System.Data"/> namespace.
	/// </summary>
	public static class DataExtensions
	{
		#region DataSet and DataTable Functions

		/// <summary>
		/// Returns the first (and only) DataTable found in the DataSet.
		/// </summary>
		/// <param name="dataSet">The DataSet to retrieve the DataTable from.</param>
		/// <returns>The first (and only) DataTable found in the DataSet, otherwise null.</returns>
		public static DataTable GetDataTable(this DataSet dataSet)
		{
			if (ValidationUtility.IsNullOrEmpty(dataSet) == false)
			{
				return dataSet.Tables[0];
			}

			return null;
		}
		
		/// <summary>
		/// Gets the first (and only) <see cref="System.Data.DataRow"/> from the first (and only) <see cref="Ssytem.Data.DataTable"/> in the <see cref="System.Data.DataSet"/>.
		/// </summary>
		/// <param name="dataSet">The <see cref="System.Data.DataSet"/> to get the <see cref="System.Data.DataRow"/> from.</param>
		/// <returns>The first (and only) <see cref="System.Data.DataRow"/> from the first (and only) <see cref="Ssytem.Data.DataTable"/> in the <see cref="System.Data.DataSet"/>.</returns>
		/// <remarks>If there are more than one <see cref="System.Data.DataTable"/> or <see cref="System.Data.DataRow"/> in the <see cref="System.Data.DataSet"/>, <code>null</code> is returned.</remarks>
		public static DataRow GetDataRow(this DataSet dataSet)
		{
			if (ValidationUtility.IsNullOrEmpty(dataSet) == false)
			{
				DataTable dataTable = dataSet.Tables[0];
				if (ValidationUtility.IsNullOrEmpty(dataTable) == false)
				{
					return dataTable.Rows[0];
				}
			}

			return null;
		}
		
		/// <summary>
		/// Gets the first (and only) <see cref="System.Data.DataRow"/> in the <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <param name="dataTable">The <see cref="System.Data.DataTable"/> to get the <see cref="System.Data.DataRow"/> from.</param>
		/// <returns>The first (and only) <see cref="System.Data.DataRow"/> in the <see cref="Ssytem.Data.DataTable"/>.</returns>
		/// <remarks>If there are more than one <see cref="System.Data.DataRow"/> in the <see cref="System.Data.DataTable"/>, <code>null</code> is returned.</remarks>
		public static DataRow GetDataRow(this DataTable dataTable)
		{
			if (ValidationUtility.IsNullOrEmpty(dataTable) == false)
			{
				return dataTable.Rows[0];
			}
			
			return null;
		}
		
		/// <summary>
		/// Gets the first (and only) <see cref="System.Data.DataRowCollection"/> from the first (and only) <see cref="System.Data.DataTable"/> in the <see cref="System.Data.DataSet"/>.
		/// </summary>
		/// <param name="dataSet">The <see cref="System.Data.DataSet"/> to get the <see cref="System.Data.DataRowCollection"/> from.</param>
		/// <returns>The <see cref="System.Data.DataRowCollection"/> from the first <see cref="System.Data.DataTable"/> in the <see cref="System.Data.DataSet"/></returns>
		/// <remarks>If there are more than one <see cref="System.Data.DataTable"/> in the <see cref="System.Data.DataSet"/>, an empty <see cref="System.Data.DataRowCollection"/> is returned.</remarks>
		public static DataRowCollection GetDataRowCollection(this DataSet dataSet)
		{
			if (ValidationUtility.IsNullOrEmpty(dataSet) == false)
			{
				DataTable dataTable = dataSet.Tables[0];
				return dataTable.Rows;
			}
			
			return null;
		}
		
		#endregion

		#region Field functions

		/// <summary>
		/// Attempts to extract the requested column value from a DataRow.
		/// </summary>
		/// <param name="dataRow">The DataRow to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static bool GetBoolean(this DataRow dataRow, string columnName, bool valueIfNull)
		{
			object value = GetObject(dataRow, columnName, null);
			if (value != null)
			{
				if (value is bool)
				{
					return (bool) value;
				}
				else if (value is byte)
				{
					byte byteValue = (byte) value;
					if (byteValue == 0)
					{
						return false;
					}
					else
					{
						return true;
					}
				}
				else
				{
					return Boolean.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a SqlDataReader.
		/// </summary>
		/// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static bool GetBoolean(this SqlDataReader dataReader, string columnName, bool valueIfNull)
		{
			object value = GetObject(dataReader, columnName, valueIfNull);
			if (value != null)
			{
				if (value is bool)
				{
					return (bool) value;
				}
				else if (value is byte)
				{
					byte byteValue = (byte) value;
					if (byteValue == 0)
					{
						return false;
					}
					else
					{
						return true;
					}
				}
				else
				{
					return Boolean.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a DataRow.
		/// </summary>
		/// <param name="dataRow">The DataRow to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise the value specified by <code>valueIfNull</code>.</returns>
		public static byte GetByte(this DataRow dataRow, string columnName, byte valueIfNull)
		{
			object value = GetObject(dataRow, columnName, null);
			if (value != null)
			{
				if (value is byte)
				{
					return (byte) value;
				}
				else
				{
					return Byte.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a SqlDataReader.
		/// </summary>
		/// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise the value specified by <code>valueIfNull</code>.</returns>
		public static byte GetByte(this SqlDataReader dataReader, string columnName, byte valueIfNull)
		{
			object value = GetObject(dataReader, columnName, null);
			if (value != null)
			{
				if (value is byte)
				{
					return (byte) value;
				}
				else
				{
					return Byte.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a DataRow.
		/// </summary>
		/// <param name="dataRow">The DataRow to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise the value specified by <code>valueIfNull</code>.</returns>
		public static byte[] GetBytes(this DataRow dataRow, string columnName, byte[] valueIfNull)
		{
			object value = GetObject(dataRow, columnName, null);
			if (value != null && value is byte[])
			{
				return (byte[]) value;
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a SqlDataReader.
		/// </summary>
		/// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise the value specified by <code>valueIfNull</code>.</returns>
		public static byte[] GetBytes(this SqlDataReader dataReader, string columnName, byte[] valueIfNull)
		{
			object value = GetObject(dataReader, columnName, null);
			if (value != null && value is byte[])
			{
				return (byte[]) value;
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a DataRow.
		/// </summary>
		/// <param name="dataRow">The DataRow to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static DateTime GetDateTime(this DataRow dataRow, string columnName, DateTime valueIfNull)
		{
			object value = GetObject(dataRow, columnName, null);
			if (value != null)
			{
				if (value is DateTime)
				{
					return (DateTime) value;
				}
				else
				{
					return DateTime.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a SqlDataReader.
		/// </summary>
		/// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static DateTime GetDateTime(this SqlDataReader dataReader, string columnName, DateTime valueIfNull)
		{
			object value = GetObject(dataReader, columnName, null);
			if (value != null)
			{
				if (value is DateTime)
				{
					return (DateTime) value;
				}
				else
				{
					return DateTime.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a DataRow.
		/// </summary>
		/// <param name="dataRow">The DataRow to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static decimal GetDecimal(this DataRow dataRow, string columnName, decimal valueIfNull)
		{
			object value = GetObject(dataRow, columnName, null);
			if (value != null)
			{
				if (value is decimal)
				{
					return (decimal) value;
				}
				else
				{
					return Decimal.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a SqlDataReader.
		/// </summary>
		/// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static decimal GetDecimal(this SqlDataReader dataReader, string columnName, decimal valueIfNull)
		{
			object value = GetObject(dataReader, columnName, null);
			if (value != null)
			{
				if (value is decimal)
				{
					return (decimal) value;
				}
				else
				{
					return Decimal.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a DataRow.
		/// </summary>
		/// <param name="dataRow">The DataRow to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static double GetDouble(this DataRow dataRow, string columnName, double valueIfNull)
		{
			object value = GetObject(dataRow, columnName, null);
			if (value != null)
			{
				if (value is double)
				{
					return (double) value;
				}
				else
				{
					return Double.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a SqlDataReader.
		/// </summary>
		/// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static double GetDouble(this SqlDataReader dataReader, string columnName, double valueIfNull)
		{
			object value = GetObject(dataReader, columnName, null);
			if (value != null)
			{
				if (value is double)
				{
					return (double) value;
				}
				else
				{
					return Double.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a DataRow.
		/// </summary>
		/// <param name="dataRow">The DataRow to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static Guid GetGuid(this DataRow dataRow, string columnName, Guid valueIfNull)
		{
			object value = GetObject(dataRow, columnName, null);
			if (value != null)
			{
				if (value is Guid)
				{
					return (Guid) value;
				}
				else
				{
					return new Guid(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a SqlDataReader.
		/// </summary>
		/// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static Guid GetGuid(this SqlDataReader dataReader, string columnName, Guid valueIfNull)
		{
			object value = GetObject(dataReader, columnName, null);
			if (value != null)
			{
				if (value is Guid)
				{
					return (Guid) value;
				}
				else
				{
					return new Guid(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a DataRow.
		/// </summary>
		/// <param name="dataRow">The DataRow to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static float GetSingle(this DataRow dataRow, string columnName, float valueIfNull)
		{
			object value = GetObject(dataRow, columnName, null);
			if (value != null)
			{
				if (value is float)
				{
					return (float) value;
				}
				else
				{
					return Single.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a SqlDataReader.
		/// </summary>
		/// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static float GetSingle(this SqlDataReader dataReader, string columnName, float valueIfNull)
		{
			object value = GetObject(dataReader, columnName, null);
			if (value != null)
			{
				if (value is float)
				{
					return (float) value;
				}
				else
				{
					return Single.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a DataRow.
		/// </summary>
		/// <param name="dataRow">The DataRow to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static short GetInt16(this DataRow dataRow, string columnName, short valueIfNull)
		{
			object value = GetObject(dataRow, columnName, null);
			if (value != null)
			{
				if (value is short)
				{
					return (short) value;
				}
				else
				{
					return Int16.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a SqlDataReader.
		/// </summary>
		/// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static short GetInt16(this SqlDataReader dataReader, string columnName, short valueIfNull)
		{
			object value = GetObject(dataReader, columnName, null);
			if (value != null)
			{
				if (value is short)
				{
					return (short) value;
				}
				else
				{
					return Int16.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a DataRow.
		/// </summary>
		/// <param name="dataRow">The DataRow to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static int GetInt32(this DataRow dataRow, string columnName, int valueIfNull)
		{
			object value = GetObject(dataRow, columnName, null);
			if (value != null)
			{
				if (value is int)
				{
					return (int) value;
				}
				else
				{
					return Int32.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a SqlDataReader.
		/// </summary>
		/// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static int GetInt32(this SqlDataReader dataReader, string columnName, int valueIfNull)
		{
			object value = GetObject(dataReader, columnName, null);
			if (value != null)
			{
				if (value is int)
				{
					return (int) value;
				}
				else
				{
					return Int32.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a DataRow.
		/// </summary>
		/// <param name="dataRow">The DataRow to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static long GetInt64(this DataRow dataRow, string columnName, long valueIfNull)
		{
			object value = GetObject(dataRow, columnName, null);
			if (value != null)
			{
				if (value is long)
				{
					return (long) value;
				}
				else
				{
					return Int64.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a SqlDataReader.
		/// </summary>
		/// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static long GetInt64(this SqlDataReader dataReader, string columnName, long valueIfNull)
		{
			object value = GetObject(dataReader, columnName, null);
			if (value != null)
			{
				if (value is long)
				{
					return (long) value;
				}
				else
				{
					return Int64.Parse(value.ToString());
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a DataRow.
		/// </summary>
		/// <param name="dataRow">The DataRow to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static object GetObject(this DataRow dataRow, string columnName, object valueIfNull)
		{
			object value = dataRow[columnName];
			if (value != null && value != DBNull.Value)
			{
				return value;
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a SqlDataReader.
		/// </summary>
		/// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static object GetObject(this SqlDataReader dataReader, string columnName, object valueIfNull)
		{
			object value = dataReader[columnName];
			if (value != null && value != DBNull.Value)
			{
				return value;
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a DataRow.
		/// </summary>
		/// <param name="dataRow">The DataRow to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static string GetString(this DataRow dataRow, string columnName, string valueIfNull)
		{
			object value = GetObject(dataRow, columnName, null);
			if (value != null)
			{
				if (value is string)
				{
					return (string) value;
				}
				else
				{
					return value.ToString();
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		/// <summary>
		/// Attempts to extract the requested column value from a SqlDataReader.
		/// </summary>
		/// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
		/// <param name="columnName">The name of the column to extract the value from.</param>
		/// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
		/// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
		public static string GetString(this SqlDataReader dataReader, string columnName, string valueIfNull)
		{
			object value = GetObject(dataReader, columnName, null);
			if (value != null)
			{
				if (value is string)
				{
					return (string) value;
				}
				else
				{
					return value.ToString();
				}
			}
			else
			{
				return valueIfNull;
			}
		}

		#endregion
	}
}
