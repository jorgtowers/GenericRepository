/*!
 * ABOUT.......: Clase abstracta que va de la mano con GenericRepository.cs, tiene como finalidad recolectar los parametros del RequestQueryString y adicionalmente valida las opciones de LOGIN
 * CREADOR.....: Jorge L. Torres A.
 * ACTUALIACION: 
 * ACTUALIZADO.: 20-03-2015 11:53PM
 * CREADO......: 20-03-2015 11:53PM
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;
// Incorporar NameSpace del "GenericRepository.EF5" para usar la PageDynamic<TEntity>
using GenericRepository.EF5;
// Incorporar Namespace del "Proyecto.Model" para usar el contexto creado por ENTITY
using NAMESPACEPROYECT.Model;

namespace NAMESPACEPROYECT.Model
{
    
    public class AbstractPage:Page
    {

        protected GenericRepository<NAME-OF-EMD_Entities> model = new GenericRepository<NAME-OF-EMD_Entities>(Global.context);

        private int _Id = -1;
        protected int Id {
            get
            {
                return _Id;
            }            
        }
        private string _Clase = "";
        protected string Clase
        {
            get
            {
                return _Clase;
            }
        }

        /* En caso de tener lógica de validación de usuarios 
        public Usuario UsuarioActual
        {
            get
            {
                if (Session["responsable"] != null)
                    return (Usuario)Session["responsable"];
                else
                    return null;
            }
            set
            {
                Session["responsable"] = value;
                Session.Timeout = 60 * 24;//El tiempo de la session dura 24 horas
            }
        }*/

        public string RequestUrl
        {
            get;
            set;
        }

        protected void CheckParametrosUrlRouting()
        {
           /* Parametros vía Routing */
        }
        protected void CheckParametrosUrlQueryString()
        {
          
            /* Id Noticia, Encuesta, etc...*/
            if (!object.Equals(Request.QueryString["Id"], null))
                int.TryParse(Request.QueryString["Id"] as string, out _Id);            
            if (!object.Equals(Request.QueryString["Clase"], null))
                _Clase = Request.QueryString["Clase"] as string;            
        }

        protected override void OnLoad(EventArgs e)
        {
            
            if (RouteData.Values.Count > 0)
                CheckParametrosUrlRouting();

            if (Request.QueryString.Count > 0)
                CheckParametrosUrlQueryString();

            

            /* Valida que el usuario este logueado 
            if (Request.Url.AbsolutePath != "/pages/login.aspx" && UsuarioActual == null)
                if (Request.Url.AbsolutePath != "/pages/registro.aspx")
                    Response.Redirect("~/pages/login.aspx?goto=" + Server.HtmlEncode(Request.Url.ToString().Replace("&", "#")));*/

            base.OnLoad(e);
        }
        
    }
}
