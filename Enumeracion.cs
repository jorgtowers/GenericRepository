using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public sealed class Enumeracion
    {
        public sealed class NM {
            public enum TamañoImagenes { 
                w165,
                w240,
                w480,
                w600,
                h400                
            }
            public enum Ids { 
                Noticia,
                Categoria,
                Contenido,
                Estatus,
                Cuenta,
                ImagenDefault
            }
            public enum AccionNoticia
            {
                Creación,
                CambioEnTítulo,
                CambioEnTexto,
                CambioEnAntetitulo,
                CambioEnUbicación,
                CambioEnFechas,
                CambioEnEstatus,
                CambioEnCrédito,
                CambioEnFuente,
                CambioEnPlantilla,
                CambioEnResponsable,
                Modificación,
                Eliminación,
                CambioPalabrasClaves,
                CambioAgencia,
                CambioVideoYoutube
            }
        }

        public sealed class DataBase {
            public enum Provider { SqlClient, OracleClient, OleDb, ODBC };
            public enum ProviderConnectionString { 
                SQL_SqlClient, 
                SQL_ODBC, 
                ORA_OracleClient_NetFramework_Standard, 
                ORA_OracleClient_NetFramework_SpecifyingUserPwd, 
                ORA_OracleClient_NetFramework_WithOutTNS_BySID, 
                ORA_OracleClient_NetFramework_WithOutTNS_ByServiceName, 
                ORA_ODBC_Oracle_OraClient10g_home, 
                ORA_ODBC_MicrosoftForOracle_NewVersion, 
                ORA_ODBC_MicrosoftForOracle_ConnectDirectly_BySID, 
                ORA_ODBC_MicrosoftForOracle_WithOutTNS_ByServiceName,
                OLE_OleDB,
                OLE_ODBC_StandardSecurity ,
                OLE_Microsoft_OleDb_Oracle_Standard,
                OLE_Microsoft_OleDb_Oracle_Standard_New_BySID,
                OLE_Microsoft_OleDb_Oracle_Standard_New_ByServiceName,
                OLE_Microsoft_OleDb_Oracle_TrustedConnection
            };
            public enum ConnectionMode { ConnectionString, AppSettings, String };
            public enum Separator { Nothing, Guion, Slash, UnderScore, VerticalBar, Comma, SemiColon, Numeral };
            public enum TypeQuery { ById, ByArgument };  
        }
        
        public sealed class Utils {
            public enum EventLog { AllInformation, Sumary, ALine,Simple};
            public enum TagHtml { P, B, I, H1, H2, H3, H4, H5, Span, Div, Br, A, Img };
            public enum TypeColumn { Int32, Int16, Int64, String, Boolean, Byte, Char };
            public enum Rol { Administrator, AdvancedUser, LimitedUser, OperatorUser };
        }
        public sealed class Date {
           public enum Format { DDMMYYYY, YYYYMMDD };
           public enum Separator { Ninguno, Guion, Slash, UnderScore };
           public enum TimeAgo { Hace, Antes };
        }
        
        #region Enumeraciones Obsoletas
        [Obsolete]
        public enum ProveedorBaseDatos { SqlClient, OracleClient, OleDb, ODBC };
        [Obsolete]
        public enum ProveedorCadenaConexion { SQL_SqlClient, SQL_ODBC, ORA_OracleClient_NetFramework_Standard, ORA_OracleClient_NetFramework_SpecifyingUserPwd, ORA_OracleClient_NetFramework_WithOutTNS_BySID, ORA_OracleClient_NetFramework_WithOutTNS_ByServiceName, ORA_ODBC_Oracle_OraClient10g_home, ORA_ODBC_MicrosoftForOracle_NewVersion, ORA_ODBC_MicrosoftForOracle_ConnectDirectly_BySID, ORA_ODBC_MicrosoftForOracle_WithOutTNS_ByServiceName };
        [Obsolete]
        public enum ModoConexion { ConnectionString, AppSettings, String };
        [Obsolete]
        public enum Separador { Ninguno, Guion, Slash, UnderScore, BarraVertical, Coma, PuntoyComa, Numeral };
        [Obsolete]
        public enum ObjetoBaseDatos { SQLStatement, SQLFunction, SQLStoredProcedured };
        [Obsolete]
        public enum ObjetoSalida { DataSet, ExecuteNonQuery, ExecuteScalar };
        [Obsolete]
        public enum TipoConsulta { Id, Query };
        [Obsolete]
        public enum EventLog { TodaInformacion, Resumida, UnaLinea };
        [Obsolete]
        public enum EtiquetaHtml { P, B, I, H1, H2, H3, H4, H5, Span, Div, Br, A, Img };
        [Obsolete]
        public enum FormatoFecha { DDMMYYYY, YYYYMMDD };
        [Obsolete]
        public enum SeparadorFecha { Ninguno, Guion, Slash, UnderScore };
        [Obsolete]
        public enum TipoColumna { Int32, Int16, Int64, String, Boolean, Byte, Char };
        [Obsolete]
        public enum Roles { Administrador, UsuarioAvanzado, UsuarioConsulta, UsuarioLimitado, UsuarioOperador };
        #endregion       
            
    }
}
