using System;

namespace EnTier.Repositories.Attributes
{
    public class StripPropertyAttribute : DataInsertionAttribute
    {
        public StripPropertyAttribute(params Type[] types):base(types)
        {
            
        }
    }
}