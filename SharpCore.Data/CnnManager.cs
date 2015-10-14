using System;
using System.Data;
using System.Data.SqlClient;
//using SharpCore.Logging;


namespace SharpCore.Data
{
    //[Serializable]
    public class CnnManager: ICnnManager
    {
        private bool m_disposed = false;

        [NonSerialized]
        private IDbConnection m_connection;

        [NonSerialized]
        private ITransaction transaction;

        [NonSerialized]
        private bool isFlushing;

        private bool ownConnection; // Whether we own the connection, i.e. connect and disconnect automatically.        

        private readonly ConnectionReleaseMode connectionReleaseMode;

        private bool flushingFromDtcTransaction;
        
        private ISessionTX m_session;

        internal ISessionTX Session
        {
            get { return this.m_session; }
        }

       

        public string CnnStrName
        {
            get { return this.m_session.SessionFactory.CnnName; }
        }
       
        public bool IsConnected
		{
            get { return this.m_connection!= null || this.ownConnection; }
		}

        public void Reconnect()
        {
            if(this.IsConnected) throw new Exception("session already connected");            
            ownConnection= true;
        }

        public bool IsInActiveTransaction
        {
            get
            {
                if (transaction != null && transaction.IsActive)
                    return true;
                else
                    return false; //session.Factory.TransactionFactory.IsInDistributedActiveTransaction(session);
            }
        }

      

        #region Expuestas por el manager... Sirven de envoltorio (wrapper) a las específicas para MSSQL de abajo

        public IDbConnection GetConnection()
        {            
            if(this.m_connection == null)
            {
                if(this.ownConnection)
                {
                    this.m_connection = this.m_session.SessionFactory.GetConnection_MSSQL();                     
                    //SharpLogger.Nfo("GetConnection() -> Hash 0x{0:X} instanciada a través del driver MSSQL por el Cnnmanager. Status: {1}", this.m_connection.GetHashCode(), this.m_connection.State);
                     (this.m_connection as SqlConnection).StateChange += new StateChangeEventHandler(CnnManager_StateChange);
                }
                else if (this.m_session.IsOpen)
                {
                    throw new Exception("Session is currently disconnected");
                }
                else
                {
                    throw new Exception("Session is closed");
                }
            }                        
            return this.m_connection;
        }
        
        public bool SupportsMultipleOpenReaders
        {
            get { return SupportsMultipleOpenReaders_MSSQL; }
        }

        public bool SupportsMultipleQueries
        {
            get { return SupportsMultipleQueries_MSSQL; }
        }

        #endregion

        #region Las anteriores, privadas personalizadas para MSSQL

        /// <summary>
        /// The SqlClient driver does NOT support more than 1 open IDataReader
        /// with only 1 IDbConnection.
        /// </summary>
        /// <value><see langword="false" /> - it is not supported.</value>
        /// <remarks>
        /// MS SQL Server 2000 (and 7) throws an exception when multiple IDataReaders are
        /// attempted to be opened.  When SQL Server 2005 comes out a new driver will be
        /// created for it because SQL Server 2005 is supposed to support it.
        /// </remarks>
        private bool SupportsMultipleOpenReaders_MSSQL
        {
            get { return false; }
        }

        private bool SupportsMultipleQueries_MSSQL
        {
            get { return true; }
        }

        #endregion

        public void AfterStatement()
        {
            //SharpLogger.CallerIn();

            if (IsAggressiveRelease)
            {
                if (isFlushing)
                {
                    //SharpLogger.Nfo("skipping aggressive-release due to flush cycle");
                }
                //else if (batcher.HasOpenResources)
                //{
                //    log.Debug("skipping aggressive-release due to open resources on batcher");
                //}
                //// TODO H3:
                ////else if (borrowedConnection != null)
                ////{
                ////    log.Debug("skipping aggressive-release due to borrowed connection");
                ////}
                else
                {
                    AggressiveRelease();
                }
            }

            //SharpLogger.CallerOut();
        }

        public void AfterTransaction()
        {
            //SharpLogger.CallerIn();

            if (IsAfterTransactionRelease)
            {
                AggressiveRelease();
            }
            else if (IsAggressiveRelease && false) //&& batcher.HasOpenResources)
            {
                //SharpLogger.Nfo("forcing batcher resource cleanup on transaction completion; forgot to close ScrollableResults/Enumerable?");
                //batcher.CloseCommands();
                AggressiveRelease();
            }
            else if (IsOnCloseRelease)
            {
                // log a message about potential connection leaks
                //SharpLogger.Nfo("transaction completed on session with on_close connection release mode; be sure to close the session to release ADO.Net resources!");
            }

            transaction = null;

            //SharpLogger.CallerOut();
        }

        private void AggressiveRelease()
        {
            TraceLog.LogEntry(this, "AggressiveRelease()");
            
            if (ownConnection && flushingFromDtcTransaction == false)
            {
                //SharpLogger.Nfo("aggressively releasing database connection");

                if (this.m_connection != null)
                {
                    this.CloseConnection();
                }
            }    
        

            //SharpLogger.CallerOut();
        }

        public IDisposable FlushingFromDtcTransaction
        {
            get
            {
                flushingFromDtcTransaction = true;
                return new StopFlushingFromDtcTransaction(this);
            }
        }

        public void FlushBeginning()
        {
            //SharpLogger.Nfo("registering flush begin");
            isFlushing = true;
        }

        public void FlushEnding()
        {
            //SharpLogger.Nfo("registering flush end");
            isFlushing = false;
            AfterStatement();
        }

        private bool IsAfterTransactionRelease
        {
            get { return connectionReleaseMode == ConnectionReleaseMode.AfterTransaction; }
        }

        private bool IsOnCloseRelease
        {
            get { return connectionReleaseMode == ConnectionReleaseMode.OnClose; }
        }

        private bool IsAggressiveRelease
        {
            get
            {
                if (connectionReleaseMode == ConnectionReleaseMode.AfterTransaction)
                {
                    return !IsInActiveTransaction;
                }
                return false;
            }
        }
    
        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            Transaction.Begin(isolationLevel);
            return transaction;
        }

        public ITransaction BeginTransaction()
        {
            Transaction.Begin();
            return transaction;
        }

        public ITransaction Transaction
        {
            get
            {
                if (transaction == null)
                {
                    transaction = new AdoTransaction(this.m_session);
                    //session.Factory.TransactionFactory.CreateTransaction(session);
                }
                return transaction;
            }
        }

        ~CnnManager()
        {

            if (this.m_session != null)
                TraceLog.LogEntry("~CnnManager(): m_session {0} m_disposed: {1}", this.m_session.ToString(), this.m_disposed);
            else
                TraceLog.LogEntry("~CnnManager()  m_session NULL m_disposed: {0}", this.m_disposed);

            this.Dispose(false);            
        }
       
        /// <summary>
        /// Manejador de conexiones contra la BBDD
        /// </summary>
        /// <param name="p_ses">Session que contiene este Connection Manager</param>
        /// <param name="p_connectionReleaseMode">Pues eso...</param>
        internal CnnManager(ISessionTX p_ses, ConnectionReleaseMode p_connectionReleaseMode)
        {
            IDbConnection suppliedConnection = null;


            TraceLog.LogEntry("CnnManager(): p_ses= {0} p_connectionReleaseMode= {1}", p_ses.SessionId, p_connectionReleaseMode);
           
            this.m_session = p_ses;
            this.m_connection = suppliedConnection;
            this.connectionReleaseMode = p_connectionReleaseMode;            

            ownConnection = (suppliedConnection == null);   // De esta forma, siempre será True.

            //SharpLogger.CallerOut();
        }

        static void CnnManager_StateChange(object sender, StateChangeEventArgs e)
        {
            TraceLog.LogEntry("*** CnnManager_StateChange(SqlConnection: 0x{0:X}) {1} -> {2}", sender.GetHashCode(), e.OriginalState, e.CurrentState);
        }        

        public IDbConnection Close()
        {
            //SharpLogger.CallerIn();

            IDbConnection res = null;
         
            if (transaction != null)
            {
                transaction.Dispose();
            }

            // When the connection is null nothing needs to be done - if there
            // is a value for connection then Disconnect() was not called - so we
            // need to ensure it gets called.
            if (m_connection == null)
            {
                ownConnection = false;
                res = null;
            }
            else
            {
                
                res= Disconnect();
            }

            if (this.m_session != null)
            {
                //SharpLogger.Nfo("m_session NO NULL.. se hará NULL ahora.");
                this.m_session = null;
            }

            //SharpLogger.CallerOut();
            return res;  
        }

        private void CloseConnection()
        {
            //SharpLogger.CallerIn();
            
            if (this.m_connection != null)
            {
                if (this.m_connection.State != ConnectionState.Closed) this.m_connection.Close();

                this.m_connection.Dispose();
                this.m_connection = null;
            }

            //SharpLogger.CallerOut();
        }

        public IDbConnection Disconnect()
        {
            //SharpLogger.CallerIn();
            
            if (IsInActiveTransaction)
                throw new InvalidOperationException("Disconnect cannot be called while a transaction is in progress.");

            try
            {              
                DisconnectOwnConnection();
                ownConnection = false;

                return null;             
            }
            finally
            {
                // Ensure that AfterTransactionCompletion gets called since
                // it takes care of the locks and cache.
                if (!IsInActiveTransaction)
                {
                    // We don't know the state of the transaction
                    this.m_session.AfterTransactionCompletion(false, null);
                }

                //SharpLogger.CallerOut();
            }

           
        }

       

        private void DisconnectOwnConnection()
        {
            //SharpLogger.CallerIn();

            if (this.m_connection == null)
            {
                return;  // No active connection
            }
          
            CloseConnection();

            //SharpLogger.CallerOut();
        }
   

        public override string ToString()
        {
            if (this.m_session != null)
            {
                if (this.transaction != null)
                    return "m_session: " + this.m_session.ToString() + ":" + String.Format("0x{0:X}", this.transaction.GetHashCode());
                else
                    return this.m_session.ToString() + ":" + "transaction: NULL";
            }
            else
                return "m_session: NULL";
        }
      
        private void Dispose(bool p_disposing)
        {
            // Check to see if Dispose has already been called.

            TraceLog.LogEntry("CnnManager.Dispose(disposing= {0})", p_disposing);

            if (!m_disposed)
            {
                this.Close();

                //if (this.m_session != null)
                //    SharpLogger.Nfo("Dispose(): m_session {0}", this.m_session.ToString());
                ////else
                ////    SharpLogger.Nfo("Dispose()  m_session NULL");

                //if (this.m_connection != null)
                //    SharpLogger.Nfo("Dispose(): m_connection {0}", this.m_connection.GetHashCode());
                ////else
                ////    SharpLogger.Nfo("Dispose()  m_connection NULL");

                //if (this.transaction != null)
                //    SharpLogger.Nfo("Dispose(): transaction 0x{0:X}", this.transaction.GetHashCode());
                //else
                //    SharpLogger.Nfo("Dispose() transaction NULL");

                GC.SuppressFinalize(this);  // nothing for Finalizer to do - so tell the GC to ignore it

                this.m_disposed = true;
            }
            //else
                //SharpLogger.Nfo("instance already disposed...");

            //SharpLogger.CallerOut();
        }


        private class StopFlushingFromDtcTransaction : IDisposable
        {
            private readonly CnnManager manager;

            public StopFlushingFromDtcTransaction(CnnManager manager)
            {
                this.manager = manager;
            }

            public void Dispose()
            {
                manager.flushingFromDtcTransaction = false;
            }
        }        

        public void Dispose()
        {
            //SharpLogger.CallerIn();

            this.Dispose(true);

            //SharpLogger.CallerOut();

        }

        //public void CommandReader_Done(object p_pk)
        //{
        //    // Raise the event by using the () operator.

        //    if (CommandReaderDone_event != null)
        //        CommandReaderDone_event(p_pk);

        //}

        //public delegate void CommandReaderDone(object p_pk);
        //public event CommandReaderDone CommandReaderDone_event;


        //public ISessionFactory Factory
        //{
        //    get { return m_session.SessionFactory; }
        //}

    }
}
