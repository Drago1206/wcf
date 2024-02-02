using System;
using System.Collections.Generic;
using System.IO;

namespace WcfPedidos30.Model
{
    /// <summary>
    /// Clase estática para manejar los errores de log.
    /// </summary>
    public static class LogErrores
    {
        /// <summary>
        /// Directorio base de la aplicación.
        /// </summary>
        private static string dir = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Lista de tareas para registrar.
        /// </summary>
        public static List<string> tareas = new List<string>();

        /// <summary>
        /// Método para agregar un mensaje a la lista de tareas.
        /// </summary>
        /// <param name="mensaje">El mensaje a agregar.</param>
        public static void log(string mensaje)
        {
            tareas.Add(mensaje);
        }

        /// <summary>
        /// Método para escribir los mensajes de la lista de tareas en un archivo de log.
        /// </summary>
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