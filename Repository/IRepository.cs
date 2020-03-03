
using System;
using System.Collections.Generic;

namespace Repository{

    public interface IRepository<Entity>
    {

        List<Entity> GetAll();

        List<Entity> Find(Func<Entity, bool> condition);

        Entity GetById<Tid>(Tid id);

        Entity Add(Entity value);

        void Remove(Entity value);

        void RemoveById<Tid>(Tid id);

        
    }


    
}