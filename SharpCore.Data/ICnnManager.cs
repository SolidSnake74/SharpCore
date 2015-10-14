using System;
using System.Data;
using System.Data.SqlClient;
namespace SharpCore.Data
{
    public interface ICnnManager : IDisposable
    {
        //void AfterStatement();
        void AfterTransaction();
        ITransaction BeginTransaction();
        ITransaction BeginTransaction(IsolationLevel isolationLevel);
        IDbConnection Close();
        string CnnStrName { get; }
        //IDbConnection Connection { get; }
        //IDbCommand CreateCommand();
        IDbConnection Disconnect();
        void FlushBeginning();
        void FlushEnding();
        IDisposable FlushingFromDtcTransaction { get; }
        IDbConnection GetConnection();
        bool IsConnected { get; }
        bool IsInActiveTransaction { get; }
        void Reconnect();
        bool SupportsMultipleOpenReaders { get; }
        bool SupportsMultipleQueries { get; }
        string ToString();
        ITransaction Transaction { get; }

        //void CommandReader_Done(object p_pk);      
    }
}
