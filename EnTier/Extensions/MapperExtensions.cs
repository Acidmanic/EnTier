// ReSharper disable once CheckNamespace

namespace EnTier.Mapper
{
    internal static class MapperExtensions
    {
        public static TDest MapId<TDest>(this IMapper mapper, object srcId)
        {
            if (srcId == null)
            {
                return default;
            }

            if (typeof(TDest) == srcId.GetType())
            {
                return (TDest) srcId;
            }

            return mapper.Map<TDest>(srcId);
        }
    }
}