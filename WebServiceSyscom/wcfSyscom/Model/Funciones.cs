using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using System.IO;
using System.Linq;
using System.Web;

namespace wcfSyscom.Model
{
    public class Funciones
    {
        public string fill0(string cadena)
        {
            for (var i = 1; i < 11 - cadena.Length; i++)
                cadena = "0" + cadena;
            return cadena;
        }

        public void CrearCSV(DataTable dt, string strFilePath, bool sinTitulo=true)
        {
            try
            {
                int fileLineas = 0;
                if (File.Exists(strFilePath))
                    fileLineas = File.ReadAllLines(strFilePath).Count();
                    
                StreamWriter sw = new StreamWriter(strFilePath, true);
                
                int columnCount = dt.Columns.Count;

                if(fileLineas==0 && sinTitulo == false)
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        sw.Write(dt.Columns[i]);
                        if (i < columnCount - 1)
                        {
                            sw.Write("|");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            sw.Write(dr[i].ToString());
                        }
                        if (i < columnCount - 1)
                        {
                            sw.Write("|");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

public void ErrorLog(string sErrMsg)
{
    //sLogFormat used to create log files format :
    // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message

}

        public static void log(string message)
        {
            string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";

            //this variable used to create log filename format "
            //for example filename : ErrorLogYYYYMMDD
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString();
            string sDay = DateTime.Now.Day.ToString();
            string sErrorTime = sYear + sMonth + sDay;

            string dir = System.AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);


            StreamWriter sw = new StreamWriter(dir + "\\" + DateTime.Now.ToString("ddMMyyyy") + ".log", true);
            sw.WriteLine(sLogFormat + message);
            sw.Flush();
            sw.Close();
        }
    }
}