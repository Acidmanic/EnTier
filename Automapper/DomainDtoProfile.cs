



using AutoMapper;

public class DomainDtoProfile:Profile{



    public DomainDtoProfile()
    {
        CreateMap<LoginResponse,LoginResponseDto>()
            .ReverseMap()
        ;
    }
}