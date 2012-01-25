using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Recepcion.Common
{
    public class Log
    {
        
        public static void WriteLog(string log)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string dir = config.AppSettings.Settings["LogPath"].Value.ToString();
            string fileName = @dir + "log_" + DateTime.Now.Date.ToShortDateString().Replace('/', '-') + ".txt";
            System.IO.StreamWriter writer = System.IO.File.AppendText(fileName);
            writer.WriteLine(DateTime.Now.ToString());
            writer.WriteLine(log);
            writer.Close();
        }
    }
}
