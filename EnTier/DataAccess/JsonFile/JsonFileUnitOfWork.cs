using System;
using System.Collections.Generic;
using System.IO;
using EnTier.Repositories;
using EnTier.UnitOfWork;
using Newtonsoft.Json;

namespace EnTier.DataAccess.JsonFile
{
    public class JsonFileUnitOfWork : IUnitOfWork
    {
        private Dictionary<string, object> _data = new Dictionary<string, object>();
        private readonly string _dataFile;

        public JsonFileUnitOfWork()
        {
            _dataFile = "SerializedDatabase.json";

            Load(_dataFile);
        }

        public ICrudRepository<TStorage, TId> GetCrudRepository<TStorage, TId>() where TStorage : class, new()
        {
            string key = Key<TStorage, TId>();

            if (!_data.ContainsKey(key))
            {
                _data.Add(key, new List<TStorage>());
            }

            var list = (List<TStorage>) _data[key];

            return new JsonFileRepository<TStorage, TId>(list);
        }

        public void Complete()
        {
            Save(_dataFile);
        }

        private string Key<TStorage, TId>()
        {
            string key = typeof(TStorage).FullName + "_i_" + typeof(TId).FullName;

            key = key.Replace(".", "_");

            return key;
        }

        private void Load(string dataFile)
        {
            try
            {
                string json = File.ReadAllText(dataFile);

                _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void Save(string dataFile)
        {
            try
            {
                if (File.Exists(dataFile))
                {
                    File.Delete(dataFile);
                }

                var content = JsonConvert.SerializeObject(_data);

                File.WriteAllText(dataFile, content);
            }
            catch (Exception)
            {
                //ignored
            }
        }
    }
}