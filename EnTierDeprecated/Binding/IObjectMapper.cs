


namespace EnTier.Binding
{

    public interface IObjectMapper{

        TDestination Map<TDestination>(object src);

        void Map<TDestination,TSource>(TSource src,TDestination dst);
    }

}