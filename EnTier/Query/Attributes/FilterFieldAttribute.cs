using System;

namespace EnTier.Query.Attributes
{
    public class FilterFieldAttribute:Attribute
    {
        public FilterFieldAttribute(EvaluationMethods method)
        {
            Method = method;
        }

        public FilterFieldAttribute():this(EvaluationMethods.Equal)
        {
            
        }
        
        public EvaluationMethods Method { get; private set; }
        
        
    }
}