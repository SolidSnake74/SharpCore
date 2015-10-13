using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace SharpCore.Utilities
{
    /// <summary>
    /// Provides validation functionality for method arguments or property assignments.
    /// </summary>
    public abstract class ValidationUtility
    {
		#region ValidateArgument
		
        /// <summary>
        /// Validates that the <see cref="System.String"/> value is not <code>null</code> or empty.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        public static void ValidateArgument(string name, string value)
        {
            ValidateArgument(name, value, true);
        }

        /// <summary>
        /// Validates that the <see cref="System.String"/> value is not <code>null</code> or empty.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <param name="checkLength">Indicates if the value's length should be validated.</param>
        public static void ValidateArgument(string name, string value, bool checkLength)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }

            if (checkLength && value.Length == 0)
            {
                string message = String.Format("{0} cannot be a zero-length string.", name);
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Validates that the <see cref="System.Object"/> value is not <code>null</code>.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        public static void ValidateArgument(string name, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }
        
        #endregion
        
        #region IsNullOrEmpty
        
        /// <summary>
        /// Determines if the specified <see cref="System.Collections.ICollection"/> is <code>null</code> or empty.
        /// </summary>
        /// <param name="collection">The collection to check.</param>
        /// <returns><code>true</code> if the collection is <code>null</code> or empty; otherwise, <code>false</code>.</returns>
        public static bool IsNullOrEmpty(ICollection collection)
        {
			if (collection == null || collection.Count == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
        }

		/// <summary>
		/// Determines if the specified <see cref="System.Data.DataSet"/> is <code>null</code> or empty.
		/// </summary>
		/// <param name="dataSet">The <see cref="System.Data.DataSet"/> to check.</param>
		/// <returns><code>true</code> if the <see cref="System.Data.DataSet"/> is <code>null</code> or empty; otherwise, <code>false</code>.</returns>
		public static bool IsNullOrEmpty(DataSet dataSet)
		{
			if (dataSet == null || dataSet.Tables.Count == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Determines if the specified <see cref="System.Data.DataTable"/> is <code>null</code> or empty.
		/// </summary>
		/// <param name="dataTable">The <see cref="System.Data.DataTable"/> to check.</param>
		/// <returns><code>true</code> if the <see cref="System.Data.DataTable"/> is <code>null</code> or empty; otherwise, <code>false</code>.</returns>
		public static bool IsNullOrEmpty(DataTable dataTable)
		{
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Determines if the specified <see cref="System.Data.SqlClient.SqlDataReader"/> is <code>null</code> or empty.
		/// </summary>
		/// <param name="dataReader">The <see cref="System.Data.SqlClient.SqlDataReader"/> to check.</param>
		/// <returns><code>true</code> if the <see cref="System.Data.SqlClient.SqlDataReader"/> is <code>null</code> or empty; otherwise, <code>false</code>.</returns>
		public static bool IsNullOrEmpty(SqlDataReader dataReader)
		{
			if (dataReader == null || dataReader.HasRows == false)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		#endregion
	}
}
