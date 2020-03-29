using System;
using System.Collections.Generic;
using System.Text;

namespace EnTier.Context
{
    public interface IIDGenerator
    {

        long NewId<T>();

    }
}
