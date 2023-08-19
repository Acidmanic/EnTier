using System.Collections.Generic;
using System.Text;
using EnTier.Extensions;

namespace EnTier.Query
{
    public class SearchQuery
    {
        private readonly Dictionary<string, EvaluationItem> _itemsByKey = new Dictionary<string, EvaluationItem>();

        public void Add(EvaluationItem item)
        {
            var key = item.Key?.ToLower();
            
            _itemsByKey.Add(key!,item);
        }

        public void Clear()
        {
            _itemsByKey.Clear();
        }

        public EvaluationItem this[string key]
        {
            get
            {
                key = key?.ToLower();

                return _itemsByKey[key!];
            }
        }

        public List<EvaluationItem> Items()
        {
            var list = new List<EvaluationItem>();
            
            list.AddRange(_itemsByKey.Values);

            return list;
        }

        public List<string> NormalizedKeys()
        {
            var list = new List<string>();
            
            list.AddRange(_itemsByKey.Keys);

            return list;
        }

        public string Hash()
        {
            var sb = new StringBuilder();

            var sep = "";

            var keys = NormalizedKeys();
            
            keys.Sort();

            foreach (var key in keys)
            {
                var item = _itemsByKey[key];
                
                sb.Append(sep).Append(item.ToColumnSeparatedString());
                sep = ":";
            }

            return sb.ToString().ComputeMd5();
        }
    }
}