





using System.Collections.Generic;

namespace Service{

    public interface IService{}

    public interface IService<DomainEntity,TId>:IService{


        List<DomainEntity> GetAll();

        DomainEntity GetById(TId id);

        DomainEntity Update(DomainEntity entity);

        DomainEntity DeleteByEntity(DomainEntity entity);

        DomainEntity DeleteById(TId id);

        DomainEntity CreateNew(DomainEntity entity);
        
    }


    public interface IService<DomainEntity>:IService<DomainEntity,long>{

    }
}