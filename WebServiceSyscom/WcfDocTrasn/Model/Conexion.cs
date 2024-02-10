using SyscomUtilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WcfDocTrasn.Model
{
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
        public Conexion()
        {
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
            //LogErrores.tareas.Add(this.sqlQuery);
            //LogErrores.write();
        }

        /// <summary>
        /// Ejecuta una consulta sql directamente desde la funcion.
        /// </summary>
        /// <param name="_sqlQuery">Sentencia SQL.</param>
        public void setCustomQuery(string _sqlQuery)
        {
            this.sqlQuery = _sqlQuery;
        }


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
        /// Funciona para combinar 2 objetos DataTable en uno solo aplicando operacion de union
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <param name="joinOn">The join on.</param>
        /// <returns></returns>
        public DataTable JoinDataTables(DataTable t1, DataTable t2, params Func<DataRow, DataRow, bool>[] joinOn)
        {
            //Se crea un nuevo objeto de DataTable que almacena los resultados de la union
            DataTable result = new DataTable();
            // Se recorren las dos columnas que recibimos mediante parametros
            foreach (DataColumn col in t1.Columns)
            {
                //Condicional por si una columna no existe en el result se agrega con el mismo nombre y tipo de dato
                if (result.Columns[col.ColumnName] == null)
                    result.Columns.Add(col.ColumnName, col.DataType);
            }
            foreach (DataColumn col in t2.Columns)
            {
                if (result.Columns[col.ColumnName] == null)
                    result.Columns.Add(col.ColumnName, col.DataType);
            }
            //Se iteran todas las filas de t1
            foreach (DataRow row1 in t1.Rows)
            {
                //Para cada fila de t1 , se busca en t2 las filas que cumplan las condiciones de union especificadas por el join on
                var joinRows = t2.AsEnumerable().Where(row2 =>
                {
                    // si se encuentra una coincidencia , se crea una nueva fila en result que combina ambas tablas
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
            //y al final retorna el objeto result que contiene la tabla resultante después de la unión.
            return result;
        }

        /// <summary>
        /// Obtiene el numero de filas de un DataTable
        /// </summary>
        /// <returns>Numero de filas</returns>
        public int getNumFilas()
        {
            //En la variable filas se accede a la propiedad Rows del DataTable y mediante Count se obtiene el numero de filas de esta
            int filas = this.getDataTable().Rows.Count;
            return filas;
        }

        /// <summary>
        ///  Restablece los valores relacionados con una consulta 
        /// </summary>
        public void resetQuery()
        {
            //Limpia la coleccion de tablas obtenidas en la consulta
            this.qryTables.Clear();

            //Limpia la coleccion de columnas obtenidas 
            this.qryFields.Clear();

            //Limpia la coleccion de condiciones
            this.qryWhereAND.Clear();

            //Limpia la coleccion de condiciones
            this.qryWhereOR.Clear();

            //Limpia la coleccion de GroupBy
            this.qryGroupBy.Clear();

            //Limpia la coleccion de OrderBy
            this.qryOrderBy.Clear();

            //Limpia la coleccion de tablas
            this.ds.Tables.Clear();

            //Limpia la coleccion de parametros
            this.sqlParameters = new List<SqlParameter>();

            //Establece el numero de filas resultadas a cero 
            this.resNumRows = 0;
        }

        /// <summary>
        /// Devuelve una cadena de strings de las columnas almacenadas en una colección llamada qryFields
        /// </summary>
        /// <returns></returns>
        public string getCampos()
        {
            //Se utiliza el metodo join de la clase String para combinar los elementos de qryFields
            //Se retorna como resultado un string que contiene los nombres de las columnas
            return String.Join(",", qryFields);
        }

        /// <summary>
        /// Inserta datos desde un datatable a una tabla en una base de datos utilizando la carga masiva
        /// </summary>
        /// <param name="_tabla">The _tabla.</param>
        /// <param name="_datos">The _datos.</param>
        public bool insertDataTable(string _tabla, DataTable _datos)
        {
            bool insertado = true;
            try
            {
                //verifica que la conexion a la base de datos esta abierta , si no la realiza
                if (this.sqlConn.State != ConnectionState.Open)
                {
                    this.Connect();
                    this.sqlConn.Open();
                }
                //Se crea un objeto SqlBulkCopy  para la carga masiva de datos.
                SqlBulkCopy sbc;
                if (this.transaccion != null)
                    sbc = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.Default, this.transaccion);
                else
                    sbc = new SqlBulkCopy(sqlConn);

                //Se establece el nombre de la tabla destino de la base de datos
                sbc.DestinationTableName = _tabla;

                //Se copian los datos del DataTable a la base de datos
                sbc.WriteToServer(_datos);

                //Si no hay una trasaccion activa se liberan los recursos y se cierra la conexion
                if (this.transaccion == null)
                {
                    this.adapter.Dispose();
                    this.sqlConn.Close();
                }
            }

            //En caso de excepcion se captura la excepción, se registra en un registro de errores y se establece insertado como false.
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
        /// Inserta filas con carga masiva a una base de datos 
        /// </summary>
        /// <param name="_tabla">The _tabla.</param>
        /// <param name="_datos">The _datos.</param>
        public void insertRows(string _tabla, DataTable _datos)
        {
            try
            {
                //Verifica que la conexion este abierta y si no es asi este la abre
                if (this.sqlConn.State != ConnectionState.Open)
                {
                    this.Connect();
                    this.sqlConn.Open();
                }
                //Crea el objeto SqlBulkCopy para la carga masiva 
                SqlBulkCopy sbc;
                if (this.transaccion != null)
                    sbc = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.TableLock, this.transaccion);
                else
                    sbc = new SqlBulkCopy(sqlConn);

                //Proporciona el nombre de la tabla destino a donde los datos se van
                sbc.DestinationTableName = _tabla;

                //Copia los datos que contiene a la base de datos
                sbc.WriteToServer(_datos);

                //Verfica que no haya trasaccion alguna para cerrarla
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

            }
            finally
            {
                if (this.transaccion == null)
                    sqlConn.Dispose();
            }

        }


        /// <summary>
        /// Convierte los datos de un objeto datarow  en una lista de parametros 
        /// </summary>
        /// <param name="_datos">The _datos.</param>
        public void dataRowtoParametersProc(DataRow _datos)
        {
            try
            {
                //Se crea una nueva lista de parametros
                sqlParameters = new List<SqlParameter>();

                //Inicializa la variable de iteracion
                int i = 0;

                //Itera los datos del objeto ItemArray
                foreach (var r in _datos.ItemArray)
                {
                    //El nombre del parametro se crea concadenando con el nombre de la columna correspondiente de cada tabla
                    SqlParameter p = new SqlParameter("@" + _datos.Table.Columns[i].ColumnName, r);

                    //El parametro recien creado se añade a la lista de parametros 
                    //Por cada iteracion se va sumando el indice de iteracion
                    sqlParameters.Add(p);
                    i = i + 1;
                }


            }
            catch
            {

            }


        }


        /// <summary>
        /// Convierte los datos de un diccionario en una lista de parametros
        /// </summary>
        /// <param name="_datos">The _datos.</param>
        public void dataRowtoParametersProc(Dictionary<string, object> _datos)
        {
            try
            {
                //Se crea una nueva lista de parametros 
                sqlParameters = new List<SqlParameter>();

                //Se inicializa la variale i para ir guardando el numero de iteraciones dadas
                int i = 0;

                //Se itera el item datos 
                foreach (var r in _datos)
                {
                    try

                    {
                        ////El nombre del parametro se crea concadenando con el nombre de la columna correspondiente de cada tabla
                        ///El valor del dato se covierte al tipo de dato correspondiente
                        SqlParameter p = new SqlParameter("@" + r.Key, Convert.ChangeType(r.Value, r.Value.GetType()));

                        //Por cada parametro creado se añade a la lista de parametros 
                        sqlParameters.Add(p);
                    }
                    catch (Exception e)
                    {

                    }

                    //Por cada finalizacion se va incrementando la variable i
                    i = i + 1;
                }


            }
            catch
            {

            }


        }


        /// <summary>
        /// Devuelve un objeto data set
        /// </summary>
        /// <returns></returns>
        public DataSet getDataSet()
        {
            return ds;
        }


        /// <summary>
        ///Devuelve un objeto DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable getDataTable()
        {
            return ds.Tables[0];
        }

        /// <summary>
        ///Abre la conexion  a la base de datos
        /// </summary>
        /// <returns></returns>
        public ConnectionState abrirConexion(bool isTransact = false)
        {
            try
            {
                //Verifica si la conexion sql es diferente de nula para abrir la conexion
                //Si se especifica el parametro isTrasnsact se inicia una trasaccion despues de la conexion.
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
        ///Sirve para ejecutar los procedimientos de almacenado
        /// </summary>
        /// <returns></returns>
        public bool ejecutarQuery(string SqlQuery, List<SqlParameter> _parametros, out DataSet _Datos, out string[] mensaje, CommandType tipo = CommandType.Text)
        {
            // Se declara resultado como valor false por defecto
            bool resultado = false;
            // Se declara datos como un DataSet vacío
            _Datos = new DataSet();
            // Se declara mensje como un lista de string vacío
            mensaje = new string[2];
            try
            {
                // Condición que verifica si la conexión con la base de datos es nula
                if (sqlConn != null)
                {
                    try
                    {
                        // Si la conexión SQL no está abierta, se abre la conexión
                        if (this.sqlConn.State != ConnectionState.Open)
                            abrirConexion();

                        // Se crea un nuevo adaptador de datos SQL con la consulta SQL y la conexión SQL
                        SqlDataAdapter adapter = new SqlDataAdapter(SqlQuery, this.sqlConn);

                        // Se establece el tipo de comando para el comando de selección del adaptador
                        adapter.SelectCommand.CommandType = tipo;

                        // Se limpian los parámetros del comando de selección
                        adapter.SelectCommand.Parameters.Clear();

                        // Se establece el tiempo de espera del comando a un valor muy alto
                        adapter.SelectCommand.CommandTimeout = 9999999;

                        // Se limpia el conjunto de datos
                        _Datos.Clear();

                        // Se añaden los parámetros al comando de selección
                        adapter.SelectCommand.Parameters.AddRange(_parametros.ToArray());

                        // Se llena el conjunto de datos con los resultados de la consulta
                        adapter.Fill(_Datos);

                        // Si todo va bien, se establece el resultado a verdadero
                        resultado = true;
                    }
                    catch (Exception ex)
                    {
                        // Si ocurre una excepción, se establece el primer elemento del mensaje a "011"
                        mensaje[0] = "011";

                        // Se establece el segundo elemento del mensaje a un mensaje de error que incluye la consulta SQL y el mensaje de la excepción
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
        /// Convierte un data set en una lista de objetos de modelo
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns></returns>
        public List<TModel> ConvertToListModel<TModel>()
        {
            //Se crea una nueva lista de tipoTModel
            List<TModel> mo = new List<TModel>();

            // Se obtiene el tipo de TModel y sus propiedades mediante reflexión.
            Type tipo = typeof(TModel);
            var atributos = tipo.GetProperties();

            //Se iteran las filas del data set y por cada iteracion se crea una instancia objeto Tmodel
            foreach (DataRow filas in ds.Tables[0].Rows)
            {
                //Para cada propiedad en TModel, se intenta asignar el valor correspondiente de la fila actual.
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
        /// Convierte un data set en una lista de objetos de modelo
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns></returns>
        public TModel ConvertToModel<TModel>(DataSet ds)
        {
            // Se crea una lista de tipo TModel
            List<TModel> mo = new List<TModel>();

            //Se obtiene el tipo Tmodel mediante reflexion
            Type tipo = typeof(TModel);
            var atributos = tipo.GetProperties();

            //Se recorre el data set y se crea una instancia de Tmodel 
            foreach (DataRow filas in ds.Tables[0].Rows)
            {
                //Se intenta asignar el valor correspondiente de cada fila a la propiedad correspondiente
                TModel objeto = (TModel)Activator.CreateInstance(typeof(TModel));

                foreach (System.Reflection.PropertyInfo campo in atributos)
                    try
                    {
                        tipo.GetProperty(campo.Name).SetValue((TModel)objeto, filas[campo.Name]);
                    }
                    catch (Exception e)
                    {

                    }

                mo.Add((TModel)objeto);
            }
            //El método devuelve el primer elemento de la lista mo (usando FirstOrDefault()).
            return mo.FirstOrDefault();

        }

        /// <summary>
        /// Convierte un conjunto de datos (dataset) en un objeto de modelo
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="modelo">The modelo.</param>
        public void ConvertToModel<TModel>(out TModel modelo)
        {
            //Se crea una lista de tipo TModel
            List<TModel> mo = new List<TModel>();

            //Se obtiene el tipo de datos mediante reflexion 
            Type tipo = typeof(TModel);
            var atributos = tipo.GetProperties();
            //Se recorre el data set y se instancia un objeto de Tmodel
            foreach (DataRow filas in ds.Tables[0].Rows)
            {
                //Se intenta asignar el valor correspondiente de la fila actual a cada propiedad.
                TModel objeto = (TModel)Activator.CreateInstance(typeof(TModel));

                foreach (System.Reflection.PropertyInfo campo in atributos)
                    try
                    {
                        tipo.GetProperty(campo.Name).SetValue((TModel)objeto, filas[campo.Name]);
                    }
                    catch
                    {

                    }
                // se agrega el objeto TModel a la lista mo.

                mo.Add((TModel)objeto);
            }
            //El primer elemento de la lista mo se asigna al parámetro modelo.
            modelo = mo.FirstOrDefault();

        }

        /// <summary>
        /// Mapea propiedades de un modelo a parametros de un procedimiento de almacenado
        /// </summary>
        /// <param name="Model">The model.</param>
        /// <param name="SQLParameter">The SQL parameter.</param>
        /// <returns></returns>
        public bool ModelToProcedure(object Model, string SQLParameter)
        {
            bool respuesta = false;

            var atributos = Model.GetType().GetProperties();

            //Se crea una lista de parametros
            List<SqlParameter> parametros = new List<SqlParameter>();

            //Se obtiene el tipo del objeto mediante reflexion
            foreach (System.Reflection.PropertyInfo campo in atributos)
                try
                {
                    //Se crea un nuevo parametro seguido de @pm y la propiedad
                    //Se obtiene el valor de la propiedad actual del objeto Model.
                    parametros.Add(new SqlParameter("@pm" + campo.Name, Model.GetType().GetProperty(campo.Name).GetValue(Model)));

                    //Se agrega a la lista de parametros
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
        /// Mapea propiedades de un objeto de modelo a parámetros de un procedimiento almacenado en una base de datos.
        /// </summary>
        /// <param name="Model">The model.</param>
        /// <returns></returns>
        public bool insertModel(object Model)
        {

            bool respuesta = false;
            //Obtiene el tipos de datos mediante reflexion
            var atributos = Model.GetType().GetProperties();

            //Crea una lista de parametros
            List<SqlParameter> parametros = new List<SqlParameter>();


            foreach (System.Reflection.PropertyInfo campo in atributos)
                try
                {
                    //Por cada parametro creado lo añade en la columna correspondiente añadiendole @pm
                    qryValues.Add("@pm" + campo.Name, Model.GetType().GetProperty(campo.Name).GetValue(Model));
                    //Lo añade a la lista
                    addParametersProc(parametros);
                    insert();

                    //Se ejecuta una consulta (query) 
                    ejecutarQuery();
                    respuesta = true;
                }
                catch
                {

                }
            return respuesta;
        }

        /// <summary>
        /// Obtiene la fecha y hora actual del servidor de la base de datos
        /// </summary>
        /// <returns></returns>
        public DateTime getFechaServer()
        {
            //elecciona la fecha y hora actual del servidor de base de datos utilizando la función GETDATE()
            this.sqlQuery = "select GETDATE() as FechaServer";
            this.ejecutarQuery();
            //Se accede a la primera tabla y la primera columna como un objeto de tipo Date time
            return this.ds.Tables[0].Rows[0].Field<DateTime>("FechaServer");
        }

        /// <summary>
        /// Se utiliza para obtener la direccion ip del cliente que visitia al sitio web
        /// </summary>
        /// <returns>The IP Address</returns>
        public static string GetUserIPAddress()
        {
            //Se obtiene el contexto actual de la aplicación web 
            var context = System.Web.HttpContext.Current;
            string ip = String.Empty;

            //Se verifica si el contexto no es nulo, se le  asigna su valor a ip.
            if (context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            //Se verifica la direccion ip del cliente para  asignar su valor a ip.
            else if (!String.IsNullOrWhiteSpace(context.Request.UserHostAddress))
                ip = context.Request.UserHostAddress;
            //se verifica si la direccion es igual  a “::1” (que representa la dirección IP local de loopback). Si es igual, se cambia a “127.0.0.1”.
            if (ip == "::1")
                ip = "127.0.0.1";
            //Se retorna la direccion.
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
            //Se relaciona y depende del metodo Connect
            //El cual se conecta a la base de datos con el parametro de entrada
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        private void Connect()
        {
            //Se realiza una instancia de la clase SqlConnection, la cual se utiliza para establecer conexiones con bases de datos SQL Server.
            this.sqlConn = new SqlConnection();

            //contendrá la cadena de conexión específica que se utilizará para conectarse a la base de datos.
            string cadenaconex = ConfigurationManager.ConnectionStrings[this.strName].ConnectionString;

            //crea una instancia de SqlConnectionStringBuilder, que es útil para construir y manipular cadenas de conexión de manera programática.
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();

            //Se asigna la cadena de conexión obtenida previamente al constructor de SqlConnectionStringBuilder.
            builder.ConnectionString = cadenaconex;

            //Se extrae el nombre de la base de datos (catálogo inicial) de la cadena de conexión y se almacena en la propiedad DataBase.
            this.DataBase = builder.InitialCatalog;

            //Se obtiene el nombre del servidor 
            this.Server = builder.DataSource;

            //Se extrae el nombre de usuario y se almacena en UserID
            this.User = builder.UserID;

            //Se instancia la clase pwdSyscom la cual se encarga de decodificar las contraseñas 
            pwdSyscom pwdSys = new pwdSyscom();
            pwdSys.Decodificar(builder.Password);
            builder.Password = pwdSys.contrasenna;
            this.Pass = builder.Password;

            //Finalmente, se establece la cadena de conexión completa en la instancia de SqlConnection llamada sqlConn.
            this.sqlConn.ConnectionString = builder.ConnectionString;

            //Se crea una instancia de DataSet para  almacenar datos en memoria.
            this.ds = new DataSet();

        }

        /// <summary>
        /// Begins the tran.
        /// </summary>
        public void beginTran()
        {
            //Llama al metodo connect
            Connect();
            //Abre la conexion
            this.sqlConn.Open();
            //Inicia la trasaccion
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



        /// <summary>
        /// Funcion  para convertir una data table en una cadena de objetos
        /// </summary>
        /// <typeparam name="T">Objeto generico</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        /// 

        public List<T> DataTableToList<T>(DataTable dtDatos) where T : class, new()
        {
            try
            {
                //Genera una lista vacia de objetos tipos genericos
                List<T> list = new List<T>();

                //Recorre el DataTable 
                foreach (DataRow row in dtDatos.Rows)
                {
                    //Para cada fila genera una instancia del objeto generico
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {   //Luego recorre las propiedades de T y asigna los valores de la columna al objeto
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            //Si hay algun error durante la asignacion de valores como de coincidencia de datos , este se omite y continua
                            continue;
                        }
                    }
                    // Si en el proceso de asignacion todo sale bien se agrega a una lista los objetos con los valores
                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                //Si no se lleva acabo dicho proceso, la funcion devuelve null
                return null;
            }
        }

        /// <summary>
        /// Funcion de DataTable que recibe un DataSet y lo convierte en una lista de objetos
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="DatosDt">DataTable</param>
        /// <param name="ds">DataSet</param>
        /// <returns>List with generic objects</returns>
        /// 
        public List<T> DataTableToList<T>(string[] DatosDt = null, DataSet ds = null) where T : class, new()
        {
            try
            {
                //Crea una lista de objetos genericos
                List<T> list = new List<T>();

                //Si el DataTable es nulo toma la primera tabla del objeto data set , 
                //de lo contrario le asgina una vista al DataTable original utilizando las columnas especificadas en DatosDt.
                DataTable dt = DatosDt == null ? ds.Tables[0] : ds.Tables[0].DefaultView.ToTable(true, DatosDt);
                foreach (DataRow row in dt.Rows)
                {
                    //Recorre el data table e inicia un nuevo objeto generico
                    T obj = new T();
                    //Para cada fila genera una instancia del objeto generico
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        //Luego recorre las propiedades de T y asigna los valores de la columna al objeto
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            //Si hay algun error durante la asignacion de valores como de coincidencia de datos , este se omite y continua
                            continue;
                        }
                    }
                    // Si en el proceso de asignacion todo sale bien se agrega a una lista los objetos con los valores
                    list.Add(obj);
                }
                //Luego de finalizar el proceso retorna la lista
                return list;
            }
            catch
            {
                //Si no se lleva acabo dicho proceso, la funcion devuelve null
                return null;
            }
        }

        #endregion Configuracion

    }
}