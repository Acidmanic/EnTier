using System;

namespace EnTier.Exceptions
{
    public class InvalidConstructorException:Exception
    {

        public InvalidConstructorException():base("Your custom repository " +
                                                  "must confirm with constructor limitations " +
                                                  "regarding the UnitOfWork your using.")
        { }
        
        
    }
}