using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using NAMESPACEPROYECT.Model;

namespace NAMESPACEPROYECT
{
    public class Global : System.Web.HttpApplication
    {

        public enum eConeccionBaseDatos { Produccion = 1, Desarrollo = 2 };


        public static string VERSION {
            get {


                Assembly dll = Assembly.LoadFrom(HttpContext.Current.Server.MapPath(@"..\bin\Tickets.dll"));
                return dll.GetName().Version.ToString();
            }
        }
        public static string CNN_PRODUCCION { get { return ConfigurationManager.AppSettings["CNN_PRODUCCION"] as string; } }
        public static string CNN_DESARROLLO { get { return ConfigurationManager.AppSettings["CNN_DESARROLLO"] as string; } }

        public static NAME-OF-EDM_Entities context;


        public static void Conexion(eConeccionBaseDatos ambiente)
        {

            switch (ambiente)
            {
                case eConeccionBaseDatos.Produccion:
                    context = new NAME-OF-EDM_Entities(CNN_PRODUCCION);
                    break;
                case eConeccionBaseDatos.Desarrollo:
                    context = new NAME-OF-EDM_Entities(CNN_DESARROLLO);
                    break;
                default:
                    context = new NAME-OF-EDM_Entities(CNN_PRODUCCION);
                    break;
            }
           
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            context = new NAME-OF-EDM_Entities(CNN_PRODUCCION);            
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
