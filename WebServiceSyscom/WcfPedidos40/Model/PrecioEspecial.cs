using connect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WcfPedidos40.Model
{
    public class PrecioEspecial
    {
        public string Tipo = null;
        public Int32? Numero = null;
        public DateTime? FecInicial = null;
        public DateTime? FecFinal = null;
        public string CdCia = null;
        public string CdLinea = null;
        public string CdGrupo = null;
        public string CdSubgrupo = null;
        public string CdMarca = null;
        public string CdProducto = null;
        public decimal? Tarifa = null;
        public string SimbTfa = null;
        public string LtaPre = null;
        public string CdTarifa = null;
        public string CdClie = null;
        public string CdAgencia = null;
        public string CdVend = null;
        public string CdCiudad = null;
        public string CdZona = null;
        public string CdSzona = null;
        public string CdGruCli = null;
        public string OtrosCrit = null;
        public bool? Anulado = null;
        public string IdUsuario = null;
        public string CodBod = null;

        public void ObtenerTarifaEspecial(DateTime _FechaActual, Int32 _ListaPrecio, string _CdCia = null, string _CdLinea = null, string _CdGrupo = null, string _CdSubgrupo = null, string _CdMarca = null, string _CdProducto = null, string _CdTarifa = null, string _CdClie = null, string _CdAgencia = null, string _CdVend = null, string _CdCiudad = null, string _CdZona = null, string _CdSzona = null, string _CdGruCli = null, string _OtrosCrit = null)
        {
            Conexion con = new Conexion();
            con.setConnection("Syscom");
            /*Se importan los campos necesarios para grabar el pedido partiendo de las tablas OPedido y Kardex*/
            con.resetQuery();
            con.qryTables.Add(@"(
                SELECT 
                    Tipo
                    ,Numero
                    ,FecInicial
                    ,FecFinal
                    ,case when [CdCia]='00' then null else[CdCia] end as [CdCia]
                    ,case when [CdLinea]='0' then null else[CdLinea] end as [CdLinea]
                    ,case when [CdGrupo]='0' then null else[CdGrupo] end as [CdGrupo]
                    ,case when [CdSubgrupo]='0' then null else[CdSubgrupo] end as [CdSubgrupo]
                    ,case when [CdMarca]='0' then null else[CdMarca] end as [CdMarca]
                    ,case when [CdProducto]='0' then null else[CdProducto] end as [CdProducto]
                    ,Tarifa
                    ,SimbTfa
                    ,LtaPre
                    ,case when [CdTarifa]='' then null else[CdTarifa] end as [CdTarifa]
                    ,case when [CdClie]='0' then null else[CdClie] end as [CdClie]
                    ,case when [CdAgencia]='0' then null else[CdAgencia] end as [CdAgencia]
                    ,case when [CdVend]='0' then null else[CdVend] end as [CdVend]
                    ,case when [CdCiudad]='0' then null else[CdCiudad] end as [CdCiudad]
                    ,case when [CdZona]='0' then null else[CdZona] end as [CdZona]
                    ,case when [CdSzona]='0' then null else[CdSzona] end as [CdSzona]
                    ,case when [CdGruCli]='0' then null else[CdGruCli] end as [CdGruCli]
                    ,OtrosCrit
                    ,Anulado
                    ,IdUsuario
                    ,CodBod
                FROM Trn_ProdTarf) v");
            con.addWhereAND("Anulado = 0 and FecInicial <= '" + _FechaActual.ToString("yyyyMMdd") + "' and FecFinal >= '" + _FechaActual.ToString("yyyyMMdd") + "' and LtaPre = '" + _ListaPrecio.ToString() + "'");

            #region Condiciones
            if (!String.IsNullOrEmpty(_CdCia))
                con.addWhereAND("isnull(CdCia, '" + _CdCia + "') = '" + _CdCia + "'");

            if (!String.IsNullOrEmpty(_CdLinea))
                con.addWhereAND("isnull(CdLinea, '" + _CdLinea + "') = '" + _CdLinea + "'");

            if (!String.IsNullOrEmpty(_CdGrupo))
                con.addWhereAND("isnull(CdGrupo, '" + _CdGrupo + "') = '" + _CdGrupo + "'");

            if (!String.IsNullOrEmpty(_CdSubgrupo))
                con.addWhereAND("isnull(CdSubgrupo, '" + _CdSubgrupo + "') = '" + _CdSubgrupo + "'");

            if (!String.IsNullOrEmpty(_CdMarca))
                con.addWhereAND("isnull(CdMarca, '" + _CdMarca + "') = '" + _CdMarca + "'");

            if (!String.IsNullOrEmpty(_CdProducto))
                con.addWhereAND("isnull(CdProducto, '" + _CdProducto + "') = '" + _CdProducto + "'");

            if (!String.IsNullOrEmpty(_CdTarifa))
                con.addWhereAND("isnull(CdTarifa, '" + _CdTarifa + "') = '" + _CdTarifa + "'");

            if (!String.IsNullOrEmpty(_CdClie))
                con.addWhereAND("isnull(CdClie, '" + _CdClie + "') = '" + _CdClie + "'");

            if (!String.IsNullOrEmpty(_CdAgencia))
                con.addWhereAND("isnull(CdAgencia, '" + _CdAgencia + "') = '" + _CdAgencia + "'");

            if (!String.IsNullOrEmpty(_CdVend))
                con.addWhereAND("isnull(CdVend, '" + _CdVend + "') = '" + _CdVend + "'");

            if (!String.IsNullOrEmpty(_CdCiudad))
                con.addWhereAND("isnull(CdCiudad, '" + _CdCiudad + "') = '" + _CdCiudad + "'");

            if (!String.IsNullOrEmpty(_CdZona))
                con.addWhereAND("isnull(CdZona, '" + _CdZona + "') = '" + _CdZona + "'");

            if (!String.IsNullOrEmpty(_CdSzona))
                con.addWhereAND("isnull(CdSzona, '" + _CdSzona + "') = '" + _CdSzona + "'");

            if (!String.IsNullOrEmpty(_CdGruCli))
                con.addWhereAND("isnull(CdGruCli, '" + _CdGruCli + "') = '" + _CdGruCli + "'");
            #endregion
            con.select();
            con.ejecutarQuery();

            if (con.getDataTable().Rows.Count > 0)
            {
                DataTable tabla = con.getDataTable();

                int indice = -1;
                int min = 99;
                for (var c = 0; c < tabla.Rows.Count; c++)
                {
                    int acum = 0;
                    for (var i = 0; i < tabla.Columns.Count; i++)
                    {
                        if (tabla.Rows[c][i].ToString() == "0" || tabla.Rows[c][i] is DBNull || String.IsNullOrEmpty(tabla.Rows[c][i].ToString()))
                            acum++;
                    }
                    if (acum < min)
                    {
                        min = acum;
                        indice = c;
                    }
                }

                Tipo = tabla.Rows[indice].Field<string>("Tipo");
                Numero = tabla.Rows[indice].Field<Int32?>("Numero");
                FecInicial = tabla.Rows[indice].Field<DateTime?>("FecInicial");
                FecFinal = tabla.Rows[indice].Field<DateTime?>("FecFinal");
                CdCia = tabla.Rows[indice].Field<string>("CdCia");
                CdLinea = tabla.Rows[indice].Field<string>("CdLinea");
                CdGrupo = tabla.Rows[indice].Field<string>("CdGrupo");
                CdSubgrupo = tabla.Rows[indice].Field<string>("CdSubgrupo");
                CdMarca = tabla.Rows[indice].Field<string>("CdMarca");
                CdProducto = tabla.Rows[indice].Field<string>("CdProducto");
                Tarifa = tabla.Rows[indice].Field<decimal?>("Tarifa");
                SimbTfa = tabla.Rows[indice].Field<string>("SimbTfa");
                LtaPre = tabla.Rows[indice].Field<string>("LtaPre");
                CdTarifa = tabla.Rows[indice].Field<string>("CdTarifa");
                CdClie = tabla.Rows[indice].Field<string>("CdClie");
                CdAgencia = tabla.Rows[indice].Field<string>("CdAgencia");
                CdVend = tabla.Rows[indice].Field<string>("CdVend");
                CdCiudad = tabla.Rows[indice].Field<string>("CdCiudad");
                CdZona = tabla.Rows[indice].Field<string>("CdZona");
                CdSzona = tabla.Rows[indice].Field<string>("CdSzona");
                CdGruCli = tabla.Rows[indice].Field<string>("CdGruCli");
                OtrosCrit = tabla.Rows[indice].Field<string>("OtrosCrit");
                Anulado = tabla.Rows[indice].Field<bool?>("Anulado");
                IdUsuario = tabla.Rows[indice].Field<string>("IdUsuario");
                CodBod = tabla.Rows[indice].Field<string>("CodBod");

            }
        }
    }
}