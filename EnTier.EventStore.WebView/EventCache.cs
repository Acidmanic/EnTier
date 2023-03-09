using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
                        Events.AddRange(cache.Events);

                        Count = Events.Count;
                    }
                }
                catch (Exception _)
                {
                }
            }
        }

        public EventCache(Type eventType, Type eventIdType, Type streamIdType) : base(eventType, eventIdType,
            streamIdType)
        {
            Events = new List<EventWrap>();
        }
    }
}