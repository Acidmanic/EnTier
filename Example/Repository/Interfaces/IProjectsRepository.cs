
using EnTier.Annotations;
using EnTier.Components;
using EnTier.Repository;

namespace Repository
{



    [InjectionEntry]
    public interface IProjectsRepository:IRepository<StorageModels.Project>
    {
        
    }
}