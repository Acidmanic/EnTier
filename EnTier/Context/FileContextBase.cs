using Components;
using Newtonsoft.Json;
using Plugging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Context
{
    public abstract class FileContextBase : IContext
    {

        private class JsonIdGenerator : IIDGenerator
        {

            private JsonDatabase _db;

            public JsonIdGenerator(JsonDatabase db)
            {
                _db = db;
            }
            public long NewId<T>()
            {
                return _db.GenerateId<T>();
            }
        }

        private string dbDirectory;

        private object _filesLock = new object();

        private JsonDatabase _database = new JsonDatabase();


        public FileContextBase(string dbDirectory)
        {
            Load(dbDirectory);
        }

        public FileContextBase()
        {

            var dbDirectory = Path.Combine(Environment.CurrentDirectory, "SerializedDatabase");

            Load(dbDirectory);

        }

        private void Load(string dbDirectory)
        {
            lock (_filesLock)
            {

                if (!Directory.Exists(dbDirectory))
                {
                    Directory.CreateDirectory(dbDirectory);
                }

                _database.Load(dbDirectory);
            }
        }

  

        public virtual void Apply()
        {
            lock (_filesLock)
            {
                _database.Save(dbDirectory);
            }
        }

        public virtual void Dispose()
        {
            // Disposed ofcourse!
        }

        public IDataset<T> GetDataset<T>() where T : class
        {
            var datalist = _database.Table<T>();

            return new JsonDataset<T>(datalist, new JsonIdGenerator(_database));

        }
    }
}
