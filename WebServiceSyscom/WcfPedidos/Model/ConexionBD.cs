using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Web;
using System.Data.SqlClient;

namespace WcfPedidos.Model
{
    public class ConexionBD
    {
        private List<string> msjError = new List<string>();
        private SqlConnectionStringBuilder conBuilder = null;
        private SqlConnection sqlConn = null;
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
        public enum Comprobantes { Comprobante, Niff, Fiscal };
        public enum operacion { insert, update, delete, anular, cancelar, import, open, close, cumplir, descumplir };
        public Dictionary<string, object> condicionupdate = new Dictionary<string, object>();

        public List<SqlParameter> sqlp = new List<SqlParameter>();

        public List<string> qryTables = new List<string>();
        public List<string> qryFields = new List<string>();
        public List<string> qryGroupBy = new List<string>();
        public List<string> qryOrderBy = new List<string>();
        public Dictionary<string, object> qryValues = new Dictionary<string, object>();
        public List<string> Errores = new List<string>();
        private SqlDataAdapter adapter;
        private ConexionSQLite conSqlite = new ConexionSQLite("");
       


        public ConexionBD()
        {
            this.conBuilder = conSqlite.obtenerConexionSyscom();
        }

        public ConexionBD(SqlConnectionStringBuilder stringConn)
        {
            stringConn.ConnectTimeout = 0;
            sqlConn = new SqlConnection(stringConn.ConnectionString);
        }


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


        public bool ejecutarQuerys(string SqlQuery, List<SqlParameter> _parametros, out DataTable _Datos, CommandType tipo = CommandType.Text)
        {
            //MessageBox.Show(SqlQuery);
            bool resultado = false;
            _Datos = new DataTable();
            try
            {
                if (sqlConn != null)
                {
                    try
                    {
                        if (this.sqlConn.State != ConnectionState.Open)
                            abrirConexion();
                        SqlCommand command = new SqlCommand(SqlQuery, sqlConn);
                        if (this.transaccion != null)
                            command.Transaction = this.transaccion;
                        command.CommandTimeout = 0;
                        command.CommandType = tipo;
                        command.Parameters.AddRange(_parametros.ToArray());
                        _Datos.Load(command.ExecuteReader());
                        _parametros = new List<SqlParameter>();
                        resultado = true;
                    }
                    catch (Exception ex)
                    {
                        Errores.Add(ex.Message);
                        //Log.tareas.Add("Error al ejecutar la consulta a la base de datos" + SqlQuery + " [mensaje: " + ex.Message + "]");
                        //Log.write();
                    }
                }
            }
            catch (Exception ex)
            {
             //   LogErrores.escribirError(ex);
              //  LogErrores.write();
            }
            return resultado;
        }

        public bool ejecutarQuerys(string SqlQuery, List<SqlParameter> _parametros, out DataSet _Datos, CommandType tipo = CommandType.Text)
        {
            bool resultado = false;
            _Datos = new DataSet();
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
                        Errores.Add(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Errores.Add(ex.Message);
            }
            return resultado;
        }

        #region Tabla
        public void ejecutarQuery(string _resultado = "select", string _aliasConsulta = null, bool _parametros = false)
        {
            try
            {
                if (this.sqlConn.State != ConnectionState.Open)
                {
                    this.Connect();
                    this.sqlConn.Open();
                }
                this.adapter = new SqlDataAdapter(sqlQuery, this.sqlConn);
                //this.Log(sqlQuery);
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
                        this.adapter.UpdateCommand = new SqlCommand(sqlQuery, this.sqlConn);
                        this.adapter.UpdateCommand.CommandType = CommandType.Text;
                        this.adapter.UpdateCommand.Parameters.Clear();
                        if (_parametros)
                            this.adapter.UpdateCommand.Parameters.AddRange(this.sqlp.ToArray());
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
                        //this.adapter.SelectCommand.ExecuteNonQuery();
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
            catch (SqlException ex)
            {
                if (this.transaccion != null)
                    this.transaccion.Rollback();

                Log(ex.Message);
                throw new FaultException(ex.Message);

                //Errores.Add(ex.Message.Split(',')[1]);
            }
            finally
            {
                if (this.transaccion == null)
                    sqlConn.Dispose();
            }
        }

        public void Log(string logMessage)
        {
            StreamWriter sw = new StreamWriter(String.Concat(System.AppDomain.CurrentDomain.BaseDirectory, "\\LogErroresConex", ".txt"), true);
            //string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\LogPuebasI.txt";
            //if (!File.Exists(path))
            //{
            //    // Create a file to write to.
            //    using (sw = File.CreateText(path))
            //    {
            sw.WriteLine(String.Concat("Fecha ", DateTime.Now.ToString("yyyyMMddHHmmss"), ": ", logMessage));
            sw.WriteLine(Environment.NewLine);
            sw.Flush();
            sw.Close();
            //    }

            //}
            //else
            //{
            //    sw.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(logMessage));
            //    sw.WriteLine(Environment.NewLine);
            //    sw.Flush();
            //    sw.Close();
            //}

        }


        #region FuncionesTabla
        public void insert()
        {
            this.sqlQuery = "insert into " + String.Join("", qryTables.FirstOrDefault().ToString());
            this.sqlQuery = sqlQuery + " (" + String.Join(",", qryValues.Keys) + ")";
            this.sqlQuery = sqlQuery + " values ('" + String.Join("','", qryValues.Values) + "')";

        }

        public void update()
        {

            List<string> datos = new List<string>();
            List<string> condicion = new List<string>();

            foreach (var val in qryValues)
            {
                datos.Add(Convert.ToString(val.Key + " = @" + val.Key + ""));
                sqlp.Add(new SqlParameter("@" + val.Key, val.Value));
            }

            foreach (var cp in this.condicionupdate)
            {
                condicion.Add(Convert.ToString(cp.Key + " = @" + cp.Key + ""));
                sqlp.Add(new SqlParameter("@" + cp.Key, cp.Value));
            }

            this.sqlQuery = "update " + String.Join("", qryTables.FirstOrDefault().ToString());
            this.sqlQuery += " set " + String.Join(",", datos);
            if (condicion.Count() > 0)
                this.sqlQuery += " where ";
            this.sqlQuery += (condicion.Count() > 0 ? String.Join(" AND ", condicion) : "");
        }

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

        public void setCustomQuery(string _sqlQuery)
        {
            this.sqlQuery = _sqlQuery;
        }

        //public void AddParametros(string _field, string _value)
        //{
        //    this.qryInsValores.Add(_field, _value);
        //}

        public void addWhereAND(string _Where)
        {
            this.qryWhereAND.Add(_Where);
        }

        public void addWhereOR(string _Where)
        {
            this.qryWhereOR.Add(_Where);
        }
        #endregion

        #endregion

        #region Procedimiento

        public void addParametersProc(List<SqlParameter> _sqlParametros)
        {
            this.sqlParameters.AddRange(_sqlParametros);
        }

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

                if (_sinresultados != false)
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
            catch (SqlException ex)
            {
                if (this.transaccion != null)
                    this.transaccion.Rollback();

                Log(ex.Message);

                //Errores.Add(ex.Message.Split(',')[1]);
                throw new FaultException(ex.Message);
                // new Exception("Error al ejecutar el procedimiento: " + _procedimiento + );
            }
            finally
            {
                if (this.transaccion == null)
                    sqlConn.Dispose();
            }
        }

        public void ejecutarProcedimiento(string _procedimiento, operacion operac, string _aliasProcedimiento = null, bool _sinresultados = false)
        {

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                string opcion = "";
                switch (operac)
                {
                    case operacion.insert:
                        opcion = "i";//insertar
                        break;
                    case operacion.update:
                        opcion = "u";//actualizar
                        break;
                    case operacion.delete:
                        opcion = "d";//eliminar
                        break;
                    case operacion.anular:
                        opcion = "a";//anular
                        break;
                    case operacion.cancelar:
                        opcion = "s";//saldar
                        break;
                    case operacion.import:
                        opcion = "p";//importar
                        break;
                    case operacion.open:
                        opcion = "o";//abrir
                        break;
                    case operacion.close:
                        opcion = "c";//cerrar
                        break;
                    case operacion.cumplir:
                        opcion = "f";//fulfill(cumplir-satisfy)
                        break;
                    case operacion.descumplir:
                        opcion = "n";//notfulfill(descumplir-not satisfy)
                        break;
                }

                parametros.Add(new SqlParameter("@operacion", opcion));
                addParametersProc(parametros);

                if (this.sqlConn.State != ConnectionState.Open)
                {
                    this.Connect();
                    this.sqlConn.Open();
                }

                this.adapter = new SqlDataAdapter(_procedimiento, this.sqlConn);
                this.adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                this.adapter.SelectCommand.CommandTimeout = 0;
                this.adapter.SelectCommand.Parameters.Clear();
                this.ds.Clear();
                SqlParameter[] _sqlParameters = sqlParameters.ToArray();
                if (this.transaccion != null)
                    this.adapter.SelectCommand.Transaction = this.transaccion;
                foreach (var item in _sqlParameters)
                {
                    this.adapter.SelectCommand.Parameters.Add(item);
                }

                if (_sinresultados != false)
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
            catch (SqlException ex)
            {
                if (this.transaccion != null)
                    this.transaccion.Rollback();

                Log(ex.Message);

                //Errores.Add(ex.Message.Split(',')[1]);
                throw new FaultException(ex.Message);
                // new Exception("Error al ejecutar el procedimiento: " + _procedimiento + );
            }
            catch (Exception ex)
            {
                if (this.transaccion != null)
                    this.transaccion.Rollback();

                Log(ex.Message);

                //Errores.Add(ex.Message.Split(',')[1]);
                throw new FaultException(ex.Message);
                // new Exception("Error al ejecutar el procedimiento: " + _procedimiento + );
            }
            finally
            {
                if (this.transaccion == null)
                    sqlConn.Dispose();
            }
        }

        public void ejecutarProcedimiento(string _procedimiento, DataTable dt, operacion operac, string _aliasProcedimiento = null, bool _sinresultados = false)
        {

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                string opcion = "";
                switch (operac)
                {
                    case operacion.insert:
                        opcion = "i";//insertar
                        break;
                    case operacion.update:
                        opcion = "u";//actualizar
                        break;
                    case operacion.delete:
                        opcion = "d";//eliminar
                        break;
                    case operacion.anular:
                        opcion = "a";//anular
                        break;
                    case operacion.cancelar:
                        opcion = "s";//saldar
                        break;
                    case operacion.import:
                        opcion = "p";//importar
                        break;
                    case operacion.open:
                        opcion = "o";//abrir
                        break;
                    case operacion.close:
                        opcion = "c";//cerrar
                        break;
                    case operacion.cumplir:
                        opcion = "f";//fulfill(cumplir-satisfy)
                        break;
                    case operacion.descumplir:
                        opcion = "n";//notfulfill(descumplir-not satisfy)
                        break;
                }

                parametros.Add(new SqlParameter("@" + dt.TableName, dt));
                parametros.Add(new SqlParameter("@operacion", opcion));
                addParametersProc(parametros);

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

                if (_sinresultados != false)
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
            catch (SqlException ex)
            {
                if (this.transaccion != null)
                    this.transaccion.Rollback();

                Log(ex.Message);

                //Errores.Add(ex.Message.Split(',')[1]);
                throw new FaultException(ex.Message);
                // new Exception("Error al ejecutar el procedimiento: " + _procedimiento + );
            }
            finally
            {
                if (this.transaccion == null)
                    sqlConn.Dispose();
            }
        }

        public void ejecutarProcedimiento(string _procedimiento, List<DataTable> _tables, operacion operac, string _aliasProcedimiento = null, bool _sinresultados = false)
        {

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>();

                string opcion = "";
                switch (operac)
                {
                    case operacion.insert:
                        opcion = "i";//insertar
                        break;
                    case operacion.update:
                        opcion = "u";//actualizar
                        break;
                    case operacion.delete:
                        opcion = "d";//eliminar
                        break;
                    case operacion.anular:
                        opcion = "a";//anular
                        break;
                    case operacion.cancelar:
                        opcion = "s";//saldar
                        break;
                    case operacion.import:
                        opcion = "p";//importar
                        break;
                    case operacion.open:
                        opcion = "o";//abrir
                        break;
                    case operacion.close:
                        opcion = "c";//cerrar
                        break;
                    case operacion.cumplir:
                        opcion = "f";//fulfill(cumplir-satisfy)
                        break;
                    case operacion.descumplir:
                        opcion = "n";//notfulfill(descumplir-not satisfy)
                        break;
                }

                foreach (var dt in _tables)
                    parametros.Add(new SqlParameter("@" + dt.TableName, dt));

                parametros.Add(new SqlParameter("@operacion", opcion));
                addParametersProc(parametros);

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

                if (_sinresultados != false)
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
            catch (SqlException ex)
            {
                if (this.transaccion != null)
                    this.transaccion.Rollback();

                Log(ex.Message);

                throw new FaultException(ex.Message);
                // new Exception("Error al ejecutar el procedimiento: " + _procedimiento + );
            }
            finally
            {
                if (this.transaccion == null)
                    sqlConn.Dispose();
            }
        }

        #endregion

        #region otrasFunciones

        public int getNumFilas()
        {
            int filas = this.getDataTable().Rows.Count;
            return filas;
        }

        public void resetQuery()
        {
            this.qryTables.Clear();
            this.qryFields.Clear();
            this.qryWhereAND.Clear();
            this.qryWhereOR.Clear();
            this.qryGroupBy.Clear();
            this.qryOrderBy.Clear();
            this.sqlParameters = new List<SqlParameter>();
            this.resNumRows = 0;
        }

        public string getCampos()
        {
            return String.Join(",", qryFields);
        }

        public string insertDataTable(string _tabla, DataTable _datos)
        {
            try
            {
                Connect();
                this.sqlConn.Open();
                SqlBulkCopy sbc = new SqlBulkCopy(sqlConn);
                sbc.DestinationTableName = _tabla;
                sbc.WriteToServer(_datos);
                this.sqlConn.Close();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                sqlConn.Dispose();
            }

        }

        public void dataRowtoParametersProc(DataRowView _datos)
        {
            try
            {
                sqlParameters = new List<SqlParameter>();

                int i = 0;
                foreach (var r in _datos.Row.ItemArray)
                {
                    SqlParameter p = new SqlParameter("@" + _datos.Row.Table.Columns[i].ColumnName, r);

                    sqlParameters.Add(p);
                    i = i + 1;
                }


            }
            catch
            {

            }


        }

        public void dataRowtoParametersProc(Dictionary<string, object> _datos)
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
                       // LogErrores.escribirError(e);
                        //LogErrores.write();
                    }
                    i = i + 1;
                }


            }
            catch
            {

            }


        }

        public DataSet getDataSet()
        {
            return ds;
        }

        public DataTable getDataTable()
        {
            return ds.Tables[0];
        }

        /// <summary>
        /// Asigna los resultados a una lista de cualquier tipo de clase.
        /// </summary>
        /// <typeparam name="TModel">Tipo de la clase.</typeparam>
        /// <returns></returns>
        public List<TModel> ConvertToListModel<TModel>()
        {
            List<TModel> mo = new List<TModel>();
            Type tipo = typeof(TModel);
            var atributos = tipo.GetProperties();

            foreach (DataRow filas in ds.Tables[0].Rows)
            {
                TModel objeto = (TModel)Activator.CreateInstance(typeof(TModel));

                foreach (var campo in atributos)
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
        /// Asigna los resultados a una lista de cualquier tipo de clase.
        /// </summary>
        /// <typeparam name="TModel">Tipo de la clase.</typeparam>
        /// <returns></returns>
        public List<TModel> ConvertToListModel<TModel>(DataTable tabla)
        {
            List<TModel> mo = new List<TModel>();
            Type tipo = typeof(TModel);
            var atributos = tipo.GetProperties();

            foreach (DataRow filas in tabla.Rows)
            {
                TModel objeto = (TModel)Activator.CreateInstance(typeof(TModel));

                foreach (var campo in atributos)
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
        /// Asigna los resultados a un vector de cualquier tipo de clase.
        /// </summary>
        /// <typeparam name="TModel">Tipo de la clase.</typeparam>
        /// <returns></returns>
        public TModel[] ConvertToArrayModel<TModel>()
        {
            List<TModel> mo = new List<TModel>();
            Type tipo = typeof(TModel);
            var atributos = tipo.GetProperties();

            foreach (DataRow filas in ds.Tables[0].Rows)
            {
                TModel objeto = (TModel)Activator.CreateInstance(typeof(TModel));

                foreach (var campo in atributos)
                    try
                    {
                        tipo.GetProperty(campo.Name).SetValue((TModel)objeto, filas[campo.Name]);
                    }
                    catch
                    {

                    }

                mo.Add((TModel)objeto);
            }
            return mo.ToArray();
        }

        /// <summary>
        /// Asigna los resultados a una clase.
        /// </summary>
        /// <typeparam name="TModel">Tipo de la clase.</typeparam>
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
        /// Almacena un modelo en la tabla resultante de la consulta.
        /// </summary>
        /// <param name="clsModelos">The CLS modelos.</param>
        public void ModelToTable<TModel>(List<TModel> clsModelos)
        {
            foreach (TModel clsModelo in clsModelos)
            {
                Type tipo = typeof(TModel);
                DataRow dr = ds.Tables[0].NewRow();
                for (Int32 i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    try
                    {
                        string campo = ds.Tables[0].Columns[i].ColumnName;
                        dr[i] = tipo.GetProperty(campo).GetValue(clsModelo);
                        if (tipo.GetProperty(campo).GetValue(clsModelo).GetType().Name.ToLower() == "datetime" && (DateTime)tipo.GetProperty(campo).GetValue(clsModelo) == DateTime.MinValue)
                            dr[i] = DBNull.Value;
                    }
                    catch
                    {

                    }
                }
                ds.Tables[0].Rows.Add(dr);
                ds.Tables[0].AcceptChanges();
            }

            //insertDataTable(ds.Tables[0].TableName, ds.Tables[0]);
        }

        /// <summary>
        /// Models to table.
        /// </summary>
        /// <param name="clsModelo">The CLS modelo.</param>
        public void ModelToTable<TModel>(TModel clsModelo)
        {
            Type tipo = typeof(TModel);
            DataRow dr = ds.Tables[0].NewRow();
            for (Int32 i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                try
                {
                    string campo = ds.Tables[0].Columns[i].ColumnName;
                    dr[i] = tipo.GetProperty(campo).GetValue(clsModelo);
                    if (tipo.GetProperty(campo).GetValue(clsModelo) != null)

                        if (tipo.GetProperty(campo).GetValue(clsModelo).GetType().Name.ToLower() == "datetime" && (DateTime)tipo.GetProperty(campo).GetValue(clsModelo) == DateTime.MinValue)
                            dr[i] = DBNull.Value;
                }
                catch
                {

                }
            }
            ds.Tables[0].Rows.Add(dr);
            ds.Tables[0].AcceptChanges();

            //insertDataTable(ds.Tables[0].TableName, ds.Tables[0]);
        }

        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        public List<T> DataTableToList<T>() where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        public List<T> DataTableToList<T>(DataTable dtDatos) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (DataRow row in dtDatos.Rows)
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        public List<T> DataTableToList<T>(string[] DatosDt = null) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();
                DataTable dt = DatosDt == null ? ds.Tables[0] : ds.Tables[0].DefaultView.ToTable(true, DatosDt);
                foreach (DataRow row in dt.Rows)
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

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

        public void ConvertToModel<TModel>(object Model, out TModel modelo)
        {
            List<TModel> mo = new List<TModel>();
            Type tipo = typeof(TModel);
            var atributos = tipo.GetProperties();

            TModel objeto = (TModel)Activator.CreateInstance(typeof(TModel));

            foreach (System.Reflection.PropertyInfo campo in atributos)
            {
                try
                {
                    tipo.GetProperty(campo.Name).SetValue((TModel)objeto, Model.GetType().GetProperty(campo.Name).GetValue(Model));
                }
                catch
                {

                }
            }

            mo.Add((TModel)objeto);

            modelo = mo.FirstOrDefault();

        }

        public void ConvertToListaModel<TModel>(List<object> Model, out List<TModel> modelo)
        {
            List<TModel> mo = new List<TModel>();

            foreach (TModel clsModelo in Model)
            {
                Type tipo = typeof(TModel);
                var atributos = tipo.GetProperties();
                TModel objeto = (TModel)Activator.CreateInstance(typeof(TModel));

                foreach (System.Reflection.PropertyInfo campo in atributos)
                {
                    try
                    {
                        tipo.GetProperty(campo.Name).SetValue((TModel)objeto, clsModelo.GetType().GetProperty(campo.Name).GetValue(clsModelo));
                    }
                    catch
                    {

                    }
                }
                mo.Add((TModel)objeto);
            }

            modelo = mo;

        }

        public bool ModelToProcedure(object Model, string SQLParameter)
        {
            bool respuesta = false;
            var atributos = Model.GetType().GetProperties();
            List<SqlParameter> parametros = new List<SqlParameter>();
            foreach (System.Reflection.PropertyInfo campo in atributos)
                try
                {

                    parametros.Add(new SqlParameter("@pm" + campo.Name, Model.GetType().GetProperty(campo.Name).GetValue(Model)));
                    addParametersProc(parametros);
                    ejecutarProcedimiento(SQLParameter);
                    respuesta = true;
                }
                catch
                {

                }
            return respuesta;

        }

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

        public void DeleteSpace<TModel>(List<TModel> clsModelos)
        {
            foreach (TModel clsModelo in clsModelos)
            {
                Type tipo = typeof(TModel);

                foreach (var prop in clsModelo.GetType().GetProperties())
                {
                    try
                    {
                        string campo = prop.Name;

                        if (tipo.GetProperty(campo).GetValue(clsModelo).GetType().Name.ToLower().Equals("string"))
                        {
                            if ((tipo.GetProperty(campo).GetValue(clsModelo).ToString()) != null)
                                tipo.GetProperty(campo).SetValue(clsModelo, tipo.GetProperty(campo).GetValue(clsModelo).ToString().Trim());
                        }
                    }
                    catch
                    {

                    }
                }
            }

        }

        public static byte[] DataTableToArray(DataTable dataTable)
        {
            byte[] binaryDataResult = null;
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter brFormatter = new BinaryFormatter();
                dataTable.RemotingFormat = SerializationFormat.Binary;
                brFormatter.Serialize(memStream, dataTable);
                binaryDataResult = memStream.ToArray();
            }
            return binaryDataResult;
        }

        public static DataTable ByteArrayToDataTable(string contenido)
        {
            DataTable dt = null;
            // Deserializing into datatable    
            try
            {
                byte[] byteArrayData = Convert.FromBase64String(contenido);
                using (MemoryStream stream = new MemoryStream(byteArrayData))
                {
                    BinaryFormatter bformatter = new BinaryFormatter();
                    dt = (DataTable)bformatter.Deserialize(stream);
                }
            }
            catch
            {

            }
            return dt;
        }

        // function that creates an object from the given data row
        public TModel ConvertRowToModel<TModel>() where TModel : new()
        {
            // create a new object
            TModel item = new TModel();

            // set the item
            SetItemFromRow(item, this.ds.Tables[0].Rows[0]);

            // return 
            return item;
        }

        public void SetItemFromRow<TModel>(TModel item, DataRow row) where TModel : new()
        {
            // go through each column
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName);

                // if exists, set the value
                if (p != null && row[c] != DBNull.Value)
                {
                    p.SetValue(item, row[c], null);
                }
            }
        }

        #endregion

        #region Configuracion

        public void setConnection(SqlConnectionStringBuilder con)
        {
            conBuilder = con;
            Connect();
        }

        private void Connect()
        {
            this.sqlConn = new SqlConnection();
            // this.sqlConn.ConnectionString = ConfigurationManager.ConnectionStrings[this.strName].ConnectionString;
            //System.Data.SqlClient.SqlConnectionStringBuilder builderSqlite = conSqlite.obtenerConexionSQLServer(strName);
            this.sqlConn.ConnectionString = conBuilder.ConnectionString;
            this.sqlConn.ConnectionTimeout.Equals(0);
            this.ds = new DataSet();
            //System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            //builder.ConnectionString = this.sqlConn.ConnectionString;
            this.DataBase = conBuilder.InitialCatalog;
            this.Server = conBuilder.DataSource;
            this.User = conBuilder.UserID;
            this.Pass = conBuilder.Password;
        }

        public void beginTran()
        {
            Connect();
            this.sqlConn.Open();
            this.transaccion = this.sqlConn.BeginTransaction();
        }

        public void commitTran()
        {
            if (this.transaccion != null)
            {
                if (this.transaccion.Connection.State == ConnectionState.Open)
                    this.transaccion.Commit();
            }
            // this.adapter.Dispose();
            this.sqlConn.Close();
        }

        public void rollback()
        {
            if (this.transaccion != null)
            {
                if (this.transaccion.Connection.State == ConnectionState.Open)
                    this.transaccion.Rollback();
            }
            //            this.adapter.Dispose();
            this.sqlConn.Close();
        }

        #endregion


        internal void addParametersProc(SqlParameter _parametros)
        {
            throw new NotImplementedException();
        }

        //internal Models.Vehiculo ModelToTable<T1>(string[] p)
        //{
        //    throw new NotImplementedException();
        //}

        internal object ModelToTable<T1>(DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        public DataTable transformar(List<Dictionary<string, List<Dictionary<string, object>>>> modelo)
        {
            DataTable dtConf = new DataTable();
            DbDataReader reader;
            try
            {
                //SqlCommand comm = new SqlCommand(procedure, this.sqlConn);
                //comm.CommandType = CommandType.StoredProcedure;
                if (modelo != null)
                {
                    foreach (var tablas in modelo)
                    {
                        foreach (var i in tablas)
                        {
                            dtConf = new DataTable(i.Key);

                            DbProviderFactory dpf = DbProviderFactories.GetFactory(this.sqlConn);
                            DbCommand cmd = dpf.CreateCommand();

                            //Verifica que la conexion se encuentre cerrada.
                            if (this.sqlConn.State != ConnectionState.Open)
                            {
                                this.Connect();
                                this.sqlConn.Open();
                            }

                            //Asigna la conexion al DBCommand creado anteriormente.
                            cmd.Connection = this.sqlConn;
                            cmd.CommandText = "paSwQryCamposTDatos";
                            cmd.CommandType = CommandType.StoredProcedure;

                            DbParameter param = dpf.CreateParameter();
                            param.Value = i.Key;
                            param.ParameterName = "tipoDato";
                            cmd.Parameters.Add(param);

                            if (this.transaccion != null)
                                cmd.Transaction = this.transaccion;
                            //this.sqlConn.Open();
                            reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                #region
                                switch (Convert.ToString(reader["DATA_TYPE"]))
                                {
                                    case "int":
                                        dtConf.Columns.Add(new DataColumn(Convert.ToString(reader["COLUMN_NAME"]), typeof(SqlInt32)));
                                        break;
                                    case "bigint":
                                        dtConf.Columns.Add(new DataColumn(Convert.ToString(reader["COLUMN_NAME"]), typeof(SqlInt32)));
                                        break;
                                    case "money":
                                        dtConf.Columns.Add(new DataColumn(Convert.ToString(reader["COLUMN_NAME"]), typeof(SqlMoney)));
                                        break;
                                    case "decimal":
                                        dtConf.Columns.Add(new DataColumn(Convert.ToString(reader["COLUMN_NAME"]), typeof(SqlDecimal)));
                                        break;
                                    case "date":
                                        dtConf.Columns.Add(new DataColumn(Convert.ToString(reader["COLUMN_NAME"]), typeof(SqlDateTime)));
                                        break;
                                    case "datetime":
                                        dtConf.Columns.Add(new DataColumn(Convert.ToString(reader["COLUMN_NAME"]), typeof(SqlDateTime)));
                                        break;
                                    case "smalldatetime":
                                        dtConf.Columns.Add(new DataColumn(Convert.ToString(reader["COLUMN_NAME"]), typeof(SqlDateTime)));
                                        break;
                                    case "bit":
                                        dtConf.Columns.Add(new DataColumn(Convert.ToString(reader["COLUMN_NAME"]), typeof(SqlBoolean)));
                                        break;
                                    case "float":
                                        dtConf.Columns.Add(new DataColumn(Convert.ToString(reader["COLUMN_NAME"]), typeof(SqlDecimal)));
                                        break;
                                    default:
                                        dtConf.Columns.Add(new DataColumn(Convert.ToString(reader["COLUMN_NAME"]), typeof(SqlString)));
                                        break;
                                }
                                #endregion
                            }

                            foreach (var j in tablas.Keys)
                            {
                                foreach (var valores in tablas[j])
                                {

                                    DataRow drConfRow = dtConf.NewRow();

                                    foreach (var a in valores)
                                    {
                                        if (a.Value != null)
                                            drConfRow[Convert.ToString(a.Key)] = a.Value;
                                        else
                                            drConfRow[Convert.ToString(a.Key)] = DBNull.Value;
                                    }

                                    dtConf.Rows.Add(drConfRow);
                                }
                            }
                        }
                    }
                }

                return dtConf;
            }
            catch
            {
                return dtConf;
            }
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
                        msjError.Add(ex.Message);
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
    }
}