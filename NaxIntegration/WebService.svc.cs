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
        public string TestVariosQuery()
        {
            dynamic naxVarios = naxContainer.GetInstance("Varios");

            var result = naxVarios.CuentaArtV("1", "1");
            naxContainer.ThrowExceptionIfError(1);

            return result.ToString();
        }

        [OperationContract]
        public decimal NewFactura()
        {
            dynamic naxFactura = naxContainer.GetInstance("Factura");
            
            naxFactura.Iniciar();
            naxContainer.ThrowExceptionIfError(1);

            naxFactura.Nuevo(
                "30/03/2014",   // Fecha de la factura
                "1",            // Código del cliente
                false,          // Indica que es de venta (True indicaría que es de compra)
                false,          // Indica que es de gestión (True indicaría que es contable)
                true,           // Indica que se desean las repercusiones contables
                true            // Indica que se desean los vencimientos
            );
            naxContainer.ThrowExceptionIfError(2);

            naxFactura.NuevaLineaArt("1", 3D);
            naxFactura.AsFloatLin["PrcMoneda"] = 1500F;
            naxFactura.AnadirLinea();
            naxContainer.ThrowExceptionIfError(3);

            decimal numFactura = naxFactura.Anade();
            naxContainer.ThrowExceptionIfError(4);
            
            naxFactura.Acabar();
            naxContainer.ThrowExceptionIfError(5);
            
            return numFactura;
        }

        [OperationContract]
        public void TestCrearCliente()
        {
            dynamic naxMaestro = naxContainer.GetInstance("Maestro");

            naxMaestro.Iniciar("Clientes");
            naxMaestro.Nuevo();
            naxContainer.ThrowExceptionIfError(1);
            naxMaestro.AsInteger["codcli"] = 25;
            naxMaestro.AsString["nomcli"] = "test cliente";
            naxContainer.ThrowExceptionIfError(2);
            naxMaestro.Guarda(false);
            naxContainer.ThrowExceptionIfError(3);
        }

    }
}





