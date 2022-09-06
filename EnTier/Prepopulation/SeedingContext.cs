using System;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using EnTier.Repositories;
using EnTier.UnitOfWork;

namespace EnTier.Prepopulation
{
    public class SeedingContext<TModel, TId> where TModel : class, new()
    {
        public ICrudRepository<TModel, TId> Repository { get;  }

        public bool HasId { get;  }

        public AccessNode IdLeaf { get;  }
        
        public Type ModelType { get;  }
        
        public Type IdType { get; }

        public SeedingContext(IUnitOfWork unitOfWork)
        {
            IdLeaf = TypeIdentity.FindIdentityLeaf<TModel, TId>();

            HasId = IdLeaf != null;

            Repository = unitOfWork.GetCrudRepository<TModel, TId>();

            ModelType = typeof(TModel);

            IdType = typeof(TId);
        }
    }
}