using System;
using System.Data;
using System.Transactions;

namespace SharpCore.Data
{

    internal class SessionExt: Session, ISessionTX
    {            
        private CnnManager m_cnnMngr = null;

        [NonSerialized]
        private readonly bool autoCloseSessionEnabled;
        [NonSerialized]
        private readonly ConnectionReleaseMode connectionReleaseMode;

        public override ISessionFactory SessionFactory
        {
            get { return this.m_sesFactory; }   
        }

        internal SessionExt(ISessionFactory p_sesFact, bool autoCloseSessionEnabled, ConnectionReleaseMode connectionReleaseMode)
            : base(p_sesFact)
        {
            // SharpLogger.CallerIn();

            this.autoCloseSessionEnabled = autoCloseSessionEnabled;
            this.connectionReleaseMode = connectionReleaseMode;

            this.m_cnnMngr = new CnnManager(this, connectionReleaseMode);

            TraceLog.LogEntry("SessionExt(): Session Id: {0}", this.sessionId);

            CheckAndUpdateSessionStatus();
          
      
            //this.ServersLoad();

            //// SharpLogger.CallerOut();
        }



        public override bool IsAutoCloseSessionEnabled
        {
            get { return autoCloseSessionEnabled; }
        }

        public override bool ShouldAutoClose
        {
            get { return IsAutoCloseSessionEnabled && !IsClosed; }
        }

        protected internal void CheckAndUpdateSessionStatus()
        {
            ErrorIfClosed();
            EnlistInAmbientTransactionIfNeeded();
        }

        public override bool TransactionInProgress
        {
            get
            {
                return !IsClosed && Transaction.IsActive;
            }
        }

        protected void EnlistInAmbientTransactionIfNeeded()
        {
            // nothing need to do here, we only support local transactions with this factory

            //factory.TransactionFactory.EnlistInDistributedTransactionIfNeeded(this);            
        }

        /// <summary>
        /// Demark beggining of an scope... No timeout defined.
        /// </summary>
        /// <returns></returns>
        public override TransactionScope GetTransactScope()
        {
            return GetTransactScope(0);
        }

        /// <summary>
        ///  Demark beggining of an scope.
        /// </summary>
        /// <param name="tOut_mseg">Set timeout for Scope in mseg... zero means infinite (no timeout)</param>
        /// <returns></returns>
        public override TransactionScope GetTransactScope(long tOut_mseg)
        {           
            return new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMilliseconds(tOut_mseg));
        }
        
        public override T GetDAO<T>()
        {            
            return (T)Activator.CreateInstance(typeof(T), this); 
        }

        public override T GetBAL<T>()
        {
            return (T)Activator.CreateInstance(typeof(T), this); 
        }

        public override bool IsConnected
        {
            get 
            {
                if (this.m_cnnMngr != null)
                    return this.m_cnnMngr.IsConnected;
                else
                    return false;
            }
        }

        public ICnnManager ConnectionManager
        {
            get { return this.m_cnnMngr; }
        }

        public override IDbConnection Connection
        {
            get
            {
                if (this.m_cnnMngr == null)
                    return null;
                else
                    return this.m_cnnMngr.GetConnection();
            }
        }

        public bool IsOpen
        {
            get { return !closed; }
        }
     
        public override IDbConnection Disconnect()
        {
            IDbConnection res = null;

            // SharpLogger.CallerIn();

            CheckAndUpdateSessionStatus();
            TraceLog.LogEntry(this, "disconnecting session");
            res= this.m_cnnMngr.Disconnect();
            
            // SharpLogger.CallerOut();

            return res;
        }

        /// <summary>
        /// Close the session and release all resources
        /// <remarks>
        /// Do not call this method inside a transaction scope, use <c>Dispose</c> instead, since
        /// Close() is not aware of distributed transactions
        /// </remarks>
        /// </summary>
        public override IDbConnection Close()
        {
            //using (new SessionIdLoggingContext(SessionId))
            //{
                TraceLog.LogEntry(this, "closing session");
                if (IsClosed) throw new Exception("Session was already closed");
               

                //if (Factory.Statistics.IsStatisticsEnabled)
                //{
                //    Factory.StatisticsImplementor.CloseSession();
                //}

                try
                {
                    try
                    {
                        //if (childSessionsByEntityMode != null)
                        //{
                        //    foreach (KeyValuePair<EntityMode, ISession> pair in childSessionsByEntityMode)
                        //    {
                        //        pair.Value.Close();
                        //    }
                        //}
                    }
                    catch
                    {
                        // just ignore
                    }

                    
                        return this.m_cnnMngr.Close();
                   
                }
                finally
                {
                    
                    SetClosed();
                              

                    // SharpLogger.CallerOut();
                }
            //}
        }

        protected internal void ErrorIfClosed()
        {
            if (IsClosed || IsAlreadyDisposed)
            {
                throw new ObjectDisposedException("ISession", "Session is closed!");
            }
        }
      
        public ITransaction Transaction
        {
            get             
            {
                if (this.m_cnnMngr == null)
                    return null;
                else
                    return this.m_cnnMngr.Transaction;
            }
                
        }

        public ITransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            //using (new SessionIdLoggingContext(SessionId))
            //{
            //    if (rootSession != null)
            //    {
            // Todo : should seriously consider not allowing a txn to begin from a child session
            //      can always route the request to the root session...
            //    // SharpLogger.Nfo("Transaction started on non-root session");
            //}

            CheckAndUpdateSessionStatus();
            return this.m_cnnMngr.BeginTransaction(isolationLevel);
            //}
        }

        public ITransaction BeginTransaction()
        {
            //using (new SessionIdLoggingContext(SessionId))
            //{
            //if (rootSession != null)
            //{
            //    // Todo : should seriously consider not allowing a txn to begin from a child session
            //    //      can always route the request to the root session...
            //    // SharpLogger.Nfo("Transaction started on non-root session");
            //}

            CheckAndUpdateSessionStatus();
            return this.m_cnnMngr.BeginTransaction();
            //}
        }

        public ISession GetSessionImplementation()
        {
            return this;
        }

        public override void AfterTransactionBegin(ITransaction tx)
        {

            CheckAndUpdateSessionStatus();
            //interceptor.AfterTransactionBegin(tx);            
        }

        /// <summary>
        /// Ensure that the locks are downgraded 
        /// and that all of the softlocks in the caché has been released.
        /// </summary>
        public override void AfterTransactionCompletion(bool success, ITransaction tx)
        {
            if (tx != null)
                TraceLog.LogEntry("AfterTransactionCompletion(): success {0},tx 0x{1:X}", success, tx.GetHashCode());
            else
                TraceLog.LogEntry("AfterTransactionCompletion(): success {0},tx NULL", success);

            //using (new SessionIdLoggingContext(SessionId))
            //{
            //    log.Debug("transaction completion");
            //    if (Factory.Statistics.IsStatisticsEnabled)
            //    {
            //        Factory.StatisticsImplementor.EndTransaction(success);
            //    }

            this.m_cnnMngr.AfterTransaction();

            //    persistenceContext.AfterTransactionCompletion();
            //    actionQueue.AfterTransactionCompletion(success);
            //    if (rootSession == null)
            //    {
            //        try
            //        {
            //            interceptor.AfterTransactionCompletion(tx);
            //        }
            //        catch (Exception t)
            //        {
            //            log.Error("exception in interceptor afterTransactionCompletion()", t);
            //        }
            //    }


            //    //if (autoClear)
            //    //	Clear();
            //}

            //// SharpLogger.CallerOut();
        }

        #region System.IDisposable Members





        /// <summary>
        /// Finalizer that ensures the object is correctly disposed of.
        /// </summary>
        ~SessionExt()
        {
            //if (this.sessionId != null)
            //    // SharpLogger.Nfo("~SessionExt(): sessionId: {0}", base.sessionId.ToString());
            //else
            //    // SharpLogger.Nfo("~SessionExt(): NULL");

            this.Dispose(false);
        }

        /// <summary>
        /// Perform a soft (distributed transaction aware) close of the session
        /// </summary>

        public override void Dispose()
        {
            // SharpLogger.CallerIn();

            // using (new SessionIdLoggingContext(SessionId))
            // {
            try
            {
                CurrentSessionContext.Unbind(SessionFactory);

                if (TransactionContext != null)
                {
                    TransactionContext.ShouldCloseSessionOnDistributedTransactionCompleted = true;
                    return;
                }
            }
            finally
            {
                this.Dispose(true);
            }

            //}

            // SharpLogger.CallerOut();
        }

        private void Dispose(bool p_disposing)
        {
            // Check to see if Dispose has already been called.

            //// SharpLogger.CallerIn("p_disposing= " + p_disposing);



            if (IsAlreadyDisposed)
            {
                // don't dispose of multiple times.

                // SharpLogger.Nfo("[session-id={0}] Is Already Disposed! p_disposing={1}", this.sessionId, p_disposing);
                return;
            }

            // SharpLogger.Nfo("[session-id={0}] executing real Dispose({1})", this.sessionId, p_disposing);

            // free managed resources that are being managed by the session if we
            // know this call came through Dispose()
            if (p_disposing && !IsClosed)
            {
                Close();
            }

            // free unmanaged resources here

            if (this.m_cnnMngr != null)
            {
                //// SharpLogger.Nfo("this.m_cnnMngr: {0} se le hará Dispose e igualará a NULL ahora...",this.m_cnnMngr.ToString());

                this.m_cnnMngr.Dispose();
                this.m_cnnMngr = null;
            }
            else
                // SharpLogger.Nfo("this.m_cnnMngr: NULL");

                //if (m_persLoaded!= null) this.m_persLoaded.Clear();
                //this.m_persLoaded= null;

                IsAlreadyDisposed = true;
            // nothing for Finalizer to do - so tell the GC to ignore it
            GC.SuppressFinalize(this);

            //// SharpLogger.CallerOut();
        }

        #endregion

        /////<summary>
        /////Get the server collection to memory.
        /////</summary>
        //private void ServersLoad()
        //{
        //    // SharpLogger.CallerIn();

        //    if (this.m_servidores == null)
        //    {
        //        this.m_servidores = new List<short>();

        //        using(SqlDataReader dataReader = SqlClientUtility.ExecuteReader(this.m_cnnMngr,SqlClientUtility.CreateCommand(this.m_cnnMngr,CommandType.StoredProcedure, "[DTier4].[ServersSelectAll]")))
        //        {

        //            while (dataReader.Read()) this.m_servidores.Add((short)((int)(dataReader["Numero"])));
        //            dataReader.Close();

        //            //Checking the private member 'm_Central', we easily can say about server ubication of the DAO, it is leaf type
        //            //and get code of node server at same time (value is not -1), otherwise(-1) means current is the one and only Node-Server.

        //            if (this.m_servidores.Count == 1)
        //            {
        //                short srv_cod = this.m_servidores[0]; // (short)

        //                if (srv_cod == 0)
        //                    this.m_Central = srv_cod;   // This DAO runs over leaf-node... points and syncs. to root-node, code Zero...
        //                else
        //                    this.m_Central = -1;        // This DAO runs over root-node... Every node sync against it.
        //            }
        //            else
        //                this.m_Central = -1;            // This DAO runs over root-node... Every leaf-node sync against it. The ArrayList contens all of them.                 
        //        }
        //    }

        //    // SharpLogger.CallerOut();
        //} 
    }
}
