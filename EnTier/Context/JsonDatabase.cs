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
            var ret = new string[_index.Count + 1];

            ret[0] = JsonConvert.SerializeObject(_index);

            for (int i = 0; i < _index.Count; i++)
            {
                ret[i + 1] = JsonConvert.SerializeObject(_data[_index[i]]);
            }

            return ret;
        }

        public void FromJsons(string[] jsons)
        {
            _index = JsonConvert.DeserializeObject<List<Type>>(jsons[0]);

            for (int i = 0; i < _index.Count; i++)
            {
                var dataset = JsonConvert.DeserializeObject(jsons[i + 1], _index[i]);

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

            File.WriteAllText(indexFile, jsons[0]);

            for (int i = 1; i < jsons.Length; i++)
            {
                var datasetPath = Path.Combine(directory, i + ".json");

                File.WriteAllText(datasetPath, jsons[i]);
            }

        }


        public void Load(string directory)
        {
            var indexFile = Path.Combine(directory, "index.json");
            
            if (File.Exists(indexFile))
            {

                var files = Directory.EnumerateFiles(directory, "*.json", SearchOption.TopDirectoryOnly).ToList();

                var jsons = new string[files.Count + 1];

                jsons[0] = File.ReadAllText(indexFile);

                _data.Clear();

                _index.Clear();

                for (int i = 0; i < files.Count; i++)
                {
                    jsons[i + 1] = File.ReadAllText(files[i]);
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
