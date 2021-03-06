



using System;
using System.Collections.Generic;
using EnTier.Reflection;
using EnTier.Repository;
using EnTier.Binding;
using EnTier.Utility;
using EnTier.DataAccess;
using EnTier.Context;
using Microsoft.EntityFrameworkCore;
using EnTier.Components;

namespace EnTier.Service
{
    public class ServiceBase<StorageEntity,DomainEntity,Tid>
        :IService<DomainEntity,Tid> , IDisposable
        where StorageEntity:class{

        protected IObjectMapper Mapper{get;private set;}

        protected IProvider<IUnitOfWork> DbProvider{get;private set;}
        private EagerScopeManager _attributesScope;
        public ServiceBase(IObjectMapper mapper)
        {
            Initialize();
           
            Mapper = mapper;
        }

        public ServiceBase(){
            Initialize();

            Mapper = new ComponentProducer().ProduceMapper();
        }


        private void Initialize()
        {
            _attributesScope = new EagerAttributeProcessor()
               .MarkEagers<StorageEntity>(this);

            DbProvider = new Provider<IUnitOfWork>(() => new UnitOfWork());
        }

        public virtual List<DomainEntity> GetAll()
        {

            var res = new List<StorageEntity> ();

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageEntity,Tid>();

                res = repo.GetAll();

                db.Compelete();
            }

            return Mapper.Map<List<DomainEntity>>(res);
        }

        public virtual DomainEntity GetById(Tid id)
        {

            StorageEntity res = null;

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageEntity,Tid>();

                res = repo.GetById(id);

                db.Compelete();
            }

            return Mapper.Map<DomainEntity>(res);
        }

        public virtual DomainEntity Update(DomainEntity entity)
        {
            
            StorageEntity storage = null;

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageEntity,Tid>();

                new DataReflection().UseId<DomainEntity,Tid>(entity,id => storage = repo.GetById(id));

                if (storage == null){
                    NotFound();
                }

                Mapper.Map(entity,storage);

                db.Compelete();
            }

            return Mapper.Map<DomainEntity>(storage);
        }

        public virtual DomainEntity DeleteByEntity(DomainEntity entity)
        {
            StorageEntity storage = null;

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageEntity,Tid>();

                new DataReflection().UseId<DomainEntity,Tid>(entity,id => storage = repo.GetById(id));
                
                storage = repo.Remove(storage);

                db.Compelete();
            }

            return Mapper.Map<DomainEntity>(storage);
        }

        public virtual DomainEntity DeleteById(Tid id)
        {
            StorageEntity storage = null;

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageEntity,Tid>();

                storage = repo.RemoveById(id);

                db.Compelete();
            }
            
            if( storage == null){
                NotFound();
            }

            return Mapper.Map<DomainEntity>(storage);
        }

        private void NotFound()
        {
            //TODO: manage exceptions to get translated to responses
            throw new Exception();
        }

        public virtual DomainEntity CreateNew(DomainEntity entity)
        {

            var storage = Mapper.Map<StorageEntity>(entity);

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageEntity,Tid>();

                storage = repo.Add(storage);

                db.Compelete();
            }

            return Mapper.Map<DomainEntity>(storage);
        }

        public virtual void Dispose()
        {
            if(_attributesScope != null){
                _attributesScope.Dispose();
            }
        }
    }


    public abstract class ServiceBase<StorageEntity, DomainEntity>
        : ServiceBase<StorageEntity, DomainEntity, long>
        where StorageEntity : class
    {
        public ServiceBase()
        {
        }

        public ServiceBase(IObjectMapper mapper) : base(mapper)
        {
        }

    }
}