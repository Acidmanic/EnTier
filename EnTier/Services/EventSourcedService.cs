using System.Collections.Generic;
using System.Threading.Tasks;
using EnTier.EventSourcing;
using EnTier.Extensions;
using EnTier.Repositories;

namespace EnTier.Services;

public class EventSourcedService<TAggregateRoot, TEvent, TEventId, TStreamId>
{
    protected IAggregateBuilder AggregateBuilderBuilder { get; }
    protected IEventStreamRepository<TEvent, TEventId, TStreamId> EventStreamRepository { get; }

    public EventSourcedService(IAggregateBuilder aggregateBuilder,
        IEventStreamRepository<TEvent, TEventId, TStreamId> eventStreamRepository)
    {
        AggregateBuilderBuilder = aggregateBuilder;

        EventStreamRepository = eventStreamRepository;
    }

    public IAggregate<TAggregateRoot, TEvent, TStreamId> GetAggregate(TStreamId id)
    {
        return GetAggregateAsync(id).Result;
    }

    public async Task<IAggregate<TAggregateRoot, TEvent, TStreamId>> GetAggregateAsync(TStreamId id)
    {
        var aggregate = AggregateBuilderBuilder.Build<TAggregateRoot, TEvent, TStreamId>();

        var events = await EventStreamRepository.ReadStream(id);

        aggregate.Initialize(id, events);

        return aggregate;
    }

    public void Save(IAggregate<TAggregateRoot, TEvent, TStreamId> aggregate)
    {
        SaveAsync(aggregate).Wait();
    }

    public async Task SaveAsync(IAggregate<TAggregateRoot, TEvent, TStreamId> aggregate)
    {
        foreach (var @event in aggregate.Updates)
        {
            await EventStreamRepository.Append(@event, aggregate.StreamId);
        }
    }


    public List<IAggregate<TAggregateRoot, TEvent, TStreamId>> GetAll()
    {
        return GetAllAsync().Result;
    }

    public async Task<List<IAggregate<TAggregateRoot, TEvent, TStreamId>>> GetAllAsync()
    {
        var eventsByStreamId = await EventStreamRepository.ReadStreamsGrouped();

        var aggregates = new List<IAggregate<TAggregateRoot, TEvent, TStreamId>>();

        foreach (var item in eventsByStreamId)
        {
            var aggregate = AggregateBuilderBuilder.Build<TAggregateRoot, TEvent, TStreamId>();

            aggregate.Initialize(item.Key, item.Value);

            aggregates.Add(aggregate);
        }

        return aggregates;
    }
}