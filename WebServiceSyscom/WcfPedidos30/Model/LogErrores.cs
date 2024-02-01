using System;
using System.Collections.Generic;
using System.IO;

namespace WcfPedidos30.Model
{
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

    }
}