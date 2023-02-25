namespace EnTier.EventSourcing;

public interface IAggregateBuilder
{
    IAggregate<TAggregateRoot, TEvent, TStreamId> Build<TAggregateRoot, TEvent, TStreamId>();
}