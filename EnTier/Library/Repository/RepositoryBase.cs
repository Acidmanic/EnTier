using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Reflection;
using DataAccess;

namespace Repository
{
    public abstract class RepositoryBase<Entity> 
        : IDisposable, IRepository<Entity> 
        where Entity:class
    {

        protected DbSet<Entity> DbSet{get; private set;}

        private readonly EagerScopeManager _attributesScope=null;

        public RepositoryBase(DbSet<Entity> dbset)
        {
            DbSet = dbset;

            _attributesScope = new EagerAttributeProcessor()
                .MarkEagers<Entity>(this);
        }

        public virtual Entity Add(Entity value)
        {
            return DbSet.Add(value).Entity;
        }

        protected IQueryable<Entity> ApplyEagerMarking(IQueryable<Entity> queryable)
        {
            return EagerScopeManager.Apply(queryable);
        }

        public virtual List<Entity> GetAll()
        {
            var ret = DbSet.Where(e => true);

            ret = ApplyEagerMarking(ret);

            var retList = ret.ToList();

            return retList;
        }

        protected List<Entity> GetByCondition(Func<Entity, bool> condition)
        {
            var data = DbSet.AsQueryable();

            data = ApplyEagerMarking(data);

            var ret = data.Where(condition);
            
            return ret.ToList();
        }

        public virtual Entity GetById<Tid>(Tid id)
        {
            var reader = new DataReflection().IdReader<Entity,Tid>(id);

            var data = DbSet.AsQueryable();
            
            data = ApplyEagerMarking(data);

            var ret = data.Where(reader);

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

        public virtual List<Entity> Find(Func<Entity, bool> condition)
        {
            return GetByCondition(condition);
        }


        public virtual Entity GetById<Tid>(Entity entity)
        {
            var idProperty = new DataReflection().GetIdProperty<Entity,Tid>();

            try
            {
                var eId = (Tid)idProperty.GetValue(entity);

                return GetById(eId);

            }
            catch (System.Exception){            }

            return null;
        }

        public virtual void Dispose(){
            if(_attributesScope !=null){
                _attributesScope.Dispose();
            }
        }
    }

}