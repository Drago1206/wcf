﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WcfPedidos30.Model
{
    public class ExisteUsuario
    {
        ConexionBD con = new ConexionBD();
        
        /// <summary>
        /// Verifica si el usuario existe.
        /// </summary>
        /// <param name="usuario">Usuario.</param>
        /// <param name="clave">contraseña.</param>
        /// <param name="mensaje">El mensaje en posicion 0 esta el codigo y en la posicion 1 la descripcion.</param>
        /// <returns></returns>
        public bool Existe(string usuario, string password, out string[] mensaje)
        {
            mensaje = null;
            //compania = "";
            //se realiza la verificacion comprobar si existe 
            //conectar con la base de datos segun la  base que es 

            bool existe = false;
            if (usuario != null)
            {
                int ResTotal = 0;
                string[] pwdDe = null;
                con.setConnection("Syscom");
                DataSet TablaIncio = new DataSet();
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Usuario", usuario));
                if (con.ejecutarQuery("WSPedidosInicioSeccion", parametros, out TablaIncio, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                {
                    if (TablaIncio.Tables[0].Rows.Count > 0)
                    {

                        /*pwdSyscom pwd = new pwdSyscom(TablaIncio.Tables[0].AsEnumerable().FirstOrDefault().Field<string>("PwdLog"));
                        pwd.Decodificar(TablaIncio.Tables[0].AsEnumerable().FirstOrDefault().Field<string>("PwdLog"));
                        if (con != null)
                            pwdDe = pwd.contrasenna.Split('=');
                            */
                        Log _err = null;
                        UsuariosResponse usuariosResponse = new UsuariosResponse();

                            DataTable ds = con.getDataTable();
                            DataRow row = ds.Rows[0];
                            if (row["NombreTercero"].ToString() == null || String.IsNullOrWhiteSpace(row["NombreTercero"].ToString()))
                                _err = new Log { Codigo = "USER_004", Descripcion = "¡El usuario no está creado como cliente!" };
                            else
                                usuariosResponse = con.ConvertRowToModel<UsuariosResponse>();
                        
                       
                    }
                    else
                    {
                        mensaje = new string[2];
                        mensaje[0] = "USER_001";
                        mensaje[1] = "¡Usuario no encontrado!";

                    }
                }
                else
                {
                    mensaje = new string[2];
                    mensaje[0] = nuevoMennsaje[0];
                    mensaje[1] = nuevoMennsaje[1];
                }
            }
            return existe;
        }
        
    }
}