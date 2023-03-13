using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Results;
using EnTier.Reflection;
using EnTier.Repositories.Models;
using EnTier.UnitOfWork;
using Newtonsoft.Json.Serialization;

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
                        StreamId = streamEvent.StreamId,
                        EventConcreteType = streamEvent.EventConcreteType
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

        public long Count()
        {
            return ReadAll().Count();
        }

        public IEnumerable<EventWrap> ReadAll()
        {
            var events = ReadEvents(new Result<object>().FailAndDefaultValue(), 0, int.MaxValue);

            return events;
        }

        public IEnumerable<EventWrap> ReadAll(int from, int count)
        {
            var events = ReadEvents(new Result<object>().FailAndDefaultValue(), from, count);

            return events;
        }

        public IEnumerable<EventWrap> ReadAll(object streamId)
        {
            var events = ReadEvents(new Result<object>(true, streamId), 0, int.MaxValue);

            return events;
        }

        public IEnumerable<EventWrap> ReadAll(object streamId, int from, int count)
        {
            var events = ReadEvents(new Result<object>(true, streamId), from, count);

            return events;
        }


        private List<EventWrap> ReadEvents(Result<object> streamId, int skip, int count)
        {
            var events = new List<EventWrap>();

            var methodName = streamId.Success ? "EnumerateStreamChunks" : "EnumerateChunks";
            var parameterCount = streamId.Success ? 3 : 2;

            var skept = 0;

            EnumerateAction(methodName, parameterCount,
                chunk =>
                {
                    foreach (var eventWrap in chunk)
                    {
                        if (skept < skip)
                        {
                            skept++;
                        }
                        else if (events.Count < count)
                        {
                            events.Add(eventWrap);
                        }
                    }
                }, 256, streamId
            );

            return events;
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
                        StreamId = streamEvent.StreamId,
                        EventConcreteTypeName = streamEvent.EventConcreteType.Name
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
    }
}