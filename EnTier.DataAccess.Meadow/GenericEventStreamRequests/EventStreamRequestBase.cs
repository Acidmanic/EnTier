using EnTier.Repositories.Models;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Requests;

namespace EnTier.DataAccess.Meadow.GenericEventStreamRequests;

public abstract class EventStreamRequestBase<TEvent,TEventId,TStreamId,TIn>:MeadowRequest<TIn,ObjectEntry<TEventId,TStreamId>>
{


    
    public EventStreamRequestBase() : base(true)
    {
    }


    protected NameConvention NameConvention => Configuration.GetNameConvention<TEvent>();

    protected abstract string PickName(NameConvention nameConvention);
    
    public override string RequestText
    {
        get => PickName(NameConvention);
        protected set { }
    }
}