using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfCortesEDS.Conexion;
using WcfCortesEDS.Model;

namespace WcfCortesEDS
{

    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IService1
    {
        string NomProyecto = "CortesEDS" + "-" + ConfigurationManager.AppSettings["NitEmpresa"];

        public RespToken generarToken(Usuarios usuario)
        {
            RespToken respuesta = new RespToken();
            respuesta.Registro = new Log();

            if (usuario == null)
            {
                respuesta.Registro = new Log { Codigo = "000", Descripcion = "Datos no valido" };
            }
            else if (string.IsNullOrWhiteSpace(usuario.Usuario) || string.IsNullOrWhiteSpace(usuario.Contraseña))
            {
                respuesta.Registro = new Log { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos" };
            }
            else
            {

                GenerarToken token = new GenerarToken();
                respuesta.Token = token.nuevoToken(NomProyecto, usuario.Usuario, usuario.Contraseña, out string[] mensajeNuevo);
                respuesta.Registro = new Log { Codigo = mensajeNuevo[0], Descripcion = mensajeNuevo[1] };
            }
            return respuesta;

        }



        public RestVentas Ventas(SolInfoVentas Info)
        {
            RestVentas respuesta = new RestVentas();
            respuesta.Registro = new Log();
            GenerarToken token = new GenerarToken();
            string Codigo = "";
            string Mensaje = "";

            Console.WriteLine("Debug: Antes de verificar el token.");

            if (token.VerificarToken(Info.token.Token, NomProyecto, out string[] nuevoMensaje, out string usuario))
            {
                Console.WriteLine("Debug: Después de verificar el token, antes de las condiciones.");
                if (Info.Insert.Prefijo.Length > 10)
                {
                    Codigo = "008";
                    Mensaje = "Parámetro Prefijo NO puede tener más de 10 caracteres!!!!";

                }
                else if (Info.Insert.CodProducto.Length > 30)
                {
                    Codigo = "009";
                    Mensaje = "Parámetro CodProducto NO puede tener más de 30 caracteres!!!!";

                }
                else if (Info.Insert.Producto.Length > 20)
                {
                    Codigo = "010";
                    Mensaje = "Parámetro Producto NO puede tener más de 20 caracteres!!!!";

                }
                else if (Info.Insert.TipoTransaccion.Length > 50)
                {
                    Codigo = "011";
                    Mensaje = "Parámetro TipoTransaccion NO puede tener más de 50 caracteres!!!!";

                }
                else if (Info.Insert.FormaPago.Length > 50)
                {
                    Codigo = "012";
                    Mensaje = "Parámetro FormaPago NO puede tener más de 50 caracteres!!!!";

                }
                else if(Info.Insert.Cuenta.Length > 100)
                {
                    Codigo = "013";
                    Mensaje = "Parámetro Cuenta NO puede tener más de 100 caracteres!!!!";

                }
                else if (Info.Insert.NIT.Length > 50)
                {

                    Codigo = "014";
                    Mensaje = "Parámetro NIT NO puede tener más de 50 caracteres!!!!";

                }
                else if (Info.Insert.Placa.Length > 10)
                {
                    Codigo = "015";
                    Mensaje = "Parámetro Placa NO puede tener más de 10 caracteres!!!!";

                }
                else if (Info.Insert.IdRom.Length > 250)
                {
                    Codigo = "016";
                    Mensaje = "Parámetro IdRom NO puede tener más de 250 caracteres!!!!";

                }
                else if (Info.Insert.CedulaConductor.Length > 20)
                {
                    Codigo = "017";
                    Mensaje = "Parámetro CedulaConductor NO puede tener más de 20 caracteres!!!!";

                }
                else if (Info.Insert.NombreConductor.Length > 100)
                {

                    Codigo = "018";
                    Mensaje = "Parámetro NombreConductor NO puede tener más de 100 caracteres!!!!";

                }
                else if (Info.Insert.CedulaVendedor.Length > 20)
                {
                    Codigo = "019";
                    Mensaje = "Parámetro CedulaVendedor NO puede tener más de 20 caracteres!!!!";

                }
                else if (Info.Insert.NombreVendedor.Length > 100)
                {
                    Codigo = "020";
                    Mensaje = "Parámetro NombreVendedor NO puede tener más de 100 caracteres!!!!";

                }
                else if (Info.Insert.FENumeroFactura.Length > 50)
                {
                    Codigo = "021";
                    Mensaje = "Parámetro FENumeroFactura NO puede tener más de 50 caracteres!!!!";

                }
                else if (Info.Insert.FETipoPersona.Length > 20)
                {
                    Codigo = "022";
                    Mensaje = "Parámetro FETipoPersona NO puede tener más de 20 caracteres!!!!";

                }
                else if (Info.Insert.FETipoDocumento.Length > 50)
                {
                    Codigo = "023";
                    Mensaje = "Parámetro FETipoDocumento NO puede tener más de 50 caracteres!!!!";

                }
                else if (Info.Insert.FEDigitoVerificacion.Length > 5)
                {
                    Codigo = "024";
                    Mensaje = "Parámetro FEDigitoVerificacion NO puede tener más de 5 caracteres!!!!";

                }
                else if (Info.Insert.FENumeroDocumento.Length > 20)
                {
                    Codigo = "025";
                    Mensaje = "Parámetro FENumeroDocumento NO puede tener más de 20 caracteres!!!!";
                }
                else if (Info.Insert.FENombreCliente.Length > 200)
                {
                    Codigo = "026";
                    Mensaje = "Parámetro FENombreCliente NO puede tener más de 200 caracteres!!!!";
                }
                else if (Info.Insert.FEDireccion.Length > 250)
                {
                    Codigo = "027";
                    Mensaje = "Parámetro FEDireccion NO puede tener más de 250 caracteres!!!!";
                }
                else if (Info.Insert.FETelefono.Length > 20)
                {
                    Codigo = "028";
                    Mensaje = "Parámetro FETelefono NO puede tener más de 20 caracteres!!!!";
                }
                else if (Info.Insert.FECorreo.Length > 250)
                {
                    Codigo = "029";
                    Mensaje = "Parámetro FECorreo NO puede tener más de 250 caracteres!!!!";
                }
                else if (Info.Insert.FECufe.Length > 500)
                {
                    Codigo = "030";
                    Mensaje = "Parámetro FECufe NO puede tener más de 500 caracteres!!!!";
                }
                else if (Info.Insert.FEQr.Length > 500)
                {
                    Codigo = "031";
                    Mensaje = "Parámetro FEQr NO puede tener más de 500 caracteres!!!!";
                }
                Console.WriteLine("Debug: Después de todas las condiciones.");




                ConexionBD ClassConexion = new ConexionBD();
                ConexionSQLite conSqlite = new ConexionSQLite("");
                string connectionString = conSqlite.obtenerConexionSyscom().ConnectionString;
                ClassConexion.setConnection(conSqlite.obtenerConexionSyscom());
                SqlConnectionStringBuilder infoCon = conSqlite.obtenerConexionSQLServer("dbsyscom");
                DataSet TablaInfo = new DataSet();
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Consecutivo", Info.Insert.Consecutivo));
                parametros.Add(new SqlParameter("@ConsecutivoDetalle", Info.Insert.ConsecutivoDetalle));
                parametros.Add(new SqlParameter("@Prefijo", Info.Insert.Prefijo));
                parametros.Add(new SqlParameter("@Factura", Info.Insert.Factura));
                parametros.Add(new SqlParameter("@Equipo", Info.Insert.Equipo));
                parametros.Add(new SqlParameter("@Cara", Info.Insert.Cara));
                parametros.Add(new SqlParameter("@Posicion", Info.Insert.Posicion));
                parametros.Add(new SqlParameter("@Manguera", Info.Insert.Manguera));
                parametros.Add(new SqlParameter("@Isla", Info.Insert.Isla));
                parametros.Add(new SqlParameter("@CodProducto", Info.Insert.CodProducto));
                parametros.Add(new SqlParameter("@Producto", Info.Insert.Producto));
                parametros.Add(new SqlParameter("@Cantidad", Info.Insert.Cantidad));
                parametros.Add(new SqlParameter("@ValorUnitario", Info.Insert.ValorUnitario));
                parametros.Add(new SqlParameter("@ValorTotal", Info.Insert.ValorTotal));
                parametros.Add(new SqlParameter("@FechaInicial", Info.Insert.FechaInicial));
                parametros.Add(new SqlParameter("@FechaFinal", Info.Insert.FechaFinal));
                parametros.Add(new SqlParameter("@LecturaVolumenInicial", Info.Insert.LecturaVolumenInicial));
                parametros.Add(new SqlParameter("@LecturaVolumenFinal", Info.Insert.LecturaVolumenFinal));
                parametros.Add(new SqlParameter("@LecturaDineroInicial", Info.Insert.LecturaDineroInicial));
                parametros.Add(new SqlParameter("@LecturaDineroFinal", Info.Insert.LecturaDineroFinal));
                parametros.Add(new SqlParameter("@IdTipoTransaccion", Info.Insert.IdTipoTransaccion));
                parametros.Add(new SqlParameter("@TipoTransaccion", Info.Insert.TipoTransaccion));
                parametros.Add(new SqlParameter("@IdFormaPago", Info.Insert.IdFormaPago));
                parametros.Add(new SqlParameter("@FormaPago", Info.Insert.FormaPago));
                parametros.Add(new SqlParameter("@Cuenta", Info.Insert.Cuenta));
                parametros.Add(new SqlParameter("@NIT", Info.Insert.NIT));
                parametros.Add(new SqlParameter("@Placa", Info.Insert.Placa));
                parametros.Add(new SqlParameter("@IdRom", Info.Insert.IdRom));
                parametros.Add(new SqlParameter("@Kilometraje", Info.Insert.Kilometraje));
                parametros.Add(new SqlParameter("@CedulaConductor", Info.Insert.CedulaConductor));
                parametros.Add(new SqlParameter("@NombreConductor", Info.Insert.NombreConductor));
                parametros.Add(new SqlParameter("@Turno", Info.Insert.Turno));
                parametros.Add(new SqlParameter("@CedulaVendedor", Info.Insert.CedulaVendedor));
                parametros.Add(new SqlParameter("@NombreVendedor", Info.Insert.NombreVendedor));
                parametros.Add(new SqlParameter("@Corte", Info.Insert.Corte));
                parametros.Add(new SqlParameter("@FEFechaFactura", Info.Insert.FEFechaFactura));
                parametros.Add(new SqlParameter("@FENumeroFactura", Info.Insert.FENumeroFactura));
                parametros.Add(new SqlParameter("@FETipoPersona", Info.Insert.FETipoPersona));
                parametros.Add(new SqlParameter("@FETipoDocumento", Info.Insert.FETipoDocumento));
                parametros.Add(new SqlParameter("@FEDigitoVerificacion", Info.Insert.FEDigitoVerificacion));
                parametros.Add(new SqlParameter("@FENumeroDocumento", Info.Insert.FENumeroDocumento));
                parametros.Add(new SqlParameter("@FENombreCliente", Info.Insert.FENombreCliente));
                parametros.Add(new SqlParameter("@FEDireccion", Info.Insert.FEDireccion));
                parametros.Add(new SqlParameter("@FETelefono", Info.Insert.FETelefono));
                parametros.Add(new SqlParameter("@FECorreo", Info.Insert.FECorreo));
                parametros.Add(new SqlParameter("@FECufe", Info.Insert.FECufe));
                parametros.Add(new SqlParameter("@FEQr", Info.Insert.FEQr));
                parametros.Add(new SqlParameter("@FacturaContingencia", Info.Insert.FacturaContingencia));
                parametros.Add(new SqlParameter("@ValorFP", Info.Insert.ValorFP));


                if (ClassConexion.ejecutarQuery("WSCortesEDS_AgregarInfoVentas", parametros, out TablaInfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                {
                    Console.WriteLine("Debug: Query ejecutado con éxito."); // Agregamos un mensaje de depuración
                    Codigo = "000"; // Código de éxito
                    Mensaje = "Operación exitosa";

                }
                else
                {
                    Console.WriteLine("Debug: Verificación de token fallida."); // Agregamos un mensaje de depuración
                    Codigo = nuevoMensaje[0];
                    Mensaje = nuevoMensaje[1];

                }

               
            }

            else
            {
                Console.WriteLine("Debug: Verificación de token fallida."); // Agregamos un mensaje de depuración
                Codigo = nuevoMensaje[0];
                Mensaje = nuevoMensaje[1];
            }

            respuesta.Registro = new Log { Codigo = Codigo, Descripcion = Mensaje, Fecha = DateTime.Now.ToString("yyyy-MM-dd") };
            return respuesta;
        }


    }
}
