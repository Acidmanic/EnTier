


using System;
using System.Collections.Generic;

namespace Generics{



    public interface IGenericBuilder<T>
    {
        
        object Build<TStorage,TDomain,Tid>() where TStorage:class;
    }
}