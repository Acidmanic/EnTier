


using System;
using System.Collections.Generic;
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
    public EntityConterollerBase(IMapper mapper,IProvider<GenericDatabaseUnit> dbProvider)
    {
        _mapper = mapper;

        _dbProvider = dbProvider;
        
    }


    [HttpGet]
    [Route("")]
    public virtual List<TransferEntity> GetAll(){

        var ret = new List<StorageEntity> ();

        using(var db = _dbProvider.Create()){

            var repo = db.CreateRepository<StorageEntity>();

            ret = repo.GetAll();
        }

        return _mapper.Map<List<TransferEntity>>(ret);
    }


    [HttpGet]
    [Route("{id}")]
    public virtual TransferEntity GetById(Tid id){

        StorageEntity ret = null;;

        using(var db = _dbProvider.Create()){

            var repo = db.CreateRepository<StorageEntity>();

            ret = repo.GetById(id);
        }

        return _mapper.Map<TransferEntity>(ret);
    }


    [HttpPost]
    [Route("")]
    public virtual TransferEntity CreateNew(TransferEntity entity){

        var storage = _mapper.Map<StorageEntity>(entity);

        using(var db = _dbProvider.Create()){

            var repo = db.CreateRepository<StorageEntity>();

            storage = repo.Add(storage);

            db.Compelete();
        }

        return _mapper.Map<TransferEntity>(storage)
        ;
    }
    

    [HttpPut]
    [Route("")]
    public virtual TransferEntity Update(TransferEntity entity){

        StorageEntity storage = null;

        using(var db = _dbProvider.Create()){

            var repo = db.CreateRepository<StorageEntity>();

            new DataReflection().UseId<TransferEntity,Tid>(entity,id => storage = repo.GetById(id));

            if (storage == null){
                //fuck  it up
            }

            _mapper.Map(entity,storage);

            db.Compelete();
        }

        return _mapper.Map<TransferEntity>(storage);
    }

    [HttpDelete]
    [Route("{id}")]
    public TransferEntity DeleteById(Tid id){

        StorageEntity storage = null;

        using(var db = _dbProvider.Create()){

            var repo = db.CreateRepository<StorageEntity>();

            storage = repo.RemoveById(id);

            db.Compelete();
        }
        // if storage == null, fuck up

        return _mapper.Map<TransferEntity>(storage);
    }


    [HttpDelete]
    [Route("")]
    public TransferEntity Delete(TransferEntity entity){

        StorageEntity storage = null;

        using(var db = _dbProvider.Create()){

            var repo = db.CreateRepository<StorageEntity>();

            new DataReflection().UseId<TransferEntity,Tid>(entity,id => storage = repo.GetById(id));

            if (storage == null){
                //fuck  it up
            }

            storage = repo.Remove(storage);

            db.Compelete();
        }

        return _mapper.Map<TransferEntity>(storage);
    }
}