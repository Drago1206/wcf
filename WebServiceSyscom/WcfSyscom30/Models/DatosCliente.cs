using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WcfSyscom30.Conexion;

namespace WcfSyscom30.Models
{
    public class DatosCliente
    {
        public string NitCliente { get; set; }
        public string NombreCliente { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Telefono { get; set; }
        public int NumLista { get; set; }
        public string NitVendedor { get; set; }
        public string NomVendedor { get; set; }
        public List<ItemAgencia> ListaAgencias { get; set; }

        private ConexionDB cnn;
        private ConexionSqlLite ConexSqlLite = new ConexionSqlLite("");
        private PaginadorCliente<DatosCliente> PaginadorCCliente;
        private ListadoClientesPag ListadoClientesPag;
        private List<DatosCliente> clientesDB;
        public Errores ConsultarCliente(string NitCliente, out List<DatosCliente> dcliente)
        {
            Errores _err = null;
            dcliente = new List<DatosCliente>();

            try
            {
                cnn = new ConexionDB();
                cnn.setConnection(ConexSqlLite.obtenerConexionSyscom("dbpar"));
                #region consulta produccion
                string cons = @"SELECT N.NIT_NIT NitCliente,N.NIT_NOM NombreCliente,N.NIT_DIR Direccion,CIU_DES Ciudad,N.NIT_TEL Telefono,N.NIT_LTA NumLista,N.NIT_CVE NitVendedor,V.NIT_NOM NomVendedor,AGE_COD CodAge,AGE_NOM NomAge
                                FROM dbo.NIT N INNER JOIN dbo.CIUDADES C ON C.CIU_COD=N.NIT_CIU 
								LEFT JOIN dbo.NIT V ON N.NIT_CVE=V.NIT_NIT
								LEFT JOIN dbo.AGENCIAS a on N.NIT_NIT=AGE_NIT
                                WHERE N.NIT_NIT='" + NitCliente + "'";
                #endregion
                #region consulta pruebas coodepetrol
                //string cons = @"SELECT NIT_NIT NitCliente,NIT_NOM NombreCliente,NIT_DIR Direccion,CIU_DES Ciudad,NIT_TEL Telefono,NIT_LTA NumLista,AGE_COD CodAge,AGE_NOM NomAge
                //                FROM dbpar_coode..NIT n inner join dbpar_coode..CIUDADES c on c.CIU_COD=n.NIT_CIU left join dbpar_coode..AGENCIAS a on NIT_NIT=AGE_NIT
                //                WHERE NIT_NIT='" + NitCliente + "'";
                #endregion
                cnn.resetQuery();
                cnn.setCustomQuery(cons);
                cnn.ejecutarQuery();
                if (cnn.getDataTable() != null && cnn.getDataTable().Rows.Count > 0)
                {
                    DataTable dtDatos = cnn.getDataTable();
                    dcliente = cnn.DataTableToList<DatosCliente>("NitCliente,NombreCliente,Direccion,Ciudad,Telefono,NumLista,NitVendedor,NomVendedor".Split(','));
                    dcliente.ForEach(m =>
                    {
                        m.ListaAgencias = new List<ItemAgencia>();
                        m.ListaAgencias = cnn.DataTableToList<ItemAgencia>(dtDatos.Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("NitCliente").Equals(m.NitCliente)).CopyToDataTable().AsDataView().ToTable(true, "CodAge,NomAge".Split(',')));
                    });
                }
                else
                {
                    _err = new Errores { codigo = "CLIEN_002", descripcion = "¡Cliente no encontrado!" };
                }

            }
            catch (Exception ex)
            {
                _err = new Errores { descripcion = ex.Message };
            }

            return _err;
        }

        public Errores ConsultarConsCliente(Clientes Clientes, out PaginadorCliente<DatosCliente> dcliente)
        {
            Errores _err = null;
            dcliente = null;
            clientesDB = new List<DatosCliente>();
            PaginadorCCliente = new PaginadorCliente<DatosCliente>();

            try
            {
                cnn = new ConexionDB();
                cnn.setConnection(ConexSqlLite.obtenerConexionSyscom("dbpar"));
                #region consulta produccion
                string cons = @"SELECT N.NIT_NIT NitCliente,N.NIT_NOM NombreCliente,N.NIT_DIR Direccion,CIU_DES Ciudad,N.NIT_TEL Telefono,N.NIT_LTA NumLista,N.NIT_CVE NitVendedor,V.NIT_NOM NomVendedor,AGE_COD CodAge,AGE_NOM NomAge
                                FROM dbo.NIT N INNER JOIN dbo.CIUDADES C ON C.CIU_COD=N.NIT_CIU 
								LEFT JOIN dbo.NIT V ON N.NIT_CVE=V.NIT_NIT
								LEFT JOIN dbo.AGENCIAS a on N.NIT_NIT=AGE_NIT
                                WHERE N.NIT_NIT LIKE '" + Clientes.NitCliente + "%'";
                #endregion
                #region consulta pruebas coodepetrol
                //string cons = @"SELECT NIT_NIT NitCliente,NIT_NOM NombreCliente,NIT_DIR Direccion,CIU_DES Ciudad,NIT_TEL Telefono,NIT_LTA NumLista,AGE_COD CodAge,AGE_NOM NomAge
                //                FROM dbpar_coode..NIT n inner join dbpar_coode..CIUDADES c on c.CIU_COD=n.NIT_CIU left join dbpar_coode..AGENCIAS a on NIT_NIT=AGE_NIT
                //                WHERE NIT_NIT='" + NitCliente + "'";
                #endregion
                cnn.resetQuery();
                cnn.setCustomQuery(cons);
                cnn.ejecutarQuery();
                if (cnn.getDataTable() != null && cnn.getDataTable().Rows.Count > 0)
                {
                    DataTable dtDatos = cnn.getDataTable();
                    clientesDB = cnn.DataTableToList<DatosCliente>("NitCliente,NombreCliente,Direccion,Ciudad,Telefono,NumLista,NitVendedor,NomVendedor".Split(','));
                    clientesDB.ForEach(m =>
                    {
                        m.ListaAgencias = new List<ItemAgencia>();
                        m.ListaAgencias = cnn.DataTableToList<ItemAgencia>(dtDatos.Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("NitCliente").Equals(m.NitCliente)).CopyToDataTable().AsDataView().ToTable(true, "CodAge,NomAge".Split(',')));
                    });
                    int _TotalRegistros = 0;
                    int _TotalPaginas = 0;
                    int registros_por_pagina;
                    int pagina;

                    if (Clientes.RegistrosPorPagina == 0)
                        registros_por_pagina = 10;
                    else
                        registros_por_pagina = Clientes.RegistrosPorPagina;

                    if (Clientes.PaginaActual == 0)
                        pagina = 1;
                    else
                        pagina = Clientes.PaginaActual;

                    _TotalRegistros = clientesDB.Count();

                    clientesDB = clientesDB.Skip((pagina - 1) * registros_por_pagina)
                                                     .Take(registros_por_pagina)
                                                     .ToList();

                    _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / registros_por_pagina);

                    PaginadorCCliente = new PaginadorCliente<DatosCliente>()
                    {
                        PaginaActual = pagina,
                        RegistrosPorPagina = registros_por_pagina,
                        TotalRegistros = _TotalRegistros,
                        TotalPaginas = _TotalPaginas,
                        Resultado = clientesDB
                    };

                    dcliente = PaginadorCCliente;
                }
                else
                {
                    _err = new Errores { codigo = "CLIEN_002", descripcion = "¡Cliente no encontrado!" };
                }

            }
            catch (Exception ex)
            {
                _err = new Errores { descripcion = ex.Message };
            }

            return _err;
        }
    }
}