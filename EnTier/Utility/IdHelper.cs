using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace EnTier.Utility
{
    public class IdHelper
    {
        public static AccessNode GetIdNode<TEntity, TId>()
        {
            var accessNode = ObjectStructure.CreateStructure<TEntity>(false);

            var idType = typeof(TId);

            var idLeaf = accessNode
                .EnumerateLeavesBelow()
                .FirstOrDefault(leaf => leaf.IsAutoValued && leaf.Type == idType);

            return idLeaf;
        }
    }
}