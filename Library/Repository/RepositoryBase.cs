using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Reflection;
using DataAccess;

namespace Repository
{
    public abstract class RepositoryBase<Entity> : IRepository<Entity> where Entity:class
    {

        protected DbSet<Entity> DbSet{get; private set;}

        public RepositoryBase(DbSet<Entity> dbset)
        {
            DbSet = dbset;
        }

        public virtual Entity Add(Entity value)
        {
            return DbSet.Add(value).Entity;
        }

        protected IQueryable<Entity> ApplyEagerMarking(IQueryable<Entity> queryable)
        {
            return EagerScopeManager.Apply(queryable);
        }

        protected List<Entity> GetAll(Action<IEagerMarker<Entity>> mark = null)
        {
            var ret = DbSet.Where(e => true);

            ret = ApplyEagerMarking(ret);

            var retList = ret.ToList();

            return retList;
        }

        protected List<Entity> GetByCondition(Func<Entity, bool> condition, Action<IEagerMarker<Entity>> mark = null)
        {
            var ret = DbSet.Where(condition).AsQueryable();

            ret = ApplyEagerMarking(ret);

            return ret.ToList();
        }

        protected Entity GetById<Tid>(Tid id, Action<IEagerMarker<Entity>> mark = null)
        {
            var reader = new DataReflection().IdReader<Entity,Tid>(id);

            var ret = DbSet.Where(reader).AsQueryable();

            ret = ApplyEagerMarking(ret);

            return ret.FirstOrDefault();
        }

        public virtual Entity Remove(Entity value)
        {
            return DbSet.Remove(value).Entity;
        }

        public virtual Entity RemoveById<Tid>(Tid id)
        {
            var entity = GetById(id);

            if(entity != null)
            {
                return DbSet.Remove(entity).Entity;
            }

            return null;
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

        protected Entity GetById<Tid>(Entity entity,Action<IEagerMarker<Entity>> marker = null )
        {
            var idProperty = new DataReflection().GetIdProperty<Entity,Tid>();

            try
            {
                var eId = (Tid)idProperty.GetValue(entity);

                return GetById(eId,marker);

            }
            catch (System.Exception){            }

            return null;
        }

        public Entity GetById<Tid>(Entity entity){
            return GetById(entity,null);
        }
    }

}