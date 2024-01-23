using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WcfSyscom30.Conexion;

namespace WcfSyscom30.Models
{
    public class DatosUsuario
    {
        public string IdUsuario { get; set; }
        public string Compañía { get; set; }
        public string Bodega { get; set; }
        public bool Esvendedor { get; set; }
        public bool EsCliente { get; set; }
        public string NombreTercero { get; set; }

        private ConexionDB cnn;
        private ConexionSqlLite ConexSqlLite = new ConexionSqlLite("");

        public Errores ConsultarUsuarios(string idusu, out DatosUsuario dusuario)
        {
            Errores _err = null;
            dusuario = new DatosUsuario();
            try
            {
                cnn = new ConexionDB();
                cnn.setConnection(ConexSqlLite.obtenerConexionSyscom("dbpar"));
                #region consulta produccion
                string cons = "SELECT USU_ID IdUsuario,USU_CIA Compañía,USU_SEC Bodega,NIT_VEN Esvendedor,NIT_CLI EsCliente,NIT_NOM NombreTercero  FROM " + ConexSqlLite.obtenerConexionSQLServer("dbacc").InitialCatalog + ".dbo.USUARIOS U LEFT JOIN dbo.NIT N ON  NIT_NIT=USU_ID WHERE usu_id='" + idusu + "'";
                #endregion
                #region consulta pruebas coodepetrol
                //string cons = "SELECT USU_ID IdUsuario,USU_CIA Compañía,USU_SEC Bodega,NIT_VEN Esvendedor,NIT_CLI EsCliente,NIT_NOM NombreTercero  FROM dbacc_coode..USUARIOS U LEFT JOIN dbpar_coode..NIT N ON  NIT_NIT=USU_ID WHERE usu_id='" + idusu + "'";
                #endregion
                cnn.resetQuery();
                cnn.setCustomQuery(cons);
                cnn.ejecutarQuery();
                if (cnn.getDataTable() != null && cnn.getDataTable().Rows.Count > 0)
                {
                    DataTable ds = cnn.getDataTable();
                    DataRow row = ds.Rows[0];
                    if (row["NombreTercero"].ToString() == null || String.IsNullOrWhiteSpace(row["NombreTercero"].ToString()))
                        _err = new Errores { codigo = "USER_004", descripcion = "¡El usuario no está creado como cliente!" };
                    else
                        dusuario = cnn.ConvertRowToModel<DatosUsuario>();
                }
                else
                {
                    _err = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
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