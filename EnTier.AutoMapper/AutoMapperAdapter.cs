using AutoMapper;

namespace EnTier.AutoMapper
{
    public class AutoMapperAdapter:EnTier.Mapper.IMapper
    {
        private readonly global::AutoMapper.Mapper _mapper;

        public AutoMapperAdapter(IConfigurationProvider configurationProvider)
        {
            _mapper = new global::AutoMapper.Mapper(configurationProvider);
        }
        public TDestination Map<TDestination>(object src)
        {
            return _mapper.Map<TDestination>(src);
        }

        public void Map<TDestination, TSource>(TSource src, TDestination dst)
        {
            _mapper.Map(src, dst);
        }
    }
}