using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Reuniones.Model;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;
using GenericRepository.EF5;

namespace ENTITDAD.Model
{
    
    public class AbstractPage:Page
    {

        protected GenericRepository<ENTITDAD> model = new GenericRepository<ENTITDAD>(Global.context);

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

        public Usuario UsuarioActual
        {
            get {
                if (Session["responsable"] != null)
                    return (Usuario)Session["responsable"];
                else
                    return null;
            }
            set { 
                Session["responsable"] = value;                
                Session.Timeout = 60 * 24;//El tiempo de la session dura 24 horas
            }
        }

        protected void CheckParametrosUrlRouting()
        {
            /* Id Noticia, Encuesta, etc...*/
            if (!object.Equals(Page.RouteData.Values["Id"], null))
                int.TryParse(Page.RouteData.Values["Id"] as string, out _Id);
            

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
            
            //SEOConfiguration seo = SEOConfiguration.Instance;
            /* Valida los parametros vía Routing */
            if (RouteData.Values.Count > 0)
                CheckParametrosUrlRouting();
            /* Valida los parametros vía QueryString */
            if (Request.QueryString.Count > 0)
                CheckParametrosUrlQueryString();

            this.Title = "Reuniones";

            //Valida que el usuario este logueado
            if (Request.Url.AbsolutePath != "/pages/login.aspx" && UsuarioActual == null)
                if (Request.Url.AbsolutePath != "/pages/registro.aspx")
                    Response.Redirect("~/pages/login.aspx");

            base.OnLoad(e);
        }
        
    }
}
