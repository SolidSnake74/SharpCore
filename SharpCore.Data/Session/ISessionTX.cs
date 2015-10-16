using System;
using System.Data;
using System.Transactions;

namespace SharpCore.Data
{
    public interface ISessionTX: ISession, IDisposable
    {        

        bool IsOpen { get; }

        //ICnnManager ConnectionManager { get; }

        bool TransactionInProgress { get; }

        bool IsInActiveTransaction { get; }

        ITransactionContext TransactionContext { get; set; }


        TransactionScope GetTransactScope();
        TransactionScope GetTransactScope(long tOut_mseg);

        //ITransaction Transaction { get; }
        //ITransaction BeginTransaction(System.Data.IsolationLevel isolationLevel);
        //ITransaction BeginTransaction();        
    }
}
