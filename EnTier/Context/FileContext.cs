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
    public class FileContext : IContext, IEnTierBuiltIn
    {





        private string dbDirectory;

        private object _filesLock = new object();

        private JsonDatabase _database = new JsonDatabase();

        public FileContext()
        {

            dbDirectory = Path.Combine(Environment.CurrentDirectory, "SerializedDatabase");

            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }


            Load();

        }

        private void Load()
        {
            lock (_filesLock)
            {
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

            return new InFileDataset<T>(datalist);

        }
    }
}
