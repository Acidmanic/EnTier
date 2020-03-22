



using System;
using System.Collections.Generic;
using Reflection;
using Repository;
using Plugging;
using Utility;
using DataAccess;
using Providers;

namespace Service
{
    public class ServiceBase<StorageEntity,DomainEntity,Tid>
        :IService<DomainEntity,Tid> , IDisposable
        where StorageEntity:class{

        protected IObjectMapper Mapper{get;private set;}

        protected IProvider<IUnitOfWork> DbProvider{get;private set;}

        private EagerScopeManager _attributesScope;
        public ServiceBase(IObjectMapper mapper,IProvider<IUnitOfWork> dbProvider)
        {
            Initialize(mapper,dbProvider);
        }


        public ServiceBase(IObjectMapper mapper)
        {
            var dbProvider = ResolveProvider();

            Initialize(mapper,dbProvider);
        }

        public ServiceBase(IProvider<IUnitOfWork> dbProvider)
        {
            var mapper = ResolveMapper();
            
            Initialize(mapper,dbProvider);
        }

        public ServiceBase()
        {
            var mapper = ResolveMapper();

            var dbProvider = ResolveProvider();
            
            Initialize(mapper,dbProvider);
        }

        private IObjectMapper ResolveMapper()
        {
            return EnTierApplication.Resolver.Resolve<IObjectMapper>();
        }

        private IProvider<IUnitOfWork> ResolveProvider()
        {
            return new UnitOfWorkProvider<StorageEntity>();
        }

        //ServiceBase Decides For it's Dependencies
        private void Initialize(IObjectMapper mapper,IProvider<IUnitOfWork> dbProvider){
            _attributesScope = new EagerAttributeProcessor()
                .MarkEagers<StorageEntity>(this);

            Mapper = mapper;
            DbProvider = dbProvider;
        }

        public List<DomainEntity> GetAll()
        {

            var res = new List<StorageEntity> ();

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageEntity,Tid>();

                res = repo.GetAll();

                db.Compelete();
            }

            return Mapper.Map<List<DomainEntity>>(res);
        }

        public DomainEntity GetById(Tid id)
        {

            StorageEntity res = null;

            using(var db = DbProvider.Create()){

                var repo = db.GetRepository<StorageEntity,Tid>();

                res = repo.GetById(id);

                db.Compelete();
            }

            return Mapper.Map<DomainEntity>(res);
        }

        public DomainEntity Update(DomainEntity entity)
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

        public DomainEntity DeleteByEntity(DomainEntity entity)
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

        public DomainEntity DeleteById(Tid id)
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

        public DomainEntity CreateNew(DomainEntity entity)
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
        public ServiceBase(IObjectMapper mapper, IProvider<IUnitOfWork> dbProvider) : base(mapper, dbProvider)
        {
        }
    }
}