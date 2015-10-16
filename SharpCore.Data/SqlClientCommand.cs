using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SharpCore.Data
{
    public class SqlClientCommand
    {
        public SqlParameter[] Parameters { get; private set; }

        public string CommandText { get; private set; }

        public SqlClientCommand(string p_commandText, params SqlParameter[] p_SqlParams)
        {
            this.Parameters = p_SqlParams;
            this.CommandText = p_commandText;
        }
    }
}
