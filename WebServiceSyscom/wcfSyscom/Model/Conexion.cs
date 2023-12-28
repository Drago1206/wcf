using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using wcfSyscom.Model;

namespace Models
{

    /// <summary>
    /// Clase Conexion
    /// </summary>
    public class Conexion
    {

        private string strName = "Conexion";
        private SqlConnection sqlConn = new SqlConnection();
        private DataSet ds = new DataSet();
        private List<string> qryWhereAND = new List<string>();
        private List<string> qryWhereOR = new List<string>();
        private List<SqlParameter> sqlParameters = new List<SqlParameter>();
        private int resNumRows = 0;
        private string sqlQuery = "";
        private DataTable target = new DataTable();
        private SqlTransaction transaccion;
        public string DataBase = "";
        public string Server = "";
        public string User = "";
        public string Pass = "";


        public List<string> qryTables = new List<string>();
        public List<string> qryFields = new List<string>();
        public List<string> qryGroupBy = new List<string>();
        public List<string> qryOrderBy = new List<string>();
        public Dictionary<string, object> qryValues = new Dictionary<string, object>();

        private SqlDataAdapter adapter;

        /// <summary>
        /// Inicializa el nombre del connectionstring <see cref="Conexion"/>.
        /// </summary>
        public Conexion(){
            this.strName = "Conexion";

        }

        #region Tabla
        /// <summary>
        /// Ejecuta la consulta.
        /// </summary>
        /// <param name="_resultado">Tipo de consulta (select, insert, update).</param>
        /// <param name="_aliasConsulta">Nombre del datatable o alias.</param>
        /// <exception cref="System.ExecutionEngineException"></exception>
        public void ejecutarQuery(string _resultado = "select", string _aliasConsulta = null)
        {
            try
            {
                if (this.sqlConn.State != ConnectionState.Open)
                {
                    this.Connect();
                    this.sqlConn.Open();
                }
                this.adapter = new SqlDataAdapter(sqlQuery, this.sqlConn);
                this.adapter.SelectCommand.CommandTimeout = 0;
                if (this.transaccion != null)
                    this.adapter.SelectCommand.Transaction = this.transaccion;
                this.ds.Clear();
                switch (_resultado)
                {
                    case "insert":
                        this.adapter.SelectCommand.CommandType = CommandType.Text;
                        this.adapter.SelectCommand.Parameters.Clear();
                        this.adapter.SelectCommand.ExecuteNonQuery();
                        break;
                    case "update":
                        this.adapter.UpdateCommand.CommandType = CommandType.Text;
                        this.adapter.UpdateCommand.Parameters.Clear();
                        this.adapter.UpdateCommand.ExecuteNonQuery();
                        break;
                    case "delete":
                        this.adapter.DeleteCommand.CommandType = CommandType.Text;
                        this.adapter.DeleteCommand.Parameters.Clear();
                        this.adapter.DeleteCommand.ExecuteNonQuery();
                        break;
                    default:
                        this.adapter.SelectCommand.CommandType = CommandType.Text;
                        this.adapter.SelectCommand.Parameters.Clear();
//                        this.adapter.SelectCommand.ExecuteNonQuery();
                        if (_aliasConsulta != null)
                            this.adapter.Fill(this.ds, _aliasConsulta);
                        else
                            this.adapter.Fill(ds);
                        break;
                }
                if (this.transaccion == null)
                {
                    this.adapter.Dispose();
                    this.sqlConn.Close();
                }
            }
            catch (Exception e)
            {
                LogErrores.tareas.Add("Ha ocurrido un error al intentar ejecutar la consulta: " + e.Message);
                LogErrores.write();
                throw new ExecutionEngineException();
            }
            finally
            {
                if (this.transaccion == null)
                    sqlConn.Dispose();
            }
        }

        #region FuncionesTabla
        /// <summary>
        /// Arma la sentencia de insercion de acuerdo a los campos, tablas y condiciones.
        /// </summary>
        public void insert()
        {
            this.sqlQuery = "insert into " + String.Join("", qryTables.FirstOrDefault().ToString());
            this.sqlQuery = sqlQuery + " (" + String.Join(",", qryValues.Keys) + ")";
            this.sqlQuery = sqlQuery + " values ('" + String.Join("','", qryValues.Values) + "')";

        }

        /// <summary>
        /// Arma la sentencia de consulta de acuerdo a los campos, tablas y condiciones.
        /// </summary>
        public void select()
        {
            this.sqlQuery = "select " + (this.qryFields.Count > 0 ? String.Join(",", this.qryFields) : " * ");
            this.sqlQuery += " from " + String.Join(" ", qryTables);
            if ((this.qryWhereAND.Count() > 0) || (this.qryWhereOR.Count() > 0))
                this.sqlQuery += " where ";
            this.sqlQuery += (this.qryWhereAND.Count() > 0 ? String.Join(" AND ", this.qryWhereAND) : "");
            this.sqlQuery += (this.qryWhereOR.Count() > 0 ? (this.qryWhereAND.Count() > 0 ? " AND " : "") + "(" + String.Join(" OR ", this.qryWhereOR) + ")" : "");
            this.sqlQuery += (this.qryGroupBy.Count() > 0 ? " group by " + String.Join(",", this.qryGroupBy) : "");
            this.sqlQuery += (this.qryOrderBy.Count() > 0 ? " order by " + String.Join(",", this.qryOrderBy) : "");
        }

        /// <summary>
        /// Ejecuta una consulta sql directamente desde la funcion.
        /// </summary>
        /// <param name="_sqlQuery">Sentencia SQL.</param>
        public void setCustomQuery(string _sqlQuery)
        {
            this.sqlQuery = _sqlQuery;
        }

        //public void AddParametros(string _field, string _value)
        //{
        //    this.qryInsValores.Add(_field, _value);
        //}

        /// <summary>
        /// Inserta una condicion a la sentencia bajo el operador logico AND.
        /// </summary>
        /// <param name="_Where">Condicion AND.</param>
        public void addWhereAND(string _Where)
        {
            this.qryWhereAND.Add(_Where);
        }

        /// <summary>
        /// Inserta una condicion a la sentencia bajo el operador logico OR.
        /// </summary>
        /// <param name="_Where">Condicion OR.</param>
        public void addWhereOR(string _Where)
        {
            this.qryWhereOR.Add(_Where);
        }
        #endregion
        #endregion Tabla

        #region Procedimiento

        /// <summary>
        /// Añade los parametros a los procedimientos.
        /// </summary>
        /// <param name="_sqlParametros">Lista de parametros.</param>
        public void addParametersProc(List<SqlParameter> _sqlParametros)
        {
            this.sqlParameters.AddRange(_sqlParametros);
        }

        /// <summary>
        /// Ejeucuta el procedimiento.
        /// </summary>
        /// <param name="_procedimiento">Nombre del procedimiento.</param>
        /// <param name="_aliasProcedimiento">Alias del datatable.</param>
        /// <param name="_sinresultados">si es <c>true</c> no obtiene resultados.</param>
        /// <exception cref="System.ExecutionEngineException">
        /// No ejecuta el codigo si hay un error.
        /// </exception>
        public void ejecutarProcedimiento(string _procedimiento, string _aliasProcedimiento = null, bool _sinresultados = false)
        {

            try
            {
                if (this.sqlConn.State != ConnectionState.Open)
                {
                    this.Connect();
                    this.sqlConn.Open();
                }

                this.adapter = new SqlDataAdapter(_procedimiento, this.sqlConn);
                this.adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                this.adapter.SelectCommand.Parameters.Clear();
                this.ds.Clear();
                SqlParameter[] _sqlParameters = sqlParameters.ToArray();
                if (this.transaccion != null)
                    this.adapter.SelectCommand.Transaction = this.transaccion;
                foreach (var item in _sqlParameters)
                {
                    this.adapter.SelectCommand.Parameters.Add(item);
                }

                if(_sinresultados!=false)
                    this.adapter.SelectCommand.ExecuteNonQuery();
                else
                    if (_aliasProcedimiento != null)
                    {
                        this.adapter.Fill(this.ds, _aliasProcedimiento);
                        ds.Tables[0].TableName = _aliasProcedimiento;
                        ds.Tables[0].Namespace = _aliasProcedimiento;
                    }
                    else
                        this.adapter.Fill(this.ds);
                this.adapter.SelectCommand.Parameters.Clear();
                if (this.transaccion == null)
                {
                    this.adapter.Dispose();
                    this.sqlConn.Close();
                }
            }
            catch (Exception e)
            {
                LogErrores.tareas.Add("Ha ocurrido un error al intentar ejecutar la consulta: " + e.Message);
                LogErrores.write();

                throw e;
            }
            finally
            {
                if (this.transaccion == null)
                    sqlConn.Dispose();
            }
        }

        #endregion Procedimiento

        #region otrasFunciones

        /// <summary>
        /// Joins the data tables.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <param name="joinOn">The join on.</param>
        /// <returns></returns>
        public DataTable JoinDataTables(DataTable t1, DataTable t2, params Func<DataRow, DataRow, bool>[] joinOn)
        {
            DataTable result = new DataTable();
            foreach (DataColumn col in t1.Columns)
            {
                if (result.Columns[col.ColumnName] == null)
                    result.Columns.Add(col.ColumnName, col.DataType);
            }
            foreach (DataColumn col in t2.Columns)
            {
                if (result.Columns[col.ColumnName] == null)
                    result.Columns.Add(col.ColumnName, col.DataType);
            }
            foreach (DataRow row1 in t1.Rows)
            {
                var joinRows = t2.AsEnumerable().Where(row2 =>
                {
                    foreach (var parameter in joinOn)
                    {
                        if (!parameter(row1, row2)) return false;
                    }
                    return true;
                });
                foreach (DataRow fromRow in joinRows)
                {
                    DataRow insertRow = result.NewRow();
                    foreach (DataColumn col1 in t1.Columns)
                    {
                        insertRow[col1.ColumnName] = row1[col1.ColumnName];
                    }
                    foreach (DataColumn col2 in t2.Columns)
                    {
                        insertRow[col2.ColumnName] = fromRow[col2.ColumnName];
                    }
                    result.Rows.Add(insertRow);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the number filas.
        /// </summary>
        /// <returns></returns>
        public int getNumFilas()
        {
            int filas = this.getDataTable().Rows.Count;
            return filas;
        }
        /// <summary>
        /// Resets the query.
        /// </summary>
        public void resetQuery()
        {
            this.qryTables.Clear();
            this.qryFields.Clear();
            this.qryWhereAND.Clear();
            this.qryWhereOR.Clear();
            this.qryGroupBy.Clear();
            this.qryOrderBy.Clear();
            this.ds.Tables.Clear();
            this.sqlParameters = new List<SqlParameter>();
            this.resNumRows = 0;
        }
        /// <summary>
        /// Gets the campos.
        /// </summary>
        /// <returns></returns>
        public string getCampos()
        {
            return String.Join(",",qryFields);
        }
        /// <summary>
        /// Inserts the data table.
        /// </summary>
        /// <param name="_tabla">The _tabla.</param>
        /// <param name="_datos">The _datos.</param>
        public bool insertDataTable(string _tabla, DataTable _datos)
        {
            bool insertado = true;
            try
            {

                if (this.sqlConn.State != ConnectionState.Open)
                {
                    this.Connect();
                    this.sqlConn.Open();
                }
                SqlBulkCopy sbc;
                if (this.transaccion != null)
                    sbc = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.Default, this.transaccion);
                else
                    sbc = new SqlBulkCopy(sqlConn);
                sbc.DestinationTableName = _tabla;
                sbc.WriteToServer(_datos);
                if (this.transaccion == null)
                {
                    this.adapter.Dispose();
                    this.sqlConn.Close();
                }
            }
            catch (Exception e)
            {
                LogErrores.tareas.Add("Ha ocurrido un error al intentar ejecutar la consulta: " + e.Message + "\nDatos=>\n" + string.Join("///", _datos.Rows.Cast<DataRow>().FirstOrDefault().ItemArray));
                LogErrores.write();
                insertado = false;

            }
            finally
            {
                if (this.transaccion == null)
                    sqlConn.Dispose();
            }
            return insertado;
        }
        /// <summary>
        /// Inserts the data table.
        /// </summary>
        /// <param name="_tabla">The _tabla.</param>
        /// <param name="_datos">The _datos.</param>
        public void insertRows(string _tabla, DataTable _datos)
        {
            try
            {

                if (this.sqlConn.State != ConnectionState.Open)
                {
                    this.Connect();
                    this.sqlConn.Open();
                }
                SqlBulkCopy sbc;
                if (this.transaccion != null)
                    sbc = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.TableLock, this.transaccion);
                else
                    sbc = new SqlBulkCopy(sqlConn);
                sbc.DestinationTableName = _tabla;
                sbc.WriteToServer(_datos);
                if (this.transaccion == null)
                {
                    this.adapter.Dispose();
                    this.sqlConn.Close();
                }
            }
            catch (Exception e)
            {
                LogErrores.tareas.Add("Ha ocurrido un error al intentar ejecutar la consulta: " + e.Message);
                LogErrores.write();

            }
            finally
            {
                if (this.transaccion == null)
                    sqlConn.Dispose();
            }

        }
        /// <summary>
        /// Datas the rowto parameters proc.
        /// </summary>
        /// <param name="_datos">The _datos.</param>
        public void dataRowtoParametersProc(DataRow _datos)
        {
            try
            {
                sqlParameters = new List<SqlParameter>();
                
                int i = 0;
                foreach (var r in _datos.ItemArray)
                { 
                    SqlParameter p = new SqlParameter("@" + _datos.Table.Columns[i].ColumnName,r);
                   
                    sqlParameters.Add(p);
                    i = i+1;
                }


            }
            catch
            { 
            
            }

            
        }
        /// <summary>
        /// Datas the rowto parameters proc.
        /// </summary>
        /// <param name="_datos">The _datos.</param>
        public void dataRowtoParametersProc(Dictionary<string,object> _datos)
        {
            try
            {
                sqlParameters = new List<SqlParameter>();

                int i = 0;
                foreach (var r in _datos)
                {
                    try
                    {
                        SqlParameter p = new SqlParameter("@" + r.Key, Convert.ChangeType(r.Value, r.Value.GetType()));
                        sqlParameters.Add(p);
                    }
                    catch (Exception e)
                    { 
                        
                    }

                    
                    i = i + 1;
                }


            }
            catch
            {

            }


        }
        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <returns></returns>
        public DataSet getDataSet()
        {
            return ds;
        }
        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <returns></returns>
        public DataTable getDataTable() {
            return ds.Tables[0];
        }


        /// <summary>
        /// Converts to list model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns></returns>
        public List<TModel> ConvertToListModel<TModel>()
        {
            List<TModel> mo = new List<TModel>();
            Type tipo = typeof(TModel);
            var atributos = tipo.GetProperties();

            foreach (DataRow filas in ds.Tables[0].Rows)
            {
                TModel objeto = (TModel)Activator.CreateInstance(typeof(TModel));

                foreach (System.Reflection.PropertyInfo campo in atributos)
                    try
                    {
                        tipo.GetProperty(campo.Name).SetValue((TModel)objeto, filas[campo.Name]);
                    }
                    catch
                    {

                    }

                mo.Add((TModel)objeto);
            }
            return mo;
        }
        /// <summary>
        /// Converts to model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns></returns>
        public TModel ConvertToModel<TModel>()
        {
            List<TModel> mo = new List<TModel>();
            Type tipo = typeof(TModel);
            var atributos = tipo.GetProperties();

            foreach (DataRow filas in ds.Tables[0].Rows)
            {
                TModel objeto = (TModel)Activator.CreateInstance(typeof(TModel));

                foreach (System.Reflection.PropertyInfo campo in atributos)
                    try
                    {
                        tipo.GetProperty(campo.Name).SetValue((TModel)objeto, filas[campo.Name]);
                    }
                    catch
                    {

                    }

                mo.Add((TModel)objeto);
            }

            return mo.FirstOrDefault();

        }
        /// <summary>
        /// Converts to model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="modelo">The modelo.</param>
        public void ConvertToModel<TModel>(out TModel modelo)
        {
            List<TModel> mo = new List<TModel>();
            Type tipo = typeof(TModel);
            var atributos = tipo.GetProperties();

            foreach (DataRow filas in ds.Tables[0].Rows)
            {
                TModel objeto = (TModel)Activator.CreateInstance(typeof(TModel));

                foreach (System.Reflection.PropertyInfo campo in atributos)
                    try
                    {
                        tipo.GetProperty(campo.Name).SetValue((TModel)objeto, filas[campo.Name]);
                    }
                    catch
                    {

                    }

                mo.Add((TModel)objeto);
            }

            modelo = mo.FirstOrDefault();

        }
        /// <summary>
        /// Models to procedure.
        /// </summary>
        /// <param name="Model">The model.</param>
        /// <param name="SQLParameter">The SQL parameter.</param>
        /// <returns></returns>
        public bool ModelToProcedure(object Model, string SQLParameter)
        {
            bool respuesta = false;
            var atributos = Model.GetType().GetProperties();
            List<SqlParameter> parametros = new List<SqlParameter>();
            foreach (System.Reflection.PropertyInfo campo in atributos)
                try
                {
                    
                    parametros.Add(new SqlParameter("@pm"+campo.Name,Model.GetType().GetProperty(campo.Name).GetValue(Model)));
                    addParametersProc(parametros);
                    ejecutarProcedimiento(SQLParameter);
                    respuesta = true;
                }
                catch
                {

                }
            return respuesta;
            
        }
        /// <summary>
        /// Inserts the model.
        /// </summary>
        /// <param name="Model">The model.</param>
        /// <returns></returns>
        public bool insertModel(object Model)
        { 

            bool respuesta = false;
            var atributos = Model.GetType().GetProperties();
            List<SqlParameter> parametros = new List<SqlParameter>();
            foreach (System.Reflection.PropertyInfo campo in atributos)
                try
                {
                    qryValues.Add("@pm" + campo.Name, Model.GetType().GetProperty(campo.Name).GetValue(Model));
                    addParametersProc(parametros);
                    insert();
                    ejecutarQuery();
                    respuesta = true;
                }
                catch
                {

                }
            return respuesta;
        }

        /// <summary>
        /// Gets the fecha server.
        /// </summary>
        /// <returns></returns>
        public DateTime getFechaServer()
        {
            this.sqlQuery = "select GETDATE() as FechaServer";
            this.ejecutarQuery();
            return this.ds.Tables[0].Rows[0].Field<DateTime>("FechaServer");
        }

        /// <summary>
        /// Get current user ip address.
        /// </summary>
        /// <returns>The IP Address</returns>
        public static string GetUserIPAddress()
        {
            var context = System.Web.HttpContext.Current;
            string ip = String.Empty;

            if (context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            else if (!String.IsNullOrWhiteSpace(context.Request.UserHostAddress))
                ip = context.Request.UserHostAddress;

            if (ip == "::1")
                ip = "127.0.0.1";

            return ip;
        }


        #endregion otrasFunciones

        #region Configuracion

        /// <summary>
        /// Sets the connection.
        /// </summary>
        /// <param name="_connectionStringName">Name of the _connection string.</param>
        public void setConnection(string _connectionStringName)
        {
            strName = _connectionStringName;
            Connect();
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        private void Connect() {
            this.sqlConn = new SqlConnection();
            this.sqlConn.ConnectionString = ConfigurationManager.ConnectionStrings[this.strName].ConnectionString;
            this.ds = new DataSet();
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.ConnectionString = this.sqlConn.ConnectionString;
            this.DataBase = builder.InitialCatalog;
            this.Server = builder.DataSource;
            this.User = builder.UserID;
            this.Pass = builder.Password;
        }

        /// <summary>
        /// Begins the tran.
        /// </summary>
        public void beginTran()
        {
            Connect();
            this.sqlConn.Open();
            this.transaccion = this.sqlConn.BeginTransaction();
        }

        /// <summary>
        /// Commits the tran.
        /// </summary>
        public void commitTran()
        {
            this.transaccion.Commit();
           // this.adapter.Dispose();
            this.sqlConn.Close();
        }

        /// <summary>
        /// Rollbacks this instance.
        /// </summary>
        public void rollback()
        {
            this.transaccion.Rollback();
//            this.adapter.Dispose();
            this.sqlConn.Close();
        }

        #endregion Configuracion


        public ConnectionState abrirConexion(bool isTransact = false)
        {
            try
            {
                if (sqlConn != null)
                {
                    sqlConn.Open();
                    if (isTransact)
                        transaccion = sqlConn.BeginTransaction();
                }
                else
                    sqlConn = new SqlConnection();
            }
            catch (Exception ex)
            {
                var str = ex.Message;
            }
            return sqlConn.State;
        }

        /// <summary>
        /// Procedimientos Almacenados.
        /// </summary>
        /// <param name="SqlQuery">metodo acceder.</param>
        /// <param name="_parametros">parametros en la consulta.</param>
        /// <param name="_Datos">dataset.</param>
        /// <param name="tipo">The tipo.</param>
        /// <returns></returns>
        public bool ejecutarQuery(string SqlQuery, List<SqlParameter> _parametros, out DataSet _Datos, out string[] mensaje, CommandType tipo = CommandType.Text)
        {

            bool resultado = false;
            _Datos = new DataSet();
            mensaje = new string[2];
            try
            {
                if (sqlConn != null)
                {
                    try
                    {
                        if (this.sqlConn.State != ConnectionState.Open)
                            abrirConexion();

                        SqlDataAdapter adapter = new SqlDataAdapter(SqlQuery, this.sqlConn);
                        adapter.SelectCommand.CommandType = tipo;
                        adapter.SelectCommand.Parameters.Clear();
                        adapter.SelectCommand.CommandTimeout = 9999999;
                        _Datos.Clear();
                        adapter.SelectCommand.Parameters.AddRange(_parametros.ToArray());
                        adapter.Fill(_Datos);

                        resultado = true;
                    }
                    catch (Exception ex)
                    {                        
                        mensaje[0] = "011";
                        mensaje[1] = "Error al ejecutar la consulta" + SqlQuery + " ha ocurrido  " + ex.Message + "]";
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resultado;
        }

        /// <summary>
        /// Cerrar conexion.
        /// </summary>
        public void Close()
        {
            this.sqlConn.Close();
        }
    }
}