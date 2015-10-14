using System;
using System.Data;

namespace SharpCore.Data
{
    public interface ISessionFactory: IDisposable
    {
        
        string CnnName { get; }
        string ConnectionString { get; }

        /// <summary>
        /// Gets the ICurrentSessionContext instance attached to this session factory.
        /// </summary>
        ICurrentSessionContext CurrentSessionContext { get; }


        /// <summary>
        /// MJBF. Gets a new open <see cref="IDbConnection"/> through the 'SharpCore.Driver.IDriver'.
        /// </summary>
        /// <returns>
        /// An Open <see cref="IDbConnection"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// If there is any problem creating or opening the <see cref="IDbConnection"/>.
        /// </exception>
        IDbConnection GetConnection_MSSQL();

        /// <summary>
        /// Obtains the current session.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The definition of what exactly "current" means is controlled by the <see cref="ICurrentSessionContext" />
        /// implementation configured for use.
        /// </para>
        /// </remarks>
        /// <returns>The current session.</returns>
        /// <exception cref="Exception">Indicates an issue locating a suitable current session.</exception>
        ISessionTX GetCurrentSession();

        

        bool Closed { get; }

        bool Close();

        /// <summary>
        /// Pues eso...
        /// </summary>
        /// <param name="p_autoCloseSessionEnabled"></param>
        /// <param name="p_connectionReleaseMode"></param>
        /// <returns></returns>
        ISession OpenSession(bool p_autoCloseSessionEnabled, ConnectionReleaseMode p_connectionReleaseMode);

        /// <summary>
        /// Pues eso...
        /// </summary>        
        /// <returns></returns>
        ISession OpenSession();
    }
}
