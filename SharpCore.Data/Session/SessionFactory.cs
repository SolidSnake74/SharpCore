using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SharpCore.Data
{   
    /// <summary>
    /// Singleton Session factory, requires initialization from connection string name.
    /// Only one per proccess pair. Useful to give Sessions to DAO layer.
    /// </summary>
    public sealed class  SessionFactory: ISessionFactory
    {
        //private static SessionFactory m_instance = null;

        //private static string m_cnnName = String.Empty;
        //private static string m_cnnStr  = String.Empty;        

        private string m_cnnName = String.Empty;
        private string m_cnnStr = String.Empty;


        //private ISessionTX m_currSes = null;

        [NonSerialized]
        private readonly ICurrentSessionContext currentSessionContext;

        private readonly string uuid=String.Empty;
        private bool m_disposed = true;
        private bool m_closed = true;



        public string CnnName
        {
            get
            {
                return m_cnnName;
            }
        }

        public string ConnectionString
        {
            get
            {
                return m_cnnStr;
            }
        }

        /// <summary>
        /// MJBF. Gets a new open <see cref="IDbConnection"/> through the 'NHibernate.Driver.IDriver'.
        /// </summary>
        /// <returns>
        /// An Open <see cref="IDbConnection"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// If there is any problem creating or opening the <see cref="IDbConnection"/>.
        /// </exception>
        public IDbConnection GetConnection_MSSQL()
        {
            //TraceLog.LogEntry(this, "Obtaining IDbConnection from Driver");

            //Driver.CreateConnection();  // Por ahora, asumimos que siemrpe se tratará de MSSQLServer.

            IDbConnection conn = new SqlConnection(this.m_cnnStr);
            try
            {                
                conn.Open();
                TraceLog.LogEntry(this, "conexion abierta.");
            }
            catch (Exception)
            {
                conn.Dispose();
                throw;
            }

            // SharpLogger.CallerOut();

            return conn;
        }


        public bool Closed 
        {
            get 
            {
                return this.m_closed;
            } 
        }

        /// <summary>
        /// Singleton constructor... constructs this singleton when 
        /// caller invokes Init() method with valid connection name...
        /// </summary>
        /// <param name="p_cnnName"></param>      
        public SessionFactory(string p_cnnName)
        {           
            Init(p_cnnName);
            currentSessionContext = BuildCurrentSessionContext();
        }

        //public ITransaction CreateTransaction(ISession session)
        //{
        //    return new AdoTransaction(session);
        //}

       

        
        /// <summary>
        /// Start the factory using a valid connection, allowing the
        /// caller to build sessions from GetSession() Method.
        /// Due the Sessionfactory is created in accordance to Singleton pattern,
        /// only creates a real instance if needed. Otherwise does nothing and return false.
        /// </summary>
        /// <param name="p_cnnName">Connection string name used to create sessions.</param>
        /// <returns>T/F if the factory was correctly initializated or not.</returns>
        public void Init(string p_cnnName)
        {
            if (this.m_cnnName == String.Empty)
            {
                this.m_cnnStr = ConfigurationManager.ConnectionStrings[p_cnnName].ConnectionString;

                if (this.m_cnnStr == null)
                    throw new Exception("Connection string name not valid (can't get the connection string)");
                else
                {
                    this.m_cnnName = p_cnnName;
                    this.m_closed = false;
                    //this.m_currSes = null;

                    
                }
            }
            else
                throw new Exception("Must be closed first!");
        }

      

       

        public ISession OpenSession()
        {          
            return OpenSession(true, ConnectionReleaseMode.OnClose); // ,timestamp, sessionLocalInterceptor);
        }

        /// <summary>
        /// Builds a session from the factory, when initialized...
        /// </summary>
        /// <returns></returns>
        public ISession OpenSession(bool p_autoCloseSessionEnabled,ConnectionReleaseMode p_connectionReleaseMode)
        {
            TraceLog.LogEntry("OpenSession(): p_autoCloseSessionEnabled {0} p_connectionReleaseMode {1}", p_autoCloseSessionEnabled, p_connectionReleaseMode);
            
            ISessionTX res = null;

            if (this.m_cnnStr == String.Empty)
                throw new Exception("Session Factory without connection string, can't get it from configuration!");
            else
                if (this.m_closed)
                    throw new Exception("Session Factory is closed!");
                else
                {
                    if (SharpCore.Data.CurrentSessionContext.HasBind(this))
                    {
                        res = this.GetCurrentSession();

                        TraceLog.LogEntry("Existe una Sesión ya ligada, GetCurrentSession() Id {0}", res.SessionId);

                    }
                    else                        
                        SharpCore.Data.CurrentSessionContext.Bind(res = new SessionExt(this, p_autoCloseSessionEnabled, p_connectionReleaseMode));                        
                }
            
            // SharpLogger.CallerOut();

            return res;
        }

        public ISessionTX GetCurrentSession()
        {
            if (currentSessionContext == null)
            {
                throw new Exception("No CurrentSessionContext configured (set the property 'CurrentSessionContextClass')");
            }
            return currentSessionContext.CurrentSession();
        
        }

        /// <summary>
        /// Forces Shutdown of the factory... Useful to cleanup and Destroy the instance.
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            // SharpLogger.CallerIn();
            bool res=false;

            try
            {
                res = !this.m_closed; //(SessionFactory.m_instance != null);                                                                                                                                        
            }
            catch (Exception ex)
            {
                TraceLog.LogEntry(this, ex.Message);                 
            }
            finally
            {
                if (res)
                {                    
                    this.m_closed = true;                   
                    this.m_cnnName = String.Empty;
                }              

                // SharpLogger.CallerOut();
            }

            return res;
        }

        public void Dispose()
        {
            // SharpLogger.CallerIn();

            if (!this.m_disposed)
            {                
                if (this.Close())
                    TraceLog.LogEntry(this,"SessionFactory closed.");                    
                else
                    TraceLog.LogEntry(this, "SessionFactory already closed.");                    
                   
                
                this.m_disposed = true;
                // SharpLogger.Nfo("SessionFactory disposed.");
            }
            else 
                TraceLog.LogEntry(this, "SessionFactory already disposed!");

            // SharpLogger.CallerOut();
        }


        /// <summary>
        /// Gets the ICurrentSessionContext instance attached to this session factory.
        /// </summary>
        public ICurrentSessionContext CurrentSessionContext
        {
            get { return currentSessionContext; }
        }

        private ICurrentSessionContext BuildCurrentSessionContext()
        {
            //string impl = PropertiesHelper.GetString(Environment.CurrentSessionContextClass, properties, null);
           
            //switch (impl)
            //{
            //    case null:
            //        return null;
            //    case "call":
            //        return new CallSessionContext(this);
            //    case "thread_static":
            return new ThreadStaticSessionContext(this);
            //    case "web":
            //        return new WebSessionContext(this);
            //    case "wcf_operation":
            //        return new WcfOperationSessionContext(this);
            //}

            //try
            //{
            //    System.Type implClass = ReflectHelper.ClassForName(impl);
            //    return
            //        (ICurrentSessionContext)Environment.BytecodeProvider.ObjectsFactory.CreateInstance(implClass, new object[] { this });
            //}
            //catch (Exception e)
            //{
            //    log.Error("Unable to construct current session context [" + impl + "]", e);
            //    return null;
            //}
        }
    }


   

}