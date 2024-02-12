using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WcfDocTrasn.Model
{
    public class Errores {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }

    }



    public static class LogErrores
    {
        
        private static string dir = AppDomain.CurrentDomain.BaseDirectory;
        public static List<string> tareas = new List<string>();

        public static void log(string mensaje)
        {
            tareas.Add(mensaje);
        }

        public static void write()
        {
            using (StreamWriter sw = new StreamWriter(dir + "\\log_" + DateTime.Now.Year + "_" + DateTime.Now.Month + ".txt", true))
            {
                foreach (var i in tareas)
                {
                    sw.WriteLine(DateTime.Now + ": " + i);
                }
                sw.Close();
            }
            tareas.Clear();
        }
        public static void escribirError(Exception ex)
        {
            string message =
        "Exception type " + ex.GetType() + Environment.NewLine +
        "Exception message: " + ex.Message + Environment.NewLine +
        "Stack trace: " + ex.StackTrace + Environment.NewLine;
            if (ex.InnerException != null)
            {
                message += "---BEGIN InnerException--- " + Environment.NewLine +
                           "Exception type " + ex.InnerException.GetType() + Environment.NewLine +
                           "Exception message: " + ex.InnerException.Message + Environment.NewLine +
                           "Stack trace: " + ex.InnerException.StackTrace + Environment.NewLine +
                           "---END Inner Exception";
            }

            tareas.Add(message);
        }

    }
}