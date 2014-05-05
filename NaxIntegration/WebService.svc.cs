using System.ServiceModel;
using System.Configuration;
using System.Reflection;
using ServiceStack.Text;

namespace NaxIntegration
{
    [ServiceContract(
        SessionMode = SessionMode.NotAllowed
    )]
    [ServiceBehavior(
        // The nax dll doesn't looks like bening threadsafe
        ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.Single
    )]
    public class WebService
    {
        NaxServicesContainer naxContainer = new NaxServicesContainer(ConfigurationManager.AppSettings.Get("WebService:dbName"));
        Logger logger = new Logger();

        [OperationContract]
        public string execute(string className, string methodName, string jsonArguments = "[]")
        {
            logger.Log(string.Format("CALL TO: execute(className: {0}, methodName: {1}, jsonArguments: {2})", className, methodName, jsonArguments));
            object[] arguments = jsonArguments.FromJson<object[]>();
            dynamic naxObject = naxContainer.GetInstance(className);
            var result = naxObject.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, naxObject, arguments);
            naxContainer.ThrowExceptionIfError();
            return result == null ? null : JsonSerializer.SerializeToString(result);
        }

        [OperationContract]
        public void assign(string className, string attributeName, string jsonValue)
        {
            logger.Log(string.Format("CALL TO: assign(className: {0}, attributeName: {1}, jsonValue: {2})", className, attributeName, jsonValue));
            BaseAssign(className, attributeName, ParseJsonValue(jsonValue));
        }

        [OperationContract]
        public void collectionAssign(string className, string attributeName, string key, string jsonValue)
        {
            logger.Log(string.Format("CALL TO: assign(className: {0}, attributeName: {1}, key: {2}, jsonValue: {3})", className, attributeName, key, jsonValue));
            BaseAssign(className, attributeName, new object[] { key, ParseJsonValue(jsonValue) });
        }

        protected void BaseAssign(string className, string attributeName, dynamic value)
        {
            dynamic naxObject = naxContainer.GetInstance(className);
            naxObject.GetType().InvokeMember(attributeName, BindingFlags.SetProperty, null, naxObject, value);
            naxContainer.ThrowExceptionIfError();
        }

        protected dynamic ParseJsonValue(string jsonValue)
        {
            dynamic parsedValue = jsonValue.FromJson<dynamic>();
            return parsedValue;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [OperationContract]
        public string TestVariosQuery()
        {
            return execute("Varios", "CuentaArtV", "[\"1\", \"1\"]");
        }

        [OperationContract]
        public string TestNewFactura()
        {
            execute("Factura", "Iniciar");
            execute("Factura", "Nuevo", "[\"02/04/2014\", \"1\", false, false, true, true]");
            execute("Factura", "NuevaLineaArt", "[\"1\", 3]");
            collectionAssign("Factura", "AsFloatLin", "PrcMoneda", "10");
            execute("Factura", "AnadirLinea");
            string numFacturaJson = execute("Factura", "Anade");
            execute("Factura", "Acabar");
            return numFacturaJson;
        }

        [OperationContract]
        public void TestCrearCliente()
        {
            execute("Maestro", "Iniciar", "[\"Clientes\"]");
            execute("Maestro", "Nuevo");
            collectionAssign("Maestro", "AsInteger", "codcli", "27");
            collectionAssign("Maestro", "AsString", "nomcli", "test cliente 2");
            execute("Maestro", "Guarda", "[false]");
        }

    }
}





