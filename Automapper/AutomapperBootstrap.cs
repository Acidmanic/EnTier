



using AutoMapper;

public class AutomapperBootstrap{


    public static IMapper CreateMapper(){

        var config = new MapperConfiguration( cfg =>{

            cfg.AddProfile(new DomainDtoProfile());

            cfg.AddProfile(new StorageDomainProfile());

        });


        return config.CreateMapper();
    }



}