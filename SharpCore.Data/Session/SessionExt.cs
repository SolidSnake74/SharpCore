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

            this.CheckAndUpdateSessionStatus();
          
      
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
            TraceLog.LogEntry("SessionExt.CheckAndUpdateSessionStatus()");
            this.ErrorIfClosed();
            this.EnlistInAmbientTransactionIfNeeded();
        }

        public override bool TransactionInProgress
        {
            get
            {
                return !this.IsClosed && this.Transaction.IsActive;
            }
        }

        public bool IsInActiveTransaction
        {
            get
            {
                if (this.m_cnnMngr == null)
                    return false;
                else
                    return this.m_cnnMngr.IsInActiveTransaction;
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
        public TransactionScope GetTransactScope()
        {
            return this.GetTransactScope(0);
        }

        /// <summary>
        ///  Demark beggining of an scope.
        /// </summary>
        /// <param name="tOut_mseg">Set timeout for Scope in mseg... zero means infinite (no timeout)</param>
        /// <returns></returns>
        public TransactionScope GetTransactScope(long tOut_mseg)
        {                       
            TransactionScope ts= new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMilliseconds(tOut_mseg));
            TraceLog.LogEntry("SessionExt.GetTransactScope(tOut_mseg= {0}): SessionId= {1}", tOut_mseg, this.sessionId);                        
            return ts;            
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

        internal ICnnManager ConnectionManager
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
            get { return !this.closed; }
        }
     
        public override IDbConnection Disconnect()
        {            
            TraceLog.LogEntry("SessionExt.Disconnect(): SessionId= {0}",this.sessionId);
            this.CheckAndUpdateSessionStatus();            
            TraceLog.LogEntry("SessionExt.Disconnect(): disconnecting session");
            return this.m_cnnMngr.Disconnect();
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
                TraceLog.LogEntry("SessionExt.Close(): SessionId= {0}", base.sessionId);

                if (this.IsClosed) throw new Exception("Session was already closed");
               

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
                    
                    base.SetClosed();
                              

                    // SharpLogger.CallerOut();
                }
            //}
        }

        protected internal void ErrorIfClosed()
        {
            if (this.IsClosed || this.IsAlreadyDisposed)
            {
                throw new ObjectDisposedException("ISession", "Session is closed!");
            }
        }
      
        internal ITransaction Transaction
        {
            get             
            {
                if (this.m_cnnMngr == null)
                    return null;
                else
                    return this.m_cnnMngr.Transaction;
            }
                
        }

        internal ITransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {           
            //using (new SessionIdLoggingContext(SessionId))
            //{
            //    if (rootSession != null)
            //    {
            // Todo : should seriously consider not allowing a txn to begin from a child session
            //      can always route the request to the root session...
            //    // SharpLogger.Nfo("Transaction started on non-root session");
            //}

            TraceLog.LogEntry("SessionExt.BeginTransaction(" + Enum.GetName(typeof(System.Data.IsolationLevel), isolationLevel) + ")");
            this.CheckAndUpdateSessionStatus();
            return this.m_cnnMngr.BeginTransaction(isolationLevel);
            //}
        }

        internal ITransaction BeginTransaction()
        {            
            //using (new SessionIdLoggingContext(SessionId))
            //{
            //if (rootSession != null)
            //{
            //    // Todo : should seriously consider not allowing a txn to begin from a child session
            //    //      can always route the request to the root session...
            //    // SharpLogger.Nfo("Transaction started on non-root session");
            //}

            TraceLog.LogEntry("SessionExt.BeginTransaction()");
            this.CheckAndUpdateSessionStatus();
            return this.m_cnnMngr.BeginTransaction();
            //}
        }

        public override void AfterTransactionBegin(ITransaction tx)
        {
            TraceLog.LogEntry("SessionExt.AfterTransactionBegin(tx 0x{0:X})", (tx == null) ? -1 : tx.GetHashCode());
            this.CheckAndUpdateSessionStatus();
            //interceptor.AfterTransactionBegin(tx);            
        }

        /// <summary>
        /// Ensure that the locks are downgraded 
        /// and that all of the softlocks in the caché has been released.
        /// </summary>
        public override void AfterTransactionCompletion(bool success, ITransaction tx)
        {

            TraceLog.LogEntry("SessionExt.AfterTransactionCompletion(success {0}, tx 0x{1:X})", success, (tx == null) ? -1 : tx.GetHashCode());

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



            if (this.IsAlreadyDisposed)
            {
                // don't dispose of multiple times.

                // SharpLogger.Nfo("[session-id={0}] Is Already Disposed! p_disposing={1}", this.sessionId, p_disposing);
                return;
            }

            // SharpLogger.Nfo("[session-id={0}] executing real Dispose({1})", this.sessionId, p_disposing);

            // free managed resources that are being managed by the session if we
            // know this call came through Dispose()
            if (p_disposing && !this.IsClosed)
            {
                this.Close();
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

        //public ISession GetSessionImplementation()
        //{
        //    return this;
        //}
    }
}
