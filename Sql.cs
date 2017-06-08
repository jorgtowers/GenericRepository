using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Utils.BaseDatos.ADONET
{
    public sealed class Sql:BdConexion  
    {
        private StringBuilder _SyntaxStoredProcedure;
        private string _Columns;
        public int ObtenerId() 
        {
            DataSet ds=EjecutarConsulta("spGetId");
            return Convert.ToInt16(ds.Tables[0].Rows[0][0]);
        }
        public string GetId() { return "Select @@IDENTITY"; }
        public void CerrarConexiones()
        {
            SqlConnection.ClearAllPools();
        }
        public DataSet ObtenerInfoTablas(string _Tabla) 
        {
            return EjecutarConsulta("SELECT TOP (100) PERCENT TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, ORDINAL_POSITION, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH, NUMERIC_PRECISION, NUMERIC_PRECISION_RADIX, NUMERIC_SCALE, DATETIME_PRECISION, CHARACTER_SET_CATALOG, CHARACTER_SET_SCHEMA, CHARACTER_SET_NAME, COLLATION_CATALOG, COLLATION_SCHEMA, COLLATION_NAME, DOMAIN_CATALOG, DOMAIN_SCHEMA, DOMAIN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME LIKE '%" + _Tabla.Trim() + "%' ORDER BY TABLE_NAME,COLUMN_NAME");
        }
        public DataSet ObtenerInfoNombreTablas()
        {
            return EjecutarConsulta("SELECT DISTINCT TOP (100) PERCENT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS ORDER BY TABLE_NAME");
        }
        public DataSet ObtenerInfoNombreColumnas(string _Tabla) {
            return EjecutarConsulta("SELECT DISTINCT TOP (100) PERCENT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE, ORDINAL_POSITION FROM         INFORMATION_SCHEMA.COLUMNS WHERE (TABLE_NAME LIKE '"+_Tabla.Trim()+"') ORDER BY ORDINAL_POSITION");
        }
        public DataSet ObtenerInfoTablas(string _Tabla,string _Column)
        {
            return EjecutarConsulta("SELECT TOP (100) PERCENT TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, ORDINAL_POSITION, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH, NUMERIC_PRECISION, NUMERIC_PRECISION_RADIX, NUMERIC_SCALE, DATETIME_PRECISION, CHARACTER_SET_CATALOG, CHARACTER_SET_SCHEMA, CHARACTER_SET_NAME, COLLATION_CATALOG, COLLATION_SCHEMA, COLLATION_NAME, DOMAIN_CATALOG, DOMAIN_SCHEMA, DOMAIN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME LIKE '%" + _Tabla.Trim() + "%' AND COLUMN_NAME LIKE '%" + _Column.Trim() + "%' ORDER BY TABLE_NAME,COLUMN_NAME ");
        }
        public string CrearStoredProcedureObtener(bool _WithWhere)
        {
            _SyntaxStoredProcedure= new StringBuilder();
            if (_WithWhere == true)
            {
                DataSet dsTables = ObtenerInfoNombreTablas();
                int i = dsTables.Tables[0].Rows.Count;
                for (int i2 = 0; i2 < i; i2++)
                {
                    _SyntaxStoredProcedure.AppendLine("-- ========================= INI ===========================");
                    _SyntaxStoredProcedure.AppendLine("SET ANSI_NULLS ON");
                    _SyntaxStoredProcedure.AppendLine("GO");
                    _SyntaxStoredProcedure.AppendLine("SET QUOTED_IDENTIFIER ON");
                    _SyntaxStoredProcedure.AppendLine("GO");
                    _SyntaxStoredProcedure.AppendLine("-- =========================================================");
                    _SyntaxStoredProcedure.AppendLine("-- Plantilla generada automáticamente por Jorge L. Torres A.");
                    _SyntaxStoredProcedure.AppendLine("-- =========================================================");
                    _SyntaxStoredProcedure.AppendLine("-- Author     :	Jorge L. Torres A.");
                    _SyntaxStoredProcedure.AppendLine("-- Create date: " + DateTime.Now.ToLongDateString());
                    _SyntaxStoredProcedure.AppendLine("-- Description:	Obtener " + dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    _SyntaxStoredProcedure.AppendLine("-- =========================================================");
                    _SyntaxStoredProcedure.AppendLine("CREATE PROCEDURE spGet" + dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    DataSet dsColumns = ObtenerInfoNombreColumnas(dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    int o = dsColumns.Tables[0].Rows.Count;
                    _SyntaxStoredProcedure.AppendLine(" @_" + dsColumns.Tables[0].Rows[0]["COLUMN_NAME"].ToString().ToLower() + " " + dsColumns.Tables[0].Rows[0]["DATA_TYPE"].ToString().ToLowerInvariant());
                    _SyntaxStoredProcedure.AppendLine("AS");
                    _SyntaxStoredProcedure.AppendLine("BEGIN");
                    _SyntaxStoredProcedure.AppendLine(" SELECT ");
                    _Columns = "";
                    for (int o2 = 0; o2 < o; o2++)
                    {
                        _Columns += dsColumns.Tables[0].Rows[o2]["COLUMN_NAME"].ToString().ToLower() + ",";
                    }
                    _SyntaxStoredProcedure.AppendLine(" " + _Columns.Substring(0, _Columns.LastIndexOf(",")));
                    _SyntaxStoredProcedure.AppendLine(" FROM " + dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    _SyntaxStoredProcedure.AppendLine(" WHERE " + dsColumns.Tables[0].Rows[0]["COLUMN_NAME"].ToString().ToLower() + " = @_" + dsColumns.Tables[0].Rows[0]["COLUMN_NAME"].ToString().ToLower());
                    _SyntaxStoredProcedure.AppendLine("END");
                    _SyntaxStoredProcedure.AppendLine("GO");
                    _SyntaxStoredProcedure.AppendLine("-- ========================= FIN ===========================");
                }
            }
            else
            {
                DataSet dsTables = ObtenerInfoNombreTablas();
                int i = dsTables.Tables[0].Rows.Count;
                for (int i2 = 0; i2 < i; i2++)
                {
                    _SyntaxStoredProcedure.AppendLine("-- ========================= INI ===========================");
                    _SyntaxStoredProcedure.AppendLine("SET ANSI_NULLS ON");
                    _SyntaxStoredProcedure.AppendLine("GO");
                    _SyntaxStoredProcedure.AppendLine("SET QUOTED_IDENTIFIER ON");
                    _SyntaxStoredProcedure.AppendLine("GO");
                    _SyntaxStoredProcedure.AppendLine("-- =========================================================");
                    _SyntaxStoredProcedure.AppendLine("-- Plantilla generada automáticamente por Jorge L. Torres A.");
                    _SyntaxStoredProcedure.AppendLine("-- =========================================================");
                    _SyntaxStoredProcedure.AppendLine("-- Author     :	Jorge L. Torres A.");
                    _SyntaxStoredProcedure.AppendLine("-- Create date: " + DateTime.Now.ToLongDateString());
                    _SyntaxStoredProcedure.AppendLine("-- Description:	Obtener Full " + dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    _SyntaxStoredProcedure.AppendLine("-- =========================================================");
                    _SyntaxStoredProcedure.AppendLine("CREATE PROCEDURE spGetFull" + dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    DataSet dsColumns = ObtenerInfoNombreColumnas(dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    int o = dsColumns.Tables[0].Rows.Count;
                    _SyntaxStoredProcedure.AppendLine("AS");
                    _SyntaxStoredProcedure.AppendLine("BEGIN");
                    _SyntaxStoredProcedure.AppendLine(" SELECT ");
                    _Columns = "";
                    for (int o2 = 0; o2 < o; o2++)
                    {
                        _Columns += dsColumns.Tables[0].Rows[o2]["COLUMN_NAME"].ToString().ToLower() + ",";
                    }
                    _SyntaxStoredProcedure.AppendLine(" " + _Columns.Substring(0, _Columns.LastIndexOf(",")));
                    _SyntaxStoredProcedure.AppendLine(" FROM " + dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    _SyntaxStoredProcedure.AppendLine("END");
                    _SyntaxStoredProcedure.AppendLine("GO");
                    _SyntaxStoredProcedure.AppendLine("-- ========================= FIN ===========================");
                }
            }
            return _SyntaxStoredProcedure.ToString();
        }
        public string CrearStoredProcedureEliminar(bool _WithWhere)
        {
            _SyntaxStoredProcedure = new StringBuilder();
            if (_WithWhere == true)
            {
                DataSet dsTables = ObtenerInfoNombreTablas();
                int i = dsTables.Tables[0].Rows.Count;
                for (int i2 = 0; i2 < i; i2++)
                {
                    _SyntaxStoredProcedure.AppendLine("-- ========================= INI ===========================");
                    _SyntaxStoredProcedure.AppendLine("SET ANSI_NULLS ON");
                    _SyntaxStoredProcedure.AppendLine("GO");
                    _SyntaxStoredProcedure.AppendLine("SET QUOTED_IDENTIFIER ON");
                    _SyntaxStoredProcedure.AppendLine("GO");
                    _SyntaxStoredProcedure.AppendLine("-- =========================================================");
                    _SyntaxStoredProcedure.AppendLine("-- Plantilla generada automáticamente por Jorge L. Torres A.");
                    _SyntaxStoredProcedure.AppendLine("-- =========================================================");
                    _SyntaxStoredProcedure.AppendLine("-- Author     :	Jorge L. Torres A.");
                    _SyntaxStoredProcedure.AppendLine("-- Create date: " + DateTime.Now.ToLongDateString());
                    _SyntaxStoredProcedure.AppendLine("-- Description:	Eliminar " + dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    _SyntaxStoredProcedure.AppendLine("-- =========================================================");
                    _SyntaxStoredProcedure.AppendLine("CREATE PROCEDURE spDel" + dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    DataSet dsColumns = ObtenerInfoNombreColumnas(dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    int o = dsColumns.Tables[0].Rows.Count;
                    _SyntaxStoredProcedure.AppendLine(" @_" + dsColumns.Tables[0].Rows[0]["COLUMN_NAME"].ToString().ToLower() + " " + dsColumns.Tables[0].Rows[0]["DATA_TYPE"].ToString().ToLowerInvariant());
                    _SyntaxStoredProcedure.AppendLine("AS");
                    _SyntaxStoredProcedure.AppendLine("BEGIN");
                    _SyntaxStoredProcedure.AppendLine(" DELETE");
                    _SyntaxStoredProcedure.AppendLine(" FROM " + dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    _SyntaxStoredProcedure.AppendLine(" WHERE " + dsColumns.Tables[0].Rows[0]["COLUMN_NAME"].ToString().ToLower() + " = @_" + dsColumns.Tables[0].Rows[0]["COLUMN_NAME"].ToString().ToLower());
                    _SyntaxStoredProcedure.AppendLine("END");
                    _SyntaxStoredProcedure.AppendLine("GO");
                    _SyntaxStoredProcedure.AppendLine("-- ========================= FIN ===========================");
                }
            }
            else
            {
                DataSet dsTables = ObtenerInfoNombreTablas();
                int i = dsTables.Tables[0].Rows.Count;
                for (int i2 = 0; i2 < i; i2++)
                {
                    _SyntaxStoredProcedure.AppendLine("-- ========================= INI ===========================");
                    _SyntaxStoredProcedure.AppendLine("SET ANSI_NULLS ON");
                    _SyntaxStoredProcedure.AppendLine("GO");
                    _SyntaxStoredProcedure.AppendLine("SET QUOTED_IDENTIFIER ON");
                    _SyntaxStoredProcedure.AppendLine("GO");
                    _SyntaxStoredProcedure.AppendLine("-- =========================================================");
                    _SyntaxStoredProcedure.AppendLine("-- Plantilla generada automáticamente por Jorge L. Torres A.");
                    _SyntaxStoredProcedure.AppendLine("-- =========================================================");
                    _SyntaxStoredProcedure.AppendLine("-- Author     :	Jorge L. Torres A.");
                    _SyntaxStoredProcedure.AppendLine("-- Create date: " + DateTime.Now.ToLongDateString());
                    _SyntaxStoredProcedure.AppendLine("-- Description:	Eliminar Full " + dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    _SyntaxStoredProcedure.AppendLine("-- =========================================================");
                    _SyntaxStoredProcedure.AppendLine("CREATE PROCEDURE spDelFull" + dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    DataSet dsColumns = ObtenerInfoNombreColumnas(dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    int o = dsColumns.Tables[0].Rows.Count;
                    _SyntaxStoredProcedure.AppendLine("AS");
                    _SyntaxStoredProcedure.AppendLine("BEGIN");
                    _SyntaxStoredProcedure.AppendLine(" DELETE ");
                    _SyntaxStoredProcedure.AppendLine(" FROM " + dsTables.Tables[0].Rows[i2][0].ToString().Trim().ToUpper());
                    _SyntaxStoredProcedure.AppendLine("END");
                    _SyntaxStoredProcedure.AppendLine("GO");
                    _SyntaxStoredProcedure.AppendLine("-- ========================= FIN ===========================");
                }
            }
            return _SyntaxStoredProcedure.ToString();
        }
    }
}
