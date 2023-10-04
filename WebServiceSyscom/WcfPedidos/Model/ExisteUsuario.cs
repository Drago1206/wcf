using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WcfPedidos.Model
{
    public class ExisteUsuario
    {
        private ConexionSQLite conSqlite = new ConexionSQLite("");
        private ConexionBD ClassConexion = new ConexionBD();

        public ExisteUsuario()
        {
            string connectionString = conSqlite.obtenerConexionSyscom().ConnectionString;

            ClassConexion.setConnection(conSqlite.obtenerConexionSyscom());
        }

        /// <summary>
        /// Verifica si el usuario existe.
        /// </summary>
        /// <param name="usuario">Usuario.</param>
        /// <param name="clave">contraseña.</param>
        /// <param name="mensaje">El mensaje en posicion 0 esta el codigo y en la posicion 1 la descripcion.</param>
        /// <returns></returns>
        public bool Existe(string usuario, string clave, out string[] mensaje)
        {
            mensaje = null;
            //se realiza la verificacion comprobar si existe 
            //conectar con la base de datos segun la  base que es 

            bool existe = false;
            if (usuario != null)
            {
                string[] pwdDe = null;
                SqlConnectionStringBuilder infoCon = conSqlite.obtenerConexionSQLServer("dbsyscom");
                string cons = infoCon != null ? "select U.IdUsuario, U.Usuario, (U.Usuario +'  '+ U.IdUsuario) as Descrip, PwdLog, IdGrupo from adm_Usuarios AS U where u.Inactivo = '0' and U.IdUsuario ='" + usuario + "'" : "select USU_ID as IdUsuario, USU_NOM as Usuario, (USU_NOM +'  '+ USU_ID) as Descrip, USU_CLV as PwdLog, USU_NIV as IdGrupo from " + conSqlite.obtenerConexionSQLServer("dbacc").InitialCatalog + ".dbo.USUARIOS as U where U.USU_ID ='" + usuario + "'";
                ClassConexion.resetQuery();
                ClassConexion.setCustomQuery(cons);
                ClassConexion.ejecutarQuery();
                DataTable dtUsu = ClassConexion.getDataTable();
                if (dtUsu.Rows.Count > 0)
                {
                    pwdSyscom pwd = new pwdSyscom(dtUsu.AsEnumerable().FirstOrDefault().Field<string>("PwdLog"));
                    pwd.Decodificar(dtUsu.AsEnumerable().FirstOrDefault().Field<string>("PwdLog"));
                    if (infoCon != null)
                        pwdDe = pwd.contrasenna.Split('=');
                    if (infoCon != null ? ((usuario.ToUpper() + "=" + dtUsu.AsEnumerable().FirstOrDefault().Field<int>("IdGrupo") + "=" + clave) == (pwdDe[0].ToUpper() + "=" + pwdDe[1] + "=" + pwdDe[2])) : ((clave.ToLower()) == pwd.contrasenna.ToLower()))
                    {
                        existe = true;
                    }
                    else
                    {
                        mensaje = new string[2];
                        mensaje[0] = "003";
                        mensaje[1] = "Contraseña inválida";
                    }
                }else
                {
                    mensaje = new string[2];
                    mensaje[0] = "002";
                    mensaje[1] = "El usuario no está creado en Syscom";

                }
            }
            return existe;
        }
        
    }
}