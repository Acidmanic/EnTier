


using System;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;


[ApiController]
public abstract class EntityConterollerBase<StorageEntity,TransferEntity,Tid> : ControllerBase where StorageEntity:class
{

    private IMapper _mapper;
    private IProvider<GenericDatabaseUnit> _dbProvider;

    private ControllerConfigurations _configurations;


    public EntityConterollerBase(IMapper mapper,IProvider<GenericDatabaseUnit> dbProvider)
    {
        _mapper = mapper;

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

        if(!_configurations.ImplementsGetAll) return NotFound();

        var ret = new List<StorageEntity> ();

        using(var db = _dbProvider.Create()){

            var repo = db.CreateRepository<StorageEntity>();

            ret = repo.GetAll();
        }

        return Ok(_mapper.Map<List<TransferEntity>>(ret));
    }

    private class SafeRunResult{

        public StorageEntity Result{get; set;}
        
        public IActionResult ErrorReturningResult{get; set;}

        public bool Success {get;set;}
        
    }

    private SafeRunResult SafeRun(Func<StorageEntity> runnable,Func<IActionResult> onError){

        var ret = new SafeRunResult(){Success = true};
        try
        {
            ret.Result = runnable();
        }
        catch (System.Exception)
        {
            ret.Success = false;
            ret.ErrorReturningResult = onError();
        }

        return ret;
    }

    protected IActionResult Error(){
        return StatusCode((int)HttpStatusCode.InternalServerError);
    }

    private SafeRunResult SafeRun(Func<StorageEntity> runnable){
        return SafeRun(runnable, () => Error());
    }

    private TransferEntity Map(StorageEntity storage){
        return _mapper.Map<TransferEntity>(storage);
    }

    private List<TransferEntity> Map(ICollection<StorageEntity> storages){
        return _mapper.Map<List<TransferEntity>>(storages);
    }

    private IActionResult Map(SafeRunResult result){
        if(result.Success){

            var transfer = Map(result.Result);

            return Ok(transfer);
        }else{
            return result.ErrorReturningResult;
        }
    }

    [HttpGet]
    [Route("{id}")]
    public virtual IActionResult GetById(Tid id){

        if(!_configurations.ImplementsGetById) return NotFound();

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

        if(!_configurations.ImplementsCreateNew) return NotFound();

        var storage = _mapper.Map<StorageEntity>(entity);

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

        if(!_configurations.ImplementsUpdate) return NotFound();

        StorageEntity storage = null;

        using(var db = _dbProvider.Create()){

            var repo = db.CreateRepository<StorageEntity>();

            new DataReflection().UseId<TransferEntity,Tid>(entity,id => storage = repo.GetById(id));

            if (storage == null){
                return NotFound();
            }

            _mapper.Map(entity,storage);

            db.Compelete();
        }

        return Ok(Map(storage));
    }

    [HttpDelete]
    [Route("{id}")]
    public IActionResult DeleteById(Tid id){

        if(!_configurations.ImplementsDeleteById) return NotFound();

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

        if(!_configurations.ImplementsDeleteByEntity) return NotFound();

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