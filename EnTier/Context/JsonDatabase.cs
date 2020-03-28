using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Context
{
    internal class JsonDatabase
    {

        private List<Type> _index = new List<Type>();

        private Dictionary<Type, object> _data = new Dictionary<Type, object>();

        private Dictionary<string, long> _ids = new Dictionary<string, long>();


        private const int INDEX_INDEX = 0;

        private const int INDEX_ID = 1;

        private const int INDEX_FIRST_TABLE = 2;


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

        public List<T> Table<T>()
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


        public string[] ToJson()
        {
            var ret = new string[_index.Count + INDEX_FIRST_TABLE];

            ret[INDEX_INDEX] = JsonConvert.SerializeObject(_index);

            ret[INDEX_ID] = JsonConvert.SerializeObject(_ids);

            for (int i = 0; i < _index.Count; i++)
            {
                ret[i + INDEX_FIRST_TABLE] = JsonConvert.SerializeObject(_data[_index[i]]);
            }

            return ret;
        }

        public void FromJsons(string[] jsons)
        {
            _index = JsonConvert.DeserializeObject<List<Type>>(jsons[INDEX_INDEX]);

            _ids = JsonConvert.DeserializeObject<Dictionary<string,long>>(jsons[INDEX_ID]);

            for (int i = 0; i < _index.Count; i++)
            {
                var dataset = JsonConvert.DeserializeObject(jsons[i + INDEX_FIRST_TABLE], _index[i]);

                _data.Add(_index[i], dataset);
            }

        }

        public void Save(string directory)
        {

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Clear(directory);

            var jsons = ToJson();

            var indexFile = Path.Combine(directory, "index.json");

            File.WriteAllText(indexFile, jsons[INDEX_INDEX]);

            var idFile = Path.Combine(directory, "id.json");

            File.WriteAllText(idFile, jsons[INDEX_ID]);

            for (int i = INDEX_FIRST_TABLE; i < jsons.Length; i++)
            {
                var datasetPath = GetFilepath(i, directory);

                File.WriteAllText(datasetPath, jsons[i]);
            }

        }

        private string GetFilepath(int index,string directory)
        {
            return Path.Combine(directory, index - INDEX_FIRST_TABLE + ".json");
        }

        public void Load(string directory)
        {
            var indexFile = Path.Combine(directory, "index.json");

            var idFile = Path.Combine(directory, "id.json");

            if (File.Exists(indexFile) && File.Exists(idFile))
            {

                var jsons = new string[Directory.EnumerateFiles(directory,"*.json",SearchOption.TopDirectoryOnly).Count()];

                jsons[INDEX_INDEX] = File.ReadAllText(indexFile);

                jsons[INDEX_ID] = File.ReadAllText(idFile);

                _data.Clear();

                _index.Clear();

                _ids.Clear();

                for (int i = INDEX_FIRST_TABLE; i < jsons.Length; i++)
                {
                    var filename = GetFilepath(i, directory);

                    jsons[i] = File.ReadAllText(filename);
                }

                FromJsons(jsons);
            }
            
        }

        public static JsonDatabase FromDirectory(string directory)
        {
            var ret = new JsonDatabase();

            ret.Load(directory);

            return ret;
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
    }

}
