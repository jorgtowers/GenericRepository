using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.Objects;
using System.Data.Objects.DataClasses;

using System.Data;
using Reuniones.Model;
using System.Web.UI.WebControls;
using System.Web.UI;
using GenericRepository.EF5;
using System.Text;


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

        protected virtual void Limpiar (object sender,EventArgs e){
            foreach (Control item in Page.Form.Controls[0].Controls)
            {
                if (item.GetType() == typeof(TextBox)) {
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
        }

    }

    public class PageDynamic<T> : AbstractPage where T : class,IId,new()
    {
        private Panel _Panel = new Panel();

        public Panel Panel
        {
            get { return _Panel; }
            set { _Panel = value; }
        }

        
       List< KeyValuePair<string, string>> _Fields = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> Fields
        {
            get { return _Fields; }
            set { _Fields = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            Type TDynamic=null;

            base.CheckParametrosUrlQueryString();


            if (string.IsNullOrEmpty(base.Clase))
                TDynamic = typeof(T);
            else
                TDynamic = Type.GetType("Reuniones.Model." + base.Clase);

            

            base.OnInit(e);

            PropertyInfo[] propiedades = TDynamic.GetProperties();

            #region Mantenimiento

            
            _Panel.Controls.Add(new LiteralControl("<table class='table'><tbody>"));
            foreach (PropertyInfo propiedad in propiedades)
            {
               // StringBuilder sb = new StringBuilder();

                //Tipo Nulleable
                string tipo = "";
                string nombre = "";
                if (propiedad.PropertyType.GetGenericArguments().Count() > 0)
                {
                    //sb.AppendLine("<b>Tipo:</b>" + propiedad.PropertyType.GetGenericArguments()[0].Name);
                    tipo = propiedad.PropertyType.GetGenericArguments()[0].Name;
                }
                else
                {
                    //sb.AppendLine("<b>Tipo:</b>" + propiedad.PropertyType.Name);
                    tipo = propiedad.PropertyType.Name;
                }
                //sb.AppendLine("<b>Namespace:</b>" + propiedad.PropertyType.Namespace);
                if (propiedad.PropertyType.Namespace == "System")
                {
                   // sb.AppendLine("<b>Campo Nombre:</b>" + propiedad.Name);
                    nombre = propiedad.Name;
                    

                    if (tipo == "String" || tipo == "Int32" || tipo == "DateTime")
                    {
                        _Fields.Add(new KeyValuePair<string, string>(nombre, tipo));
                        _Panel.Controls.Add(new LiteralControl("<tr class='help'><td  class='info'>" + nombre + "</td><td>"));
                        TextBox t = new TextBox() { ID = "txt" + nombre.Replace(" ", ""), CssClass = "form-control" };
                        if (nombre == "Id")
                        {
                            t.Enabled = false;
                            t.Attributes.Add("optional", "si");
                        }
                        if (nombre == "Password")
                        {
                            t.TextMode = TextBoxMode.Password;
                        }
                        _Panel.Controls.Add(t);
                        _Panel.Controls.Add(new LiteralControl("</td></tr>"));
                    }
                    if (tipo == "Boolean")
                    {
                        _Panel.Controls.Add(new LiteralControl("<tr><td>" + nombre + "</td><td>"));
                        CheckBox t = new CheckBox() { ID = "chk" + nombre.Replace(" ", "") };
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
            Label lblEstatus = new Label() { ID = "lblEstatus"};
            _Panel.Controls.Add(lblEstatus);
            _Panel.Controls.Add(new LiteralControl("</td></tr>"));

            _Panel.Controls.Add(new LiteralControl("<tr><td colspan='2'>"));
            
            Button btnAgregar = new Button() { ID = "btnAgregar", CssClass = "btn btn-primary", Text="Agregar" };
            btnAgregar.Click += Agregar;
            if (base.Id > 0)
                btnAgregar.Enabled = false;
            _Panel.Controls.Add(btnAgregar);
            
            Button btnModificar = new Button() { ID = "btnModificar", CssClass = "btn btn-default", Text="Modificar" };
            btnModificar.Click += Modificar;
            if (base.Id <0)
                btnModificar.Enabled = false;
            _Panel.Controls.Add(btnModificar);

            Button btnEliminar = new Button() { ID = "btnEliminar", CssClass = "btn btn-default" , Text="Eliminar"};
            btnEliminar.Click += Eliminar;
            if (base.Id < 0)
                btnEliminar.Enabled = false;
            _Panel.Controls.Add(btnEliminar);

            Button btnLimpiar = new Button() { ID = "btnLimpiar", CssClass = "btn btn-default", Text="Limpiar" };
            btnLimpiar.Click += Limpiar;
            _Panel.Controls.Add(btnLimpiar);
            
            _Panel.Controls.Add(new LiteralControl("</td></tr>"));

            _Panel.Controls.Add(new LiteralControl("</tbody></table>"));

            #endregion

            #region Listado
            _Panel.Controls.Add(new LiteralControl("<table class='table table-condensed'><thead><tr>"));            
            foreach (KeyValuePair<string,string> headers in _Fields)
            {
            _Panel.Controls.Add(new LiteralControl("<td>" + headers.Key + "</td>"));    
            }
            _Panel.Controls.Add(new LiteralControl("</tr></thead>"));
            _Panel.Controls.Add(new LiteralControl("<tbody><tr>"));
            foreach (KeyValuePair<string,string> headers in _Fields)
            {
            _Panel.Controls.Add(new LiteralControl("<td>" + headers.Key + "</td>"));    
            }
            _Panel.Controls.Add(new LiteralControl("</tr></tbody></table>"));            

            #endregion

        }

        private T ObjectToUpdate()
        {
            T _;
            if (base.Id > 0)
                _ = model.Obtener<T>(base.Id);
            else
                _ = new T();

            foreach (KeyValuePair<string, string> campo in Fields)
            {

                foreach (Control control in _Panel.Controls)
                {
                    if (control.ID == "txt" + campo.Key)
                    {
                        TextBox txt=((TextBox)control);
                        if (txt.Enabled)
                        {
                            //Type tipoDeItem = _.GetType().BaseType;
                            Type tipoDePropiedad = Type.GetType("System." + campo.Value);
                            PropertyInfo propiedad = _.GetType().GetProperty(campo.Key);
                            var valor = txt.Text;
                            propiedad.SetValue(_, Convert.ChangeType(valor, tipoDePropiedad), null);
                        }
                    }
                }
            }

            return _;
        }

        private string _Resultado = "";
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
              }
        }

        private void FillCampos(T item) {
            try
            {
                foreach (KeyValuePair<string, string> campo in Fields)
                {

                    foreach (Control control in _Panel.Controls)
                    {
                        if (control.ID == "txt" + campo.Key)
                        {
                            TextBox txt=((TextBox)control);
                            //Type tipoDeItem = item.GetType().BaseType;
                            Type tipoDePropiedad = Type.GetType("System." + campo.Value);
                            PropertyInfo propiedad = item.GetType().GetProperty(campo.Key);
                            object resultado = propiedad.GetValue(item, null);
                            txt.Text = resultado.ToString();
                        }

                    }
                }
            }
            catch (Exception ex){
                _Resultado = ex.Message;
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
            finally {
                foreach (Control control in _Panel.Controls)
                {                    
                    if (control.ID == "lblEstatus")
                    {
                        ((Label)control).Text = _Resultado;                        
                    }
                }
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
                foreach (Control control in _Panel.Controls)
                {
                    if (control.ID == "lblEstatus")
                    {
                        ((Label)control).Text = _Resultado;
                    }
                }
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
                foreach (Control control in _Panel.Controls)
                {
                    if (control.ID == "lblEstatus")
                    {
                        ((Label)control).Text = _Resultado;
                    }
                }
                Limpiar(sender, e);
            }
        }

        protected virtual void Limpiar(object sender, EventArgs e)
        {
            foreach (Control item in _Panel.Controls)
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
        }

    }

    public class Utils
    {
        public interface IDescripcionId
        {
            string Descripcion { get; set; }
            int Id { get; set; }
        }
        public static void Llenar<T>(ListControl ctrl, List<T> datos, bool todos = false, bool seleccionar = false) where T : IDescripcionId, new()
        {
            List<T> t = datos;
            if (todos)
                t.Add(new T() { Id = -1, Descripcion = "( -- Todos -- )" });
            if (seleccionar)
                t.Add(new T() { Id = -1, Descripcion = "( -- Todos -- )" });
            ctrl.DataTextField = "Descripcion";
            ctrl.DataValueField = "Id";
            ctrl.DataSource = t.OrderBy(x => x.Id);
            ctrl.DataBind();
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
            protected static Entities context = new Entities();

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
    {    /// <summary>
        /// Interfaz que asegura la existencia del campo Id
        /// </summary>
        public interface IId
        {
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
            /// <param name="id">Id del objeto a consultar, el cual debe implementar la interfaz EntityFramework.EF5.IId</param>
            /// <returns>Returna instancia del objeto del tipo T</returns>
            public virtual T Obtener<T>(int id) where T : class,IId
            {
                return Listado<T>().Where(x => x.Id == id).FirstOrDefault();
            }
            /// <summary>
            /// Permite obtener un IQueryable para ejecutar un listado del Type de la clase solicitada
            /// </summary>
            /// <typeparam name="T">Type de la clase solicitada</typeparam>
            /// <returns>IQueriable para ejecutar un .ToList()</returns>
            public virtual IQueryable<T> Listado<T>() where T : class
            {
                return _context.Set<T>();
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

                _context.Entry<T>(entity).State = EntityState.Deleted;
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
                _context.Entry<T>(entity).State = EntityState.Modified;
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
            /// Salva los cambios realizados en el Contexto
            /// </summary>
            private void Save()
            {
                _context.SaveChanges();
            }
        }
    }

}
