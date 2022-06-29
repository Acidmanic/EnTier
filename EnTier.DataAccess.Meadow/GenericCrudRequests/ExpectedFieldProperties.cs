using System;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace EnTier.DataAccess.Meadow.GenericCrudRequests
{
    internal class ExpectedFieldProperties
    {
        public string Name { get; }

        public AccessNode Leaf { get; }

        public Type Type { get; }

        public bool Exists { get; }

        public ExpectedFieldProperties(AccessNode leaf)
        {
            Exists = leaf != null;

            if (leaf != null)
            {
                Name = leaf.Name;

                Type = leaf.Type;

                Leaf = leaf;
            }
        }
    }
}