using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SyscomUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfDocTrasn.Model;

namespace WcfDocTrasn
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IDocTrasn
    {
        public RespuestaWS GetToken(User User)
        {
            RespuestaWS respuesta = new RespuestaWS();
            respuesta.Errores = new Errores();
            try
            {
                if (User.Usuario == null)
                    respuesta.Errores = new Errores { Codigo = "001", Descripcion = "¡Todas las variables del usuario no pueden ser nulas!" };
                else
                {
                    if (string.IsNullOrWhiteSpace(User.Usuario.Usuario) || string.IsNullOrWhiteSpace(User.Usuario.Password))
                    {
                        respuesta.Errores = new Errores { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos!" };
                    }
                    if (string.IsNullOrWhiteSpace(User.Usuario.Compania)) { 
                        respuesta.Errores = new Errores { Codigo = "001", Descripcion = "Parámetro 'Compania', NO pueden ser nulo!" };
                    }
                    if (string.IsNullOrWhiteSpace(User.Usuario.Nit))
                    {
                        respuesta.Errores = new Errores { Codigo = "001", Descripcion = "Parámetro 'Nit', NO pueden ser nulo!" };

                    }
                    else
                    {
                        Usuarios dusuarios = new Usuarios();
                        Usuarios usu = new Usuarios();
                        Model.Conexion con = new Model.Conexion();

                        pwdSyscom pwdSys = new pwdSyscom();
                        con.setConnection("Syscom");
                        //Se realiza una lista de parametros para poder ingresar los datos a los procesos de almacenado
                        List<SqlParameter> parametros = new List<SqlParameter>();
                        DataSet Tablainfo = new DataSet();
                        //Indicamos el parametro que vamos a pasar 
                        parametros.Add(new SqlParameter("@NitCliente",User.Usuario.Nit));
                        parametros.Add(new SqlParameter("@IdUsuario", User.Usuario.Usuario));
                        con.addParametersProc(parametros);

                        //Ejecuta procedimiento almacenado
                        //Representa una tabla de datos en memoria, en esta mostraremos los datos obtenidos en una tabla.
                        DataTable DT = new DataTable();
                        // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                        con.resetQuery();
                        //Verificamos y ejecutamos al mismo tiempo el procedimiento de almacenado 
                        if (con.ejecutarQuerys("WcfPedidos_ObtenerToken", parametros, out DT, out string[] nuevoMennsaje, CommandType.StoredProcedure)) {

                            DataRow row = DT.Rows[0];

                            string clase = "";
                            if (row["EsVendedor"].Equals(true))
                                clase = "vendedor";
                            else
                                clase = "cliente";

                            DateTime _expiration = DateTime.Now.AddMinutes(30);
                            pwdSys.Codificar(string.Concat(row["idusuario"].ToString(), "=", usu.Compania, "=", usu.Nit, "=", clase, "=", _expiration.ToString("dd/MM/yyyy HH:mm:ss")));

                            var _claims = new[]{
                                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName, pwdSys.Codificado),
                                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            };
                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("23901d6e-36e9-4278-a005-d927c471596d"));
                            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                            JwtSecurityToken token = new JwtSecurityToken(
                              issuer: "",
                              audience: "",
                              claims: _claims,
                              expires: _expiration,
                              signingCredentials: creds
                            );
                            respuesta.Respuesta.Token = new JwtSecurityTokenHandler().WriteToken(token);


                        }


                    }

                }

            }
            catch (Exception ex)
            {
                Log.escribirError(ex);
            }
            return respuesta;
        }

        [return: MessageParameter(Name = "Trazabilidad")]
        public RespuestaTrazabilidad GetTrazabilida(DtTrazabilidad DtTrazabilidad)
        {
            throw new NotImplementedException();
        }

        [return: MessageParameter(Name = "Pedido")]
        public RespuestaPedido SetPedido(DtPedido DtPedido)
        {
            throw new NotImplementedException();
        }
    }
}
