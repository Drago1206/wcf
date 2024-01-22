using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos.Model;
using System.Globalization;
using System.Configuration;
using Newtonsoft.Json;

namespace WcfPedidos.Model
{
    public class Pedido
    {
        public ResGenerarPedido GenerarPedido(GenPedido consulta, string usuario, string compania)
        {
            string Codigo = "";
            string Mensaje = "";
            ResGenerarPedido respuesta = new ResGenerarPedido();
            respuesta.Registro = new Log();

            try
            {
                //variables para hacer el pedido
                int DiasEntrega = 0;
                DateTime FechaPedido = DateTime.Now;
                string FormaDePago = "";
                string NitContacto = "";
                string NombreContacto = "";
                string TelefonoContacto = "";
                string EmailContacto = "";
                string CargoContacto = "";
                string DiasDePlazo = "";
                string IdVendedor = "";
                //validar la informacion para la cvompañia de registro del pedido
                string Cia = compania;
                string Bodega = "";
                string TarifaVendedor = "";
                decimal ValorTarifaVendedor = 0;
                string agencia = "0";
                string Localidad = "";
                string Ruta = "0";
                string Plazo = "0";

                string pmFormaPago = "";
                if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmFormaPago))
                {
                    pmFormaPago = consulta.DatosDelPedido.pmFormaPago;
                }
                if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmIdVendedor))
                {
                    IdVendedor = consulta.DatosDelPedido.pmIdVendedor;
                }
                else
                {
                    IdVendedor = usuario;
                }
                if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmIdBodega))
                {
                    Bodega = consulta.DatosDelPedido.pmIdBodega;
                }
                if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmTarifaComision))
                {
                    TarifaVendedor = consulta.DatosDelPedido.pmTarifaComision;
                }
                if (!string.IsNullOrEmpty(consulta.Cliente.CdAgencia))
                {
                    agencia = consulta.Cliente.CdAgencia;
                }

                if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmIdCompania))
                {
                    Cia = consulta.DatosDelPedido.pmIdCompania;
                }

                if (!string.IsNullOrEmpty(consulta.Cliente.Municipio))
                {
                    Localidad = consulta.Cliente.Municipio;
                }
                if (!string.IsNullOrEmpty(consulta.Cliente.Ruta))
                {
                    Ruta = consulta.Cliente.Ruta;
                }
                if (!string.IsNullOrEmpty(consulta.Cliente.Plazo))
                {
                    Plazo = consulta.Cliente.Plazo;
                }

                string productosingresados = "";
                string tanqueingresado = "";
                string TarifaDescuento = "";

                //organizamos los datos de producto 
                foreach (ProductosPed productos in consulta.Productos)
                {
                    if (productosingresados == "")
                    {
                        productosingresados += productos.pmIdProducto;
                        if (!string.IsNullOrEmpty(productos.pmIdTanque))
                        {
                            tanqueingresado = productos.pmIdTanque;
                        }
                        if (!string.IsNullOrEmpty(productos.pmIdTarDcto))
                        {
                            TarifaDescuento = productos.pmIdTarDcto;
                        }

                    }
                    else
                    {
                        productosingresados += "," + productos.pmIdProducto;
                        if (!string.IsNullOrEmpty(productos.pmIdTanque))
                        {
                            tanqueingresado += "," + productos.pmIdTanque;
                        }
                        else
                        {
                            tanqueingresado += ",";
                        }

                        if (!string.IsNullOrEmpty(productos.pmIdTarDcto))
                        {
                            TarifaDescuento += "," + productos.pmIdTarDcto;
                        }
                        else
                        {
                            TarifaDescuento += ",";
                        }
                    }

                }


                //realizamos el consulta de los permisos
                ConexionBD ClassConexion = new ConexionBD();
                ConexionSQLite conSqlite = new ConexionSQLite("");
                string connectionString = conSqlite.obtenerConexionSyscom().ConnectionString;
                ClassConexion.setConnection(conSqlite.obtenerConexionSyscom());
                SqlConnectionStringBuilder infoCon = conSqlite.obtenerConexionSQLServer("dbsyscom");
                DataSet TablaPermisos = new DataSet();
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Usuario", usuario));
                parametros.Add(new SqlParameter("@Cia", compania));
                parametros.Add(new SqlParameter("@Cliente", consulta.Cliente.Documento));
                parametros.Add(new SqlParameter("@FormaPago", pmFormaPago));
                parametros.Add(new SqlParameter("@vendedor", IdVendedor));
                parametros.Add(new SqlParameter("@Bodega", Bodega));
                parametros.Add(new SqlParameter("@TarifaVendedor", TarifaVendedor));
                parametros.Add(new SqlParameter("@Agencia", agencia));
                parametros.Add(new SqlParameter("@Productos", productosingresados));
                parametros.Add(new SqlParameter("@Tanques", tanqueingresado));
                parametros.Add(new SqlParameter("@CiIngresado", Cia));
                parametros.Add(new SqlParameter("@TarifaDescuentos", TarifaDescuento));
                parametros.Add(new SqlParameter("@localidad", Localidad));
                parametros.Add(new SqlParameter("@Ruta", Ruta));
                parametros.Add(new SqlParameter("@Plazo", Plazo));


                if (ClassConexion.ejecutarQuery("WSPedido_ConsPerVenta", parametros, out TablaPermisos, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                {
                    if (TablaPermisos.Tables[0].Rows.Count > 0)
                    {
                        DataRow Permisos = TablaPermisos.Tables[0].Rows[0];

                        //comprobar que tenga habilidato el permiso de vendedor
                        if (Permisos.Field<bool>("Esvendedor"))
                        {
                            //Que tenga un nivel 3 en adelante para hacer pedido
                            if (Permisos.Field<int>("Nivel") >= 3)
                            {
                                //Comprobar que tenga habilitado el permiso de hacer pedido
                                if (Permisos.Field<string>("Permiso") == "FRMDPED")
                                {
                                    //comprobar que el cliente no se encuentr bloqueado y tenga permiso para desbloquear                                 
                                    if (Permisos.Field<string>("Cliente") == "activo")
                                    {
                                        //Validar que el cliente se encuentre en la compañia
                                        if (Permisos.Field<string>("HabCompania") == "permitido")
                                        {
                                            //validamos si cumple con la localidad del pedido 
                                            if ((Permisos.Field<string>("Localidad") != "denegado"))
                                            {
                                                //verificamos la ruta
                                                if (Permisos.Field<string>("ruta") != "denegado")
                                                {
                                                    //verificamos el estado del pedido si tiene permisdo o no
                                                    if ((Permisos.Field<string>("PermisoEST") != "den") | (string.IsNullOrEmpty(consulta.DatosDelPedido.pmEstadoPedido)) | (consulta.DatosDelPedido.pmEstadoPedido == "0002"))
                                                    {
                                                        if (string.IsNullOrEmpty(consulta.DatosDelPedido.pmEstadoPedido))
                                                        {
                                                            consulta.DatosDelPedido.pmEstadoPedido = "0001";
                                                        }

                                                        if ((consulta.DatosDelPedido.pmEstadoPedido == "0002") | (consulta.DatosDelPedido.pmEstadoPedido == "0001"))
                                                        {

                                                            //verificamos que el plazo ingresado sea el adecuado
                                                            if (Permisos.Field<string>("Plazo") != "denegado")
                                                            {
                                                                //se valida que el plazo que coloca el usuario sea el mismo al del cliente o que tenga el permiso PZO
                                                                if (Permisos.Field<string>("PermisoPZO") == "PZO" | Permisos.Field<string>("plazoCliente") == Plazo)
                                                                {
                                                                    //validar si esta en mora 
                                                                    if (Permisos.Field<string>("PermisoMor") == "MOR" | Permisos.Field<string>("EstadoMora") == "AlDia")
                                                                    {
                                                                        //validamos la agencia
                                                                        bool permisoAgencia = false;

                                                                        if (!string.IsNullOrEmpty(consulta.Cliente.CdAgencia))
                                                                        {
                                                                            if (Permisos.Field<string>("ExisteAgencia") != "denegado")
                                                                            {
                                                                                permisoAgencia = true;
                                                                            }
                                                                            else
                                                                            {
                                                                                Codigo = "044";
                                                                                Mensaje = "La agencia agregada no pertenece al cliente";
                                                                            }

                                                                        }
                                                                        else
                                                                        {
                                                                            permisoAgencia = true;
                                                                            if (Permisos.Field<string>("AgenciaAsiganada") != "denegado")
                                                                            {
                                                                                agencia = Permisos.Field<string>("AgenciaAsiganada");
                                                                            }
                                                                        }

                                                                        if (permisoAgencia)
                                                                        {
                                                                            //validamos que la tarifa ingresada exita
                                                                            if (Permisos.Field<string>("ExisteTarifa") != "dene" | string.IsNullOrEmpty(consulta.DatosDelPedido.pmTarifaComision))
                                                                            {
                                                                                bool permisoTarifa = false;

                                                                                if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmTarifaComision))
                                                                                {
                                                                                    if (Permisos.Field<string>("ExisteTarifa") == "dene")
                                                                                    {
                                                                                        Codigo = "046";
                                                                                        Mensaje = "El código de pmTarifaComision no existe en syscom";
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        //validamos para que sea igual 
                                                                                        if ((Permisos.Field<string>("PermisoCms") == "CMS") | (Permisos.Field<string>("ExisteTarifa") == Permisos.Field<string>("TarifaVendedor")))
                                                                                        {
                                                                                            TarifaVendedor = Permisos.Field<string>("ExisteTarifa");
                                                                                            ValorTarifaVendedor = Permisos.Field<decimal>("ValorTarifaIngre");
                                                                                            permisoTarifa = true;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            Codigo = "075";
                                                                                            Mensaje = "El usuario no tiene permiso CMS para cambiar la tarifa";
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    TarifaVendedor = Permisos.Field<string>("TarifaVendedor");
                                                                                    ValorTarifaVendedor = Permisos.Field<decimal>("ValorTarifaExis");
                                                                                    permisoTarifa = true;
                                                                                }

                                                                                if (permisoTarifa)
                                                                                {
                                                                                    //validar que si tiene el permiso EGA y si ingreso un valor en diasEntrega
                                                                                    if ((Permisos.Field<string>("PermisoEGA") == "EGA") | (consulta.Cliente.DiasEntrega == 0) | (consulta.Cliente.DiasEntrega == Permisos.Field<int>("DiasEntrega")))
                                                                                    {
                                                                                        DiasEntrega = consulta.Cliente.DiasEntrega;
                                                                                        bool permisoParaFEC = false;
                                                                                        //se realiza la evaluacion de los datos para saber si cumple con las  condiciones para la fecha
                                                                                        if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmFechaPedido))
                                                                                        {
                                                                                            if (DateTime.TryParse(consulta.DatosDelPedido.pmFechaPedido, out DateTime fechaingresada))
                                                                                            {
                                                                                                if (DateTime.TryParse(Permisos.Field<string>("FechaDelPedidod"), out DateTime fechabase))
                                                                                                {
                                                                                                    if (fechaingresada.ToString("yyyyMMdd") == fechabase.ToString("yyyyMMdd"))
                                                                                                    {
                                                                                                        FechaPedido = fechaingresada;
                                                                                                        permisoParaFEC = true;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Codigo = "023";
                                                                                                        Mensaje = "No tiene el permiso habilitado fecha abierta";
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    //verificamos que el formato que tenemos y si cumple 
                                                                                                    if (Permisos.Field<string>("PermisoFEC") == "FEC")
                                                                                                    {
                                                                                                        //comprobamos los valores que estan
                                                                                                        if (Permisos.Field<string>("FechaDelPedidod") == "D")
                                                                                                        {
                                                                                                            if (fechaingresada.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                                                                                                            {
                                                                                                                FechaPedido = fechaingresada;
                                                                                                                permisoParaFEC = true;
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Codigo = "024";
                                                                                                                Mensaje = "La fecha no válida debe ser la fecha de dia de hoy.";
                                                                                                            }
                                                                                                        }
                                                                                                        else if (Permisos.Field<string>("FechaDelPedidod") == "S")
                                                                                                        {
                                                                                                            DateTime fechaActual = DateTime.Now; // Fecha y hora actual

                                                                                                            // Calcular el primer día de la semana actual
                                                                                                            DateTime primerDiaSemana = fechaActual.Date.AddDays(-(int)fechaActual.DayOfWeek);

                                                                                                            // Calcular el último día de la semana actual
                                                                                                            DateTime ultimoDiaSemana = primerDiaSemana.AddDays(6);

                                                                                                            if (fechaingresada < primerDiaSemana || fechaingresada > ultimoDiaSemana)
                                                                                                            {
                                                                                                                Codigo = "025";
                                                                                                                Mensaje = "la fecha ingresada debe coincidir con la semana actual";
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                FechaPedido = fechaingresada;
                                                                                                                permisoParaFEC = true;
                                                                                                            }

                                                                                                        }
                                                                                                        else if (Permisos.Field<string>("FechaDelPedidod") == "M")
                                                                                                        {
                                                                                                            if (fechaingresada.ToString("yyyyMM") == DateTime.Now.ToString("yyyyMM"))
                                                                                                            {
                                                                                                                FechaPedido = fechaingresada;
                                                                                                                permisoParaFEC = true;
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Codigo = "026";
                                                                                                                Mensaje = "La fecha ingresada debe coincidir con el mes actual";
                                                                                                            }
                                                                                                        }
                                                                                                        else if (Permisos.Field<string>("FechaDelPedidod") == "A")
                                                                                                        {
                                                                                                            if (fechaingresada.Year == DateTime.Now.Year)
                                                                                                            {
                                                                                                                FechaPedido = fechaingresada;
                                                                                                                permisoParaFEC = true;
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Codigo = "027";
                                                                                                                Mensaje = "La fecha ingresada debe coincidir con el año actual";
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (double.TryParse(Permisos.Field<string>("FechaDelPedidod"), out double numero))
                                                                                                            {
                                                                                                                //se convierte el valor 
                                                                                                                if (numero > 0)
                                                                                                                {
                                                                                                                    DateTime fechaActual = DateTime.Now; // Fecha y hora actual

                                                                                                                    // Calcular el primer día de la semana actual
                                                                                                                    DateTime primerDia = fechaActual;

                                                                                                                    // Calcular el último día de la semana actual
                                                                                                                    DateTime ultimoDiaSemana = primerDia.AddDays(numero);

                                                                                                                    if (fechaingresada < primerDia || fechaingresada > ultimoDiaSemana)
                                                                                                                    {
                                                                                                                        Codigo = "035";
                                                                                                                        Mensaje = "la fecha ingresada debe coincidir con los dias establecidos";
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        FechaPedido = fechaingresada;
                                                                                                                        permisoParaFEC = true;
                                                                                                                    }

                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    DateTime fechaActual = DateTime.Now; // Fecha y hora actual

                                                                                                                    // Calcular el primer día de la semana actual
                                                                                                                    DateTime primerDia = fechaActual.AddDays(numero);

                                                                                                                    // Calcular el último día de la semana actual
                                                                                                                    DateTime ultimoDiaSemana = fechaActual;

                                                                                                                    if (fechaingresada < primerDia || fechaingresada > ultimoDiaSemana)
                                                                                                                    {
                                                                                                                        Codigo = "035";
                                                                                                                        Mensaje = "la fecha ingresada debe coincidir con los días establecidos.";
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        FechaPedido = fechaingresada;
                                                                                                                        permisoParaFEC = true;
                                                                                                                    }

                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Codigo = "082";
                                                                                                                Mensaje = "No se ha establecido la fecha abierta en Syscom en la compañia";
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Codigo = "081";
                                                                                                        Mensaje = "el usuario no tiene permiso FEC 'Habilitar Fecha Abierta'";
                                                                                                    }
                                                                                                }

                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Codigo = "022";
                                                                                                Mensaje = "el formato de pmFechaPedido que ingreso no es el correcto. el formato correcto es año-mes-día hora:minuto:segundo ejemplo 2024-01-25 13:13:00 ";
                                                                                            }

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            permisoParaFEC = true;
                                                                                        }

                                                                                        if (permisoParaFEC)
                                                                                        {
                                                                                            //comprobar forma de pago del cliente
                                                                                            bool permisoFOR = false;
                                                                                            if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmFormaPago))
                                                                                            {
                                                                                                if (Permisos.Field<string>("FormaDePagoIngresado") != "dene")
                                                                                                {
                                                                                                    if (Permisos.Field<string>("FormaDePagoIngresado") == Permisos.Field<string>("FormaDePago"))
                                                                                                    {
                                                                                                        FormaDePago = Permisos.Field<string>("FormaDePago");
                                                                                                        permisoFOR = true;
                                                                                                    }
                                                                                                    else if (Permisos.Field<string>("PermisoFOR") == "FOR")
                                                                                                    {
                                                                                                        FormaDePago = Permisos.Field<string>("FormaDePagoIngresado");
                                                                                                        permisoFOR = true;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Codigo = "029";
                                                                                                        Mensaje = "El usuario no tiene permiso para cambiar la forma de pago";
                                                                                                    }

                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    Codigo = "028";
                                                                                                    Mensaje = "la forma de pago ingresado no está registrada en syscom.";
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                FormaDePago = Permisos.Field<string>("FormaDePago");
                                                                                                permisoFOR = true;
                                                                                            }
                                                                                            if (permisoFOR)
                                                                                            {
                                                                                                //verificamos la informacion de contacto y si cuenta con el permiso 
                                                                                                bool permisoMCO = false;
                                                                                                //comprobamos si hay un valor ingresado
                                                                                                if (!string.IsNullOrEmpty(consulta.InformacionDeContacto.NitContacto) | !string.IsNullOrEmpty(consulta.InformacionDeContacto.NombreContacto) | !string.IsNullOrEmpty(consulta.InformacionDeContacto.TelefonoContacto) | !string.IsNullOrEmpty(consulta.InformacionDeContacto.EmailContacto) | !string.IsNullOrEmpty(consulta.InformacionDeContacto.CargoContacto))
                                                                                                {
                                                                                                    if (Permisos.Field<string>("PermisoMCO") == "MCO")
                                                                                                    {
                                                                                                        if (!string.IsNullOrEmpty(consulta.InformacionDeContacto.NitContacto))
                                                                                                        {
                                                                                                            NitContacto = consulta.InformacionDeContacto.NitContacto;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            NitContacto = Permisos.Field<string>("ContaNIt");
                                                                                                        }

                                                                                                        if (!string.IsNullOrEmpty(consulta.InformacionDeContacto.NombreContacto))
                                                                                                        {
                                                                                                            NombreContacto = consulta.InformacionDeContacto.NombreContacto;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            NombreContacto = Permisos.Field<string>("ContaNombre");
                                                                                                        }

                                                                                                        if (!string.IsNullOrEmpty(consulta.InformacionDeContacto.TelefonoContacto))
                                                                                                        {
                                                                                                            TelefonoContacto = consulta.InformacionDeContacto.TelefonoContacto;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            TelefonoContacto = Permisos.Field<string>("ContaTelefono");
                                                                                                        }

                                                                                                        if (!string.IsNullOrEmpty(consulta.InformacionDeContacto.EmailContacto))
                                                                                                        {
                                                                                                            EmailContacto = consulta.InformacionDeContacto.EmailContacto;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            EmailContacto = Permisos.Field<string>("ContaEmail");
                                                                                                        }

                                                                                                        if (!string.IsNullOrEmpty(consulta.InformacionDeContacto.CargoContacto))
                                                                                                        {
                                                                                                            CargoContacto = consulta.InformacionDeContacto.CargoContacto;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            CargoContacto = Permisos.Field<string>("ContaCargo");
                                                                                                        }
                                                                                                        permisoMCO = true;


                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Codigo = "030";
                                                                                                        Mensaje = "El usuario no tiene permiso para cambiar la información de contacto.";
                                                                                                    }

                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    permisoMCO = true;
                                                                                                    NitContacto = Permisos.Field<string>("ContaNIt");
                                                                                                    NombreContacto = Permisos.Field<string>("ContaNombre");
                                                                                                    TelefonoContacto = Permisos.Field<string>("ContaTelefono");
                                                                                                    EmailContacto = Permisos.Field<string>("ContaEmail");
                                                                                                    CargoContacto = Permisos.Field<string>("ContaCargo");
                                                                                                }
                                                                                                if (permisoMCO)
                                                                                                {
                                                                                                    bool permisoVEN = false;
                                                                                                    //permiso para vender a un cliente que no tiene asignado el vendedor
                                                                                                    if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmIdVendedor))
                                                                                                    {
                                                                                                        if ((Permisos.Field<string>("PermisoVEN") == "VEN") | (consulta.DatosDelPedido.pmIdVendedor == Permisos.Field<string>("VendedorDelCLiente")))
                                                                                                        {
                                                                                                            if (Permisos.Field<bool>("EsVendedorIngresado"))
                                                                                                            {
                                                                                                                permisoVEN = true;
                                                                                                                IdVendedor = consulta.DatosDelPedido.pmIdVendedor;
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Codigo = "036";
                                                                                                                Mensaje = "El pmIdVendedor ingresado no está marcado como vendedor";
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            Codigo = "034";
                                                                                                            Mensaje = "El usuario no tiene permiso VEN para vender a otro cliente";
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (usuario == Permisos.Field<string>("VendedorDelCLiente"))
                                                                                                        {
                                                                                                            permisoVEN = true;
                                                                                                            IdVendedor = usuario;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            Codigo = "032";
                                                                                                            Mensaje = "El usuario no tiene permiso VEN para vender a otro cliente";
                                                                                                        }

                                                                                                    }

                                                                                                    //comprobamos el valor de la compañia para registrarlo
                                                                                                    if ((Permisos.Field<string>("PermisoCia") == "CIA") | (consulta.DatosDelPedido.pmIdCompania == compania))
                                                                                                    {
                                                                                                        Cia = consulta.DatosDelPedido.pmIdCompania;

                                                                                                        //comprobamos para añadir una tarifa diferente a la del vendedor
                                                                                                        if (permisoVEN)
                                                                                                        {

                                                                                                            if (TablaPermisos.Tables[1].Rows.Count > 0)
                                                                                                            {

                                                                                                                bool permisoLTA = false;

                                                                                                                bool permisoDCT = false;
                                                                                                                bool permisoOBQ = false;
                                                                                                                if (Permisos.Field<string>("PermisoOBQ") == "OBQ")
                                                                                                                {
                                                                                                                    permisoOBQ = true;
                                                                                                                }
                                                                                                                bool permisoPEC = false;
                                                                                                                bool permisoBOD = false;
                                                                                                                bool verificacionCompleta = true;
                                                                                                                //verificamos la lista preterminada 
                                                                                                                if (Permisos.Field<string>("PermisoLTA") == "LTA")
                                                                                                                {
                                                                                                                    permisoLTA = true;
                                                                                                                }

                                                                                                                //permiso para agregar una bodega 
                                                                                                                if (Permisos.Field<string>("PermisoBod") == "BOD")
                                                                                                                {
                                                                                                                    permisoBOD = true;
                                                                                                                }
                                                                                                                if (Permisos.Field<string>("PermisoDCT") == "DCT")
                                                                                                                {
                                                                                                                    permisoDCT = true;
                                                                                                                }

                                                                                                                DataTable productosConsulta = TablaPermisos.Tables[1];
                                                                                                                DataTable TablaOpedido = TablaPermisos.Tables[2];
                                                                                                                DataTable TablaKadex = TablaPermisos.Tables[3];
                                                                                                                DataTable TablaTarifaEspecial = TablaPermisos.Tables[4];
                                                                                                                int recorrer = 0;
                                                                                                                //creamos dos columnas nuevas para añadir los datos del precio y descuento
                                                                                                                productosConsulta.Columns.Add("precio", typeof(decimal));
                                                                                                                productosConsulta.Columns.Add("descuento", typeof(decimal));
                                                                                                                productosConsulta.Columns.Add("PrecioTotalConIva", typeof(decimal));
                                                                                                                productosConsulta.Columns.Add("TotalIten", typeof(int));
                                                                                                                productosConsulta.Columns.Add("Obsequios", typeof(int));
                                                                                                                productosConsulta.Columns.Add("Cantidad", typeof(int));
                                                                                                                productosConsulta.Columns.Add("TotalIva", typeof(decimal));
                                                                                                                productosConsulta.Columns.Add("SubTotal", typeof(decimal));
                                                                                                                productosConsulta.Columns.Add("ValorUnitario", typeof(decimal));

                                                                                                                foreach (ProductosPed productos in consulta.Productos)
                                                                                                                {
                                                                                                                    if (productosConsulta.Rows.Count == 0)
                                                                                                                    {
                                                                                                                        Codigo = "078";
                                                                                                                        Mensaje = "El producto ingresado " + productos.pmIdProducto + " No Existe";
                                                                                                                        verificacionCompleta = false;
                                                                                                                        break;
                                                                                                                    }
                                                                                                                    //comprobamos que el producto exista
                                                                                                                    if (productosConsulta.Rows[recorrer].Field<string>("IdProducto") == productos.pmIdProducto)
                                                                                                                    {
                                                                                                                        int ListaProd = 1;
                                                                                                                        //verificamos que el producto este disponible
                                                                                                                        if (productosConsulta.Rows[recorrer].Field<string>("disponibleCia") == "disponible")
                                                                                                                        {

                                                                                                                            //comprobamos el valor a escoger de la lista
                                                                                                                            if (productos.pmIdListaDePrecio != 0)
                                                                                                                            {
                                                                                                                                if (permisoLTA)
                                                                                                                                {
                                                                                                                                    if (productos.pmIdListaDePrecio <= 5 & productos.pmIdListaDePrecio > 0)
                                                                                                                                    {
                                                                                                                                        //realizamos el cambio del precio a la primera fila
                                                                                                                                        string nomColumPrecio = "pmVrPrecio" + productos.pmIdListaDePrecio;
                                                                                                                                        string nomColumDescuentp = "CdDct" + productos.pmIdListaDePrecio;
                                                                                                                                        productosConsulta.Rows[recorrer]["precio"] = productosConsulta.Rows[recorrer][nomColumPrecio];
                                                                                                                                        productosConsulta.Rows[recorrer]["descuento"] = productosConsulta.Rows[recorrer][nomColumDescuentp];
                                                                                                                                        ListaProd = productos.pmIdListaDePrecio;

                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        Codigo = "039";
                                                                                                                                        Mensaje = "La lista ingresada es incorrecta debe estar en un valor entre 1 y 5";
                                                                                                                                        verificacionCompleta = false;
                                                                                                                                        break;
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                else if (productos.pmIdListaDePrecio == productosConsulta.Rows[recorrer].Field<int>("ListaDePrecio"))
                                                                                                                                {
                                                                                                                                    string nomColumPrecio = "pmVrPrecio" + productos.pmIdListaDePrecio;
                                                                                                                                    string nomColumDescuentp = "CdDct" + productos.pmIdListaDePrecio;
                                                                                                                                    productosConsulta.Rows[recorrer]["precio"] = productosConsulta.Rows[recorrer][nomColumPrecio];
                                                                                                                                    productosConsulta.Rows[recorrer]["descuento"] = productosConsulta.Rows[recorrer][nomColumDescuentp];
                                                                                                                                    ListaProd = productos.pmIdListaDePrecio;
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    Codigo = "041";
                                                                                                                                    Mensaje = "No tiene permisos para ingresar una lista diferente a la establecida";
                                                                                                                                    verificacionCompleta = false;
                                                                                                                                    break;
                                                                                                                                }
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                string nomColumPrecio = "pmVrPrecio" + productosConsulta.Rows[recorrer].Field<int>("ListaDePrecio");
                                                                                                                                string nomColumDescuentp = "CdDct" + productosConsulta.Rows[recorrer].Field<int>("ListaDePrecio");
                                                                                                                                productosConsulta.Rows[recorrer]["precio"] = productosConsulta.Rows[recorrer][nomColumPrecio];
                                                                                                                                productosConsulta.Rows[recorrer]["descuento"] = productosConsulta.Rows[recorrer][nomColumDescuentp];
                                                                                                                                ListaProd = productosConsulta.Rows[recorrer].Field<int>("ListaDePrecio");
                                                                                                                            }

                                                                                                                            //validamos la bodega del producto
                                                                                                                            if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmIdBodega))
                                                                                                                            {
                                                                                                                                //comprobamos si el valor es distintop para el correspondiente cambio
                                                                                                                                if (consulta.DatosDelPedido.pmIdBodega != productosConsulta.Rows[recorrer].Field<string>("Bodega"))
                                                                                                                                {
                                                                                                                                    if (permisoBOD)
                                                                                                                                    {
                                                                                                                                        if (Permisos.Field<string>("BodegaPermitida") != "dene")
                                                                                                                                        {
                                                                                                                                            productosConsulta.Rows[recorrer]["Bodega"] = Permisos.Field<string>("BodegaPermitida");
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            Codigo = "043";
                                                                                                                                            Mensaje = "La bodega ingresada no esta permitida en la compañía";
                                                                                                                                            verificacionCompleta = false;
                                                                                                                                            break;
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        Codigo = "042";
                                                                                                                                        Mensaje = "El usuario no tiene permiso BOD para cambiar la bodega";
                                                                                                                                        verificacionCompleta = false;
                                                                                                                                        break;
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            }

                                                                                                                            if (!((productos.pmCantObsequio > 0) | (productos.pmCantidad > 0)))
                                                                                                                            {
                                                                                                                                Codigo = "080";
                                                                                                                                Mensaje = "El valor de pmCantObsequio o pmCantidad debe ser mayor que cero ";
                                                                                                                                verificacionCompleta = false;
                                                                                                                                break;
                                                                                                                            }
                                                                                                                            if (!((productos.pmCantObsequio == 0) | permisoOBQ))
                                                                                                                            {
                                                                                                                                Codigo = "077";
                                                                                                                                Mensaje = "El usuario no tiene permiso OBQ para agregar productos de obsequio.";
                                                                                                                                verificacionCompleta = false;
                                                                                                                                break;
                                                                                                                            }
                                                                                                                            //se realiza la modificacion del precio para saber si obsequio 
                                                                                                                            if ((productos.pmCantObsequio > 0) & (productos.pmCantidad == 0))
                                                                                                                            {
                                                                                                                                //si solo esta la cantida de obsequio se cambia el valor para registarar el nuevo valor de solo el iva
                                                                                                                                productosConsulta.Rows[recorrer]["precio"] = (productosConsulta.Rows[recorrer].Field<decimal>("precio") * productosConsulta.Rows[recorrer].Field<decimal>("IVA")) / 100;
                                                                                                                            }

                                                                                                                            //varificamos que el valor del producto sea igual a cero y que tenga el permiso PEC
                                                                                                                            if (productos.pmVrPrecio == 0)
                                                                                                                            {
                                                                                                                                if (Permisos.Field<string>("PermisoPEC") == "PEC")
                                                                                                                                {
                                                                                                                                    productosConsulta.Rows[recorrer]["precio"] = productos.pmVrPrecio;
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    //se verifica que el producto 
                                                                                                                                    Codigo = "048";
                                                                                                                                    Mensaje = "El usuario no tiene permiso PEC para registrar el producto en valor cero";
                                                                                                                                    verificacionCompleta = false;
                                                                                                                                    break;
                                                                                                                                }
                                                                                                                            }
                                                                                                                            else if (productosConsulta.Rows[recorrer].Field<decimal>("precio") == productos.pmVrPrecio)
                                                                                                                            {
                                                                                                                                //validacion que el precio sea igual a de la lista
                                                                                                                                productosConsulta.Rows[recorrer]["precio"] = productos.pmVrPrecio;
                                                                                                                            }
                                                                                                                            else if (productosConsulta.Rows[recorrer].Field<decimal>("precio") != productos.pmVrPrecio)
                                                                                                                            {
                                                                                                                                //asiganamos una tabla para agregar la tabla de tarifas expeciales y saber en que parte esta el desceunto 

                                                                                                                                //verificamos qu en la tabla tenga resultados para realizar el proceso 
                                                                                                                                if (TablaTarifaEspecial.Rows.Count > 0)
                                                                                                                                {
                                                                                                                                    DataTable PrecioEspecial = null;

                                                                                                                                    var filasCoincidentes = TablaTarifaEspecial.AsEnumerable()
                                                                                                                                          .Where(row => row.Field<decimal>("tarifa") == productos.pmVrPrecio && row.Field<string>("SimbTfa") == "$");

                                                                                                                                    //se verifica si encuentra resultados
                                                                                                                                    if (filasCoincidentes.Any())
                                                                                                                                    {
                                                                                                                                        PrecioEspecial = filasCoincidentes.CopyToDataTable();
                                                                                                                                        //se realiza la comprobacion si cumple con los criterios del producto
                                                                                                                                        bool ValorExiste = PrecioEspecial.AsEnumerable()
                                                                                                                                        .Any(row => row.Field<string>("CdProducto") == productos.pmIdProducto ||
                                                                                                                                                    row.Field<string>("CdMarca") == productosConsulta.Rows[recorrer].Field<string>("Marca") ||
                                                                                                                                                    row.Field<string>("CdSubgrupo") == productosConsulta.Rows[recorrer].Field<string>("subgrupo") ||
                                                                                                                                                    row.Field<string>("CdGrupo") == productosConsulta.Rows[recorrer].Field<string>("Grupo") ||
                                                                                                                                                    row.Field<string>("cdlinea") == productosConsulta.Rows[recorrer].Field<string>("Linea"));

                                                                                                                                        if (ValorExiste)
                                                                                                                                        {
                                                                                                                                            productosConsulta.Rows[recorrer]["precio"] = productos.pmVrPrecio;
                                                                                                                                        }
                                                                                                                                    }


                                                                                                                                }
                                                                                                                            }
                                                                                                                            // se realiza la vrificacion si l precio ya coincide o sino se procede a verificar los permisos 
                                                                                                                            if (productosConsulta.Rows[recorrer].Field<decimal>("precio") != productos.pmVrPrecio)
                                                                                                                            {
                                                                                                                                string permisoMP = "PermisoMP" + ListaProd;
                                                                                                                                string MP = "MP" + ListaProd;
                                                                                                                                if (Permisos.Field<string>(permisoMP) == MP)
                                                                                                                                {
                                                                                                                                    productosConsulta.Rows[recorrer]["precio"] = productos.pmVrPrecio;
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    Codigo = "049";
                                                                                                                                    Mensaje = "El usuario no tiene permiso " + MP + " para cambiar el valor del producto";
                                                                                                                                    verificacionCompleta = false;
                                                                                                                                    break;
                                                                                                                                }
                                                                                                                            }

                                                                                                                            //validamos la tarifa de descuento 
                                                                                                                            //varificamos que el usuario ingreso una tarifa
                                                                                                                            if (!string.IsNullOrEmpty(productos.pmIdTarDcto))
                                                                                                                            {
                                                                                                                                //verificamos la tarifa ingresada sea validad
                                                                                                                                if (productosConsulta.Rows[recorrer].Field<string>("ExisteTarifa") == "dene")
                                                                                                                                {
                                                                                                                                    Codigo = "050";
                                                                                                                                    Mensaje = "La tarifa de descuento " + productos.pmIdTarDcto + " no Existe";
                                                                                                                                    verificacionCompleta = false;
                                                                                                                                    break;
                                                                                                                                }
                                                                                                                                else if (productosConsulta.Rows[recorrer].Field<decimal?>("descuento") == productosConsulta.Rows[recorrer].Field<decimal?>("Tarifa"))
                                                                                                                                {
                                                                                                                                    //si los valores coinciden se dejan igual 
                                                                                                                                }
                                                                                                                                else if (permisoDCT)
                                                                                                                                {
                                                                                                                                    //se cambia el valor por tener el permiso de cambiar la< tarifa
                                                                                                                                    productosConsulta.Rows[recorrer]["descuento"] = productosConsulta.Rows[recorrer]["Tarifa"];

                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    // se realiza la comprobacion con la tarifa en especial si no cumple arrojar error


                                                                                                                                    //asiganamos una tabla para agregar la tabla de tarifas expeciales y saber en que parte esta el desceunto 
                                                                                                                                    DataTable TablaTarifaDescuento = TablaPermisos.Tables[2];
                                                                                                                                    //verificamos qu en la tabla tenga resultados para realizar el proceso 
                                                                                                                                    if (TablaPermisos.Tables[2].Rows.Count > 0)
                                                                                                                                    {
                                                                                                                                        DataTable TablaTarDescuento = null;
                                                                                                                                        //Se realiza la consulta del valor
                                                                                                                                        var filasCoincidentes = TablaTarifaDescuento.AsEnumerable()
                                                                                                                                            .Where(row => row.Field<decimal>("tarifa") == productos.pmVrPrecio && row.Field<string>("SimbTfa") == "%");

                                                                                                                                        //se verifica si encuentra resultados
                                                                                                                                        if (filasCoincidentes.Any())
                                                                                                                                        {
                                                                                                                                            TablaTarDescuento = filasCoincidentes.CopyToDataTable();
                                                                                                                                            //se realiza la comprobacion si cumple con los criterios del producto
                                                                                                                                            bool ValorExiste = TablaTarDescuento.AsEnumerable()
                                                                                                                                            .Any(row => row.Field<string>("CdProducto") == productos.pmIdProducto ||
                                                                                                                                                        row.Field<string>("CdMarca") == productosConsulta.Rows[recorrer].Field<string>("Marca") ||
                                                                                                                                                        row.Field<string>("CdSubgrupo") == productosConsulta.Rows[recorrer].Field<string>("subgrupo") ||
                                                                                                                                                        row.Field<string>("CdGrupo") == productosConsulta.Rows[recorrer].Field<string>("Grupo") ||
                                                                                                                                                        row.Field<string>("cdlinea") == productosConsulta.Rows[recorrer].Field<string>("Linea"));

                                                                                                                                            if (ValorExiste)
                                                                                                                                            {
                                                                                                                                                productosConsulta.Rows[recorrer]["descuento"] = productosConsulta.Rows[recorrer]["Tarifa"];
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                Codigo = "051";
                                                                                                                                                Mensaje = "El usuario no tiene permiso DCT para cambiar la tarifa de descuento";
                                                                                                                                                verificacionCompleta = false;
                                                                                                                                                break;
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            Codigo = "051";
                                                                                                                                            Mensaje = "El usuario no tiene permiso DCT para cambiar la tarifa de descuento";
                                                                                                                                            verificacionCompleta = false;
                                                                                                                                            break;
                                                                                                                                        }


                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        Codigo = "051";
                                                                                                                                        Mensaje = "El usuario no tiene permiso DCT para cambiar la tarifa de descuento";
                                                                                                                                        verificacionCompleta = false;
                                                                                                                                        break;
                                                                                                                                    }
                                                                                                                                }


                                                                                                                            }

                                                                                                                            //se realiza la suma total para saber el valor total del producto
                                                                                                                            //obtenemos a valor con el descuento


                                                                                                                            decimal valor = productosConsulta.Rows[recorrer].Field<decimal>("precio") + ((productosConsulta.Rows[recorrer].Field<decimal>("precio") * (productosConsulta.Rows[recorrer].Field<decimal?>("descuento") ?? 0)) / 100);

                                                                                                                            productosConsulta.Rows[recorrer]["SubTotal"] = valor;

                                                                                                                            //obtenemos el valor con el iva
                                                                                                                            valor = ((valor * (productosConsulta.Rows[recorrer].Field<decimal?>("IVA") ?? 0)) / 100) + valor;

                                                                                                                            //obtenemos el valor para obsequios
                                                                                                                            decimal CanObsequio = (productosConsulta.Rows[recorrer].Field<decimal>("precio") * (productosConsulta.Rows[recorrer].Field<decimal?>("IVA") ?? 0)) / 100;
                                                                                                                            CanObsequio = CanObsequio * productos.pmCantObsequio;



                                                                                                                            productosConsulta.Rows[recorrer]["PrecioTotalConIva"] = valor * productos.pmCantidad;
                                                                                                                            productosConsulta.Rows[recorrer]["ValorUnitario"] = valor;

                                                                                                                            valor = valor * productos.pmCantObsequio;
                                                                                                                            productosConsulta.Rows[recorrer]["PrecioTotalConIva"] = productosConsulta.Rows[recorrer].Field<decimal>("PrecioTotalConIva") + CanObsequio;
                                                                                                                            productosConsulta.Rows[recorrer]["TotalIten"] = productos.pmCantidad + productos.pmCantObsequio;
                                                                                                                            productosConsulta.Rows[recorrer]["Obsequios"] = productos.pmCantObsequio;
                                                                                                                            productosConsulta.Rows[recorrer]["Cantidad"] = productos.pmCantidad;
                                                                                                                            productosConsulta.Rows[recorrer]["TotalIva"] = (productosConsulta.Rows[recorrer].Field<decimal>("precio") * (productosConsulta.Rows[recorrer].Field<decimal?>("IVA") ?? 0)) / 100;

                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            Codigo = "047";
                                                                                                                            Mensaje = "El producto ingresado con código " + productos.pmIdProducto + " y descripción " + productosConsulta.Rows[recorrer].Field<string>("DescripProd") + " No esta disponible en la compañia";
                                                                                                                            verificacionCompleta = false;
                                                                                                                            break;
                                                                                                                        }

                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        Codigo = "040";
                                                                                                                        Mensaje = "El producto ingresado " + productos.pmIdProducto + " No Existe";
                                                                                                                        verificacionCompleta = false;
                                                                                                                        break;
                                                                                                                    }
                                                                                                                    recorrer++;
                                                                                                                }

                                                                                                                if (verificacionCompleta)
                                                                                                                {
                                                                                                                    //verificamos el cupo si cumple 
                                                                                                                    decimal sumaPrecio = productosConsulta.AsEnumerable()
                                                                                                                             .Sum(row => row.Field<decimal>("PrecioTotalConIva"));

                                                                                                                    int sumaIten = productosConsulta.AsEnumerable()
                                                                                                                        .Sum(row => row.Field<int>("TotalIten"));

                                                                                                                    decimal TotalIva = productosConsulta.AsEnumerable()
                                                                                                                             .Sum(row => row.Field<decimal>("TotalIva"));

                                                                                                                    decimal SubTotal = productosConsulta.AsEnumerable()
                                                                                                                             .Sum(row => row.Field<decimal>("SubTotal"));

                                                                                                                    int cantidad = productosConsulta.AsEnumerable()
                                                                                                                      .Sum(row => row.Field<int>("Cantidad"));

                                                                                                                    int Obsequio = productosConsulta.AsEnumerable()
                                                                                                                      .Sum(row => row.Field<int>("Obsequios"));

                                                                                                                    decimal precio = productosConsulta.AsEnumerable()
                                                                                                                            .Sum(row => row.Field<decimal>("precio"));

                                                                                                                    if (sumaPrecio <= decimal.Parse(Permisos.Field<string>("saldo")) | (Permisos.Field<string>("PermisoCupo") == "CUP"))
                                                                                                                    {

                                                                                                                        #region Tabla Kardex
                                                                                                                        int iten = 1;
                                                                                                                        decimal VrImpBU = 0;
                                                                                                                        decimal VrImpCup = 0;

                                                                                                                        foreach (DataRow dr in productosConsulta.Rows)
                                                                                                                        {
                                                                                                                            string IdProducto = dr.Field<string>("IdProducto");

                                                                                                                            string IdBodega = dr.Field<string>("Bodega");
                                                                                                                            string CdTanque = dr.Field<string>("Bodega"); ;

                                                                                                                            float Salidas = dr.Field<int>("TotalIten");
                                                                                                                            string IdUnd = dr.Field<string>("presentacion");
                                                                                                                            decimal VrUnitario = dr.Field<decimal?>("ValorUnitario") ?? 0m;
                                                                                                                            decimal VrPrecio = dr.Field<decimal?>("SubTotal") ?? 0m;
                                                                                                                            decimal VrCostProm = 0;
                                                                                                                            decimal TarifaIva = dr.Field<decimal?>("IVA") ?? 0m;

                                                                                                                            decimal VrIvaSal = dr.Field<decimal?>("TotalIva") ?? 0m;
                                                                                                                            decimal TarifaDct = dr.Field<decimal?>("descuento") ?? 0m;

                                                                                                                            decimal VrDctoSal = dr.Field<decimal?>("SubTotal") ?? 0m;

                                                                                                                            decimal VrCostoSal = dr.Field<int>("Cantidad");

                                                                                                                            decimal VrBruto = dr.Field<decimal>("PrecioTotalConIva");

                                                                                                                            string CdLocal = Permisos.Field<string>("IDLocal");
                                                                                                                            string CdSzona = Permisos.Field<string>("SZona");

                                                                                                                            decimal Comision = ValorTarifaVendedor;

                                                                                                                            string Referencia = "";
                                                                                                                            if (dr.Field<int>("Obsequios") > 0 & dr.Field<int>("Cantidad") == 0)
                                                                                                                            {
                                                                                                                                Referencia = "Producto Obsequio";
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                Referencia = "Producto";
                                                                                                                            }
                                                                                                                            string Descripcion = dr.Field<string>("DescripProd");

                                                                                                                            bool EsCombo = false;
                                                                                                                            if (dr.Field<int>("Obsequios") > 0 & dr.Field<int>("Cantidad") > 0)
                                                                                                                            {
                                                                                                                                EsCombo = true;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {

                                                                                                                                EsCombo = false;
                                                                                                                            }

                                                                                                                            string CdMoneda = Permisos.Field<string>("CodMon");

                                                                                                                            string CodTarDct = dr.Field<string>("TarifaProducto") ?? "";
                                                                                                                            string CodTarIva = dr.Field<string>("CodigoIVA");
                                                                                                                            int ListaPrecc = dr.Field<int>("ListaDePrecio");
                                                                                                                            decimal VrBase = dr.Field<decimal>("TotalIva");


                                                                                                                            int CantObseq = dr.Field<int>("Obsequios");
                                                                                                                            decimal VrIvaObseq = dr.Field<decimal>("TotalIva");

                                                                                                                            DataRow Kadex = TablaKadex.NewRow();
                                                                                                                            Kadex["TipDoc"] = "PED";
                                                                                                                            Kadex["Documento"] = 0;
                                                                                                                            Kadex["IdCia"] = Cia;
                                                                                                                            Kadex["Item"] = iten;
                                                                                                                            Kadex["Fecha"] = FechaPedido.ToString("yyyy-MM-dd  HH:mm:ss");
                                                                                                                            Kadex["IdProducto"] = IdProducto;
                                                                                                                            Kadex["IdBodega"] = IdBodega;
                                                                                                                            Kadex["CdTanque"] = CdTanque;
                                                                                                                            Kadex["Entradas"] = 0;
                                                                                                                            Kadex["Salidas"] = Salidas;
                                                                                                                            Kadex["IdUnd"] = IdUnd;
                                                                                                                            Kadex["VrUnitario"] = VrUnitario;
                                                                                                                            Kadex["VrPrecio"] = VrPrecio;
                                                                                                                            Kadex["VrCostProm"] = VrCostProm;
                                                                                                                            Kadex["TarifaIva"] = TarifaIva;
                                                                                                                            Kadex["VrIvaEnt"] = 0;
                                                                                                                            Kadex["VrIvaSal"] = VrIvaSal;
                                                                                                                            Kadex["TarifaDct"] = TarifaDct;
                                                                                                                            Kadex["VrDctoEnt"] = 0;
                                                                                                                            Kadex["VrDctoSal"] = VrDctoSal;
                                                                                                                            Kadex["VrCostoEnt"] = 0;
                                                                                                                            Kadex["VrCostoSal"] = VrCostoSal;
                                                                                                                            Kadex["TarifaRet"] = 0;
                                                                                                                            Kadex["VrReteEnt"] = 0;
                                                                                                                            Kadex["VrReteSal"] = 0;
                                                                                                                            Kadex["TarifaIca"] = 0;
                                                                                                                            Kadex["VrIcaEnt"] = 0;
                                                                                                                            Kadex["VrIcaSal"] = 0;
                                                                                                                            Kadex["VrBruto"] = VrBruto;
                                                                                                                            Kadex["CdUbic"] = 0;
                                                                                                                            Kadex["NumLote"] = 0;
                                                                                                                            // Kadex["FechLote"] = null; 
                                                                                                                            Kadex["IdConcepto"] = "PED";
                                                                                                                            Kadex["IdTercero"] = consulta.Cliente.Documento ?? "";
                                                                                                                            Kadex["CdAgencia"] = agencia;
                                                                                                                            Kadex["CdCCosto"] = "";
                                                                                                                            Kadex["CdSubCos"] = "";
                                                                                                                            Kadex["CdLocal"] = CdLocal;
                                                                                                                            Kadex["CdSzona"] = CdSzona;
                                                                                                                            Kadex["pVehiculo"] = 0;
                                                                                                                            Kadex["IdVend"] = IdVendedor;
                                                                                                                            Kadex["Comision"] = Comision;
                                                                                                                            Kadex["CdOperario"] = "";
                                                                                                                            Kadex["ComisnOper"] = 0;
                                                                                                                            Kadex["Referencia"] = Referencia;
                                                                                                                            Kadex["Descripcion"] = Descripcion;
                                                                                                                            Kadex["Comptmntos"] = "";
                                                                                                                            Kadex["CdProdEquiv"] = "";
                                                                                                                            Kadex["TipOrd"] = "0";
                                                                                                                            Kadex["NumOrden"] = 0;
                                                                                                                            Kadex["IdCiaOrd"] = "00";
                                                                                                                            Kadex["Cotizacion"] = 0;
                                                                                                                            Kadex["IdCiaCot"] = "00";
                                                                                                                            Kadex["Remision"] = 0;
                                                                                                                            Kadex["IdCiaRem"] = 01;
                                                                                                                            Kadex["Factura"] = "";
                                                                                                                            Kadex["TipDocDev"] = 0;
                                                                                                                            Kadex["NumDocDev"] = 0;
                                                                                                                            Kadex["CdMngra"] = "";
                                                                                                                            Kadex["NumInicial"] = 0;
                                                                                                                            Kadex["NumFinal"] = 0;
                                                                                                                            Kadex["Sobretasa"] = 0;
                                                                                                                            Kadex["TasaNac"] = 0;
                                                                                                                            Kadex["TasaDep"] = 0;
                                                                                                                            Kadex["TasaMun"] = 0;
                                                                                                                            Kadex["Soldicom"] = 0;
                                                                                                                            Kadex["ImpGlobal"] = 0;
                                                                                                                            Kadex["OtroImpto"] = 0;
                                                                                                                            Kadex["Unidades"] = 0;
                                                                                                                            Kadex["ItemCombo"] = 0;
                                                                                                                            Kadex["Servcios"] = 0;
                                                                                                                            Kadex["NoVentas"] = 0;
                                                                                                                            Kadex["EsCombo"] = EsCombo;
                                                                                                                            Kadex["EsProdBase"] = 0;
                                                                                                                            Kadex["CodTarDct"] = CodTarDct;
                                                                                                                            Kadex["CodTarIva"] = CodTarIva;
                                                                                                                            Kadex["CodTarIca"] = "";
                                                                                                                            Kadex["CodTarRet"] = "";
                                                                                                                            Kadex["CodTarCom"] = "";
                                                                                                                            Kadex["CodTarCmc"] = "";
                                                                                                                            Kadex["ListaPrec"] = ListaPrecc;
                                                                                                                            Kadex["VrBase"] = VrBase;
                                                                                                                            Kadex["CdMoneda"] = CdMoneda;
                                                                                                                            Kadex["VrTasaCamb"] = 0;
                                                                                                                            Kadex["VrDivisa1"] = 0;
                                                                                                                            Kadex["VrDivisa2"] = 0;
                                                                                                                            Kadex["VrDivisa3"] = 0;
                                                                                                                            Kadex["Referencia2"] = "";
                                                                                                                            // Kadex["FecOrden"] = null
                                                                                                                            Kadex["galsbruto"] = 0;
                                                                                                                            Kadex["galsneto"] = 0;
                                                                                                                            Kadex["Temperatura"] = 0;
                                                                                                                            Kadex["UmTemp"] = "";
                                                                                                                            Kadex["Densidad"] = 0;
                                                                                                                            Kadex["TimeSys"] = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
                                                                                                                            Kadex["IdUsuario"] = usuario;
                                                                                                                            Kadex["Rec_Costo"] = 0;
                                                                                                                            Kadex["MgenCont"] = 0;
                                                                                                                            Kadex["VrImvCosto"] = 0;
                                                                                                                            Kadex["TarifaIco"] = 0;
                                                                                                                            Kadex["VrImpCon"] = 0;
                                                                                                                            Kadex["CantObseq"] = CantObseq;
                                                                                                                            Kadex["VrIvaObseq"] = VrIvaObseq;
                                                                                                                            Kadex["BaseIvaCom"] = 0;
                                                                                                                            Kadex["ImpCarbono"] = 0;
                                                                                                                            Kadex["IngBaseCom"] = 0;
                                                                                                                            Kadex["TarifaStc"] = 0;
                                                                                                                            Kadex["SobtasaCons"] = 0;
                                                                                                                            //Kadex["CodTarIco"] = "";
                                                                                                                            Kadex["BaseIvp"] = 0;
                                                                                                                            Kadex["TarifaIvp"] = 0;
                                                                                                                            Kadex["IvaIngProd"] = 0;
                                                                                                                            if (dr.Field<string>("TieneImpSal") == "BA")
                                                                                                                            {

                                                                                                                                CultureInfo culture = new CultureInfo("es-ES");

                                                                                                                                VrImpBU += (Convert.ToDecimal(dr.Field<string>("VrImpBu"), culture) * Convert.ToInt32(Salidas));
                                                                                                                                Kadex["TarifaIba"] = Convert.ToDecimal(dr.Field<string>("tarifaba"), culture);
                                                                                                                                Kadex["VrImpuBa"] = Convert.ToDecimal(dr.Field<string>("VrImpBu"), culture);
                                                                                                                                Kadex["CodTarBa"] = " ";
                                                                                                                                Kadex["TarifaCup"] = 0;
                                                                                                                                Kadex["VrImpuCup"] = 0;
                                                                                                                                Kadex["CodTarCup"] = "  ";


                                                                                                                            }
                                                                                                                            else if (dr.Field<string>("TieneImpSal") == "CUP")
                                                                                                                            {
                                                                                                                                CultureInfo culture = new CultureInfo("es-ES");

                                                                                                                                Kadex["TarifaIba"] = 0;
                                                                                                                                Kadex["VrImpuBa"] = 0;
                                                                                                                                Kadex["CodTarBa"] = " ";
                                                                                                                                Kadex["TarifaCup"] = Convert.ToDecimal(dr.Field<string>("tarifaba"), culture);
                                                                                                                                Kadex["VrImpuCup"] = (VrPrecio * Convert.ToDecimal(dr.Field<string>("tarifaba"), culture)) / 100;
                                                                                                                                Kadex["CodTarCup"] = dr.Field<string>("CadTarba");
                                                                                                                                VrImpCup += ((VrPrecio * Convert.ToDecimal(dr.Field<string>("tarifaba"), culture)) / 100) * Convert.ToInt32(Salidas);
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                Kadex["TarifaIba"] = 0;
                                                                                                                                Kadex["VrImpuBa"] = 0;
                                                                                                                                Kadex["CodTarBa"] = " ";
                                                                                                                                Kadex["TarifaCup"] = 0;
                                                                                                                                Kadex["VrImpuCup"] = 0;
                                                                                                                                Kadex["CodTarCup"] = " ";
                                                                                                                            }
                                                                                                                            TablaKadex.Rows.Add(Kadex);

                                                                                                                            iten++;
                                                                                                                        }
                                                                                                                        #endregion

                                                                                                                        #region Completar la tabla de Trn_Opedido 
                                                                                                                        DataRow Opedido = TablaOpedido.NewRow();
                                                                                                                        Opedido["TipDoc"] = "PED";
                                                                                                                        Opedido["Pedido"] = 0;
                                                                                                                        Opedido["IdCia"] = Cia;
                                                                                                                        Opedido["Fecha"] = FechaPedido.ToString("yyyy-MM-dd  HH:mm:ss");
                                                                                                                        Opedido["FechaVence"] = FechaPedido.AddDays(DiasEntrega).ToString("yyyy-MM-dd HH:mm:ss");
                                                                                                                        Opedido["IdConcepto"] = "PED";
                                                                                                                        Opedido["IdCliente"] = consulta.Cliente.Documento ?? "";
                                                                                                                        Opedido["IdAgencia"] = agencia;
                                                                                                                        Opedido["IdClieFact"] = consulta.Cliente.Documento ?? "";
                                                                                                                        Opedido["VrSubTotal"] = SubTotal;
                                                                                                                        Opedido["VrDescuento"] = SubTotal - precio;
                                                                                                                        Opedido["VrImpuesto"] = TotalIva;
                                                                                                                        Opedido["VrFletes"] = 0;
                                                                                                                        Opedido["VrOtros"] = 0;
                                                                                                                        Opedido["VrCargos"] = 0;
                                                                                                                        Opedido["VrOtrDcto"] = 0;
                                                                                                                        Opedido["VrSobretasa"] = 0;
                                                                                                                        Opedido["VrImpGlobal"] = 0;
                                                                                                                        Opedido["VrNeto"] = sumaPrecio;
                                                                                                                        Opedido["Cantidad"] = cantidad;
                                                                                                                        Opedido["IdVend"] = IdVendedor;
                                                                                                                        Opedido["TarifaCom"] = ValorTarifaVendedor;
                                                                                                                        Opedido["CodTarCom"] = TarifaVendedor;
                                                                                                                        Opedido["DirEnvio"] = consulta.Cliente.Direccion ?? "";
                                                                                                                        Opedido["IdLocEnv"] = consulta.Cliente.Municipio ?? "";
                                                                                                                        Opedido["LugarEnvio"] = Permisos.Field<string>("Localidad");
                                                                                                                        Opedido["DiasEntraga"] = DiasEntrega;
                                                                                                                        Opedido["NitContac"] = NitContacto;
                                                                                                                        Opedido["NomContac"] = NombreContacto;
                                                                                                                        Opedido["TelContac"] = TelefonoContacto;
                                                                                                                        Opedido["emlContac"] = EmailContacto;
                                                                                                                        Opedido["CargoContac"] = CargoContacto;
                                                                                                                        Opedido["IdForma"] = FormaDePago;
                                                                                                                        Opedido["DetallePago"] = "";
                                                                                                                        Opedido["MulPlazos"] = false;
                                                                                                                        Opedido["IdPlazo"] = Plazo;
                                                                                                                        Opedido["CdMney"] = Permisos.Field<string>("CodMon");
                                                                                                                        Opedido["NitEmpTrans"] = "0";
                                                                                                                        Opedido["EmpTrans"] = "";
                                                                                                                        Opedido["AsignarVeh"] = false;
                                                                                                                        Opedido["pVehiculo"] = "0";
                                                                                                                        Opedido["CdConductor"] = "0";
                                                                                                                        Opedido["CdRuta"] = Ruta;
                                                                                                                        Opedido["ListaPrec"] = 0;
                                                                                                                        Opedido["RefPedido"] = "CONTADO";
                                                                                                                        Opedido["Modalidad"] = "INVENTARIO";
                                                                                                                        Opedido["Vigencia"] = "NORMAL";
                                                                                                                        Opedido["NumAutoriza"] = 0;
                                                                                                                        Opedido["NumAutCupo"] = 0;
                                                                                                                        Opedido["NumAutCheq"] = 0;
                                                                                                                        Opedido["NumAprob"] = 0;
                                                                                                                        Opedido["IdCiaApr"] = Cia;
                                                                                                                        //Opedido["FecAprob"] = "";
                                                                                                                        Opedido["DetalleAprob"] = "APROBADO WS";
                                                                                                                        Opedido["CdUsuAprob"] = usuario;
                                                                                                                        Opedido["TipFac"] = "0";
                                                                                                                        Opedido["Factura"] = 0;
                                                                                                                        Opedido["IdCiaFac"] = "00";
                                                                                                                        Opedido["FechaFact"] = DBNull.Value;
                                                                                                                        Opedido["TipRem"] = "0";
                                                                                                                        Opedido["Remision"] = 0;
                                                                                                                        Opedido["IdCiaRem"] = "00";
                                                                                                                        Opedido["FechaRem"] = DBNull.Value;
                                                                                                                        Opedido["NumCotizac"] = 0;
                                                                                                                        Opedido["CdCiaCotizac"] = compania;
                                                                                                                        Opedido["OrigenAdd"] = "WS";
                                                                                                                        Opedido["ZonaFrontera"] = 0;
                                                                                                                        Opedido["TipoTrans"] = 0;
                                                                                                                        Opedido["TipoOrden"] = "";
                                                                                                                        Opedido["TipoModifica"] = "";
                                                                                                                        Opedido["Anulado"] = false;
                                                                                                                        Opedido["FecDev"] = DBNull.Value;
                                                                                                                        Opedido["Observacion"] = "Generado interfaz Web Services";
                                                                                                                        Opedido["IdEstado"] = consulta.DatosDelPedido.pmEstadoPedido;
                                                                                                                        Opedido["TimeSys"] = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
                                                                                                                        Opedido["FecUpdate"] = DBNull.Value;
                                                                                                                        Opedido["IdCiaCrea"] = compania;
                                                                                                                        Opedido["IdUsuario"] = usuario;
                                                                                                                        Opedido["NumAutSicom"] = "";
                                                                                                                        Opedido["VrImpCarbono"] = 0;
                                                                                                                        Opedido["BaseIvaIgp"] = 0;
                                                                                                                        Opedido["VrIvaIngProd"] = 0;
                                                                                                                        Opedido["VrImpuBA"] = VrImpBU;
                                                                                                                        Opedido["VrImpuCUP"] = VrImpCup;

                                                                                                                        TablaOpedido.Rows.Add(Opedido);
                                                                                                                        #endregion

                                                                                                                        //Proceso de insercion del pedido se utiliza un solo proceso para este pasopara evitar concurrencia
                                                                                                                        lock (Service1.lockObject)
                                                                                                                        {
                                                                                                                            //realizamos la insercion en la base de datos 
                                                                                                                            DataSet TablaInsert = new DataSet();
                                                                                                                            List<SqlParameter> parametro = new List<SqlParameter>();

                                                                                                                            parametro.Add(new SqlParameter("@OpedidoTable", SqlDbType.Structured)
                                                                                                                            {
                                                                                                                                TypeName = "dbo.wcfPedidos_Opedidos", // Nombre del tipo de tabla definido por el usuario
                                                                                                                                Value = TablaOpedido
                                                                                                                            });

                                                                                                                            parametro.Add(new SqlParameter("@KadexTable", SqlDbType.Structured)
                                                                                                                            {
                                                                                                                                TypeName = "dbo.wcfPedidos_Kadex", // Nombre del tipo de tabla definido por el usuario
                                                                                                                                Value = TablaKadex
                                                                                                                            });

                                                                                                                            if (ClassConexion.ejecutarQuery("WSPedido_ConsAgregarPedido", parametro, out TablaInsert, out string[] nuevoMennsajes, CommandType.StoredProcedure))
                                                                                                                            {
                                                                                                                                int Pedido = TablaInsert.Tables[0].Rows[0].Field<int>("Pedido");
                                                                                                                                respuesta.IdCia = Opedido["IdCia"].ToString();
                                                                                                                                respuesta.TipoDoc = Opedido["TipDoc"].ToString();
                                                                                                                                respuesta.CdAgencia = Opedido["IdAgencia"].ToString();
                                                                                                                                respuesta.Fecha = Opedido["Fecha"].ToString();
                                                                                                                                Codigo = "066";
                                                                                                                                Mensaje = "Se ha registrado el pedido exitosamente con número de factura " + Pedido;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                Codigo = nuevoMennsajes[0];
                                                                                                                                Mensaje = nuevoMennsajes[1];
                                                                                                                            }
                                                                                                                        }

                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        Codigo = "052";
                                                                                                                        Mensaje = "El usuario no tiene permiso para Superar el cupo del cliente";
                                                                                                                    }
                                                                                                                }


                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Codigo = "037";
                                                                                                                Mensaje = "No se han encontrado los productos";
                                                                                                            }




                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Codigo = "038";
                                                                                                        Mensaje = "El usuario no tiene permiso CIA para registrar el pedido en otra compañía";
                                                                                                    }

                                                                                                }

                                                                                            }

                                                                                        }

                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Codigo = "022";
                                                                                        Mensaje = "No tiene el usuario el permiso EGA para modificar Días y fecha de entrega";
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                Codigo = "045";
                                                                                Mensaje = "El código de pmTarifaComision no existe en syscom";
                                                                            }
                                                                        }


                                                                    }
                                                                    else
                                                                    {
                                                                        Codigo = "022";
                                                                        Mensaje = "El cliente se encuentra en mora y el usuario no tiene el permiso MOR";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Codigo = "031";
                                                                    Mensaje = "El Código de plazo no corresponde al cliente y el usuario no tiene permiso PZO.";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Codigo = "055";
                                                                Mensaje = "El código de plazo ingresado no existe";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Codigo = "055";
                                                            Mensaje = "El codigo de pmEstadoPedido no esta permitido solo estan 0001 y 0002";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Codigo = "074";
                                                        Mensaje = "El Usuario no tiene permiso para cambiar el estado del pedido";
                                                    }
                                                }
                                                else
                                                {
                                                    Codigo = "054";
                                                    Mensaje = "La Ruta ingresada no existe";
                                                }
                                            }
                                            else
                                            {
                                                Codigo = "053";
                                                Mensaje = "El código ingresado de la localidad no existe";
                                            }
                                        }
                                        else
                                        {
                                            Codigo = "021";
                                            Mensaje = "El cliente no puede recibir pedidos de la compañía asignada del usuario.";
                                        }

                                    }
                                    else
                                    {
                                        if (Permisos.Field<string>("Cliente") == "Noexiste")
                                        {
                                            Codigo = "019";
                                            Mensaje = "El cliente no existe";
                                        }
                                        else
                                        {
                                            Codigo = "020";
                                            Mensaje = "El cliente se encuentra bloqueado y el vendedor no tiene el permiso BLO";
                                        }
                                    }

                                }
                                else
                                {
                                    Codigo = "018";
                                    Mensaje = "El usuario no tiene permisos para hacer pedidos";
                                }
                            }
                            else
                            {
                                Codigo = "017";
                                Mensaje = "El Usuario no pertenece al grupo de usuarios avanzados";
                            }
                        }
                        else
                        {
                            Codigo = "016";
                            Mensaje = "El Usuario no está marcado como vendedor";
                        }


                    }
                    else
                    {
                        Codigo = "015";
                        Mensaje = "El Usuario no está registrado  como vendedor";
                    }

                }
                else
                {
                    Codigo = nuevoMennsaje[0];
                    Mensaje = nuevoMennsaje[1];
                }
                respuesta.Registro = new Log { Codigo = Codigo, Descripcion = Mensaje };
            }
            catch (Exception ex)
            {
                respuesta.Registro = new Log { Codigo = "076", Descripcion = "Error al generar el pedido" };
            }

            return respuesta;
        }
    }
}