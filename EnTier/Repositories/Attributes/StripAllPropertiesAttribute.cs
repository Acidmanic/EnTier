using System;

namespace EnTier.Repositories.Attributes
{
    public class StripAllPropertiesAttribute:DataInsertionAttribute
    {
        public StripAllPropertiesAttribute(params Type[] types):base(types)
        {
            
        }
    }
}