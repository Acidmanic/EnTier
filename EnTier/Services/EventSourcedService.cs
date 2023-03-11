using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acidmanic.Utilities.Results;
using EnTier.EventSourcing;
using EnTier.Extensions;
using EnTier.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace EnTier.Services
{
    public class EventSourcedService<TAggregateRoot, TEvent, TStreamId, TEventId>
    {
        protected IAggregateBuilder AggregateBuilder { get; }

        protected ILogger Logger { get; private set; }


        protected IUnitOfWork UnitOfWork { get; private set; }

        public EventSourcedService(EnTierEssence essence)
        {
            UnitOfWork = essence.UnitOfWork;

            Logger = essence.Logger;

            AggregateBuilder = essence.AggregateBuilder;
        }

        public Result<IAggregate<TAggregateRoot, TEvent, TStreamId>> GetAggregate(TStreamId id)
        {
            return GetAggregateAsync(id).Result;
        }


        public IAggregate<TAggregateRoot, TEvent, TStreamId> CreateAggregateInstance()
        {
            var aggregate = AggregateBuilder.Build<TAggregateRoot, TEvent, TStreamId>();

            return aggregate;
        }


        public async Task<Result<IAggregate<TAggregateRoot, TEvent, TStreamId>>> GetAggregateAsync(TStreamId id)
        {
            var aggregate = AggregateBuilder.Build<TAggregateRoot, TEvent, TStreamId>();

            var repository = UnitOfWork.GetStreamRepository<TEvent, TEventId, TStreamId>();

            var events = await repository.ReadStream(id);

            var eventsArray = events as TEvent[] ?? events.ToArray();

            if (eventsArray.Length == 0)
            {
                return new Result<IAggregate<TAggregateRoot, TEvent, TStreamId>>().FailAndDefaultValue();
            }

            aggregate.Initialize(id, eventsArray);

            UnitOfWork.Complete();

            return new Result<IAggregate<TAggregateRoot, TEvent, TStreamId>>(true, aggregate);
        }

        public void Save(IAggregate<TAggregateRoot, TEvent, TStreamId> aggregate)
        {
            SaveAsync(aggregate).Wait();
        }

        public async Task SaveAsync(IAggregate<TAggregateRoot, TEvent, TStreamId> aggregate)
        {
            var repository = UnitOfWork.GetStreamRepository<TEvent, TEventId, TStreamId>();

            foreach (var @event in aggregate.Updates)
            {
                await repository.Append(@event, aggregate.StreamId);
            }

            UnitOfWork.Complete();
        }


        public List<IAggregate<TAggregateRoot, TEvent, TStreamId>> GetAll()
        {
            return GetAllAsync().Result;
        }

        public async Task<List<IAggregate<TAggregateRoot, TEvent, TStreamId>>> GetAllAsync()
        {
            var repository = UnitOfWork.GetStreamRepository<TEvent, TEventId, TStreamId>();

            var eventsByStreamId = await repository.ReadStreamsGrouped();

            var aggregates = new List<IAggregate<TAggregateRoot, TEvent, TStreamId>>();

            foreach (var item in eventsByStreamId)
            {
                var aggregate = AggregateBuilder.Build<TAggregateRoot, TEvent, TStreamId>();

                aggregate.Initialize(item.Key, item.Value);

                aggregates.Add(aggregate);
            }

            UnitOfWork.Complete();

            return aggregates;
        }
    }
}