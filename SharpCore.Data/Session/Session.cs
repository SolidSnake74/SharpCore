using System;
using System.Data;

namespace SharpCore.Data
{
    //[Serializable]
    /// <summary>
    /// Session logical concept abstraction... Intended for create from initialized Sessionfactory only 
    /// when initialized... Many sessions from same factory, only one factory per calling proccess-
    /// </summary>
    internal abstract class Session: ISession
    {
        protected readonly Guid sessionId = Guid.Empty;

        [NonSerialized]
        protected ISessionFactory m_sesFactory;

              
        protected bool isAlreadyDisposed;

        protected bool closed;
       
        public abstract IDbConnection Connection { get; }

        public abstract ISessionFactory SessionFactory { get; }
           
        public Guid SessionId
        {
            get { return this.sessionId; }
        }

        public string CnnStrName 
        { 
            get 
            {
                return this.m_sesFactory.CnnName; 
            } 
        }

        public string CnnString
        {
            get
            {
                return this.m_sesFactory.ConnectionString;
            }
        }

        
        public ITransactionContext TransactionContext
        {
            get;
            set;
        }

        protected bool IsAlreadyDisposed
        {
            get { return isAlreadyDisposed; }
            set { isAlreadyDisposed = value; }
        }

        public abstract IDbConnection Close();
        public abstract IDbConnection Disconnect();

        public abstract bool IsConnected { get; }
        public abstract bool IsAutoCloseSessionEnabled { get; }
        public abstract bool ShouldAutoClose { get; }

        //public abstract TransactionScope GetTransactScope();
        //public abstract TransactionScope GetTransactScope(long tOut_mseg);

        public abstract bool TransactionInProgress { get; }
        public abstract void AfterTransactionBegin(ITransaction tx);
      
        public abstract void AfterTransactionCompletion(bool successful, ITransaction tx);

        public abstract void Dispose();

        /// <summary>
        /// La p_cnn debemos pasar null siempre..
        /// </summary>
        /// <param name="p_sesFact"></param>            
        internal Session(ISessionFactory p_sesFact) 
        {
            this.sessionId = Guid.NewGuid();
            this.m_sesFactory = p_sesFact;           
        }

        protected internal void SetClosed()
        {
            TraceLog.LogEntry("Session.SetClosed()");

            try
            {
                if (this.TransactionContext != null) this.TransactionContext.Dispose();
            }
            catch (Exception)
            {
                //ignore
            }
            this.closed = true;

            //SharpLogger.CallerOut();
        }

        public abstract T GetDAO<T>() where T : BaseDAL;

        public abstract T GetBAL<T>() where T : BaseDAL;

         

        public bool IsClosed
        {
            get { return this.closed; }
        }

        public override string ToString()
        {
            return "sessionId= " + this.sessionId.ToString(); // +":RootNode= " + this.RootNode;            
        }

        //~Session()
        //{
        //    SharpLogger.Nfo("~Session(): sessionId: {0}", this.sessionId.ToString());
        //}     

        //public List<short> ServersLoaded
        //{
        //    get
        //    {
        //        return this.m_servidores;
        //    }
        //}
        /////<summary>
        /////Code of root node server value when server is a leaf node,otherwise
        /////that is, current server is a root node,contains -1. Setted in construction.
        /////</summary>
        //protected short m_Central = -1;    // DTO Sync. uploading changes only

        ///// <summary>
        ///// Contains every non-root node when DAO was created in root server, otherwise, when was created on leaf-node, 
        ///// contains only one element wich is code of root node (usually Zero).
        ///// </summary>
        //protected List<short> m_servidores = null;
        //public short RootNode
        //{
        //    get
        //    {
        //        return this.m_Central;
        //    }
        //}

    }
  
}
