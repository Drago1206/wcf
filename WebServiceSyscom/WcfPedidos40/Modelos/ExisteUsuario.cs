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
        public bool Existe(string usuario, string password, out string[] mensaje)
        {
            Conexion con = new Conexion();

            try
            {
                con.setConnection("Syscom");

                con.resetQuery();
                con.qryFields.Add("IdUsuario, Inactivo, PwdLog");
                con.qryTables.Add("adm_Usuarios");
                con.addWhereAND("lower(IdUsuario) = lower('" + password + "')");
                con.select();
                con.ejecutarQuery();
                mensaje = new string[] { "Usuario  encontrado" };
            }
            catch (Exception e) {
                mensaje = new string[] { "El usuario no se ha  encontrado" };
            }

            return con.getDataTable(); 
            

        }
    }
}