using System;

/// <summary>
/// Container for Nax objects
/// </summary>
public class NaxServicesContainer
{
    static string DB_NAME = "pruebabd";

    Enlace naxEnlace = new Enlace();
    Factura naxFactura = new Factura();
    Varios naxVarios = new Varios();

    public NaxServicesContainer()
    {
        //naxEnlace.Iniciar(DB_NAME);
        //ThrowExceptionIfError(1);
    }

}
