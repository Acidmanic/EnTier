


using System;

namespace EnTier.Annotations
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