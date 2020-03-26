using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    interface IComponentProvider
    {
        T Provide<T>( params object[] args);
    }
}
