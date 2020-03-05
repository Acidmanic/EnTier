




using AutoMapper;
using DataAccess;
using DataTransferModels;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{


    [Route("api/v1/{Controller}")]
    public class KoonController : EntityConterollerBase<StorageModels.User, User, long>
    {
        public KoonController(IMapper mapper, IProvider<GenericDatabaseUnit> dbProvider) : base(mapper, dbProvider)
        {
        }
    }
}