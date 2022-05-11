using System;
using System.Collections.Generic;

namespace EnTier.Repositories.Attributes
{
    
    
    public class DataInsertionAttribute : Attribute
    {
        public DataInsertionAttribute(params Type[] types)
        {
            foreach (var type in types)
            {
                this.TypesList.Add(type);
            }
        }

        public List<Type> TypesList { get; set; } = new List<Type>();
    }
}