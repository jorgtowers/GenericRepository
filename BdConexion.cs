using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.OracleClient;
using System.Web;
using System.Configuration;
using System.Runtime.Remoting;
using Microsoft.SqlServer;


namespace Utils.BaseDatos.ADONET
{    
    public abstract class BdConexion : MarshalByRefObject
    {
        #region Variables
        static String _KeyNameAppWebConfig = "";
        static String _CadenaConexion = "";
        static String _ConnectionString = "";
        static StringBuilder _BuildStatement;
        static Enumeracion.DataBase.Provider _ProviderDataBase = Enumeracion.DataBase.Provider.SqlClient;
        static Enumeracion.DataBase.ConnectionMode _ConnectionMode = Enumeracion.DataBase.ConnectionMode.ConnectionString;
        static Enumeracion.DataBase.Separator _Separator = Enumeracion.DataBase.Separator.VerticalBar;
        static Enumeracion.ProveedorBaseDatos _ProveedorBaseDatos = Enumeracion.ProveedorBaseDatos.SqlClient;
        static Enumeracion.ModoConexion _ModoConexion = Enumeracion.ModoConexion.ConnectionString;
        static Enumeracion.Separador _Separador = Enumeracion.Separador.BarraVertical;
        private SqlConnection _SqlConexion;
        private SqlConnection _SqlConnection = null;
        private SqlTransaction _SqlTransaction = null;
        private OracleConnection _OraConexion;
        private OracleConnection _OraConnection = null;
        private OracleTransaction _OraTransaction = null;
        private OleDbConnection _OleDbConexion;
        private OleDbConnection _OleDbConnection = null;
        private OleDbTransaction _OleDbTransaction = null;
        private IDbConnection _IDbConnection;
        private IDbCommand _IDbCommand;
        private IDbDataAdapter _IDbDataAdapter;
        #endregion
        #region Constructor
        public BdConexion() { }
        #endregion
        #region Metodos
        public static string BuildConnectionString(Enumeracion.DataBase.ProviderConnectionString providerStringConexion, string serverName, string userID, string password, string sqlInitialCatalog, string oraSID, string oraPort, string oraServiceName, string pathMdbFile)
        {
            _BuildStatement = new StringBuilder();
            switch (providerStringConexion)
            {
                case Enumeracion.DataBase.ProviderConnectionString.SQL_SqlClient:
                    _BuildStatement.Append("Data Source=" + serverName);
                    _BuildStatement.Append(";Initial Catalog=" + sqlInitialCatalog);
                    _BuildStatement.Append(";Persist Security Info=" + true);
                    _BuildStatement.Append(";User ID=" + userID);
                    _BuildStatement.Append(";Password=" + password);
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.SQL_ODBC:
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.ORA_OracleClient_NetFramework_Standard:
                    _BuildStatement.Append("Data Source=" + serverName);
                    _BuildStatement.Append(";Integrated Security=yes;");
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.ORA_OracleClient_NetFramework_SpecifyingUserPwd:
                    _BuildStatement.Append("Data Source=" + serverName);
                    _BuildStatement.Append(";User ID=" + userID);
                    _BuildStatement.Append(";Password=" + password);
                    _BuildStatement.Append(";Integrated Security=no;");
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.ORA_OracleClient_NetFramework_WithOutTNS_BySID:
                    _BuildStatement.Append("Server=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + serverName + ")(PORT=" + oraPort + "))(CONNECT_DATA=(SID=" + oraSID + ")))");
                    _BuildStatement.Append(";Uid=" + userID);
                    _BuildStatement.Append(";Pwd=" + password);
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.ORA_OracleClient_NetFramework_WithOutTNS_ByServiceName:
                    _BuildStatement.Append("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + serverName + ")(PORT=" + oraPort + "))(CONNECT_DATA=(SERVICE_NAME=" + oraServiceName + ")))");
                    _BuildStatement.Append(";Uid=" + userID);
                    _BuildStatement.Append(";Pwd=" + password);
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.ORA_ODBC_Oracle_OraClient10g_home:
                    _BuildStatement.Append("Dsn=" + serverName);
                    _BuildStatement.Append(";Uid=" + userID);
                    _BuildStatement.Append(";Pwd=" + password);
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.ORA_ODBC_MicrosoftForOracle_NewVersion:
                    _BuildStatement.Append("Driver={Microsoft ODBC for Oracle}");
                    _BuildStatement.Append(";Server=" + serverName);
                    _BuildStatement.Append(";Uid=" + userID);
                    _BuildStatement.Append(";Pwd=" + password);
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.ORA_ODBC_MicrosoftForOracle_ConnectDirectly_BySID:
                    _BuildStatement.Append("Driver={Microsoft ODBC for Oracle}");
                    _BuildStatement.Append(";Server=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + serverName + ")(PORT=" + oraPort + "))(CONNECT_DATA=(SID=" + oraSID + ")))");
                    _BuildStatement.Append(";Uid=" + userID);
                    _BuildStatement.Append(";Pwd=" + password);
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.ORA_ODBC_MicrosoftForOracle_WithOutTNS_ByServiceName:
                    _BuildStatement.Append("Driver={Microsoft ODBC for Oracle}");
                    _BuildStatement.Append(";CONNECTSTRING=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + serverName + ")(PORT=" + oraPort + "))(CONNECT_DATA=(SERVICE_NAME=" + oraServiceName + ")))");
                    _BuildStatement.Append(";Uid=" + userID);
                    _BuildStatement.Append(";Pwd=" + password);
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.OLE_OleDB:
                    _BuildStatement.Append("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathMdbFile);
                    _BuildStatement.Append(";Persist Security Info=False;");
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.OLE_ODBC_StandardSecurity:
                    _BuildStatement.Append("Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=" + pathMdbFile);
                    _BuildStatement.Append(";Uid=" + userID);
                    _BuildStatement.Append(";Pwd=" + password);
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.OLE_Microsoft_OleDb_Oracle_Standard:
                    _BuildStatement.Append("Provider=msdaora;Data Source=" + serverName);
                    _BuildStatement.Append(";User Id=" + userID);
                    _BuildStatement.Append(";Password=" + password);
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.OLE_Microsoft_OleDb_Oracle_Standard_New_BySID:
                    _BuildStatement.Append("Provider=msdaora;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + serverName + ")(PORT=" + oraPort + "))(CONNECT_DATA=(SID=" + oraSID + ")))");
                    _BuildStatement.Append(";User Id=" + userID);
                    _BuildStatement.Append(";Password=" + password);
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.OLE_Microsoft_OleDb_Oracle_Standard_New_ByServiceName:
                    _BuildStatement.Append("Provider=msdaora;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + serverName + ")(PORT=" + oraPort + "))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + oraServiceName + ")))");
                    _BuildStatement.Append(";User Id=" + userID);
                    _BuildStatement.Append(";Password=" + password);
                    break;
                case Enumeracion.DataBase.ProviderConnectionString.OLE_Microsoft_OleDb_Oracle_TrustedConnection:
                    _BuildStatement.Append("Provider=msdaora;Data Source=" + serverName);
                    _BuildStatement.Append(";Persist Security Info=False");
                    _BuildStatement.Append(";Integrated Security=Yes");
                    break;
                default:
                    break;
            }
            return _BuildStatement.ToString();
        }
        protected string GetConnectionString()
        {

            string _return = "";
            switch (_ConnectionMode)
            {
                case Enumeracion.DataBase.ConnectionMode.ConnectionString:
                    {
                        _return = ConfigurationManager.ConnectionStrings[_KeyNameAppWebConfig].ToString();
                        break;
                    }
                case Enumeracion.DataBase.ConnectionMode.AppSettings:
                    {
                        _return = ConfigurationManager.AppSettings[_KeyNameAppWebConfig].ToString();
                        break;
                    }
                case Enumeracion.DataBase.ConnectionMode.String:
                    {
                        _return = _ConnectionString;
                        break;
                    }
                default:
                    {
                        _return = "";
                        break;
                    }
            }
            return _return;
        }
        public static string GetSeparator()
        {
            string _split = "";
            switch (_Separator)
            {
                case Enumeracion.DataBase.Separator.Nothing:
                    {
                        _split = "";
                        break;
                    }
                case Enumeracion.DataBase.Separator.Guion:
                    {
                        _split = "-";
                        break;
                    }
                case Enumeracion.DataBase.Separator.Slash:
                    {
                        _split = "/";
                        break;
                    }
                case Enumeracion.DataBase.Separator.UnderScore:
                    {
                        _split = "_";
                        break;
                    }
                case Enumeracion.DataBase.Separator.Comma:
                    {
                        _split = ",";
                        break;
                    }
                case Enumeracion.DataBase.Separator.SemiColon:
                    {
                        _split = ";";
                        break;
                    }
                case Enumeracion.DataBase.Separator.VerticalBar:
                    {
                        _split = "|";

                        break;
                    }
                default:
                    {
                        _split = "|";
                        break;
                    }
            }
            return _split;
        }

        #region Metodos de Base de Datos

        private void SQLOpenConnection()
        {
            if (_SqlConexion == null)
                _SqlConexion = new SqlConnection(GetConnectionString());
            if (_SqlConexion.State != ConnectionState.Open)
                _SqlConexion.Open();

        }
        private void ORAOpenConnection()
        {
            if (_OraConexion == null)
                _OraConexion = new OracleConnection(GetConnectionString());
            if (_OraConexion.State != ConnectionState.Open)
                _OraConexion.Open();

        }
        private void OLEDBOpenConnection()
        {
            if (_OleDbConexion == null)
                _OleDbConexion = new OleDbConnection(GetConnectionString());
            if (_OleDbConexion.State != ConnectionState.Open)
                _OleDbConexion.Open();

        }

        private void SQLCloseConnection()
        {
            if (_SqlConexion != null && _SqlConexion.State != ConnectionState.Closed)
            {
                _SqlConexion.Close();
                _SqlConexion = null;
            }
        }
        private void ORACloseConnection()
        {
            if (_OraConexion != null && _OraConexion.State != ConnectionState.Closed)
            {
                _OraConexion.Close();
                _OraConexion = null;
            }
        }
        private void OLEDBCloseConnection()
        {
            if (_OleDbConexion != null && _OleDbConexion.State != ConnectionState.Closed)
            {
                _OleDbConexion.Close();
                _OleDbConexion = null;
            }
        }

        private SqlCommand SQLCreateParameters(KeyValuePair<string, object>[] parametros)
        {
            SqlCommand commnad = new SqlCommand();
            if (parametros != null)
            {
                foreach (KeyValuePair<string, object> item in parametros)
                {
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = item.Key.ToString();
                    param.Value = item.Value;
                    if (param.SqlDbType == SqlDbType.Timestamp)
                    {
                        param.SqlDbType = SqlDbType.VarChar;
                    }
                    commnad.Parameters.Add(param);
                }
            }
            return commnad;
        }
        private OracleCommand ORACreateParameters(KeyValuePair<string, object>[] parametros)
        {
            OracleCommand commnad = new OracleCommand();
            if (parametros != null)
            {
                foreach (KeyValuePair<string, object> item in parametros)
                {
                    OracleParameter param = new OracleParameter();
                    param.ParameterName = item.Key.ToString();
                    param.Value = item.Value;
                    if (param.OracleType == OracleType.Timestamp)
                    {
                        param.OracleType = OracleType.VarChar;
                    }
                    commnad.Parameters.Add(param);
                }
            }
            return commnad;
        }
        private OleDbCommand OLEDBCreateParameters(KeyValuePair<string, object>[] parametros)
        {
            OleDbCommand commnad = new OleDbCommand();
            if (parametros != null)
            {
                foreach (KeyValuePair<string, object> item in parametros)
                {
                    OleDbParameter param = new OleDbParameter();
                    param.ParameterName = item.Key.ToString();
                    param.Value = item.Value;
                    if (param.OleDbType == OleDbType.DBTimeStamp)
                    {
                        param.OleDbType = OleDbType.VarChar;
                    }
                    commnad.Parameters.Add(param);
                }
            }
            return commnad;
        }


        private void SQLCrearTransaction()
        {
            _SqlTransaction = SQLConnection.BeginTransaction();
        }
        private void ORACrearTransaction()
        {
            _OraTransaction = ORAConnection.BeginTransaction();
        }
        private void OLEDBCrearTransaction()
        {
            _OleDbTransaction = OLEDBConnection.BeginTransaction();
        }

        private void SQLCommitTransaction()
        {
            try
            {
                _SqlTransaction.Commit();
                _SqlTransaction = null;
                SQLCloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void ORACommitTransaction()
        {
            try
            {
                _OraTransaction.Commit();
                _OraTransaction = null;
                ORACloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void OleDbCommitTransaction()
        {
            try
            {
                _OleDbTransaction.Commit();
                _OleDbTransaction = null;
                OLEDBCloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void SQLRollbackTransaction()
        {
            try
            {
                if (_SqlTransaction != null)
                {
                    _SqlTransaction.Rollback();
                    _SqlTransaction = null;
                }
                SQLCloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void ORARollbackTransaction()
        {
            try
            {
                if (_OraTransaction != null)
                {
                    _OraTransaction.Rollback();
                    _OraTransaction = null;
                }
                ORACloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void OLEDBRollbackTransaction()
        {
            try
            {
                if (_OleDbTransaction != null)
                {
                    _OleDbTransaction.Rollback();
                    _OleDbTransaction = null;
                }
                OLEDBCloseConnection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private SqlConnection SQLConnection
        {

            get
            {
                //if (m_connection == null)

                try
                {
                    _SqlConnection = new SqlConnection();
                    _SqlConnection.ConnectionString = GetConnectionString();
                    _SqlConnection.Open();
                }
                catch
                {
                    throw;
                }


                return _SqlConnection;

            }

        }
        private OracleConnection ORAConnection
        {

            get
            {
                //if (m_connection == null)

                try
                {
                    _OraConexion = new OracleConnection();
                    _OraConexion.ConnectionString = GetConnectionString();
                    _OraConexion.Open();
                }
                catch
                {
                    throw;
                }


                return _OraConexion;

            }

        }
        private OleDbConnection OLEDBConnection
        {

            get
            {
                //if (m_connection == null)

                try
                {
                    _OleDbConexion = new OleDbConnection();
                    _OleDbConexion.ConnectionString = GetConnectionString();
                    _OleDbConexion.Open();
                }
                catch
                {
                    throw;
                }


                return _OleDbConexion;

            }

        }
        /// <summary>
        /// Ejecuta un sentencia y retorna un entero con el resultado de la ejecución, puede recibir arreglo de parametros usando un KeyValuePair String,Object[]
        /// </summary>
        /// <param name="comando">Sentencia a ejecutar (SELECT; INSERT; UPDATE; DELETE)</param>
        /// <param name="parametros">Arreglo de parametros usando un KeyValuePair String,Object</param>
        /// <returns></returns>
        protected int SQLExecuteCommand(string comando, params KeyValuePair<string, object>[] parametros)
        {
            try
            {
                SqlCommand command;
                if (parametros != null && parametros.Length > 0)
                {
                    command = SQLCreateParameters(parametros);
                }
                else
                {
                    command = new SqlCommand();
                }
                command.CommandText = comando;
                SQLOpenConnection();
                command.Connection = _SqlConexion;
                int valor = command.ExecuteNonQuery();
                return valor;
            }
            catch
            {
                throw;
            }
            finally
            {
                SQLCloseConnection();
            }

        }
        protected int ORAExecuteCommand(string comando, params KeyValuePair<string, object>[] parametros)
        {
            try
            {
                OracleCommand command;
                if (parametros != null && parametros.Length > 0)
                {
                    command = ORACreateParameters(parametros);
                }
                else
                {
                    command = new OracleCommand();
                }
                command.CommandText = comando;
                ORAOpenConnection();
                command.Connection = _OraConexion;
                int valor = command.ExecuteNonQuery();
                return valor;
            }
            catch
            {
                throw;
            }
            finally
            {
                ORACloseConnection();
            }

        }
        protected int OLEDBExecuteCommand(string comando, params KeyValuePair<string, object>[] parametros)
        {
            try
            {
                OleDbCommand command;
                if (parametros != null && parametros.Length > 0)
                {
                    command = OLEDBCreateParameters(parametros);
                }
                else
                {
                    command = new OleDbCommand();
                }
                command.CommandText = comando;
                OLEDBOpenConnection();
                command.Connection = _OleDbConexion;
                int valor = command.ExecuteNonQuery();
                return valor;
            }
            catch
            {
                throw;
            }
            finally
            {
                OLEDBCloseConnection();
            }

        }

        /// <summary>
        /// Ejecuta un StoredProcedure y recibe un arreglo de object[] que contiene los valores de los parametros
        /// </summary>
        /// <param name="procedureName">Nombre del StoredProcedure</param>
        /// <param name="parameters">Arreglo de objetos pasados como un new Object[], indicando la cantidad de parametros que recibe el StoredProcedure</param>
        protected void SQLExecuteNonQuery(string procedureName, params object[] parameters)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                if (_SqlTransaction != null)
                {
                    command.Transaction = _SqlTransaction;
                    command.Connection = _SqlTransaction.Connection;
                }
                else
                {
                    command.Connection = SQLConnection;
                }
                SqlCommandBuilder.DeriveParameters(command);

                for (int i = 1; i < command.Parameters.Count; i++)
                {
                    command.Parameters[i].Value = parameters[i - 1];
                }
                command.ExecuteNonQuery();
                if (_SqlTransaction == null)
                {
                    SQLCloseConnection();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        protected void ORAExecuteNonQuery(string procedureName, params object[] parameters)
        {
            try
            {
                OracleCommand command = new OracleCommand();
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                if (_OraTransaction != null)
                {
                    command.Transaction = _OraTransaction;
                    command.Connection = _OraTransaction.Connection;
                }
                else
                {
                    command.Connection = ORAConnection;
                }
                OracleCommandBuilder.DeriveParameters(command);

                for (int i = 1; i < command.Parameters.Count; i++)
                {
                    command.Parameters[i].Value = parameters[i - 1];
                }
                command.ExecuteNonQuery();
                if (_OraTransaction == null)
                {
                    ORACloseConnection();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        protected void OLEDBExecuteNonQuery(string procedureName, params object[] parameters)
        {
            try
            {
                OleDbCommand command = new OleDbCommand();
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                if (_OleDbTransaction != null)
                {
                    command.Transaction = _OleDbTransaction;
                    command.Connection = _OleDbTransaction.Connection;
                }
                else
                {
                    command.Connection = OLEDBConnection;
                }
                OleDbCommandBuilder.DeriveParameters(command);

                for (int i = 1; i < command.Parameters.Count; i++)
                {
                    command.Parameters[i].Value = parameters[i - 1];
                }
                command.ExecuteNonQuery();
                if (_OleDbTransaction == null)
                {
                    OLEDBCloseConnection();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Ejecuta una sentencia tipo SELECT y puede pasar pametros mediante un arreglo KeyValuePair String,Object
        /// </summary>
        /// <param name="strcommand">Sentencia SELECT,  los parametros se indicarán usando '@paramName' y se relacionan al KeyValuePair</param>
        /// <param name="parametros">Arreglo de objetos pasados como un new KeyValuePair donde la llave String tomará el lugar del @paramName y el object tomará el lugar del valor  </param>
        /// <returns>Retorna un DataTable con los valores del SELECT</returns>
        protected DataTable SQLExecuteSelect(string strcommand, params KeyValuePair<string, object>[] parametros)
        {
            DataTable table = new DataTable();
            SqlCommand command;
            if (parametros != null && parametros.Length > 0)
            {
                command = SQLCreateParameters(parametros);
            }
            else
            {
                command = new SqlCommand();
            }
            command.CommandText = strcommand;
            SQLOpenConnection();
            command.Connection = _SqlConexion;
            SqlDataReader reader = command.ExecuteReader();
            DataTable tableSchema = reader.GetSchemaTable();
            DataColumn[] columnas = new DataColumn[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnas[i] = new DataColumn();
                columnas[i].ColumnName = tableSchema.Rows[i][0].ToString();
            }
            table.Columns.AddRange(columnas);
            object[] data = new object[reader.FieldCount];
            while (reader.Read())
            {
                reader.GetValues(data);
                table.Rows.Add(data);
            }
            SQLCloseConnection();
            return table;
        }
        /// <summary>
        /// Ejecuta una sentencia tipo SELECT y puede pasar pametros mediante un arreglo KeyValuePair String,Object
        /// </summary>
        /// <param name="strcommand">Sentencia SELECT,  los parametros se indicarán usando '&paramName' y se relacionan al KeyValuePair</param>
        /// <param name="parametros">Arreglo de objetos pasados como un new KeyValuePair donde la llave String tomará el lugar del &paramName y el object tomará el lugar del valor  </param>
        /// <returns>Retorna un DataTable con los valores del SELECT</returns>
        protected DataTable ORAExecuteSelect(string strcommand, params KeyValuePair<string, object>[] parametros)
        {
            DataTable table = new DataTable();
            OracleCommand command;
            if (parametros != null && parametros.Length > 0)
            {
                command = ORACreateParameters(parametros);
            }
            else
            {
                command = new OracleCommand();
            }
            command.CommandText = strcommand;
            ORAOpenConnection();
            command.Connection = _OraConexion;
            OracleDataReader reader = command.ExecuteReader();
            DataTable tableSchema = reader.GetSchemaTable();
            DataColumn[] columnas = new DataColumn[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnas[i] = new DataColumn();
                //Asegura el tipo de dato en la tabla nueva
                columnas[i].DataType = (Type)tableSchema.Rows[i][5];
                columnas[i].ColumnName = tableSchema.Rows[i][0].ToString();
            }
            table.Columns.AddRange(columnas);
            object[] data = new object[reader.FieldCount];
            while (reader.Read())
            {
                reader.GetValues(data);
                table.Rows.Add(data);
            }
            ORACloseConnection();
            return table;
        }
        protected DataTable OLEDBExecuteSelect(string strcommand, params KeyValuePair<string, object>[] parametros)
        {
            DataTable table = new DataTable();
            OleDbCommand command;
            if (parametros != null && parametros.Length > 0)
            {
                command = OLEDBCreateParameters(parametros);
            }
            else
            {
                command = new OleDbCommand();
            }
            command.CommandText = strcommand;
            OLEDBOpenConnection();
            command.Connection = _OleDbConexion;
            OleDbDataReader reader = command.ExecuteReader();
            DataTable tableSchema = reader.GetSchemaTable();
            DataColumn[] columnas = new DataColumn[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnas[i] = new DataColumn();
                columnas[i].ColumnName = tableSchema.Rows[i][0].ToString();
            }
            table.Columns.AddRange(columnas);
            object[] data = new object[reader.FieldCount];
            while (reader.Read())
            {
                reader.GetValues(data);
                table.Rows.Add(data);
            }
            OLEDBCloseConnection();
            return table;
        }

        /// <summary>
        /// Ejecuta un StoredProcedure
        /// </summary>
        /// <param name="procedureName">Nombre del StoredProcedure</param>
        /// <returns>Returna un DataTable con los valores del StoredProcedure</returns>
        protected DataTable SQLExecuteStoredProcedure(string procedureName)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.Connection = SQLConnection;
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                SqlCommandBuilder.DeriveParameters(command);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable result = new DataTable();
                adapter.Fill(result);
                return result;
            }
            catch
            {
                throw;
            }

        }
        protected DataTable ORAExecuteStoredProcedure(string procedureName)
        {
            try
            {
                OracleCommand command = new OracleCommand();
                command.Connection = ORAConnection;
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                OracleCommandBuilder.DeriveParameters(command);
                OracleDataAdapter adapter = new OracleDataAdapter(command);
                DataTable result = new DataTable();
                adapter.Fill(result);
                return result;
            }
            catch
            {
                throw;
            }

        }
        protected DataTable OLEDBExecuteStoredProcedure(string procedureName)
        {
            try
            {
                OleDbCommand command = new OleDbCommand();
                command.Connection = OLEDBConnection;
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                OleDbCommandBuilder.DeriveParameters(command);
                OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                DataTable result = new DataTable();
                adapter.Fill(result);
                return result;
            }
            catch
            {
                throw;
            }

        }

        protected DataTable SQLExecuteStoredProcedure(string procedureName, params object[] parameters)
        {

            SqlCommand command = new SqlCommand();
            command.Connection = SQLConnection;
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            SqlCommandBuilder.DeriveParameters(command);
            for (int i = 1; i < command.Parameters.Count; i++)
            {
                command.Parameters[i].Value = parameters[i - 1];
            }
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable result = new DataTable();
            adapter.Fill(result);
            SQLCloseConnection();
            return result;


        }
        protected DataTable ORAExecuteStoredProcedure(string procedureName, params object[] parameters)
        {

            OracleCommand command = new OracleCommand();
            command.Connection = ORAConnection;
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            OracleCommandBuilder.DeriveParameters(command);
            for (int i = 1; i < command.Parameters.Count; i++)
            {
                command.Parameters[i].Value = parameters[i - 1];
            }
            OracleDataAdapter adapter = new OracleDataAdapter(command);
            DataTable result = new DataTable();
            adapter.Fill(result);
            ORACloseConnection();
            return result;


        }
        protected DataTable OLEDBExecuteStoredProcedure(string procedureName, params object[] parameters)
        {

            OleDbCommand command = new OleDbCommand();
            command.Connection = OLEDBConnection;
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            OleDbCommandBuilder.DeriveParameters(command);
            for (int i = 1; i < command.Parameters.Count; i++)
            {
                command.Parameters[i].Value = parameters[i - 1];
            }
            OleDbDataAdapter adapter = new OleDbDataAdapter(command);
            DataTable result = new DataTable();
            adapter.Fill(result);
            OLEDBCloseConnection();
            return result;


        }

        protected DataTable SQLExecuteStoredProcedure(string procedureName, params KeyValuePair<string, object>[] parameters)
        {

            SqlCommand command = new SqlCommand();
            command.Connection = SQLConnection;
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            for (int i = 0; i < parameters.Length; i++)
            {
                command.Parameters.AddWithValue(parameters[i].Key, parameters[i].Value);
            }
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable result = new DataTable();
            adapter.Fill(result);
            SQLCloseConnection();
            return result;


        }
        protected DataTable ORAExecuteStoredProcedure(string procedureName, params KeyValuePair<string, object>[] parameters)
        {

            OracleCommand command = new OracleCommand();
            command.Connection = ORAConnection;
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            for (int i = 0; i < parameters.Length; i++)
            {
                command.Parameters.AddWithValue(parameters[i].Key, parameters[i].Value);
            }
            OracleDataAdapter adapter = new OracleDataAdapter(command);
            DataTable result = new DataTable();
            adapter.Fill(result);
            ORACloseConnection();
            return result;


        }
        protected DataTable OLEDBExecuteStoredProcedure(string procedureName, params KeyValuePair<string, object>[] parameters)
        {

            OleDbCommand command = new OleDbCommand();
            command.Connection = OLEDBConnection;
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            for (int i = 0; i < parameters.Length; i++)
            {
                command.Parameters.AddWithValue(parameters[i].Key, parameters[i].Value);
            }
            OleDbDataAdapter adapter = new OleDbDataAdapter(command);
            DataTable result = new DataTable();
            adapter.Fill(result);
            OLEDBCloseConnection();
            return result;


        }

        protected DataTable SQLExecuteQuery(string select, params KeyValuePair<string, object>[] parameters)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = SQLConnection;
            command.CommandText = select;
            for (int i = 0; i < parameters.Length; i++)
            {
                command.Parameters.AddWithValue(parameters[i].Key, parameters[i].Value);
            }
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable result = new DataTable();
            adapter.Fill(result);
            SQLCloseConnection();
            return result;

        }
        protected DataTable ORAExecuteQuery(string select, params KeyValuePair<string, object>[] parameters)
        {
            OracleCommand command = new OracleCommand();
            command.Connection = ORAConnection;
            command.CommandText = select;
            for (int i = 0; i < parameters.Length; i++)
            {
                command.Parameters.AddWithValue(parameters[i].Key, parameters[i].Value);
            }
            OracleDataAdapter adapter = new OracleDataAdapter(command);
            DataTable result = new DataTable();
            adapter.Fill(result);
            ORACloseConnection();
            return result;

        }
        protected DataTable OLEDBExecuteQuery(string select, params KeyValuePair<string, object>[] parameters)
        {
            OleDbCommand command = new OleDbCommand();
            command.Connection = OLEDBConnection;
            command.CommandText = select;
            for (int i = 0; i < parameters.Length; i++)
            {
                command.Parameters.AddWithValue(parameters[i].Key, parameters[i].Value);
            }
            OleDbDataAdapter adapter = new OleDbDataAdapter(command);
            DataTable result = new DataTable();
            adapter.Fill(result);
            OLEDBCloseConnection();
            return result;

        }

        protected int SQLExecuteEscalar(string ProcedureName, params object[] Parameters)
        {

            SqlCommand command = new SqlCommand();
            command.Connection = SQLConnection;
            command.CommandText = ProcedureName;
            command.CommandType = CommandType.StoredProcedure;
            SqlCommandBuilder.DeriveParameters(command);
            for (int k = 1; k < command.Parameters.Count; k++)
            {
                command.Parameters[k].Value = Parameters[k - 1];
            }
            int t = Convert.ToInt32(command.ExecuteScalar());
            return t;

        }
        protected int ORAExecuteEscalar(string ProcedureName, params object[] Parameters)
        {

            OracleCommand command = new OracleCommand();
            command.Connection = ORAConnection;
            command.CommandText = ProcedureName;
            command.CommandType = CommandType.StoredProcedure;
            OracleCommandBuilder.DeriveParameters(command);
            for (int k = 1; k < command.Parameters.Count; k++)
            {
                command.Parameters[k].Value = Parameters[k - 1];
            }
            int t = Convert.ToInt32(command.ExecuteScalar());
            return t;

        }
        protected int OLEDBExecuteEscalar(string ProcedureName, params object[] Parameters)
        {

            OleDbCommand command = new OleDbCommand();
            command.Connection = OLEDBConnection;
            command.CommandText = ProcedureName;
            command.CommandType = CommandType.StoredProcedure;
            OleDbCommandBuilder.DeriveParameters(command);
            for (int k = 1; k < command.Parameters.Count; k++)
            {
                command.Parameters[k].Value = Parameters[k - 1];
            }
            int t = Convert.ToInt32(command.ExecuteScalar());
            return t;

        }

        #endregion

        ///// <summary>
        ///// Ejecuta scripts de Servidor SQL Server
        ///// </summary>
        ///// <param name="_Script"></param>
        //protected void EjecutarScripts(string _Script)
        //{
        //    string _CnnStringConexion = GetConnectionString();
        //    switch (_ProviderDataBase)
        //    {
        //        case Enumeracion.DataBase.Provider.SqlClient:

        //            //string connectionString, scriptText;
        //            SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
        //            ServerConnection svrConnection = new ServerConnection(cnn1);
        //            Server server = new Server(svrConnection);
        //            server.ConnectionContext.ExecuteNonQuery(_Script);
        //            break;

        //        case Enumeracion.DataBase.Provider.OracleClient:
        //            OracleConnection cnn2 = new OracleConnection(_CnnStringConexion);
        //            OracleCommand cmd2 = new OracleCommand(_Script, cnn2);
        //            cnn2.Open();
        //            cmd2.ExecuteNonQuery();
        //            cnn2.Close();
        //            break;

        //        case Enumeracion.DataBase.Provider.OleDb:
        //            OleDbConnection cnn3 = new OleDbConnection(_CnnStringConexion);
        //            OleDbCommand cmd3 = new OleDbCommand(_Script, cnn3);
        //            cnn3.Open();
        //            cmd3.ExecuteNonQuery();
        //            cnn3.Close();
        //            break;

        //        default:
        //            OleDbConnection cnn4 = new OleDbConnection(_CnnStringConexion);
        //            OleDbCommand cmd4 = new OleDbCommand(_Script, cnn4);
        //            cnn4.Open();
        //            cmd4.ExecuteNonQuery();
        //            cnn4.Close();
        //            break;
        //    }
        //}
        #region Metodos OBSOLETOS
        [Obsolete]
        public static string GetSeparador()
        {
            string _split = "";
            switch (Separador)
            {
                case Enumeracion.Separador.Ninguno:
                    {
                        _split = "";
                        break;
                    }
                case Enumeracion.Separador.Guion:
                    {
                        _split = "-";
                        break;
                    }
                case Enumeracion.Separador.Slash:
                    {
                        _split = "/";
                        break;
                    }
                case Enumeracion.Separador.UnderScore:
                    {
                        _split = "_";
                        break;
                    }
                case Enumeracion.Separador.Coma:
                    {
                        _split = ",";
                        break;
                    }
                case Enumeracion.Separador.PuntoyComa:
                    {
                        _split = ";";
                        break;
                    }
                case Enumeracion.Separador.BarraVertical:
                    {
                        _split = "|";

                        break;
                    }
                default:
                    {
                        _split = "|";
                        break;
                    }
            }
            return _split;
        }
        [Obsolete]
        public static string GenerarCadenaConexion(Enumeracion.ProveedorCadenaConexion providerStringConexion, string serverName, string userID, string password, string sqlInitialCatalog, string oraSID, string oraPort, string oraServiceName)
        {
            _BuildStatement = new StringBuilder();
            switch (providerStringConexion)
            {
                case Enumeracion.ProveedorCadenaConexion.SQL_SqlClient:
                    _BuildStatement.Append("Data Source=" + serverName);
                    _BuildStatement.Append(";Initial Catalog=" + sqlInitialCatalog);
                    _BuildStatement.Append(";Persist Security Info=" + true);
                    _BuildStatement.Append(";User ID=" + userID);
                    _BuildStatement.Append(";Password=" + password);
                    break;
                case Enumeracion.ProveedorCadenaConexion.SQL_ODBC:
                    break;
                case Enumeracion.ProveedorCadenaConexion.ORA_OracleClient_NetFramework_Standard:
                    _BuildStatement.Append("Data Source=" + serverName);
                    _BuildStatement.Append(";Integrated Security=yes;");
                    break;
                case Enumeracion.ProveedorCadenaConexion.ORA_OracleClient_NetFramework_SpecifyingUserPwd:
                    _BuildStatement.Append("Data Source=" + serverName);
                    _BuildStatement.Append(";User ID=" + userID);
                    _BuildStatement.Append(";Password=" + password);
                    _BuildStatement.Append(";Integrated Security=no;");
                    break;
                case Enumeracion.ProveedorCadenaConexion.ORA_OracleClient_NetFramework_WithOutTNS_BySID:
                    _BuildStatement.Append("Server=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + serverName + ")(PORT=" + oraPort + "))(CONNECT_DATA=(SID=" + oraSID + ")))");
                    _BuildStatement.Append(";Uid=" + userID);
                    _BuildStatement.Append(";Pwd=" + password);
                    break;
                case Enumeracion.ProveedorCadenaConexion.ORA_OracleClient_NetFramework_WithOutTNS_ByServiceName:
                    _BuildStatement.Append("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + serverName + ")(PORT=" + oraPort + "))(CONNECT_DATA=(SERVICE_NAME=" + oraServiceName + ")))");
                    _BuildStatement.Append(";Uid=" + userID);
                    _BuildStatement.Append(";Pwd=" + password);
                    break;
                case Enumeracion.ProveedorCadenaConexion.ORA_ODBC_Oracle_OraClient10g_home:
                    _BuildStatement.Append("Dsn=" + serverName);
                    _BuildStatement.Append(";Uid=" + userID);
                    _BuildStatement.Append(";Pwd=" + password);
                    break;
                case Enumeracion.ProveedorCadenaConexion.ORA_ODBC_MicrosoftForOracle_NewVersion:
                    _BuildStatement.Append("Driver={Microsoft ODBC for Oracle}");
                    _BuildStatement.Append(";Server=" + serverName);
                    _BuildStatement.Append(";Uid=" + userID);
                    _BuildStatement.Append(";Pwd=" + password);
                    break;
                case Enumeracion.ProveedorCadenaConexion.ORA_ODBC_MicrosoftForOracle_ConnectDirectly_BySID:
                    _BuildStatement.Append("Driver={Microsoft ODBC for Oracle}");
                    _BuildStatement.Append(";Server=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + serverName + ")(PORT=" + oraPort + "))(CONNECT_DATA=(SID=" + oraSID + ")))");
                    _BuildStatement.Append(";Uid=" + userID);
                    _BuildStatement.Append(";Pwd=" + password);
                    break;
                case Enumeracion.ProveedorCadenaConexion.ORA_ODBC_MicrosoftForOracle_WithOutTNS_ByServiceName:
                    _BuildStatement.Append("Driver={Microsoft ODBC for Oracle}");
                    _BuildStatement.Append(";CONNECTSTRING=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + serverName + ")(PORT=" + oraPort + "))(CONNECT_DATA=(SERVICE_NAME=" + oraServiceName + ")))");
                    _BuildStatement.Append(";Uid=" + userID);
                    _BuildStatement.Append(";Pwd=" + password);
                    break;

                default:
                    break;
            }
            return _BuildStatement.ToString();
        }

        [Obsolete]
        protected object Ejecutar(string _Statement, string _Parameters, Enumeracion.ObjetoBaseDatos _ObjetoBD, Enumeracion.ObjetoSalida _ObjetoSalida)
        {

            #region Variables y Chequeo de Parametros
            object _ReturnObject = new object();
            string _ConnectionString = GetConnectionString();
            string Parametros = null;
            if (!string.IsNullOrEmpty(_Parameters))
            {
                Parametros = Textos.GetParametrosSplit(_Parameters);
            }
            #endregion
            GetObjetoEjecutar(_Statement, _ObjetoBD, Parametros);
            _ReturnObject = GetProveedorBaseDatos(_ObjetoSalida, _ReturnObject, _BuildStatement, _ConnectionString);
            return _ReturnObject;
        }
        [Obsolete]
        private object GetProveedorBaseDatos(Enumeracion.ObjetoSalida _ObjetoSalida, object _ReturnObject, StringBuilder _BuildStatement, string _ConnectionString)
        {
            DataSet ds = new DataSet();
            switch (_ProviderDataBase)
            {
                case Enumeracion.DataBase.Provider.SqlClient:
                    _IDbConnection = new SqlConnection(_ConnectionString);
                    _IDbCommand = new SqlCommand(_BuildStatement.ToString(), _IDbConnection as SqlConnection);
                    _IDbConnection.Open();
                    switch (_ObjetoSalida)
                    {
                        case Enumeracion.ObjetoSalida.DataSet:
                            _IDbDataAdapter = new SqlDataAdapter(_IDbCommand as SqlCommand);
                            _IDbDataAdapter.Fill(ds);
                            _ReturnObject = (object)ds;
                            break;
                        case Enumeracion.ObjetoSalida.ExecuteNonQuery:
                            _IDbCommand.ExecuteNonQuery();
                            _ReturnObject = (object)"";
                            break;
                        case Enumeracion.ObjetoSalida.ExecuteScalar:
                            _ReturnObject = (object)_IDbCommand.ExecuteScalar();
                            break;
                        default:
                            break;
                    }
                    _IDbConnection.Close();
                    break;
                case Enumeracion.DataBase.Provider.OracleClient:
                    _IDbConnection = new OracleConnection(_ConnectionString);
                    _IDbCommand = new OracleCommand(_BuildStatement.ToString(), _IDbConnection as OracleConnection);
                    _IDbConnection.Open();
                    switch (_ObjetoSalida)
                    {
                        case Enumeracion.ObjetoSalida.DataSet:
                            _IDbDataAdapter = new OracleDataAdapter(_IDbCommand as OracleCommand);
                            _IDbDataAdapter.Fill(ds);
                            _ReturnObject = (object)ds;
                            break;
                        case Enumeracion.ObjetoSalida.ExecuteNonQuery:
                            _IDbCommand.ExecuteNonQuery();
                            _ReturnObject = (object)"";
                            break;
                        case Enumeracion.ObjetoSalida.ExecuteScalar:
                            _ReturnObject = (object)_IDbCommand.ExecuteScalar();
                            break;
                        default:
                            break;
                    }
                    _IDbConnection.Close();
                    break;
                case Enumeracion.DataBase.Provider.OleDb:
                    _IDbConnection = new OleDbConnection(_ConnectionString);
                    _IDbCommand = new OleDbCommand(_BuildStatement.ToString(), _IDbConnection as OleDbConnection);
                    _IDbConnection.Open();
                    switch (_ObjetoSalida)
                    {
                        case Enumeracion.ObjetoSalida.DataSet:
                            _IDbDataAdapter = new OleDbDataAdapter(_IDbCommand as OleDbCommand);
                            _IDbDataAdapter.Fill(ds);
                            _ReturnObject = (object)ds;
                            break;
                        case Enumeracion.ObjetoSalida.ExecuteNonQuery:
                            _IDbCommand.ExecuteNonQuery();
                            _ReturnObject = (object)"";
                            break;
                        case Enumeracion.ObjetoSalida.ExecuteScalar:
                            _ReturnObject = (object)_IDbCommand.ExecuteScalar();
                            break;
                        default:
                            break;
                    }
                    _IDbConnection.Close();
                    break;

                case Enumeracion.DataBase.Provider.ODBC:
                    _IDbConnection = new OdbcConnection(_ConnectionString);
                    _IDbCommand = new OdbcCommand(_BuildStatement.ToString(), _IDbConnection as OdbcConnection);
                    _IDbConnection.Open();
                    switch (_ObjetoSalida)
                    {
                        case Enumeracion.ObjetoSalida.DataSet:
                            _IDbDataAdapter = new OdbcDataAdapter(_IDbCommand as OdbcCommand);
                            _IDbDataAdapter.Fill(ds);
                            _ReturnObject = (object)ds;
                            break;
                        case Enumeracion.ObjetoSalida.ExecuteNonQuery:
                            _IDbCommand.ExecuteNonQuery();
                            _ReturnObject = (object)"";
                            break;
                        case Enumeracion.ObjetoSalida.ExecuteScalar:
                            _ReturnObject = (object)_IDbCommand.ExecuteScalar();
                            break;
                        default:
                            break;
                    }
                    _IDbConnection.Close();
                    break;
            }
            return _ReturnObject;
        }
        [Obsolete]
        private void GetObjetoEjecutar(string _Statement, Enumeracion.ObjetoBaseDatos _ObjetoBD, string Parametros)
        {
            _BuildStatement = new StringBuilder();
            switch (_ObjetoBD)
            {
                case Enumeracion.ObjetoBaseDatos.SQLStatement:
                    _BuildStatement.Append(_Statement);
                    break;
                case Enumeracion.ObjetoBaseDatos.SQLFunction:
                    _BuildStatement.Append("SELECT");
                    _BuildStatement.Append(" ");
                    _BuildStatement.Append(_Statement);
                    if (!string.IsNullOrEmpty(Parametros))
                    {
                        _BuildStatement.Append(" (" + Parametros + ") ");
                    }
                    break;
                case Enumeracion.ObjetoBaseDatos.SQLStoredProcedured:
                    _BuildStatement.Append("exec");
                    _BuildStatement.Append(" ");
                    _BuildStatement.Append(_Statement);
                    if (!string.IsNullOrEmpty(Parametros))
                    {
                        _BuildStatement.Append(" " + Parametros);
                    }
                    break;
                default:
                    break;
            }
        }

        [Obsolete]
        public static string ORAGenerarCadenaConexion(string serverName, string userID, string password)
        {
            _BuildStatement = new StringBuilder();
            _BuildStatement.Append("Data Source=" + serverName);
            _BuildStatement.Append(";User ID=" + userID);
            _BuildStatement.Append(";Password=" + password);
            return _BuildStatement.ToString();
        }
        [Obsolete]
        public static string ODBCGenerarCadenaConexion(string dnsName, string userName, string password)
        {
            _BuildStatement = new StringBuilder();
            _BuildStatement.Append("Dsn=" + dnsName);
            _BuildStatement.Append(";Uid=" + userName);
            _BuildStatement.Append(";Pwd=" + password);
            return _BuildStatement.ToString();
        }

        [Obsolete]
        public static string SQLGenerarCadenaConexion(string dataSource, string initialCatalog, bool persistSecurityInfo, string userId, string password)
        {
            _BuildStatement = new StringBuilder();
            _BuildStatement.Append("Data Source=" + dataSource);
            _BuildStatement.Append(";Initial Catalog=" + initialCatalog);
            _BuildStatement.Append(";Persist Security Info=" + persistSecurityInfo);
            _BuildStatement.Append(";User ID=" + userId);
            _BuildStatement.Append(";Password=" + password);
            return _BuildStatement.ToString();
        }

        [Obsolete]
        protected DataSet EjecutarConsulta(string _Cadena)
        {
            DataSet ds = new DataSet();
            string _ConnectionString = GetConnectionString();
            #region Switch con String
            switch (ProveedorBaseDatos)
            {
                case Enumeracion.ProveedorBaseDatos.SqlClient:
                    SqlConnection cnn1 = new SqlConnection(_ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(_Cadena, cnn1);
                    da1.Fill(ds);
                    return ds;

                case Enumeracion.ProveedorBaseDatos.OracleClient:
                    OracleConnection cnn2 = new OracleConnection(_ConnectionString);
                    OracleDataAdapter da2 = new OracleDataAdapter(_Cadena, cnn2);
                    da2.Fill(ds);
                    return ds;

                case Enumeracion.ProveedorBaseDatos.OleDb:
                    OleDbConnection cnn3 = new OleDbConnection(_ConnectionString);
                    OleDbDataAdapter da3 = new OleDbDataAdapter(_Cadena, cnn3);
                    da3.Fill(ds);
                    return ds;

                case Enumeracion.ProveedorBaseDatos.ODBC:
                    OdbcConnection cnn4 = new OdbcConnection(_ConnectionString);
                    OdbcDataAdapter da4 = new OdbcDataAdapter(_Cadena, cnn4);
                    da4.Fill(ds);
                    return ds;

                default:
                    return ds;

            }
            #endregion

        }
        [Obsolete]
        protected DataSet EjecutarConsulta(string _Cadena, bool _IsCnnVariable)
        {
            DataSet ds = new DataSet();
            string _CnnStringConexion = GetConnectionString();
            #region Switch con String
            switch (ProveedorBaseDatos)
            {
                case Enumeracion.ProveedorBaseDatos.SqlClient:
                    SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
                    SqlDataAdapter da1 = new SqlDataAdapter(_Cadena, cnn1);
                    da1.Fill(ds);
                    return ds;

                case Enumeracion.ProveedorBaseDatos.OracleClient:
                    OracleConnection cnn2 = new OracleConnection(_CnnStringConexion);
                    OracleDataAdapter da2 = new OracleDataAdapter(_Cadena, cnn2);
                    da2.Fill(ds);
                    return ds;

                case Enumeracion.ProveedorBaseDatos.OleDb:
                    OleDbConnection cnn3 = new OleDbConnection(_CnnStringConexion);
                    OleDbDataAdapter da3 = new OleDbDataAdapter(_Cadena, cnn3);
                    da3.Fill(ds);
                    return ds;

                case Enumeracion.ProveedorBaseDatos.ODBC:
                    OdbcConnection cnn4 = new OdbcConnection(_CnnStringConexion);
                    OdbcDataAdapter da4 = new OdbcDataAdapter(_Cadena, cnn4);
                    da4.Fill(ds);
                    return ds;

                default:
                    return ds;

            }
            #endregion

        }
        [Obsolete]
        protected DataSet EjecutarConsulta(string _StoreProcedure, string _Parametros)
        {
            string Parametros = Textos.GetParametrosSplit(_Parametros);
            string _CnnStringConexion = GetConnectionString();
            DataSet ds = new DataSet();
            #region Switch con Propiedad String
            switch (ProveedorBaseDatos)
            {
                case Enumeracion.ProveedorBaseDatos.SqlClient:
                    SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
                    SqlDataAdapter da1 = new SqlDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn1);
                    da1.Fill(ds);
                    return ds;

                case Enumeracion.ProveedorBaseDatos.OracleClient:
                    OracleConnection cnn2 = new OracleConnection(_CnnStringConexion);
                    OracleDataAdapter da2 = new OracleDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn2);
                    da2.Fill(ds);
                    return ds;

                case Enumeracion.ProveedorBaseDatos.OleDb:
                    OleDbConnection cnn3 = new OleDbConnection(_CnnStringConexion);
                    OleDbDataAdapter da3 = new OleDbDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn3);
                    da3.Fill(ds);
                    return ds;

                default:
                    OdbcConnection cnn4 = new OdbcConnection(_CnnStringConexion);
                    OdbcDataAdapter da4 = new OdbcDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn4);
                    da4.Fill(ds);
                    return ds;
            }
            #endregion
        }
        [Obsolete]
        protected DataSet EjecutarConsulta(string _StoreProcedure, string _Parametros, bool _IsCnnVariable)
        {
            string Parametros = Textos.GetParametrosSplit(_Parametros);
            string _CnnStringConexion = GetConnectionString();
            DataSet ds = new DataSet();
            #region Switch con Propiedad String
            switch (ProveedorBaseDatos)
            {
                case Enumeracion.ProveedorBaseDatos.SqlClient:
                    SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
                    SqlDataAdapter da1 = new SqlDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn1);
                    da1.Fill(ds);
                    return ds;

                case Enumeracion.ProveedorBaseDatos.OracleClient:
                    OracleConnection cnn2 = new OracleConnection(_CnnStringConexion);
                    OracleDataAdapter da2 = new OracleDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn2);
                    da2.Fill(ds);
                    return ds;

                case Enumeracion.ProveedorBaseDatos.OleDb:
                    OleDbConnection cnn3 = new OleDbConnection(_CnnStringConexion);
                    OleDbDataAdapter da3 = new OleDbDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn3);
                    da3.Fill(ds);
                    return ds;

                default:
                    OdbcConnection cnn4 = new OdbcConnection(_CnnStringConexion);
                    OdbcDataAdapter da4 = new OdbcDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn4);
                    da4.Fill(ds);
                    return ds;
            }
            #endregion
        }
        /// <summary>
        /// Este metodo recibe dos parametros, para su ejecución, no pueden ser NULL
        /// </summary>
        /// <param name="_StoreProcedure">String que indica el nombre del StoredProcedure.</param>
        /// <param name="_Parametros">String que indican los parametros del StoredProcedure, si son más de uno (1), deben estar separados con el mismo separador identificado en la propiedad BdConexion.Separador</param>
        /// <returns>Dataset</returns>
        //[Obsolete]
        //protected DataSet EjecutarConsulta(string _StoreProcedure, Filtro _Filtro)
        //{
        //    string Parametros = Utils.GetParametrosSplitWithName(_Filtro.GetValores(_Filtro));
        //    DataSet ds = new DataSet();
        //    string _CnnStringConexion = GetConnectionString();
        //    #region Switch con Propiedad String
        //    switch (ProveedorBaseDatos)
        //    {
        //        case Enumeracion.ProveedorBaseDatos.SqlClient:
        //            SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
        //            SqlDataAdapter da1 = new SqlDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn1);
        //            da1.Fill(ds);
        //            return ds;

        //        case Enumeracion.ProveedorBaseDatos.OracleClient:
        //            OracleConnection cnn2 = new OracleConnection(_CnnStringConexion);
        //            OracleDataAdapter da2 = new OracleDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn2);
        //            da2.Fill(ds);
        //            return ds;

        //        case Enumeracion.ProveedorBaseDatos.OleDb:
        //            OleDbConnection cnn3 = new OleDbConnection(_CnnStringConexion);
        //            OleDbDataAdapter da3 = new OleDbDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn3);
        //            da3.Fill(ds);
        //            return ds;

        //        default:
        //            OdbcConnection cnn4 = new OdbcConnection(_CnnStringConexion);
        //            OdbcDataAdapter da4 = new OdbcDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn4);
        //            da4.Fill(ds);
        //            return ds;
        //    }
        //    #endregion
        //}
        /// <summary>
        /// Este metodo recibe dos parametros, para su ejecución, no pueden ser NULL
        /// </summary>
        /// <param name="_StoreProcedure">String que indica el nombre del StoredProcedure.</param>
        /// <param name="_Parametros">String que indican los parametros del StoredProcedure, si son más de uno (1), deben estar separados con el mismo separador identificado en la propiedad BdConexion.Separador</param>
        /// <returns>Dataset</returns>
        [Obsolete]
        protected DataSet EjecutarConsulta(string _StoreProcedure, string _Cursor, string _ParamName, string _ParamValue)
        {
            string Parametros = Textos.GetParametrosSplit(_ParamName);
            DataSet ds = new DataSet();
            string _CnnStringConexion = GetConnectionString();
            switch (ProveedorBaseDatos)
            {
                case Enumeracion.ProveedorBaseDatos.SqlClient:
                    SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
                    SqlDataAdapter da1 = new SqlDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn1);
                    da1.Fill(ds);
                    return ds;

                case Enumeracion.ProveedorBaseDatos.OracleClient:
                    OracleConnection cnn2 = new OracleConnection(_CnnStringConexion);
                    OracleCommand cmd2 = new OracleCommand();
                    cmd2.Connection = cnn2;
                    cmd2.CommandText = _StoreProcedure;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    if (!string.IsNullOrEmpty(_ParamName))
                    {
                        cmd2.Parameters.Add(_ParamName, OracleType.Number).Value = _ParamValue;
                    }
                    cmd2.Parameters.Add(_Cursor, OracleType.Cursor).Direction = ParameterDirection.Output;
                    OracleDataAdapter da2 = new OracleDataAdapter(cmd2);
                    da2.Fill(ds);
                    return ds;

                case Enumeracion.ProveedorBaseDatos.OleDb:
                    OleDbConnection cnn3 = new OleDbConnection(_CnnStringConexion);
                    OleDbDataAdapter da3 = new OleDbDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn3);
                    da3.Fill(ds);
                    return ds;

                default:
                    OdbcConnection cnn4 = new OdbcConnection(_CnnStringConexion);
                    OdbcDataAdapter da4 = new OdbcDataAdapter("exec " + _StoreProcedure + " " + Parametros, cnn4);
                    da4.Fill(ds);
                    return ds;
            }


        }
        /// <summary>
        /// (OBSOLETA) Este metodo recibe dos parametros, para su ejecución, no pueden ser NULL
        /// </summary>
        /// <param name="_StoreProcedure">String que indica el nombre del StoredProcedure.</param>
        /// <param name="_Parametros">String que indican los parametros del StoredProcedure, si son más de uno (1), deben estar separados con el mismo separador identificado en la propiedad BdConexion.Separador</param>
        /// <returns>Dataset</returns>

        /// <summary>
        /// Este metodo recibe dos parametros, para su ejecución, no pueden ser NULL
        /// </summary>
        /// <param name="_StoreProcedure">String que indica el nombre del StoredProcedure.</param>
        /// <param name="_Parametros">String que indican los parametros del StoredProcedure, si son más de uno (1), deben estar separados con el mismo separador identificado en la propiedad BdConexion.Separador</param>
        /// <returns></returns>
        [Obsolete]
        protected void EjecutarComando(string _Cadena, bool _IsCnnVariable)
        {
            string _CnnStringConexion = GetConnectionString();
            switch (ProveedorBaseDatos)
            {
                case Enumeracion.ProveedorBaseDatos.SqlClient:
                    SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd1 = new SqlCommand(_Cadena, cnn1);
                    cnn1.Open();
                    cmd1.ExecuteNonQuery();
                    cnn1.Close();
                    break;

                case Enumeracion.ProveedorBaseDatos.OracleClient:
                    OracleConnection cnn2 = new OracleConnection(_CnnStringConexion);
                    OracleCommand cmd2 = new OracleCommand(_Cadena, cnn2);
                    cnn2.Open();
                    cmd2.ExecuteNonQuery();
                    cnn2.Close();
                    break;

                case Enumeracion.ProveedorBaseDatos.OleDb:
                    OleDbConnection cnn3 = new OleDbConnection(_CnnStringConexion);
                    OleDbCommand cmd3 = new OleDbCommand(_Cadena, cnn3);
                    cnn3.Open();
                    cmd3.ExecuteNonQuery();
                    cnn3.Close();
                    break;

                default:
                    OleDbConnection cnn4 = new OleDbConnection(_CnnStringConexion);
                    OleDbCommand cmd4 = new OleDbCommand(_Cadena, cnn4);
                    cnn4.Open();
                    cmd4.ExecuteNonQuery();
                    cnn4.Close();
                    break;
            }
        }
        /// <summary>
        /// Este metodo recibe dos parametros, para su ejecución, no pueden ser NULL
        /// </summary>
        /// <param name="_StoreProcedure">String que indica el nombre del StoredProcedure.</param>
        /// <param name="_Parametros">String que indican los parametros del StoredProcedure, si son más de uno (1), deben estar separados con el mismo separador identificado en la propiedad BdConexion.Separador</param>
        /// <returns></returns>
        [Obsolete]
        protected void EjecutarComando(string _StoreProcedure, string _Parametros)
        {
            string Parametros = Textos.GetParametrosSplit(_Parametros);
            string _CnnStringConexion = GetConnectionString();
            switch (ProveedorBaseDatos)
            {
                case Enumeracion.ProveedorBaseDatos.SqlClient:
                    SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd1 = new SqlCommand("exec " + _StoreProcedure + " " + Parametros, cnn1);
                    cnn1.Open();
                    cmd1.ExecuteNonQuery();
                    cnn1.Close();
                    break;

                case Enumeracion.ProveedorBaseDatos.OracleClient:
                    OracleConnection cnn2 = new OracleConnection(_CnnStringConexion);
                    OracleCommand cmd2 = new OracleCommand("exec " + _StoreProcedure + " " + Parametros, cnn2);
                    cnn2.Open();
                    cmd2.ExecuteNonQuery();
                    cnn2.Close();
                    break;

                case Enumeracion.ProveedorBaseDatos.OleDb:
                    OleDbConnection cnn3 = new OleDbConnection(_CnnStringConexion);
                    OleDbCommand cmd3 = new OleDbCommand("exec " + _StoreProcedure + " " + Parametros, cnn3);
                    cnn3.Open();
                    cmd3.ExecuteNonQuery();
                    cnn3.Close();
                    break;

                default:
                    OleDbConnection cnn4 = new OleDbConnection(_CnnStringConexion);
                    OleDbCommand cmd4 = new OleDbCommand("exec " + _StoreProcedure + " " + Parametros, cnn4);
                    cnn4.Open();
                    cmd4.ExecuteNonQuery();
                    cnn4.Close();
                    break;
            }
        }
        /// <summary>
        /// Este metodo recibe dos parametros, para su ejecución, no pueden ser NULL
        /// </summary>
        /// <param name="_StoreProcedure">String que indica el nombre del StoredProcedure.</param>
        /// <param name="_Parametros">String que indican los parametros del StoredProcedure, si son más de uno (1), deben estar separados con el mismo separador identificado en la propiedad BdConexion.Separador</param>
        /// <returns></returns>
        [Obsolete]
        protected void EjecutarComando(string _StoreProcedure, string _Parametros, bool _IsCnnVariable)
        {
            string Parametros = Textos.GetParametrosSplit(_Parametros);
            string _CnnStringConexion = GetConnectionString();
            switch (ProveedorBaseDatos)
            {
                case Enumeracion.ProveedorBaseDatos.SqlClient:
                    SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd1 = new SqlCommand("exec " + _StoreProcedure + " " + Parametros, cnn1);
                    cnn1.Open();
                    cmd1.ExecuteNonQuery();
                    cnn1.Close();
                    break;

                case Enumeracion.ProveedorBaseDatos.OracleClient:
                    OracleConnection cnn2 = new OracleConnection(_CnnStringConexion);
                    OracleCommand cmd2 = new OracleCommand("exec " + _StoreProcedure + " " + Parametros, cnn2);
                    cnn2.Open();
                    cmd2.ExecuteNonQuery();
                    cnn2.Close();
                    break;

                case Enumeracion.ProveedorBaseDatos.OleDb:
                    OleDbConnection cnn3 = new OleDbConnection(_CnnStringConexion);
                    OleDbCommand cmd3 = new OleDbCommand("exec " + _StoreProcedure + " " + Parametros, cnn3);
                    cnn3.Open();
                    cmd3.ExecuteNonQuery();
                    cnn3.Close();
                    break;

                default:
                    OleDbConnection cnn4 = new OleDbConnection(_CnnStringConexion);
                    OleDbCommand cmd4 = new OleDbCommand("exec " + _StoreProcedure + " " + Parametros, cnn4);
                    cnn4.Open();
                    cmd4.ExecuteNonQuery();
                    cnn4.Close();
                    break;
            }
        }

        #region Funciones SQL
        /// <summary>
        /// (OBSOLETA) Ejecuta un funcion 
        /// </summary>
        /// <param name="_StoreFunction"> el nombre de la funcion a ejecutar</param>
        /// <param name="_Parametros"> parametros de la funcion</param>
        /// <returns></returns>
        //protected string EjecutarFuncion(string _StoreFunction, string _Parametros)
        //{
        //    string Parametros = Utils.GetParametrosSplit(_Parametros);
        //    string _CnnStringConexion = "";
        //    #region Switch con Propiedad String
        //    switch (ProveedorBaseDatos)
        //    {
        //        case Enumeracion.ProveedorBaseDatos.SqlClient:
        //            SqlConnection cnn1 = new SqlConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
        //            cnn1.Open();
        //            SqlCommand cmd = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn1);
        //            return (string)cmd.ExecuteScalar();

        //        case Enumeracion.ProveedorBaseDatos.OracleClient:
        //            //OracleConnection cnn2 = new OracleConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
        //            SqlConnection cnn2 = new SqlConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
        //            SqlCommand cmd1 = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn2);
        //            return (string)cmd1.ExecuteScalar();

        //        case Enumeracion.ProveedorBaseDatos.OleDb:
        //            // OleDbConnection cnn3 = new OleDbConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
        //            SqlConnection cnn3 = new SqlConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
        //            SqlCommand cmd2 = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn3);
        //            return (string)cmd2.ExecuteScalar();

        //        default:
        //            // OdbcConnection cnn4 = new OdbcConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
        //            SqlConnection cnn4 = new SqlConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
        //            SqlCommand cmd3 = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn4);
        //            return (string)cmd3.ExecuteScalar();
        //    }
        //    #endregion
        //}
        /// <summary>
        /// (OBSOLETA) Ejecuta un funcion 
        /// </summary>
        /// <param name="_StoreFunction"> el nombre de la funcion a ejecutar</param>
        /// <param name="_Parametros"> parametros de la funcion</param>
        /// <returns></returns>
        [Obsolete]
        protected object EjecutarFuncion(string _StoreFunction, string _Parametros)
        {
            string Parametros = Textos.GetParametrosSplit(_Parametros);
            string _CnnStringConexion = GetConnectionString();
            #region Switch con Propiedad String
            switch (ProveedorBaseDatos)
            {
                case Enumeracion.ProveedorBaseDatos.SqlClient:
                    SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
                    cnn1.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn1);
                    return (object)cmd.ExecuteScalar();

                case Enumeracion.ProveedorBaseDatos.OracleClient:
                    //OracleConnection cnn2 = new OracleConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
                    SqlConnection cnn2 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd1 = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn2);
                    return (object)cmd1.ExecuteScalar();

                case Enumeracion.ProveedorBaseDatos.OleDb:
                    // OleDbConnection cnn3 = new OleDbConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
                    SqlConnection cnn3 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd2 = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn3);
                    return (object)cmd2.ExecuteScalar();

                default:
                    // OdbcConnection cnn4 = new OdbcConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
                    SqlConnection cnn4 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd3 = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn4);
                    return (object)cmd3.ExecuteScalar();
            }
            #endregion
        }
        ///// <summary>
        ///// Ejecuta un funcion 
        ///// </summary>
        ///// <param name="_StoreFunction"> el nombre de la funcion a ejecutar</param>
        ///// <param name="_Parametros"> parametros de la funcion</param>
        ///// <returns></returns>
        //protected string EjecutarFuncion(string _StoreFunction, string _Parametros, bool _IsCnnVariable)
        //{
        //    string Parametros = Utils.GetParametrosSplit(_Parametros);
        //    string _CnnStringConexion = "";
        //    #region Tipo Cadena de Conexion
        //    switch (TipoCadenaConexion)
        //    {
        //        case TipoCnn.ConnectionString:
        //            {
        //                _CnnStringConexion = ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString();
        //                break;
        //            }
        //        case TipoCnn.AppSettings:
        //            {
        //                _CnnStringConexion = ConfigurationManager.AppSettings[KeyNameAppWebConfig].ToString();
        //                break;
        //            }
        //        case TipoCnn.String:
        //            {
        //                _CnnStringConexion = CadenaConexion;
        //                break;
        //            }
        //        default:
        //            {
        //                _CnnStringConexion = "";
        //                break;
        //            }
        //    }
        //    #endregion
        //    #region Switch con Propiedad String
        //    switch (ProveedorBaseDatos)
        //    {
        //        case Enumeracion.ProveedorBaseDatos.SqlClient:
        //            SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
        //            cnn1.Open();
        //            SqlCommand cmd = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn1);
        //            return (string)cmd.ExecuteScalar();

        //        case Enumeracion.ProveedorBaseDatos.OracleClient:
        //            //OracleConnection cnn2 = new OracleConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
        //            SqlConnection cnn2 = new SqlConnection(_CnnStringConexion);
        //            SqlCommand cmd1 = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn2);
        //            return (string)cmd1.ExecuteScalar();

        //        case Enumeracion.ProveedorBaseDatos.OleDb:
        //            // OleDbConnection cnn3 = new OleDbConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
        //            SqlConnection cnn3 = new SqlConnection(_CnnStringConexion);
        //            SqlCommand cmd2 = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn3);
        //            return (string)cmd2.ExecuteScalar();

        //        default:
        //            // OdbcConnection cnn4 = new OdbcConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
        //            SqlConnection cnn4 = new SqlConnection(_CnnStringConexion);
        //            SqlCommand cmd3 = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn4);
        //            return (string)cmd3.ExecuteScalar();
        //    }
        //    #endregion
        //}
        /// <summary>
        /// Ejecuta un funcion 
        /// </summary>
        /// <param name="_StoreFunction"> el nombre de la funcion a ejecutar</param>
        /// <param name="_Parametros"> parametros de la funcion</param>
        /// <returns></returns>
        [Obsolete]
        protected object EjecutarFuncion(string _StoreFunction, string _Parametros, bool _IsCnnVariable)
        {
            string Parametros = Textos.GetParametrosSplit(_Parametros);
            string _CnnStringConexion = GetConnectionString();
            #region Switch con Propiedad String
            switch (ProveedorBaseDatos)
            {
                case Enumeracion.ProveedorBaseDatos.SqlClient:
                    SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
                    cnn1.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn1);
                    return (object)cmd.ExecuteScalar();

                case Enumeracion.ProveedorBaseDatos.OracleClient:
                    //OracleConnection cnn2 = new OracleConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
                    SqlConnection cnn2 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd1 = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn2);
                    return (object)cmd1.ExecuteScalar();

                case Enumeracion.ProveedorBaseDatos.OleDb:
                    // OleDbConnection cnn3 = new OleDbConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
                    SqlConnection cnn3 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd2 = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn3);
                    return (object)cmd2.ExecuteScalar();

                default:
                    // OdbcConnection cnn4 = new OdbcConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
                    SqlConnection cnn4 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd3 = new SqlCommand(@"SELECT " + _StoreFunction + "(" + Parametros + ")", cnn4);
                    return (object)cmd3.ExecuteScalar();
            }
            #endregion
        }
        /// <summary>
        /// (OBSOLETA) Ejecuta un funcion 
        /// </summary>
        /// <param name="_StoreFunction"> el nombre de la funcion a ejecutar</param>
        /// <param name="_Parametros"> parametros de la funcion</param>
        /// <returns></returns>
        [Obsolete]
        protected object EjecutarFuncion(string _StoreFunction)
        {
            string _CnnStringConexion = GetConnectionString();
            #region Switch con Propiedad String
            switch (ProveedorBaseDatos)
            {
                case Enumeracion.ProveedorBaseDatos.SqlClient:
                    SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
                    cnn1.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT " + _StoreFunction, cnn1);
                    return (object)cmd.ExecuteScalar();

                case Enumeracion.ProveedorBaseDatos.OracleClient:
                    //OracleConnection cnn2 = new OracleConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
                    SqlConnection cnn2 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd1 = new SqlCommand(@"SELECT " + _StoreFunction, cnn2);
                    return (object)cmd1.ExecuteScalar();

                case Enumeracion.ProveedorBaseDatos.OleDb:
                    // OleDbConnection cnn3 = new OleDbConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
                    SqlConnection cnn3 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd2 = new SqlCommand(@"SELECT " + _StoreFunction, cnn3);
                    return (object)cmd2.ExecuteScalar();

                default:
                    // OdbcConnection cnn4 = new OdbcConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
                    SqlConnection cnn4 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd3 = new SqlCommand(@"SELECT " + _StoreFunction, cnn4);
                    return (object)cmd3.ExecuteScalar();
            }
            #endregion
        }
        /// <summary>
        /// Ejecuta un funcion 
        /// </summary>
        /// <param name="_StoreFunction"> el nombre de la funcion a ejecutar</param>
        /// <param name="_Parametros"> parametros de la funcion</param>
        /// <returns></returns>
        [Obsolete]
        protected object EjecutarFuncion(string _StoreFunction, bool _IsCnnVariable)
        {
            string _CnnStringConexion = GetConnectionString();
            #region Switch con Propiedad String
            switch (ProveedorBaseDatos)
            {
                case Enumeracion.ProveedorBaseDatos.SqlClient:
                    SqlConnection cnn1 = new SqlConnection(_CnnStringConexion);
                    cnn1.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT " + _StoreFunction, cnn1);
                    return (object)cmd.ExecuteScalar();

                case Enumeracion.ProveedorBaseDatos.OracleClient:
                    //OracleConnection cnn2 = new OracleConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
                    SqlConnection cnn2 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd1 = new SqlCommand(@"SELECT " + _StoreFunction, cnn2);
                    return (object)cmd1.ExecuteScalar();

                case Enumeracion.ProveedorBaseDatos.OleDb:
                    // OleDbConnection cnn3 = new OleDbConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
                    SqlConnection cnn3 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd2 = new SqlCommand(@"SELECT " + _StoreFunction, cnn3);
                    return (object)cmd2.ExecuteScalar();

                default:
                    // OdbcConnection cnn4 = new OdbcConnection(ConfigurationManager.ConnectionStrings[KeyNameAppWebConfig].ToString());
                    SqlConnection cnn4 = new SqlConnection(_CnnStringConexion);
                    SqlCommand cmd3 = new SqlCommand(@"SELECT " + _StoreFunction, cnn4);
                    return (object)cmd3.ExecuteScalar();
            }
            #endregion
        }
        #endregion
        #endregion
        #endregion
        #region Propiedades
        /// <summary>
        /// Describe el proveedor de la conexión de la Base de Datos, que se esta usando, no puede ser NULL
        /// </summary>
        /// <value>String, existen 4 tipos actualmente: SqlClient, OracleClient, Odbc, OleDb</value>
        [Obsolete]
        public static Enumeracion.ProveedorBaseDatos ProveedorBaseDatos { get { return _ProveedorBaseDatos; } set { _ProveedorBaseDatos = value; } }
        public static Enumeracion.DataBase.Provider ProviderDataBase { get { return _ProviderDataBase; } set { _ProviderDataBase = value; } }
        [Obsolete]
        public static Enumeracion.ModoConexion ModoConexion { get { return _ModoConexion; } set { _ModoConexion = value; } }
        public static Enumeracion.DataBase.ConnectionMode ConnectionMode { get { return _ConnectionMode; } set { _ConnectionMode = value; } }
        /// <summary>
        /// Describe el carácter usado como separador de los parametros asignado a un StoreProcedure, no puede ser NULL
        /// </summary>
        /// <value>String, El valor usado en este caso es la coma (,)</value>
        [Obsolete]
        public static Enumeracion.Separador Separador
        {
            get { return _Separador; }
            set { _Separador = value; }
        }
        public static Enumeracion.DataBase.Separator Separator
        {
            get { return _Separator; }
            set { _Separator = value; }
        }
        /// <summary>
        /// Describe el nombre de la cadena de conexión usada en el Web.config y/o App.config, no puede ser NULL
        /// </summary>
        /// <value>String</value>
        public static string KeyNameAppWebConfig
        {
            get { return _KeyNameAppWebConfig; }
            set { _KeyNameAppWebConfig = value; }
        }
        /// <summary>
        /// String con la cadena de conexión para la base de datos
        /// </summary>
        [Obsolete]
        public static string CadenaConexion
        {
            get { return _CadenaConexion; }
            set { _CadenaConexion = value; }
        }
        public static string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }

        #endregion
    }
}
