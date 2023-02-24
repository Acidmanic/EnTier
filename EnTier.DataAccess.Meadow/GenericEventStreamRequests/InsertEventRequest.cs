using EnTier.Repositories.Models;
using Meadow.Contracts;

namespace EnTier.DataAccess.Meadow.GenericEventStreamRequests;

public sealed class
    InsertEventRequest<TEvent, TEventId, TStreamId> : 
        EventStreamRequestBase<TEvent, TEventId, TStreamId, ObjectEntry<TEventId, TStreamId>>
{
    public InsertEventRequest(ObjectEntry<TEventId, TStreamId> entry)
    {
        ToStorage = entry;
    }


    protected override string PickName(NameConvention nameConvention)
    {
        return nameConvention.InsertEvent;
    }
}