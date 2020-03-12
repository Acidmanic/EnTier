



using AutoMapper;


namespace Mapping{



    public class Automapper : Plugging.IObjectMapper
    {
        private readonly IMapper _mapper;

        public Automapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TDestination Map<TDestination>(object src)
        {
            return _mapper.Map<TDestination>(src);
        }

        public void Map<TDestination, TSource>(TSource src, TDestination dst)
        {
            _mapper.Map(src,dst);
        }
    }
}