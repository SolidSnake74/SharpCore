using System;
using System.Data;


namespace SharpCore.Data
{
    public interface ISessionTX: ISession, IDisposable
    {        

        bool IsOpen { get; }
       
        ITransaction Transaction { get; }
        
        ICnnManager ConnectionManager { get; }

        bool TransactionInProgress { get; }
        ITransactionContext TransactionContext { get; set; }
        
        ITransaction BeginTransaction(IsolationLevel isolationLevel);
        ITransaction BeginTransaction();        
    }
}
