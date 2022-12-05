using System;

namespace EnTier.Exceptions
{
    public class EnTierEssenceIsNotRegisteredException:Exception
    {
        public EnTierEssenceIsNotRegisteredException()
            :base("Before configuring EnTier, you should first register EnTierEssence in your container.")
        { }
    }
}