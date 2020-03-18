
using System;
using System.Collections.Generic;

namespace Repository{

    public interface IRepository<Entity>
    {

        List<Entity> GetAll();

        List<Entity> Find(Func<Entity, bool> condition);

        Entity GetById<Tid>(Tid id);

        Entity GetById<Tid>(Entity entity);

        Entity Add(Entity value);

        Entity Remove(Entity value);

        Entity RemoveById<Tid>(Tid id);

        
    }


    
}