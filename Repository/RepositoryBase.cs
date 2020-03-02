using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Repository
{
    public abstract class RepositoryBase<Entity> : IRepository<Entity> where Entity:class
    {

        private DbSet<Entity> _dbset;

        public RepositoryBase(DbSet<Entity> dbset)
        {
            _dbset = dbset;
        }

        public virtual void Add(Entity value)
        {
            _dbset.Add(value);
        }

        private IQueryable<Entity> ApplyEagerMarking(IQueryable<Entity> queryable, Action<EagerMarker<Entity>> mark)
        {
            var marker = new IQuariableWrapperEagerMarker<Entity>(queryable);

            mark?.Invoke(marker);

            return marker.Result;
        }

        protected List<Entity> GetAll(Action<EagerMarker<Entity>> mark = null)
        {
            var ret = _dbset.Where(e => true);

            ret = ApplyEagerMarking(ret, mark);

            return ret.ToList();
        }

        protected List<Entity> GetByCondition(Func<Entity, bool> condition, Action<EagerMarker<Entity>> mark = null)
        {
            var ret = _dbset.Where(condition).AsQueryable();

            ret = ApplyEagerMarking(ret, mark);

            return ret.ToList();
        }


        private Func<Entity,bool> IdReader<Tid>(Tid id)
        {
            var idType = id.GetType();

            var entityType = typeof(Entity);

            var idProperty = entityType.GetProperty("Id", idType);

            if (idProperty != null)
            {
                return (Entity e) => {
                    try
                    {
                        var eId = (Tid)idProperty.GetValue(e);

                        return eId.Equals(id);
                    }
                    catch (Exception){}

                    return false;
                };
            }

            return e => false;
        }

        protected Entity GetById<Tid>(Tid id, Action<EagerMarker<Entity>> mark = null)
        {
            var reader = IdReader(id);

            var ret = _dbset.Where(reader).AsQueryable();

            ret = ApplyEagerMarking(ret, mark);

            return ret.FirstOrDefault();
        }

        public virtual void Remove(Entity value)
        {
            _dbset.Remove(value);
        }

        public virtual void RemoveById<Tid>(Tid id)
        {
            var entity = GetById(id);

            if(entity != null)
            {
                _dbset.Remove(entity);
            }
        }

        public virtual List<Entity> GetAll()
        {
            return GetAll(null);
        }

        public virtual List<Entity> Find(Func<Entity, bool> condition)
        {
            return GetByCondition(condition, null);
        }

        public virtual Entity GetById<Tid>(Tid id)
        {
            return GetById(id, null);
        }
    }

}