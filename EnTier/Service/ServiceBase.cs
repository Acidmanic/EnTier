



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
using EnTier.Binding.Abstractions;

namespace EnTier.Service
{
    public class ServiceBase<StorageModel,DomainModel,Tid>
        :IService<DomainModel,Tid> , IDisposable
        where StorageModel:class{

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
            //TODO: Manage this requirment
            ReflectionService.Make().Cache(typeof(GenericRepository<StorageModel, Tid>));

            _attributesScope = new EagerAttributeProcessor()
               .MarkEagers<StorageModel>(this);

            DbProvider = new Provider<IUnitOfWork>(() => new UnitOfWork());
        }

        public virtual List<DomainModel> GetAll()
        {

            var res = new List<StorageModel> ();

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageModel,Tid>();

                res = repo.GetAll();

                db.Compelete();
            }

            return Mapper.Map<List<DomainModel>>(res);
        }

        public virtual DomainModel GetById(Tid id)
        {

            StorageModel res = null;

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageModel,Tid>();

                res = repo.GetById(id);

                db.Compelete();
            }

            return Mapper.Map<DomainModel>(res);
        }

        public virtual DomainModel Update(DomainModel entity)
        {
            
            StorageModel storage = null;

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageModel,Tid>();

                new DataReflection().UseId<DomainModel,Tid>(entity,id => storage = repo.GetById(id));

                if (storage == null){
                    NotFound();
                }

                Mapper.Map(entity,storage);

                db.Compelete();
            }

            return Mapper.Map<DomainModel>(storage);
        }

        public virtual DomainModel DeleteByEntity(DomainModel entity)
        {
            StorageModel storage = null;

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageModel,Tid>();

                new DataReflection().UseId<DomainModel,Tid>(entity,id => storage = repo.GetById(id));
                
                storage = repo.Remove(storage);

                db.Compelete();
            }

            return Mapper.Map<DomainModel>(storage);
        }

        public virtual DomainModel DeleteById(Tid id)
        {
            StorageModel storage = null;

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageModel,Tid>();

                storage = repo.RemoveById(id);

                db.Compelete();
            }
            
            if( storage == null){
                NotFound();
            }

            return Mapper.Map<DomainModel>(storage);
        }

        private void NotFound()
        {
            //TODO: manage exceptions to get translated to responses
            throw new Exception();
        }

        public virtual DomainModel CreateNew(DomainModel entity)
        {

            var storage = Mapper.Map<StorageModel>(entity);

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageModel,Tid>();

                storage = repo.Add(storage);

                db.Compelete();
            }

            return Mapper.Map<DomainModel>(storage);
        }

        public virtual void Dispose()
        {
            if(_attributesScope != null){
                _attributesScope.Dispose();
            }
        }
    }


    public abstract class ServiceBase<StorageModel, DomainModel>
        : ServiceBase<StorageModel, DomainModel, long>
        where StorageModel : class
    {
        public ServiceBase()
        {
        }

        public ServiceBase(IObjectMapper mapper) : base(mapper)
        {
        }

    }
}