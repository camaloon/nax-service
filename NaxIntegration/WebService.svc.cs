using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Diagnostics;

using System.Reflection;
using System.Runtime.InteropServices;

using ServiceStack.Text;

using a3ERPActiveX;

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
        NaxServicesContainer naxContainer = new NaxServicesContainer();

        [OperationContract]
        public string execute(string className, string methodName, string jsonArguments = "[]")
        {
            object[] arguments = jsonArguments.FromJson<object[]>();
            dynamic naxObject = naxContainer.GetInstance(className);
            var result = naxObject.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, naxObject, arguments);
            naxContainer.ThrowExceptionIfError(0);
            return result == null ? null : JsonSerializer.SerializeToString(result);
        }

        [OperationContract]
        public void assign(string className, string attributeName, string jsonValue)
        {
            baseAssign(className, attributeName, jsonValue.FromJson<object>());
        }

        [OperationContract]
        public void collectionAssign(string className, string attributeName, string arrayKey, string jsonValue)
        {
            baseAssign(className, attributeName, new object[] {arrayKey, jsonValue.FromJson<object>()});
        }

        protected void baseAssign(string className, string attributeName, dynamic value)
        {
            dynamic naxObject = naxContainer.GetInstance(className);
            naxObject.GetType().InvokeMember(attributeName, BindingFlags.SetProperty, null, naxObject, value);
            naxContainer.ThrowExceptionIfError(0);
        }

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





