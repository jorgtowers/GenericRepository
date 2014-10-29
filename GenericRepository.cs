using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Objects.DataClasses;
using System.Data.Objects;
using System.Collections;
using WebApplication1.Model;

namespace WebProject.Model
{
    public class GenericRepository
    {
        protected NOMBRE_PROYECTO-Entities model = new NOMBRE_PROYECTO-Entities ();
        const string keyPropertyName = "Id";
        protected interface IRepository
        {
            IQueryable<T> List<T>() where T : class ;
            T Get<T>( int id) where T : class;
            void Create<T>(T entityTOCreate) where T : class;
            void Edit<T>(T entityToEdit) where T : class;
            void Delete<T>(T entityToDelete) where T : class;
        }
        public virtual T Obtener<T>( int id) where T : class
        {
            return Get<T>(id);
        }
        public virtual List<T> Listado<T>() where T : class
        {
            return List<T>().ToList();
        }
        public virtual void Agregar<T>(T entity) where T : class
        {
            /*  AddObject: aplica para el Framework <= 4.0
                model.AddObject(string.Format("{0}Set", entity.GetType().Name), entity);
             */
            //Entry<T>: aplica para el Framework = > 4.1
            model.Entry<T>(entity).State = System.Data. EntityState.Added;
           
            model.SaveChanges();
        }
        public virtual void Modificar<T>(T entity) where T : class
        {
            //model.GetObjectByKey(entity.EntityKey);
            var orginalEntity = Get<T>(GetKeyPropertyValue<T>(entity));
            /*  ApplyCurrentValues<T>: aplica para el Framework <= 4.0
                model.ApplyCurrentValues<T>(string.Format("{0}Set", entity.GetType().Name), entity);
             */            
            //Entry<T>: aplica para el Framework => 4.1
            model.Entry<T>(entity).State = System.Data. EntityState.Modified;
            model.SaveChanges();
            // ORIGNIAL model.ApplyCurrentValues<entity as entity.GetType()>(string.Format("{0}Set", entity.GetType().Name), entity);
            //var orginalEntity = Get<T>(GetKeyPropertyValue<T>(entity));
            //model.ApplyPropertyChanges(GetEntitySetName<T>(), entity);
            //model.SaveChanges();
        }
        public virtual void Eliminar<T>(T entity) where T : class
        {
            var orginalEntity = Get<T>(GetKeyPropertyValue<T>(entity));
            /*  ApplyCurrentValues<T>: aplica para el Framework <= 4.0
                model.DeleteObject(orginalEntity);
             */
            //Entry<T>: aplica para el Framework => 4.1
            model.Entry<T>(entity).State = System.Data. EntityState.Deleted;
           
            model.SaveChanges();
        }
        protected T Get<T>( int id) where T : class
        {
            return List<T>().FirstOrDefault<T>(CreateGetExpression<T>(id));
        }
        protected int GetKeyPropertyValue<T>( object entity)
        {
            return ( int) typeof(T).GetProperty(keyPropertyName).GetValue(entity, null);
        }
        protected string GetEntitySetName<T>()
        {
            return String.Format( "{0}Set", typeof (T).Name);
        }
        protected Expression<Func <T, bool >> CreateGetExpression<T>(int id)
        {
            ParameterExpression e = Expression .Parameter(typeof(T), "e");
            PropertyInfo propinfo = typeof (T).GetProperty(keyPropertyName);
            MemberExpression m = Expression .MakeMemberAccess(e, propinfo);
            ConstantExpression c = Expression .Constant(id, typeof(int));
            BinaryExpression b = Expression .Equal(m, c);
            Expression<Func <T, bool >> lambda = Expression.Lambda< Func<T, bool>>(b, e);
            return lambda;
        }
        protected IQueryable<T> List<T>() where T : class
        {
            //Set<T>: aplica para el Framework => 4.1
            return model.Set<T>();
            /*  CreateQuery<T>: aplica para el Framework <= 4.0
                return model.CreateQuery<T>(GetEntitySetName<T>());
             */

        }

      


    }

    public interface IPropiedades
    {
        int Id { get; set; }
    }

    public class GenericCollection<T> : ICollection<T>, IList <T> where T : IPropiedades, new ()
    {
        List<T> lista = new List<T>();

        public GenericCollection()
        {

        }
        public GenericCollection( List<T> items)
        {
            lista = items;
        }

        public List<T> this[ int desde, int hasta]
        {
            get
            {
                List<T> items = new List<T>();
                if (desde == 0 && hasta == -1)
                   return lista;
                try
                {
                    hasta = (hasta == -1 ? lista.Count - 1 : (lista.Count > hasta ? hasta : lista.Count));
                    for ( int i = desde; i <= hasta; i++)
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

        IEnumerator IEnumerable .GetEnumerator()
        {
            foreach ( var item in lista)
            {
                yield return item;
            }
        }
        #endregion

        public T GetObject( int Id)
        {
            var query = from o in lista
                        where o.Id == Id
                        select o;
            foreach ( var item in query)
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

        public void Insert( int index, T item)
        {
            lista.Insert(index, item);
        }

        public void RemoveAt( int index)
        {
            lista.RemoveAt(index);
        }

        public T this[ int index]
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
