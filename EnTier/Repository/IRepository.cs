
using System;
using System.Collections.Generic;

namespace Repository{

    public interface IRepository{};
    public interface IRepository<Entity,Tid>:IRepository
    {

        List<Entity> GetAll();

        List<Entity> Find(Func<Entity, bool> condition);

        Entity GetById(Tid id);

        Entity GetById(Entity entity);

        Entity Add(Entity value);

        Entity Remove(Entity value);

        Entity RemoveById(Tid id);

        
    }

    public interface IRepository<Entity>:IRepository<Entity,long>{
        
    }
    
}