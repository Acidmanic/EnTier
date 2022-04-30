using System;

namespace EnTier.Exceptions
{
    public class UnAcceptableModelException:Exception
    {
        public UnAcceptableModelException():base("Received model is rejected during data access regulation.")
        {
        }
    }
}