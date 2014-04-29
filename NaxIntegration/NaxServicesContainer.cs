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

        Dictionary<Type, object> naxObjects = new Dictionary<Type, object>();

        public NaxServicesContainer()
        {
            Enlace naxEnlace = GetInstance<Enlace>();
            naxEnlace.Iniciar(DB_NAME);
            ThrowExceptionIfError(1);
        }

        public T GetInstance<T>()
        {
            Type interfaceType = typeof(T);

            if (interfaceType.Namespace != "a3ERPActiveX")
            {
                throw new ApplicationException("Trying to instantiate a non a3ERPActiveX class!!");
            }

            if (!naxObjects.ContainsKey(interfaceType))
            {
                string progId = interfaceType.ToString(); // TODO: is this allways correct?
                Type classType = Type.GetTypeFromProgID(progId);
                naxObjects.Add(interfaceType, Activator.CreateInstance(classType));
            }
            return (T)naxObjects[interfaceType];
        }

        public dynamic GetInstance(string className, out Type type)
        {
            string progId = "a3ERPActiveX." + className;
            type = Type.GetTypeFromProgID(progId);
            Debug.WriteLine("Searching for progId: " + progId);
            if (!naxObjects.ContainsKey(type))
            {
                Debug.WriteLine("ProgId not found in cache. Creating...");
                object instance = Activator.CreateInstance(type);
                type = instance.GetType();
                naxObjects.Add(type, instance);
            }
            Debug.WriteLine("New cache count: " + naxObjects.Count().ToString());
            return naxObjects[type];
        }

        public void ThrowExceptionIfError(UInt16 code)
        {
            Enlace naxEnlace = GetInstance<Enlace>();
            if (naxEnlace.bError)
            {
                throw new ApplicationException(string.Format("[NAX_ERROR {0}: {1}]", code, naxEnlace.sMensaje));
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string SearchRegistry(string dllName)
        {
            RegistryKey clsidKey = Registry.ClassesRoot.OpenSubKey("CLSID");
            foreach (string subKey in clsidKey.GetSubKeyNames()) {
                RegistryKey inProcServer32Key = Registry.ClassesRoot.OpenSubKey("CLSID\\" + subKey + "\\InProcServer32");
                if (inProcServer32Key != null)
                {
                    string valueName = (from value in inProcServer32Key.GetValueNames() where string.IsNullOrEmpty(value) select value).FirstOrDefault();
                    string dllPathCandidate = GetLongPathName(inProcServer32Key.GetValue(valueName).ToString());
                    if (dllPathCandidate.EndsWith("\\" + dllName))
                    {
                        Debug.WriteLine("Dll found: " + dllPathCandidate);
                        return dllPathCandidate;
                    }
                }
            }
            throw new ApplicationException(string.Format("Dll {0} not found.", dllName));
        }

        public static String GetLongPathName(String shortPath)
        {
            StringBuilder longPath = new StringBuilder(1024);
            if (0 == GetLongPathName(shortPath, longPath, longPath.Capacity))
            {
                return shortPath;
            }
            return longPath.ToString();
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Int32 GetLongPathName(String shortPath, StringBuilder longPath, Int32 longPathLength);

    }
}
