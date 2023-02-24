using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnTier.DataAccess.Meadow.GenericEventStreamRequests;
using EnTier.Repositories;
using EnTier.Repositories.Models;
using Meadow;
using Meadow.Contracts;
using Meadow.Requests;

namespace EnTier.DataAccess.Meadow
{
    public class MeadowEventStreamRepository<TEvent, TEventId, TStreamId> : EventStreamRepositoryBase<TEvent, TEventId, TStreamId>
    {
        
        private readonly IMeadowConfigurationProvider _configurationProvider;

        public MeadowEventStreamRepository(IMeadowConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        private MeadowEngine GetEngine()
        {
            return new MeadowEngine(_configurationProvider.GetConfigurations());
        }
        
        private async Task<IEnumerable<TOut>> ReadEntries<TIn, TOut>(MeadowRequest<TIn, TOut> request)
            where TOut : class, new()
        {
            var engine = GetEngine();

            var response = await engine.PerformRequestAsync(request);

            if (response.Failed)
            {
                throw response.FailureException;
            }

            return response.FromStorage;
        }
        
        protected override TEventId AppendEntry(ObjectEntry<TEventId, TStreamId> entry)
        {
            var engine = GetEngine();

            var request = new InsertEventRequest<TEvent, TEventId, TStreamId>(entry);

            var response = engine.PerformRequest(request);

            if (response.Failed)
            {
                throw response.FailureException;
            }

            if (response.FromStorage.Count == 0)
            {
                throw new Exception("Unable to append event.");
            }

            return response.FromStorage[0].EventId;
        }

        protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadAllEntries()
        {
            var request = new ReadAllStreamsRequest<TEvent, TEventId, TStreamId>();

            return ReadEntries(request);
        }

        protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntriesByStreamId(TStreamId streamId)
        {
            var request = new ReadStreamByStreamIdRequest<TEvent, TEventId, TStreamId>(streamId);

            return ReadEntries(request);
        }

        protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntryChunk(TStreamId streamId, TEventId baseEventId, long count)
        {
            var request = new ReadStreamChunkByStreamIdRequest<TEvent, TEventId, TStreamId>(streamId, baseEventId, count);

            return ReadEntries(request);
        }

        protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntryChunk(TEventId baseEventId, long count)
        {
            var request = new ReadAllStreamsChunksRequest<TEvent, TEventId, TStreamId>(baseEventId, count);

            return ReadEntries(request);
        }
    }
}