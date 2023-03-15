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
            return ReadEvents(new Result<object>().FailAndDefaultValue(), 
                    0, int.MaxValue, true)
                .TotalCount;
        }

        public ResultPage ReadAll()
        {
            var events = ReadEvents(new Result<object>().FailAndDefaultValue(), 0, int.MaxValue);

            return events;
        }

        public ResultPage ReadAll(int from, int count)
        {
            var events = ReadEvents(new Result<object>().FailAndDefaultValue(), from, count);

            return events;
        }

        public ResultPage ReadAll(object streamId)
        {
            var events = ReadEvents(new Result<object>(true, streamId), 0, int.MaxValue);

            return events;
        }

        public ResultPage ReadAll(object streamId, int from, int count)
        {
            var events = ReadEvents(new Result<object>(true, streamId), from, count);

            return events;
        }


        private ResultPage ReadEvents(Result<object> streamId, int skip, int count, bool countOnly = false)
        {
            var resultPage = new ResultPage();

            var methodName = streamId.Success ? "EnumerateStreamChunks" : "EnumerateChunks";
            var parameterCount = streamId.Success ? 3 : 2;

            var skept = 0;

            EnumerateChunksAction(methodName, parameterCount,
                chunk =>
                {
                    resultPage.TotalCount += chunk.TotalCount;
                    
                    if (!countOnly)
                    {
                        foreach (var eventWrap in chunk.Events)
                        {
                            if (skept < skip)
                            {
                                skept++;
                            }
                            else if (resultPage.Events.Count < count)
                            {
                                resultPage.Events.Add(eventWrap);
                            }
                        }
                    }
                }, 256, streamId, countOnly
            );

            return resultPage;
        }


        private void EnumerateChunksAction(string methodName, int methodParametersCount,
            Action<ResultPage> onEventChunk, long chunkSize, Result<object> streamId, bool countOnly = false)
        {
            void Action(IEnumerable<StreamEvent<object, object, object>> chunk)
            {
                var pageChunk = new ResultPage();

                foreach (var streamEvent in chunk)
                {
                    pageChunk.TotalCount++;

                    if (!countOnly)
                    {
                        pageChunk.Events.Add(new EventWrap
                        {
                            Event = streamEvent.Event,
                            EventId = streamEvent.EventId,
                            StreamId = streamEvent.StreamId,
                            EventConcreteTypeName = streamEvent.EventConcreteType.Name
                        });
                    }
                }

                onEventChunk(pageChunk);
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