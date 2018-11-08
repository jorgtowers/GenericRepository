/* ----------------------------------------------------------------------------------------------------------------------------
 * ABOUT.......: Clase generica que permite conectarse a un EDM, en varías versiones de EF4, EF4SupportEF5, EF5 y EF6
 * CREADOR.....: Jorge L. Torres A.
 * MEJORAS.....: 20-03-2015 11:53PM .- Se agregan class='sortable filterable" para que los listados trabajen con los JS sortTable.js y 
 *                                     filterTable.js que permiten filtrar y ordenar la tabla, y se agrega id='listado' requerido por 
 *                                     el script sortTable.js
 *               20-03-2015 11:53PM .- Se agrega propiedad a PageDynamic<T> que permite configuarar campos de 
 *                                     texto como Multilinea, para esto debe indicarse cuales campos separados por coma (,) en la 
 *                                     propiedad CamposTextoMultiLinea
 *               20-03-2015 11:53PM .- Se mejora redireccionamiento al precionar click sobre el boton limpiar
 *               20-03-2015 11:53PM .- Mejora de método de Eliminar, ya no tiene que capturar el ObjectToUpdate de la pantalla
 *               20-03-2015 11:53PM .- Se agrega propiedad de Cantidad a PageDynamic<T> para filtrar el listado, por defecto traerá 100 registros
 *               03-08-2015 09:00PM .- Se agrera EF5.BulkInsertAll<T> que permite agregar grandes lotes de registros
 *               03-08-2015 09:00PM .- Se agrera EF6.BulkInsertAll<T> que permite agregar grandes lotes de registros
 *               07-08-2015 03:36PM .- Se agrega  Utils.CamelCase
 *               25-08-2015 07:45PM .- Se agrega permisología sobre acciones a las pantallas, tomado de la tabla RolPagina
 *               21-10-2015 12:41PM .- Se incluye compatibilidad con los campos de tipo GUID
 *               26-10-2015 02:33PM .- Se agrega propiedad TituloPagina para poder cambiarle el titulo generado por la clase 
 *                                     de forma automatica, ppara cambiar se debe agregar hacer override OnInit(EventArgs e)
 *                                     y agregar la propiedad TituloPagina="titulo deseado";
 *               27-10-2015 04:17PM .- Se agrega opción para eButtonAs {Button, LinkButton} para generación de botones como enlaces
 *                                     con la finalidad de agregar iconos de FontAwesome.
 *               03-11-2015 02:21PM .- Se agrega mejora para los casos donde el MasterPages a usar, dependa de otro MasterPage
 *               21-11-2015 01:21PM .- Se incluye propiedad Redondear para controlar la cantidad de decimales en la visualización de las tablas y de los valores almacenados en la tabla
 *               06-12-2015 02:47PM .- Se incluye propiedad Campos para controlar la visualización de los campos en el listado HTML
 *                                     se crean nueva clase "Custom" internta dentro de PageDynamic<T> para ordenar todas las propiedades
 *                                     de personalización de la clase de PageDynamic<T>.
 *               21-12-2015 02:34PM .- Se incluye propiedad Campos para controlar el MaxLenght de los campos de Texto
 *               15-01-2016 01:43PM .- Se incluye propiedad Campos para controlar validaciones de Expresiones REgulares sobre los campos usando
 *                                     ValidationPattern del objeto App.Validation.js
 *               19-01-2016 12:00PM .- Se incluye propiedad para controlar el Orden Descendente del Listado, para esto se debe Extender el modelo 
 *                                     e implementar la interfaz GenericRepository.EF5.IId para cada Entidad y modificar la propiedad 
 *                                     PageDynamic<T>.Custom.Core.Listado.OrdenDescendente
 *               30-03-2016 04:05PM .- Se incluye validación de lblEstatus cuando es null y no ha sido creada, por no tener acceso a la pagina
 *               02-04-2016 08:18PM .- Se cambia nombre de interfaz GenericRepository.EF5.IDescripcionId a GenericRepository.EF5.IRequiredFields, se le incluye la propiedad "Activo" para 
 *                                     los filtros de los DropDownList filtren por los registros activos solamente. Se comenta bloque de RolPagina para dejar que la seguridad por defecto 
 *                                     no sea cargada por BBDD, en caso de que se requiera se debe descomentar. Se actualiza GenericRepository.Utils.Llenar<> para que incluya el llamada a 
 *                                     la interfaz IRequiredFilds
 *               03-12-2016 15:34PM .- Se integra framework Material Designs ACE http://ace.jeka.by/ para mejorar apariencia del proyecto y de las Paginas Dinamicas
 *               07-12-2016 05:16PM .- Sólo filtrara campos Read/Write, ya que los campos Extendidos de solo lectura generaban error por no tener Set;
 *
 * CREADO......: 20-03-2015 11:53PM
 * ACTUALIZADO.: 19-01-2016 12:00PM
 * ----------------------------------------------------------------------------------------------------------------------------- */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using GenericRepository.EF5;
using System.Text;
using Tickets.Model;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Validation;
using Newtonsoft.Json;
using System.Globalization;
using System.Data.OleDb;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;
using System.Dynamic;
using System.ComponentModel;
using Resource = Tickets.Resources.TicketsResources;


namespace GenericRepository
{
 
    public static class ExtensionMethods {
        public static CultureInfo UICulture {
            get {
                return new System.Globalization.CultureInfo(Resource.Culture.Name.ToString());
            }
        }
        
        public static string ToHtmlTable<T>(this List<T> items)
        {
            var ret = string.Empty;

            return items == null || !items.Any()
                ? ret
                : "<table class='table table-condensed table-stripped'>" +
                  items.First().GetType().GetProperties().Select(p => p.Name).ToList().ToColumnHeaders() +
                  items.Aggregate(ret, (current, t) => current + t.ToHtmlTableRow()) +
                  "</table>";
        }

        public static string ToArrayStringAsString(this string[] items){
         return "['"+ string.Join("','", items.ToArray()) + "']";
        }
        public static string ToArrayStringAsDecimal(this string[] items)
        {
            return "[" + string.Join(",", items.ToArray()) + "]";
        }
        public static string FromObjectToDetailView<T>(this T item)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("<table id='formDetailView' class='table'>\n");
            sb.Append("<tbody>\n");
            sb.Append("<tr><th colspan='2'>"+ item.GetType().BaseType.Name + "</th></tr>");

            foreach (PropertyInfo column in item.GetType().GetProperties())
            {
                
                sb.Append("<tr>");
                string valueOf = "";
                if (column != null)
                {
                    
                    bool starWithID = column.Name.Length > 2 && column.Name.Substring(0, 3).Contains("Id")?true : false;
                    if (!starWithID) {
                        sb.Append("<td class='info'>" + Utils.SplitCamelCase(column.Name) + "</td>");

                        string typeOfColumn = "";
                        if (column.PropertyType.GenericTypeArguments.Length > 0)
                            typeOfColumn = column.PropertyType.GetGenericArguments()[0].FullName;
                        else
                            typeOfColumn = column.PropertyType.FullName;
                        if (column.PropertyType.Namespace == "System")
                            switch (typeOfColumn)
                            {
                                case "System.Decimal":
                                    valueOf = string.Format(UICulture, "{0:N}", column.GetValue(item, null));
                                    break;
                                case "System.DateTime":
                                    valueOf = string.Format(UICulture, "{0:dd/MM/yyyy}", column.GetValue(item, null));
                                    break;
                                default:
                                    valueOf = column.GetValue(item, null).ToString();
                                    break;
                            }
                        else if (column.PropertyType.Namespace == "System.Collections.Generic")
                            valueOf = "Collection of " + column.Name;
                        else
                        {
                            dynamic o = column.GetGetMethod().Invoke(item, null);
                            if (o != null)
                            {
                                valueOf = o.Descripcion;
                            }
                        }
                        sb.Append("<td>" + valueOf + "</td>");
                    }
                    sb.Append("</tr>");
                }
            }
            sb.Append("</tbody>");
            sb.Append("</table>");
            return sb.ToString();
        }

        public static string ToColumnHeaders<T>(this List<T> listOfProperties)
        {
            var ret = string.Empty;

            return listOfProperties == null || !listOfProperties.Any()
                ? ret
                : "<tr>" +
                  listOfProperties.Aggregate(ret,
                      (current, propValue) =>
                          current +
                          ("<th>" +
                           (Convert.ToString(propValue).Length < 60
                               ? Convert.ToString(propValue)
                               : Convert.ToString(propValue).Substring(0, 60)) + "..." + "</th>")) +
                  "</tr>";
        }

        public static string ToHtmlTableRow<T>(this T classObject)
        {
            var ret = string.Empty;

            return classObject == null
                ? ret
                : "<tr>" +
                  classObject.GetType()
                      .GetProperties()
                      .Aggregate(ret,
                          (current, prop) => 
                              current + ("<td>" +
                                         (Convert.ToString(prop.GetValue(classObject, null)).Length <= 100
                                             ? Convert.ToString(prop.GetValue(classObject, null))
                                             : Convert.ToString(prop.GetValue(classObject, null)).Substring(0, 100) +
                                               "...") +
                                         "</td>")) + "</tr>";
        }

        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static string TrimAndReduce(this string str)
        {
            return ConvertWhitespacesToSingleSpaces(str).Trim();
        }

        public static string ConvertWhitespacesToSingleSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }
        /// <summary>
        /// Extension de String, para retornar una cantidad de caracteres definidos agregandole un sufijo como "..." 
        /// </summary>
        /// <param name="value">string a evaluar</param>
        /// <param name="maxLength">caracteres a retornar</param>
        /// <param name="padString">caracteres de relleno</param>
        /// <returns></returns>
        public static string MaxLenght(this string value, int maxLength, string padString = "")
        {
            if (value.Length > maxLength)
                return value.Substring(0, maxLength) + padString;
            else
                return value;
        }
        public static string ChangeApostropheByLiteralApostrophe(this string value)
        {
            return value.Replace("'", "`");
        }
        public static int ToInteger(this string value) {
            int i = 0;
            int.TryParse(value,out i);
            return i;
        }
    }

    [Information(Descripcion = "Clase que implementa metodos genericos apartir del modelo de EDM")]
    public class PageGeneric<T> : AbstractPage where T : class, IId, new()
    {

        private T _ObjectToUpdate = new T();
        protected T ObjectToUpdate
        {
            get { return _ObjectToUpdate; }
            set { _ObjectToUpdate = value; }
        }
        private string _Resultado = "";
        public string Resultado
        {
            get { return _Resultado; }
            set { _Resultado = value; }
        }
        protected virtual void Agregar(object sender, EventArgs e)
        {
            try
            {
                model.Agregar<T>(ObjectToUpdate);
                Limpiar(sender, e);
                _Resultado = "Registro agregado satisfactoriamente...";
            }
            catch (Exception ex) { _Resultado = ex.Message; }
        }
        protected virtual void Modificar(object sender, EventArgs e)
        {
            try
            {
                model.Modificar<T>(ObjectToUpdate);
                Limpiar(sender, e);
                _Resultado = "Registro modificado satisfactoriamente...";
            }
            catch (Exception ex) { _Resultado = ex.Message; }
        }
        protected virtual void Eliminar(object sender, EventArgs e)
        {
            try
            {
                _ObjectToUpdate = model.Obtener<T>(base.Id);
                model.Eliminar<T>(_ObjectToUpdate);
                Limpiar(sender, e);
                _Resultado = "Registro eliminado satisfactoriamente...";
            }
            catch (Exception ex) { _Resultado = ex.Message; }
        }

        protected virtual void Limpiar(object sender, EventArgs e)
        {
            foreach (Control item in Page.Form.Controls[0].Controls)
            {
                if (item.GetType() == typeof(TextBox))
                {
                    ((TextBox)item).Text = "";

                }
                if (item.GetType() == typeof(RadioButtonList))
                {
                    foreach (ListItem radio in ((RadioButtonList)item).Items)
                    {
                        radio.Selected = false;
                    }
                }
                if (item.GetType() == typeof(CheckBoxList))
                {
                    foreach (ListItem radio in ((CheckBoxList)item).Items)
                    {
                        radio.Selected = false;
                    }
                }
            }
            /*
            Button btn = ((Button)sender);
            if (btn.Text == "Limpiar" || btn.Text == "Eliminar")
            {
                Response.Redirect(Request.Url.LocalPath, false);
            }
            else
            {
                Response.Redirect(Request.Url.AbsoluteUri, false);
            }
            */
            int urlCompleta = Request.Url.AbsoluteUri.Length;
            bool tieneId = (Request.Url.AbsoluteUri.ToLower().LastIndexOf("id=") == -1 ? false : true);
            if (tieneId)
                urlCompleta = urlCompleta - 5;
            Response.Redirect(Request.Url.AbsoluteUri.Substring(0, urlCompleta));
        }

    }

   
    
    /// <summary>
    /// Clase especializada para la generación de páginas web apartir del nombre de una instancia, usando reflextion, para reescribir su configuración se debe hacer un override de OnInit y afectar a las propiedades expuestas
    /// </summary>
    /// <typeparam name="T">Instancia de Type a usar</typeparam>
    [Information(Descripcion = "Clase especializada para la generación de páginas web apartir del nombre de una instancia, usando reflextion")]
    public abstract class PageDynamic<T> : AbstractPage where T : class, IId, new()
    {

        internal string Translate(string name)
        {
            string finded= Resource.ResourceManager.GetString(name, CurrentCulture);
            return finded!=null ? finded : name;
        }

    private CultureInfo _CurrentCulture = Resource.Culture;
        public CultureInfo CurrentCulture {
            get { return _CurrentCulture;
                
            }
            set { _CurrentCulture = value; }
        }
        

        /// <summary>
        /// Enumerativo que determinar la presentación usarán los datos de tipo Boolean
        /// </summary>
        public enum eBooleanAs { RadioButton, CheckBox, Toogle }
        /// <summary>
        /// Enumerativo que determinar la presentación usarán los botones de acciones para el CRUD
        /// </summary>
        public enum eButtonsAs { Button, LinkButton }
        internal class Custom
        {
            internal class Core
            {
                internal class Listado
                {
                    private static int _MinRegistros = 10;
                    /// <summary>
                    /// Cantidad de registros a retornar en el listado por defecto
                    /// </summary>
                    public static int MinRegistros
                    {
                        get { return _MinRegistros; }
                        set { _MinRegistros = value; }
                    }
                    private static int _MaxRegistros = 1000;
                    /// <summary>
                    /// Cantidad de registros a retornar en el listado por defecto
                    /// </summary>
                    public static int MaxRegistros
                    {
                        get { return _MaxRegistros; }
                        set { _MaxRegistros = value; }
                    }
                    private static bool _OrdenDescendente = false;
                    /// <summary>
                    /// Permite ordenar los registros de forma descendente por su Id, su orden por defecto será Ascendente, es decir _OrdenDescendente = false;
                    /// </summary>
                    public static bool OrdenDescendente
                    {
                        get { return _OrdenDescendente; }
                        set { _OrdenDescendente = value; }
                    }
                }
                private static short _Redondear = 2;
                /// <summary>
                /// Cantidad de decimales que serán usados para números decimales, su valor por defecto serán 2
                /// </summary>
                public static short Redondear
                {
                    get { return _Redondear; }
                    set { _Redondear = value; }
                }
            }
            internal class UI
            {
                internal class TableHTML
                {
                    private static string _Campos = "*";
                    /// <summary>
                    /// Indique nombre de campos separando por coma (,) los que serán mostradas en la Tabla HTML , por defecto mostrará todos los campos (*). Ej.: Id,Descripcion
                    /// </summary>
                    public static string Campos
                    {
                        get { return _Campos; }
                        set { _Campos = value; }
                    }
                    private static string _Titulo = string.Empty;
                    /// <summary>
                    /// Nombre que tendrá el titulo de la página, su valor por defecto está en blanco "String.Empty"
                    /// </summary>
                    public static string Titulo
                    {
                        get { return _Titulo; }
                        set { _Titulo = value; }
                    }
                }
                internal class BooleanAs
                {
                    private static eBooleanAs _MostrarBoolenosComo = eBooleanAs.Toogle;
                    /// <summary>
                    /// Permite controlar la presentación que usarán los datos de tipo Boolean, por defecto se usará eBooleanAs.CheckBox
                    /// </summary>
                    public static eBooleanAs MostrarBoolenosComo
                    {
                        get { return _MostrarBoolenosComo; }
                        set { _MostrarBoolenosComo = value; }
                    }
                }
                internal class TextBoxAs
                {
                    private static string _Color = "color";
                    /// <summary>
                    /// Indique nombre de campos separando por coma (,) los que activaran el selector de colores usado por el jscolor.min.js, por defecto se agregarà a la clase="jscolor". Ej.: Color
                    /// </summary>
                    public static string Color
                    {
                        get { return _Color; }
                        set { _Color = value; }
                    }
                    private static string _MultiLinea = "texto";
                    /// <summary>
                    /// Indique nombre de campos separando por coma (,) los que serán tipo "Multilinea", por defecto buscará campo llamado TEXTO. Ej.: Texto,Observacion,Descripcion
                    /// </summary>
                    public static string MultiLinea
                    {
                        get { return _MultiLinea; }
                        set { _MultiLinea = value; }
                    }
                    private static string _Fecha = "";
                    /// <summary>
                    /// Indique nombre de campos separando por coma (,) los que serán tipo "DateTime" se le agregará la clase ".datapicker", por defecto buscará campo llamado FECHA. Ej.: FechaInicio,FechaFin
                    /// </summary>
                    public static string Fecha
                    {
                        get
                        {
                            return _Fecha;
                        }

                        set
                        {
                            _Fecha = value;
                        }
                    }
                    private static string _MaxLength = "descripcion:80";
                    /// <summary>
                    /// Indique nombre de campos separando por coma (,) y dos puntos (:) el valor del MaxLenght por campo, a los cuales se les establecerá el atributo maxlength. Ej.: Descripcion:80,Observacion:255
                    /// </summary>
                    public static string MaxLength
                    {
                        get { return _MaxLength; }
                        set { _MaxLength = value; }
                    }

                    private static string _ValidationPattern = "";
                    /// <summary>
                    /// Indique nombre de campos separando por coma (,) y dos puntos (:) del valor de la clave del JS App.JS para el NS "App.Utils.Validation" por campo. Ej.: Descripcion:1,Observacion:0, donde "1" permite solo valores númericos y "0" una direccion url
                    /// </summary>
                    public static string ValidationPattern
                    {
                        get { return _ValidationPattern; }
                        set { _ValidationPattern = value; }
                    }
                }
                internal class ButtonAs
                {
                    internal class Names
                    {
                        private static string _Agregar = Resource.btnAdd;
                        /// <summary>
                        /// Nombre que tendrá el boton de btnAgregar, su valor por defecto es "Nuevo"
                        /// </summary>                        
                        public static string Agregar
                        {
                            get { return _Agregar; }
                            set { _Agregar = value; }
                        }
                        private static string _Modificar = Resource.btnEdit;
                        /// <summary>
                        /// Nombre que tendrá el boton de btnModificar, su valor por defecto es "Guardar"
                        /// </summary>                        
                        public static string Modificar
                        {
                            get { return _Modificar; }
                            set { _Modificar = value; }
                        }
                        private static string _Eliminar = Resource.btnDel;
                        /// <summary>
                        /// Nombre que tendrá el boton de btnEliminar, su valor por defecto es "Borrar"
                        /// </summary>
                        public static string Eliminar
                        {
                            get { return _Eliminar; }
                            set { _Eliminar = value; }
                        }
                        private static string _Limpiar = Resource.btnClear;
                        /// <summary>
                        /// Nombre que tendrá el boton de btnLimpiar, su valor por defecto es "Cancelar"
                        /// </summary>
                        public static string Limpiar
                        {
                            get { return _Limpiar; }
                            set { _Limpiar = value; }
                        }
                    }
                    private static eButtonsAs _MostrarBotonesComo = eButtonsAs.LinkButton;
                    /// <summary>
                    /// Permite controlar la presentación que usarán los botones de acciones para el CRUD, por defecto se usará eButtonsAs.LinkButton
                    /// </summary>
                    public static eButtonsAs MostrarBotonesComo
                    {
                        get { return _MostrarBotonesComo; }
                        set { _MostrarBotonesComo = value; }
                    }
                }

            }
        }

        internal Panel _Panel = new Panel();
        internal Panel _PanelCustoms = new Panel();
        /// <summary>
        /// Instancia del Panel que será usado para crear todos los elementos de la instancia del objeto recibido
        /// </summary>
        public Panel Panel
        {
            get { return _Panel; }
            set { _Panel = value; }
        }

        internal List<KeyValuePair<string, string>> _Fields = new List<KeyValuePair<string, string>>();
        /// <summary>
        /// Listado de campos y tipo de valor para a creación de los elementos y validar sus tipos de datos
        /// </summary>
        public List<KeyValuePair<string, string>> Fields
        {
            get { return _Fields; }
            set { _Fields = value; }
        }
        internal List<T> _Listado = new List<T>();
        /// <summary>
        /// Expone listado de T para usarlo en el metodo de actualizar listado
        /// </summary>
        public List<T> Listado
        {
            get { return _Listado; }
            set { _Listado = value; }
        }
        /// <summary>
        /// Instancia de Label para mostrar notificación de las operaciones básicas
        /// </summary>
        public Label lblEstatus
        {

            get
            {
                
                return _Panel.Controls.OfType<Label>().Where(x => x.ID == "lblEstatus").FirstOrDefault();
            }
        }
        /// <summary>
        /// Prepara la página para crear por Reflextion todos los campos y botones propios de la instancia del objeto recibido
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e) 
        {
            Type TDynamic = null;
            base.CheckParametrosUrlQueryString();
            if (string.IsNullOrEmpty(base.Clase))
                TDynamic = typeof(T);
            else
                TDynamic = Type.GetType(typeof(T).Namespace + "." + base.Clase);

            /* ---------------------------------------------------
             * Lectura del esamblado y de la documentación en XML
             * --------------------------------------------------- */
            string ruta = HttpContext.Current.Server.MapPath(@"\bin\" + TDynamic.Assembly.ManifestModule.Name);
            Assembly dll = Assembly.LoadFrom(ruta);
            XDocument xml = XDocument.Load(ruta.Replace(".dll", ".xml"));
            var sumarios = xml.Descendants("member").Where(x => x.LastAttribute.Value.Substring(0, 2) == "P:" && x.LastAttribute.Value.Contains(TDynamic.Namespace)).ToList();
            var sumariosGeneric = xml.Descendants("member").Where(x => x.LastAttribute.Value.Contains("P:GenericRepository.PageDynamic`1.Custom")).ToList();
            _Panel = this.Controls.OfType<Panel>().FirstOrDefault();
            if (_Panel == null)
            {
                _Panel = new System.Web.UI.WebControls.Panel() { ID = "PN" };
                MasterPage masterPage = this.Master;
                HtmlForm form = null;
                form = this.Master.Controls.OfType<System.Web.UI.HtmlControls.HtmlForm>().FirstOrDefault();
                if (form == null)
                    form = this.Master.Master.Controls.OfType<System.Web.UI.HtmlControls.HtmlForm>().FirstOrDefault();
                ContentPlaceHolder cph = form.Controls.OfType<ContentPlaceHolder>().FirstOrDefault();
                /* ---------------- _PanelCustoms ------------------------
                 * Se agrega _PanelCustoms para los campos a personalizar
                 * ------------------------------------------------------- */
                //Type TypeOfGenericRespository = typeof(Custom);
                //_PanelCustoms = new System.Web.UI.WebControls.Panel() { ID = "PNCustoms" };
                //_PanelCustoms.Attributes.Add("style", "display:none");
                //_PanelCustoms.Controls.Add(new LiteralControl("<style>div#CPH_BODY_PNCustoms{padding:1em;}div#CPH_BODY_PNCustoms ul>li {width: 49%;display: inline-grid;padding: 1%;}span#closeCustoms {background: #62a8d1;padding: .2em .5em .5em;position: absolute;z-index: 1;right: 1em;cursor: pointer;border-bottom-left-radius: 10px;border-bottom-right-radius: 10px;color: white;}</style>"));

                //MemberInfo[] subClases = TypeOfGenericRespository.GetMembers(BindingFlags.NonPublic);
                //foreach (MemberInfo miembro in subClases)
                //{
                //    //Obtiene las Clases (UI,CORE)
                //    _PanelCustoms.Controls.Add(new LiteralControl(miembro.Name + "<br/>"));
                //    Type typeMiembro = Type.GetType(miembro.ReflectedType.FullName + "+" + miembro.Name);
                //    MemberInfo[] membersInEach = typeMiembro.GetMembers(BindingFlags.NonPublic);
                //    if (membersInEach.Length > 0) {
                //        //Evalue si tiene más SubClases
                //        foreach (MemberInfo memberInfoOf in membersInEach)
                //        {
                //            //Obtiene las subclases de UI (TextBoxAs,BooleanAs,ButtonAs,TableHTML)
                //            _PanelCustoms.Controls.Add(new LiteralControl(memberInfoOf.Name+"<br/>"));
                //            Type typeMemberInEach = Type.GetType(memberInfoOf.ReflectedType.FullName + "+" + memberInfoOf.Name);
                //            Type instanceGeneric = typeMemberInEach.MakeGenericType(new Type[1] { TDynamic });
                //            object instancia = Activator.CreateInstance(instanceGeneric, null);

                //            PropertyInfo[] propertyInfoOf = typeMemberInEach.GetProperties();
                //            _PanelCustoms.Controls.Add(new LiteralControl("<ul>"));
                //            foreach (PropertyInfo item in propertyInfoOf)
                //            {
                //                PropertyInfo[] propertinesInstancia = instancia.GetType().GetProperties();

                //                TextBox t = new TextBox() { ID = "txt" + item.Name.Replace(" ", ""), CssClass = "form-control" };
                //                t.Attributes.Add("placeholder", item.Name);
                //                var sumarioGeneric = sumariosGeneric.Where(x => x.LastAttribute.Value.Contains(item.Name)).FirstOrDefault();
                //                string txtSumarioLabel = (sumarioGeneric != null ? sumarioGeneric.Value : "");
                //                t.Attributes.Add("title", txtSumarioLabel);
                //                foreach (PropertyInfo itemProperty in propertinesInstancia)
                //                {
                //                    if (itemProperty.Name == item.Name)
                //                        t.Text = itemProperty.GetValue(instancia).ToString();
                //                }
                //                _PanelCustoms.Controls.Add(new LiteralControl("<li><b>" + item.Name + "</b>"));
                //                _PanelCustoms.Controls.Add(t);
                //                _PanelCustoms.Controls.Add(new LiteralControl("<i>"+txtSumarioLabel + "</i></li>"));
                //            }
                //            _PanelCustoms.Controls.Add(new LiteralControl("</ul>"));
                //        }
                //    }
                //    PropertyInfo[] propertyInEach = typeMiembro.GetProperties();
                //}
                //Button btnCustoms = new Button() { ID = "btnCustoms", CssClass = "btn btn-primary", Text = "Guardar ajustes" };
                //_PanelCustoms.Controls.Add(btnCustoms);
                //cph.Controls.Add(new LiteralControl("<span id='closeCustoms'><b class='fa fa-cogs'></b></span>"));
                //cph.Controls.Add(_PanelCustoms);

                cph.Controls.Add(_Panel);
            }


            /* ---------------------------------------------------
             * Acciones y permisos del rol
             * Activar si el sitio implementa seguridad por BBDD
             * ---------------------------------------------------
             *
             *  string urlActual = Request.Url.LocalPath;
             *  RolPagina rolEnPagina = null;
             *  try
             *  {
             *      rolEnPagina = UsuarioActual.Rol.RolPagina.Where(x => x.Pagina.Ruta.ToLower() == urlActual.ToLower()).FirstOrDefault();
             *  }
             *  catch { }
             *  bool? tieneAcceso = null, puedeSeleccionar = null, puedeListar = null, puedeAgregar = null, puedeModificar = null, puedeEliminar = null;
             *  if (rolEnPagina != null)
             *  {
             *      try
             *      {
             *          tieneAcceso = rolEnPagina.Acceso;
             *          puedeSeleccionar = rolEnPagina.Seleccionar;
             *          puedeListar = rolEnPagina.Listar;
             *          puedeAgregar = rolEnPagina.Agregar;
             *          puedeModificar = rolEnPagina.Modificar;
             *          puedeEliminar = rolEnPagina.Eliminar;
             *      }
             *      catch { }
             *  }
             *  else
             *      tieneAcceso = false;
             *
             * --------------------------------------------------- */



            /* ---------------------------------------------------
             * Desactivar si el sitio será controlado por BBDD
             * --------------------------------------------------- */
            bool? tieneAcceso = true, puedeSeleccionar = true, puedeListar = true, puedeAgregar = true, puedeModificar = true, puedeEliminar = true;




            #region Definición del título de la página a partir de la entidad
            /* ------------------------------
             * Agregando título a la pagina
             * ------------------------------ */
            string title = Translate("lblMaster");
            string end = TDynamic.Name.Substring(TDynamic.Name.Length - 1).ToLower();
            string nameof = " " + Utils.SplitCamelCase(Translate(TDynamic.Name));
            switch (end)
            {
                case "s":
                    title += nameof;
                    break;
                case "n":
                    title += nameof + "es";
                    break;
                case "r":
                    title += nameof + "es";
                    break;
                case "l":
                    title += nameof + "es";
                    break;
                default:
                    title += nameof + "s";
                    break;
            }
            if (string.IsNullOrEmpty(Custom.UI.TableHTML.Titulo))
                this.Page.Title = title;
            else
            {
                this.Page.Title = Custom.UI.TableHTML.Titulo;
                title = Custom.UI.TableHTML.Titulo;
            }
            #endregion

            string[] labelsToTranslate = { "lblOpenPanelEdit", "lblLimitRecords" };
            


            base.OnInit(e);


            if (!tieneAcceso.HasValue || tieneAcceso.Value)
            {
                /* ---------------------------------------------------------------------------------------------------------------
                 * Solo filtrara campos Read/Write, ya que los campos Extendidos de solo lectura generaban error por no tener Set;
                 * ---------------------------------------------------------------------------------------------------------------*/
                PropertyInfo[] propiedades = TDynamic.GetProperties().Where(x => x.SetMethod != null).ToArray();
                #region Inicio Integracion con Framwork ACE
                _Panel.Controls.Add(new LiteralControl("<div class='breadcrumbs ace-save-state' id='breadcrumbs'><ul class='breadcrumb'><li><i class='ace-icon fa fa-home home-icon'></i><a href='/pages/'>Home</a></li><li class='active'>"+ title + "</li></ul><!-- /.breadcrumb --></div><div class='page-content'><div class='page-header'><h1>"+title+"<small><i class='ace-icon fa fa-angle-double-right'></i>Información</small></h1></div><!-- /.page-header --><div class='row'><div class='col-xs-12'><!-- PAGE CONTENT BEGINS -->"));
                #endregion
                #region Region del Mantenimiento para hacer CRUD de los registros
                _Panel.Controls.Add(new LiteralControl("<p id='btnToogleEditPanel'><b class='fa fa-edit'></b>"+ Translate(labelsToTranslate[0]) + "</p><div id='editPanel' style='display: none'><span id='closeEditPanel' ><b class='fa fa-times'></b></span>"));
                //_Panel.Controls.Add(new LiteralControl("<nav><h4>Gestión de datos</h4></nav>"));
                _Panel.Controls.Add(new LiteralControl("<table class='table'><tbody>"));
                #region Campos para Agregar y/o Editar la información de los registros seleccionados o por crear

                foreach (PropertyInfo propiedad in propiedades)
                {
                    string tipo = "";
                    string nombre = "";
                    if (propiedad.PropertyType.GetGenericArguments().Count() > 0)
                    {
                        tipo = propiedad.PropertyType.GetGenericArguments()[0].Name;
                    }
                    else
                    {
                        tipo = propiedad.PropertyType.Name;
                    }
                    /* ----------------
                     * Agregando campos de texto y/o checkbox a la pagina
                     * ----------------*/
                    if (TDynamic.Namespace == propiedad.PropertyType.Namespace)
                    {
                        nombre = propiedad.PropertyType.Name;
                        var sumarioPropiedad = sumarios.Where(x => x.LastAttribute.Value.Contains(TDynamic.FullName + "." + nombre)).FirstOrDefault();


                        _Fields.Add(new KeyValuePair<string, string>("ddl" + nombre + "-" + propiedad.Name.Replace(nombre, ""), "Int32"));
                        _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" +  Utils.SplitCamelCase(Translate(propiedad.Name)) + "</b><p>" + (sumarioPropiedad != null ? sumarioPropiedad.Value.Trim() : "") + "</p></td><td>"));
                        string typeName = propiedad.PropertyType.Namespace + "." + nombre;
                        Type clase = Type.GetType(typeName);


                        IRequiredFields obj = (IRequiredFields)Activator.CreateInstance(clase);


                        //AssemblyName assemblyName = dll.GetName();
                        //System.Reflection.Emit.AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, System.Reflection.Emit.AssemblyBuilderAccess.Run);                        
                        //System.Reflection.Emit.ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(dll.Modules.FirstOrDefault().Name);
                        //System.Reflection.Emit.TypeBuilder tb = moduleBuilder.DefineType(typeName, TypeAttributes.Public);
                        //tb.SetParent(clase);
                        //tb.AddInterfaceImplementation(typeof(IActivo));


                        obj.Descripcion = "( -- " +Translate("Seleccione un item de")  + Translate(clase.Name) + "-- )";
                        obj.Id = -1;

                        DbSet setClase = null;
                        if (clase != null)
                        {
                            setClase = model.model.Set(clase);
                            setClase.Load();
                        }
                        DropDownList t = new DropDownList()
                        {
                            ID = "ddl" + nombre + "-" + propiedad.Name.Replace(nombre, ""),
                            DataTextField = "Descripcion",
                            DataValueField = "Id",
                            CssClass = "form-control"
                        };

                        IList<IRequiredFields> listado = setClase != null ? setClase.Local.Cast<IRequiredFields>().Where(x => x.Activo.Value).ToList() : null;
                        listado.Add(obj);

                        t.DataSource = listado.OrderBy(x => x.Descripcion);
                        t.DataBind();

                        _Panel.Controls.Add(t);
                        _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                    }

                    if (propiedad.PropertyType.Namespace == "System")
                    {
                        nombre = propiedad.Name;
                        var sumarioPropiedad = sumarios.Where(x => x.LastAttribute.Value.Contains(TDynamic.FullName + "." + nombre)).FirstOrDefault();
                        /* ----------------
                         * Leyendo la Descripcion de la clase Info:System.Attribute
                         * ----------------*/
                        string labelDescripcion = "";
                        labelDescripcion = (sumarioPropiedad != null ? sumarioPropiedad.Value.Trim() : "");

                        if (tipo == "String" || tipo == "Int32" || tipo == "DateTime" || tipo == "Decimal" || tipo == "Float" || tipo == "Guid")
                        {
                            _Fields.Add(new KeyValuePair<string, string>("txt" + nombre, tipo));
                            TextBox t = new TextBox() { ID = "txt" + nombre.Replace(" ", ""), CssClass = "form-control" };

                            //Establece el tipo Multilinea a los campos indicados en la propiedad Custom.UI.TextBoxAs.MultiLinea
                            foreach (string item in Custom.UI.TextBoxAs.MultiLinea.ToLower().Split(','))
                            {
                                if (nombre.ToLower() == item)
                                    t.TextMode = TextBoxMode.MultiLine;
                            }

                            //Establece el Maximo de caracteres permitido a los campos indicados en la propiedad Custom.UI.TextBoxAs.MaxLength
                            foreach (string item in Custom.UI.TextBoxAs.MaxLength.ToLower().Split(','))
                            {
                                string[] campo = item.Split(':');
                                if (nombre.ToLower() == campo[0])
                                    t.MaxLength = int.Parse(campo[1]);
                            }

                            //Establece el Patron de Validacion de JS, somando el patron de NS App.Utils.Validation.Pattern 
                            foreach (string item in Custom.UI.TextBoxAs.ValidationPattern.ToLower().Split(','))
                            {
                                string[] campo = item.Split(':');
                                if (nombre.ToLower() == campo[0])
                                    t.Attributes.Add("validation", campo[1]);
                            }

                            //Establece nombre de clase "jscolor" a la propiedad class, para los campos que activan selector de color
                            foreach (string item in Custom.UI.TextBoxAs.Color.ToLower().Split(','))
                            {
                                string[] campo = item.Split(':');
                                if (nombre.ToLower() == campo[0])
                                    t.CssClass += " jscolor";
                            }

                            t.Attributes.Add("placeHolder", Utils.SplitCamelCase(Translate(nombre)));
                            if (nombre == "Id")
                            {
                                _Panel.Controls.Add(new LiteralControl("<tr class='help' style='display:none'><td  class='info'><b>" + Utils.SplitCamelCase(Translate(nombre)) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                                t.Enabled = false;
                                _Panel.Controls.Add(t);
                                _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                            }
                            else
                                if (nombre.ToLower() == "userid" || !nombre.Contains("Id"))
                                {
                                    //Establece un JQuery de DateTimePicker para los campos indicados en la propiedad Custom.UI.TextBoxAs.Fecha
                                    if (Custom.UI.TextBoxAs.Fecha.Length > 0 && Custom.UI.TextBoxAs.Fecha.Split(',').ToList().Contains(nombre))
                                    {
                                        _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(Translate(nombre)) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                                        _Panel.Controls.Add(new LiteralControl("<div class='input-group date date-picker'>"));
                                        t.CssClass += " date-picker";
                                        _Panel.Controls.Add(t);
                                        _Panel.Controls.Add(new LiteralControl("<span class='input-group-addon'><span class='fa fa-calendar-o'></span></span></div></td></tr>"));
                                    }
                                    else
                                    {

                                        _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(Translate(nombre)) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                                        _Panel.Controls.Add(t);
                                        _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                                    }
                                }
                        }
                        if (tipo == "Boolean")
                        {
                            /* ----------------
                             * Evalua enumerativo "eBooleanAs" para determinar que presentación usarán los datos de tipo Boolean, por defecto se usará eBooleanAs.CheckBox
                             * ----------------*/
                            switch (Custom.UI.BooleanAs.MostrarBoolenosComo)
                            {
                                case eBooleanAs.RadioButton:
                                    {
                                        _Fields.Add(new KeyValuePair<string, string>("rbt" + nombre, tipo));
                                        _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(Translate(nombre)) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                                        RadioButton t = new RadioButton() { GroupName = "rbt" + nombre.Replace(" ", ""), Text = "Si" };
                                        _Panel.Controls.Add(t);
                                        t = new RadioButton() { GroupName = "rbt" + nombre.Replace(" ", ""), Text = "No" };
                                        _Panel.Controls.Add(t);
                                        _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                                        break;
                                    }
                                case eBooleanAs.CheckBox:
                                    {
                                        _Fields.Add(new KeyValuePair<string, string>("chk" + nombre, tipo));
                                        _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(Translate(nombre)) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                                        CheckBox t = new CheckBox() { ID = "chk" + nombre.Replace(" ", "") };
                                        _Panel.Controls.Add(t);
                                        _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                                        break;
                                    }
                                case eBooleanAs.Toogle:
                                    {
                                        _Fields.Add(new KeyValuePair<string, string>("chk" + nombre, tipo));
                                        _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(Translate(nombre)) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                                        CheckBox t = new CheckBox() { ID = "chk" + nombre.Replace(" ", "") };
                                        _Panel.Controls.Add(t);
                                        _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }



                    }
                    //if (TDynamic.Namespace == propiedad.PropertyType.Namespace)
                    //    sb.AppendLine("<b>Objeto Nombre:</b>" + propiedad.Name);
                    //if (propiedad.PropertyType.Namespace == "System.Collections.Generic")
                    //    sb.AppendLine("<b>Lista Nombre:</b>" + propiedad.Name);

                    //sb.AppendLine("<br>");
                    //Response.Write(sb.ToString());
                    #endregion
                }
                _Panel.Controls.Add(new LiteralControl("<tr><td colspan='2'>"));
                Label lblEstatus = new Label() { ID = "lblEstatus" };
                _Panel.Controls.Add(lblEstatus);
                _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                _Panel.Controls.Add(new LiteralControl("<tr><td colspan='2' style='text-align: right;position:relative'>"));
                #region Botones de accion para Agregar, Modificar, Eliminar y/o Cancelar
                /* ----------------
                 * Agregando botones de acciones a la página
                 * ----------------*/

                switch (Custom.UI.ButtonAs.MostrarBotonesComo)
                {
                    case eButtonsAs.Button:
                        {
                            Button btnEliminar = new Button() { ID = "btnEliminar", CssClass = "btn btn-danger", Text = Custom.UI.ButtonAs.Names.Eliminar };
                            btnEliminar.Attributes.Add("style", "position: absolute;  left:0");
                            btnEliminar.Click += Eliminar;
                            if ((puedeEliminar.HasValue && !puedeEliminar.Value) || base.Id < 1)
                                btnEliminar.Visible = false;

                            if ((puedeEliminar.HasValue && !puedeEliminar.Value) || base.Id < 1)
                                btnEliminar.Visible = false;
                            _Panel.Controls.Add(btnEliminar);

                            Button btnModificar = new Button() { ID = "btnModificar", CssClass = "btn btn-primary", Text = Custom.UI.ButtonAs.Names.Modificar };
                            btnModificar.Click += Modificar;
                            //btnModificar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                            if ((puedeModificar.HasValue && !puedeModificar.Value) || base.Id < 1)
                                btnModificar.Visible = false;

                            _Panel.Controls.Add(btnModificar);

                            Button btnAgregar = new Button() { ID = "btnAgregar", CssClass = "btn btn-success", Text = Custom.UI.ButtonAs.Names.Agregar };
                            btnAgregar.Click += Agregar;
                            //btnAgregar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                            if ((puedeAgregar.HasValue && !puedeAgregar.Value) || base.Id > 0)
                                btnAgregar.Visible = false;

                            _Panel.Controls.Add(btnAgregar);

                            Button btnLimpiar = new Button() { ID = "btnLimpiar", CssClass = "btn btn-default", Text = Custom.UI.ButtonAs.Names.Limpiar };
                            btnLimpiar.Click += Limpiar;
                            _Panel.Controls.Add(btnLimpiar);
                            break;
                        }
                    case eButtonsAs.LinkButton:
                        {
                            LinkButton btnEliminar = new LinkButton() { ID = "btnEliminar", CssClass = "btn btn-danger", Text = "<b class='fa fa-times' ></b>&nbsp;" + Custom.UI.ButtonAs.Names.Eliminar };
                            btnEliminar.Attributes.Add("style", "position: absolute;  left:0");
                            btnEliminar.Click += Eliminar;
                            if ((puedeEliminar.HasValue && !puedeEliminar.Value) || base.Id < 1)
                                btnEliminar.Visible = false;
                            if ((puedeEliminar.HasValue && !puedeEliminar.Value) || base.Id < 1)
                                btnEliminar.Visible = false;
                            _Panel.Controls.Add(btnEliminar);

                            LinkButton btnModificar = new LinkButton() { ID = "btnModificar", CssClass = "btn btn-primary", Text = "<b class='fa fa-save' ></b>&nbsp;" + Custom.UI.ButtonAs.Names.Modificar };
                            btnModificar.Click += Modificar;
                            //btnModificar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                            if ((puedeModificar.HasValue && !puedeModificar.Value) || base.Id < 1)
                                btnModificar.Visible = false;
                            _Panel.Controls.Add(btnModificar);

                            LinkButton btnAgregar = new LinkButton() { ID = "btnAgregar", CssClass = "btn btn-success", Text = "<b class='fa fa-plus-circle' ></b>&nbsp;" + Custom.UI.ButtonAs.Names.Agregar };
                            btnAgregar.Click += Agregar;
                            //btnAgregar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                            if ((puedeAgregar.HasValue && !puedeAgregar.Value) || base.Id > 0)
                                btnAgregar.Visible = false;
                            _Panel.Controls.Add(btnAgregar);

                            LinkButton btnLimpiar = new LinkButton() { ID = "btnLimpiar", CssClass = "btn btn-default", Text = Custom.UI.ButtonAs.Names.Limpiar };
                            btnLimpiar.Click += Limpiar;
                            _Panel.Controls.Add(btnLimpiar);
                            break;
                        }
                    default:
                        break;
                }

                #endregion
                _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                _Panel.Controls.Add(new LiteralControl("</tbody></table></div>"));
                #endregion
                #region Paginacion con Entity
                DropDownList ddlRegistros = new DropDownList() { ID = "ddlRegistros", CssClass = "form-control", AutoPostBack=true };
                ddlRegistros.SelectedIndexChanged += SeleccionRegistros;
                _Panel.Controls.Add(new LiteralControl("<table></tr><td>" + Translate(labelsToTranslate[1]) + "</td><td>"));
                _Panel.Controls.Add(ddlRegistros);
                _Panel.Controls.Add(new LiteralControl("</td></tr></table>"));
                //Button btnAgregar = new Button() { ID = "btnAgregar", CssClass = "btn btn-success", Text = Custom.UI.ButtonAs.Names.Agregar };
                //btnAgregar.Click += Agregar;
                ////btnAgregar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                //if ((puedeAgregar.HasValue && !puedeAgregar.Value) || base.Id > 0)
                //    btnAgregar.Visible = false;
                #endregion

                #region Region del Listado en tabla HTML, muestra todos los registros de la tabla
                if (!puedeListar.HasValue || puedeListar.Value)
                {
                   // _Panel.Controls.Add(new LiteralControl("<nav><h2>" + title + "</h2></nav>"));
                    _Panel.Controls.Add(new LiteralControl("<input id='filtro' class='form-control' placeholder='Buscar...' style='margin: .5em 0 0;'/>"));
                    _Panel.Controls.Add(new LiteralControl("<section class='gridContainer'><table id='listado' class='jTable sortable filterable more'><thead><tr>"));
                    #region ListadoHeader
                    /* ----------------
                     * Agregando encabezados de listado en una tabla de HTML
                     * ----------------*/
                    foreach (KeyValuePair<string, string> headers in _Fields)
                    {


                        string key = headers.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                        if (Custom.UI.TableHTML.Campos == "*" || Custom.UI.TableHTML.Campos.Split(',').ToList().Contains(key))
                        {
                            if (key == "Id")
                                _Panel.Controls.Add(new LiteralControl("<th  class='unsortable'>" + key + "</th>"));
                            if (key.ToLower() == "userid" || !key.Contains("Id"))
                            {
                                if (key.Substring(key.IndexOf("-") + 1).Length > 0)
                                    _Panel.Controls.Add(new LiteralControl("<th  " + (key == "Avanza" || key == "Seleccionar" || key == "Listar" || key == "Modificar" || key == "Eliminar" || key == "Agregar" || key == "Acceso" || key == "Visible" ? "class='unsortable'" : "") + "    >" + Utils.SplitCamelCase(Translate(key).Replace("-", "")) + "</th>"));
                                else
                                    _Panel.Controls.Add(new LiteralControl("<th>" + Utils.SplitCamelCase(Translate(key.Substring(0, key.IndexOf("-")))) + "</th>"));
                            }
                        }

                    }
                    #endregion
                    _Panel.Controls.Add(new LiteralControl("</tr></thead>"));
                    _Panel.Controls.Add(new LiteralControl("<tbody id='toPaginador'>"));
                    #region ListadoBody
                    /* ----------------
                     * Agregando cuerpo de listado en una tabla de HTML
                     * ----------------*/
                    int limitRecords = 0;
                    int.TryParse(Request.QueryString["limit"] as string, out limitRecords);
                    if(limitRecords==0)
                        limitRecords = Custom.Core.Listado.MinRegistros;

                   
                        _Listado = model.Listado<T>().Take(limitRecords).ToList();
                    foreach (T item in _Listado)
                    {
                        _Panel.Controls.Add(new LiteralControl("<tr>"));
                        foreach (KeyValuePair<string, string> campo in _Fields)
                        {
                            string key = campo.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                            if (Custom.UI.TableHTML.Campos == "*" || Custom.UI.TableHTML.Campos.Split(',').ToList().Contains(key))
                            {
                                #region Campos de Tabla HTML

                                bool isDDL = campo.Key.Substring(0, 3) == "ddl";

                                object resultado = null;
                                if (!isDDL)
                                {
                                    Type tipoDePropiedad = Type.GetType("System." + campo.Value);
                                    PropertyInfo propiedad = item.GetType().GetProperty(key);
                                    if (campo.Value == "Decimal")
                                    {
                                        resultado = propiedad.GetValue(item, null) != null ? Math.Round((decimal)propiedad.GetValue(item, null), Custom.Core.Redondear).ToString("N" + Custom.Core.Redondear, CurrentCulture) : "";
                                    }
                                    else
                                        resultado = propiedad.GetValue(item, null) != null ? propiedad.GetValue(item, null).ToString() : "";
                                }
                                else
                                {
                                    object id = 0;

                                    Type tipoDePropiedad = Type.GetType("System." + campo.Value);

                                    PropertyInfo propiedad = null;
                                    string[] tablaPropiedad = key.Split('-');
                                    if (key.Substring(key.IndexOf("-") + 1).Length > 0)
                                    {
                                        //propiedad = item.GetType().GetProperty("Id" + key.Replace("-", ""));
                                        //key = key.Substring(0, key.IndexOf("-"));
                                        // correccion para tablas anidadas
                                        propiedad = item.GetType().GetProperty("Id" + tablaPropiedad[1]);
                                        key = tablaPropiedad[0];
                                    }
                                    else
                                    {
                                        propiedad = item.GetType().GetProperty("Id" + key.Substring(0, key.IndexOf("-")));
                                        key = key.Replace("-", "");
                                    }
                                    /* ------------------------------------------------------------------------
                                     * Excpeción controlada al llenar ddlList de tablas relacionadas asi mismas
                                     * ------------------------------------------------------------------------ */
                                    try
                                    {
                                        id = propiedad.GetValue(item, null);
                                    }
                                    catch { id = 0; }

                                    Type clase = Type.GetType(TDynamic.Namespace + "." + key);
                                    DbSet setClase = null;
                                    if (clase != null)
                                    {
                                        setClase = model.model.Set(clase);
                                        object instancia = setClase.Find(id);
                                        try
                                        {
                                            resultado = instancia.GetType().GetProperty("Descripcion").GetValue(instancia, null);
                                        }
                                        catch { resultado = ""; }
                                    }
                                }
                                if (key == "Id")
                                {
                                    if (!puedeListar.HasValue || puedeSeleccionar.Value)
                                        _Panel.Controls.Add(new LiteralControl("<td><a href='?Id=" + (resultado != null ? resultado.ToString() : "") + "'><b class='fa fa-edit'></b></a></td>"));
                                    else
                                        _Panel.Controls.Add(new LiteralControl("<td></td>"));
                                }
                                else
                                {
                                    if (key.ToLower() == "userid" || !key.Contains("Id"))
                                    {
                                        if (campo.Value != "Boolean")
                                            _Panel.Controls.Add(new LiteralControl("<td>" + (resultado != null ? resultado.ToString() : "") + "</td>"));
                                        else
                                        {
                                            switch (Custom.UI.BooleanAs.MostrarBoolenosComo)
                                            {
                                                case eBooleanAs.RadioButton:
                                                    _Panel.Controls.Add(new LiteralControl("<td>" + (resultado != null ? (resultado.ToString() == "True" ? "Si" : "No") : "") + "</td>"));
                                                    break;
                                                case eBooleanAs.CheckBox:
                                                    _Panel.Controls.Add(new LiteralControl("<td><input type='checkbox' " + (resultado != null ? (resultado.ToString() == "True" ? "checked" : "") : "") + "/></td>"));
                                                    break;
                                                case eBooleanAs.Toogle:
                                                    _Panel.Controls.Add(new LiteralControl("<td><b class='fa fa-toggle-" + (resultado != null ? (resultado.ToString() == "True" ? "on" : "off") : "off") + "' style='color: " + (resultado != null ? (resultado.ToString() == "True" ? "green" : "gray") : "gray") + "'></b></td>"));
                                                    break;
                                                default:
                                                    break;
                                            }

                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        _Panel.Controls.Add(new LiteralControl("</tr>"));
                    }
                    #endregion
                    _Panel.Controls.Add(new LiteralControl("</tbody></table></section>"));
                }
                #endregion
                #region Fin Integracion con Framwork ACE
                _Panel.Controls.Add(new LiteralControl("<!-- PAGE CONTENT ENDS --></div><!-- /.col --></div><!-- /.row --></div><!-- /.page-content -->"));
                #endregion
            }
            else
                _Panel.Controls.Add(new LiteralControl("<p>Disculpe los inconvenientes, ud no tiene acceso a esta página, contacte a su administrador...</p>"));
        }
        /// <summary>
        /// Obtiene una instancia para agregar, edición y/o eliminación de un elemento
        /// </summary>
        /// <returns>Instancia de T</returns>
        private T ObjectToUpdate()
        {
            T _;
            if (base.Id > 0)
                _ = model.Obtener<T>(base.Id);
            else
                _ = new T();
            #region Evalua todos los DropDownList de la página
            List<DropDownList> ddls = _Panel.Controls.OfType<DropDownList>().ToList();
            foreach (DropDownList ddl in ddls)
            {
                if(!ddl.ID.Contains("ddlRegistros"))
                if (ddl.Enabled)
                {
                    KeyValuePair<string, string> par = Fields.Where(x => x.Key.Replace("-", "") == ddl.ID.Replace("-", "")).FirstOrDefault();
                    int valor = 0;
                    int.TryParse(ddl.SelectedValue, out valor);
                    string key = par.Key;
                    Type.GetType("System.Int32");
                        string[] tablaPropiedad = key.Split('-');
                    if (key.Substring(key.IndexOf("-") + 1).Length > 0)
                        key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "").Replace("-", "");
                    else
                    {
                        key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                        key = key.Substring(0, key.IndexOf("-"));
                    }
                    //_.GetType().GetProperty("Id" + key).SetValue(_, Convert.ChangeType(valor, Type.GetType("System." + par.Value)), null);
                    //se mejora por campos de tablas relacionadas entre si
                        _.GetType().GetProperty("Id" + (tablaPropiedad[1].Length>0?tablaPropiedad[1]:key)).SetValue(_, Convert.ChangeType(valor, Type.GetType("System." + par.Value)), null);
                    }
            }
            #endregion
            #region Evalua todos los TextBox de la página
            List<TextBox> txts = _Panel.Controls.OfType<TextBox>().ToList();
            foreach (TextBox txt in txts)
            {
                if (txt.Enabled)
                {
                    KeyValuePair<string, string> par = Fields.Where(x => x.Key == txt.ID).FirstOrDefault();
                    string key = par.Key;
                    key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                    Type.GetType("System." + par.Value);
                    if (par.Value == "Guid")
                        _.GetType().GetProperty(key).SetValue(_, new Guid(txt.Text), null);
                    else
                        _.GetType().GetProperty(key).SetValue(_, Convert.ChangeType(txt.Text, Type.GetType("System." + par.Value)), null);
                }
            }
            #endregion
            #region Evalua todos los CheckBox y/o RadioButton segun la propiedad eBooleanAs
            switch (Custom.UI.BooleanAs.MostrarBoolenosComo)
            {
                case eBooleanAs.RadioButton:
                    {
                        List<RadioButton> rbls = _Panel.Controls.OfType<RadioButton>().ToList();
                        foreach (RadioButton rbt in rbls)
                        {
                            List<KeyValuePair<string, string>> pares = Fields.Where(x => x.Key == rbt.GroupName).ToList();
                            bool isSi = false, isNo = false;
                            foreach (KeyValuePair<string, string> par in pares)
                            {
                                if (rbt.Text == "Si" && rbt.Checked)
                                    isSi = true;
                                if (rbt.Text == "No" && rbt.Checked)
                                    isNo = true;
                                string key = par.Key;
                                key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                                Type.GetType("System." + par.Value);
                                if (isSi)
                                    _.GetType().GetProperty(key).SetValue(_, !isSi, null);
                                else
                                    _.GetType().GetProperty(key).SetValue(_, !isNo, null);
                                break;
                            }
                        }
                        break;
                    }
                case eBooleanAs.CheckBox:
                    {
                        List<CheckBox> chks = _Panel.Controls.OfType<CheckBox>().ToList();
                        foreach (CheckBox chk in chks)
                        {
                            KeyValuePair<string, string> par = Fields.Where(x => x.Key == chk.ID).FirstOrDefault();
                            string key = par.Key;
                            key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                            Type.GetType("System." + par.Value);
                            _.GetType().GetProperty(key).SetValue(_, chk.Checked, null);
                        }
                        break;
                    }
                case eBooleanAs.Toogle:
                    {
                        List<CheckBox> chks = _Panel.Controls.OfType<CheckBox>().ToList();
                        foreach (CheckBox chk in chks)
                        {
                            KeyValuePair<string, string> par = Fields.Where(x => x.Key == chk.ID).FirstOrDefault();
                            string key = par.Key;
                            key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                            Type.GetType("System." + par.Value);
                            _.GetType().GetProperty(key).SetValue(_, chk.Checked, null);
                        }
                        break;
                    }
                default:
                    break;
            }

            #endregion

            return _;
        }
        private string _Resultado = "";
        /// <summary>
        /// Notifica de los eventos que se realizan en caso Agregar, Modificar, Eliminar y en caso de error se notifica vía Exception.Message 
        /// </summary>
        public string Resultado
        {
            get { return _Resultado; }
            set { _Resultado = value;
                Mensaje = value; }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                Mensaje = "";
                if (lblEstatus != null)
                {
                    lblEstatus.Text = Mensaje;
                }
                if (base.Id > 0)
                {
                    FillCampos(model.Obtener<T>(base.Id));
                }
                
                RefreshListado();
                FillFields();
            }
        }
        

        protected string Mensaje
        {
            get
            {
                if (Session["mensaje"] != null)
                    return Session["mensaje"] as string;
                else
                    return string.Empty;
            }
            set
            {
                Session["mensaje"] = value;
            }
        }
        private void FillFields()
        {
            DropDownList ddlRegistros = _Panel.Controls.OfType<DropDownList>().Where(x => x.ID == "ddlRegistros").FirstOrDefault();
            if (ddlRegistros != null)
            {
                int[] items = { 10, 15, 20, 30, 50, 100, 200, 500, 1000, 5000, 10000, 100000, 1000000 };
                ddlRegistros.DataSource = items;
                ddlRegistros.DataBind();
                int limit = 10;
                int.TryParse(Request.QueryString["limit"] as string, out limit);
                ddlRegistros.Items.FindByText(limit > 0 ? limit.ToString() : "10").Selected = true;
            }
           
            
        }
        private void RefreshListado()
        {
            if (Custom.Core.Listado.OrdenDescendente)
                _Listado = model.Listado<T>().OrderByDescending(x => x.Id).Take(Custom.Core.Listado.MinRegistros).ToList();
            else
                _Listado = model.Listado<T>().Take(Custom.Core.Listado.MinRegistros).ToList();
        }
        private void FillCampos(T item)
        {
            try
            {
                #region Evalua todos los DropDownList de la página
                List<DropDownList> ddls = _Panel.Controls.OfType<DropDownList>().ToList();
                foreach (DropDownList ddl in ddls)
                {
                    if (!ddl.ID.Contains("ddlRegistros"))
                        if (ddl.Enabled)
                    {
                        KeyValuePair<string, string> par = Fields.Where(x => x.Key.Replace("-", "") == ddl.ID.Replace("-", "")).FirstOrDefault();
                        object result = null;
                        string key = par.Key;
                        Type.GetType("System.Int32");
                        if (key.Substring(key.IndexOf("-") + 1).Length > 0)
                        {
                            key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                            result = item.GetType().GetProperty("Id" + key.Replace("-", "")).GetValue(item, null);
                        }
                        else
                        {
                            key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                            result = item.GetType().GetProperty("Id" + key.Substring(0, key.IndexOf("-"))).GetValue(item, null);
                        }

                        if (ddl.Items.Count > 0)
                        {
                            ListItem seleccionar = ddl.Items.FindByValue(result.ToString());
                            if (seleccionar != null)
                                seleccionar.Selected = true;
                        }
                    }
                }
                #endregion
                #region Evalua todos los TextBox de la página
                List<TextBox> txts = _Panel.Controls.OfType<TextBox>().ToList();
                foreach (TextBox txt in txts)
                {
                    KeyValuePair<string, string> par = Fields.Where(x => x.Key == txt.ID).FirstOrDefault();
                    string key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                    Type.GetType("System." + par.Value);
                    object result = item.GetType().GetProperty(key).GetValue(item, null);
                    if (par.Value == "Decimal")
                        txt.Text = Math.Round((decimal)result, Custom.Core.Redondear).ToString();
                    else
                        txt.Text = result.ToString();
                }

                #endregion
                #region Evalua todos los CheckBox y/o RadioButton segun la propiedad eBooleanAs
                switch (Custom.UI.BooleanAs.MostrarBoolenosComo)
                {
                    case eBooleanAs.RadioButton:
                        {
                            List<RadioButton> rbls = _Panel.Controls.OfType<RadioButton>().ToList();
                            foreach (RadioButton rbt in rbls)
                            {
                                KeyValuePair<string, string> par = Fields.Where(x => x.Key == rbt.GroupName).FirstOrDefault();
                                string key = par.Key;
                                key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                                Type.GetType("System." + par.Value);
                                object result = item.GetType().GetProperty(key).GetValue(item, null);

                                if (rbt.Text == "Si" && (bool)result)
                                    rbt.Checked = true;
                                if (rbt.Text == "No" && !(bool)result)
                                    rbt.Checked = true;
                            }
                            break;
                        }
                    case eBooleanAs.CheckBox:
                        {
                            List<CheckBox> chks = _Panel.Controls.OfType<CheckBox>().ToList();
                            foreach (CheckBox chk in chks)
                            {
                                KeyValuePair<string, string> par = Fields.Where(x => x.Key == chk.ID).FirstOrDefault();
                                string key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                                Type.GetType("System." + par.Value);
                                object result = item.GetType().GetProperty(key).GetValue(item, null);
                                chk.Checked = (bool)result;
                            }
                            break;
                        }
                    case eBooleanAs.Toogle:
                        {
                            List<CheckBox> chks = _Panel.Controls.OfType<CheckBox>().ToList();
                            foreach (CheckBox chk in chks)
                            {
                                KeyValuePair<string, string> par = Fields.Where(x => x.Key == chk.ID).FirstOrDefault();
                                string key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                                Type.GetType("System." + par.Value);
                                object result = item.GetType().GetProperty(key).GetValue(item, null);
                                chk.Checked = (bool)result;
                            }
                            break;
                        }
                    default:
                        break;
                }
                #endregion

            }
            catch (Exception ex)
            {
                _Resultado = ex.Message;
            }
            finally
            {
                lblEstatus.Text = _Resultado;
            }
        }


        protected virtual void SeleccionRegistros(object sender, EventArgs e)
        {
            int maxRecords = int.Parse(((DropDownList)sender).SelectedValue);
            Response.Redirect(Request.Url.AbsolutePath.ToString() + "?limit=" + maxRecords);
            
            
        }

        protected virtual void Agregar(object sender, EventArgs e)
        {
            try
            {
                model.Agregar<T>(this.ObjectToUpdate());
                _Resultado = "Registro agregado satisfactoriamente...";
            }            
            catch (Exception ex) {

                 Mensaje = ex.Message;
                _Resultado = ex.Message; }
            finally
            {
                lblEstatus.Text = _Resultado;
                Limpiar(sender, e);

            }
        }
        protected virtual void Modificar(object sender, EventArgs e)
        {
            try
            {
                model.Modificar<T>(this.ObjectToUpdate());
                _Resultado = "Registro modificado satisfactoriamente...";
            }
            catch (Exception ex) { _Resultado = ex.Message; }
            finally
            {
                lblEstatus.Text = _Resultado;
                Limpiar(sender, e);

            }
        }
        protected virtual void Eliminar(object sender, EventArgs e)
        {
            try
            {
                T _ObjectToUpdate = model.Obtener<T>(base.Id);
                model.Eliminar<T>(_ObjectToUpdate);

                _Resultado = "Registro eliminado satisfactoriamente...";
            }
            catch (Exception ex) { _Resultado = ex.Message; }
            finally
            {
                lblEstatus.Text = _Resultado;
                Limpiar(sender, e);

            }
        }
        protected virtual void Limpiar(object sender, EventArgs e)
        {
            foreach (TextBox item in _Panel.Controls.OfType<TextBox>())
            {
                item.Text = "";
            }
            foreach (RadioButtonList radios in _Panel.Controls.OfType<RadioButtonList>())
            {
                foreach (ListItem radio in radios.Items)
                {
                    radio.Selected = false;
                }
            }
            foreach (CheckBoxList checks in _Panel.Controls.OfType<CheckBoxList>())
            {
                foreach (ListItem check in checks.Items)
                {
                    check.Selected = false;
                }
            }
            foreach (RadioButton rbt in _Panel.Controls.OfType<RadioButton>())
            {
                ((RadioButton)rbt).Checked = false;
            }
            foreach (CheckBox chk in _Panel.Controls.OfType<CheckBox>())
            {
                ((CheckBox)chk).Checked = false;
            }
            RefreshListado();
            /*
            Button btn = ((Button)sender);
            if (btn.Text == _NombreBotonLimpiar || btn.Text == _NombreBotonEliminar)
            {
                Response.Redirect(Request.Url.LocalPath, false);
            }
            else
            {
                Response.Redirect(Request.Url.AbsoluteUri, false);
            }
            */
            int urlCompleta = Request.Url.AbsoluteUri.Length;
            bool tieneId = (Request.Url.AbsoluteUri.ToLower().LastIndexOf("id=") == -1 ? false : true);
            if (tieneId)
                urlCompleta = urlCompleta - 5;
            Response.Redirect(Request.Url.AbsoluteUri.Substring(0, urlCompleta));
        }



        

    }
    public abstract class PageDynamicWithIdEmpresa<T>:AbstractPage where T : class, IId, IIdEmpresa, new()
    {

        internal string Translate(string name)
        {
            string finded = Resource.ResourceManager.GetString(name, CurrentCulture);
            return finded != null ? finded : name;
        }

        private CultureInfo _CurrentCulture = Resource.Culture;
        public CultureInfo CurrentCulture
        {
            get
            {
                return _CurrentCulture;

            }
            set { _CurrentCulture = value; }
        }


        /// <summary>
        /// Enumerativo que determinar la presentación usarán los datos de tipo Boolean
        /// </summary>
        public enum eBooleanAs { RadioButton, CheckBox, Toogle }
        /// <summary>
        /// Enumerativo que determinar la presentación usarán los botones de acciones para el CRUD
        /// </summary>
        public enum eButtonsAs { Button, LinkButton }
        internal class Custom
        {
            internal class Core
            {
                internal class Listado
                {
                    private static int _MinRegistros = 10;
                    /// <summary>
                    /// Cantidad de registros a retornar en el listado por defecto
                    /// </summary>
                    public static int MinRegistros
                    {
                        get { return _MinRegistros; }
                        set { _MinRegistros = value; }
                    }
                    private static int _MaxRegistros = 1000;
                    /// <summary>
                    /// Cantidad de registros a retornar en el listado por defecto
                    /// </summary>
                    public static int MaxRegistros
                    {
                        get { return _MaxRegistros; }
                        set { _MaxRegistros = value; }
                    }
                    private static bool _OrdenDescendente = false;
                    /// <summary>
                    /// Permite ordenar los registros de forma descendente por su Id, su orden por defecto será Ascendente, es decir _OrdenDescendente = false;
                    /// </summary>
                    public static bool OrdenDescendente
                    {
                        get { return _OrdenDescendente; }
                        set { _OrdenDescendente = value; }
                    }
                }
                private static short _Redondear = 2;
                /// <summary>
                /// Cantidad de decimales que serán usados para números decimales, su valor por defecto serán 2
                /// </summary>
                public static short Redondear
                {
                    get { return _Redondear; }
                    set { _Redondear = value; }
                }
            }
            internal class UI
            {
                internal class TableHTML
                {
                    private static string _Campos = "*";
                    /// <summary>
                    /// Indique nombre de campos separando por coma (,) los que serán mostradas en la Tabla HTML , por defecto mostrará todos los campos (*). Ej.: Id,Descripcion
                    /// </summary>
                    public static string Campos
                    {
                        get { return _Campos; }
                        set { _Campos = value; }
                    }
                    private static string _Titulo = string.Empty;
                    /// <summary>
                    /// Nombre que tendrá el titulo de la página, su valor por defecto está en blanco "String.Empty"
                    /// </summary>
                    public static string Titulo
                    {
                        get { return _Titulo; }
                        set { _Titulo = value; }
                    }
                }
                internal class BooleanAs
                {
                    private static eBooleanAs _MostrarBoolenosComo = eBooleanAs.Toogle;
                    /// <summary>
                    /// Permite controlar la presentación que usarán los datos de tipo Boolean, por defecto se usará eBooleanAs.CheckBox
                    /// </summary>
                    public static eBooleanAs MostrarBoolenosComo
                    {
                        get { return _MostrarBoolenosComo; }
                        set { _MostrarBoolenosComo = value; }
                    }
                }
                internal class TextBoxAs
                {
                    private static string _Color = "color";
                    /// <summary>
                    /// Indique nombre de campos separando por coma (,) los que activaran el selector de colores usado por el jscolor.min.js, por defecto se agregarà a la clase="jscolor". Ej.: Color
                    /// </summary>
                    public static string Color
                    {
                        get { return _Color; }
                        set { _Color = value; }
                    }
                    private static string _MultiLinea = "texto";
                    /// <summary>
                    /// Indique nombre de campos separando por coma (,) los que serán tipo "Multilinea", por defecto buscará campo llamado TEXTO. Ej.: Texto,Observacion,Descripcion
                    /// </summary>
                    public static string MultiLinea
                    {
                        get { return _MultiLinea; }
                        set { _MultiLinea = value; }
                    }
                    private static string _Fecha = "";
                    /// <summary>
                    /// Indique nombre de campos separando por coma (,) los que serán tipo "DateTime" se le agregará la clase ".datapicker", por defecto buscará campo llamado FECHA. Ej.: FechaInicio,FechaFin
                    /// </summary>
                    public static string Fecha
                    {
                        get
                        {
                            return _Fecha;
                        }

                        set
                        {
                            _Fecha = value;
                        }
                    }
                    private static string _MaxLength = "descripcion:80";
                    /// <summary>
                    /// Indique nombre de campos separando por coma (,) y dos puntos (:) el valor del MaxLenght por campo, a los cuales se les establecerá el atributo maxlength. Ej.: Descripcion:80,Observacion:255
                    /// </summary>
                    public static string MaxLength
                    {
                        get { return _MaxLength; }
                        set { _MaxLength = value; }
                    }

                    private static string _ValidationPattern = "";
                    /// <summary>
                    /// Indique nombre de campos separando por coma (,) y dos puntos (:) del valor de la clave del JS App.JS para el NS "App.Utils.Validation" por campo. Ej.: Descripcion:1,Observacion:0, donde "1" permite solo valores númericos y "0" una direccion url
                    /// </summary>
                    public static string ValidationPattern
                    {
                        get { return _ValidationPattern; }
                        set { _ValidationPattern = value; }
                    }
                }
                internal class ButtonAs
                {
                    internal class Names
                    {
                        private static string _Agregar = Resource.btnAdd;
                        /// <summary>
                        /// Nombre que tendrá el boton de btnAgregar, su valor por defecto es "Nuevo"
                        /// </summary>                        
                        public static string Agregar
                        {
                            get { return _Agregar; }
                            set { _Agregar = value; }
                        }
                        private static string _Modificar = Resource.btnEdit;
                        /// <summary>
                        /// Nombre que tendrá el boton de btnModificar, su valor por defecto es "Guardar"
                        /// </summary>                        
                        public static string Modificar
                        {
                            get { return _Modificar; }
                            set { _Modificar = value; }
                        }
                        private static string _Eliminar = Resource.btnDel;
                        /// <summary>
                        /// Nombre que tendrá el boton de btnEliminar, su valor por defecto es "Borrar"
                        /// </summary>
                        public static string Eliminar
                        {
                            get { return _Eliminar; }
                            set { _Eliminar = value; }
                        }
                        private static string _Limpiar = Resource.btnClear;
                        /// <summary>
                        /// Nombre que tendrá el boton de btnLimpiar, su valor por defecto es "Cancelar"
                        /// </summary>
                        public static string Limpiar
                        {
                            get { return _Limpiar; }
                            set { _Limpiar = value; }
                        }
                    }
                    private static eButtonsAs _MostrarBotonesComo = eButtonsAs.LinkButton;
                    /// <summary>
                    /// Permite controlar la presentación que usarán los botones de acciones para el CRUD, por defecto se usará eButtonsAs.LinkButton
                    /// </summary>
                    public static eButtonsAs MostrarBotonesComo
                    {
                        get { return _MostrarBotonesComo; }
                        set { _MostrarBotonesComo = value; }
                    }
                }

            }
        }

        internal Panel _Panel = new Panel();
        internal Panel _PanelCustoms = new Panel();
        /// <summary>
        /// Instancia del Panel que será usado para crear todos los elementos de la instancia del objeto recibido
        /// </summary>
        public Panel Panel
        {
            get { return _Panel; }
            set { _Panel = value; }
        }

        internal List<KeyValuePair<string, string>> _Fields = new List<KeyValuePair<string, string>>();
        /// <summary>
        /// Listado de campos y tipo de valor para a creación de los elementos y validar sus tipos de datos
        /// </summary>
        public List<KeyValuePair<string, string>> Fields
        {
            get { return _Fields; }
            set { _Fields = value; }
        }
        internal List<T> _Listado = new List<T>();
        /// <summary>
        /// Expone listado de T para usarlo en el metodo de actualizar listado
        /// </summary>
        public List<T> Listado
        {
            get { return _Listado; }
            set { _Listado = value; }
        }
        /// <summary>
        /// Instancia de Label para mostrar notificación de las operaciones básicas
        /// </summary>
        public Label lblEstatus
        {

            get
            {

                return _Panel.Controls.OfType<Label>().Where(x => x.ID == "lblEstatus").FirstOrDefault();
            }
        }
        /// <summary>
        /// Prepara la página para crear por Reflextion todos los campos y botones propios de la instancia del objeto recibido
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            Type TDynamic = null;
            base.CheckParametrosUrlQueryString();
            if (string.IsNullOrEmpty(base.Clase))
                TDynamic = typeof(T);
            else
                TDynamic = Type.GetType(typeof(T).Namespace + "." + base.Clase);

            /* ---------------------------------------------------
             * Lectura del esamblado y de la documentación en XML
             * --------------------------------------------------- */
            string ruta = HttpContext.Current.Server.MapPath(@"\bin\" + TDynamic.Assembly.ManifestModule.Name);
            Assembly dll = Assembly.LoadFrom(ruta);
            XDocument xml = XDocument.Load(ruta.Replace(".dll", ".xml"));
            var sumarios = xml.Descendants("member").Where(x => x.LastAttribute.Value.Substring(0, 2) == "P:" && x.LastAttribute.Value.Contains(TDynamic.Namespace)).ToList();
            var sumariosGeneric = xml.Descendants("member").Where(x => x.LastAttribute.Value.Contains("P:GenericRepository.PageDynamic`1.Custom")).ToList();
            _Panel = this.Controls.OfType<Panel>().FirstOrDefault();
            if (_Panel == null)
            {
                _Panel = new System.Web.UI.WebControls.Panel() { ID = "PN" };
                MasterPage masterPage = this.Master;
                HtmlForm form = null;
                form = this.Master.Controls.OfType<System.Web.UI.HtmlControls.HtmlForm>().FirstOrDefault();
                if (form == null)
                    form = this.Master.Master.Controls.OfType<System.Web.UI.HtmlControls.HtmlForm>().FirstOrDefault();
                ContentPlaceHolder cph = form.Controls.OfType<ContentPlaceHolder>().FirstOrDefault();
                /* ---------------- _PanelCustoms ------------------------
                 * Se agrega _PanelCustoms para los campos a personalizar
                 * ------------------------------------------------------- */
                //Type TypeOfGenericRespository = typeof(Custom);
                //_PanelCustoms = new System.Web.UI.WebControls.Panel() { ID = "PNCustoms" };
                //_PanelCustoms.Attributes.Add("style", "display:none");
                //_PanelCustoms.Controls.Add(new LiteralControl("<style>div#CPH_BODY_PNCustoms{padding:1em;}div#CPH_BODY_PNCustoms ul>li {width: 49%;display: inline-grid;padding: 1%;}span#closeCustoms {background: #62a8d1;padding: .2em .5em .5em;position: absolute;z-index: 1;right: 1em;cursor: pointer;border-bottom-left-radius: 10px;border-bottom-right-radius: 10px;color: white;}</style>"));

                //MemberInfo[] subClases = TypeOfGenericRespository.GetMembers(BindingFlags.NonPublic);
                //foreach (MemberInfo miembro in subClases)
                //{
                //    //Obtiene las Clases (UI,CORE)
                //    _PanelCustoms.Controls.Add(new LiteralControl(miembro.Name + "<br/>"));
                //    Type typeMiembro = Type.GetType(miembro.ReflectedType.FullName + "+" + miembro.Name);
                //    MemberInfo[] membersInEach = typeMiembro.GetMembers(BindingFlags.NonPublic);
                //    if (membersInEach.Length > 0) {
                //        //Evalue si tiene más SubClases
                //        foreach (MemberInfo memberInfoOf in membersInEach)
                //        {
                //            //Obtiene las subclases de UI (TextBoxAs,BooleanAs,ButtonAs,TableHTML)
                //            _PanelCustoms.Controls.Add(new LiteralControl(memberInfoOf.Name+"<br/>"));
                //            Type typeMemberInEach = Type.GetType(memberInfoOf.ReflectedType.FullName + "+" + memberInfoOf.Name);
                //            Type instanceGeneric = typeMemberInEach.MakeGenericType(new Type[1] { TDynamic });
                //            object instancia = Activator.CreateInstance(instanceGeneric, null);

                //            PropertyInfo[] propertyInfoOf = typeMemberInEach.GetProperties();
                //            _PanelCustoms.Controls.Add(new LiteralControl("<ul>"));
                //            foreach (PropertyInfo item in propertyInfoOf)
                //            {
                //                PropertyInfo[] propertinesInstancia = instancia.GetType().GetProperties();

                //                TextBox t = new TextBox() { ID = "txt" + item.Name.Replace(" ", ""), CssClass = "form-control" };
                //                t.Attributes.Add("placeholder", item.Name);
                //                var sumarioGeneric = sumariosGeneric.Where(x => x.LastAttribute.Value.Contains(item.Name)).FirstOrDefault();
                //                string txtSumarioLabel = (sumarioGeneric != null ? sumarioGeneric.Value : "");
                //                t.Attributes.Add("title", txtSumarioLabel);
                //                foreach (PropertyInfo itemProperty in propertinesInstancia)
                //                {
                //                    if (itemProperty.Name == item.Name)
                //                        t.Text = itemProperty.GetValue(instancia).ToString();
                //                }
                //                _PanelCustoms.Controls.Add(new LiteralControl("<li><b>" + item.Name + "</b>"));
                //                _PanelCustoms.Controls.Add(t);
                //                _PanelCustoms.Controls.Add(new LiteralControl("<i>"+txtSumarioLabel + "</i></li>"));
                //            }
                //            _PanelCustoms.Controls.Add(new LiteralControl("</ul>"));
                //        }
                //    }
                //    PropertyInfo[] propertyInEach = typeMiembro.GetProperties();
                //}
                //Button btnCustoms = new Button() { ID = "btnCustoms", CssClass = "btn btn-primary", Text = "Guardar ajustes" };
                //_PanelCustoms.Controls.Add(btnCustoms);
                //cph.Controls.Add(new LiteralControl("<span id='closeCustoms'><b class='fa fa-cogs'></b></span>"));
                //cph.Controls.Add(_PanelCustoms);

                cph.Controls.Add(_Panel);
            }


            /* ---------------------------------------------------
             * Acciones y permisos del rol
             * Activar si el sitio implementa seguridad por BBDD
             * ---------------------------------------------------
             *
             *  string urlActual = Request.Url.LocalPath;
             *  RolPagina rolEnPagina = null;
             *  try
             *  {
             *      rolEnPagina = UsuarioActual.Rol.RolPagina.Where(x => x.Pagina.Ruta.ToLower() == urlActual.ToLower()).FirstOrDefault();
             *  }
             *  catch { }
             *  bool? tieneAcceso = null, puedeSeleccionar = null, puedeListar = null, puedeAgregar = null, puedeModificar = null, puedeEliminar = null;
             *  if (rolEnPagina != null)
             *  {
             *      try
             *      {
             *          tieneAcceso = rolEnPagina.Acceso;
             *          puedeSeleccionar = rolEnPagina.Seleccionar;
             *          puedeListar = rolEnPagina.Listar;
             *          puedeAgregar = rolEnPagina.Agregar;
             *          puedeModificar = rolEnPagina.Modificar;
             *          puedeEliminar = rolEnPagina.Eliminar;
             *      }
             *      catch { }
             *  }
             *  else
             *      tieneAcceso = false;
             *
             * --------------------------------------------------- */



            /* ---------------------------------------------------
             * Desactivar si el sitio será controlado por BBDD
             * --------------------------------------------------- */
            bool? tieneAcceso = true, puedeSeleccionar = true, puedeListar = true, puedeAgregar = true, puedeModificar = true, puedeEliminar = true;




            #region Definición del título de la página a partir de la entidad
            /* ------------------------------
             * Agregando título a la pagina
             * ------------------------------ */
            string title = Translate("lblMaster");
            string end = TDynamic.Name.Substring(TDynamic.Name.Length - 1).ToLower();
            string nameof = " " + Utils.SplitCamelCase(Translate(TDynamic.Name));
            switch (end)
            {
                case "s":
                    title += nameof;
                    break;
                case "n":
                    title += nameof + "es";
                    break;
                case "r":
                    title += nameof + "es";
                    break;
                case "l":
                    title += nameof + "es";
                    break;
                default:
                    title += nameof + "s";
                    break;
            }
            if (string.IsNullOrEmpty(Custom.UI.TableHTML.Titulo))
                this.Page.Title = title;
            else
            {
                this.Page.Title = Custom.UI.TableHTML.Titulo;
                title = Custom.UI.TableHTML.Titulo;
            }
            #endregion

            string[] labelsToTranslate = { "lblOpenPanelEdit", "lblLimitRecords" };



            base.OnInit(e);


            if (!tieneAcceso.HasValue || tieneAcceso.Value)
            {
                /* ---------------------------------------------------------------------------------------------------------------
                 * Solo filtrara campos Read/Write, ya que los campos Extendidos de solo lectura generaban error por no tener Set;
                 * ---------------------------------------------------------------------------------------------------------------*/
                PropertyInfo[] propiedades = TDynamic.GetProperties().Where(x => x.SetMethod != null).ToArray();
                #region Inicio Integracion con Framwork ACE
                _Panel.Controls.Add(new LiteralControl("<div class='breadcrumbs ace-save-state' id='breadcrumbs'><ul class='breadcrumb'><li><i class='ace-icon fa fa-home home-icon'></i><a href='/pages/'>Home</a></li><li class='active'>" + title + "</li></ul><!-- /.breadcrumb --></div><div class='page-content'><div class='page-header'><h1>" + title + "<small><i class='ace-icon fa fa-angle-double-right'></i>Información</small></h1></div><!-- /.page-header --><div class='row'><div class='col-xs-12'><!-- PAGE CONTENT BEGINS -->"));
                #endregion
                #region Region del Mantenimiento para hacer CRUD de los registros
                _Panel.Controls.Add(new LiteralControl("<p id='btnToogleEditPanel'><b class='fa fa-edit'></b>" + Translate(labelsToTranslate[0]) + "</p><div id='editPanel' style='display: none'><span id='closeEditPanel' ><b class='fa fa-times'></b></span>"));
                //_Panel.Controls.Add(new LiteralControl("<nav><h4>Gestión de datos</h4></nav>"));
                _Panel.Controls.Add(new LiteralControl("<table class='table'><tbody>"));
                #region Campos para Agregar y/o Editar la información de los registros seleccionados o por crear

                foreach (PropertyInfo propiedad in propiedades)
                {
                    string tipo = "";
                    string nombre = "";
                    if (propiedad.PropertyType.GetGenericArguments().Count() > 0)
                    {
                        tipo = propiedad.PropertyType.GetGenericArguments()[0].Name;
                    }
                    else
                    {
                        tipo = propiedad.PropertyType.Name;
                    }
                    /* ----------------
                     * Agregando campos de texto y/o checkbox a la pagina
                     * ----------------*/
                    if (TDynamic.Namespace == propiedad.PropertyType.Namespace)
                    {
                        nombre = propiedad.PropertyType.Name;
                        var sumarioPropiedad = sumarios.Where(x => x.LastAttribute.Value.Contains(TDynamic.FullName + "." + nombre)).FirstOrDefault();


                        _Fields.Add(new KeyValuePair<string, string>("ddl" + nombre + "-" + propiedad.Name.Replace(nombre, ""), "Int32"));
                        _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(Translate(propiedad.Name)) + "</b><p>" + (sumarioPropiedad != null ? sumarioPropiedad.Value.Trim() : "") + "</p></td><td>"));
                        string typeName = propiedad.PropertyType.Namespace + "." + nombre;
                        Type clase = Type.GetType(typeName);


                        IRequiredFields obj = (IRequiredFields)Activator.CreateInstance(clase);


                        //AssemblyName assemblyName = dll.GetName();
                        //System.Reflection.Emit.AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, System.Reflection.Emit.AssemblyBuilderAccess.Run);                        
                        //System.Reflection.Emit.ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(dll.Modules.FirstOrDefault().Name);
                        //System.Reflection.Emit.TypeBuilder tb = moduleBuilder.DefineType(typeName, TypeAttributes.Public);
                        //tb.SetParent(clase);
                        //tb.AddInterfaceImplementation(typeof(IActivo));


                        obj.Descripcion = "( -- " + Translate("Seleccione un item de") + Translate(clase.Name) + "-- )";
                        obj.Id = -1;

                        DbSet setClase = null;
                        if (clase != null)
                        {
                            setClase = model.model.Set(clase);
                            setClase.Load();
                        }
                        DropDownList t = new DropDownList()
                        {
                            ID = "ddl" + nombre + "-" + propiedad.Name.Replace(nombre, ""),
                            DataTextField = "Descripcion",
                            DataValueField = "Id",
                            CssClass = "form-control"
                        };
                        switch (clase.Name)
                        {
                            case "Empresa": {
                                    Empresa objEmp = (Empresa)Activator.CreateInstance(clase);
                                    objEmp.Descripcion = "( -- " + Translate("Seleccione un item de") + Translate(clase.Name) + "-- )";
                                    objEmp.Id = -1;
                                    IList<Empresa> listado = UsuarioActual.EmpresaUsuario.Select(x => x.Empresa).ToList();
                                    listado.Add(objEmp);
                                    t.DataSource = listado.OrderBy(x => x.Descripcion);
                                    t.DataBind();
                                    break;
                                }
                            case "Producto":
                                {
                                    Producto objEmp = (Producto)Activator.CreateInstance(clase);
                                    objEmp.Descripcion = "( -- " + Translate("Seleccione un item de") + Translate(clase.Name) + "-- )";
                                    objEmp.Id = -1;
                                    IList<Producto> listado = setClase != null ? setClase.Local.Cast<Producto>()
                                        .Where(x => x.Activo.Value && x.IdEmpresa == EmpresaActual.IdEmpresa.Value).ToList() : null;
                                    listado.Add(objEmp);
                                    t.DataSource = listado.OrderBy(x => x.Descripcion);
                                    t.DataBind();
                                    break;
                                }
                            case "Tarifa":
                                {
                                    Tarifa objEmp = (Tarifa)Activator.CreateInstance(clase);
                                    objEmp.Descripcion = "( -- " + Translate("Seleccione un item de") + Translate(clase.Name) + "-- )";
                                    objEmp.Id = -1;
                                    IList<Tarifa> listado = setClase != null ? setClase.Local.Cast<Tarifa>()
                                        .Where(x => x.Activo.Value && x.IdEmpresa == EmpresaActual.IdEmpresa.Value).ToList() : null;
                                    listado.Add(objEmp);
                                    t.DataSource = listado.OrderBy(x => x.Descripcion);
                                    t.DataBind();
                                    break;
                                }
                            case "ServicioAdministracion":
                                {
                                    ServicioAdministracion objEmp = (ServicioAdministracion)Activator.CreateInstance(clase);
                                    objEmp.Descripcion = "( -- " + Translate("Seleccione un item de") + Translate(clase.Name) + "-- )";
                                    objEmp.Id = -1;
                                    IList<ServicioAdministracion> listado = setClase != null ? setClase.Local.Cast<ServicioAdministracion>()
                                        .Where(x => x.Activo.Value && x.IdEmpresa == EmpresaActual.IdEmpresa.Value).ToList() : null;
                                    listado.Add(objEmp);
                                    t.DataSource = listado.OrderBy(x => x.Descripcion);
                                    t.DataBind();
                                    break;
                                }
                            case "Interviniente":
                                {
                                    Interviniente objInt = (Interviniente)Activator.CreateInstance(clase);
                                    objInt.Descripcion = "( -- " + Translate("Seleccione un item de") + Translate(clase.Name) + "-- )";
                                    objInt.Id = -1;
                                    IList<Interviniente> listado = setClase != null ? setClase.Local.Cast<Interviniente>()
                                        .Where(x => x.Activo.Value && x.IdEmpresa==EmpresaActual.IdEmpresa.Value).ToList() : null;
                                    listado.Add(objInt);
                                    t.DataSource = listado.OrderBy(x => x.Descripcion);
                                    t.DataBind();
                                    break;
                                }
                            default:
                                {
                                    IList<IRequiredFields> listado = setClase != null ? setClase.Local.Cast<IRequiredFields>()
                               .Where(x => x.Activo.Value).ToList() : null;
                                    listado.Add(obj);
                                    t.DataSource = listado.OrderBy(x => x.Descripcion);
                                    t.DataBind();
                                    break;
                                }
                        }
                        
                        _Panel.Controls.Add(t);
                        _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                    }

                    if (propiedad.PropertyType.Namespace == "System")
                    {
                        nombre = propiedad.Name;
                        var sumarioPropiedad = sumarios.Where(x => x.LastAttribute.Value.Contains(TDynamic.FullName + "." + nombre)).FirstOrDefault();
                        /* ----------------
                         * Leyendo la Descripcion de la clase Info:System.Attribute
                         * ----------------*/
                        string labelDescripcion = "";
                        labelDescripcion = (sumarioPropiedad != null ? sumarioPropiedad.Value.Trim() : "");

                        if (tipo == "String" || tipo == "Int32" || tipo == "DateTime" || tipo == "Decimal" || tipo == "Float" || tipo == "Guid")
                        {
                            _Fields.Add(new KeyValuePair<string, string>("txt" + nombre, tipo));
                            TextBox t = new TextBox() { ID = "txt" + nombre.Replace(" ", ""), CssClass = "form-control" };

                            //Establece el tipo Multilinea a los campos indicados en la propiedad Custom.UI.TextBoxAs.MultiLinea
                            foreach (string item in Custom.UI.TextBoxAs.MultiLinea.ToLower().Split(','))
                            {
                                if (nombre.ToLower() == item)
                                    t.TextMode = TextBoxMode.MultiLine;
                            }

                            //Establece el Maximo de caracteres permitido a los campos indicados en la propiedad Custom.UI.TextBoxAs.MaxLength
                            foreach (string item in Custom.UI.TextBoxAs.MaxLength.ToLower().Split(','))
                            {
                                string[] campo = item.Split(':');
                                if (nombre.ToLower() == campo[0])
                                    t.MaxLength = int.Parse(campo[1]);
                            }

                            //Establece el Patron de Validacion de JS, somando el patron de NS App.Utils.Validation.Pattern 
                            foreach (string item in Custom.UI.TextBoxAs.ValidationPattern.ToLower().Split(','))
                            {
                                string[] campo = item.Split(':');
                                if (nombre.ToLower() == campo[0])
                                    t.Attributes.Add("validation", campo[1]);
                            }

                            //Establece nombre de clase "jscolor" a la propiedad class, para los campos que activan selector de color
                            foreach (string item in Custom.UI.TextBoxAs.Color.ToLower().Split(','))
                            {
                                string[] campo = item.Split(':');
                                if (nombre.ToLower() == campo[0])
                                    t.CssClass += " jscolor";
                            }

                            t.Attributes.Add("placeHolder", Utils.SplitCamelCase(Translate(nombre)));
                            if (nombre == "Id")
                            {
                                _Panel.Controls.Add(new LiteralControl("<tr class='help' style='display:none'><td  class='info'><b>" + Utils.SplitCamelCase(Translate(nombre)) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                                t.Enabled = false;
                                _Panel.Controls.Add(t);
                                _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                            }
                            else
                                if (nombre.ToLower() == "userid" || !nombre.Contains("Id"))
                            {
                                //Establece un JQuery de DateTimePicker para los campos indicados en la propiedad Custom.UI.TextBoxAs.Fecha
                                if (Custom.UI.TextBoxAs.Fecha.Length > 0 && Custom.UI.TextBoxAs.Fecha.Split(',').ToList().Contains(nombre))
                                {
                                    _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(Translate(nombre)) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                                    _Panel.Controls.Add(new LiteralControl("<div class='input-group date date-picker'>"));
                                    t.CssClass += " date-picker";
                                    _Panel.Controls.Add(t);
                                    _Panel.Controls.Add(new LiteralControl("<span class='input-group-addon'><span class='fa fa-calendar-o'></span></span></div></td></tr>"));
                                }
                                else
                                {

                                    _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(Translate(nombre)) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                                    _Panel.Controls.Add(t);
                                    _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                                }
                            }
                        }
                        if (tipo == "Boolean")
                        {
                            /* ----------------
                             * Evalua enumerativo "eBooleanAs" para determinar que presentación usarán los datos de tipo Boolean, por defecto se usará eBooleanAs.CheckBox
                             * ----------------*/
                            switch (Custom.UI.BooleanAs.MostrarBoolenosComo)
                            {
                                case eBooleanAs.RadioButton:
                                    {
                                        _Fields.Add(new KeyValuePair<string, string>("rbt" + nombre, tipo));
                                        _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(Translate(nombre)) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                                        RadioButton t = new RadioButton() { GroupName = "rbt" + nombre.Replace(" ", ""), Text = "Si" };
                                        _Panel.Controls.Add(t);
                                        t = new RadioButton() { GroupName = "rbt" + nombre.Replace(" ", ""), Text = "No" };
                                        _Panel.Controls.Add(t);
                                        _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                                        break;
                                    }
                                case eBooleanAs.CheckBox:
                                    {
                                        _Fields.Add(new KeyValuePair<string, string>("chk" + nombre, tipo));
                                        _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(Translate(nombre)) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                                        CheckBox t = new CheckBox() { ID = "chk" + nombre.Replace(" ", "") };
                                        _Panel.Controls.Add(t);
                                        _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                                        break;
                                    }
                                case eBooleanAs.Toogle:
                                    {
                                        _Fields.Add(new KeyValuePair<string, string>("chk" + nombre, tipo));
                                        _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(Translate(nombre)) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                                        CheckBox t = new CheckBox() { ID = "chk" + nombre.Replace(" ", "") };
                                        _Panel.Controls.Add(t);
                                        _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }



                    }
                    //if (TDynamic.Namespace == propiedad.PropertyType.Namespace)
                    //    sb.AppendLine("<b>Objeto Nombre:</b>" + propiedad.Name);
                    //if (propiedad.PropertyType.Namespace == "System.Collections.Generic")
                    //    sb.AppendLine("<b>Lista Nombre:</b>" + propiedad.Name);

                    //sb.AppendLine("<br>");
                    //Response.Write(sb.ToString());
                   
                } 
                #endregion
                _Panel.Controls.Add(new LiteralControl("<tr><td colspan='2'>"));
                Label lblEstatus = new Label() { ID = "lblEstatus" };
                _Panel.Controls.Add(lblEstatus);
                _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                _Panel.Controls.Add(new LiteralControl("<tr><td colspan='2' style='text-align: right;position:relative'>"));
                #region Botones de accion para Agregar, Modificar, Eliminar y/o Cancelar
                /* ----------------
                 * Agregando botones de acciones a la página
                 * ----------------*/

                switch (Custom.UI.ButtonAs.MostrarBotonesComo)
                {
                    case eButtonsAs.Button:
                        {
                            Button btnEliminar = new Button() { ID = "btnEliminar", CssClass = "btn btn-danger", Text = Custom.UI.ButtonAs.Names.Eliminar };
                            btnEliminar.Attributes.Add("style", "position: absolute;  left:0");
                            btnEliminar.Click += Eliminar;
                            if ((puedeEliminar.HasValue && !puedeEliminar.Value) || base.Id < 1)
                                btnEliminar.Visible = false;

                            if ((puedeEliminar.HasValue && !puedeEliminar.Value) || base.Id < 1)
                                btnEliminar.Visible = false;
                            _Panel.Controls.Add(btnEliminar);

                            Button btnModificar = new Button() { ID = "btnModificar", CssClass = "btn btn-primary", Text = Custom.UI.ButtonAs.Names.Modificar };
                            btnModificar.Click += Modificar;
                            //btnModificar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                            if ((puedeModificar.HasValue && !puedeModificar.Value) || base.Id < 1)
                                btnModificar.Visible = false;

                            _Panel.Controls.Add(btnModificar);

                            Button btnAgregar = new Button() { ID = "btnAgregar", CssClass = "btn btn-success", Text = Custom.UI.ButtonAs.Names.Agregar };
                            btnAgregar.Click += Agregar;
                            //btnAgregar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                            if ((puedeAgregar.HasValue && !puedeAgregar.Value) || base.Id > 0)
                                btnAgregar.Visible = false;

                            _Panel.Controls.Add(btnAgregar);

                            Button btnLimpiar = new Button() { ID = "btnLimpiar", CssClass = "btn btn-default", Text = Custom.UI.ButtonAs.Names.Limpiar };
                            btnLimpiar.Click += Limpiar;
                            _Panel.Controls.Add(btnLimpiar);
                            break;
                        }
                    case eButtonsAs.LinkButton:
                        {
                            LinkButton btnEliminar = new LinkButton() { ID = "btnEliminar", CssClass = "btn btn-danger", Text = "<b class='fa fa-times' ></b>&nbsp;" + Custom.UI.ButtonAs.Names.Eliminar };
                            btnEliminar.Attributes.Add("style", "position: absolute;  left:0");
                            btnEliminar.Click += Eliminar;
                            if ((puedeEliminar.HasValue && !puedeEliminar.Value) || base.Id < 1)
                                btnEliminar.Visible = false;
                            if ((puedeEliminar.HasValue && !puedeEliminar.Value) || base.Id < 1)
                                btnEliminar.Visible = false;
                            _Panel.Controls.Add(btnEliminar);

                            LinkButton btnModificar = new LinkButton() { ID = "btnModificar", CssClass = "btn btn-primary", Text = "<b class='fa fa-save' ></b>&nbsp;" + Custom.UI.ButtonAs.Names.Modificar };
                            btnModificar.Click += Modificar;
                            //btnModificar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                            if ((puedeModificar.HasValue && !puedeModificar.Value) || base.Id < 1)
                                btnModificar.Visible = false;
                            _Panel.Controls.Add(btnModificar);

                            LinkButton btnAgregar = new LinkButton() { ID = "btnAgregar", CssClass = "btn btn-success", Text = "<b class='fa fa-plus-circle' ></b>&nbsp;" + Custom.UI.ButtonAs.Names.Agregar };
                            btnAgregar.Click += Agregar;
                            //btnAgregar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                            if ((puedeAgregar.HasValue && !puedeAgregar.Value) || base.Id > 0)
                                btnAgregar.Visible = false;
                            _Panel.Controls.Add(btnAgregar);

                            LinkButton btnLimpiar = new LinkButton() { ID = "btnLimpiar", CssClass = "btn btn-default", Text = Custom.UI.ButtonAs.Names.Limpiar };
                            btnLimpiar.Click += Limpiar;
                            _Panel.Controls.Add(btnLimpiar);
                            break;
                        }
                    default:
                        break;
                }

                #endregion
                _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                _Panel.Controls.Add(new LiteralControl("</tbody></table></div>"));
                #endregion
                #region Paginacion con Entity
                DropDownList ddlRegistros = new DropDownList() { ID = "ddlRegistros", CssClass = "form-control", AutoPostBack = true };
                ddlRegistros.SelectedIndexChanged += SeleccionRegistros;
                _Panel.Controls.Add(new LiteralControl("<table></tr><td>" + Translate(labelsToTranslate[1]) + "</td><td>"));
                _Panel.Controls.Add(ddlRegistros);
                _Panel.Controls.Add(new LiteralControl("</td></tr></table>"));
                //Button btnAgregar = new Button() { ID = "btnAgregar", CssClass = "btn btn-success", Text = Custom.UI.ButtonAs.Names.Agregar };
                //btnAgregar.Click += Agregar;
                ////btnAgregar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                //if ((puedeAgregar.HasValue && !puedeAgregar.Value) || base.Id > 0)
                //    btnAgregar.Visible = false;
                #endregion

                #region Region del Listado en tabla HTML, muestra todos los registros de la tabla
                if (!puedeListar.HasValue || puedeListar.Value)
                {
                    // _Panel.Controls.Add(new LiteralControl("<nav><h2>" + title + "</h2></nav>"));
                    _Panel.Controls.Add(new LiteralControl("<input id='filtro' class='form-control' placeholder='Buscar...' style='margin: .5em 0 0;'/>"));
                    _Panel.Controls.Add(new LiteralControl("<section class='gridContainer'><table id='listado' class='jTable sortable filterable more'><thead><tr>"));
                    #region ListadoHeader
                    /* ----------------
                     * Agregando encabezados de listado en una tabla de HTML
                     * ----------------*/
                    foreach (KeyValuePair<string, string> headers in _Fields)
                    {


                        string key = headers.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                        if (Custom.UI.TableHTML.Campos == "*" || Custom.UI.TableHTML.Campos.Split(',').ToList().Contains(key))
                        {
                            if (key == "Id")
                                _Panel.Controls.Add(new LiteralControl("<th  class='unsortable'>" + key + "</th>"));
                            if (key.ToLower() == "userid" || !key.Contains("Id"))
                            {
                                if (key.Substring(key.IndexOf("-") + 1).Length > 0)
                                    _Panel.Controls.Add(new LiteralControl("<th  " + (key == "Avanza" || key == "Seleccionar" || key == "Listar" || key == "Modificar" || key == "Eliminar" || key == "Agregar" || key == "Acceso" || key == "Visible" ? "class='unsortable'" : "") + "    >" + Utils.SplitCamelCase(Translate(key).Replace("-", "")) + "</th>"));
                                else
                                    _Panel.Controls.Add(new LiteralControl("<th>" + Utils.SplitCamelCase(Translate(key.Substring(0, key.IndexOf("-")))) + "</th>"));
                            }
                        }

                    }
                    #endregion
                    _Panel.Controls.Add(new LiteralControl("</tr></thead>"));
                    _Panel.Controls.Add(new LiteralControl("<tbody id='toPaginador'>"));
                    #region ListadoBody
                    /* ----------------
                     * Agregando cuerpo de listado en una tabla de HTML
                     * ----------------*/
                    int limitRecords = 0;
                    int.TryParse(Request.QueryString["limit"] as string, out limitRecords);
                    if (limitRecords == 0)
                        limitRecords = Custom.Core.Listado.MinRegistros;
                    int idEmpresa = (EmpresaActual != null ? EmpresaActual.IdEmpresa.Value : 0);
                    _Listado = model.Listado<T>()
                    .Where(x => x.IdEmpresa == idEmpresa)
                    .Take(limitRecords)
                    .ToList();
                    foreach (T item in _Listado)
                    {
                        _Panel.Controls.Add(new LiteralControl("<tr>"));
                        foreach (KeyValuePair<string, string> campo in _Fields)
                        {
                            string key = campo.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                            if (Custom.UI.TableHTML.Campos == "*" || Custom.UI.TableHTML.Campos.Split(',').ToList().Contains(key))
                            {
                                #region Campos de Tabla HTML

                                bool isDDL = campo.Key.Substring(0, 3) == "ddl";

                                object resultado = null;
                                if (!isDDL)
                                {
                                    Type tipoDePropiedad = Type.GetType("System." + campo.Value);
                                    PropertyInfo propiedad = item.GetType().GetProperty(key);
                                    if (campo.Value == "Decimal")
                                    {
                                        resultado = propiedad.GetValue(item, null) != null ? Math.Round((decimal)propiedad.GetValue(item, null), Custom.Core.Redondear).ToString("N" + Custom.Core.Redondear, CurrentCulture) : "";
                                    }
                                    else
                                        resultado = propiedad.GetValue(item, null) != null ? propiedad.GetValue(item, null).ToString() : "";
                                }
                                else
                                {
                                    object id = 0;

                                    Type tipoDePropiedad = Type.GetType("System." + campo.Value);

                                    PropertyInfo propiedad = null;
                                    if (key.Substring(key.IndexOf("-") + 1).Length > 0)
                                    {
                                        propiedad = item.GetType().GetProperty("Id" + key.Replace("-", ""));
                                        key = key.Substring(0, key.IndexOf("-"));
                                    }
                                    else
                                    {
                                        propiedad = item.GetType().GetProperty("Id" + key.Substring(0, key.IndexOf("-")));
                                        key = key.Replace("-", "");
                                    }
                                    /* ------------------------------------------------------------------------
                                     * Excpeción controlada al llenar ddlList de tablas relacionadas asi mismas
                                     * ------------------------------------------------------------------------ */
                                    try
                                    {
                                        id = propiedad.GetValue(item, null);
                                    }
                                    catch { id = 0; }

                                    Type clase = Type.GetType(TDynamic.Namespace + "." + key);
                                    DbSet setClase = null;
                                    if (clase != null)
                                    {
                                        setClase = model.model.Set(clase);
                                        object instancia = setClase.Find(id);
                                        try
                                        {
                                            resultado = instancia.GetType().GetProperty("Descripcion").GetValue(instancia, null);
                                        }
                                        catch { resultado = ""; }
                                    }
                                }
                                if (key == "Id")
                                {
                                    if (!puedeListar.HasValue || puedeSeleccionar.Value)
                                        _Panel.Controls.Add(new LiteralControl("<td><a href='?Id=" + (resultado != null ? resultado.ToString() : "") + "'><b class='fa fa-edit'></b></a></td>"));
                                    else
                                        _Panel.Controls.Add(new LiteralControl("<td></td>"));
                                }
                                else
                                {
                                    if (key.ToLower() == "userid" || !key.Contains("Id"))
                                    {
                                        if (campo.Value != "Boolean")
                                            _Panel.Controls.Add(new LiteralControl("<td>" + (resultado != null ? resultado.ToString() : "") + "</td>"));
                                        else
                                        {
                                            switch (Custom.UI.BooleanAs.MostrarBoolenosComo)
                                            {
                                                case eBooleanAs.RadioButton:
                                                    _Panel.Controls.Add(new LiteralControl("<td>" + (resultado != null ? (resultado.ToString() == "True" ? "Si" : "No") : "") + "</td>"));
                                                    break;
                                                case eBooleanAs.CheckBox:
                                                    _Panel.Controls.Add(new LiteralControl("<td><input type='checkbox' " + (resultado != null ? (resultado.ToString() == "True" ? "checked" : "") : "") + "/></td>"));
                                                    break;
                                                case eBooleanAs.Toogle:
                                                    _Panel.Controls.Add(new LiteralControl("<td><b class='fa fa-toggle-" + (resultado != null ? (resultado.ToString() == "True" ? "on" : "off") : "off") + "' style='color: " + (resultado != null ? (resultado.ToString() == "True" ? "green" : "gray") : "gray") + "'></b></td>"));
                                                    break;
                                                default:
                                                    break;
                                            }

                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        _Panel.Controls.Add(new LiteralControl("</tr>"));
                    }
                    #endregion
                    _Panel.Controls.Add(new LiteralControl("</tbody></table></section>"));
                }
                #endregion
                #region Fin Integracion con Framwork ACE
                _Panel.Controls.Add(new LiteralControl("<!-- PAGE CONTENT ENDS --></div><!-- /.col --></div><!-- /.row --></div><!-- /.page-content -->"));
                #endregion
            }
            else
                _Panel.Controls.Add(new LiteralControl("<p>Disculpe los inconvenientes, ud no tiene acceso a esta página, contacte a su administrador...</p>"));
        }

        /// <summary>
        /// Obtiene una instancia para agregar, edición y/o eliminación de un elemento
        /// </summary>
        /// <returns>Instancia de T</returns>
        private T ObjectToUpdate()
        {
            T _;
            if (base.Id > 0)
                _ = model.Obtener<T>(base.Id);
            else
                _ = new T();
            #region Evalua todos los DropDownList de la página
            List<DropDownList> ddls = _Panel.Controls.OfType<DropDownList>().ToList();
            foreach (DropDownList ddl in ddls)
            {
                if (!ddl.ID.Contains("ddlRegistros"))
                    if (ddl.Enabled)
                    {
                        KeyValuePair<string, string> par = Fields.Where(x => x.Key.Replace("-", "") == ddl.ID.Replace("-", "")).FirstOrDefault();
                        int valor = 0;
                        int.TryParse(ddl.SelectedValue, out valor);
                        string key = par.Key;
                        Type.GetType("System.Int32");
                        if (key.Substring(key.IndexOf("-") + 1).Length > 0)
                            key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "").Replace("-", "");
                        else
                        {
                            key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                            key = key.Substring(0, key.IndexOf("-"));
                        }
                        _.GetType().GetProperty("Id" + key).SetValue(_, Convert.ChangeType(valor, Type.GetType("System." + par.Value)), null);
                    }
            }
            #endregion
            #region Evalua todos los TextBox de la página
            List<TextBox> txts = _Panel.Controls.OfType<TextBox>().ToList();
            foreach (TextBox txt in txts)
            {
                if (txt.Enabled)
                {
                    KeyValuePair<string, string> par = Fields.Where(x => x.Key == txt.ID).FirstOrDefault();
                    string key = par.Key;
                    key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                    Type.GetType("System." + par.Value);
                    if (par.Value == "Guid")
                        _.GetType().GetProperty(key).SetValue(_, new Guid(txt.Text), null);
                    else
                        _.GetType().GetProperty(key).SetValue(_, Convert.ChangeType(txt.Text, Type.GetType("System." + par.Value)), null);
                }
            }
            #endregion
            #region Evalua todos los CheckBox y/o RadioButton segun la propiedad eBooleanAs
            switch (Custom.UI.BooleanAs.MostrarBoolenosComo)
            {
                case eBooleanAs.RadioButton:
                    {
                        List<RadioButton> rbls = _Panel.Controls.OfType<RadioButton>().ToList();
                        foreach (RadioButton rbt in rbls)
                        {
                            List<KeyValuePair<string, string>> pares = Fields.Where(x => x.Key == rbt.GroupName).ToList();
                            bool isSi = false, isNo = false;
                            foreach (KeyValuePair<string, string> par in pares)
                            {
                                if (rbt.Text == "Si" && rbt.Checked)
                                    isSi = true;
                                if (rbt.Text == "No" && rbt.Checked)
                                    isNo = true;
                                string key = par.Key;
                                key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                                Type.GetType("System." + par.Value);
                                if (isSi)
                                    _.GetType().GetProperty(key).SetValue(_, !isSi, null);
                                else
                                    _.GetType().GetProperty(key).SetValue(_, !isNo, null);
                                break;
                            }
                        }
                        break;
                    }
                case eBooleanAs.CheckBox:
                    {
                        List<CheckBox> chks = _Panel.Controls.OfType<CheckBox>().ToList();
                        foreach (CheckBox chk in chks)
                        {
                            KeyValuePair<string, string> par = Fields.Where(x => x.Key == chk.ID).FirstOrDefault();
                            string key = par.Key;
                            key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                            Type.GetType("System." + par.Value);
                            _.GetType().GetProperty(key).SetValue(_, chk.Checked, null);
                        }
                        break;
                    }
                case eBooleanAs.Toogle:
                    {
                        List<CheckBox> chks = _Panel.Controls.OfType<CheckBox>().ToList();
                        foreach (CheckBox chk in chks)
                        {
                            KeyValuePair<string, string> par = Fields.Where(x => x.Key == chk.ID).FirstOrDefault();
                            string key = par.Key;
                            key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                            Type.GetType("System." + par.Value);
                            _.GetType().GetProperty(key).SetValue(_, chk.Checked, null);
                        }
                        break;
                    }
                default:
                    break;
            }

            #endregion

            return _;
        }
        private string _Resultado = "";
        /// <summary>
        /// Notifica de los eventos que se realizan en caso Agregar, Modificar, Eliminar y en caso de error se notifica vía Exception.Message 
        /// </summary>
        public string Resultado
        {
            get { return _Resultado; }
            set
            {
                _Resultado = value;
                Mensaje = value;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                Mensaje = "";
                if (lblEstatus != null)
                {
                    lblEstatus.Text = Mensaje;
                }
                if (base.Id > 0)
                {
                    FillCampos(model.Obtener<T>(base.Id));
                }

                RefreshListado();
                FillFields();
            }
        }


        protected string Mensaje
        {
            get
            {
                if (Session["mensaje"] != null)
                    return Session["mensaje"] as string;
                else
                    return string.Empty;
            }
            set
            {
                Session["mensaje"] = value;
            }
        }
        private void FillFields()
        {
            DropDownList ddlRegistros = _Panel.Controls.OfType<DropDownList>().Where(x => x.ID == "ddlRegistros").FirstOrDefault();
            if (ddlRegistros != null)
            {
                int[] items = { 10, 15, 20, 30, 50, 100, 200, 500, 1000, 5000, 10000, 100000, 1000000 };
                ddlRegistros.DataSource = items;
                ddlRegistros.DataBind();
                int limit = 10;
                int.TryParse(Request.QueryString["limit"] as string, out limit);
                ddlRegistros.Items.FindByText(limit > 0 ? limit.ToString() : "10").Selected = true;
            }


        }
        private void RefreshListado()
        {
            if (Custom.Core.Listado.OrdenDescendente)
                _Listado = model.Listado<T>().OrderByDescending(x => x.Id).Take(Custom.Core.Listado.MinRegistros).ToList();
            else
                _Listado = model.Listado<T>().Take(Custom.Core.Listado.MinRegistros).ToList();
        }
        private void FillCampos(T item)
        {
            try
            {
                #region Evalua todos los DropDownList de la página
                List<DropDownList> ddls = _Panel.Controls.OfType<DropDownList>().ToList();
                foreach (DropDownList ddl in ddls)
                {
                    if (!ddl.ID.Contains("ddlRegistros"))
                        if (ddl.Enabled)
                        {
                            KeyValuePair<string, string> par = Fields.Where(x => x.Key.Replace("-", "") == ddl.ID.Replace("-", "")).FirstOrDefault();
                            object result = null;
                            string key = par.Key;
                            Type.GetType("System.Int32");
                            if (key.Substring(key.IndexOf("-") + 1).Length > 0)
                            {
                                key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                                result = item.GetType().GetProperty("Id" + key.Replace("-", "")).GetValue(item, null);
                            }
                            else
                            {
                                key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                                result = item.GetType().GetProperty("Id" + key.Substring(0, key.IndexOf("-"))).GetValue(item, null);
                            }

                            if (ddl.Items.Count > 0)
                            {
                                ListItem seleccionar = ddl.Items.FindByValue(result.ToString());
                                if (seleccionar != null)
                                    seleccionar.Selected = true;
                            }
                        }
                }
                #endregion
                #region Evalua todos los TextBox de la página
                List<TextBox> txts = _Panel.Controls.OfType<TextBox>().ToList();
                foreach (TextBox txt in txts)
                {
                    KeyValuePair<string, string> par = Fields.Where(x => x.Key == txt.ID).FirstOrDefault();
                    string key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                    Type.GetType("System." + par.Value);
                    object result = item.GetType().GetProperty(key).GetValue(item, null);
                    if (par.Value == "Decimal")
                        txt.Text = Math.Round((decimal)result, Custom.Core.Redondear).ToString();
                    else
                        txt.Text = result.ToString();
                }

                #endregion
                #region Evalua todos los CheckBox y/o RadioButton segun la propiedad eBooleanAs
                switch (Custom.UI.BooleanAs.MostrarBoolenosComo)
                {
                    case eBooleanAs.RadioButton:
                        {
                            List<RadioButton> rbls = _Panel.Controls.OfType<RadioButton>().ToList();
                            foreach (RadioButton rbt in rbls)
                            {
                                KeyValuePair<string, string> par = Fields.Where(x => x.Key == rbt.GroupName).FirstOrDefault();
                                string key = par.Key;
                                key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                                Type.GetType("System." + par.Value);
                                object result = item.GetType().GetProperty(key).GetValue(item, null);

                                if (rbt.Text == "Si" && (bool)result)
                                    rbt.Checked = true;
                                if (rbt.Text == "No" && !(bool)result)
                                    rbt.Checked = true;
                            }
                            break;
                        }
                    case eBooleanAs.CheckBox:
                        {
                            List<CheckBox> chks = _Panel.Controls.OfType<CheckBox>().ToList();
                            foreach (CheckBox chk in chks)
                            {
                                KeyValuePair<string, string> par = Fields.Where(x => x.Key == chk.ID).FirstOrDefault();
                                string key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                                Type.GetType("System." + par.Value);
                                object result = item.GetType().GetProperty(key).GetValue(item, null);
                                chk.Checked = (bool)result;
                            }
                            break;
                        }
                    case eBooleanAs.Toogle:
                        {
                            List<CheckBox> chks = _Panel.Controls.OfType<CheckBox>().ToList();
                            foreach (CheckBox chk in chks)
                            {
                                KeyValuePair<string, string> par = Fields.Where(x => x.Key == chk.ID).FirstOrDefault();
                                string key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                                Type.GetType("System." + par.Value);
                                object result = item.GetType().GetProperty(key).GetValue(item, null);
                                chk.Checked = (bool)result;
                            }
                            break;
                        }
                    default:
                        break;
                }
                #endregion

            }
            catch (Exception ex)
            {
                _Resultado = ex.Message;
            }
            finally
            {
                lblEstatus.Text = _Resultado;
            }
        }


        protected virtual void SeleccionRegistros(object sender, EventArgs e)
        {
            int maxRecords = int.Parse(((DropDownList)sender).SelectedValue);
            Response.Redirect(Request.Url.AbsolutePath.ToString() + "?limit=" + maxRecords);


        }

        protected virtual void Agregar(object sender, EventArgs e)
        {
            try
            {
                model.Agregar<T>(this.ObjectToUpdate());
                _Resultado = "Registro agregado satisfactoriamente...";
            }
            catch (Exception ex)
            {

                Mensaje = ex.Message;
                _Resultado = ex.Message;
            }
            finally
            {
                lblEstatus.Text = _Resultado;
                Limpiar(sender, e);

            }
        }
        protected virtual void Modificar(object sender, EventArgs e)
        {
            try
            {
                model.Modificar<T>(this.ObjectToUpdate());
                _Resultado = "Registro modificado satisfactoriamente...";
            }
            catch (Exception ex) { _Resultado = ex.Message; }
            finally
            {
                lblEstatus.Text = _Resultado;
                Limpiar(sender, e);

            }
        }
        protected virtual void Eliminar(object sender, EventArgs e)
        {
            try
            {
                T _ObjectToUpdate = model.Obtener<T>(base.Id);
                model.Eliminar<T>(_ObjectToUpdate);

                _Resultado = "Registro eliminado satisfactoriamente...";
            }
            catch (Exception ex) { _Resultado = ex.Message; }
            finally
            {
                lblEstatus.Text = _Resultado;
                Limpiar(sender, e);

            }
        }
        protected virtual void Limpiar(object sender, EventArgs e)
        {
            foreach (TextBox item in _Panel.Controls.OfType<TextBox>())
            {
                item.Text = "";
            }
            foreach (RadioButtonList radios in _Panel.Controls.OfType<RadioButtonList>())
            {
                foreach (ListItem radio in radios.Items)
                {
                    radio.Selected = false;
                }
            }
            foreach (CheckBoxList checks in _Panel.Controls.OfType<CheckBoxList>())
            {
                foreach (ListItem check in checks.Items)
                {
                    check.Selected = false;
                }
            }
            foreach (RadioButton rbt in _Panel.Controls.OfType<RadioButton>())
            {
                ((RadioButton)rbt).Checked = false;
            }
            foreach (CheckBox chk in _Panel.Controls.OfType<CheckBox>())
            {
                ((CheckBox)chk).Checked = false;
            }
            RefreshListado();
            /*
            Button btn = ((Button)sender);
            if (btn.Text == _NombreBotonLimpiar || btn.Text == _NombreBotonEliminar)
            {
                Response.Redirect(Request.Url.LocalPath, false);
            }
            else
            {
                Response.Redirect(Request.Url.AbsoluteUri, false);
            }
            */
            int urlCompleta = Request.Url.AbsoluteUri.Length;
            bool tieneId = (Request.Url.AbsoluteUri.ToLower().LastIndexOf("id=") == -1 ? false : true);
            if (tieneId)
                urlCompleta = urlCompleta - 5;
            Response.Redirect(Request.Url.AbsoluteUri.Substring(0, urlCompleta));
        }

    }
    //public class ReadFromExcel
    //{
    //    public static List<T> GetFile<T>(string path) where T: class,new() 
    //    {
    //        Excel.Application xlApp = new Excel.Application();
    //        Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(path);
    //        Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
    //        Excel.Range xlRange = xlWorksheet.UsedRange;
    //        int rowCount = xlRange.Rows.Count;
    //        int colCount = xlRange.Columns.Count;
    //        List<T> items = new List<T>();
    //        dynamic[][] excel = new dynamic[rowCount][];
    //        try
    //        {
    //            dynamic[] headers = new dynamic[colCount];
    //            T item =null;
    //            for (int i = 0; i <= rowCount; i++)
    //            {
    //                if (i > 0 )
    //                    item = new T();
    //                dynamic[] datacolumnas = new dynamic[colCount];
    //                for (int j = 0; j <= colCount; j++)
    //                {
    //                    if (xlRange.Cells[i+1, j+1] != null && xlRange.Cells[i+1, j+1].Text != null)
    //                    {
    //                        try
    //                        {
    //                            datacolumnas[j] = xlRange.Cells[i + 1, j + 1].Text;
    //                            if (i == 0)
    //                                headers[j] = xlRange.Cells[i + 1, j + 1].Text;
    //                            else
    //                            {
    //                                PropertyInfo[] propiedadesEntidad = item.GetType().GetProperties();
    //                                string nameOfColumnFromExcel = headers[j];
    //                                PropertyInfo propiedadBuscada = propiedadesEntidad.Where(x => x.Name.ToLower() == nameOfColumnFromExcel.Replace(" ", "").ToLower()).FirstOrDefault();
    //                                if (propiedadBuscada != null)
    //                                    propiedadBuscada.SetValue(item, xlRange.Cells[i + 1, j + 1].Text);
    //                            }
    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            continue;
    //                        }
    //                    }
    //                }
    //                if (i > 0 && i < rowCount)
    //                    items.Add(item);
    //               excel[i] = datacolumnas;
    //            }
    //        }
    //        catch (Exception ex)
    //        {

    //        }
    //        finally {
    //            GC.Collect();
    //            GC.WaitForPendingFinalizers();
    //            Marshal.ReleaseComObject(xlRange);
    //            Marshal.ReleaseComObject(xlWorksheet);
    //            xlWorkbook.Close();
    //            Marshal.ReleaseComObject(xlWorkbook);
    //            xlApp.Quit();
    //            Marshal.ReleaseComObject(xlApp);
    //        }
    //        return items;
    //    }
    //}
    public partial class Utils
    {
        /// <summary>
        /// Permite llenar controles tipo listados
        /// </summary>
        /// <typeparam name="T">Entidad del modelo EDM</typeparam>
        /// <param name="ctrl">Elemento de lista</param>
        /// <param name="datos">Origen de datos a llenar la propiedad DataSource del elemento de lista</param>
        /// <param name="todos">Indica si se agrega la opción "( -- Todos -- )" al listado, y su valor es "0"</param>
        /// <param name="seleccionar">Indica si se agrega la opción "( -- Seleccionar -- )" al listado, y su valor es "-1"</param>
        /// <param name="orderByDescripcion">Indica si la opción de ordenación es aplicada al campo Descripción</param>
        public static void Llenar<T>(ListControl ctrl, List<T> datos, bool todos = false, bool seleccionar = false, bool orderByDescripcion = false) where T : IRequiredFields, new()
        {
            Type tipo = datos.GetType();
            List<T> t = datos;
            if (todos)
                t.Add(new T() { Id = 0, Descripcion = "( -- Todos -- )" });
            if (seleccionar)
                t.Add(new T() { Id = -1, Descripcion = "( -- Seleccione un item de " + tipo.GenericTypeArguments[0].Name + " -- )" });
            ctrl.DataTextField = "Descripcion";
            ctrl.DataValueField = "Id";
            if (orderByDescripcion)
                ctrl.DataSource = t.OrderBy(x => x.Descripcion);
            else
                ctrl.DataSource = t.OrderBy(x => x.Id);

            ctrl.DataBind();
        }

        public static DataTable ReadExcelFile(string nameSheet, string _excelFilePath)
        {
            string commandText = "SELECT * FROM [" + nameSheet + "$]";
            OleDbConnectionStringBuilder cb = new OleDbConnectionStringBuilder();
            cb.DataSource = _excelFilePath;
            if (Path.GetExtension(_excelFilePath).ToUpper() == ".XLS")
            {
                cb.Provider = "Microsoft.Jet.OLEDB.4.0";
                cb.Add("Extended Properties", "Excel 8.0;HDR=YES;IMEX=0;");
            }
            else if (Path.GetExtension(_excelFilePath).ToUpper() == ".XLSX")
            {
                cb.Provider = "Microsoft.ACE.OLEDB.12.0";
                cb.Add("Extended Properties", "Excel 12.0 Xml;HDR=YES;IMEX=0;");
            }
            DataTable dt = new DataTable("Datos");
            using (OleDbConnection conn = new OleDbConnection(cb.ConnectionString))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = commandText;
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(dt);
                }
                conn.Close();
            }
            return dt;

            /* COMO USAR: */
            //HttpPostedFile file = this.txtFileUpLoad.PostedFile;
            //string path = Server.MapPath("../administracion/resources/noticias/" + file.FileName);
            //file.SaveAs(path);
            //ExcelHandler ex = new ExcelHandler();
            //foreach (DataRow row in ex.ReadExcelFile("Routes", path).Rows)
            //{
            //    Route item = new Route
            //    {
            //        RouteUrl = row["RouteUrl"].ToString(),
            //        PhysicalPath = row["PhysicalPath"].ToString(),
            //        IsGroupAdminPage = Convert.ToBoolean(row["IsGroupAdminPage"].ToString()),
            //        Descripcion = txtDescripcion.Text,
            //        PreFix = txtPrefix.Text
            //    };
            //    roots.Agregar<Route>(item);
            //}
        }

        public static string Duracion(DateTime desde, DateTime hasta)
        {
            TimeSpan span = hasta - desde;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("{0} {1}", years, years == 1 ? "año" : "años");
            }
            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("{0} {1}", months, months == 1 ? "mes" : "meses");
            }
            if (span.Days > 0)
                return String.Format("{0} {1}", span.Days, span.Days == 1 ? "día" : "días");
            if (span.Hours > 0)
                return String.Format("{0} {1}", span.Hours, span.Hours == 1 ? "hora" : "horas");
            if (span.Minutes > 0)
                return String.Format("{0} {1}", span.Minutes, span.Minutes == 1 ? "minuto" : "minutos");
            if (span.Seconds > 5)
                return String.Format("{0} segundos", span.Seconds);
            if (span.Seconds <= 5)
                return "ahora";
            return string.Empty;
        }
        public static string Duracion(DateTime desde, DateTime hasta, bool hhmmss)
        {
            var diff = hasta.Subtract(desde);
            return String.Format("{0}:{1}:{2}", diff.Hours.ToString().PadLeft(2, '0'), diff.Minutes.ToString().PadLeft(2, '0'), diff.Seconds.ToString().PadLeft(2, '0'));
        }
        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }
        public static string CamelCase(string input)
        {
            return input.Substring(0, 1).ToUpper() + input.Substring(1, input.Length - 1).ToLower();
        }
        public static string FirstWorkLowerRestPascalCase( string input, char split)
        {
            StringBuilder newWord = new StringBuilder();
            string[] words = input.Split(split);
            newWord.Append(words[0].ToLower());
            for (int i = 1; i < words.Length; i++)
            {
                string fixedWord = words[i];
                newWord.Append(fixedWord.Substring(0, 1).ToUpper() + fixedWord.Substring(1, fixedWord.Length - 1).ToLower());
            }
            return newWord.ToString();
        }
        public static T[][] SplitArray<T>(IEnumerable<T> data, int limit)
        {
            IEnumerable<T> itemList = data;
            decimal iteraciones = Math.Ceiling((decimal)itemList.Count() / limit);
            T[][] lotes = new T[(int)iteraciones][];
            for (int i = 0; i < iteraciones; i++)
            {
                lotes[i] = itemList.Skip(limit * i).Take(limit).ToArray();
            }
            return lotes;
        }
        public static long ToJulian(DateTime dateTime)
        {
            DateTime firstDayOfYear = new DateTime(dateTime.Year, 1, 1);
            TimeSpan diferencia = dateTime - firstDayOfYear;

            int year = dateTime.Year - 1900;
           
            
            string result = string.Format("{0}{1}",year,(diferencia.Days+1).ToString().PadLeft(3,'0'));
            return Convert.ToInt32( result);
            
        }

        public static string ToGregorian(long julianDate, string format)
        {
            long L = julianDate + 68569;
            long N = (long)((4 * L) / 146097);
            L = L - ((long)((146097 * N + 3) / 4));
            long I = (long)((4000 * (L + 1) / 1461001));
            L = L - (long)((1461 * I) / 4) + 31;
            long J = (long)((80 * L) / 2447);
            int Day = (int)(L - (long)((2447 * J) / 80));
            L = (long)(J / 11);
            int Month = (int)(J + 2 - 12 * L);
            int Year = (int)(100 * (N - 49) + I + L);

            // example format "dd/MM/yyyy"
            return new DateTime(Year, Month, Day).ToString(format);
        }

        public static string GetMyTable<T>(IEnumerable<T> list, params Expression<Func<T, object>>[] fxns)
        {
            T firstRow = list.FirstOrDefault();

            StringBuilder sb = new StringBuilder();
            sb.Append("<table id='listado' class='jTable sortable filterable more'>\n");

            sb.Append("<thead><tr>\n");

            IEnumerable<FieldInfo> info = firstRow.GetType().GetRuntimeFields();

           FieldInfo dic= info.ToList()[0];
            try
            {
                foreach (var item in (Dictionary<string, object>)dic.GetValue(firstRow))
                {
                    sb.Append("<th>");
                    sb.Append(item.Key);
                    sb.Append("</th>");
                }
            }
            catch (Exception)
            {

               
            }


            //foreach (PropertyInfo column in firstRow.GetType().GetProperties())
            //{
            //    sb.Append("<th>");
            //    sb.Append(column.Name);
            //    sb.Append("</th>");
            //}

            //foreach (var fxn in fxns)
            //{
            //    sb.Append("<TD>");
            //    sb.Append(GetName(fxn));
            //    sb.Append("</TD>");
            //}
            sb.Append("</tr></thead><tbody>");


            foreach (var item in list)
            {
                sb.Append("<tr>\n");
                try
                {
                    foreach (var col in (Dictionary<string, object>)dic.GetValue(item))
                    {
                        sb.Append("<td>");
                        sb.Append(col.Value);
                        sb.Append("</td>");
                    }
                }
                catch (Exception)
                {

                    
                }
                


                //foreach (PropertyInfo column in item.GetType().GetProperties())
                //{
                //    sb.Append("<td>");
                //    sb.Append(column.GetValue(item, null));
                //    sb.Append("</td>");
                //}

                //foreach (var fxn in fxns)
                //{
                //    sb.Append("<TD>");
                //    sb.Append(fxn.Compile()(item));
                //    sb.Append("</TD>");
                //}
                sb.Append("</tr>\n");
            }
            sb.Append("</tbody></table>");

            return sb.ToString();
        }

        static string GetName<T>(Expression<Func<T, object>> expr)
        {
            var member = expr.Body as MemberExpression;
            if (member != null)
                return GetName2(member);

            var unary = expr.Body as UnaryExpression;
            if (unary != null)
                return GetName2((MemberExpression)unary.Operand);

            return "?+?";
        }

        static string GetName2(MemberExpression member)
        {
            var fieldInfo = member.Member as FieldInfo;
            if (fieldInfo != null)
            {
                var d = fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (d != null) return d.Description;
                return fieldInfo.Name;
            }

            var propertInfo = member.Member as PropertyInfo;
            if (propertInfo != null)
            {
                var d = propertInfo.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (d != null) return d.Description;
                return propertInfo.Name;
            }

            return "?-?";
        }

        public IEnumerable<T> GetResultFromSP<T>(string spname, string paramInt = "0") where T : class, new()
        {
            List<T> list = new List<T>();
            //Tools.SQLUtilities sqlUtils = new Tools.SQLUtilities();
            string sqlComm = string.Format("exec {0} {1}", spname, paramInt);

            DataSet data = null;// sqlUtils.GetDataSet(Tools.Common.getConnectionWithoutproviderStringSecond(), sqlComm);

            if (data != null && data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in data.Tables[0].Rows)
                {
                    list.Add(CreateItemFromRow<T>(item));
                }
            }
            return list;
        }


        // function that creates an object from the given data row
        public static T CreateItemFromRow<T>(DataRow row) where T : new()
        {
            T item = new T();
            SetItemFromRow(item, row);
            return item;
        }

        public static void SetItemFromRow<T>(T item, DataRow row) where T : new()
        {
            foreach (DataColumn c in row.Table.Columns)
            {
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName);
                if (p != null && row[c] != DBNull.Value)
                {
                    p.SetValue(item, row[c], null);
                }
            }
        }
        public static object GetInstance(string strFullyQualifiedName)
        {
            Type type = Type.GetType(strFullyQualifiedName);
            if (type != null)
                return Activator.CreateInstance(type);
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(strFullyQualifiedName);
                if (type != null)
                    return Activator.CreateInstance(type);
            }
            return null;
        }
        public int NonNullPropertiesCount(object entity)
        {
            return entity.GetType()
                         .GetProperties()
                         .Select(x => x.GetValue(entity, null))
                         .Count(v => v != null);
        }

        public static object DeserializeJson<T>(string Json)
        {
            JavaScriptSerializer JavaScriptSerializer = new JavaScriptSerializer();
            Dictionary<string, object> json = null;
            json = JavaScriptSerializer.Deserialize<Dictionary<string, object>>(Json);
            KeyValuePair<string, object> first = json.First();
            dynamic oJson = new ExpandoObject();
            dynamic oProperties = new ExpandoObject();
            Dictionary<string, object> props = (Dictionary<string, object>)first.Value;
            foreach (KeyValuePair<string, object> item in props.ToList())
                ((IDictionary<string, object>)oProperties).Add(item.Key, item.Value);
            ((IDictionary<string, object>)oJson).Add(first.Key, oProperties);
            return oJson;
        }
        public static object DeserializeJson<T>(string Json,string keyToFind)
        {
            JavaScriptSerializer JavaScriptSerializer = new JavaScriptSerializer();
            Dictionary<string, object> json = null;
            json = JavaScriptSerializer.Deserialize<Dictionary<string, object>>(Json);
            KeyValuePair<string, object> first = json.Where(x=>x.Key==keyToFind).First();
            dynamic oJson = new ExpandoObject();
            dynamic oProperties = new ExpandoObject();
            Dictionary<string, object> props = (Dictionary<string, object>)first.Value;
            foreach (KeyValuePair<string, object> item in props.ToList())
                ((IDictionary<string, object>)oProperties).Add(item.Key, item.Value);
            ((IDictionary<string, object>)oJson).Add(first.Key, oProperties);
            return oJson;
        }
    }

    namespace EF5
    {
        /// <summary>
        /// Interfaz que asegura la existencia del campo Id, permite ordenar por el campo Id en forma descendente
        /// </summary>
        public interface IId
        {
            int Id { get; set; }
        }
       
        /// <summary>
        /// Interfaz que asegura la existencia del campo Id, Descripcion y Activo
        /// </summary>
        public interface IRequiredFields
        {
            bool? Activo { get; set; }
            string Descripcion { get; set; }
            int Id { get; set; }
        }

        public class Information : System.Attribute
        {
            public string Version;
            public string Creador;
            public string Colaborador;
            public string Descripcion;
            public string Creado;
            public Information()
            {
                Creado = "20/03/2015";
                Version = "1.0.0.0";
                Creador = "Jorge L. Torres A.";
            }
        }
        /// <summary>
        /// Clase generica que recibe un contexto y puede trabajar con cualer clase del model
        /// </summary>
        /// <typeparam name="TContext">Object del Contexto del modelo de entity EDM</typeparam>
        [Information(Descripcion = "Clase generica que recibe un contexto y puede trabajar con cualer clase del model")]
        public partial class GenericRepository<TContext> where TContext : DbContext, new()
        {
            /// <summary>
            /// Crea una nueva instancia del Contexto
            /// </summary>
            protected DbContext _context;
            public GenericRepository()
            {
                _context = new TContext();
            }
            /// <summary>
            /// Crea una nueva instancia del Contexto apartir de una instancia ya creada
            /// </summary>
            /// <param name="instanceOfDBContext">Instancia del DBContext ya creada</param>
            public GenericRepository(DbContext instanceOfDBContext)
            {
                _context = instanceOfDBContext;
            }
            /// <summary>
            /// Expone objecto Database del Contexto, para ejecutar metodos propios del contexto
            /// .SqlQuery<T>(string sql);
            /// .ExecuteSqlCommand(string sql,params object[] parameters)
            /// </summary>
            public Database DBContext
            {
                get
                {
                    return _context.Database;
                }
            }
            /// <summary>
            /// Instancia expuesta para hacer uso directo del contexto instanciado
            /// </summary>
            public DbContext model
            {
                get { return _context; }
            }
            /// <summary>
            /// Retorna objeto solicitado por su Id
            /// </summary>
            /// <typeparam name="T">Type de la clase solicitada</typeparam>
            /// <param name="id">Id del objeto a consultar</param>
            /// <returns>Returna instancia del objeto del tipo T</returns>
            [Obsolete("Se recomienda usar T Obtener<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : class", false)]
            public virtual T Obtener<T>(int id) where T : class
            {
                //return Listado<T>().Where(x => x.Id == id).FirstOrDefault();
                //Error: Varias instancias de IEntityChangeTracker no pueden hacer referencia a un objeto entidad.
                //Solución: agregar ".AsNoTracking()"
                //return Listado<T>().Where(x => x.Id == id).AsNoTracking().FirstOrDefault();
                return _context.Set<T>().Find(id);
            }
            /// <summary>
            /// Retorna objeto solicitado filtrando el valpor el predicado de Linq
            /// </summary>
            /// <typeparam name="T">Type de la clase solicitada</typeparam>
            /// <param name="predicate">Expression(Func(T, bool)) LinQ que permite efectuar un filtro por ejemplo: predicado sería (x=>x.Id==1 && x.Fecha.Value==DateTime.Now.Year)</param>
            /// <param name="includes">IEnumerable(string) con listado de las propieades de Navegación y/o tablas dependientes del objeto que se esté recuperando</param>
            /// <returns>Returna instancia del objeto del tipo T</returns>
            [Obsolete("Se recomienda usar T Obtener<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : class", false)]
            public T Obtener<T>(Expression<Func<T, bool>> predicate, IEnumerable<string> includePaths) where T : class
            {
                IQueryable<T> query = _context.Set<T>().AsNoTracking();
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }
                if (includePaths != null)
                {
                    query = includePaths.Aggregate(query, (current, includePath) => current.Include<T>(includePath));
                }
                return query.FirstOrDefault();
            }
            /// <summary>
            /// Retorna objeto solicitado filtrando el valpor el predicado de Linq
            /// </summary>
            /// <typeparam name="T">Type de la clase solicitada</typeparam>
            /// <param name="predicate">Expression(Func(T, bool)) LinQ que permite efectuar un filtro por ejemplo: predicado sería (x=>x.Id==1 && x.Fecha.Value==DateTime.Now.Year)</param>
            /// <param name="includes">Expression(Func(T, bool)) LinQ Permite incluir las propieades de Navegación y/o tablas dependientes del objeto que se esté recuperando</param>
            /// <returns>Returna instancia del objeto del tipo T</returns>
            public T Obtener<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : class
            {
                var result = Listado<T>(includes);
                if (includes.Any())
                {
                    foreach (var include in includes)
                    {
                        result = result.Include(include);
                    }
                }
                return result.FirstOrDefault(predicate);
            }
            /// <summary>
            /// Permite obtener un IQueryable para ejecutar un listado del Type de la clase solicitada, y el resutaldo implementa .AsNoTracking()
            /// controla que no genere error: Varias instancias de IEntityChangeTracker no pueden hacer referencia a un objeto entidad
            /// </summary>
            /// <typeparam name="T">Type de la clase solicitada</typeparam>
            /// <returns>IQueriable para ejecutar un .ToList()</returns>
            public virtual IQueryable<T> Listado<T>() where T : class
            {
                return _context.Set<T>().AsNoTracking();
            }
            /// <summary>
            /// Permite obtener un IQueryable para ejecutar un listado del Type de la clase solicitada, y el resutaldo implementa .AsNoTracking()
            /// controla que no genere error: Varias instancias de IEntityChangeTracker no pueden hacer referencia a un objeto entidad
            /// </summary>
            /// <typeparam name="T">Type de la clase solicitada</typeparam>
            /// <param name="includes">Predicado de Linq para incluir las dependencias</param>
            /// <returns></returns>
            public IQueryable<T> Listado<T>(params Expression<Func<T, object>>[] includes) where T : class
            {
                IQueryable<T> set = _context.Set<T>().AsNoTracking();
                foreach (var include in includes)
                {
                    set = set.Include(include);                    
                }
                return set.AsQueryable<T>();
            }
            /// <summary>
            /// Agrega un nuevo elemento al Contexto
            /// </summary>
            /// <typeparam name="T">Type de la clase solicitada</typeparam>
            /// <param name="entity">Instancia del objeto a agregar</param>
            public virtual void Agregar<T>(T entity) where T : class
            {
                _context.Entry<T>(entity).State = System.Data.Entity.EntityState.Added;
                Save();
            }
            /// <summary>
            /// Permite agregar varios elemento en lote al Contexto
            /// </summary>
            /// <typeparam name="T">Type de la clase solicitada</typeparam>
            /// <param name="entity">Instancia del objeto a agregar</param>
            public virtual void Agregar<T>(IEnumerable<T> entity) where T : class
            {
                foreach (var ent in entity)
                {
                    Agregar<T>(ent);
                }

            }
            /// <summary>
            /// Elimina un nuevo elemento al Contexto
            /// </summary>
            /// <typeparam name="T">Type de la clase solicitada</typeparam>
            /// <param name="entity">Instancia del objeto a agregar</param>
            public virtual void Eliminar<T>(T entity) where T : class
            {
                var a = CheckIsAttached<T>(entity);
                a.State = System.Data.Entity.EntityState.Deleted;
                Save();
            }
            /// <summary>
            /// Permite eliminar varios elemento en lote al Contexto
            /// </summary>
            /// <typeparam name="T">Type de la clase solicitada</typeparam>
            /// <param name="entity">Instancia del objeto a agregar</param>
            public virtual void Eliminar<T>(IEnumerable<T> entity) where T : class
            {
                foreach (var ent in entity)
                {
                    Eliminar<T>(ent);
                }
            }
            /// <summary>
            /// Modifica un nuevo elemento al Contexto
            /// </summary>
            /// <typeparam name="T">Type de la clase solicitada</typeparam>
            /// <param name="entity">Instancia del objeto a agregar</param>
            public virtual void Modificar<T>(T entity) where T : class
            {
                var a = CheckIsAttached<T>(entity);
                a.State = System.Data.Entity.EntityState.Modified;
                Save();
            }
            /// <summary>
            /// Permite modificar varios elemento en lote al Contexto
            /// </summary>
            /// <typeparam name="T">Type de la clase solicitada</typeparam>
            /// <param name="entity">Instancia del objeto a agregar</param>
            public virtual void Modificar<T>(IEnumerable<T> entity) where T : class
            {
                foreach (var ent in entity)
                {
                    Modificar<T>(ent);
                }
            }
            /// <summary>
            /// Validación que controla error: Un objeto con la misma clave ya existe en el ObjectStateManager. El ObjectStateManager puede realizar un seguimiento de múltiples objetos con la misma clave.
            /// </summary>
            /// <typeparam name="T">Type de la clase solicitada</typeparam>
            /// <param name="entity">Instancia del objeto a chequar</param>
            /// <returns>Intancia de DbEntityEntry validada </returns>
            public DbEntityEntry<T> CheckIsAttached<T>(T entity) where T : class
            {
                var e = _context.Entry(entity);
                if (e.State == System.Data.Entity.EntityState.Detached)
                {
                    _context.Set<T>().Attach(entity);
                    e = _context.Entry(entity);
                }
                return e;
            }
            /// <summary>
            /// Salva los cambios realizados en el Contexto
            /// </summary>
            private void Save()
            {
                try
                {
                    _context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    List<string> logs = new List<string>();
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            logs.Add(string.Format("{Property:\"{0}\", Error:\"{1}\"}", ve.PropertyName, ve.ErrorMessage));
                        }
                    }

                    throw new Exception(JsonConvert.SerializeObject(logs));
                }
               
            }
            /// <summary>
            /// Inserción en Lote
            /// </summary>
            /// <typeparam name="T">Type de la entidad a insertar</typeparam>
            /// <param name="entities">Arreglo de datos de tipo T</param>
            public void BulkInsertAll<T>(T[] entities) where T : class
            {
                var conn = (SqlConnection)_context.Database.Connection;
                conn.Open();
                Type t = typeof(T);
                _context.Set(t).ToString();
                var objectContext = ((IObjectContextAdapter)_context).ObjectContext;
                var workspace = objectContext.MetadataWorkspace;
                var mappings = GetMappings(workspace, objectContext.DefaultContainerName, typeof(T).Name);

                var tableName = GetTableName<T>();
                var bulkCopy = new SqlBulkCopy(conn) { DestinationTableName = tableName };

                // Foreign key relations show up as virtual declared 
                // properties and we want to ignore these.
                var properties = t.GetProperties().Where(p => !p.GetGetMethod().IsVirtual).ToArray();
                var table = new DataTable();
                foreach (var property in properties)
                {
                    Type propertyType = property.PropertyType;

                    // Nullable properties need special treatment.
                    if (propertyType.IsGenericType &&
                        propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propertyType = Nullable.GetUnderlyingType(propertyType);
                    }

                    // Since we cannot trust the CLR type properties to be in the same order as
                    // the table columns we use the SqlBulkCopy column mappings.
                    table.Columns.Add(new DataColumn(property.Name, propertyType));
                    var clrPropertyName = property.Name;
                    var tableColumnName = mappings[property.Name];
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(clrPropertyName, tableColumnName));
                }

                // Add all our entities to our data table
                foreach (var entity in entities)
                {
                    var e = entity;
                    table.Rows.Add(properties.Select(property => GetPropertyValue(property.GetValue(e, null))).ToArray());
                }

                // send it to the server for bulk execution
                bulkCopy.BulkCopyTimeout = 5 * 60;
                bulkCopy.WriteToServer(table);

                conn.Close();
            }

            private string GetTableName<T>() where T : class
            {
                var dbSet = _context.Set<T>();
                var sql = dbSet.ToString();
                var regex = new Regex(@"FROM (?<table>.*) AS");
                var match = regex.Match(sql);
                return match.Groups["table"].Value;
            }

            private object GetPropertyValue(object o)
            {
                if (o == null)
                    return DBNull.Value;
                return o;
            }

            private Dictionary<string, string> GetMappings(MetadataWorkspace workspace, string containerName, string entityName)
            {
                var mappings = new Dictionary<string, string>();
                var storageMapping = workspace.GetItem<GlobalItem>(containerName, DataSpace.CSSpace);
                dynamic entitySetMaps = storageMapping.GetType().InvokeMember(
                    "EntitySetMaps",
                    BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance,
                    null, storageMapping, null);

                foreach (var entitySetMap in entitySetMaps)
                {
                    var typeMappings = GetArrayList("TypeMappings", entitySetMap);
                    dynamic typeMapping = typeMappings[0];
                    dynamic types = GetArrayList("Types", typeMapping);

                    if (types[0].Name == entityName)
                    {
                        var fragments = GetArrayList("MappingFragments", typeMapping);
                        var fragment = fragments[0];
                        var properties = GetArrayList("AllProperties", fragment);
                        foreach (var property in properties)
                        {
                            var edmProperty = GetProperty("EdmProperty", property);
                            var columnProperty = GetProperty("ColumnProperty", property);
                            mappings.Add(edmProperty.Name, columnProperty.Name);
                        }
                    }
                }

                return mappings;
            }

            private ArrayList GetArrayList(string property, object instance)
            {
                var type = instance.GetType();
                var objects = (IEnumerable)type.InvokeMember(property, BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance, null, instance, null);
                var list = new ArrayList();
                foreach (var o in objects)
                {
                    list.Add(o);
                }
                return list;
            }

            private dynamic GetProperty(string property, object instance)
            {
                var type = instance.GetType();
                return type.InvokeMember(property, BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance, null, instance, null);
            }



        }
    }
    
}
