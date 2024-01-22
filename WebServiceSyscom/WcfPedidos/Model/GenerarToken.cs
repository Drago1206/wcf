using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Configuration;

namespace WcfPedidos.Model
{
    public class GenerarToken
    {
        public string Token { get; set; }

        /// <summary>
        /// Genera un nuevo token y el string de regreso es el mensaje de exito o error
        /// </summary>
        /// <param name="Proyecto">Nombre del proyecto.</param>
        /// <param name="usuario">Usuario.</param>
        /// <param name="contrasena">la contraseña.</param>
        /// <param name="NuToken">nuevo token creado</param>
        /// <returns></returns>
        public GenerarToken nuevoToken(string Proyecto, string usuario, string contrasena, out string[] mensaje)
        {
            if (!ConexionSQLite.configuracion)
            {
                ConexionSQLite.ComprobarBD();
            }

            if (ConexionSQLite.configuracion)
            {
                GenerarToken NuToken = null;
                NuToken = new GenerarToken();
                mensaje = null;
                //se realiza la verififcacion de los dato spara comprobar si el usuario si existe 
                ExisteUsuario VerificarUsuario = new ExisteUsuario();
                ExisteUsuario existe = new ExisteUsuario();
                if (existe.Existe(usuario, contrasena, out string[] mensajeNuevo, out string compania))
                {
                    //se genera el token
                    pwdSyscom pwdSys = new pwdSyscom();
                    DateTime _expiration = DateTime.Now.AddMinutes(5);
                    pwdSys.Codificar(string.Concat(usuario, "=", contrasena, "=", _expiration.ToString("dd/MM/yyyy HH:mm:ss"), "=", Proyecto, "=", compania));

                    var _claims = new[]{
                                 new Claim(JwtRegisteredClaimNames.UniqueName, pwdSys.Codificado),
                                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                             };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("23901d6e-36e9-4278-a005-d927c471596d"));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    JwtSecurityToken Token = new JwtSecurityToken(
                      issuer: "",
                      audience: "",
                      claims: _claims,
                      expires: _expiration,
                      signingCredentials: creds
                    );
                    NuToken.Token = new JwtSecurityTokenHandler().WriteToken(Token);
                    //enviamos los mensajes 
                    mensaje = new string[2];
                    mensaje[0] = "007";
                    mensaje[1] = "Token Creado Correctamente";
                    return NuToken;

                    //   _err = new ErrorRespuesta { Codigo = "007", Descripcion = "Token Creado Correctamente" + ConexionBD.BDSQLiteExiste };

                }
                else
                {
                    mensaje = mensajeNuevo;
                         return null;
                }
            }
            else
            {
                mensaje = new string[2];
                mensaje[0] = "079";
                mensaje[1] = "Error con la configuración de la base de datos SQlite" + "Data Source=" + System.AppDomain.CurrentDomain.BaseDirectory + "\\BD.reg;Version=3;" + ConexionSQLite.error; 
                return null;
            }

           
        }


        /// <summary>
        /// Verifica si el token Esta Vigente.
        /// </summary>
        /// <param name="token">Token a verificar.</param>
        /// <param name="NomProyecto">nombre del proyecto.</param>
        /// <param name="mensaje">Mensaje de respuesta.</param>
        /// <param name="Usuario">Devuelve el nombre del usuario que esta vinculado al token.</param>
        /// <returns></returns>
        public bool VerificarToken(string token, string NomProyecto, out string[] mensaje, out string Usuario, out string comp)
        {
            DateTime dateTimeOffset = new DateTime();
            DateTime dateTimeActual = new DateTime();
            Usuario = "";
            comp = "";
            // Validar el token de seguridad
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken tokenD = handler.ReadJwtToken(token.Contains("Bearer") ? token.Replace("Bearer", "").Trim() : token) as JwtSecurityToken;
            pwdSyscom pwd = new pwdSyscom();
            pwd.Decodificar(tokenD.Payload["unique_name"].ToString());
            string[] tokendecod = pwd.contrasenna.Split('=');

            if (tokendecod[3] != NomProyecto)
            {
                mensaje = null;
                mensaje[0] = "004";
                mensaje[1] = "Parámetro 'Token', No pertenece al servicio";
                return false;
            }
            else
            {
                // Validar la vigencia del token
                DateTime.TryParseExact(tokendecod[2], "dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out dateTimeOffset);
                string fechaActual = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                DateTime.TryParseExact(fechaActual, "dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out dateTimeActual);

                if (dateTimeOffset < dateTimeActual)
                {
                    mensaje = new string[2];
                    mensaje[0] = "006";
                    mensaje[1] = "El Token de Seguridad ha caducado ";
                    return false;
                }
                else
                {
                    //se realiza la verififcacion de los dato spara comprobar si el usuario si existe 
                    ExisteUsuario VerificarUsuario = new ExisteUsuario();
                    ExisteUsuario existe = new ExisteUsuario();
                    if (existe.Existe(tokendecod[0], tokendecod[1], out string[] mensajeNuevo, out string compania))
                    {
                        mensaje = mensajeNuevo;
                        Usuario = tokendecod[0];
                        comp = compania;
                        return true;
                    }
                    else
                    {
                        mensaje = mensajeNuevo;
                        return false;
                    }

                }
            }
        }
    }
}