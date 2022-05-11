using System;

namespace EnTier.Repositories.Attributes
{
    public class KeepPropertyAttribute:DataInsertionAttribute
    {
        public KeepPropertyAttribute(params Type[] types):base(types)
        {
            
        }
    }
}