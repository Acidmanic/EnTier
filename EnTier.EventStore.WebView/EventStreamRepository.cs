using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Results;
using EnTier.Reflection;
using EnTier.Repositories.Models;
using EnTier.UnitOfWork;

namespace EnTier.EventStore.WebView
{
    internal class EventStreamRepository
    {
        private readonly object _repository;
        private readonly Type _eventType;
        private readonly Type _eventIdType;
        private readonly Type _streamIdType;
        private readonly Type _repositoryType;

        private EventStreamRepository(object repository, Type streamIdType, Type eventType, Type eventIdType)
        {
            _repository = repository;
            _streamIdType = streamIdType;
            _eventType = eventType;
            _eventIdType = eventIdType;
            _repositoryType = _repository.GetType();
        }


        public static EventStreamRepository Create(IUnitOfWork unitOfWork, EventStreamProfile profile)
        {
            var repository =
                unitOfWork.GetStreamRepository(profile.EventType, profile.EventIdType, profile.StreamIdType);

            return new EventStreamRepository(repository, profile.StreamIdType, profile.EventType, profile.EventIdType);
        }


        // public List<object> ReadStream()
        // {
        //     var method = _repositoryType.GetMethods().FirstOrDefault
        //         (m => m.Name == "ReadStream" && m.GetParameters().Length == 0);
        //
        //     var exe = new MethodExecute();
        //
        //     var result = exe.Execute(method, _repository, new object[] { });
        //
        //     if (result.Successful)
        //     {
        //         return List(result.ReturnValue);
        //     }
        //
        //     return new List<object>();
        // }
        //
        // public List<object> ReadStream(object streamId)
        // {
        //     var method = _repositoryType.GetMethods().FirstOrDefault
        //         (m => m.Name == "ReadStream" && m.GetParameters().Length == 1);
        //
        //     var exe = new MethodExecute();
        //
        //     var result = exe.Execute(method, _repository, new[] { streamId });
        //
        //     if (result.Successful)
        //     {
        //         return List(result.ReturnValue);
        //     }
        //
        //     return new List<object>();
        // }
        //
        // public List<object> ReadStreamChunk(object streamId, object baseEventId, long count)
        // {
        //     var method = _repositoryType.GetMethods().FirstOrDefault
        //         (m => m.Name == "ReadStreamChunk" && m.GetParameters().Length == 3);
        //
        //     var exe = new MethodExecute();
        //
        //     var result = exe.Execute(method, _repository,
        //         new[] { streamId, baseEventId, count });
        //
        //     if (result.Successful)
        //     {
        //         return List(result.ReturnValue);
        //     }
        //
        //     return new List<object>();
        // }

        //
        // private List<object> List(object value)
        // {
        //     var list = new List<object>();
        //
        //     if (value is IEnumerable enumerable)
        //     {
        //         foreach (var item in enumerable)
        //         {
        //             list.Add(item);
        //         }
        //     }
        //
        //     return list;
        // }

        private static Action<IEnumerable<StreamEvent<TEvent, TEventId, TStreamId>>> CastPickerAction<TEvent, TEventId,
            TStreamId>(
            Action<IEnumerable<StreamEvent<object, object, object>>> picker)
        {
            return chunk =>
            {
                var list = new List<StreamEvent<object, object, object>>();

                foreach (var streamEvent in chunk)
                {
                    list.Add(new StreamEvent<object, object, object>
                    {
                        Event = streamEvent.Event,
                        EventId = streamEvent.EventId,
                        StreamId = streamEvent.StreamId
                    });
                }

                picker(list);
            };
        }

        private object CreateGenericPickerAction(Action<IEnumerable<StreamEvent<object, object, object>>> picker)
        {
            var method = GetType().GetRuntimeMethods()
                .FirstOrDefault(m => m.Name == nameof(CastPickerAction));


            var genericMethod = method.MakeGenericMethod(_eventType, _eventIdType, _streamIdType);

            var action = genericMethod.Invoke(this, new object[] { picker });

            return action;
        }
        //
        // public List<EventWrap> EnumerateAll(long chunkSize = 50)
        // {
        //     var result = new List<EventWrap>();
        //
        //     EnumerateAction("EnumerateStreamChunks", 3,
        //         chunk => result.AddRange(chunk),
        //         chunkSize, new Result<object>().FailAndDefaultValue());
        //
        //     return result;
        // }
        //
        // public List<EventWrap> EnumerateStream(object streamId, object baseEventId, long chunkSize = 50)
        // {
        //     var result = new List<EventWrap>();
        //
        //     EnumerateAction("EnumerateStreamChunks", 3,
        //         chunk =>
        //         {
        //             foreach (var eventWrap in chunk)
        //             {
        //             }
        //
        //             result.AddRange(chunk)
        //         },
        //         chunkSize, new Result<object>(true, streamId));
        //
        //     return result;
        // }
        //
        //
        // public List<EventWrap> EnumerateStream(object streamId, long chunkSize = 50)
        // {
        //     var result = new List<EventWrap>();
        //
        //     EnumerateAction("EnumerateStreamChunks", 3,
        //         chunk => result.AddRange(chunk),
        //         chunkSize, new Result<object>(true, streamId));
        //
        //     return result;
        // }


        public EventCacheHeader Header()
        {
            var header = new EventCacheHeader(_eventType, _eventIdType, _streamIdType);
            header.Load();

            return header;
        }

        

        public long Count()
        {
            return CacheCheck().Count;
        }
        
        public IEnumerable<EventWrap> ReadAll()
        {
            CacheCheck();

            var cache = InstantiateEventCache();

            return cache.Events;
        }

        public IEnumerable<EventWrap> ReadAll(int from, int count)
        {
            CacheCheck();

            var cache = InstantiateEventCache();

            count = (int)Math.Min(count, cache.Count);

            return cache.Events.Skip(from).Take(count);
        }
        
        public IEnumerable<EventWrap> ReadAll(object streamId)
        {
            CacheCheck();

            var cache = InstantiateEventCache();

            return cache.Events.Where(e => e.StreamId.Equals(streamId));
        }

        public IEnumerable<EventWrap> ReadAll(object streamId, int from, int count)
        {
            CacheCheck();

            var cache = InstantiateEventCache();

            count = (int)Math.Min(count, cache.Count);

            return cache.Events.Where(e => streamId.Equals(e.StreamId)).Skip(from).Take(count);
        }
        
        private void EnumerateAction(string methodName, int methodParametersCount,
            Action<List<EventWrap>> onEventChunk, long chunkSize, Result<object> streamId)
        {
            void Action(IEnumerable<StreamEvent<object, object, object>> chunk)
            {
                var list = new List<EventWrap>();

                foreach (var streamEvent in chunk)
                {
                    list.Add(new EventWrap
                    {
                        Event = streamEvent.Event,
                        EventId = streamEvent.EventId,
                        StreamId = streamEvent.StreamId
                    });
                }

                onEventChunk(list);
            }

            var genericAction = CreateGenericPickerAction(Action);

            var method = _repositoryType.GetMethods().FirstOrDefault
                (m => m.Name == methodName && m.GetParameters().Length == methodParametersCount);

            var exe = new MethodExecute();

            var parameters = streamId.Success
                ? new[] { streamId.Value, genericAction, chunkSize }
                : new[] { genericAction, chunkSize };

            exe.Execute(method, _repository, parameters);
        }

        private EventCache InstantiateEventCache()
        {
            var cache = new EventCache(_eventType, _eventIdType, _streamIdType);
            
            cache.Load();
            
            return cache;
        }

        public EventCacheHeader CacheCheck()
        {
            var  header = new EventCacheHeader(_eventType, _eventIdType, _streamIdType);
            
            header.Load();
            
            var existingEventHasBeenPassed = header.LastEventId == null;

            var unSeenEvents = new List<EventWrap>();
            
            EnumerateAction("EnumerateChunks", 2, chunk =>
            {
                foreach (var item in chunk)
                {
                    var eventId = item.EventId;

                    if (existingEventHasBeenPassed)
                    {
                        unSeenEvents.Add(item);
                    }

                    existingEventHasBeenPassed = header.LastEventId == null ||
                                                 (eventId != null && eventId.Equals(header.LastEventId));
                }
            }, 256, new Result<object>().FailAndDefaultValue());

            if (unSeenEvents.Count > 0)
            {
                var cache = InstantiateEventCache();
                
                unSeenEvents.ForEach( e => cache.Add(e));
                
                cache.Save();

                header = new EventCacheHeader(_eventType, _eventIdType, _streamIdType);
                
                header.Load();
            }
            
            return header;
        }
    }
}