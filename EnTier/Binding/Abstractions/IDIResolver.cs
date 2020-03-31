



using System;

namespace EnTier.Binding.Abstractions
{



    public interface IDIResolver
    {

        T Resolve<T>();

        object Resolve(Type serviceType);
        
    }


    
}