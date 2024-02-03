using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using wcfSyscom30.Conexion;

namespace wcfSyscom30.Models
{
    public class DatosCartera
    {
        public string Tercero { get; set; }
        public int SaldoCartera { get; set; }
        public List<itemCartera> Detalle { get; set; }


        private Conexion cnn;
        private clsConnSqlite ConexSqlLite = new clsConnSqlite("");
        public Errores ConsultarCartera(string NitCliente, out List<DatosCartera> DatCartera)
        {
            Errores _err = null;
            DatCartera = new List<DatosCartera>();
            try
            {
                cnn = new Conexion();
                cnn.setConnection(ConexSqlLite.obtenerConexionSyscom("dbsal"));
                string cons = @"SELECT VCE_NIT Tercero,(SELECT sum(VCE_VAL-VCE_ABO)  FROM VENCIMIENTOS WHERE VCE_NIT='" + NitCliente + @"') SaldoCartera,VCE_TIP TipoDocumento,VCE_NUM Documento,VCE_CIA Compañia,VCE_NV Vencimiento,VCE_EMI FechaEmision,VCE_FEC FechaVencimiento,VCE_VAL ValorTotal,VCE_ABO Abono,VCE_VAL-VCE_ABO Saldo
                                FROM VENCIMIENTOS WHERE VCE_NIT='" + NitCliente + @"' 
                                GROUP BY VCE_NUM,VCE_TIP,VCE_NV,VCE_CIA,VCE_EMI,VCE_FEC,VCE_VAL,VCE_ABO,VCE_NIT 
                                HAVING VCE_VAL > VCE_ABO";
                cnn.resetQuery();
                cnn.setCustomQuery(cons);
                cnn.ejecutarQuery();
                if (cnn.getDataTable() != null && cnn.getDataTable().Rows.Count > 0)
                {
                    DataTable dtDatos = cnn.getDataTable();
                    DatCartera = cnn.DataTableToList<DatosCartera>("Tercero,SaldoCartera".Split(','));
                    DatCartera.ForEach(m =>
                    {
                        m.Detalle = new List<itemCartera>();
                        //m.Detalle = cnn.DataTableToList<itemCartera>(dtDatos.AsEnumerable().Where(r => Convert.ToString(r["Tercero"]) != Convert.ToString(m.Tercero)).AsDataView().ToTable(true, "TipoDocumento,Documento,Compañia,Vencimiento,FechaEmision,FechaVencimiento,ValorTotal,Abono,Saldo".Split(',')));
                        m.Detalle = cnn.DataTableToList<itemCartera>(dtDatos.Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("Tercero").Equals(m.Tercero)).CopyToDataTable().AsDataView().ToTable(true, "TipoDocumento,Documento,Compañia,Vencimiento,FechaEmision,FechaVencimiento,ValorTotal,Abono,Saldo".Split(',')));
                    });
                }
                else
                {
                    _err = new Errores { codigo = "CLIEN_002", descripcion = "¡Cliente no está registrado o no tiene Deuda!" };
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