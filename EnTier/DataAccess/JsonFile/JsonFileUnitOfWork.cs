﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EnTier.Repositories;
using EnTier.UnitOfWork;
using Newtonsoft.Json;

namespace EnTier.DataAccess.JsonFile
{
    public class JsonFileUnitOfWork:IUnitOfWork
    {

        private List<Type> _index = new List<Type>();

        private readonly Dictionary<Type, object> _data = new Dictionary<Type, object>();

        private Dictionary<string, long> _ids = new Dictionary<string, long>();

        private readonly string _dataDirectory;

        private const int IndexIndex = 0;

        private const int IndexId = 1;

        private const int IndexFirstTable = 2;


        public long GenerateId<T>()
        {
            var key = typeof(T).FullName;

            if (!_ids.ContainsKey(key))
            {
                _ids.Add(key, 0);
            }

            _ids[key] += 1;

            return _ids[key];
        }

        public JsonFileUnitOfWork()
        {
            var executionDirectory = new FileInfo(Assembly.GetEntryAssembly()?.Location ?? "").Directory?.FullName ?? "";

            _dataDirectory = Path.Combine(executionDirectory, "JsonDatabase");
            
            Load(_dataDirectory);
        }
        
        private List<T> Table<T>()
        {
            var type = typeof(List<T>);

            List<T> table;

            if (!_data.ContainsKey(type))
            {
                table = new List<T>();

                _data.Add(type, table);

                UpdateIndex();
            }
            else
            {
                table = (List<T>)_data[type];
            }

            return table;
        }

        private void UpdateIndex()
        {
            _index = new List<Type>();

            _index.AddRange(_data.Keys);
        }


        private string[] ToJson()
        {
            var ret = new string[_index.Count + IndexFirstTable];

            ret[IndexIndex] = JsonConvert.SerializeObject(_index);

            ret[IndexId] = JsonConvert.SerializeObject(_ids);

            for (int i = 0; i < _index.Count; i++)
            {
                ret[i + IndexFirstTable] = JsonConvert.SerializeObject(_data[_index[i]]);
            }

            return ret;
        }

        private void FromJsons(string[] jsons)
        {
            _index = JsonConvert.DeserializeObject<List<Type>>(jsons[IndexIndex]);

            _ids = JsonConvert.DeserializeObject<Dictionary<string,long>>(jsons[IndexId]);

            for (int i = 0; i < _index.Count; i++)
            {
                var dataset = JsonConvert.DeserializeObject(jsons[i + IndexFirstTable], _index[i]);

                _data.Add(_index[i], dataset);
            }

        }

        private void Save(string directory)
        {

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Clear(directory);

            var jsons = ToJson();

            var indexFile = Path.Combine(directory, "index.json");

            File.WriteAllText(indexFile, jsons[IndexIndex]);

            var idFile = Path.Combine(directory, "id.json");

            File.WriteAllText(idFile, jsons[IndexId]);

            for (int i = IndexFirstTable; i < jsons.Length; i++)
            {
                var datasetPath = GetFilepath(i, directory);

                File.WriteAllText(datasetPath, jsons[i]);
            }

        }

        private string GetFilepath(int index,string directory)
        {
            return Path.Combine(directory, index - IndexFirstTable + ".json");
        }

        private void Load(string directory)
        {
            var indexFile = Path.Combine(directory, "index.json");

            var idFile = Path.Combine(directory, "id.json");

            if (File.Exists(indexFile) && File.Exists(idFile))
            {

                var jsons = new string[Directory.EnumerateFiles(directory,"*.json",SearchOption.TopDirectoryOnly).Count()];

                jsons[IndexIndex] = File.ReadAllText(indexFile);

                jsons[IndexId] = File.ReadAllText(idFile);

                _data.Clear();

                _index.Clear();

                _ids.Clear();

                for (int i = IndexFirstTable; i < jsons.Length; i++)
                {
                    var filename = GetFilepath(i, directory);

                    jsons[i] = File.ReadAllText(filename);
                }

                FromJsons(jsons);
            }
            
        }


        private void Clear(string directory)
        {
            var files = Directory.EnumerateFiles(directory);

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception) { }
            }
        }

        public ICrudRepository<TStorage, TId> GetCrudRepository<TStorage, TId>() where TStorage : class, new()
        {
            var table = Table<TStorage>();
            
            return new JsonFileRepository<TStorage, TId>(table);
        }

        public void Complete()
        {
            Save(_dataDirectory);
        }
    }

}
