using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Diagnostics;

using a3ERPActiveX;

namespace NaxIntegration
{
    [ServiceContract(
        SessionMode = SessionMode.NotAllowed
    )]
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Single,           // The nax dll doesn't looks like threadsafe
        InstanceContextMode = InstanceContextMode.Single
    )]
    public class WebService
    {
        static string DB_NAME = "pruebabd";

        Enlace naxEnlace = new Enlace();
        Maestro naxMaestro = new Maestro();
        Factura naxFactura = new Factura();
        Varios naxVarios = new Varios();

        public WebService()
        {
            Debug.WriteLine("in WebService() constructor");

            naxEnlace.Iniciar(DB_NAME);
            ThrowExceptionIfError(1);
        }

        protected void ThrowExceptionIfError(UInt16 code)
        {
            if (naxEnlace.bError)
            {
                throw new ApplicationException(string.Format("[NAX_ERROR {0}: {1}]", code, naxEnlace.sMensaje));
            }
        }

        [OperationContract]
        public decimal TestFactura()
        {
            Debug.WriteLine("in NewFactura()");

            naxFactura.Iniciar();
            ThrowExceptionIfError(1);

            naxFactura.Nuevo("30/03/2014", // Fecha de la factura
                "1", // Código del cliente
                false, // Indica que es de venta (True indicaría que es de compra)
                false, // Indica que es de gestión (True indicaría que es contable)
                true, // Indica que se desean las repercusiones contables
                true); // Indica que se desean los vencimientos
            ThrowExceptionIfError(2);

            naxFactura.NuevaLineaArt("1", 3D);
            naxFactura.AsFloatLin["PrcMoneda"] = 1500F;
            naxFactura.AnadirLinea();
            ThrowExceptionIfError(3);

            decimal numFactura = naxFactura.Anade();
            ThrowExceptionIfError(4);
            
            naxFactura.Acabar();
            ThrowExceptionIfError(5);
            
            return numFactura;
        }

        [OperationContract]
        public string TestVariosQuery()
        {
            Debug.WriteLine("in TestVariosQuery()");

            var result = naxVarios.CuentaArtV("1", "1");
            ThrowExceptionIfError(1);

            return result.ToString();
        }

        [OperationContract]
        public void TestCrearCliente()
        {
            Debug.WriteLine("in TestCrearCliente()");

            naxMaestro.Iniciar("Clientes");
            naxMaestro.Nuevo();
            ThrowExceptionIfError(1);
            naxMaestro.AsInteger["codcli"] = 24;
            naxMaestro.AsString["nomcli"] = "test cliente";
            ThrowExceptionIfError(2);
            naxMaestro.Guarda(false);
            ThrowExceptionIfError(3);
        }

    }
}





