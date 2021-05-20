namespace EnTier.Mapper
{
    public interface IMapper
    {
        TDestination Map<TDestination>(object src);

        void Map<TDestination, TSource>(TSource src, TDestination dst);
    }
}