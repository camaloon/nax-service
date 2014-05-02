using System;
using System.ServiceModel;
using System.Configuration;
using System.Reflection;
using ServiceStack.Text;
using System.ServiceModel.Web;
using System.Net;
using System.Diagnostics;
using System.Web;

namespace NaxIntegration
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    public class WebService
    {
        protected string SESSION_COOKIE_NAME = "sid";

        protected SessionContainer sessionContainer = new SessionContainer();
        protected Logger logger = new Logger();

        [OperationContract]
        public string StartSession() {
            logger.Log("CALL TO: StartSession()");
            Session session = sessionContainer.StartSession();
            SetCookie(SESSION_COOKIE_NAME, session.SessionId(), session.ExpiresAt());
            return session.SessionId();
        }

        [OperationContract]
        public void EndSession()
        {
            logger.Log("CALL TO: EndSession()");
            sessionContainer.EndSession(GetSessionIdFromRequest());
        }

        [OperationContract]
        public string Execute(string className, string methodName, string jsonArguments = "[]")
        {
            logger.Log(string.Format("CALL TO: Execute(className: {0}, methodName: {1}, jsonArguments: {2})", className, methodName, jsonArguments));
            NaxServicesContainer naxContainer = GetNaxContainerFromSession();
            object[] arguments = jsonArguments.FromJson<object[]>();
            dynamic naxObject = naxContainer.GetInstance(className);
            var result = naxObject.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, naxObject, arguments);
            naxContainer.ThrowExceptionIfError();
            return result == null ? null : JsonSerializer.SerializeToString(result);
        }

        [OperationContract]
        public void Assign(string className, string attributeName, string jsonValue)
        {
            logger.Log(string.Format("CALL TO: Assign(className: {0}, attributeName: {1}, jsonValue: {2})", className, attributeName, jsonValue));
            BaseAssign(className, attributeName, ParseJsonValue(jsonValue));
        }

        [OperationContract]
        public void CollectionAssign(string className, string attributeName, string key, string jsonValue)
        {
            logger.Log(string.Format("CALL TO: CollectionAssign(className: {0}, attributeName: {1}, key: {2}, jsonValue: {3})", className, attributeName, key, jsonValue));
            BaseAssign(className, attributeName, new object[] { key, ParseJsonValue(jsonValue) });
        }

        protected void BaseAssign(string className, string attributeName, dynamic value)
        {
            NaxServicesContainer naxContainer = GetNaxContainerFromSession();
            dynamic naxObject = naxContainer.GetInstance(className);
            naxObject.GetType().InvokeMember(attributeName, BindingFlags.SetProperty, null, naxObject, value);
            naxContainer.ThrowExceptionIfError();
        }

        protected dynamic ParseJsonValue(string jsonValue)
        {
            return jsonValue.FromJson<dynamic>();
        }

        protected NaxServicesContainer GetNaxContainerFromSession()
        {
            return sessionContainer.GetSession(GetSessionIdFromRequest()).GetNaxServicesContainer();
        }

        protected string GetSessionIdFromRequest()
        {
            return GetCookie(SESSION_COOKIE_NAME); 
        }

        protected string GetCookie(string key)
        {
            return HttpContext.Current.Request.Cookies[key].Value;
        }

        protected void SetCookie(string key, string value, DateTime expires)
        {
            HttpCookie cookie = new HttpCookie(key, value);
            cookie.Expires = expires;
            HttpContext.Current.Response.SetCookie(cookie);
        }
    }
}





