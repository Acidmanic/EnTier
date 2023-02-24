using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using EnTier.Repositories;
using EnTier.Repositories.Models;
using EnTier.Utility;

namespace EnTier.DataAccess.InMemory;

public class InMemoryEventStreamRepository<TEvent, TEventId, TStreamId> 
     : EventStreamRepositoryBase<TEvent, TEventId, TStreamId>
{


     private static readonly List<ObjectEntry<TEventId, TStreamId>> Entries =  new List<ObjectEntry<TEventId, TStreamId>>();

     private static readonly IdGenerator<TEventId> IdGenerator = new IdGenerator<TEventId>();
     
     
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

     protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntriesByStreamId(TStreamId streamId)
     {
          return Task.FromResult(Entries
             .Where(se => streamId.Equals(se.StreamId)));
     }

     private bool IsGreaterThan(TEventId greater, TEventId smaller)
     {
         if (greater == null)
         {
             return false;
         }

         if (smaller == null)
         {
             return true;
         }

         var type = greater.GetType();
         
         if (TypeCheck.IsReferenceType(type))
         {
             return false;
         }

         if (greater is string sGreater && smaller is string sSmaller)
         {
             return string.CompareOrdinal(sGreater, sSmaller) > 0;
         }
         
         if (greater is char cGreater && smaller is char cSmaller)
         {
             return cGreater > cSmaller;
         }

         if (TypeCheck.IsNumerical(type))
         {
             var gNumber = greater.AsNumber();
             var sNumber = smaller.AsNumber();

             return gNumber > sNumber;
         }

         return false;
     }
     
     protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntryChunk(TStreamId streamId, TEventId baseEventId, long count)
     {
          return Task.FromResult(Entries
             .Where(se => streamId.Equals(se.StreamId))
             .Where(se => IsGreaterThan(se.EventId , baseEventId)));
     }

     protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntryChunk(TEventId baseEventId, long count)
     {
         return Task.FromResult(Entries
             .Where(se => IsGreaterThan(se.EventId , baseEventId)));
     }
}