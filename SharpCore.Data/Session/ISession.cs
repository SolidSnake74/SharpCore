using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;


namespace SharpCore.Data
{
    public interface ISession : IDisposable
    {
        Guid SessionId { get; }
        string CnnStrName { get; }
        string CnnString { get; }

        IDbConnection Connection { get; }

        /// <summary>
        /// Disconnect the <c>ISession</c> from the current ADO.NET connection.
        /// </summary>
        /// <remarks>
        /// If the connection was obtained by Hibernate, close it or return it to the connection
        /// pool. Otherwise return it to the application. This is used by applications which require
        /// long transactions.
        /// </remarks>
        /// <returns>The connection provided by the application or <see langword="null" /></returns>
        IDbConnection Disconnect();

        /// <summary>
        /// End the <c>ISession</c> by disconnecting from the ADO.NET connection and cleaning up.
        /// </summary>
        /// <remarks>
        /// It is not strictly necessary to <c>Close()</c> the <c>ISession</c> but you must
        /// at least <c>Disconnect()</c> it.
        /// </remarks>
        /// <returns>The connection provided by the application or <see langword="null" /></returns>
        IDbConnection Close();


        /// <summary>
        /// Get the <see cref="ISessionFactory" /> that created this instance.
        /// </summary>
        ISessionFactory SessionFactory { get; }

        bool IsConnected { get; }

        /// <summary> 
        /// Determine whether the session is closed.  Provided separately from
        /// {@link #isOpen()} as this method does not attempt any JTA synch
        /// registration, where as {@link #isOpen()} does; which makes this one
        /// nicer to use for most internal purposes. 
        /// </summary>
        /// <returns> True if the session is closed; false otherwise.
        /// </returns>
        bool IsClosed { get; }
       
        #region declarados como abstract para quien implemente ISession

        bool IsAutoCloseSessionEnabled { get; }
        bool ShouldAutoClose { get; }

        TransactionScope GetTransactScope();
        TransactionScope GetTransactScope(long tOut_mseg);

        void AfterTransactionBegin(ITransaction tx);
        void AfterTransactionCompletion(bool successful, ITransaction tx);

        T GetDAO<T>() where T:BaseDAL;

        T GetBAL<T>() where T:BaseDAL;

        #endregion


        //short RootNode { get; }
        //List<short> ServersLoaded { get; }

    }    
}
