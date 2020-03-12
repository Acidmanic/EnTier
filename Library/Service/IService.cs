





using System.Collections.Generic;

namespace Services{


    public interface IService<DomainEntity,TId>{


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