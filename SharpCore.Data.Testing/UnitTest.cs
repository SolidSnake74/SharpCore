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
        public void TransactionTest_Session()
        {
            SessionFactory sf = new SessionFactory("ConnectionString");
           
            if(sf!= null)
            {                               
                SqlDataReader dr = null;
                SqlDataReader dr2 = null;
                                
                using (ISessionTX ses = sf.OpenSession() as ISessionTX)
                {                    
                    dr = SqlClientUtility.ExecuteReader(ses, "PadreSelectAll");
                    dr.Close();
                   
                    dr2 = SqlClientUtility.ExecuteReader(ses, "HijoSelectAllByPadre", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32){Value=0} });
                    dr2.Close();
                }

                sf.Close();
            }
            
        }

        [TestMethod]
        public void TransactionTest_Session_update()
        {
            SessionFactory sf = new SessionFactory("ConnectionString");

            if (sf != null)
            {
                SqlDataReader dr = null;
                SqlDataReader dr2 = null;

                using (ISessionTX ses = sf.OpenSession() as ISessionTX)
                {
                    using (TransactionScope tx = ses.GetTransactScope())
                    {
                        SqlClientUtility.ExecuteNonQuery(ses, "PadreUpdate", new SqlParameter[] { new SqlParameter("@Id", DbType.Int32) { Value = 0 }, 
                                                                                                                  new SqlParameter("@Nombre", DbType.String) { Value = DateTime.Now.ToLongTimeString() }});

                        SqlClientUtility.ExecuteNonQuery(ses, "PadreUpdate", new SqlParameter[] { new SqlParameter("@Id", DbType.Int32) { Value = 1 }, 
                                                                                                                  new SqlParameter("@Nombre", DbType.String) { Value = DateTime.Now.ToLongTimeString() }});

                        tx.Complete();
                    }

                    dr = SqlClientUtility.ExecuteReader(ses, "PadreSelectAll");
                    dr.Close();

                    dr2 = SqlClientUtility.ExecuteReader(ses, "HijoSelectAllByPadre", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32) { Value = 0 } });
                    dr2.Close();
                }

                sf.Close();
            }

        }

        [TestMethod]
        public void TransactionTest_Session_update2()
        {
            SessionFactory sf = new SessionFactory("ConnectionString");

            if (sf != null)
            {
                using (ISessionTX ses = sf.OpenSession() as ISessionTX)
                {
                    string str_val = string.Format("{0}", DateTime.Now.TimeOfDay);
                                       
                    List<SqlClientCommand> lst_SqlCmd = new List<SqlClientCommand>(new SqlClientCommand[] 
                        {                         
                            new SqlClientCommand("HijoUpdate", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32) { Value = 0 }, 
                                                                                    new SqlParameter("@Id", DbType.Int32) { Value = 0 }, 
                                                                                    new SqlParameter("@Nombre", DbType.String) { Value = "P0.h0-" + str_val } }),
                            new SqlClientCommand("HijoUpdate", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32) { Value = 0 }, 
                                                                                    new SqlParameter("@Id", DbType.Int32) { Value = 1 }, 
                                                                                    new SqlParameter("@Nombre", DbType.String) { Value = "P0.h1-" + str_val } }),
                            new SqlClientCommand("HijoUpdate", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32) { Value = 0 }, 
                                                                                    new SqlParameter("@Id", DbType.Int32) { Value = 2 }, 
                                                                                    new SqlParameter("@Nombre", DbType.String) { Value = "P0.h2-" + str_val } }),
                            new SqlClientCommand("HijoUpdate", new SqlParameter[] { new SqlParameter("@Padre", DbType.Int32) { Value = 0 }, 
                                                                                    new SqlParameter("@Id", DbType.Int32) { Value = 3 }, 
                                                                                    new SqlParameter("@Nombre", DbType.String) { Value = "P0.h3-" + str_val } })
                        });

                    try
                    {
                        
                        using (TransactionScope ts = ses.GetTransactScope())
                        {                       
                            SqlClientUtility.ExecuteNonQuery(ses, "PadreUpdate", new SqlParameter[] { new SqlParameter("@Id", DbType.Int32) { Value = 0 }, 
                                                                                                      new SqlParameter("@Nombre", DbType.String) { Value = "P0-" + str_val } });
                            SqlClientUtility.ExecuteNonQuery(ses, lst_SqlCmd);
                            
                            ts.Complete();
                        }
                    }
                    catch (TransactionAbortedException ex)
                    {
                        System.Diagnostics.Trace.WriteLine("TransactionAbortedException Message: " + ex.Message);
                    }
                    finally
                    {
                    }
                }

                sf.Close();
            }

        }
	}
}
