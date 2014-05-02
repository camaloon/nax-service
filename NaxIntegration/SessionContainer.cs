using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Diagnostics;

namespace NaxIntegration
{
    public class SessionContainer
    {
        Dictionary<string, Session> sessions = new Dictionary<string,Session>();
        protected Logger logger = new Logger();

        public Session StartSession()
        {
            Clean();
            Session session = new Session();
            sessions.Add(session.SessionId(), session);
            logger.Log(string.Format("Created session with SID: {0}", session.SessionId()));
            return session;
        }

        public Session GetSession(string sessionId)
        {
            Clean();
            if (!sessions.ContainsKey(sessionId)) throw new ApplicationException("SessionID not found");
            Session session = sessions[sessionId];
            session.Touch();
            return session;
        }

        public void EndSession(string sessionId)
        {
            sessions.Remove(sessionId);
            logger.Log(string.Format("Terminated session with SID: {0}", sessionId));
        }

        public void Clean()
        {
            List<string> sessionsToDelete = new List<string>();
            foreach (KeyValuePair<string, Session> sessionEntry in sessions)
            {
                string sessionId = sessionEntry.Key;
                Session session = sessionEntry.Value;
                if (session.Expired())
                {
                    sessionsToDelete.Add(sessionId);
                    logger.Log(string.Format("Session expired SID: {0}", sessionId));
                }
            }
            foreach (string sessionIdToDelete in sessionsToDelete)
            {
                EndSession(sessionIdToDelete);
            }
        }
    }
}