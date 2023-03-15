using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EnTier.Repositories.Models;

namespace EnTier.DataAccess.EntityFramework;

public class EfObjectEntry<TEventId, TStreamId> : ObjectEntry<TEventId, TStreamId>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override TEventId EventId { get; set; }
}

public static class ObjectEntryEfExtensions
{
    public static EfObjectEntry<TEventId, TStreamId> ToEf<TEventId, TStreamId>(
        this ObjectEntry<TEventId, TStreamId> value)
    {
        return new EfObjectEntry<TEventId, TStreamId>
        {
            EventId = value.EventId,
            SerializedValue = value.SerializedValue,
            StreamId = value.StreamId,
            TypeName = value.TypeName
        };
    }
}