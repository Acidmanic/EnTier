
using System;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Controllers{

    public abstract class EntityControllerBase
        <StorageEntity,TransferEntity,Tid> : 
        RitchControllerBase<StorageEntity,TransferEntity> where StorageEntity:class
    {

        private IProvider<GenericDatabaseUnit> _dbProvider;

        private ControllerConfigurations _configurations;

        public EntityControllerBase(
            IObjectMapper mapper,
            IProvider<GenericDatabaseUnit> dbProvider
            ):base(mapper)
        {

            _dbProvider = dbProvider;

            ControllerConfigurationBuilder builder = new ControllerConfigurationBuilder();

            Configure(builder);

            _configurations = builder.Build();
            
        }

        protected virtual void Configure(ControllerConfigurationBuilder builder){
            builder.ImplementAll();
        }

        [HttpGet]
        [Route("")]
        public virtual IActionResult GetAll(){

            if(!_configurations.ImplementsGetAll) return Error(HttpStatusCode.MethodNotAllowed);

            var ret = new List<StorageEntity> ();

            using(var db = _dbProvider.Create()){

                var repo = db.CreateRepository<StorageEntity>();

                ret = repo.GetAll();
            }

            return Ok(Mapper.Map<List<TransferEntity>>(ret));
        }

        [HttpGet]
        [Route("{id}")]
        public virtual IActionResult GetById(Tid id){

            if(!_configurations.ImplementsGetById) return Error(HttpStatusCode.MethodNotAllowed);

            SafeRunResult ret ;

            using(var db = _dbProvider.Create()){

                var repo = db.CreateRepository<StorageEntity>();

                ret = SafeRun(()=> repo.GetById(id),()=> NotFound());
            }

            return Map(ret);
        }

        [HttpPost]
        [Route("")]
        public virtual IActionResult CreateNew(TransferEntity entity){

            if(!_configurations.ImplementsCreateNew) return Error(HttpStatusCode.MethodNotAllowed);

            var storage = Mapper.Map<StorageEntity>(entity);

            SafeRunResult ret ;

            using(var db = _dbProvider.Create()){

                var repo = db.CreateRepository<StorageEntity>();

                ret = SafeRun(()=>repo.Add(storage));

                db.Compelete();
            }

            return Map(ret);
        }
        

        [HttpPut]
        [Route("")]
        public virtual IActionResult Update(TransferEntity entity){

            if(!_configurations.ImplementsUpdate) return Error(HttpStatusCode.MethodNotAllowed);

            StorageEntity storage = null;

            using(var db = _dbProvider.Create()){

                var repo = db.CreateRepository<StorageEntity>();

                new DataReflection().UseId<TransferEntity,Tid>(entity,id => storage = repo.GetById(id));

                if (storage == null){
                    return NotFound();
                }

                Mapper.Map(entity,storage);

                db.Compelete();
            }

            return Ok(Map(storage));
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteById(Tid id){

            if(!_configurations.ImplementsDeleteById) return Error(HttpStatusCode.MethodNotAllowed);

            StorageEntity storage = null;

            using(var db = _dbProvider.Create()){

                var repo = db.CreateRepository<StorageEntity>();

                storage = repo.RemoveById(id);

                db.Compelete();
            }
            
            if( storage == null){
                return NotFound();
            }

            return Ok(Map(storage));
        }

        [HttpDelete]
        [Route("")]
        public IActionResult Delete(TransferEntity entity){

            if(!_configurations.ImplementsDeleteByEntity) return Error(HttpStatusCode.MethodNotAllowed);

            StorageEntity storage = null;

            SafeRunResult result;
            using(var db = _dbProvider.Create()){

                var repo = db.CreateRepository<StorageEntity>();

                result = SafeRun(() => {
                    new DataReflection().UseId<TransferEntity,Tid>(entity,id => storage = repo.GetById(id));
                    return storage;
                },()=> NotFound());
                
                if (result.Success){
                    result = SafeRun(() => storage = repo.Remove(storage));
                }

                db.Compelete();
            }

            return Map(result);
        }
    }
}