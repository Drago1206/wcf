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
        private string _idVendedor;
        public Service1(string idVendedor)
        {
            _idVendedor = idVendedor;
        }

        public RespuestaWS GetToken(User User)
        {
            RespuestaWS respuesta = new RespuestaWS();
            respuesta.Respuesta = new Usuario();
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
                        if (con.ejecutarQuerys("WcfPedidosTrasn_ObtenerToken", parametros, out DT, out string[] nuevoMennsaje, CommandType.StoredProcedure)) {

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

       
        public RespuestaTrazabilidad GetTrazabilida(DtTrazabilidad DtTrazabilidad)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken tokenD = handler.ReadJwtToken(DtTrazabilidad.Token.Contains("Bearer") ? DtTrazabilidad.Token.Replace("Bearer", "").Trim() : DtTrazabilidad.Token) as JwtSecurityToken;
            pwdSyscom pwd = new pwdSyscom();
            pwd.Decodificar(tokenD.Payload["unique_name"].ToString());
            string[] tokendecod = pwd.contrasenna.Split('=');
            DateTime dateTimeOffset = Convert.ToDateTime(tokendecod[4]);
            RespuestaTrazabilidad respuesta = new RespuestaTrazabilidad();


            if (DtTrazabilidad.Token == null)
            {
                respuesta.Errores = new Errores { Codigo = "001", Descripcion = "¡El token es requerido!" };
            }
            else if (DtTrazabilidad.NumPedido == 0)
            {
                respuesta.Errores = new Errores { Codigo = "001", Descripcion = "¡El número de pedido es requerido!" };
            }
            else if (DtTrazabilidad.CiaPedido == null || String.IsNullOrWhiteSpace(DtTrazabilidad.CiaPedido))
            {
                respuesta.Errores = new Errores { Codigo = "003", Descripcion = "¡La compañía es requerida!" };
            }
            else {

                Model.Conexion con = new Model.Conexion();

                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@CiaPedido", DtTrazabilidad.CiaPedido));
                parametros.Add(new SqlParameter("@NumeroPedido", DtTrazabilidad.NumPedido));

                DataSet Tablainfo = new DataSet();
                DataTable Dt = new DataTable();


                if (dateTimeOffset > DateTime.Now)
                {
                    if (con.ejecutarQuery("WSPedidosTrans_Trazabilidad", parametros, out Tablainfo, out string[] nuevomennsaje, CommandType.StoredProcedure))
                    {
                        respuesta.Respuesta = con.DataTableToList<DatosTrazabilidad>("TipoDocumento,Numero,Compania,Fecha,Valor,Placa,Modelo,ClaseVeh,IdConductor,NomConductor,GPSoperador,GPSUsuario,GPSClave,FechaInicioCargue,HoraInicioCargue,FechaFinCargue,HoraFinCargue,FechaSalida,HoraSalida,Precinto".Split(','));
                        respuesta.Respuesta.ForEach(m =>
                        {
                            if (m.TipoDocumento == "RMT" || m.TipoDocumento == "MUC")
                            {
                                m.novedad = new List<itemnovedad>();
                                m.novedad = con.DataTableToList<itemnovedad>(Dt.AsEnumerable().Where(r => r["Item"] != DBNull.Value && r.Field<string>("TipoDocumento").Equals(m.TipoDocumento) && r.Field<int>("Numero").Equals(m.Numero) && r.Field<string>("Compania").Equals(m.Compania)).AsDataView().ToTable(true, "Item,Descripcion,Rubro,Total".Split(',')));
                            }
                        });
                    }
                }

            }

            return respuesta;
        }

        public RespuestaPedido SetPedido(DtPedido DtPedido)
        {
            RespuestaPedido respuesta = new RespuestaPedido();
            respuesta.Errores  = new Errores();
            Pedidos pedido = new Pedidos();
            DtPedido.Encabezado = new Pedidos();
            DtPedido.Detalle = new List<Items>();


            try
            {
                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken tokenD = handler.ReadJwtToken(DtPedido.Token.Contains("Bearer") ? DtPedido.Token.Replace("Bearer", "").Trim() : DtPedido.Token) as JwtSecurityToken;
                pwdSyscom pwd = new pwdSyscom();
                pwd.Decodificar(tokenD.Payload["unique_name"].ToString());
                string[] tokendecod = pwd.contrasenna.Split('=');
                DateTime dateTimeOffset = new DateTime();
                DateTime.TryParse(tokendecod[4], out dateTimeOffset);

                if (dateTimeOffset > DateTime.Now)
                {
                    if (DtPedido.Token == null || string.IsNullOrWhiteSpace(DtPedido.Token))
                    {
                        respuesta.Errores = new Errores { Codigo = "001", Descripcion = "¡El Token de Seguridad, NO puede ser nulo!" };
                    }
                    else
                    {
                        Model.Conexion con = new Model.Conexion();
                        con.setConnection("Syscom");
                        List<SqlParameter> parametros = new List<SqlParameter>();
                        List<SqlParameter> param = new List<SqlParameter>();

                        foreach (var item in DtPedido.Detalle)
                        {
                            parametros.Add(new SqlParameter("@NitRemitente", item.NitRemitente));
                            parametros.Add(new SqlParameter("@NitDestinatario", item.NitDestinatario));
                            parametros.Add(new SqlParameter("@IdMercancia", item.IdMercancia));
                  
                        }

                        DataSet Tablainfo = new DataSet();
                        DataSet TablaPedidos = new DataSet();
                        //Indicamos el parametro que vamos a pasar 
                        parametros.Add(new SqlParameter("@NitCliente", DtPedido.Encabezado.NitCliente));
                        parametros.Add(new SqlParameter("@IdUsuario", tokendecod));
                        parametros.Add(new SqlParameter("@IdAgencia", DtPedido.Encabezado.IdAgencia));
                        parametros.Add(new SqlParameter("@NitCiaPoliza", DtPedido.Encabezado.NitCiaPoliza));
                        parametros.Add(new SqlParameter("@NitSIA",DtPedido.Encabezado.NitSIA));
                        parametros.Add(new SqlParameter("@IdRuta", DtPedido.Encabezado.IdRuta));

                        con.addParametersProc(parametros);

                        
                        DataTable DT = new DataTable();
                        // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                        con.resetQuery();
                        //Verificamos y ejecutamos al mismo tiempo el procedimiento de almacenado 
                        if (con.ejecutarQuery("WcfPedidosTrasn_ObtenerPedido", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                        {
                           

                            
                            List<DataRow> _mercancia = Tablainfo.Tables[7].AsEnumerable().ToList();
                            List<DataRow> _remitente = Tablainfo.Tables[5].AsEnumerable().ToList();
                            List<DataRow> _destinatario = Tablainfo.Tables[6].AsEnumerable().ToList();
                            List<DataRow> _conceptos = Tablainfo.Tables[21].AsEnumerable().ToList();
                            List<DataRow> _permisos = Tablainfo.Tables[13].AsEnumerable().ToList();
                            List<DataRow> _opciones = Tablainfo.Tables[15].AsEnumerable().ToList();
                            List<DataRow> _riesgos = Tablainfo.Tables[18].AsEnumerable().ToList();
                            DataRow permisos = Tablainfo.Tables[13].Rows.Count > 0 ? Tablainfo.Tables[13].Rows[0] : null;
                            DataRow cliente = Tablainfo.Tables[3].Rows.Count > 0 ? Tablainfo.Tables[3].Rows[0] : null;
                            DataRow agencia = Tablainfo.Tables[4].Rows.Count > 0 ? Tablainfo.Tables[4].Rows[0] : null;
                            DataRow poliza = Tablainfo.Tables[8].Rows.Count > 0 ? Tablainfo.Tables[8].Rows[0] : null;
                            DataRow SIA = Tablainfo.Tables[9].Rows.Count > 0 ? Tablainfo.Tables[9].Rows[0] : null;
                            DataRow ruta = Tablainfo.Tables[12].Rows.Count > 0 ? Tablainfo.Tables[12].Rows[0] : null;
                            decimal suma = Convert.ToDecimal(DtPedido.Detalle.Sum(item => item.Peso));
                            int Item = 1;
                            //string TMO = "TMO";
                            string BLO = "BLO";
                            string CRT = "CRT";
                            string DVP = "DVP";
                            string EPT = "EPT";
                            //MAP // cliente en mora 
                            //CAP // cliente sin cupo
                            string formulario = "FRMDPDT";
                            bool permisoBLO = false;
                            DateTime fechaDespacho = DateTime.Now.Date;
                            string opcionCRT = _opciones.Where(pi => pi.Field<string>("IdOpc").Equals(CRT)).SingleOrDefault().Field<string>("Valor");
                            string opcionDVP = _opciones.Where(pi => pi.Field<string>("IdOpc").Equals(DVP)).SingleOrDefault().Field<string>("Valor");
                            string opcionEPT = _opciones.Where(pi => pi.Field<string>("IdOpc").Equals(EPT)).SingleOrDefault().Field<string>("Valor");
                            decimal VrCliente = 0;
                            decimal VrPago = 0;
                            decimal Vrtabla = 0;

                            DtPedido.Detalle.ToList().ForEach(n =>
                            {
                            
                                if (n.UnidadCliente.ToUpper() == "UNIDADES")
                                {
                                    VrCliente = VrCliente + Convert.ToDecimal(n.Cantidad * n.TarifaCliente);
                                }
                                else
                                {
                                    VrCliente = VrCliente + Convert.ToDecimal(n.Peso * n.TarifaCliente);
                                }
                               
                                if (n.UndTarifaPago.ToUpper() == "UNIDADES")
                                {
                                    VrPago = VrPago + Convert.ToDecimal(n.Cantidad * n.TarifaPago);
                                    Vrtabla = Vrtabla + Convert.ToDecimal(n.Cantidad * n.TarifaTabla);
                                }
                                else
                                {
                                    VrPago = VrPago + Convert.ToDecimal(n.Peso * n.TarifaPago);
                                    Vrtabla = Vrtabla + Convert.ToDecimal(n.Peso * n.TarifaTabla);
                                }
                            });
                            if (permisos != null)
                            {
                                permisoBLO = _permisos.Where(pi => pi.Field<string>("IdRole").Equals(BLO) && pi.Field<string>("IdObj").Equals(formulario)).SingleOrDefault().Field<bool>("Asignado");

                                if (cliente != null)
                                {
                                    if (permisoBLO == false && Convert.ToString(cliente["IdEstado"]) == "9999")
                                    {
                                        respuesta.Errores = new Errores { Codigo = "001", Descripcion = "Cliente bloqueado" };
                                    }
                                    else if (suma == 0)
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "El peso no puede ser cero" };
                                    }
                                    else if (DtPedido.Detalle.Any(d => d.UnidadCliente == null))
                                    {
                                            respuesta.Errores = new Errores { Codigo = "002", Descripcion = "¡La unidad de medida del cliente no puede ser null!" };
                                    }
                                    else if (DtPedido.Detalle.Any(d => d.Cantidad == 0 && (d.UnidadCliente.ToUpper() == "UNIDADES" || (d.UndTarifaPago == null ? "PESO" : d.UndTarifaPago.ToUpper()) == "UNIDADES")))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "¡La cantidad es obligatoria!" };
                                    }
                                    else if (DtPedido.Encabezado.Moneda != null && DtPedido.Encabezado.Moneda.Length > 5)
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "El codigo de la moneda no puede superar los 5 caracteres" };
                                    }
                                    else if (DtPedido.Conceptos != null && DtPedido.Conceptos.Any(d => !Tablainfo.Tables[21].AsEnumerable().Any(lt => d.CodigoConcepto == lt.Field<string>("IdConcepto"))))
                                    {
                                            respuesta.Errores = new Errores { Codigo = "002", Descripcion = "El codigo del concepto no puede ser nulo" }; 
                                    }
                                    else if (DtPedido.Conceptos != null && DtPedido.Conceptos.Any(d => d.Descripcion == null || d.Descripcion == ""))
                                    {
                                            respuesta.Errores = new Errores { Codigo = "002", Descripcion = "La descripcion del concepto es nula" };
                                    }
                                    else if (DtPedido.Conceptos != null && DtPedido.Conceptos.Any(d => d.Cantidad <= 0))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "La cantidad del concepto no puede ir nula" };
                                    }
                                    else if (DtPedido.Conceptos != null && DtPedido.Conceptos.Any(d => d.VrUnitario <= 0))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "El valor unitario del concepto no puede ser nulo" };
                                    }
                                    else if (DtPedido.Detalle.Any(d => !Tablainfo.Tables[5].AsEnumerable().Any(lt => d.NitRemitente == lt.Field<string>("IdTercero"))))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "El Remitente es null o no existe" };
                                    }
                                    else if (DtPedido.Detalle.Any(d => !Tablainfo.Tables[6].AsEnumerable().Any(lt => d.NitDestinatario == lt.Field<string>("IdTercero"))))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "El destinatario es nulo o no existe" };
                                    }
                                    else if (DtPedido.Detalle.Any(d => !Tablainfo.Tables[7].AsEnumerable().Any(lt => d.IdMercancia == lt.Field<string>("IdMercancia"))))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "El código de la mercancía es nula o no existe" };
                                    }
                                    else if (DtPedido.Detalle.Any(d => !Tablainfo.Tables[14].AsEnumerable().Any(lt => d.SedeDestinatario == lt.Field<string>("IdSede") && d.NitDestinatario == lt.Field<string>("IdTercero"))))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "La sede para el destinatario no existe " };
                                    }
                                    else if (DtPedido.Detalle.Any(d => !Tablainfo.Tables[11].AsEnumerable().Any(lt => d.SedeRemitente == lt.Field<string>("IdSede") && d.NitRemitente == lt.Field<string>("IdTercero"))))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "La sede para el remitente no existe" };

                                    }
                                    else if (DtPedido.Detalle.Any(d => d.CiudadOrigen == "" || d.CiudadOrigen == null))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "La ciudad de origen no existe" };
                                    }
                                    else if (DtPedido.Detalle.Any(d => !Tablainfo.Tables[16].AsEnumerable().Any(lt => (d.IdManejo == null ? "0" : d.IdManejo) == lt.Field<string>("IdMnjo"))))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "La unidad de embalaje es nula o no existe" };
                                    }
                                    else if (DtPedido.Detalle.Any(d => d.DirOrigen == "" || d.DirOrigen == null))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "¡La direccion de origen es obligatoria! " };
                                    }
                                    else if (DtPedido.Detalle.Any(d => !Tablainfo.Tables[16].AsEnumerable().Any(lt => (d.IdManejo == null ? "0" : d.IdManejo) == lt.Field<string>("IdMnjo"))))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "El código de manejo de mercancía es nulo o no existe" };
                                    }
                                    else if ((DtPedido.Detalle.Any(d => d.DirDestino == "" || d.DirDestino == null)))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "La direccion de destino es obligatoria" };
                                    }
                                    else if (DtPedido.Detalle.Any(d => d.TarifaCliente == 0))
                                    {

                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "La tarifa del cliente debe ser mayor a cero" };
                                    }
                                    else if (DtPedido.Detalle.Any(d => d.Tipo_Servicio == null || d.Tipo_Servicio == ""))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "La descripcion del tipo de servivio es requerida" };
                                    }
                                    else if (DtPedido.Detalle.Any(d => !Tablainfo.Tables[18].AsEnumerable().Any(lt => d.Riesgos == lt.Field<string>("TipoMcia"))))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "El riesgo es null o no existe" };

                                    }
                                    else if (DtPedido.Detalle.Any(d => !Tablainfo.Tables[19].AsEnumerable().Any(lt => d.UndMed == lt.Field<string>("UndMed"))))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "La unidad de medida es nula o no existe" };
                                    }
                                    else if (DtPedido.Detalle.Any(d => !Tablainfo.Tables[19].AsEnumerable().Any(lt => d.UndMed == lt.Field<string>("UndMed"))))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "El cd rango no puede tener mas de 4 caracteres" };
                                    }
                                    else if (DtPedido.Detalle.Any(d => d.IdEmpaque != null && d.IdEmpaque.Length > 4))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "El IdEmpaque no puede tener más de 4 caracteres" };
                                    }
                                    else if (opcionCRT == "MENSAJE" && (Convert.ToDecimal(Tablainfo.Tables[3].Rows[0]["VrSaldo"]) + VrCliente) > Convert.ToDecimal(Tablainfo.Tables[3].Rows[0]["VrCupo"]))
                                    {
                                        respuesta.Errores = new Errores { Codigo = "002", Descripcion = "Excede el límite de crédito" };
                                    }
                                    else
                                    {
                                        if (Tablainfo != null)
                                        {
                                            foreach (Conceptos d in DtPedido.Conceptos)
                                            {
                                               
                                                    DataRow row =  Tablainfo.Tables[20].NewRow();
                                                    row["TipDoc"] = "PDT";
                                                    row["Documento"] = (int)0;
                                                    row["IdCia"] = tokendecod[1];
                                                    row["Item"] = Item;
                                                    row["Descripcion"] = d.Descripcion;
                                                    row["Tarifa"] = (d.Cantidad * d.VrUnitario);
                                                    row["TipoConc"] = d.TipoConcepto == "" ? "CARGOS" : d.TipoConcepto;
                                                    row["Cantidad"] = d.Cantidad;
                                                    row["RubroConcep"] = d.Rubro == "" ? "CARGOS" : d.Rubro;
                                                    row["VrUnitario"] = d.VrUnitario;
                                                    row["TarifIva"] = Tablainfo.Tables[21].Rows[0]["TarifIva"] == DBNull.Value ? 0 : Tablainfo.Tables[21].Rows[0]["TarifIva"]; //_conceptos.Where(pi => pi.Field<string>("IdConcepto").Equals(d.CodigoConcepto)).SingleOrDefault().Field<decimal>("TarifIva");
                                                    row["IdConcepto"] = d.CodigoConcepto;
                                                    row["CdCuenta"] = 0;
                                                    row["NitTercero"] = d.Nit;
                                                    row["CdTipoEsc"] = d.TipoEscolta;
                                                    row["FechaNov"] = DtPedido.Encabezado.Fecha == null ? DateTime.Now.Date : DtPedido.Encabezado.Fecha;
                                                    row["VrBase"] = 0;
                                                    row["TipoTarif"] = _conceptos.Where(pi => pi.Field<string>("IdConcepto").Equals(d.CodigoConcepto)).SingleOrDefault().Field<string>("grupo") == "FACTURA" ? "$" : "%";
                                                    row["RefConc"] = "";
                                                    row["Fijos"] = false;
                                                    row["IncBaseRet"] = 0;
                                                    row["Referencia2"] = "";
                                                    row["Referencia3"] = "";
                                                Tablainfo.Tables[20].Rows.Add(row);
                                                param.Add(new SqlParameter("@dataTypeTrn_TraConceptos", Tablainfo.Tables[20]));
                                                Item++;

                                            }

                                            foreach (Items d in DtPedido.Detalle)
                                            {       
                                                    DataRow row = Tablainfo.Tables[1].NewRow();
                                                    row["TipDoc"] = "PDT";
                                                    row["Pedido"] = (int)0;
                                                    row["IdCia"] = tokendecod[1];
                                                    row["Item"] = Item;
                                                    row["IdMercancia"] = d.IdMercancia;
                                                    row["DescripMcias"] = _mercancia.Where(pi => pi.Field<string>("IdMercancia").Equals(d.IdMercancia)).SingleOrDefault().Field<string>("DescripMcia");
                                                    row["Cantidad"] = Convert.ToDecimal(d.Cantidad);
                                                    row["PesoNeto"] = d.Peso;
                                                    row["UndMed"] = d.UndMed;
                                                    row["dmsAlto"] = d.dmsAlto == null ? 0 : d.dmsAlto;
                                                    row["dmsAncho"] = d.dmsAncho == null ? 0 : d.dmsAncho;
                                                    row["dmsLargo"] = d.dmsLargo == null ? 0 : d.dmsLargo;
                                                    row["Volumen"] = d.Volumen == null ? 0 : d.Volumen;
                                                    row["UndVol"] = d.UndVol == null ? "m3" : d.UndVol;
                                                    row["IdUnd"] = d.Embalajes == null ? "0" : d.Embalajes;
                                                    row["IdEmp"] = d.IdEmpaque == null ? "0" : d.IdEmpaque;
                                                    row["IdNat"] = d.IdNaturaleza == null ? "0" : d.IdNaturaleza;
                                                    row["IdMnjo"] = d.IdManejo == null ? "0" : d.IdManejo;
                                                    row["IdTmcia"] = _riesgos.Where(pi => pi.Field<string>("TipoMcia").Equals(d.Riesgos)).SingleOrDefault().Field<string>("IdTmcia");
                                                    row["CdRango"] = d.CdRango;
                                                    row["Cases"] = d.Cases == null ? 0 : d.Cases;
                                                    row["Cajas"] = d.Cajas == null ? 0 : d.Cajas;
                                                    row["Palets"] = d.Palets == null ? 0 : d.Palets;
                                                    row["NitRemite"] = d.NitRemitente;
                                                    row["Remitente"] = _remitente.Where(pi => pi.Field<string>("IdTercero").Equals(d.NitRemitente)).SingleOrDefault().Field<string>("RazonSocial");
                                                    row["DirOrigen"] = d.DirOrigen;
                                                    row["IdOrigen"] = d.CiudadOrigen;
                                                    row["NitDestntario"] = d.NitDestinatario;
                                                    row["Destinatario"] = _destinatario.Where(pi => pi.Field<string>("IdTercero").Equals(d.NitDestinatario)).SingleOrDefault().Field<string>("RazonSocial"); ;
                                                    row["DirDestino"] = d.DirDestino;
                                                    row["IdDestino"] = d.CiudadDestino;
                                                    row["TarifClie"] = d.TarifaCliente;
                                                    row["TarifPago"] = d.TarifaPago == null ? 0 : d.TarifaPago;
                                                    row["TarifTabla"] = d.TarifaTabla == null ? 0 : d.TarifaTabla;
                                                    row["VrDeclarado"] = d.VrDeclarado == null ? 0 : d.VrDeclarado;
                                                    row["VrSeguro"] = d.TarifSeguro == null ? 0 : (d.VrDeclarado * d.TarifSeguro) / 100;
                                                    row["TarifSeguro"] = d.TarifSeguro == null ? 0 : d.TarifSeguro;
                                                    row["Referencia1"] = d.Referencia1 != null && d.Referencia1.Length > 50 ? d.Referencia1.Substring(0, 50) : d.Referencia1;
                                                    row["Referencia2"] = d.Referencia2 != null && d.Referencia2.Length > 50 ? d.Referencia2.Substring(0, 50) : d.Referencia2;
                                                    row["Contenedor1"] = d.Contenedor1;
                                                    row["Contenedor2"] = d.Contenedor2;
                                                    row["UndTarifa"] = d.UnidadCliente;
                                                    row["UndTarifPago"] = d.UndTarifaPago ?? "PESO";
                                                    row["DocCliente"] = d.DocCliente;
                                                    row["Referencia3"] = d.Referencia3 != null && d.Referencia3.Length > 50 ? d.Referencia3.Substring(0, 50) : d.Referencia3;
                                                    row["CdTipoVehic"] = DBNull.Value;
                                                    row["Tipo_Servicio"] = d.Tipo_Servicio;
                                                    row["SedeRem"] = d.SedeRemitente;
                                                    row["SedeDest"] = d.SedeDestinatario;
                                                    Tablainfo.Tables[1].Rows.Add(row);
                                                param.Add(new SqlParameter("@dataTypeTrn_TraPedAnexo", Tablainfo.Tables[1]));
                                                Item++;
                                                
                                            }

                                         
                                                DataRow rows = Tablainfo.Tables[0].NewRow();
                                                rows["TipDoc"] = "PDT";
                                                rows["Pedido"] = (int)0;
                                                rows["IdCia"] = tokendecod[1];
                                                rows["Fecha"] = DtPedido.Encabezado.Fecha == null ? DateTime.Now.Date : Convert.ToDateTime(DtPedido.Encabezado.Fecha).Date;
                                                rows["FechaVence"] = DtPedido.Encabezado.DiasVigencia == 0 ? (Convert.ToInt32(opcionDVP) == 0 ? DateTime.Now.Date : fechaDespacho.AddDays(Convert.ToInt32(opcionDVP))) : fechaDespacho.AddDays(DtPedido.Encabezado.DiasVigencia);
                                                rows["FecDespacho"] = DtPedido.Encabezado.FecDespacho == null ? DateTime.Now.Date : DtPedido.Encabezado.FecDespacho;
                                                rows["FecEntrega"] = Convert.ToDateTime(DtPedido.Encabezado.FecEntrega).Date;
                                                rows["IdCliente"] = Tablainfo.Tables[3].Rows[0]["IdTercero"];
                                                rows["IdAgencia"] = agencia == null ? 0 : Tablainfo.Tables[4].Rows[0]["CodAgencia"];
                                                rows["IdClieFact"] = Tablainfo.Tables[3].Rows[0]["IdTercero"];
                                                rows["IdRemitente"] = Tablainfo.Tables[5].Rows[0]["IdTercero"];
                                                rows["IdDestinatario"] = Tablainfo.Tables[6].Rows[0]["IdTercero"];
                                                rows["IdLocOrigen"] = Tablainfo.Tables[12].Rows[0]["IdLocOri"];
                                                rows["IdLocDestino"] = Tablainfo.Tables[12].Rows[0]["IdLocDes"];
                                                Item++;


                                                if (tokendecod[3] == "vendedor")
                                                {
                                                rows["IdVend"] = tokendecod[2];
                                                }


                                                rows["IdTarifCom"] = DtPedido.Encabezado.TarifaComision == null ? "C1" : DtPedido.Encabezado.TarifaComision;
                                                rows["Modalidad"] = DtPedido.Encabezado.Modalidad == null ? "ESTANDAR" : DtPedido.Encabezado.Modalidad;
                                                rows["Vigencia"] = DtPedido.Encabezado.Vigencia == null ? "NORMAL" : DtPedido.Encabezado.Vigencia;
                                                rows["TipoTarifa"] = DtPedido.Encabezado.TipoTarifa == null ? "PEDIDO" : DtPedido.Encabezado.TipoTarifa;
                                                rows["VrCobro"] = VrCliente;
                                                rows["VrPagos"] = VrPago;
                                                rows["VrFletes"] = Vrtabla;
                                                rows["VrCargue"] = DtPedido.Encabezado.VrCargue == null ? 0 : DtPedido.Encabezado.VrCargue;
                                                rows["VrDesCargue"] = DtPedido.Encabezado.VrDesCargue == null ? 0 : DtPedido.Encabezado.VrDesCargue;
                                                rows["VrEscolta"] = DtPedido.Encabezado.VrEscolta == null ? 0 : DtPedido.Encabezado.VrEscolta;
                                                rows["VrDevContdor"] = DtPedido.Encabezado.VrDevolucionContdor == null ? 0 : DtPedido.Encabezado.VrDevolucionContdor;
                                                rows["VrTraUrbano"] = DtPedido.Encabezado.VrTraUrbano == null ? 0 : DtPedido.Encabezado.VrTraUrbano;
                                                rows["VrEmbalajes"] = DtPedido.Encabezado.VrEmbalajes == null ? 0 : DtPedido.Encabezado.VrEmbalajes;
                                                rows["VrCargos"] = DtPedido.Encabezado.VrCargos == null ? 0 : DtPedido.Encabezado.VrCargos;
                                                rows["VrDctos"] = DtPedido.Encabezado.VrDctos == null ? 0 : DtPedido.Encabezado.VrDctos;
                                                rows["VrDeclarado"] = Tablainfo.Tables[1].AsEnumerable().Sum(vn => vn.Field<decimal>("VrDeclarado"));
                                                rows["VrSeguro"] = Tablainfo.Tables[1].AsEnumerable().Sum(vn => vn.Field<decimal>("VrSeguro"));
                                                rows["Cantidad"] = Convert.ToDecimal(suma);
                                                rows["CantDesp"] = (int)0;
                                                rows["IdMneda"] = DtPedido.Encabezado.Moneda == null ? "COP" : DtPedido.Encabezado.Moneda;
                                                rows["VrTasa"] = DtPedido.Encabezado.VrTasa == null ? 0 : DtPedido.Encabezado.VrTasa;
                                                rows["Cotizacion"] = (int)0;
                                                rows["IdCiaCot"] = "01";
                                                rows["NumAprob"] = opcionEPT == "1" ? 1999999999 : (int)0;
                                                rows["IdCiaApr"] = opcionEPT == "1" ? tokendecod[1] : "00";
                                                rows["FecAprob"] = opcionEPT == "1" ? DtPedido.Encabezado.Fecha.ToString("yyyy-MM-dd 00:00:00") : "NULL";
                                                rows["TipOdc"] = "OCT";
                                                rows["OCargue"] = (int)0;
                                                rows["IdCiaOdc"] = "01";
                                                rows["FechaOdc"] = DBNull.Value;
                                                rows["TipRem"] = "RMT";
                                                rows["Remesa"] = (int)0;
                                                rows["IdCiaRem"] = "01";
                                                rows["FechaRem"] = DBNull.Value;
                                                rows["TipFac"] = "0";
                                                rows["Factura"] = (int)0;
                                                rows["IdCiaFac"] = "00";
                                                rows["FechaFac"] = DBNull.Value;
                                                rows["OrigenAdd"] = "WSDocTrans";
                                                rows["Anulado"] = false;
                                                rows["FecDev"] = DBNull.Value;
                                                rows["Observacion"] = DtPedido.Encabezado.Observacion;
                                                rows["IdEstado"] = "0001";
                                                rows["TimeSys"] = DateTime.Now;
                                                rows["FecUpdate"] = DBNull.Value;
                                                rows["IdCiaCrea"] = tokendecod[1];
                                                rows["IdUsuario"] = tokendecod[0];

                                            string Rol = (Tablainfo.Tables[0].AsEnumerable().FirstOrDefault().Field<string>("EsVendedor"));
                                            if (Rol != null)
                                            {
                                                rows["IdVend"] = Rol;
                                            }
                                            else
                                            {
                                                parametros.Add(new SqlParameter("IdCliente", tokendecod[0]));
                                                string id = (Tablainfo.Tables[0].AsEnumerable().FirstOrDefault().Field<string>("IdVend"));
                                                rows["IdVend"] = id;

                                            }
                                            Tablainfo.Tables[0].Rows.Add(rows);
                                            param.Add(new SqlParameter("@dataTypeTrn_TraPedido", Tablainfo.Tables[0]));

                                            DataRow rowEncab = Tablainfo.Tables[2].NewRow();
                                            rowEncab["TipDoc"] = "PDT";
                                                rowEncab["Pedido"] = (int)0;
                                                rowEncab["IdCia"] = tokendecod[1];
                                                rowEncab["NomCliente"] = Tablainfo.Tables[3].Rows[0]["RazonSocial"];
                                                rowEncab["NomRemite"] = Tablainfo.Tables[1].Rows[0]["Remitente"];
                                                rowEncab["NomDestino"] = Tablainfo.Tables[1].Rows[0]["Destinatario"];
                                                rowEncab["LugarCargue"] = Tablainfo.Tables[1].Rows[0]["DirOrigen"];
                                                rowEncab["LugarDescargue"] = Tablainfo.Tables[1].Rows[0]["DirDestino"];
                                                rowEncab["NomContacto"] = DtPedido.Encabezado.NomContacto;
                                                rowEncab["TelContacto"] = DtPedido.Encabezado.TelContacto;
                                                rowEncab["emlContacto"] = DtPedido.Encabezado.emailContac;
                                                rowEncab["ContacDestino"] = DtPedido.Encabezado.NomContactoDest;
                                                rowEncab["TelContacDest"] = DtPedido.Encabezado.TelContactoDest;
                                                rowEncab["emlContacDest"] = DtPedido.Encabezado.emailContacDest;
                                                rowEncab["PolizaEsp"] = DtPedido.Encabezado.PolizaEspecifica;
                                                rowEncab["NumPolizaEsp"] = DtPedido.Encabezado.NumPolizaEsp == null ? "" : DtPedido.Encabezado.NumPolizaEsp;
                                                rowEncab["NitCiaPoliza"] = DtPedido.Encabezado.NitCiaPoliza == null ? "" : DtPedido.Encabezado.NitCiaPoliza;
                                                rowEncab["NomCiaPoliza"] = poliza == null ? "" : Tablainfo.Tables[8].Rows[0]["RazonSocial"];
                                                rowEncab["FecVencePol"] = DtPedido.Encabezado.FecVencePol == null ? DBNull.Value : (object)DtPedido.Encabezado.FecVencePol; //  == null ? DBNull.Value : DtPedido.Encabezado.FecVencePol
                                                rowEncab["VrLimiteDesp"] = DtPedido.Encabezado.VrLimiteDesp;
                                                rowEncab["Seguros"] = DtPedido.Encabezado.Seguros;
                                                rowEncab["Cargue"] = DtPedido.Encabezado.Cargue;
                                                rowEncab["Descargue"] = DtPedido.Encabezado.Descargue;
                                                rowEncab["CdTipoEsc"] = DtPedido.Encabezado.CdTipoEsc == null ? "" : DtPedido.Encabezado.CdTipoEsc;
                                                rowEncab["NitSIA"] = DtPedido.Encabezado.NitSIA;
                                                rowEncab["NombreSIA"] = SIA == null ? "" : Tablainfo.Tables[9].Rows[0]["RazonSocial"];
                                                rowEncab["ContactoSIA"] = DtPedido.Encabezado.ContactoSIA;
                                                rowEncab["TelContacSIA"] = DtPedido.Encabezado.TeleContactoSIA == null ? "" : DtPedido.Encabezado.TeleContactoSIA;
                                                rowEncab["TipoRuta"] = DtPedido.Encabezado.TipoRuta;
                                                rowEncab["TipoTrans"] = DtPedido.Encabezado.TipoTransporte == null ? "GENERAL" : DtPedido.Encabezado.TipoTransporte;
                                                rowEncab["Embarque"] = DtPedido.Encabezado.Embarque;
                                                rowEncab["CdTipCarga"] = DtPedido.Encabezado.CdTipCarga;
                                                rowEncab["DevContenedor"] = DtPedido.Encabezado.DevContenedor;
                                                rowEncab["IdLocCont"] = DtPedido.Encabezado.CiudadDevContenedor == null ? Tablainfo.Tables[12].Rows[0]["IdLocOri"] : DtPedido.Encabezado.CiudadDevContenedor;
                                                rowEncab["PatioCont"] = DtPedido.Encabezado.PatioCont == null ? "" : DtPedido.Encabezado.PatioCont;
                                                rowEncab["CdTipoVeh"] = DtPedido.Encabezado.CdTipoVehic == null ? "0" : DtPedido.Encabezado.CdTipoVehic;
                                                rowEncab["MargenFalt"] = DtPedido.Encabezado.MargenFalt;
                                                rowEncab["TipoMargen"] = DtPedido.Encabezado.TipoMargen;
                                                rowEncab["UndCalcFalt"] = DtPedido.Encabezado.UndCalcFalt;
                                                rowEncab["TarifFaltPago"] = DtPedido.Encabezado.TarifFaltPago;
                                                rowEncab["TarifFaltCobro"] = DtPedido.Encabezado.TarifFaltCobro;
                                                rowEncab["EmbAdicional"] = "";
                                                rowEncab["CdRutaTarif"] = DtPedido.Encabezado.IdRuta;
                                                rowEncab["TipoServicio"] = "";
                                                rowEncab["CodTipoOper"] = "";
                                            Tablainfo.Tables[2].Rows.Add(rowEncab);
                                            param.Add(new SqlParameter("@dataTypeTrn_TraPedMcias", Tablainfo.Tables[2]));

                                            if (con.ejecutarQuery("WSPedidosTrans_GenerarPedido", param, out TablaPedidos, out string[] nuevomennsaje, CommandType.StoredProcedure))
                                            {
                                                if (TablaPedidos != null)
                                                {
                                                    respuesta.Respuesta = con.ConvertRowToModel<Pedidos>();
                                                }
                                                else
                                                {
                                                    respuesta.Errores = new Errores {Codigo = "002", Descripcion = "No se pudo generar el pedido" };
                                                }
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    respuesta.Errores = new Errores { Codigo = "001", Descripcion = "No se encuentra el cliente: "+DtPedido.Encabezado.NitCliente };
                                }
                            }
                            else
                            {
                                respuesta.Errores = new Errores { Codigo = "001", Descripcion = "¡El Usuario no cuenta con el permiso necesario!" };
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                respuesta.Errores = new Errores { Codigo = "000", Descripcion = "Error" + e };
            }
            return null;
        }
    }
}
