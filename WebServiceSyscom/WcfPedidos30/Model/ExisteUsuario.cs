using System;
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
        public bool Existe(string usuario, string clave, out string[] mensaje, out string compania)
        {
            mensaje = null;
            compania = "";
            //se realiza la verificacion comprobar si existe 
            //conectar con la base de datos segun la  base que es 

            bool existe = false;
            if (usuario != null)
            {
                int ResTotal = 0;
                string[] pwdDe = null;
                con.setConnection("syscom");
                DataSet TablaIncio = new DataSet();
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Usuario", usuario));
                if (con.ejecutarQuery("WSPedidosInicioSeccion", parametros, out TablaIncio, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                {
                    if (TablaIncio.Tables[0].Rows.Count > 0)
                    {

                        pwdSyscom pwd = new pwdSyscom(TablaIncio.Tables[0].AsEnumerable().FirstOrDefault().Field<string>("PwdLog"));
                        pwd.Decodificar(TablaIncio.Tables[0].AsEnumerable().FirstOrDefault().Field<string>("PwdLog"));
                        if (con != null)
                            pwdDe = pwd.contrasenna.Split('=');
                        if (con != null ? ((usuario.ToUpper() + "=" + TablaIncio.Tables[0].AsEnumerable().FirstOrDefault().Field<int>("IdGrupo") + "=" + clave) == (pwdDe[0].ToUpper() + "=" + pwdDe[1] + "=" + pwdDe[2])) : ((clave.ToLower()) == pwd.contrasenna.ToLower()))
                        {
                            compania = TablaIncio.Tables[0].Rows[0]["Compania"].ToString();
                            existe = true;
                        }
                        else
                        {
                            mensaje = new string[2];
                            mensaje[0] = "003";
                            mensaje[1] = "Contraseña inválida";
                        }
                    }
                    else
                    {
                        mensaje = new string[2];
                        mensaje[0] = "002";
                        mensaje[1] = "El usuario no está creado en Syscom";

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