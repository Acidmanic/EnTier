


using System;

namespace EnTier.Components
{


    public class InjectionEntry:Attribute{

        public Type InjectionType {get; private set;}
        public InjectionEntry()
        {
            InjectionType = null;
        }
        public InjectionEntry(Type type)
        {
            InjectionType = type;
        }
    }
}