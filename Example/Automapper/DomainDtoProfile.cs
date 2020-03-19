



using AutoMapper;

public class DomainDtoProfile:Profile{



    public DomainDtoProfile()
    {
        CreateMap<DomainModels.User,DataTransferModels.User>()
            .ReverseMap()
        ;
        CreateMap<DomainModels.Post,DataTransferModels.Post>()
            .ReverseMap()
        ;

        CreateMap<DomainModels.Project,DataTransferModels.Project>()
            .ReverseMap()
        ;
    }
}