using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using EnTier.Exceptions;
using EnTier.Repositories;
using EnTier.Repositories.Models;
using EnTier.Utility;
using Newtonsoft.Json;

namespace EnTier.DataAccess.JsonFile
{

    public class
        JsonFileEventStreamRepository<TEvent, TEventId, TStreamId> : EventStreamRepositoryBase<TEvent, TEventId,
            TStreamId>
    {
        private readonly string _currentStreamDirectoryPath;

        private readonly UniqueIdGenerator<TEventId> _idGenerator;

        private readonly string _baseDirectory;
        
        public JsonFileEventStreamRepository(string baseDirectory)
        {
            if (!TypeCheck.IsNumerical<TEventId>())
            {
                throw new InvalidEventIdTypeException<TEventId>
                    ($"{typeof(TEventId).FullName} is not valid for Json Event Stream. " +
                     $"Please Use a numeric type like long, int, etc.");
            }
            
            _baseDirectory = baseDirectory;
            
            _currentStreamDirectoryPath = GetCurrentStreamDirectoryPath();

            _idGenerator = new UniqueIdGenerator<TEventId>(_currentStreamDirectoryPath);
        }

        public JsonFileEventStreamRepository(Action<TEvent, TEventId, TStreamId> eventPublisher, string baseDirectory) : base(eventPublisher)
        {
            
            if (!TypeCheck.IsNumerical<TEventId>())
            {
                throw new InvalidEventIdTypeException<TEventId>
                ($"{typeof(TEventId).FullName} is not valid for Json Event Stream. " +
                 $"Please Use a numeric type like long, int, etc.");
            }
            
            _baseDirectory = baseDirectory;
            
            _currentStreamDirectoryPath = GetCurrentStreamDirectoryPath();

            _idGenerator = new UniqueIdGenerator<TEventId>(_currentStreamDirectoryPath);
        }

        public JsonFileEventStreamRepository(EnTierEssence essence, string baseDirectory) : base(essence)
        {
            if (!TypeCheck.IsNumerical<TEventId>())
            {
                throw new InvalidEventIdTypeException<TEventId>
                ($"{typeof(TEventId).FullName} is not valid for Json Event Stream. " +
                 $"Please Use a numeric type like long, int, etc.");
            }
            
            _baseDirectory = baseDirectory;
            
            _currentStreamDirectoryPath = GetCurrentStreamDirectoryPath();

            _idGenerator = new UniqueIdGenerator<TEventId>(_currentStreamDirectoryPath);
        }

        private string GetCurrentStreamDirectoryPath()
        {
            var eventStreamsDatabaseDirectoryPath =
                Path.Join(_baseDirectory, "EventStreams");

            if (!Directory.Exists(eventStreamsDatabaseDirectoryPath))
            {
                Directory.CreateDirectory(eventStreamsDatabaseDirectoryPath);
            }

            var name = GetNameKey();

            var path = Path.Join(eventStreamsDatabaseDirectoryPath, name);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }


        private string GetStreamFilePath(TStreamId streamId)
        {
            var path = Path.Join(_currentStreamDirectoryPath, streamId.ToString());

            return path;
        }


        private string GetNameKey()
        {
            return typeof(TEvent).Name + "." + typeof(TEventId).Name + "." + typeof(TStreamId).Name;
        }

        protected override TEventId AppendEntry(ObjectEntry<TEventId, TStreamId> entry)
        {
            var id = _idGenerator.Generate();

            var filePath = GetStreamFilePath(entry.StreamId);

            var entries = ReadStream(filePath);

            entry.EventId = id;

            entries.Add(entry);

            SaveStream(filePath, entries);

            return id;
        }

        private void SaveStream(string filePath, List<ObjectEntry<TEventId, TStreamId>> entries)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var json = JsonConvert.SerializeObject(entries);

            File.WriteAllText(filePath, json);
        }


        private List<ObjectEntry<TEventId, TStreamId>> ReadStream(string filePath,bool botherToSort = true)
        {
            List<ObjectEntry<TEventId, TStreamId>> list = null;

            if (File.Exists(filePath))
            {
                try
                {
                    var json = File.ReadAllText(filePath);

                    list = JsonConvert.DeserializeObject<List<ObjectEntry<TEventId, TStreamId>>>(json);
                }
                catch (Exception e)
                {
                }
            }

            if (list == null)
            {
                list = new List<ObjectEntry<TEventId, TStreamId>>();
            }

            if (botherToSort)
            {
                list.Sort(new ObjectEntryComparer());   
            }

            return list;
        }

        private class ObjectEntryComparer : Comparer<ObjectEntry<TEventId, TStreamId>>
        {
            public override int Compare(ObjectEntry<TEventId, TStreamId> x, ObjectEntry<TEventId, TStreamId> y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                if (y == null)
                {
                    return 1;
                }
                var numX = x.EventId.AsNumber();
                var numY = y.EventId.AsNumber();
                
                if (Math.Abs(numX - numY) < 0.00000000000000000000000000000000000000001)
                {
                    return 0;
                }

                return numX < numY ? -1 : 1;
            }
        }

        private List<ObjectEntry<TEventId, TStreamId>> ReadAllEntriesSync()
        {
            var files = new DirectoryInfo(_currentStreamDirectoryPath).GetFiles();

            var entries = new List<ObjectEntry<TEventId, TStreamId>>();

            foreach (var file in files)
            {
                var stream = ReadStream(file.FullName,false);

                if (stream != null)
                {
                    entries.AddRange(stream);
                }
            }
            
            entries.Sort(new ObjectEntryComparer());
            
            return entries;
        }

        protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadAllEntries()
        {
            var entries = ReadAllEntriesSync();

            return Task.FromResult<IEnumerable<ObjectEntry<TEventId, TStreamId>>>(entries);
        }

        protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntriesByStreamId(TStreamId streamId)
        {
            var streamFile = GetStreamFilePath(streamId);

            var entries = ReadStream(streamFile);

            return Task.FromResult<IEnumerable<ObjectEntry<TEventId, TStreamId>>>(entries);
        }

        protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntryChunk(TStreamId streamId,
            TEventId baseEventId, long count)
        {
            var streamFile = GetStreamFilePath(streamId);

            var entries = ReadStream(streamFile)
                .Where(e => ObjectOperations.IsGreaterThan(e.EventId, baseEventId))
                .Take((int)count);

            return Task.FromResult<IEnumerable<ObjectEntry<TEventId, TStreamId>>>(entries);
        }

        protected override Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntryChunk(TEventId baseEventId,
            long count)
        {
            var entries = ReadAllEntriesSync()
                .Where(e => ObjectOperations.IsGreaterThan(e.EventId, baseEventId))
                .Take((int)count);

            return Task.FromResult<IEnumerable<ObjectEntry<TEventId, TStreamId>>>(entries);
        }
    }
}