using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Context
{
    public class FileContext : IContext, IEnTierBuiltIn
    {

        private Dictionary<string, object> _data = new Dictionary<string, object>();

        private string path;

        public FileContext()
        {

            path = Path.Combine(Environment.CurrentDirectory, "SerializedDatabase.dat");

            Load();

        }

        private void Load()
        {
            var f = new FileInfo(path);
            if (f.Exists)
            {
                Stream s = f.Open(FileMode.Open);

                BinaryFormatter b = new BinaryFormatter();

                _data = (Dictionary<string, object>)b.Deserialize(s);

                s.Close();
            }
            
        }
        public virtual void Apply()
        {
            var f = new FileInfo(path);

            Stream s = f.Open(FileMode.OpenOrCreate);

            BinaryFormatter b = new BinaryFormatter();

            b.Serialize(s, _data);

            s.Close();
        }

        public virtual void Dispose()
        {
            // Disposed ofcourse!
        }

        public IDataset<T> GetDataset<T>() where T : class
        {
            var key = typeof(T).GUID.ToString();

            InFileDataset<T> dataset;

            if (!_data.ContainsKey(key))
            {
                dataset = new InFileDataset<T>();

                _data.Add(key, dataset);
            }
            else
            {
                dataset = (InFileDataset<T>)_data[key];
            }

            return dataset;

        }
    }
}
