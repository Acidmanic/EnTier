using System;
using System.Collections.Generic;

namespace EnTier.Repositories.Attributes
{
    public class KeepAllPropertiesAttribute : DataInsertionAttribute
    {
        public KeepAllPropertiesAttribute(params Type[] types):base(types)
        {
            
        }
    }
}