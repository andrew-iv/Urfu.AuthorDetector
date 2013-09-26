using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Urfu.AuthorDetector.DataLayer
{

    /*public class InMemoryDbSetWithConstructor<T>: InMemoryDbSet<T> where T : class,new
    {
        
    }*/

    public class InMemoryDbSet<T> : IDbSet<T> where T : class
    {

        readonly HashSet<T> _set;
        readonly IQueryable<T> _queryableSet;


        public InMemoryDbSet() : this(Enumerable.Empty<T>()) { }

        public InMemoryDbSet(IEnumerable<T> entities)
        {
            _set = new HashSet<T>();

            foreach (var entity in entities)
            {
                _set.Add(entity);
            }

            _queryableSet = _set.AsQueryable();
        }

        public T Add(T entity)
        {
            _set.Add(entity);
            return entity;
        }

        public void Add(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }


        public T Attach(T entity)
        {
            _set.Add(entity);
            return entity;
        }

        public virtual TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            throw new NotImplementedException();
        }

        public virtual T Create()
        {
            /*if (typeof (T).GetConstructor())
            {
                
            }*/

            throw new NotImplementedException();
        }

        public virtual T Find(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public System.Collections.ObjectModel.ObservableCollection<T> Local
        {
            get { throw new NotImplementedException(); }
        }

        public T Remove(T entity)
        {
            _set.Remove(entity);
            return entity;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType
        {
            get { return _queryableSet.ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return _queryableSet.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _queryableSet.Provider; }
        }
    }

}