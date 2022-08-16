using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;

namespace EnTier.DataAccess.EntityFramework.Extensions
{
    public static class ObjectExtensions
    {
        public static void CopyInto(this object me, object target)
        {
            
            var myEvaluator = new ObjectEvaluator(me);
            var tarEvaluator = new ObjectEvaluator(target);


            var myData = myEvaluator.ToStandardFlatData();
            
            tarEvaluator.LoadStandardData(myData);
        }
        
        
        public static void CopyInto(this object me, object target,params string[] excludedAddresses)
        {
            
            var myEvaluator = new ObjectEvaluator(me);
            var tarEvaluator = new ObjectEvaluator(target);


            var myData = myEvaluator.ToStandardFlatData();

            var filtered = myData.Where(dp => !excludedAddresses.Contains(dp.Identifier));
            
            var srcData = new Record(filtered);
            
            
            tarEvaluator.LoadStandardData(srcData);
        }
    }
}