/*!
 * ABOUT.......: Clase generica que permite conectarse a un EDM, en varías versiones de EF4, EF4SupportEF5 y EF5
 * CREADOR.....: Jorge L. Torres A.
 * ACTUALIACION: Se corrige metodo FillCampos  "chk.Checked = (bool)result;"
 * ACTUALIZADO.: 15-04-2015 09:02AM
 * CREADO......: 20-03-2015 11:53PM
 */
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

namespace GenericRepository
{
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
                model.Eliminar<T>(ObjectToUpdate);
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
            Button btn = ((Button)sender);
            if (btn.Text == "Limpiar" || btn.Text == "Eliminar")
            {
                Response.Redirect(Request.Url.LocalPath, false);
            }
            else
            {
                Response.Redirect(Request.Url.AbsoluteUri, false);
            }
        }

    }
    /// <summary>
    /// Clase especializada para la generación de páginas web apartir del nombre de una instancia, usando reflextion
    /// </summary>
    /// <typeparam name="T">Instancia de Type a usar</typeparam>
    public class PageDynamic<T> : AbstractPage where T : class, new()
    {
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
            string title = "Maestro de ";
            string end=TDynamic.Name.Substring(TDynamic.Name.Length - 1).ToLower();
            switch (end)
            {
                case "s":
                    title += TDynamic.Name ;
                    break;
                case "n":
                    title += TDynamic.Name + "es";
                    break;
                default:
                    title += TDynamic.Name +"s";
                    break;
            }
            this.Page.Title = title;
            base.OnInit(e);

            PropertyInfo[] propiedades = TDynamic.GetProperties();
            
            #region Mantenimiento
            _Panel.Controls.Add(new LiteralControl("<p onclick=app.Utils.Toogle('editPanel')><b class='fa fa-edit'></b>Presione clic o la tecla F9, para abrir panel de edición.</p><div id='editPanel' style='display: none'><span id='closeEditPanel' onclick=app.Utils.Toogle('editPanel')><b class='fa fa-times'></b></span>"));
            _Panel.Controls.Add(new LiteralControl("<table class='table'><tbody>"));
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
                if (TDynamic.Namespace == propiedad.PropertyType.Namespace)
                {
                    nombre = propiedad.PropertyType.Name;
                    /* ------------------
                     * propiedad.Name: Ofrece el "tipo" del Object, por se un DropDownList se le asigna un Int32 para poder manipular su Id
                     *  _Fields.Add(new KeyValuePair<string, string>(propiedad.Name, tipo));
                     --------------------*/
                    _Fields.Add(new KeyValuePair<string, string>("ddl" + nombre + "-" + propiedad.Name.Replace(nombre, ""), "Int32"));
                    _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + propiedad.Name + "</b><p></p></td><td>"));
                    Type clase = Type.GetType(propiedad.PropertyType.Namespace + "." + nombre);

                    IDescripcionId obj = (IDescripcionId)Activator.CreateInstance(clase);
                    obj.Descripcion = "( -- Seleccionar -- )";
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
                        CssClass="form-control"
                    };

                    IList<IDescripcionId> listado = setClase != null ? setClase.Local.Cast<IDescripcionId>().ToList() : null;
                    listado.Add(obj);

                    t.DataSource = listado.OrderBy(x => x.Id); 
                    t.DataBind();

                    _Panel.Controls.Add(t);
                    _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                }

                if (propiedad.PropertyType.Namespace == "System")
                {
                    nombre = propiedad.Name;
                    if (tipo == "String" || tipo == "Int32" || tipo == "DateTime")
                    {
                        _Fields.Add(new KeyValuePair<string, string>("txt"+nombre, tipo));
                        TextBox t = new TextBox() { ID = "txt" + nombre.Replace(" ", ""), CssClass = "form-control" };
                        if (nombre == "Id")
                        {
                            _Panel.Controls.Add(new LiteralControl("<tr class='help' style='display:none'><td  class='info'><b>" + nombre + "</b><p></p></td><td>"));
                            t.Enabled = false;
                            _Panel.Controls.Add(t);
                            _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                        }
                        else
                            if (!nombre.Contains("Id"))
                            {
                                _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + nombre + "</b><p></p></td><td>"));
                                _Panel.Controls.Add(t);
                                _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                            }                        
                    }
                    if (tipo == "Boolean")
                    {
                        _Fields.Add(new KeyValuePair<string, string>("chk"+nombre, tipo));
                        _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'><b>" + nombre + "</b><p></p></td><td>"));
                        CheckBox t = new CheckBox() {  ID = "chk" +nombre.Replace(" ", "") };
                        _Panel.Controls.Add(t);
                        _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                    }



                }
                //if (TDynamic.Namespace == propiedad.PropertyType.Namespace)
                //    sb.AppendLine("<b>Objeto Nombre:</b>" + propiedad.Name);
                //if (propiedad.PropertyType.Namespace == "System.Collections.Generic")
                //    sb.AppendLine("<b>Lista Nombre:</b>" + propiedad.Name);

                //sb.AppendLine("<br>");
                //Response.Write(sb.ToString());
            }
            _Panel.Controls.Add(new LiteralControl("<tr><td colspan='2'>"));
            Label lblEstatus = new Label() { ID = "lblEstatus" };
            _Panel.Controls.Add(lblEstatus);
            _Panel.Controls.Add(new LiteralControl("</td></tr>"));

            _Panel.Controls.Add(new LiteralControl("<tr><td colspan='2' style='text-align: right;'>"));

            Button btnAgregar = new Button() { ID = "btnAgregar", CssClass = "btn btn-primary", Text = "Agregar" };
            btnAgregar.Click += Agregar;
            btnAgregar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
            if (base.Id > 0)
                btnAgregar.Visible = false;
            _Panel.Controls.Add(btnAgregar);

            Button btnModificar = new Button() { ID = "btnModificar", CssClass = "btn btn-default", Text = "Modificar" };
            btnModificar.Click += Modificar;
            btnModificar.OnClientClick = "return app.Utils.ValidarCampos('editPanel',true)";
            if (base.Id < 0)
                btnModificar.Visible = false;
            _Panel.Controls.Add(btnModificar);

            Button btnEliminar = new Button() { ID = "btnEliminar", CssClass = "btn btn-default", Text = "Eliminar" };
            btnEliminar.Click += Eliminar;
            if (base.Id < 0)
                btnEliminar.Visible = false;
            _Panel.Controls.Add(btnEliminar);

            Button btnLimpiar = new Button() { ID = "btnLimpiar", CssClass = "btn btn-default", Text = "Cancelar" };
            btnLimpiar.Click += Limpiar;
            _Panel.Controls.Add(btnLimpiar);

            _Panel.Controls.Add(new LiteralControl("</td></tr>"));

            _Panel.Controls.Add(new LiteralControl("</tbody></table></div>"));

            #endregion

            #region Listado
            _Panel.Controls.Add(new LiteralControl("<nav><h2>" + title + "</h2></nav>"));
            _Panel.Controls.Add(new LiteralControl("<table class='table table-condensed table-striped'><thead><tr>"));
            foreach (KeyValuePair<string, string> headers in _Fields)
            {
                string key = headers.Key.Replace("txt","").Replace("ddl","").Replace("chk","");
                if (key == "Id")
                    _Panel.Controls.Add(new LiteralControl("<td>" + key + "</td>"));
                if (!key.Contains("Id"))
                {
                    if (key.Substring(key.IndexOf("-") + 1).Length > 0)
                        _Panel.Controls.Add(new LiteralControl("<td>" + key.Replace("-", "") + "</td>"));
                    else
                        _Panel.Controls.Add(new LiteralControl("<td>" + key.Substring(0, key.IndexOf("-")) + "</td>"));
                }
            }
            _Panel.Controls.Add(new LiteralControl("</tr></thead>"));
            _Panel.Controls.Add(new LiteralControl("<tbody id='toPaginador'>"));

            _Listado = model.Listado<T>().ToList();
            foreach (T item in _Listado)
            {
                _Panel.Controls.Add(new LiteralControl("<tr>"));
                foreach (KeyValuePair<string, string> campo in _Fields)
                {
                    object resultado=null; 
                    string key = campo.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "");
                    bool isDDL = campo.Key.Substring(0, 3) == "ddl";

                    if (!isDDL)
                    {
                        Type tipoDePropiedad = Type.GetType("System." + campo.Value);
                        PropertyInfo propiedad = item.GetType().GetProperty(key);
                        resultado = propiedad.GetValue(item, null) != null ? propiedad.GetValue(item, null).ToString() : "";
                    }
                    else {
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
                        id = propiedad.GetValue(item, null);

                        Type clase = Type.GetType(TDynamic.Namespace + "." + key);
                        DbSet setClase = null;
                        if (clase != null)
                        {
                            setClase = model.model.Set(clase);
                            object instancia = setClase.Find(id);
                            resultado = instancia.GetType().GetProperty("Descripcion").GetValue(instancia, null);
                        }                        
                    }
                    if (key == "Id")
                        _Panel.Controls.Add(new LiteralControl("<td><a href='?Id=" + (resultado != null ? resultado.ToString(): "") + "'><b class='fa fa-edit'></b></a></td>"));
                    else
                    {
                        if (!key.Contains("Id"))
                        {
                            if (campo.Value != "Boolean")
                                _Panel.Controls.Add(new LiteralControl("<td>" + (resultado != null ? resultado.ToString() : "") + "</td>"));
                            else
                                _Panel.Controls.Add(new LiteralControl("<td><input type='checkbox' " + (resultado != null ? (resultado.ToString() == "True" ? "checked" : "") : "") + "/></td>"));
                        }
                    }

                } _Panel.Controls.Add(new LiteralControl("</tr>"));
            }
            _Panel.Controls.Add(new LiteralControl("</tbody></table>"));

            #endregion

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
            List<DropDownList> ddls = _Panel.Controls.OfType<DropDownList>().ToList();
            foreach (DropDownList ddl in ddls)
            {
                if (ddl.Enabled)
                {
                    KeyValuePair<string, string> par = Fields.Where(x => x.Key.Replace("-", "") == ddl.ID.Replace("-", "")).FirstOrDefault();                    
                    int valor=0;
                    int.TryParse(ddl.SelectedValue, out valor);
                    string key = par.Key;
                    Type.GetType("System.Int32");
                    if (key.Substring(key.IndexOf("-") + 1).Length > 0)
                        key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "").Replace("-", "");
                    else
                    {
                        key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "");
                        key = key.Substring(0, key.IndexOf("-"));
                    }
                    _.GetType().GetProperty("Id" + key).SetValue(_, Convert.ChangeType(valor, Type.GetType("System." + par.Value)), null);


                    //_.GetType().GetProperty("Id"+par.Key).SetValue(_, Convert.ChangeType(ddl.Text, Type.GetType("System." + par.Value)), null);
                }
            }
            List<TextBox> txts = _Panel.Controls.OfType<TextBox>().ToList();
            foreach (TextBox txt in txts)
            {
                if (txt.Enabled)
                {
                    KeyValuePair<string, string> par = Fields.Where(x => x.Key == txt.ID).FirstOrDefault();
                    string key = par.Key;
                    key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "");
                    Type.GetType("System." + par.Value);
                    _.GetType().GetProperty(key).SetValue(_, Convert.ChangeType(txt.Text, Type.GetType("System." + par.Value)), null);
                }
            }
            List<CheckBox> chks = _Panel.Controls.OfType<CheckBox>().ToList();
            foreach (CheckBox chk in chks)
            {

                KeyValuePair<string, string> par = Fields.Where(x => x.Key == chk.ID).FirstOrDefault();
                string key = par.Key;
                key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "");
                Type.GetType("System." + par.Value);
                _.GetType().GetProperty(key).SetValue(_, chk.Checked, null);
            }
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

                List<DropDownList> ddls = _Panel.Controls.OfType<DropDownList>().ToList();
                foreach (DropDownList ddl in ddls)
                {
                    if (ddl.Enabled)
                    {
                        KeyValuePair<string, string> par = Fields.Where(x => x.Key.Replace("-", "") == ddl.ID.Replace("-", "")).FirstOrDefault();
                        object result=null;
                        string key = par.Key;
                        Type.GetType("System.Int32");
                        if (key.Substring(key.IndexOf("-") + 1).Length > 0)
                        {
                            key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "");
                            result = item.GetType().GetProperty("Id" + key.Replace("-", "")).GetValue(item, null);
                        }
                        else
                        {
                            key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "");
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
                List<TextBox> txts = _Panel.Controls.OfType<TextBox>().ToList();
                foreach (TextBox txt in txts)
                {
                    if (txt.Enabled)
                    {                        
                        KeyValuePair<string, string> par = Fields.Where(x => x.Key == txt.ID).FirstOrDefault();
                        string key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "");
                        Type.GetType("System." + par.Value);
                        object result = item.GetType().GetProperty(key).GetValue(item, null);
                        txt.Text = result.ToString();
                    }
                }
                List<CheckBox> chks = _Panel.Controls.OfType<CheckBox>().ToList();
                foreach (CheckBox chk in chks)
                {

                    KeyValuePair<string, string> par = Fields.Where(x => x.Key == chk.ID).FirstOrDefault();
                    string key = par.Key.Replace("txt", "").Replace("ddl", "").Replace("chk", "");
                    Type.GetType("System." + par.Value);
                    object result = item.GetType().GetProperty(key).GetValue(item, null);
                    chk.Checked = (bool)result;
                }
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
                model.Eliminar<T>(this.ObjectToUpdate());

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
            RefreshListado();
            Button btn = ((Button)sender);
            if (btn.Text == "Limpiar" || btn.Text == "Eliminar")
            {
                Response.Redirect(Request.Url.LocalPath, false);
            }
            else
            {
                Response.Redirect(Request.Url.AbsoluteUri, false);
            }
        }
    }

    public class Utils
    {
        public static void Llenar<T>(ListControl ctrl, List<T> datos, bool todos = false, bool seleccionar = false, bool orderByDescripcion = false) where T : IDescripcionId, new()
        {
            List<T> t = datos;
            if (todos)
                t.Add(new T() { Id = -1, Descripcion = "( -- Todos -- )" });
            if (seleccionar)
                t.Add(new T() { Id = -1, Descripcion = "( -- Seleccionar -- )" });
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
        /// <summary>
        /// Clase generica que recibe un contexto y puede trabajar con cualer clase del model
        /// </summary>
        /// <typeparam name="TContext">Object del Contexto del modelo de entity EDM</typeparam>
        public class GenericRepository<TContext> where TContext : DbContext, new()
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
        }
    }

}
