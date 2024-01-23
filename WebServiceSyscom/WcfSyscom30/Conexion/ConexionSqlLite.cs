using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SQLite;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace WcfSyscom30.Conexion
{
    public class ConexionSqlLite
    {

        /// <summary>
        ///
        /// </summary>
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        public List<string> sqlCondicionesY = new List<string>();
        public List<string> sqlCondicionesO = new List<string>();
        public List<string> sqlTablas = new List<string>();
        public List<string> sqlCampos = new List<string>();
        public List<string> sqlOrderBy = new List<string>();
        private List<string> sqlConsultas = new List<string>();

        public ConexionSqlLite(string _pmDBFileSqlite)
        {
            this.DB = new SQLiteDataAdapter();
            this.DS = new DataSet();

            sql_con = new SQLiteConnection
    ("Data Source=" + System.AppDomain.CurrentDomain.BaseDirectory + "\\BD.reg;Version=3;");

            if (!File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\BD.reg"))
                createDataBase();

            if (sql_con.State != ConnectionState.Open)
                sql_con.Open();
        }


        /// <summary>
        /// Verdadero si esta BD.reg [BDSQ lite existe].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [BDSQ lite existe]; otherwise, <c>false</c>.
        /// </value>
        public static bool BDSQLiteExiste { get; set; }
        /// <summary>
        /// Verdadero si se identifico que base de datos es <see cref="ConexionBD"/> is configuracion.
        /// </summary>
        /// <value>
        ///   <c>true</c> if configuracion; otherwise, <c>false</c>.
        /// </value>
        public static bool configuracion { get; set; }
        /// <summary>
        /// 40 o 30 segun la base de datos que es.
        /// </summary>
        /// <value>
        /// The base dedatos.
        /// </value>
        public static int BaseDedatos { get; set; }
        public static string error { get; set; }

        /// <summary>
        /// Comprobar la base de datos 
        /// </summary>
        public static void ComprobarBD()
        {

            //comprobar que el archivo este en el directorio 
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\BD.reg"))
            {
                BDSQLiteExiste = true;
            }
            Console.WriteLine("Conexión exitosa a la base de datos SQLite.");
            if (BDSQLiteExiste)
            {
                if (!configuracion)
                {
                    string rutaBaseDatos = System.AppDomain.CurrentDomain.BaseDirectory + "\\BD.reg"; // Reemplaza con la ruta real del archivo .reg

                    if (System.IO.File.Exists(rutaBaseDatos))
                    {
                        string cadenaConexion = "Data Source=" + System.AppDomain.CurrentDomain.BaseDirectory + "\\BD.reg;Version=3;";

                        try
                        {
                            using (SQLiteConnection conexion = new SQLiteConnection(cadenaConexion))
                            {
                                conexion.Open();

                                string consulta = "SELECT cnfnombre FROM sysConfigBD";
                                using (SQLiteCommand cmd = new SQLiteCommand(consulta, conexion))
                                {
                                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            string cnfnombre = reader["cnfnombre"].ToString();

                                            // Comprueba si la columna cnfnombre contiene "dbmov"
                                            if (cnfnombre.Contains("dbmov"))
                                            {
                                                //base de datos 30
                                                BaseDedatos = 30;
                                                configuracion = true;


                                            }
                                            else if (cnfnombre.Contains("dbSyscom"))//comprobacion de base de datos 40
                                            {
                                                BaseDedatos = 40;
                                                configuracion = true;
                                            }
                                            else
                                            {
                                                configuracion = false;
                                                //error de la base de datos
                                                error = "no se identifica ninguna base de datos";
                                            }

                                        }
                                    }
                                }
                                conexion.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            error = "Error durante la ejecución de la consulta SQL: " + ex.Message;
                        }

                    }
                    else
                    {
                        error = "El archivo .reg no existe.";
                    }
                }
            }
        }

        public void setCustomQuery(string _pmQuery)
        {
            this.sqlConsultas.Add(_pmQuery);
        }

        public void reiniciar()
        {
            this.sqlCondicionesY.Clear();
            this.sqlCondicionesO.Clear();
            this.sqlTablas.Clear();
            this.sqlCampos.Clear();
            this.sqlOrderBy.Clear();
            this.DS = new DataSet();
            this.DT = new DataTable();
        }

        public void reiniciarSql()
        {
            this.reiniciar();
            this.sqlConsultas.Clear();
        }

        private string consCondiciones()
        {
            string sqlQuery = (this.sqlCondicionesY.Count() > 0 ? String.Join(" AND ", this.sqlCondicionesY.ToArray()) : "");
            sqlQuery += (this.sqlCondicionesO.Count() > 0 ? (this.sqlCondicionesY.Count() > 0 ? " AND " : "") + "(" + String.Join(" OR ", this.sqlCondicionesO.ToArray()) + ")" : "");
            return sqlQuery;
        }

        public void Close()
        {
            this.sql_con.Close();
            this.sql_con.Dispose();
        }

        public void select()
        {
            string sqlQuery = "select " + (this.sqlCampos.Count > 0 ? String.Join(",", this.sqlCampos.ToArray()) : " * ");
            sqlQuery += " from " + String.Join(" ", sqlTablas.ToArray());
            if ((this.sqlCondicionesY.Count() > 0) || (this.sqlCondicionesO.Count() > 0))
            {
                sqlQuery += " where ";
                sqlQuery += (this.sqlCondicionesY.Count() > 0 ? String.Join(" AND ", this.sqlCondicionesY.ToArray()) : "");
                sqlQuery += (this.sqlCondicionesO.Count() > 0 ? (this.sqlCondicionesY.Count() > 0 ? " AND " : "") + "(" + String.Join(" OR ", this.sqlCondicionesO.ToArray()) + ")" : "");
            }
            sqlQuery += (this.sqlOrderBy.Count() > 0 ? " order by " + String.Join(",", this.sqlOrderBy.ToArray()) : "");
            reiniciar();
            this.sqlConsultas.Add(sqlQuery);
        }

        public DataRow nuevaFila(string _tabla = null)
        {
            DataRow dr;
            if (String.IsNullOrEmpty(_tabla))
                dr = this.DS.Tables[0].NewRow();
            else
                dr = this.DS.Tables[_tabla].NewRow();
            return dr;
        }

        public bool insert(DataRow pmNuevoRegistro)
        {
            bool exito = true;

            try
            {
                if (this.DS.Tables.Count == 0)
                    throw new Exception("No hay ninguna tabla asociada a la conexion.");

                List<string> columnas = new List<string>();
                List<string> valores = new List<string>();
                foreach (DataColumn col in pmNuevoRegistro.Table.Columns)
                {
                    columnas.Add(col.ColumnName);
                    valores.Add("@" + col.ColumnName);
                }

                string sqlQuery = "INSERT INTO " + pmNuevoRegistro.Table.TableName;
                sqlQuery = sqlQuery + " (" + String.Join(",", columnas.ToArray()) + ")";
                sqlQuery = sqlQuery + " VALUES (" + String.Join(",", valores.ToArray()) + ")";

                this.DB.InsertCommand = new SQLiteCommand(sqlQuery, this.sql_con);

                for (Int32 i = 0; i < pmNuevoRegistro.Table.Columns.Count; i++)
                    this.DB.InsertCommand.Parameters.Add(new SQLiteParameter("@" + pmNuevoRegistro.Table.Columns[i].ColumnName, pmNuevoRegistro[i]));

                this.DB.InsertCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                exito = false;

            }
            return exito;

        }

        public bool update(DataRow pmRegistro, Dictionary<string, object> pmCondicion)
        {
            bool error = false;

            try
            {
                if (this.DS.Tables.Count == 0)
                    throw new Exception("No hay ninguna tabla asociada a la conexion.");

                List<string> campos = new List<string>();
                foreach (DataColumn columna in pmRegistro.Table.Columns)
                    campos.Add(columna.ColumnName + " = @" + columna.ColumnName);

                List<string> condicion = new List<string>();
                foreach (var cond in pmCondicion)
                    condicion.Add(cond.Key + " = @pm" + cond.Key);

                string sqlQuery = "UPDATE " + this.DS.Tables[0].TableName;
                sqlQuery = sqlQuery + " SET " + String.Join(",", campos.ToArray());
                sqlQuery = sqlQuery + (condicion.ToArray().Count() > 0 ? " WHERE " + String.Join(" and ", condicion.ToArray()) + "" : "");

                this.DB.UpdateCommand = new SQLiteCommand(sqlQuery, this.sql_con);

                for (Int32 i = 0; i < pmRegistro.Table.Columns.Count; i++)
                    this.DB.UpdateCommand.Parameters.Add(new SQLiteParameter("@" + pmRegistro.Table.Columns[i].ColumnName, pmRegistro[i]));

                foreach (var cond in pmCondicion)
                    this.DB.UpdateCommand.Parameters.Add(new SQLiteParameter("@pm" + cond.Key, cond.Value));

                this.DB.UpdateCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                error = true;
            }
            return error;
        }

        public void beginTran()
        {

        }

        public void ejecutarSQL(string _pmTabla = null, bool result = true)
        {
            foreach (string sql in this.sqlConsultas)
            {
                this.DB.SelectCommand = new SQLiteCommand(sql, this.sql_con);
                this.DB.SelectCommand.CommandType = CommandType.Text;
                this.DB.SelectCommand.Parameters.Clear();
                switch (sql.Split(' ')[0].ToLower())
                {
                    case "update":
                        this.DB.SelectCommand.ExecuteNonQuery();
                        break;
                    case "delete":
                        this.DB.SelectCommand.ExecuteNonQuery();
                        break;
                    case "insert":
                        this.DB.SelectCommand.ExecuteNonQuery();
                        break;
                    default:
                        this.DB.SelectCommand.ExecuteNonQuery();
                        if (result == true)
                        {
                            if (String.IsNullOrEmpty(_pmTabla))
                                this.DB.Fill(this.DS);
                            else
                                this.DB.Fill(this.DS, _pmTabla);
                        }
                        break;
                }
            }
            this.sqlConsultas.Clear();
        }

        public DataSet obtenerTablas()
        {
            return DS;
        }

        public DataTable obtenerTabla(string _pmNombreTablaProc)
        {
            return DS.Tables[_pmNombreTablaProc];
        }

        private void createDataBase()
        {
            SQLiteConnection.CreateFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\BD.reg");
            sql_con.Open();
            sql_cmd = new SQLiteCommand(@"CREATE TABLE sysComponentes (
                appId integer primary key autoincrement,
                appNombre text not null,
                appDescripcion text not null,
                appVersion varchar(14) not null,
                appDisponible text not null,
                appArchivoZip varchar(20) not null,
                appImagen text
            );

            CREATE INDEX sysComponentes_appNombre_idx ON sysComponentes (appNombre);
            CREATE INDEX sysComponentes_appDescripcion_idx ON sysComponentes (appDescripcion);

            CREATE TABLE sysConfigBD (
	            cnfidbd varchar(80) NOT NULL,
	            cnfnombre varchar(80) NOT NULL,
	            cnfservidor varchar(40) NOT NULL,
	            cnfusuario varchar(20) NOT NULL,
	            cnfclave varchar(20) NOT NULL,
	            cnfbasedatos varchar(20) NOT NULL,
	            PRIMARY KEY(cnfidbd)
            );
            CREATE INDEX sysConfigBD_cnfnombre_idx ON sysConfigBD (cnfnombre);
            CREATE INDEX sysConfigBD_cnfbasedatos_idx ON sysConfigBD (cnfbasedatos);

            CREATE TABLE sysConfigEmail (
	            emlnombre varchar(80) NOT NULL,
	            emlservidor varchar(40) NOT NULL,
	            emlusuario varchar(20) NOT NULL,
	            emlclave varchar(20) NOT NULL,
	            emlpuerto INT NOT NULL,
	            emlssl BIT NOT NULL,
	            PRIMARY KEY(emlNombre)
            );

            CREATE TABLE sysInterfaces (
                wsExtension VARCHAR(20), 
                wsUsuario VARCHAR(50) not null, 
                wsClave VARCHAR(50) not null, 
                wsReferencia1 VARCHAR(50) null, 
                wsReferencia2 VARCHAR(50) null, 
                wsReferencia3 VARCHAR(50) null, 
                wsFechaCargue datetime not null, 
                wsIntervalo INT not null,
                PRIMARY KEY(wsExtension)
            );

 ", sql_con);
            sql_cmd.ExecuteNonQuery();
        }

        public bool Eliminar(string pmTablaBD = null, Dictionary<string, object> pmCondicion = null)
        {
            bool exito = true;
            try
            {
                List<string> sqlCondicion = new List<string>();
                List<SQLiteParameter> sqlParametros = new List<SQLiteParameter>();
                foreach (var condicion in pmCondicion)
                {
                    sqlCondicion.Add(condicion.Key + " = @" + condicion.Key);
                    sqlParametros.Add(new SQLiteParameter("@" + condicion.Key, condicion.Value));
                }

                this.DB.DeleteCommand = new SQLiteCommand("DELETE FROM " + (pmTablaBD ?? this.DS.Tables[0].TableName) + (pmCondicion != null ? " WHERE " + String.Join(" and ", sqlCondicion.ToArray()) : ""), this.sql_con);

                this.DB.DeleteCommand.Parameters.AddRange(sqlParametros.ToArray());
                this.DB.DeleteCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                exito = true;

            }
            return exito;
        }

        public SqlConnectionStringBuilder obtenerConexionSQLServer(string dbNombreDBServer)
        {
            SqlConnectionStringBuilder con = null;
            this.sqlTablas.Add("sysConfigBD");
            this.sqlCampos.Add("cnfservidor,cnfusuario,cnfclave,cnfbasedatos");
            this.sqlCondicionesY.Add("lower(cnfnombre) = '" + dbNombreDBServer.ToLower() + "'");
            this.select();
            this.ejecutarSQL("sysConfigBD");
            DataTable dt = this.obtenerTabla("sysConfigBD");
            this.reiniciarSql();

            if (dt.Rows.Count > 0)
            {
                if (!String.IsNullOrEmpty(dt.Rows[0].Field<string>("cnfservidor")))
                {
                    con = new SqlConnectionStringBuilder();
                    con.DataSource = dt.Rows[0].Field<string>("cnfservidor");
                    con.InitialCatalog = dt.Rows[0].Field<string>("cnfbasedatos");
                    con.UserID = dt.Rows[0].Field<string>("cnfusuario");
                    con.Password = dt.Rows[0].Field<string>("cnfclave");
                }
            }
            else
            {
                //throw new SyntaxErrorException("No se encontró la configuración de la base de datos '" + dbNombreDBServer +  "'");
            }

            return con;
        }

        public SqlConnectionStringBuilder obtenerConexionSyscom(string db = "dbacc")
        {
            SqlConnectionStringBuilder conexion = this.obtenerConexionSQLServer(db);

            if (conexion == null)
                conexion = this.obtenerConexionSQLServer("dbSyscom");

            return conexion;
        }

        public SqlConnectionStringBuilder obtenerConexionSpeed()
        {
            SqlConnectionStringBuilder conexion = this.obtenerConexionSQLServer("dbspeed");

            if (conexion == null)
                conexion = this.obtenerConexionSQLServer("dbspeed");

            return conexion;
        }

        public bool InsertRegFromModel<TModel>(TModel Instancia, List<string> LlavesPrimarias)
        {
            bool exito = true;
            Dictionary<string, object> Condicion = new Dictionary<string, object>();

            try
            {
                Type modelo = typeof(TModel);

                this.reiniciarSql();

                this.setCustomQuery("select * from " + modelo.Name + " LIMIT 0");
                this.ejecutarSQL(modelo.Name);

                DataRow filaNueva = this.nuevaFila(modelo.Name);

                foreach (DataColumn col in filaNueva.Table.Columns)
                {
                    try
                    {
                        filaNueva[col.ColumnName] = modelo.GetProperty(col.ColumnName).GetValue((TModel)Instancia, null);
                    }
                    catch (Exception ex2)
                    {

                    }
                }

                foreach (string campo in LlavesPrimarias)
                {
                    try
                    {
                        Condicion.Add(campo, modelo.GetProperty(campo).GetValue((TModel)Instancia, null));
                    }
                    catch (Exception ex2)
                    {

                    }
                }

                if (this.insert(filaNueva))
                    exito = true;
                else
                    exito = this.update(filaNueva, Condicion);
            }
            catch (Exception ex)
            {
                exito = false;

            }
            return exito;
        }

        public bool DeleteRegFromModel<TModel>(TModel Instancia, List<string> LlavesPrimarias)
        {
            bool exito = false;
            Dictionary<string, object> Condicion = new Dictionary<string, object>();

            try
            {
                Type modelo = typeof(TModel);

                this.reiniciarSql();

                foreach (string campo in LlavesPrimarias)
                {
                    try
                    {
                        Condicion.Add(campo, modelo.GetProperty(campo).GetValue((TModel)Instancia, null));
                    }
                    catch (Exception ex2)
                    {

                    }
                }

                exito = this.Eliminar(modelo.Name, Condicion);
            }
            catch (Exception ex)
            {
                exito = false;

            }

            return exito;
        }

    }
}