using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace EnTier.Extensions;

public static class StringExpressionExtensions
{
    
        public static Expression<Func<TOwner,TMember>> CreatePropertyPickerLambdaHeadless<TOwner,TMember>(this string headlessAddress)
        {
            return CreatePropertyPickerLambda<TOwner,TMember>( typeof(TOwner).Name + "." + headlessAddress);
        }

        public static Expression<Func<TOwner,TMember>> CreatePropertyPickerLambda<TOwner,TMember>( string standardAddress)
        {
            var type = typeof(TOwner);
            
            var key = FieldKey.Parse(standardAddress);

            var length = key.Count;

            var ev = new ObjectEvaluator(type);

            ParameterExpression parameter = Expression.Parameter(type, "obj");

            Expression expression = parameter;

            for (int l = 2; l <= length; l++)
            {
                var currentKey = key.SubKey(0, l);

                var node = ev.Map.NodeByKey(currentKey);

                //a=>a.id
                expression = Expression.Property(expression, node.Name);
            }

            var lambda = Expression.Lambda<Func<TOwner,TMember>>(expression, parameter);

            return lambda;
        }
        
}