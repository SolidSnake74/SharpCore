using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

using SharpCore.Caching;
using SharpCore.Data;

namespace SharpCore.Caching.Caches
{
	/// <summary>
	/// Represents a cache in which items are stored in SQL Server.
	/// </summary>
	internal sealed class SqlCache : CacheBase
	{
		private string connectionStringName;
		private string insertSql;
		private string updateSql;
		private string deleteSql;
		private string deleteAllSql;
		private string purgeSql;
		private string selectSql;
		private string selectAllSql;

		public override void Configure(XmlElement element)
		{
			base.Configure(element);

			connectionStringName = element.GetAttribute("connectionStringName");
			insertSql = element.GetAttribute("insertSql");
			updateSql = element.GetAttribute("updateSql");
			deleteSql = element.GetAttribute("deleteSql");
			deleteAllSql = element.GetAttribute("deleteAllSql");
			purgeSql = element.GetAttribute("purgeSql");
			selectSql = element.GetAttribute("selectSql");
			selectAllSql = element.GetAttribute("selectAllSql");

			RemoveAll();
		}

		public override void Add(string key, object item, DateTime absoluteExpiration, TimeSpan slidingExpiration)
		{
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Context", context),
				new SqlParameter("@Key", key),
				new	SqlParameter("@Item", ConvertObjectToByteArray(item)),
				new SqlParameter("@AbsoluteExpiration", absoluteExpiration),
				new SqlParameter("@SlidingExpiration", slidingExpiration)
			};
			
			SqlClientUtility.ExecuteNonQuery(connectionStringName, CommandType.StoredProcedure, insertSql, parameters);
		}

		public override bool Exists(string key)
		{
			if (Get(key) == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public override object Get(string key)
		{
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Context", context),
				new SqlParameter("@Key", key)
			};
			
			using (SqlDataReader dataReader = SqlClientUtility.ExecuteReader(connectionStringName, CommandType.StoredProcedure, selectSql, parameters))
			{
				if (dataReader.Read())
				{
					BinaryFormatter formatter = new BinaryFormatter();
					MemoryStream stream = new MemoryStream(dataReader.GetSqlBinary(2).Value);
					return formatter.Deserialize(stream);
				}
				else
				{
					return null;
				}
			}
		}

		protected override CachedItem GetCachedItem(string key)
		{
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Context", context),
				new SqlParameter("@Key", key)
			};
			
			using (SqlDataReader dataReader = SqlClientUtility.ExecuteReader(connectionStringName, CommandType.StoredProcedure, selectSql, parameters))
			{
				if (dataReader.Read())
				{
					BinaryFormatter formatter = new BinaryFormatter();
					MemoryStream stream = new MemoryStream(dataReader.GetSqlBinary(2).Value);

					return new CachedItem(
						key,
						formatter.Deserialize(stream),
						dataReader.GetDateTime(2),
						TimeSpan.FromSeconds(dataReader.GetInt32(3))
					);
				}
				else
				{
					return null;
				}
			}
		}

		protected override void Purge()
		{
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Context", context)
			};

			SqlClientUtility.ExecuteNonQuery(connectionStringName, CommandType.StoredProcedure, purgeSql, parameters);
		}

		public override void Remove(string key)
		{
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Context", context),
				new SqlParameter("@Key", key)
			};

			SqlClientUtility.ExecuteNonQuery(connectionStringName, CommandType.StoredProcedure, deleteSql, parameters);
		}

		private void RemoveAll()
		{
			SqlClientUtility.ExecuteNonQuery(connectionStringName, CommandType.StoredProcedure, deleteAllSql);
		}

		public override void Update(string key, object item)
		{
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Context", context),
				new SqlParameter("@Key", key),
				new	SqlParameter("@Item", ConvertObjectToByteArray(item))
			};

			SqlClientUtility.ExecuteNonQuery(connectionStringName, CommandType.StoredProcedure, updateSql, parameters);
		}

		public override IEnumerator GetEnumerator()
		{
			List<string> list = new List<string>();

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Context", context)
			};

			using (SqlDataReader dataReader = SqlClientUtility.ExecuteReader(connectionStringName, CommandType.StoredProcedure, selectAllSql, parameters))
			{
				while (dataReader.Read())
				{
					list.Add(dataReader.GetString(1));
				}
			}

			return ((IEnumerable) list).GetEnumerator();
		}

		private byte[] ConvertObjectToByteArray(object obj)
		{
			byte[] bytes;

			using (MemoryStream stream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, obj);
				bytes = new byte[stream.Length];
				stream.Write(bytes, 0, (int) stream.Length);
			}

			return bytes;
		}
	}
}
