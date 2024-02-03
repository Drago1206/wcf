using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using wcfSyscom30.Conexion;

namespace wcfSyscom30.Models
{
    public class sys_Usuarios
    {
        public string IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string Descrip { get; set; }
        public bool Inactivo { get; set; }
        public string PwdLog { get; set; }
        public int IdGrupo { get; set; }
        public string IdEstacion { get; set; }
        public string IdCargo { get; set; }
        public bool Conectado { get; set; }
        public DateTime FechaAcc { get; set; }
        public DateTime FechaClave { get; set; }
        public int Caducidad { get; set; }
        public string LoginSQL { get; set; }
        public string PwdSQL { get; set; }
        public string ModoSQL { get; set; }
        public int IdGrupR { get; set; }
        public string nameserver { get; set; }
        public int Idlog { get; set; }

        private Conexion ClassConexion = new Conexion();
        private clsConnSqlite conSqlite = new clsConnSqlite("");
        private List<SqlParameter> lsp = new List<SqlParameter>();
        List<string> errores = new List<string>();
        DataTable dt = new DataTable();
        pwdSyscom usuario = new pwdSyscom();
        string[] Splitcontrasena;

        public sys_Usuarios()
        {
            ClassConexion.setConnection(conSqlite.obtenerConexionSyscom());
        }

        /// <summary>
        /// Validar Usuario.
        /// </summary>
        /// <param name="modelo">Usuario.</param>
        /// <returns>retorna un Dictionary donde indica un valor booleano y un valor string(Descripcion de error)</returns>
        public Dictionary<bool, string> ValidarUsu(Usuario modelo)
        {
            List<sys_Usuarios> _var = new List<sys_Usuarios>();
            sys_Usuarios us = new sys_Usuarios();
            List<string> Errores = buscarUsuario(out _var, modelo.UserName, true);
            us = _var.FirstOrDefault();
            Dictionary<bool, string> res = new Dictionary<bool, string>();

            try
            {
                if (us != null)
                {
                    if (us.Inactivo == false)
                    {
                        if (us.Conectado == false)
                        {
                            pwdSyscom usuario = new pwdSyscom();
                            usuario.Decodificar(us.PwdLog);
                            Splitcontrasena = usuario.contrasenna.Split('=');
                            if (Splitcontrasena[0].ToString().ToUpper() + "=" + Splitcontrasena[1].ToString() + "=" + Splitcontrasena[2].ToString() == modelo.UserName.ToUpper() + "=" + Convert.ToString(us.IdGrupR) + "=" + modelo.Password)
                                res.Add(true, null);
                            else
                                res.Add(false, "La Contraseña es incorrecta.");
                        }
                        else
                            res.Add(false, "El usuario esta conectado.");
                    }
                    else
                        res.Add(false, "El usuario está inactivo.");
                }
                else
                    res.Add(false, "El usuario no existe.");
            }
            catch (Exception ex)
            {
                res.Add(false, "Error: " + ex.Message + "");
            }

            return res;
        }

        public List<string> buscarUsuario(out List<sys_Usuarios> us, string usuDocumento, bool validar = false)
        {
            errores = new List<string>();
            us = new List<sys_Usuarios>();
            try
            {
                #region consulta de usuario
                ClassConexion.resetQuery();
                ClassConexion.qryFields.Add("U.IdUsuario, U.Usuario, U.PwdLog, U.IdGrupo");
                ClassConexion.qryFields.Add("U.IdEstacion, U.IdCargo, U.Conectado, U.FechaAcc, U.FechaClave, U.Caducidad, U.LoginSQL, U.PwdSQL, U.ModoSQL, U.Inactivo");
                ClassConexion.qryTables.Add("adm_Usuarios AS U");
                if (usuDocumento != null)
                    ClassConexion.addWhereAND("U.IdUsuario = '" + usuDocumento + "'");
                ClassConexion.select();
                ClassConexion.ejecutarQuery();
                List<sys_Usuarios> _us = ClassConexion.DataTableToList<sys_Usuarios>();

                #endregion
            }
            catch (SqlException ex)
            {
                errores.Add(ex.Message);
                ClassConexion.Log("aqui en buscar usuario:" + ex.Message);
            }

            return errores;
        }

        public bool ExisteUsuario(string usuario, string clave)
        {
            bool existe = false;
            if (usuario != null)
            {
                string[] pwdDe = null;
                SqlConnectionStringBuilder infoCon = conSqlite.obtenerConexionSQLServer("dbsyscom");
                #region consulta produccion
                //string cons = infoCon != null ? "select U.IdUsuario, U.Usuario, (U.Usuario +'  '+ U.IdUsuario) as Descrip, PwdLog, IdGrupo from adm_Usuarios AS U where u.Inactivo = '0' and U.IdUsuario ='" + usuario + "'" : "select U.IdUsuario, U.Usuario, (U.Usuario +'  '+ U.IdUsuario) as Descrip, PwdLog, IdGrupo from adm_Usuarios " + conSqlite.obtenerConexionSQLServer("dbacc").InitialCatalog + ".dbo.USUARIOS as U where U.USU_ID ='" + usuario + "'";
                string cons = infoCon != null ? "select U.IdUsuario, U.Usuario, (U.Usuario +'  '+ U.IdUsuario) as Descrip, PwdLog, IdGrupo from adm_Usuarios AS U where u.Inactivo = '0' and U.IdUsuario ='" + usuario + "'" : "select USU_ID as IdUsuario, USU_NOM as Usuario, (USU_NOM +'  '+ USU_ID) as Descrip, USU_CLV as PwdLog, USU_NIV as IdGrupo from " + conSqlite.obtenerConexionSQLServer("dbacc").InitialCatalog + ".dbo.USUARIOS as U where U.USU_ID ='" + usuario + "'";
                #endregion
                #region consulta pruebas coodepetrol
                //string cons = infoCon != null ? "select U.IdUsuario, U.Usuario, (U.Usuario +'  '+ U.IdUsuario) as Descrip, PwdLog, IdGrupo from adm_Usuarios AS U where u.Inactivo = '0' and U.IdUsuario ='" + usuario + "'" : "select USU_ID as IdUsuario, USU_NOM as Usuario, (USU_NOM +'  '+ USU_ID) as Descrip, USU_CLV as PwdLog, USU_NIV as IdGrupo from dbacc_coode.dbo.USUARIOS as U where U.USU_ID ='" + usuario + "'";
                #endregion
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
                        existe = true;

                }
            }
            return existe;
        }


        public int Conectar(Usuario Modelo)
        {
            string Fech = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            int rep = 0;
            ClassConexion.resetQuery();
            ClassConexion.beginTran();
            try
            {
                ClassConexion.setCustomQuery("Update adm_Usuarios Set Conectado = '1', FechaAcc = CONVERT(DATETIME, REPLACE('" + Fech + "','/','')) Where IdUsuario = '" + Modelo.UserName.ToUpper() + "'");
                ClassConexion.ejecutarQuery();
                ClassConexion.commitTran();
            }
            catch
            {
                ClassConexion.rollback();
            }

            return rep;
        }


        public bool Desconectar(Usuario Modelo)
        {
            string Fech = DateTime.Now.ToShortDateString();

            ClassConexion.resetQuery();
            ClassConexion.beginTran();
            try
            {
                ClassConexion.setCustomQuery("Update adm_Usuarios Set Conectado = '0' Where IdUsuario = '" + Modelo.UserName.ToUpper() + "'");
                ClassConexion.ejecutarQuery();
                ClassConexion.commitTran();
                return true;
            }
            catch
            {
                ClassConexion.rollback();
                return false;
            }
        }

        public string Decodificar(string codigo)
        {
            usuario.Decodificar(codigo);
            return usuario.contrasenna.Split('=')[0];
        }

        public bool ExistUsuarioWs(string usudoc)
        {
            ClassConexion.resetQuery();
            ClassConexion.qryFields.Add("U.IdUsuario");
            ClassConexion.qryTables.Add("adm_Usuarios AS U");
            if (usudoc != null)
                ClassConexion.addWhereAND("U.IdUsuario = '" + usudoc + "'");
            ClassConexion.select();
            ClassConexion.ejecutarQuery();

            if (ClassConexion.getNumFilas() > 0)
                return true;
            else
                return false;
        }
    }
}