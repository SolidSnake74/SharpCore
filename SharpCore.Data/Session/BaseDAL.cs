//using SharpCore.Utilities;
using System;
//using SharpCore.Logging;


namespace SharpCore.Data
{
    [Serializable]
    public abstract class BaseDAL
    {

        protected internal ISessionTX m_session = null;
                             
        ///<summary>
        ///Use this construction when requires 'BaseDAL' works opening a indepent connection.
        ///</summary>
        ///<param name="p_ses">Session instance to be used by DAL repository. Validity of internal connection string will be checked.</param>
        //internal BaseDAL(ISession p_ses)
        public BaseDAL(ISession p_ses)
        {            
            this.m_session = p_ses as ISessionTX;                  
            //SharpLogger.Nfo("Instanciado {0} SessionId {1}",this.GetType().Name,this.m_session.SessionId);
            //ValidationUtility.ValidateArgument("CnnStrName", this.m_session.CnnStrName);                    
        }       

    }
}
