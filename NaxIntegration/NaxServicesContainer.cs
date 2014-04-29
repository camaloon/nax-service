using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;

using a3ERPActiveX;

namespace NaxIntegration
{
    /// <summary>
    /// Container for Nax objects
    /// </summary>
    public class NaxServicesContainer
    {
        static string DB_NAME = "pruebabd";

        Dictionary<string, dynamic> naxObjects = new Dictionary<string, dynamic>();

        public NaxServicesContainer()
        {
            Enlace naxEnlace = GetInstance("Enlace");
            naxEnlace.Iniciar(DB_NAME);
            ThrowExceptionIfError(1);
        }

        public dynamic GetInstance(string className)
        {
            string progId = "a3ERPActiveX." + className;
            if (!naxObjects.ContainsKey(progId))
            {
                naxObjects.Add(progId, Activator.CreateInstance(Type.GetTypeFromProgID(progId)));
            }
            return naxObjects[progId];
        }

        public void ThrowExceptionIfError(UInt16 code)
        {
            Enlace naxEnlace = GetInstance("Enlace");
            if (naxEnlace.bError)
            {
                throw new ApplicationException(string.Format("[NAX_ERROR {0}: {1}]", code, naxEnlace.sMensaje));
            }
        }
    }
}
