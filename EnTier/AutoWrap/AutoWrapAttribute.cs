using System;

namespace EnTier.AutoWrap
{
   
    public class AutoWrapAttribute:Attribute
    {
        public bool AutoName { get;  }
        
        public string Name { get; }


        public AutoWrapAttribute()
        {
            AutoName = true;
            Name = null;
        }

        public AutoWrapAttribute(string name)
        {
            AutoName = false;

            Name = name;
        }
        
    }
}