using System;
using System.Collections.Generic;

namespace EnTier.Query
{
    public class FilterItem
    {
        private static readonly string[] OutOfHashStrings = { ":" };
        public string Key { get; set; }

        public string Maximum { get; set; }

        public string Minimum { get; set; }

        public Type ValueType { get; set; } = typeof(string);

        public List<string> EqualValues { get; set; } = new List<string>();

        public EvaluationMethods EvaluationMethod { get; set; }

        internal string ToColumnSeparatedString()
        {
            var hash = ClearForHash(Key) + ":";

            hash += ((int)EvaluationMethod).ToString() + ":";
            hash += ValueType.FullName + ":";
            hash += ClearForHash(Maximum) + ":";
            hash += ClearForHash(Minimum) + ":";

            var eq = "";
            var sep = "";

            foreach (var value in EqualValues)
            {
                eq += sep + ClearForHash(value);
                sep = ":";
            }

            hash += eq;

            return hash;
        }

        private string ClearForHash(string value)
        {
            value = value?.ToLower() ?? "";

            return value;
        }
    }
}