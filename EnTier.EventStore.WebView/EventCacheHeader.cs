using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace EnTier.EventStore.WebView
{
    internal class EventCacheHeader
    {
        public Type StreamIdType { get; protected set; }

        public Type EventIdType { get;protected set; }

        public Type EventType { get; protected set;}

        public long Count { get; set; }

        public object LastEventId { get; set; }
        
        protected readonly string FilePath;

        public EventCacheHeader(Type eventType, Type eventIdType, Type streamIdType)
        {
            EventType = eventType;
            EventIdType = eventIdType;
            StreamIdType = streamIdType;


            var assemblyDirectory = Assembly.GetEntryAssembly()?.Location;

            if (assemblyDirectory != null)
            {
                assemblyDirectory = new FileInfo(assemblyDirectory).Directory?.FullName;
            }
            
            var filePath = assemblyDirectory ?? ".";

            filePath = new DirectoryInfo(filePath).FullName;

            FilePath = Path.Combine(filePath, eventType.Name + ".event.cache.json");
        }
        
        public virtual void Load()
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);

                try
                {
                    var cache = JsonConvert.DeserializeObject<EventCacheHeader>(json);

                    if (cache != null)
                    {
                        StreamIdType = cache.StreamIdType;
                        EventIdType = cache.EventIdType;
                        EventType = cache.EventType;
                        Count = cache.Count;
                        LastEventId = cache.LastEventId;

                    }
                }
                catch (Exception _)
                {
                }
            }
        }

        public void Delete()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}