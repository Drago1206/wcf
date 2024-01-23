using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WcfSyscom30.Conexion;

namespace WcfSyscom30.Models
{
    public class ItemProducto
    {

        public string CodProducto { get; set; }
        public string Descripción { get; set; }
        public int Lista1 { get; set; }
        public int Lista2 { get; set; }
        public int Lista3 { get; set; }
        public int Impuesto { get; set; }
        public int Descuento { get; set; }
        public string CodigoGru { get; set; }
        public string NombreGru { get; set; }
        public string CodigoSub { get; set; }
        public string NombreSub { get; set; }
        public int SaldoTotal { get; set; }
        public List<ItemCia> itemCia { get; set; }
        public DateTime FechaCreacion { get; set; }


        private ConexionDB cnn;
        private ConexionSqlLite ConexSqlLite = new ConexionSqlLite("");
        private PaginadorProducto<ItemProducto> PaginadorCProductos;
        private ListadoProductPag ListadoProductosPag;
        private List<ItemProducto> productoDB;

        public Errores ConsultarProducto(DatosProducto DatosProducto, Usuario Usuario, out PaginadorProducto<ItemProducto> dproducto)
        {
            Errores _err = null;
            dproducto = null;
            productoDB = new List<ItemProducto>();
            PaginadorCProductos = new PaginadorProducto<ItemProducto>();
            string cons;
            try
            {
                cnn = new ConexionDB();
                cnn.setConnection(ConexSqlLite.obtenerConexionSyscom("dbpar"));
                #region consulta produccion
                if (!DatosProducto.SaldosCiaBod)
                {
                    cons = @"select  REF_COD CodProducto,REF_DES Descripción,REF_PR1 Lista1,REF_PR2 Lista2,REF_PR3 Lista3,P.POR_VAL Impuesto,D.POR_VAL Descuento," +
                                   "REF_GRU CodigoGru,GRU_DES NombreGru,REF_SUB CodigoSub,SUB_DES NombreSub,REF_SAL SaldoTotal,REF_FEC FechaCreacion,SC.SAL_CIA CodCia,SCIA.SAL_SAL Saldocia,SC.SAL_SEC CodBodega,SC.SAL_SAL Saldobodega  " +
                                   " from dbo.REFERENCIAS R " +
                                   " left join dbo.SUBGRUPOS S on R.REF_SUB=S.SUB_COD" +
                                   " left join dbo.GRUPOS G on R.REF_GRU=G.GRU_COD" +
                                   " left join dbo.TABLAPOR P on G.GRU_IVA=P.POR_COD" +
                                   " left join dbo.TABLAPOR D on G.GRU_DCT=D.POR_COD" +
                                   " left join " + ConexSqlLite.obtenerConexionSQLServer("dbsal").InitialCatalog + ".dbo.saldosref SC on REF_COD=SAL_COD /*and SAL_CIA=(select usu_cia from " + ConexSqlLite.obtenerConexionSQLServer("dbacc").InitialCatalog + ".dbo.usuarios where usu_id='" + Usuario.UserName + "')*/" +
                                   " left join (select SAL_COD,SAL_CIA,SUM(SAL_SAL) SAL_SAL from " + ConexSqlLite.obtenerConexionSQLServer("dbsal").InitialCatalog + ".dbo.saldosref where SAL_COD='ACD0076' group by SAL_COD,SAL_CIA) SCIA on REF_COD=SCIA.SAL_COD and SC.SAL_CIA=SCIA.SAL_CIA " +
                                   " where (REF_COD like '%" + DatosProducto.CodOrDesProd + "%' OR REF_DES like '%" + DatosProducto.CodOrDesProd + "%') and" +
                                   " (REF_SUB like '%" + DatosProducto.Subgrupo + "%' OR SUB_DES like '%" + DatosProducto.Subgrupo + "%') and " +
                                   " (REF_GRU like '%" + DatosProducto.Grupo + "%' OR GRU_DES like '%" + DatosProducto.Grupo + "%') and REF_BLO=0";
                }
                else
                {
                    cons = @"select  REF_COD CodProducto,REF_DES Descripción,REF_PR1 Lista1,REF_PR2 Lista2,REF_PR3 Lista3,P.POR_VAL Impuesto,D.POR_VAL Descuento," +
                                   "REF_GRU CodigoGru,GRU_DES NombreGru,REF_SUB CodigoSub,SUB_DES NombreSub,REF_SAL SaldoTotal,REF_FEC FechaCreacion,SC.SAL_CIA CodCia,SCIA.SAL_SAL Saldocia,SC.SAL_SEC CodBodega,SC.SAL_SAL Saldobodega  " +
                                   " from dbo.REFERENCIAS R " +
                                   " left join dbo.SUBGRUPOS S on R.REF_SUB=S.SUB_COD" +
                                   " left join dbo.GRUPOS G on R.REF_GRU=G.GRU_COD" +
                                   " left join dbo.TABLAPOR P on G.GRU_IVA=P.POR_COD" +
                                   " left join dbo.TABLAPOR D on G.GRU_DCT=D.POR_COD" +
                                   " left join " + ConexSqlLite.obtenerConexionSQLServer("dbsal").InitialCatalog + ".dbo.saldosref SC on REF_COD=SAL_COD and SAL_CIA=(select usu_cia from " + ConexSqlLite.obtenerConexionSQLServer("dbacc").InitialCatalog + ".dbo.usuarios where usu_id='" + Usuario.UserName + "')" +
                                   " left join (select SAL_COD,SAL_CIA,SUM(SAL_SAL) SAL_SAL from " + ConexSqlLite.obtenerConexionSQLServer("dbsal").InitialCatalog + ".dbo.saldosref where SAL_COD='ACD0076' group by SAL_COD,SAL_CIA) SCIA on REF_COD=SCIA.SAL_COD and SC.SAL_CIA=SCIA.SAL_CIA " +
                                   " where (REF_COD like '%" + DatosProducto.CodOrDesProd + "%' OR REF_DES like '%" + DatosProducto.CodOrDesProd + "%') and " +
                                  " (REF_SUB like '%" + DatosProducto.Subgrupo + "%' OR SUB_DES like '%" + DatosProducto.Subgrupo + "%') and " +
                                   " (REF_GRU like '%" + DatosProducto.Grupo + "%' OR GRU_DES like '%" + DatosProducto.Grupo + "%') and REF_BLO=0";
                }

                #endregion
                #region consulta pruebas coodepetrol
                //string cons = @"select top 10 REF_COD CodProducto,REF_DES Descripción,REF_PR1 Lista1,REF_PR2 Lista2,REF_PR3 Lista3,P.POR_VAL Impuesto,D.POR_VAL Descuento
                //                from dbpar_coode..REFERENCIAS R left join dbpar_coode..GRUPOS G on R.REF_GRU=G.GRU_COD
                //                left join dbpar_coode..TABLAPOR P on G.GRU_IVA=P.POR_COD
                //                left join dbpar_coode..TABLAPOR D on G.GRU_DCT=D.POR_COD
                //                where REF_COD like '%" + CodProducto + "%' OR REF_DES like '%" + CodProducto + "%'";
                #endregion
                cnn.resetQuery();
                cnn.setCustomQuery(cons);
                cnn.ejecutarQuery();
                if (cnn.getDataTable() != null && cnn.getDataTable().Rows.Count > 0)
                {
                    DataTable dtDatos = cnn.getDataTable();
                    productoDB = cnn.DataTableToList<ItemProducto>("CodProducto,Descripción,Lista1,Lista2,Lista3,Impuesto,Descuento,CodigoGru,NombreGru,CodigoSub,NombreSub,SaldoTotal,FechaCreacion".Split(','));
                    productoDB.ForEach(m =>
                    {
                        m.itemCia = new List<ItemCia>();
                        m.itemCia = cnn.DataTableToList<ItemCia>(dtDatos.Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("CodProducto").Equals(m.CodProducto)).CopyToDataTable().AsDataView().ToTable(true, "CodCia,Saldocia,CodBodega,Saldobodega".Split(',')));
                    });
                    //dproducto = cnn.DataTableToList<itemProducto>();
                    //productoDB = cnn.DataTableToList<itemProducto>();
                    int _TotalRegistros = 0;
                    int _TotalPaginas = 0;
                    int registros_por_pagina;
                    int pagina;

                    if (DatosProducto.RegistrosPorPagina == 0)
                        registros_por_pagina = 10;
                    else
                        registros_por_pagina = DatosProducto.RegistrosPorPagina;

                    if (DatosProducto.PaginaActual == 0)
                        pagina = 1;
                    else
                        pagina = DatosProducto.PaginaActual;



                    _TotalRegistros = productoDB.Count();

                    productoDB = productoDB.Skip((pagina - 1) * registros_por_pagina)
                                                     .Take(registros_por_pagina)
                                                     .ToList();

                    _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / registros_por_pagina);


                    PaginadorCProductos = new PaginadorProducto<ItemProducto>()
                    {
                        PaginaActual = pagina,
                        RegistrosPorPagina = registros_por_pagina,
                        TotalRegistros = _TotalRegistros,
                        TotalPaginas = _TotalPaginas,
                        Resultado = productoDB
                    };

                    dproducto = PaginadorCProductos;

                }
                else
                    _err = new Errores { codigo = "PROD_003", descripcion = "¡No se encontró ningún producto!" };
            }
            catch (Exception ex)
            {
                _err = new Errores { descripcion = ex.Message };
            }
            return _err;
        }
    }
}