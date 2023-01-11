using System;
using System.Data.SqlTypes;

namespace EnTier.Prepopulation.Attributes
{
    
    [AttributeUsage(AttributeTargets.Class,
        AllowMultiple = true,
        Inherited = true)]
    public class DependsOnSeedAttribute:Attribute
    {
        public DependsOnSeedAttribute(Type dependeeType)
        {
            DependeeType = dependeeType;
        }

        public Type DependeeType { get; }
    }
}