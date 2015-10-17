using System;
using System.Data;
//using SharpCore.Logging;


namespace SharpCore.Data
{
    internal class AdoTransaction: ITransaction
    {
        ISessionTX session = null;

        //private Guid sessionId;

        bool begun = false;
        bool committed = false;
        bool rolledBack = false;
        bool commitFailed = false;
        IDbTransaction trans = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoTransaction"/> class.
        /// </summary>
        /// <param name="p_ses"></param>
        internal AdoTransaction(ISessionTX p_ses)
        {
            //SharpLogger.Nfo("* AdoTransaction() construction. p_ses= {0}", p_ses.ToString());
            
            this.session = p_ses;
          
            //SharpLogger.CallerOut();
        }

        public void Begin()
        {            
            this.Begin(IsolationLevel.Unspecified);           
        }

        public void Begin(IsolationLevel isolationLevel)
        {
            
            if (begun)
            {
                return;
            }

            if (this.commitFailed)
            {
                throw new System.Transactions.TransactionException("Cannot restart transaction after failed commit");
            }

            if (isolationLevel == IsolationLevel.Unspecified)
            {
                isolationLevel = IsolationLevel.Serializable; //session.Factory.Settings.IsolationLevel;
            }
            
            try
            {
                IDbConnection m_DbConn = session.Connection;

                TraceLog.LogEntry("AdoTransaction.Begin(): {0} DbConnection 0x{1:X} State: {2}", isolationLevel, m_DbConn.GetHashCode(), m_DbConn.State);

                if (m_DbConn.State != ConnectionState.Open)
                {
                    //TraceLog.LogEntry("Se procederá a abrir la conexión...");
                    m_DbConn.Open();
                }

                if (isolationLevel == IsolationLevel.Unspecified)                
                    trans = m_DbConn.BeginTransaction();                
                else                
                    trans = m_DbConn.BeginTransaction(isolationLevel);
                
                begun = true;
                committed = false;
                rolledBack = false;

            }
            catch (Exception e)
            {                
                throw new System.Transactions.TransactionException("Begin Transaction failed with SQL exception: ", e);
            }

            this.session.AfterTransactionBegin(this);            
        }


        public void Commit()
        {
            string m_hashc="NULL";
            if(this.trans!=null) m_hashc=String.Format("0x{0:X}",this.trans.GetHashCode());

            //SharpLogger.Nfo("*** Transaction {0} Commit...",m_hashc);

            //using (new SessionIdLoggingContext(sessionId))
            //{

                this.CheckNotDisposed();
                this.CheckBegun();
                this.CheckNotZombied();

                //if (session.FlushMode != FlushMode.Never)
                //{
                //    session.Flush();
                //}

                //NotifyLocalSynchsBeforeTransactionCompletion();
                //session.BeforeTransactionCompletion(this);

                this.commitFailed = false;

                try
                {
                    this.trans.Commit();
                    this.committed = !this.commitFailed;
                    TraceLog.LogEntry("Trans. " + m_hashc + " Committed!");                                       
                }
                catch (Exception ex)
                {                                      
                    //SharpLogger.Error(e,String.Format("Commit Transaction {0} failed with SQL exception",m_hashc));                    
                    this.committed = !(this.commitFailed = true);

                    // Don't wrap SharpCore.Data Exceptions:
                    throw new System.Transactions.TransactionException("Commit failed.", ex);
                }     
                finally
                {
                    this.AfterTransactionCompletion(committed && !this.commitFailed);
                    this.Dispose();
                    //this.CloseIfRequired();
                }
            //}
        }

        public void Rollback()
        {
            string m_hashc = "NULL";
            if (this.trans != null) m_hashc = String.Format("0x{0:X}", this.trans.GetHashCode());

            TraceLog.LogEntry("AdoTransaction.Rollback(): Transaction " + m_hashc + "...");    

            this.CheckNotDisposed();
            this.CheckBegun();
            this.CheckNotZombied();

            if (this.commitFailed)
                TraceLog.LogEntry(this, "Rollback(): Skipped rollbacking transaction " + m_hashc + " because this.commitFailed is True.");
            else
            {
                try
                {
                    this.trans.Rollback();
                    this.rolledBack = true;                   
                }
                catch (Exception e)
                {
                    //SharpLogger.Error(e);
                    // Don't wrap HibernateExceptions
                    throw new System.Transactions.TransactionException(String.Format("Rollback Transaction {0} failed with SQL exception", m_hashc), e);
                }
                finally
                {
                    this.AfterTransactionCompletion(this.rolledBack);                    
                    this.Dispose();
                    //this.CloseIfRequired();

                    if (this.rolledBack) 
                        TraceLog.LogEntry(this, "Rollback(): Transaction" + m_hashc + " succesfully RollBacked.");    
                    else
                        TraceLog.LogEntry(this, "Rollback(): Transaction" + m_hashc + " failed.");    
                }
            }                               
        }

        private void AfterTransactionCompletion(bool successful)
        {
            //TraceLog.LogEntry("* AfterTransactionCompletion(): Hash= 0x{0:X} successful= {1} ", this.trans.GetHashCode(), successful);

            //using (new SessionIdLoggingContext(sessionId))
            {
                this.session.AfterTransactionCompletion(successful, this);
                this.NotifyLocalSynchsAfterTransactionCompletion(successful);
                this.session = null;
                this.begun = false;
            }

           
        }

        
        

        	/// <summary>
		/// Gets a <see cref="Boolean"/> indicating if the transaction was rolled back.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the <see cref="IDbTransaction"/> had <c>Rollback</c> called
		/// without any exceptions.
		/// </value>
		public bool WasRolledBack
		{
            get { return this.rolledBack; }
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> indicating if the transaction was committed.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the <see cref="IDbTransaction"/> had <c>Commit</c> called
		/// without any exceptions.
		/// </value>
		public bool WasCommitted
		{
            get { return this.committed; }
		}

		public bool IsActive
		{
            get { return this.begun && !this.rolledBack && !this.committed; }
		}

        /// <summary>
        /// When internal trans. instance is null, returns always 'Unspecified'...
        /// </summary>
		public IsolationLevel IsolationLevel
		{
			get 
            {
                if (this.trans == null)
                    return IsolationLevel.Unspecified;                
                else
                    return this.trans.IsolationLevel; 
            }
		}

        //void CloseIfRequired()
        //{
        //    // Eliminar...
        //}

        /// <summary>
        /// Enlist the <see cref="IDbCommand"/> in the current <see cref="ITransaction"/>.
        /// </summary>
        /// <param name="command">The <see cref="IDbCommand"/> to enlist in this Transaction.</param>
        /// <remarks>
        /// <para>
        /// This takes care of making sure the <see cref="IDbCommand"/>'s Transaction property 
        /// contains the correct <see cref="IDbTransaction"/> or <see langword="null" /> if there is no
        /// Transaction for the ISession - ie <c>BeginTransaction()</c> not called.
        /// </para>
        /// <para>
        /// This method may be called even when the transaction is disposed.
        /// </para>
        /// </remarks>
        public void Enlist(IDbCommand command)
        {            
            if (this.trans == null)
            {
                if (command.Transaction != null)
                {
                    TraceLog.LogEntry("AdoTransaction.EnList('{1}'): Found NOT-NULL preexistent IDbCommand.Transaction (0x{0:X}), it will be setted to NULL,  because attached Session had no Transaction.", command.Transaction.GetHashCode(), command.CommandText);
                    command.Transaction = null;                
                }
                else
                    TraceLog.LogEntry("AdoTransaction.EnList('{0}'): Will be null setted the Non-Null preexistent IDbCommand.Transaction (NULL) because the Session had no Transaction", command.CommandText);                
            }
            else
            {
                // got into here because the command was being initialized and had a null Transaction - probably
                // don't need to be confused by that - just a normal part of initialization...
                               
                if (command.Transaction == this.trans)
                {
                    TraceLog.LogEntry("AdoTransaction.EnList('{1}'): this.trans ({0}) instance is same as pre-asigned to this command.", this.trans, command.CommandText);
                }
                else
                {
                    if (command.Transaction != null && command.Transaction != this.trans)
                        TraceLog.LogEntry("AdoTransaction.EnList('{2}'): The IDbCommand had a different Transaction (0x{0:X}) than the Session 0x{1:X}.", command.Transaction.GetHashCode(), this.trans.GetHashCode(), command.CommandText);

                    // If you try to assign a disposed transaction to a command with MSSQL, it will leave the command's
                    // transaction as null and not throw an error.  With SQLite, for example, it will throw an exception
                    // here instead.  Because of this, we set the trans field to null in when Dispose is called.

                    command.Transaction = this.trans;
                    //TraceLog.LogEntry("AdoTransaction.EnList(): Transaction instance 0x{0:X} was succesfully attached to '{1}' command.", command.Transaction.GetHashCode(), command.CommandText);
                }
            }
        }

        private void NotifyLocalSynchsAfterTransactionCompletion(bool success)
        {            
            TraceLog.LogEntry("AdoTransaction.NotifyLocalSynchsAfterTransactionCompletion(success= " + success.ToString() + ")");
            begun = false;

            ////if (synchronizations != null)
            ////{
            ////    for (int i = 0; i < synchronizations.Count; i++)
            ////    {
            ////        ISynchronization sync = synchronizations[i];
            ////        try
            ////        {
            ////            sync.AfterCompletion(success);
            ////        }
            ////        catch (Exception e)
            ////        {
            ////            log.Error("exception calling user Synchronization", e);
            ////        }
            ////    }
            ////}

            //SharpLogger.CallerOut();
        }

        #region System.IDisposable Members

        /// <summary>
        /// A flag to indicate if <c>Disose()</c> has been called.
        /// </summary>
        private bool _isAlreadyDisposed;

        /// <summary>
        /// Finalizer that ensures the object is correctly disposed of.
        /// </summary>
        ~AdoTransaction()
        {
            string m_hashc = "NULL";
            if (this.trans != null) m_hashc = String.Format("0x{0:X}", this.GetHashCode());
            //SharpLogger.Nfo("~AdoTransaction(): " + m_hashc);

            Dispose(false);
        }

        /// <summary>
        /// Takes care of freeing the managed and unmanaged resources that 
        /// this class is responsible for.
        /// </summary>
        public void Dispose()
        {
            //SharpLogger.CallerIn();
            Dispose(true);
            //SharpLogger.CallerOut();
        }

        /// <summary>
        /// Takes care of freeing the managed and unmanaged resources that 
        /// this class is responsible for.
        /// </summary>
        /// <param name="isDisposing">Indicates if this AdoTransaction is being Disposed of or Finalized.</param>
        /// <remarks>
        /// If this AdoTransaction is being Finalized (<c>isDisposing==false</c>) then make sure not
        /// to call any methods that could potentially bring this AdoTransaction back to life.
        /// </remarks>
        private void Dispose(bool isDisposing)
        {
            string m_hashc = "NULL";
            if (this.trans != null) m_hashc = String.Format("0x{0:X}", this.GetHashCode());
                                                              
            if (!_isAlreadyDisposed)      // don't dispose of multiple times.            
            {
                //SharpLogger.Nfo("Trans. HashCode: {0} isDisposing {1}", m_hashc, isDisposing); 

                // free managed resources that are being managed by the AdoTransaction if we know this call came through Dispose()

                if (isDisposing)
                {
                    if (trans != null)
                    {
                        this.trans.Dispose();
                        this.trans = null;
                        //SharpLogger.Nfo("IDbTransaction {0} disposed.",m_hashc);
                    }

                    if (this.session != null)
                    {
                        if (this.IsActive) this.AfterTransactionCompletion(false); // Assume we are rolled back   
                        //else
                        //    SharpLogger.Nfo("this.session!=NULL, AdoTransaction.Active=FALSE");
                                                          
                    }
                    //else
                    //    SharpLogger.Nfo("this.session IS NULL!");
                }
                //else
                //    SharpLogger.Nfo("isDisposing=FALSE");
                
                // {...} // free unmanaged resources here
                
                this._isAlreadyDisposed = true;
                GC.SuppressFinalize(this);  // nothing for Finalizer to do - so tell the GC to ignore it                
            }
            //else
            //    SharpLogger.Nfo("Trans. HashCode {0}: Already disposed!",m_hashc);

            //SharpLogger.CallerOut();
        }

        #endregion

        private void CheckNotDisposed()
        {
            if (_isAlreadyDisposed)
            {
                throw new ObjectDisposedException("AdoTransaction");
            }
        }

        private void CheckBegun()
        {
            if (!begun)
            {
                throw new System.Transactions.TransactionException("Transaction not successfully started");
            }
        }

        private void CheckNotZombied()
        {
            if (trans != null && trans.Connection == null)
            {
                throw new System.Transactions.TransactionException("Transaction not connected, or was disconnected");
            }
        }

        public override int GetHashCode()
        {
            if (this.trans != null)
                return this.trans.GetHashCode();
            else
                return -1;
        }

        public override string ToString()
        {
            string m_hashc = "NULL";
            if (this.trans != null) m_hashc = String.Format("0x{0:X}", this.GetHashCode());
           
            if (this.session != null)
                return String.Format("SessionId {0}:  TX HashCode: {1}", this.session.SessionId, m_hashc);
            else
                return String.Format("Session  NULL:  TX HashCode: {0}", m_hashc);
            
            
                
        }
        
    }
}
