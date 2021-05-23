using System;
using AutoMapper;
using Example.AutoMapper.Domain;
using Example.AutoMapper.Dto;
using Example.AutoMapper.Storage;

namespace Example.AutoMapper.MapperProfiles
{
    public class ExampleProfile:Profile
    {
        public ExampleProfile()
        {
            // CreateMap<Post, PostStg>()
            //     .ForMember(p => p.Id,
            //         opt => opt.MapFrom(
            //             (p, pd) => p.Id.ToString()));
            // CreateMap<PostStg, Post>()
            //     .ForMember(p => p.Id,
            //         opt => opt.MapFrom(
            //             (pd, p) => Guid.TryParse(pd.Id, out var guid) ? guid : new Guid()));

            CreateMap<Guid,string>().ConvertUsing(g => g.ToString());
            
            CreateMap<string,Guid>().ConvertUsing(s => Guid.Parse(s));
            
            CreateMap<Post, PostStg>().ReverseMap();
            
            CreateMap<Post, PostDto>()
                .ForMember(p => p.Id,
                    opt => opt.MapFrom(
                        (p, pd) => p.Id.ToString()));
            CreateMap<PostDto, Post>()
                .ForMember(p => p.Id,
                    opt => opt.MapFrom(
                        (pd, p) => Guid.TryParse(pd.Id, out var guid) ? guid : new Guid()));
        }

        
    }
}