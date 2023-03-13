using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Results;
using EnTier.Reflection;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;

namespace EnTier.EventStore.WebView
{
    internal class EventCache : EventCacheHeader
    {
        public List<EventWrap> Events { get; private set; }

        public void Add(EventWrap entry)
        {
            LastEventId = entry.EventId;

            Events.Add(entry);

            Count = Events.Count;
        }

        public void Save()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }

            var json = JsonConvert.SerializeObject(this);

            File.WriteAllText(FilePath, json);
        }


        public override void Load()
        {
            Events.Clear();

            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);

                try
                {
                    var cache = JsonConvert.DeserializeObject<EventCache>(json);

                    if (cache != null)
                    {
                        Events.AddRange(cache.Events.Select(Cast));

                        Count = Events.Count;
                    }
                }
                catch (Exception)
                {
                    // ignore
                }
            }
        }

        private EventWrap Cast(EventWrap e)
        {
            return new EventWrap
            {
                Event = ParseOrCastTo(e.Event, EventType) ?? e.Event,
                EventId = ParseOrCastTo(e.EventId, EventIdType) ?? e.EventId,
                StreamId = ParseOrCastTo(e.StreamId, StreamIdType) ?? e.StreamId,
                EventConcreteTypeName = e.EventConcreteTypeName
            };
        }

        public object ParseOrCastTo(object value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }

            if (value.GetType() == targetType)
            {
                return value;
            }

            if (value is string stringValue)
            {
                var parsed = Parse(stringValue, targetType);

                if (parsed)
                {
                    return parsed.Value;
                }
            }

            try
            {
                return value.CastTo(targetType);
            }
            catch (Exception)
            {
                // ignore
            }

            return null;
        }

        public Result<object> Parse(string value, Type targetType)
        {
            if (targetType == typeof(string))
            {
                return new Result<object>(true, value);
            }

            var execute = new MethodExecute();

            var result = execute.ExecuteStatic(targetType, "Parse", value);

            if (result.Successful && result.ReturnsValue && result.ReturnType == targetType)
            {
                return new Result<object>(true, result.ReturnValue);
            }

            result = execute.ExecuteStatic(targetType, "FromString", value);

            if (result.Successful && result.ReturnsValue && result.ReturnType == targetType)
            {
                return new Result<object>(true, result.ReturnValue);
            }

            return new Result<object>().FailAndDefaultValue();
        }

        public EventCache(Type eventType, Type eventIdType, Type streamIdType) : base(eventType, eventIdType,
            streamIdType)
        {
            Events = new List<EventWrap>();
        }
    }
}