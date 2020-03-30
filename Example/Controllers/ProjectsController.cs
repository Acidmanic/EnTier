




using EnTier.Controllers;
using Microsoft.AspNetCore.Mvc;
using EnTier.Binding;

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