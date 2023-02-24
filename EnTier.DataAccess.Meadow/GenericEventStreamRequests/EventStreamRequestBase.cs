using EnTier.Repositories.Models;
using Meadow.Contracts;
using Meadow.Requests;

namespace EnTier.DataAccess.Meadow.GenericEventStreamRequests;

public abstract class EventStreamRequestBase<TEvent,TEventId,TStreamId,TIn>:MeadowRequest<TIn,ObjectEntry<TEventId,TStreamId>>
{

    protected NameConvention NameConvention { get; }= new NameConvention<TEvent>();
    
    public EventStreamRequestBase() : base(true)
    {
    }

    protected abstract string PickName(NameConvention nameConvention);
    
    public override string RequestText
    {
        get => PickName(NameConvention);
        protected set { }
    }
}