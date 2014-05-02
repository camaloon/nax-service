using System;
using System.Configuration;

namespace NaxIntegration
{
    public class Logger
    {
        protected static System.IO.StreamWriter file;
        string logfilePath = null;

        public Logger()
        {
            logfilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                + @"\" + ConfigurationManager.AppSettings.Get("WebService:logFilename"); // TODO: better path concatenation?
        }

        public void Log(string message)
        {
            string logLine = string.Format("[{0} | {1}]", DateTime.Now.ToString("yyyy\\/MM\\/dd h\\:mm\\:ss tt"), message);
            System.IO.StreamWriter file = new System.IO.StreamWriter(logfilePath, true);
            file.WriteLine(logLine);
            file.Close();
        }
    }
}