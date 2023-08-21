using System;

namespace EnTier.Query.Attributes
{
    public class FilterFieldAttribute:Attribute
    {
        public FilterFieldAttribute(ValueComparison method)
        {
            Method = method;
        }

        public FilterFieldAttribute():this(ValueComparison.Equal)
        {
            
        }
        
        public ValueComparison Method { get; private set; }
        
        
    }
}