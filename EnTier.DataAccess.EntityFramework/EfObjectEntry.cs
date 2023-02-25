using EnTier.Repositories.Models;

namespace EnTier.DataAccess.EntityFramework;

public class EfObjectEntry<TEventId, TStreamId> : ObjectEntry<TEventId, TStreamId>
{
    public TEventId Id { get; set; }

    public override TEventId EventId
    {
        get => Id;
        set => Id = value;
    }

}

public static class ObjectEntryEfExtensions
{

    public static EfObjectEntry<TEventId, TStreamId> ToEf<TEventId, TStreamId>(
        this ObjectEntry<TEventId, TStreamId> value)
    {
        return new EfObjectEntry<TEventId, TStreamId>
        {
            Id = value.EventId,
            SerializedValue = value.SerializedValue,
            StreamId = value.StreamId,
            TypeName = value.TypeName
        };
    }
}