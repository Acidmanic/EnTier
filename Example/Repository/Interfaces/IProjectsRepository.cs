
using DIBinding;

namespace Repository{



    [InjectionEntry]
    public interface IProjectsRepository:IRepository<StorageModels.Project>
    {
        
    }
}