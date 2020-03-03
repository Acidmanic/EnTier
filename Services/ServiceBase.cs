



using AutoMapper;
using DataAccess;

namespace Services
{
    public class ServiceBase{

        protected IMapper Mapper{get;private set;}
        protected IProvider<DatabaseUnit> DbProvider{get;private set;}



        public ServiceBase(IMapper mapper,IProvider<DatabaseUnit> dbProvider)
        {
            Mapper = mapper;
            DbProvider = dbProvider;
        }
    }
}