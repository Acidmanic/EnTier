



namespace AutoMapper
{
    

    public class StorageDomainProfile:Profile{



        public StorageDomainProfile()
        {
            CreateMap<StorageModels.User,DomainModels.User>()
                .ReverseMap();

                
        }
    }

}