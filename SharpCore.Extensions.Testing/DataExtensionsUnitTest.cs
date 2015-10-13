using System;
using System.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpCore.Extensions;

namespace SharpCore.Extensions.Testing
{
	/// <summary>
	/// Tests the functionality of <see cref="System.Extensions.DataExtensions"/>.
	/// </summary>
	[TestClass]
	public class DataExtensionsUnitTest
	{
		[TestMethod]
		public void GetDataRowTest()
		{
			DataSet dataSet = new DataSet();
			DataRow dataRow = null;
			
			dataRow = dataSet.GetDataRow();
			Assert.IsNull(dataRow);
			
			DataTable dataTable = dataSet.Tables.Add();
			dataTable.Columns.Add("TestColumn", typeof(string));
			dataRow = dataTable.Rows.Add("TestValue");

			dataRow = dataSet.GetDataRow();
			Assert.IsNotNull(dataRow);
			
			dataRow = dataTable.Rows.Add("AnotherTestValue");
			dataRow = dataSet.GetDataRow();
			Assert.IsNotNull(dataRow);
			
			dataSet.Tables[0].Rows.RemoveAt(1);
			dataRow = dataSet.GetDataRow();
			Assert.IsNotNull(dataRow);
			
			dataSet.Tables.Add();
			dataRow = dataSet.GetDataRow();
			Assert.IsNotNull(dataRow);
		}
		
		[TestMethod]
		public void GetDataRowCollectionTest()
		{
			DataSet dataSet = new DataSet();
			DataRowCollection dataRows = null;

			dataRows = dataSet.GetDataRowCollection();
			Assert.IsNull(dataRows);
			
			DataTable dataTable = dataSet.Tables.Add();
			dataTable.Columns.Add("TestColumn", typeof(string));
			dataTable.Rows.Add("TestValue");
			dataRows = dataSet.GetDataRowCollection();
			Assert.IsNotNull(dataRows);
			Assert.AreNotEqual(dataRows.Count, 0);
		}
		
		[TestMethod]
		public void GetFieldsTest()
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = dataSet.Tables.Add();
			DataRow dataRow = null;
			
			dataTable.Columns.Add("BooleanColumn", typeof(bool));
			dataTable.Columns.Add("ByteColumn", typeof(byte));
			dataTable.Columns.Add("BytesColumn", typeof(byte[]));
			dataTable.Columns.Add("DateTimeColumn", typeof(DateTime));
			dataTable.Columns.Add("DecimalColumn", typeof(Decimal));
			dataTable.Columns.Add("DoubleColumn", typeof(double));
			dataTable.Columns.Add("GuidColumn", typeof(Guid));
			dataTable.Columns.Add("SingleColumn", typeof(float));
			dataTable.Columns.Add("Int16Column", typeof(short));
			dataTable.Columns.Add("Int32Column", typeof(int));
			dataTable.Columns.Add("Int64Column", typeof(long));
			dataTable.Columns.Add("ObjectColumn", typeof(object));
			dataTable.Columns.Add("StringColumn", typeof(string));
			
			dataRow = dataTable.Rows.Add(
				true,
				Byte.MaxValue,
				new byte[] { 1, 2, 3 },
				DateTime.Today,
				Decimal.MaxValue,
				Double.MaxValue,
				Guid.Empty,
				Single.MaxValue,
				Int16.MaxValue,
				Int32.MaxValue,
				Int64.MaxValue,
				Environment.Version,
				Environment.MachineName
			);

			bool boolValue = dataRow.GetBoolean("BooleanColumn", false);
			Assert.AreEqual<bool>(true, boolValue);
			
			byte byteValue = dataRow.GetByte("ByteColumn", Byte.MinValue);
			Assert.AreEqual<byte>(Byte.MaxValue, byteValue);
			
			byte[] bytesValue = dataRow.GetBytes("BytesColumn", new byte[0]);
			Assert.AreEqual<int>(3, bytesValue.Length);
			
			DateTime dateTimeValue = dataRow.GetDateTime("DateTimeColumn", DateTime.MinValue);
			Assert.AreEqual<DateTime>(DateTime.Today, dateTimeValue);
			
			Decimal decimalValue = dataRow.GetDecimal("DecimalColumn", Decimal.MinValue);
			Assert.AreEqual<Decimal>(Decimal.MaxValue, decimalValue);
			
			double doubleValue = dataRow.GetDouble("DoubleColumn", Double.MinValue);
			Assert.AreEqual<double>(Double.MaxValue, doubleValue);
			
			Guid guidValue = dataRow.GetGuid("GuidColumn", new Guid("12345678-1234-1234-1234-1234567890AB"));
			Assert.AreEqual<Guid>(Guid.Empty, guidValue);
			
			float singleValue = dataRow.GetSingle("SingleColumn", Single.MinValue);
			Assert.AreEqual<float>(Single.MaxValue, singleValue);
			
			short int16Value = dataRow.GetInt16("Int16Column", Int16.MinValue);
			Assert.AreEqual<short>(Int16.MaxValue, int16Value);

			int int32Value = dataRow.GetInt32("Int32Column", Int32.MinValue);
			Assert.AreEqual<int>(Int32.MaxValue, int32Value);

			long int64Value = dataRow.GetInt64("Int64Column", Int64.MinValue);
			Assert.AreEqual<long>(Int64.MaxValue, int64Value);
			
			object objectValue = dataRow.GetObject("ObjectColumn", new Version(0, 0, 0, 0));
			Assert.AreEqual<object>(Environment.Version, objectValue);
			
			string stringValue = dataRow.GetString("StringColumn", String.Empty);
			Assert.AreEqual<string>(Environment.MachineName, stringValue);
		}

		[TestMethod]
		public void GetNullFieldsTest()
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = dataSet.Tables.Add();
			DataRow dataRow = null;

			dataTable.Columns.Add("BooleanColumn", typeof(bool));
			dataTable.Columns.Add("ByteColumn", typeof(byte));
			dataTable.Columns.Add("BytesColumn", typeof(byte[]));
			dataTable.Columns.Add("DateTimeColumn", typeof(DateTime));
			dataTable.Columns.Add("DecimalColumn", typeof(Decimal));
			dataTable.Columns.Add("DoubleColumn", typeof(double));
			dataTable.Columns.Add("GuidColumn", typeof(Guid));
			dataTable.Columns.Add("SingleColumn", typeof(float));
			dataTable.Columns.Add("Int16Column", typeof(short));
			dataTable.Columns.Add("Int32Column", typeof(int));
			dataTable.Columns.Add("Int64Column", typeof(long));
			dataTable.Columns.Add("ObjectColumn", typeof(object));
			dataTable.Columns.Add("StringColumn", typeof(string));

			dataRow = dataTable.Rows.Add(DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);

			bool boolValue = dataRow.GetBoolean("BooleanColumn", false);
			Assert.AreEqual<bool>(false, boolValue);

			byte byteValue = dataRow.GetByte("ByteColumn", Byte.MinValue);
			Assert.AreEqual<byte>(Byte.MinValue, byteValue);

			byte[] bytesValue = dataRow.GetBytes("BytesColumn", new byte[0]);
			Assert.AreEqual<int>(0, bytesValue.Length);

			DateTime dateTimeValue = dataRow.GetDateTime("DateTimeColumn", DateTime.MinValue);
			Assert.AreEqual<DateTime>(DateTime.MinValue, dateTimeValue);

			Decimal decimalValue = dataRow.GetDecimal("DecimalColumn", Decimal.MinValue);
			Assert.AreEqual<Decimal>(Decimal.MinValue, decimalValue);

			double doubleValue = dataRow.GetDouble("DoubleColumn", Double.MinValue);
			Assert.AreEqual<double>(Double.MinValue, doubleValue);

			Guid guidValue = dataRow.GetGuid("GuidColumn", Guid.Empty);
			Assert.AreEqual<Guid>(Guid.Empty, guidValue);

			float singleValue = dataRow.GetSingle("SingleColumn", Single.MinValue);
			Assert.AreEqual<float>(Single.MinValue, singleValue);

			short int16Value = dataRow.GetInt16("Int16Column", Int16.MinValue);
			Assert.AreEqual<short>(Int16.MinValue, int16Value);

			int int32Value = dataRow.GetInt32("Int32Column", Int32.MinValue);
			Assert.AreEqual<int>(Int32.MinValue, int32Value);

			long int64Value = dataRow.GetInt64("Int64Column", Int64.MinValue);
			Assert.AreEqual<long>(Int64.MinValue, int64Value);

			object objectValue = dataRow.GetObject("ObjectColumn", new Version(0, 0, 0, 0));
			Assert.AreEqual<object>(new Version(0, 0, 0, 0), objectValue);

			string stringValue = dataRow.GetString("StringColumn", String.Empty);
			Assert.AreEqual<string>(String.Empty, stringValue);
		}
	}
}
