






namespace AutoMapper
{
    public class StorageTransferProfile:Profile{


        public StorageTransferProfile()
        {
            CreateMap<StorageModels.User,DataTransferModels.User>()
                .ReverseMap();

        }
    }
}