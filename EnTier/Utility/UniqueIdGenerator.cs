using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace EnTier.Utility
{
    public class UniqueIdGenerator<TId>
    {
        private readonly string _filePath;
        private readonly object _accessLock = new object();

        public UniqueIdGenerator(string directoryPath)
        {
            _filePath = Path.Join(directoryPath, "last.id");
        }

        public UniqueIdGenerator() : this(SpecialPaths.GetExecutionDirectory())
        {
        }


        public TId Generate()
        {
            lock (_accessLock)
            {
                var existings = ReadExistingIds();

                var generator = new IdGenerator<TId>(existings);

                var id = generator.New();

                UpdateLastId(id);

                return id;
            }
        }

        public bool IsTaken(TId id)
        {
            lock (_accessLock)
            {
                if (id == null)
                {
                    return false;
                }

                var existingIds = ReadExistingIds();

                return existingIds.Any(i => id.Equals(i));
            }
        }

        private void UpdateLastId(TId id)
        {
            var ids = ReadExistingIds();

            ids.Add(id);

            WriteIds(ids);
        }

        private void WriteIds(List<TId> ids)
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            var json = JsonConvert.SerializeObject(ids);

            File.WriteAllText(_filePath, json);
        }

        private List<TId> ReadExistingIds()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);

                try
                {
                    var ids = JsonConvert.DeserializeObject<List<TId>>(json);

                    return ids;
                }
                catch (Exception e)
                {
                }
            }

            return new List<TId>();
        }
    }
}