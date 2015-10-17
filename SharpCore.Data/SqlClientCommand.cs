using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SharpCore.Data
{
    public class SqlClientCommand
    {
        private string m_parmDescr = string.Empty;

        public SqlParameter[] Parameters { get; private set; }


        public string CommandText { get; private set; }

        public SqlClientCommand(string p_commandText, params SqlParameter[] p_SqlParams)
        {
            this.Parameters = p_SqlParams;
            this.CommandText = p_commandText;

            this.m_parmDescr = "[";
            if (p_SqlParams != null)
            {
                foreach (SqlParameter p in p_SqlParams)
                    this.m_parmDescr += string.Format("({0},{1})", p.ParameterName, p.Value == null ? "NULL" : p.Value.ToString());
            }
            this.m_parmDescr += "]";            
        }

        public string ParametersDescr
        {
            get 
            {
                return this.m_parmDescr;
            }
        }
    }
}
