using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using WcfPedidos40.Conexion;
using WcfPedidos40.Models;

namespace WcfPruebas40.Models
{
    public class ExisteUsuario
    {

        Conexion con = new Conexion();

        /// <summary>
        /// Verifica si el usuario existe.
        /// </summary>
        /// <param name="usuario">Usuario.</param>
        /// <param name="clave">contraseña.</param>
        /// <param name="mensaje">El mensaje en posicion 0 esta el codigo y en la posicion 1 la descripcion.</param>
        /// <returns></returns>




        private bool existeUsuario(Usuario usuario)
        {
            bool existe = false;
            con.setConnection("Syscom");
            con.resetQuery();
            con.qryFields.Add("IdUsuario, PwdLog");
            con.qryTables.Add("adm_Usuarios");
            con.addWhereAND("Inactivo = 0 and lower(IdUsuario) = lower('" + usuario.UserName + "')");
            con.select();
            con.ejecutarQuery();
            DataTable ds = con.getDataTable();
            DataRow row = ds.Rows[0];
            if (ds.Rows.Count > 0)
            {
                pwdSyscom pwdSys = new pwdSyscom();
                pwdSys.Decodificar(row["PwdLog"].ToString());
                var contra = pwdSys.contrasenna.Split('=');
                if (contra[2] == usuario.Password)
                {
                    existe = true;

                }
                else
                {
                    existe = false;
                }
            }
            return existe;
        }
    }
}