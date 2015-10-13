using System;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpCore.Data;

namespace SharpCore.Data.Testing
{
	/// <summary>
	/// Summary description for UnitTest
	/// </summary>
	[TestClass]
	public class UnitTest
	{
		#region SqlClientUtility
		[TestMethod]
		public void ExecuteDataTableUnitTest()
		{
			DataTable dataTable = SqlClientUtility.ExecuteDataTable("ConnectionString", CommandType.StoredProcedure, "sp_tables");
			Assert.IsTrue(dataTable.Columns.Count > 0);
			Assert.IsTrue(dataTable.Rows.Count > 0);
		}

		[TestMethod]
		public void ExecuteDataSetUnitTest()
		{
			DataSet dataSet = SqlClientUtility.ExecuteDataSet("ConnectionString", CommandType.StoredProcedure, "sp_tables");
			Assert.IsTrue(dataSet.Tables.Count > 0);
			Assert.IsTrue(dataSet.Tables[0].Columns.Count > 0);
			Assert.IsTrue(dataSet.Tables[0].Rows.Count > 0);
		}

		[TestMethod]
		public void ExecuteNonQueryUnitTest()
		{
			SqlClientUtility.ExecuteNonQuery("ConnectionString", CommandType.StoredProcedure, "sp_tables");
		}

		[TestMethod]
		public void ExecuteReaderUnitTest()
		{
			using (SqlDataReader dataReader = SqlClientUtility.ExecuteReader("ConnectionString", CommandType.StoredProcedure, "sp_tables"))
			{
				Assert.IsTrue(dataReader.FieldCount > 0);
				Assert.IsTrue(dataReader.HasRows);
				Assert.IsFalse(dataReader.IsClosed);
			}
		}

		[TestMethod]
		public void ExecuteScalarUnitTest()
		{
			string value = (string) SqlClientUtility.ExecuteScalar("ConnectionString", CommandType.StoredProcedure, "sp_tables");
			Assert.IsNotNull(value);
			Assert.IsTrue(value.Length > 0);
		}
		
		[TestMethod]
		public void TransactionTest()
		{
			using (TransactionScope scope = new TransactionScope())
			{
				SqlClientUtility.ExecuteNonQuery("ConnectionString", CommandType.StoredProcedure, "sp_tables");
				SqlClientUtility.ExecuteNonQuery("ConnectionString", CommandType.StoredProcedure, "sp_tables");
			}
		}
		#endregion
	}
}
