using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    class TypeBundle
    {
        public Type Storage { get; set; }

        public Type Domain { get; set; }

        public Type Transfer { get; set; }


        public TypeBundle()
        {
                
        }


        public TypeBundle(Type all)
        {
            Storage = all;

            Domain = all;

            Transfer = all;
        }

        public TypeBundle(Type storage,Type domain,Type transfer)
        {
            Storage = storage;

            Domain = domain;

            Transfer = transfer;
        }
    }
}
