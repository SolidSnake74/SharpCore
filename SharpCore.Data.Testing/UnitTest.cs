using System;
using System.Collections.Generic;
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

               
        [TestMethod]
        public void Transaction_ScopeCommit_Test()
        {
            SessionFactory sf = new SessionFactory("ConnectionString");

            if (sf != null)
            {
                using (ISessionTX ses = sf.OpenSession() as ISessionTX)
                {
                    string str_val = DateTime.Now.TimeOfDay.ToString();
                    
                    SqlClientCommand sqlCmdPadre= new SqlClientCommand("PadreUpdate", new SqlParameter[] { new SqlParameter("@Id", DbType.Int32) { Value = 0 }, 
                                                                                                           new SqlParameter("@Nombre", DbType.String) { Value = "P0-" + str_val }});

                    List<SqlClientCommand> lst_SqlCmdUpd = new List<SqlClientCommand>(new SqlClientCommand[] 
                    {                         
                        new SqlClientCommand("HijoUpdate", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32)    { Value = 0 }, 
                                                                                new SqlParameter("@Id", DbType.Int32)       { Value = 0 }, 
                                                                                new SqlParameter("@Nombre", DbType.String)  { Value = "P0.h0-" + str_val }}),
                        new SqlClientCommand("HijoUpdate", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32)    { Value = 0 }, 
                                                                                new SqlParameter("@Id", DbType.Int32)       { Value = 1 }, 
                                                                                new SqlParameter("@Nombre", DbType.String)  { Value = "P0.h1-" + str_val }}),
                        new SqlClientCommand("HijoUpdate", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32)    { Value = 0 }, 
                                                                                new SqlParameter("@Id", DbType.Int32)       { Value = 2 }, 
                                                                                new SqlParameter("@Nombre", DbType.String)  { Value = "P0.h2-" + str_val }}),
                        new SqlClientCommand("HijoUpdate", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32)    { Value = 0 }, 
                                                                                new SqlParameter("@Id", DbType.Int32)       { Value = 3 }, 
                                                                                new SqlParameter("@Nombre", DbType.String)  { Value = "P0.h3-" + str_val }}),
                        new SqlClientCommand("HijoUpdate", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32)    { Value = 0 }, 
                                                                                new SqlParameter("@Id", DbType.Int32)       { Value = 4 }, 
                                                                                new SqlParameter("@Nombre", DbType.String)  { Value = "P0.h4-" + str_val }})
                    });

                    int idPadre = 0;
                    int idBase  = 5;

                    List<SqlClientCommand> lst_SqlCmdIns = new List<SqlClientCommand>(new SqlClientCommand[] 
                    {                         
                        new SqlClientCommand("HijoInsert", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32)    { Value = idPadre }, 
                                                                                new SqlParameter("@Id", DbType.Int32)       { Value = idBase }, 
                                                                                new SqlParameter("@Nombre", DbType.String)  { Value = string.Format("P{0}.h{1}-{2}",idPadre, idBase, str_val) }}),
                        new SqlClientCommand("HijoInsert", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32)    { Value = idPadre }, 
                                                                                new SqlParameter("@Id", DbType.Int32)       { Value = idBase+1 }, 
                                                                                new SqlParameter("@Nombre", DbType.String)  { Value = string.Format("P{0}.h{1}-{2}",idPadre, idBase+1, str_val) }}),
                        new SqlClientCommand("HijoInsert", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32)    { Value = idPadre }, 
                                                                                new SqlParameter("@Id", DbType.Int32)       { Value = idBase+2 }, 
                                                                                new SqlParameter("@Nombre", DbType.String)  { Value = string.Format("P{0}.h{1}-{2}",idPadre, idBase+2, str_val) }})                            
                    });

                                            
                    //using(TransactionScope ts = ses.GetTransactScope())                    
                    try
                    {
                        SqlClientUtility.ExecuteNonQuery(ses, sqlCmdPadre);
                        SqlClientUtility.ExecuteNonQuery(ses, lst_SqlCmdUpd);
                        SqlClientUtility.ExecuteNonQuery(ses, lst_SqlCmdIns);

                        //ts.Complete();  // Transaction Manager will commit entire scope while disposing... Otherwise, will be RollBacked.
                    }
                    catch(SqlException ex)
                    {                            
                        System.Diagnostics.Trace.WriteLine("SqlException into Scope: " + ex.ToString());
                    }
                }

                sf.Close();
            }

        }
	}
}
