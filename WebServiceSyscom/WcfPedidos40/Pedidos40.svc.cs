using Newtonsoft.Json;
using SyscomUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using WcfPedidos40.Model;

namespace WcfPedidos40
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Pedidos40 : IPedidos40
    {

        private Models.Conexion con = new Models.Conexion();
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProducto", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Producto")]
        public RespProducto GetProducto(ProductoReq reqProducto)
        {
            //List<Errores> Errores = new List<Errores>();
            RespProducto rta = new RespProducto();
            rta.Errores = new List<Errores>();
            try
            {
                if (string.IsNullOrWhiteSpace(reqProducto.IdUsuario))
                    rta.Errores.Add(new Errores { coderror = "001", descripcion = "El parámetro 'Id Usuario' no puede ser nulo!" });
                if (string.IsNullOrWhiteSpace(reqProducto.Contrasena))
                    rta.Errores.Add(new Errores { coderror = "002", descripcion = "El parámetro 'Contraseña' no puede ser nulo!" });
                if (string.IsNullOrWhiteSpace(reqProducto.IdProducto))
                    rta.Errores.Add(new Errores { coderror = "003", descripcion = "El parámetro 'Id Producto' no puede ser nulo!" });
                if (rta.Errores.Count <= 0)
                {
                    DataTable us = existeUsuario(reqProducto.IdUsuario, reqProducto.Contrasena);
                    if (us == null || us.Rows.Count <= 0)
                        rta.Errores.Add(new Errores { coderror = "004", descripcion = "El usuario '" + reqProducto.IdUsuario + "', NO existe!" });
                    else
                    {
                        pwdSyscom pwd = new pwdSyscom();
                        pwd.Decodificar(us.Rows[0].Field<string>("PwdLog"));
                        if (pwd.contrasenna.Split('=')[2] != reqProducto.Contrasena)
                            rta.Errores.Add(new Errores { coderror = "005", descripcion = "Contraseña o Usuario incorrectos!" });
                        else
                        {
                            ProductosResponse p = new ProductosResponse();
                            rta = p.GetProducto(reqProducto);
                            if (rta == null || rta.Codigo == null)
                            {
                                rta = new RespProducto();
                                rta.Errores = new List<Errores>();
                                rta.Errores.Add(new Errores { coderror = "006", descripcion = "El producto '" + reqProducto.IdProducto + "', NO existe!" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rta.Errores.Add(new Errores { coderror = "100", descripcion = ex.Message });
            }
            return rta;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProductoTP", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Producto")]
        public RespProducto GetProductoTP(ProductoTPReq reqProdTP)
        {
            //List<Errores> Errores = new List<Errores>();
            RespProducto rta = new RespProducto();
            rta.Errores = new List<Errores>();
            try
            {
                if (string.IsNullOrWhiteSpace(reqProdTP.IdUsuario))
                    rta.Errores.Add(new Errores { coderror = "001", descripcion = "El parámetro 'Id Usuario' no puede ser nulo!" });
                if (string.IsNullOrWhiteSpace(reqProdTP.Contrasena))
                    rta.Errores.Add(new Errores { coderror = "002", descripcion = "El parámetro 'Contraseña' no puede ser nulo!" });
                if (string.IsNullOrWhiteSpace(reqProdTP.IdProducto))
                    rta.Errores.Add(new Errores { coderror = "003", descripcion = "El parámetro 'Id Producto' no puede ser nulo!" });
                if (rta.Errores.Count <= 0)
                {
                    DataTable us = existeUsuario(reqProdTP.IdUsuario, reqProdTP.Contrasena);
                    if (us == null || us.Rows.Count <= 0)
                        rta.Errores.Add(new Errores { coderror = "004", descripcion = "El usuario '" + reqProdTP.IdUsuario + "', NO existe!" });
                    else
                    {
                        pwdSyscom pwd = new pwdSyscom();
                        pwd.Decodificar(us.Rows[0].Field<string>("PwdLog"));
                        if (pwd.contrasenna.Split('=')[2] != reqProdTP.Contrasena)
                            rta.Errores.Add(new Errores { coderror = "005", descripcion = "Contraseña o Usuario incorrectos!" });
                        else
                        {
                            ProductosResponse p = new ProductosResponse();
                            rta = p.GetProductoTP(reqProdTP);
                            if (rta == null || rta.Codigo == null)
                            {
                                rta = new RespProducto();
                                rta.Errores = new List<Errores>();
                                rta.Errores.Add(new Errores { coderror = "006", descripcion = "El producto '" + reqProdTP.IdProducto + "', NO existe!" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rta.Errores.Add(new Errores { coderror = "100", descripcion = ex.Message });
            }
            return rta;
        }
        public RespProductos GetProductos(Usuario usuario)
        {
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            RespProductos productos = new RespProductos();
            List<ProductosResponse> prods = new List<ProductosResponse>();
            List<SqlParameter> parametros = new List<SqlParameter>();
            DataSet TablaProductos = new DataSet();
            if (existeUsuario(usuario))
            {
                con.setConnection("Syscom");
                DateTime pmFecha_Actual = DateTime.Parse(usuario.Fecha_Act);
                parametros.Add(new SqlParameter("@IdUsuario", usuario.IdUsuario));
                parametros.Add(new SqlParameter("@FechaActual", pmFecha_Actual));
                if (con.ejecutarQuery("WSPedidos40Productos", parametros, out TablaProductos, out string[] mensajeProductos, CommandType.StoredProcedure))
                {
                    if (TablaProductos.Tables[0].Rows.Count > 0)
                    {
                        TablaProductos.Tables[0].AsEnumerable().ToList().ForEach(i =>
                        {
                            ProductosResponse prod = new ProductosResponse
                            {
                                IdProducto = i.Field<string>("IdProducto"),
                                DescripProd = i.Field<string>("DescripProd"),
                                IvaInc = i.Field<string>("IvaInc"),
                                LtPreDef = i.Field<string>("LtPreDef"),
                                VrPrecio1 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("1") ? i.Field<decimal>("VrPrecio1") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio1"),
                                VrPrecio2 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("2") ? i.Field<decimal>("VrPrecio2") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio2"),
                                VrPrecio3 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("3") ? i.Field<decimal>("VrPrecio3") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio3"),
                                VrPrecio4 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("4") ? i.Field<decimal>("VrPrecio4") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio4"),
                                VrPrecio5 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("5") ? i.Field<decimal>("VrPrecio5") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio5"),
                                ExcluidoImp = i.Field<bool>("ExcluidoImp"),
                                TarifaIva = i.Field<decimal>("Tarifa")
                            };

                            //Jefersón dice que esto no se trae para ecom 16/10/2019
                            //PrecioEspecial TarEsp = new PrecioEspecial();
                            //TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 1, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                            //if (TarEsp.Numero != null)
                            //    prod.pmVrPrecio1 = TarEsp.Tarifa.Value;

                            //TarEsp = new PrecioEspecial();
                            //TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 2, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                            //if (TarEsp.Numero != null)
                            //{
                            //    LogErrores.tareas.Add("Ingreso dos");
                            //    LogErrores.tareas.Add(TarEsp.Numero.ToString());
                            //    LogErrores.write();
                            //    prod.pmVrPrecio2 = TarEsp.Tarifa.Value;
                            //}

                            //TarEsp = new PrecioEspecial();
                            //TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 3, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                            //if (TarEsp.Numero != null)
                            //    prod.pmVrPrecio3 = TarEsp.Tarifa.Value;

                            //TarEsp = new PrecioEspecial();
                            //TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 4, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                            //if (TarEsp.Numero != null)
                            //    prod.pmVrPrecio4 = TarEsp.Tarifa.Value;

                            //TarEsp = new PrecioEspecial();
                            //TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 5, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                            //if (TarEsp.Numero != null)
                            //    prod.pmVrPrecio5 = TarEsp.Tarifa.Value;

                            prods.Add(prod);
                        });

                        prods.ForEach(r =>
                        {
                            con.resetQuery();
                            con.qryFields.Add(@"IdCia");
                            con.qryTables.Add(@"ProdCompanias");
                            con.addWhereAND("IdProducto = '" + r.IdProducto + "'");
                            con.select();
                            con.ejecutarQuery();
                            r.DisponibleEnCia = con.getDataTable().AsEnumerable().Select(s => s.Field<string>("IdCia")).ToList();
                        });
                        productos.Productos = prods;
                        //productos.Productos = JValue.Parse(JsonConvert.SerializeObject(prods.Select(r => new { r.pmIdProducto, r.pmDescripProd, r.pmIvaInc, r.pmLtPreDef, r.pmVrPrecio1, r.pmVrPrecio2, r.pmVrPrecio3, r.pmVrPrecio4, r.pmVrPrecio5, r.pmExcluidoImp, r.pmTarifaIva, r.pmDisponibleEnConpania }), Formatting.Indented, serializerSettings)).ToString(Formatting.Indented);
                    }
                }
                productos.Registro = new Log { Codigo = "1", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Registros = prods.Count, Msg = "Se ejecutó correctamente la consulta." };

            }
            else
            {
                productos.Registro = new Log { Codigo = "USER_001", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Registros = prods.Count, Msg = "No se encontró el usuario o la contraseña es incorrecta." };
            }

            return productos;
        }
        private DataTable existeUsuario(string agusuario, string agcontrasena)
        {
            con.setConnection("Syscom");
            con.resetQuery();
            con.qryFields.Add("IdUsuario, Inactivo, PwdLog");
            con.qryTables.Add("adm_Usuarios");
            con.addWhereAND("lower(IdUsuario) = lower('" + agusuario + "')");
            con.select();
            con.ejecutarQuery();
            return con.getDataTable();
        }
        private bool existeUsuario(Usuario usuario)
        {
            bool existe = false;
            con.setConnection("Syscom");
            con.resetQuery();
            con.qryFields.Add("IdUsuario, PwdLog");
            con.qryTables.Add("adm_Usuarios");
            con.addWhereAND("Inactivo = 0 and lower(IdUsuario) = lower('" + usuario.IdUsuario + "')");
            con.select();
            con.ejecutarQuery();
            DataTable ds = con.getDataTable();
            DataRow row = ds.Rows[0];
            if (ds.Rows.Count > 0)
            {
                pwdSyscom pwdSys = new pwdSyscom();
                pwdSys.Decodificar(row["PwdLog"].ToString());
                var contra = pwdSys.contrasenna.Split('=');
                if (contra[2] == usuario.Contrasena)
                {
                    existe = true;

                }
                else
                {
                    existe = false;
                }
            }
            return existe;
        }
    }
}
