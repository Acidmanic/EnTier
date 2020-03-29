




using EnTier.Controllers;
using Microsoft.AspNetCore.Mvc;
using EnTier.Plugging;

namespace Controllers{



    [Route("api/v1/{Controller}")]
    public class ProjectsController : EnTierControllerBase
        <StorageModels.Project, DomainModels.Project, DataTransferModels.Project>
    {
        public ProjectsController(IObjectMapper mapper) : base(mapper)
        {
        }

        
    }
}