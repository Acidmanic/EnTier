




using Microsoft.AspNetCore.Mvc;
using Plugging;

namespace Controllers{



    [Route("api/v1/{Controller}")]
    public class ProjectsController : EntityControllerBase
        <StorageModels.Project, DomainModels.Project, DataTransferModels.Project>
    {
        public ProjectsController(IObjectMapper mapper) : base(mapper)
        {
        }

        
    }
}