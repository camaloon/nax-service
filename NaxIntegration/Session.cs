using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;


namespace NaxIntegration
{
    public class Session
    {
        protected DateTime lastTouch = DateTime.Now;
        protected string sessionId = Guid.NewGuid().ToString();

        protected NaxServicesContainer naxContainer = new NaxServicesContainer(ConfigurationManager.AppSettings.Get("WebService:dbName"));

        public string SessionId()
        {
            return sessionId;
        }

        public void Touch()
        {
            lastTouch = DateTime.Now;
        }

        public DateTime LastTouchedAt()
        {
            return lastTouch;
        }

        public DateTime ExpiresAt()
        {
            UInt16 sessionDurationInSeconds = Convert.ToUInt16(ConfigurationManager.AppSettings.Get("WebService:sessionDurationSeconds"));
            return LastTouchedAt().AddSeconds(sessionDurationInSeconds);
        }

        public bool Expired()
        {
            return DateTime.Now > ExpiresAt();
        }

        public NaxServicesContainer GetNaxServicesContainer()
        {
            return naxContainer;
        }
    }
}