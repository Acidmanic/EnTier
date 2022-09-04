using System;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace EnTier.DataAccess.Meadow.GenericCrudRequests
{
    internal class ExpectedFieldProperties
    {
        public string Name { get; }

        public AccessNode Leaf { get; }

        public Type Type { get; }

        public bool Exists { get; }
        
        public FieldKey Key { get; }

        public ExpectedFieldProperties(AccessNode leaf, FieldKey key)
        {
            Exists = leaf != null;

            if (leaf != null)
            {
                Name = leaf.Name;

                Type = leaf.Type;

                Leaf = leaf;

                Key = key;
            }
        }
    }
}