using System;
using System.Collections.Generic;
using System.Text;

namespace Context
{
    public interface IIDGenerator
    {

        long NewId<T>();

    }
}
