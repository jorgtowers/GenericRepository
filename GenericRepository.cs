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
 *                                     con la finalidad de agregar iconos de FontAwesome
 *
 * CREADO......: 20-03-2015 11:53PM
 * ACTUALIZADO.: 27-10-2015 04:17PM
 * ----------------------------------------------------------------------------------------------------------------------------- */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using GenericRepository.EF5;
using System.Text;
using PageDynamc.Model;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Data.Metadata.Edm;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace GenericRepository
{
    [Information(Descripcion = "Clase que implementa metodos genericos apartir del modelo de EDM")]
    public class PageGeneric<T> : AbstractPage where T : class,new()
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
    public abstract class PageDynamic<T> : AbstractPage where T : class, new()
    {
        private int _Cantidad = 1000;
        /// <summary>
        /// Cantidad de registros a retornar en el listado por defecto
        /// </summary>
        public int Cantidad
        {
            get { return _Cantidad; }
            set { _Cantidad = value; }
        }
        /// <summary>
        /// Enumerativo que determinar la presentación usarán los datos de tipo Boolean
        /// </summary>
        public enum eBooleanAs { RadioButton, CheckBox,Toogle }
        /// <summary>
        /// Enumerativo que determinar la presentación usarán los botones de acciones para el CRUD
        /// </summary>
        public enum eButtonsAs { Button, LinkButton }
        private eBooleanAs _BooleanAs = eBooleanAs.Toogle;
        /// <summary>
        /// Permite controlar la presentación que usarán los datos de tipo Boolean, por defecto se usará eBooleanAs.CheckBox
        /// </summary>
        public eBooleanAs BooleanAs
        {
            get { return _BooleanAs; }
            set { _BooleanAs = value; }
        }
        private eButtonsAs _ButtonAs = eButtonsAs.LinkButton;
        /// <summary>
        /// Permite controlar la presentación que usarán los botones de acciones para el CRUD, por defecto se usará eButtonsAs.LinkButton
        /// </summary>
        public eButtonsAs ButtonAs
        {
            get { return _ButtonAs; }
            set { _ButtonAs = value; }
        }
        private string _CamposTextoMultiLinea = "texto";
        /// <summary>
        /// Indique nombre de campos separando por coma (,) los que serán tipo "Multilinea", por defecto buscará campo llamado TEXTO. Ej.: Texto,Observacion,Descripcion
        /// </summary>
        public string CamposTextoMultiLinea
        {
            get { return _CamposTextoMultiLinea; }
            set { _CamposTextoMultiLinea = value; }
        }
        private string _NombreBotonAgregar = "Nuevo";
        /// <summary>
        /// Nombre que tendrá el boton de btnAgregar, su valor por defecto es "Nuevo"
        /// </summary>
        public string NombreBotonAgregar
        {
            get { return _NombreBotonAgregar; }
            set { _NombreBotonAgregar = value; }
        }
        private string _NombreBotonModificar = "Guardar";
        /// <summary>
        /// Nombre que tendrá el boton de btnModificar, su valor por defecto es "Guardar"
        /// </summary>
        public string NombreBotonModificar
        {
            get { return _NombreBotonModificar; }
            set { _NombreBotonModificar = value; }
        }
        private string _NombreBotonEliminar = "Borrar";
        /// <summary>
        /// Nombre que tendrá el boton de btnEliminar, su valor por defecto es "Borrar"
        /// </summary>
        public string NombreBotonEliminar
        {
            get { return _NombreBotonEliminar; }
            set { _NombreBotonEliminar = value; }
        }
        private string _NombreBotonLimpiar = "Cancelar";
        /// <summary>
        /// Nombre que tendrá el boton de btnLimpiar, su valor por defecto es "Cancelar"
        /// </summary>
        public string NombreBotonLimpiar
        {
            get { return _NombreBotonLimpiar; }
            set { _NombreBotonLimpiar = value; }
        }
        private string _TituloPagina = string.Empty;
        /// <summary>
        /// Nombre que tendrá el titulo de la página, su valor por defecto está en blanco "String.Empty"
        /// </summary>
        public string TituloPagina
        {
            get { return _TituloPagina; }
            set { _TituloPagina = value; }
        }
        private Panel _Panel = null;
        /// <summary>
        /// Instancia del Panel que será usado para crear todos los elementos de la instancia del objeto recibido
        /// </summary>
        public Panel Panel
        {
            get { return _Panel; }
            set { _Panel = value; }
        }

        List<KeyValuePair<string, string>> _Fields = new List<KeyValuePair<string, string>>();
        /// <summary>
        /// Listado de campos y tipo de valor para a creación de los elementos y validar sus tipos de datos
        /// </summary>
        public List<KeyValuePair<string, string>> Fields
        {
            get { return _Fields; }
            set { _Fields = value; }
        }
        List<T> _Listado = new List<T>();
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

            _Panel = this.Controls.OfType<Panel>().FirstOrDefault();
            if (_Panel == null)
            {
                _Panel = new System.Web.UI.WebControls.Panel() { ID = "PN" };
                MasterPage masterPage = this.Master;
                HtmlForm form = this.Master.Controls.OfType<System.Web.UI.HtmlControls.HtmlForm>().FirstOrDefault();
                ContentPlaceHolder cph = form.Controls.OfType<ContentPlaceHolder>().FirstOrDefault();
                cph.Controls.Add(_Panel);
            }


            /* ---------------------------------------------------
             * Acciones y permisos del rol
             * --------------------------------------------------- */
            string urlActual = Request.Url.LocalPath;
            PaginaRol rolEnPagina = null;
            try {
                rolEnPagina = UsuarioActual.Rol.PaginaRol.Where(x => x.Pagina.Ruta.ToLower() == urlActual.ToLower()).FirstOrDefault();
            } catch { }
            bool? tieneAcceso = null, puedeSeleccionar = null, puedeListar = null, puedeAgregar = null, puedeModificar = null, puedeEliminar = null;
            if (rolEnPagina != null)
            {
                try
                {
                    tieneAcceso = rolEnPagina.Acceso;
                    puedeSeleccionar = rolEnPagina.Seleccionar;
                    puedeListar = rolEnPagina.Listar;
                    puedeAgregar = rolEnPagina.Agregar;
                    puedeModificar = rolEnPagina.Modificar;
                    puedeEliminar = rolEnPagina.Eliminar;
                }
                catch { }
            }
            else
                tieneAcceso = false;


            /* ---------------------------------------------------
             * Lectura del esamblado y de la documentación en XML
             * --------------------------------------------------- */
            string ruta = HttpContext.Current.Server.MapPath(@"\bin\" + TDynamic.Assembly.ManifestModule.Name);
            Assembly dll = Assembly.LoadFrom(ruta);
            XDocument xml = XDocument.Load(ruta.Replace(".dll", ".xml"));
            var sumarios = xml.Descendants("member").Where(x => x.LastAttribute.Value.Substring(0, 2) == "P:" && x.LastAttribute.Value.Contains(TDynamic.Namespace)).ToList();

            #region Definición del título de la página a partir de la entidad
            /* ------------------------------
             * Agregando título a la pagina
             * ------------------------------ */
            string title = "Maestro de ";
            string end = TDynamic.Name.Substring(TDynamic.Name.Length - 1).ToLower();
            switch (end)
            {
                case "s":
                    title += TDynamic.Name;
                    break;
                case "n":
                    title += TDynamic.Name + "es";
                    break;
                case "r":
                    title += TDynamic.Name + "es";
                    break;
                case "l":
                    title += TDynamic.Name + "es";
                    break;
                default:
                    title += TDynamic.Name + "s";
                    break;
            }
            if (string.IsNullOrEmpty(_TituloPagina))
                this.Page.Title = title;
            else
            {
                this.Page.Title = _TituloPagina;
                title = _TituloPagina;
            }
            #endregion

            base.OnInit(e);


            if (!tieneAcceso.HasValue || tieneAcceso.Value) {          

            PropertyInfo[] propiedades = TDynamic.GetProperties();

            #region Region del Mantenimiento para hacer CRUD de los registros
            _Panel.Controls.Add(new LiteralControl("<p onclick=app.Utils.Toogle('editPanel')><b class='fa fa-edit'></b>Presione clic o la tecla F9, para abrir panel de edición.</p><div id='editPanel' style='display: none'><span id='closeEditPanel' onclick=app.Utils.Toogle('editPanel')><b class='fa fa-times'></b></span>"));
            _Panel.Controls.Add(new LiteralControl("<nav><h4>Gestión de datos</h4></nav>"));
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
                    _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(propiedad.Name) + "</b><p>" + (sumarioPropiedad != null ? sumarioPropiedad.Value.Trim() : "") + "</p></td><td>"));
                    Type clase = Type.GetType(propiedad.PropertyType.Namespace + "." + nombre);

                    IDescripcionId obj = (IDescripcionId)Activator.CreateInstance(clase);
                    //obj.Descripcion = "( -- Seleccionar -- )";
                    obj.Descripcion = "( -- Seleccione un item de " + clase.Name + " -- )";
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

                    IList<IDescripcionId> listado = setClase != null ? setClase.Local.Cast<IDescripcionId>().ToList() : null;
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
                        foreach (string item in _CamposTextoMultiLinea.ToLower().Split(','))
                        {
                            if (nombre.ToLower() == item)
                                t.TextMode = TextBoxMode.MultiLine;
                        }

                        t.Attributes.Add("placeHolder", Utils.SplitCamelCase(nombre));
                        if (nombre == "Id")
                        {
                            _Panel.Controls.Add(new LiteralControl("<tr class='help' style='display:none'><td  class='info'><b>" + Utils.SplitCamelCase(nombre) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                            t.Enabled = false;
                            _Panel.Controls.Add(t);
                            _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                        }
                        else
                            if (nombre.ToLower() == "userid" || !nombre.Contains("Id"))
                        {
                            _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(nombre) + "</b><p>" + labelDescripcion + "</p></td><td>"));
                            _Panel.Controls.Add(t);
                            _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                        }
                    }
                    if (tipo == "Boolean")
                    {
                        /* ----------------
                         * Evalua enumerativo "eBooleanAs" para determinar que presentación usarán los datos de tipo Boolean, por defecto se usará eBooleanAs.CheckBox
                         * ----------------*/
                        switch (_BooleanAs)
                        {
                            case eBooleanAs.RadioButton:
                                {
                                    _Fields.Add(new KeyValuePair<string, string>("rbt" + nombre, tipo));
                                    _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(nombre) + "</b><p>" + labelDescripcion + "</p></td><td>"));
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
                                    _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + Utils.SplitCamelCase(nombre) + "</b><p>" + labelDescripcion + "</p></td><td>"));
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

             switch (_ButtonAs)
                {
                    case eButtonsAs.Button:
                        {
                            Button btnEliminar = new Button() { ID = "btnEliminar", CssClass = "btn btn-danger", Text = _NombreBotonEliminar };
                            btnEliminar.Attributes.Add("style", "position: absolute;  left:0");
                            btnEliminar.Click += Eliminar;
                            if ((puedeEliminar.HasValue && !puedeEliminar.Value) || base.Id < 1)
                                btnEliminar.Visible = false;

                            if ((puedeEliminar.HasValue && !puedeEliminar.Value) || base.Id < 1)
                                btnEliminar.Visible = false;
                            _Panel.Controls.Add(btnEliminar);

                            Button btnModificar = new Button() { ID = "btnModificar", CssClass = "btn btn-primary", Text = _NombreBotonModificar };
                            btnModificar.Click += Modificar;
                            btnModificar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                            if ((puedeModificar.HasValue && !puedeModificar.Value) || base.Id < 1)
                                btnModificar.Visible = false;

                            _Panel.Controls.Add(btnModificar);

                            Button btnAgregar = new Button() { ID = "btnAgregar", CssClass = "btn btn-success", Text = _NombreBotonAgregar };
                            btnAgregar.Click += Agregar;
                            btnAgregar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                            if ((puedeAgregar.HasValue && !puedeAgregar.Value) || base.Id > 0)
                                btnAgregar.Visible = false;

                            _Panel.Controls.Add(btnAgregar);

                            Button btnLimpiar = new Button() { ID = "btnLimpiar", CssClass = "btn btn-default", Text = _NombreBotonLimpiar };
                            btnLimpiar.Click += Limpiar;
                            _Panel.Controls.Add(btnLimpiar);
                            break;
                        }
                    case eButtonsAs.LinkButton:
                        {
                            LinkButton btnEliminar = new LinkButton() { ID = "btnEliminar", CssClass = "btn btn-danger", Text = "<b class='fa fa-times' ></b>&nbsp;" + _NombreBotonEliminar };
                            btnEliminar.Attributes.Add("style", "position: absolute;  left:0");
                            btnEliminar.Click += Eliminar;
                            if ((puedeEliminar.HasValue && !puedeEliminar.Value) || base.Id < 1)
                                btnEliminar.Visible = false;
                            if ((puedeEliminar.HasValue && !puedeEliminar.Value) || base.Id < 1)
                                btnEliminar.Visible = false;
                            _Panel.Controls.Add(btnEliminar);

                            LinkButton btnModificar = new LinkButton() { ID = "btnModificar", CssClass = "btn btn-primary", Text = "<b class='fa fa-save' ></b>&nbsp;" + _NombreBotonModificar };
                            btnModificar.Click += Modificar;
                            btnModificar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                            if ((puedeModificar.HasValue && !puedeModificar.Value) || base.Id < 1)
                                btnModificar.Visible = false;
                            _Panel.Controls.Add(btnModificar);

                            LinkButton btnAgregar = new LinkButton() { ID = "btnAgregar", CssClass = "btn btn-success", Text = "<b class='fa fa-plus-circle' ></b>&nbsp;" + _NombreBotonAgregar };
                            btnAgregar.Click += Agregar;
                            btnAgregar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
                            if ((puedeAgregar.HasValue && !puedeAgregar.Value) || base.Id > 0)
                                btnAgregar.Visible = false;
                            _Panel.Controls.Add(btnAgregar);

                            LinkButton btnLimpiar = new LinkButton() { ID = "btnLimpiar", CssClass = "btn btn-default", Text =  _NombreBotonLimpiar };
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
            #region Region del Listado en tabla HTML, muestra todos los registros de la tabla
            if (!puedeListar.HasValue || puedeListar.Value)
            {
                _Panel.Controls.Add(new LiteralControl("<nav><h2>" + title + "</h2></nav>"));
                _Panel.Controls.Add(new LiteralControl("<input id='filtro' class='form-control' placeholder='Buscar...'/>"));
                _Panel.Controls.Add(new LiteralControl("<section class='gridContainer'><table id='listado' class='table table-condensed table-striped sortable filterable more'><thead><tr>"));
                #region ListadoHeader
                /* ----------------
                 * Agregando encabezados de listado en una tabla de HTML
                 * ----------------*/
                foreach (KeyValuePair<string, string> headers in _Fields)
                {
                    string key = headers.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                    if (key == "Id")
                        _Panel.Controls.Add(new LiteralControl("<td  class='unsortable'>" + key + "</td>"));
                    if (key.ToLower() == "userid" || !key.Contains("Id"))
                    {
                        if (key.Substring(key.IndexOf("-") + 1).Length > 0)
                            _Panel.Controls.Add(new LiteralControl("<td>" + Utils.SplitCamelCase(key.Replace("-", "")) + "</td>"));
                        else
                            _Panel.Controls.Add(new LiteralControl("<td>" + Utils.SplitCamelCase(key.Substring(0, key.IndexOf("-"))) + "</td>"));
                    }
                }
                #endregion
                _Panel.Controls.Add(new LiteralControl("</tr></thead>"));
                _Panel.Controls.Add(new LiteralControl("<tbody id='toPaginador'>"));
                #region ListadoBody
                /* ----------------
                 * Agregando cuerpo de listado en una tabla de HTML
                 * ----------------*/
                _Listado = model.Listado<T>().Take(_Cantidad).ToList();
                foreach (T item in _Listado)
                {
                    _Panel.Controls.Add(new LiteralControl("<tr>"));
                    foreach (KeyValuePair<string, string> campo in _Fields)
                    {
                        object resultado = null;
                        string key = campo.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("rbt", "");
                        bool isDDL = campo.Key.Substring(0, 3) == "ddl";

                        if (!isDDL)
                        {
                            Type tipoDePropiedad = Type.GetType("System." + campo.Value);
                            PropertyInfo propiedad = item.GetType().GetProperty(key);
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
                            if (key.ToLower()=="userid" || !key.Contains("Id"))
                            {
                                if (campo.Value != "Boolean")
                                    _Panel.Controls.Add(new LiteralControl("<td>" + (resultado != null ? resultado.ToString() : "") + "</td>"));
                                else
                                {
                                    switch (_BooleanAs)
                                    {
                                        case eBooleanAs.RadioButton:
                                            _Panel.Controls.Add(new LiteralControl("<td>" + (resultado != null ? (resultado.ToString() == "True" ? "Si" : "No") : "") + "</td>"));
                                            break;
                                        case eBooleanAs.CheckBox:
                                            _Panel.Controls.Add(new LiteralControl("<td><input type='checkbox' " + (resultado != null ? (resultado.ToString() == "True" ? "checked" : "") : "") + "/></td>"));
                                            break;
                                        default:
                                            break;
                                    }

                                }
                            }
                        }

                    }
                    _Panel.Controls.Add(new LiteralControl("</tr>"));
                }
                #endregion
                _Panel.Controls.Add(new LiteralControl("</tbody></table></section>"));
            }
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
            switch (_BooleanAs)
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
            set { _Resultado = value; }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                if (base.Id > 0)
                {
                    FillCampos(model.Obtener<T>(base.Id));
                }
                RefreshListado();
            }
        }
        private void RefreshListado()
        {
            _Listado = model.Listado<T>().ToList();
        }
        private void FillCampos(T item)
        {
            try
            {
                #region Evalua todos los DropDownList de la página
                List<DropDownList> ddls = _Panel.Controls.OfType<DropDownList>().ToList();
                foreach (DropDownList ddl in ddls)
                {
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
                    txt.Text = result.ToString();
                }

                #endregion
                #region Evalua todos los CheckBox y/o RadioButton segun la propiedad eBooleanAs
                switch (_BooleanAs)
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

        protected virtual void Agregar(object sender, EventArgs e)
        {
            try
            {
                model.Agregar<T>(this.ObjectToUpdate());
                _Resultado = "Registro agregado satisfactoriamente...";
            }
            catch (Exception ex) { _Resultado = ex.Message; }
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

    public partial class Utils
    {
        public static void Llenar<T>(ListControl ctrl, List<T> datos, bool todos = false, bool seleccionar = false, bool orderByDescripcion = false) where T : IDescripcionId, new()
        {
            Type tipo= datos.GetType();
            List<T> t = datos;
            if (todos)
                t.Add(new T() { Id = -1, Descripcion = "( -- Todos -- )" });
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
        public static string Duracion(DateTime desde,DateTime hasta)
        {
            TimeSpan span = hasta - desde;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("{0} {1}",years, years == 1 ? "año" : "años");
            }
            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("{0} {1}",months, months == 1 ? "mes" : "meses");
            }
            if (span.Days > 0)
                return String.Format("{0} {1}", span.Days, span.Days == 1 ? "día" : "días");
            if (span.Hours > 0)
                return String.Format("{0} {1}",span.Hours, span.Hours == 1 ? "hora" : "horas");
            if (span.Minutes > 0)
                return String.Format("{0} {1}",span.Minutes, span.Minutes == 1 ? "minuto" : "minutos");
            if (span.Seconds > 5)
                return String.Format("{0} segundos", span.Seconds);
            if (span.Seconds <= 5)
                return "ahora";
            return string.Empty;
        }
        public static string Duracion(DateTime desde, DateTime hasta, bool hhmmss) {            
            var diff = hasta.Subtract(desde);
            return String.Format("{0}:{1}:{2}", diff.Hours.ToString().PadLeft(2,'0'), diff.Minutes.ToString().PadLeft(2, '0'), diff.Seconds.ToString().PadLeft(2, '0'));
        }
        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }
        public static string CamelCase(string input) {
            return input.Substring(0, 1).ToUpper() + input.Substring(1, input.Length - 1).ToLower();
        }
    }

    namespace EF4
    {
        [Obsolete("Usar GenericRepository.EF5", true)]
        public class GenericRepository<TContext> where TContext : ObjectContext, new()
        {
            protected static ObjectContext model;
            public GenericRepository()
            {
                model = new TContext();
            }


            const string keyPropertyName = "Id";
            protected interface IRepository
            {
                IQueryable<T> List<T>() where T : class;
                T Get<T>(int id) where T : class;
                void Create<T>(T entityTOCreate) where T : class;
                void Edit<T>(T entityToEdit) where T : class;
                void Delete<T>(T entityToDelete) where T : class;
            }
            public virtual T Obtener<T>(int id)
            {
                return Get<T>(id);
            }
            public virtual List<T> Listado<T>()
            {
                return List<T>().ToList();
            }
            public virtual void Agregar<T>(T entity)
            {

                model.AddObject(string.Format("{0}Set", entity.GetType().Name), entity);
                model.SaveChanges();
            }
            public virtual void Modificar<T>(T entity) where T : EntityObject
            {
                //model.GetObjectByKey(entity.EntityKey);
                var orginalEntity = Get<T>(GetKeyPropertyValue<T>(entity));
                model.ApplyCurrentValues<T>(string.Format("{0}Set", entity.GetType().Name), entity);
                model.SaveChanges();
                // ORIGNIAL model.ApplyCurrentValues<entity as entity.GetType()>(string.Format("{0}Set", entity.GetType().Name), entity);
                //var orginalEntity = Get<T>(GetKeyPropertyValue<T>(entity));
                //model.ApplyPropertyChanges(GetEntitySetName<T>(), entity);
                //model.SaveChanges();
            }
            public virtual void Eliminar<T>(T entity)
            {
                var orginalEntity = Get<T>(GetKeyPropertyValue<T>(entity));
                model.DeleteObject(orginalEntity);
                model.SaveChanges();
            }
            protected T Get<T>(int id)
            {
                return List<T>().FirstOrDefault<T>(CreateGetExpression<T>(id));
            }
            protected int GetKeyPropertyValue<T>(object entity)
            {
                return (int)typeof(T).GetProperty(keyPropertyName).GetValue(entity, null);
            }
            protected string GetEntitySetName<T>()
            {
                return String.Format("{0}Set", typeof(T).Name);
            }
            protected Expression<Func<T, bool>> CreateGetExpression<T>(int id)
            {
                ParameterExpression e = Expression.Parameter(typeof(T), "e");
                PropertyInfo propinfo = typeof(T).GetProperty(keyPropertyName);
                MemberExpression m = Expression.MakeMemberAccess(e, propinfo);
                ConstantExpression c = Expression.Constant(id, typeof(int));
                BinaryExpression b = Expression.Equal(m, c);
                Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(b, e);
                return lambda;
            }
            protected IQueryable<T> List<T>()
            {
                return model.CreateQuery<T>(GetEntitySetName<T>());
            }




        }

        public interface IPropiedades
        {
            int Id { get; set; }
        }

        public class GenericCollection<T> : ICollection<T>, IList<T> where T : IPropiedades, new()
        {
            List<T> lista = new List<T>();

            public GenericCollection()
            {

            }
            public GenericCollection(List<T> items)
            {
                lista = items;
            }

            public List<T> this[int desde, int hasta]
            {
                get
                {
                    List<T> items = new List<T>();
                    if (desde == 0 && hasta == -1)
                        return lista;
                    try
                    {
                        hasta = (hasta == -1 ? lista.Count - 1 : (lista.Count > hasta ? hasta : lista.Count));
                        for (int i = desde; i <= hasta; i++)
                        {
                            items.Add(lista[i]);
                        }
                    }
                    catch { return items; }
                    return items;
                }

            }


            #region ICollection<T> Members
            public void Add(T item)
            {
                lista.Add(item);
            }
            public void Clear()
            {
                lista.Clear();
            }

            public bool Contains(T item)
            {
                return lista.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                lista.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return lista.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(T item)
            {
                return lista.Remove(item);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return lista.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                foreach (var item in lista)
                {
                    yield return item;
                }
            }
            #endregion

            public T GetObject(int Id)
            {
                var query = from o in lista
                            where o.Id == Id
                            select o;
                foreach (var item in query)
                {
                    return item;
                }
                return default(T);
            }

            #region IList<T> Members

            public int IndexOf(T item)
            {
                return lista.IndexOf(item);
            }

            public void Insert(int index, T item)
            {
                lista.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                lista.RemoveAt(index);
            }

            public T this[int index]
            {
                get
                {
                    return lista[index];
                }
                set
                {
                    lista[index] = value;
                }
            }

            #endregion
        }
    }
    namespace EF4SupportEF5
    {
        [Obsolete("Usar GenericRepository.EF5", true)]
        public class GenericRepository
        {
            //INSTANCIA DE OBJETO DEL EDM
            protected static MetasEntities context = new MetasEntities();

            protected ObjectContext model = ((IObjectContextAdapter)context).ObjectContext;

            const string keyPropertyName = "Id";
            protected interface IRepository
            {
                IQueryable<T> List<T>() where T : class;
                T Get<T>(int id) where T : class;
                void Create<T>(T entityTOCreate) where T : class;
                void Edit<T>(T entityToEdit) where T : class;
                void Delete<T>(T entityToDelete) where T : class;
            }
            public virtual T Obtener<T>(int id)
            {



                return Get<T>(id);
            }
            public virtual List<T> Listado<T>()
            {
                return List<T>().ToList();
            }
            public virtual void Agregar<T>(T entity)
            {
                model.AddObject(string.Format("{0}Set", entity.GetType().Name), entity);
                model.SaveChanges();
            }
            public virtual void Modificar<T>(T entity) where T : EntityObject
            {
                //model.GetObjectByKey(entity.EntityKey);
                var orginalEntity = Get<T>(GetKeyPropertyValue<T>(entity));
                model.ApplyCurrentValues<T>(string.Format("{0}Set", entity.GetType().Name), entity);
                model.SaveChanges();
                // ORIGNIAL model.ApplyCurrentValues<entity as entity.GetType()>(string.Format("{0}Set", entity.GetType().Name), entity);
                //var orginalEntity = Get<T>(GetKeyPropertyValue<T>(entity));
                //model.ApplyPropertyChanges(GetEntitySetName<T>(), entity);
                //model.SaveChanges();
            }
            public virtual void Eliminar<T>(T entity)
            {
                var orginalEntity = Get<T>(GetKeyPropertyValue<T>(entity));
                model.DeleteObject(orginalEntity);
                model.SaveChanges();
            }
            protected T Get<T>(int id)
            {
                return List<T>().FirstOrDefault<T>(CreateGetExpression<T>(id));
            }
            protected int GetKeyPropertyValue<T>(object entity)
            {
                return (int)typeof(T).GetProperty(keyPropertyName).GetValue(entity, null);
            }
            protected string GetEntitySetName<T>()
            {
                return String.Format("{0}Set", typeof(T).Name);
            }
            protected Expression<Func<T, bool>> CreateGetExpression<T>(int id)
            {
                ParameterExpression e = Expression.Parameter(typeof(T), "e");
                PropertyInfo propinfo = typeof(T).GetProperty(keyPropertyName);
                MemberExpression m = Expression.MakeMemberAccess(e, propinfo);
                ConstantExpression c = Expression.Constant(id, typeof(int));
                BinaryExpression b = Expression.Equal(m, c);
                Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(b, e);
                return lambda;
            }
            protected IQueryable<T> List<T>()
            {
                return model.CreateQuery<T>(GetEntitySetName<T>());
            }

        }
        public interface IPropiedades
        {
            int Id { get; set; }
        }
        public class GenericCollection<T> : ICollection<T>, IList<T> where T : IPropiedades, new()
        {
            List<T> lista = new List<T>();

            public GenericCollection()
            {

            }
            public GenericCollection(List<T> items)
            {
                lista = items;
            }

            public List<T> this[int desde, int hasta]
            {
                get
                {
                    List<T> items = new List<T>();
                    if (desde == 0 && hasta == -1)
                        return lista;
                    try
                    {
                        hasta = (hasta == -1 ? lista.Count - 1 : (lista.Count > hasta ? hasta : lista.Count));
                        for (int i = desde; i <= hasta; i++)
                        {
                            items.Add(lista[i]);
                        }
                    }
                    catch { return items; }
                    return items;
                }

            }


            #region ICollection<T> Members
            public void Add(T item)
            {
                lista.Add(item);
            }
            public void Clear()
            {
                lista.Clear();
            }

            public bool Contains(T item)
            {
                return lista.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                lista.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return lista.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(T item)
            {
                return lista.Remove(item);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return lista.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                foreach (var item in lista)
                {
                    yield return item;
                }
            }
            #endregion

            public T GetObject(int Id)
            {
                var query = from o in lista
                            where o.Id == Id
                            select o;
                foreach (var item in query)
                {
                    return item;
                }
                return default(T);
            }

            #region IList<T> Members

            public int IndexOf(T item)
            {
                return lista.IndexOf(item);
            }

            public void Insert(int index, T item)
            {
                lista.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                lista.RemoveAt(index);
            }

            public T this[int index]
            {
                get
                {
                    return lista[index];
                }
                set
                {
                    lista[index] = value;
                }
            }

            #endregion
        }
    }
    namespace EF5
    {   
        /// <summary>
        /// Interfaz que asegura la existencia del campo Id
        /// </summary>
        [Obsolete("Usar IDescripcionId, si usará PageDynamic<T>, ya que los campos de DropDonwList requieren de un valor ID y DESCRIPCION")]
        public interface IId
        {
            int Id { get; set; }
        }
        /// <summary>
        /// Interfaz que asegura la existencia del campo Id y Descripcion
        /// </summary
        public interface IDescripcionId
        {
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
            [Obsolete("Se recomienda usar T Obtener<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : class",false)]
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
            [Obsolete("Se recomienda usar T Obtener<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : class",false)  ]
            public T Obtener<T>(Expression<Func<T, bool>> predicate , IEnumerable<string> includePaths ) where T : class
            {
                IQueryable<T> query = _context.Set<T>().AsNoTracking();
                if (predicate != null) {
                    query = query.Where(predicate);
                }
                if (includePaths != null){
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
                foreach (var include in includes){
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
                _context.Entry<T>(entity).State = EntityState.Added;
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
                a.State = EntityState.Deleted;
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
                a.State = EntityState.Modified;
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
                if (e.State == EntityState.Detached)
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
                _context.SaveChanges();
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
    namespace EF6 {
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
                var objectContext = ((IObjectContextAdapter)this).ObjectContext;
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
                    BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
                    null, storageMapping, null);

                foreach (var entitySetMap in entitySetMaps)
                {
                    var typeMappings = GetArrayList("EntityTypeMappings", entitySetMap);
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
                var objects = (IEnumerable)type.InvokeMember(
                    property,
                    BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, instance, null);
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
                return type.InvokeMember(property, BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, instance, null);
            }
        }
    }
}
