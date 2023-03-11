using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnTier.Repositories;
using EnTier.Repositories.Models;
using EnTier.Utility;

namespace EnTier.DataAccess.InMemory
{

     public class InMemoryEventStreamRepository<TEvent, TEventId, TStreamId>
          : EventStreamRepositoryBase<TEvent, TEventId, TStreamId>
     {


          private static readonly List<ObjectEntry<TEventId, TStreamId>> Entries =
               new List<ObjectEntry<TEventId, TStreamId>>();

          private static readonly IdGenerator<TEventId> IdGenerator = new IdGenerator<TEventId>();

          public InMemoryEventStreamRepository()
          {
          }

          public InMemoryEventStreamRepository(Action<TEvent, TEventId, TStreamId> eventPublisher) : base(
               eventPublisher)
          {
          }

          public InMemoryEventStreamRepository(EnTierEssence essence) : base(essence)
          {
          }


          protected override TEventId AppendEntry(ObjectEntry<TEventId, TStreamId> entry)
          {
               var id = IdGenerator.New();

               entry.EventId = id;

               Entries.Add(entry);

               return id;
          }

          protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadAllEntries()
          {
               return Task.FromResult<IEnumerable<ObjectEntry<TEventId, TStreamId>>>(Entries);
          }

          protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntriesByStreamId(
               TStreamId streamId)
          {
               return Task.FromResult(Entries
                    .Where(se => streamId.Equals(se.StreamId)));
          }



          protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntryChunk(TStreamId streamId,
               TEventId baseEventId, long count)
          {
               return Task.FromResult(Entries
                    .Where(se => streamId.Equals(se.StreamId))
                    .Where(se => ObjectOperations.IsGreaterThan(se.EventId, baseEventId)));
          }

          protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntryChunk(TEventId baseEventId,
               long count)
          {
               return Task.FromResult(Entries
                    .Where(se => ObjectOperations.IsGreaterThan(se.EventId, baseEventId)));
          }
     }
}