using System;
using System.Collections.Generic;
using a3ERPActiveX;

namespace NaxIntegration
{
    /// <summary>
    /// Container for Nax objects
    /// </summary>
    public class NaxServicesContainer
    {
        protected static string PROG_ID_PREFIX = "a3ERPActiveX";

        Dictionary<string, dynamic> naxObjects = new Dictionary<string, dynamic>();

        public NaxServicesContainer(string dbName)
        {
            Enlace naxEnlace = GetInstance("Enlace");
            naxEnlace.Iniciar(dbName);
            ThrowExceptionIfError();
        }

        public dynamic GetInstance(string className)
        {
            string progId = PROG_ID_PREFIX + "." + className;
            if (!naxObjects.ContainsKey(progId))
            {
                naxObjects.Add(progId, Activator.CreateInstance(Type.GetTypeFromProgID(progId)));
            }
            return naxObjects[progId];
        }

        public void ThrowExceptionIfError()
        {
            Enlace naxEnlace = GetInstance("Enlace");
            if (naxEnlace.bError)
            {
                throw new ApplicationException(string.Format("[NAX_ERROR: {0}]", naxEnlace.sMensaje));
            }
        }
    }
}
