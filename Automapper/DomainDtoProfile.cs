



using AutoMapper;

public class DomainDtoProfile:Profile{



    public DomainDtoProfile()
    {
        CreateMap<LoginResponse,LoginResponseDto>()
            .ReverseMap()
        ;

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