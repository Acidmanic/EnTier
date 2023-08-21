using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Results;

namespace EnTier.DataAccess.EntityFramework;

public static class FilterQueryExpressionExtensions
{
    public static IEnumerable<Expression<Func<TStorage, bool>>> ToExpression<TStorage>
        (this FilterQuery query)
    {
        var expressions = new List<Expression<Func<TStorage, bool>>>();

        foreach (var filterItem in query.Items())
        {
            var madeExpression = filterItem.ToExpression<TStorage>();

            if (madeExpression)
            {
                expressions.Add(madeExpression.Value);    
            }
        }
        return expressions;
    }
    
    // Based On the Code from stackoverflow: https://stackoverflow.com/a/62192157
    public static Result<Expression<Func<TStorage, bool>>> ToExpression<TStorage>(this FilterItem filter)
    {
        // smaller than
        
        var storageType = typeof(TStorage);

        var foundProperty = FindPropertyByKey(storageType, filter.Key);

        // a =>
        ParameterExpression parameter = Expression.Parameter(storageType, "a");
        
        if (foundProperty)
        {
            
            // a => a.<PropertyName>
            MemberExpression property = Expression.Property(parameter, foundProperty.Value);
            
            // a => boolean expression
            BinaryExpression body = CreateBinaryExpression(property, filter);
            
            Expression<Func<TStorage, bool>> expression = Expression.Lambda<Func<TStorage, bool>>(body, parameter);

            return new Result<Expression<Func<TStorage, bool>>>(true,expression);
        }

        return new Result<Expression<Func<TStorage, bool>>>().FailAndDefaultValue();
    }


    private static BinaryExpression CreateBinaryExpression(MemberExpression leftOperand, FilterItem filterItem)
    {
        switch (filterItem.ValueComparison)
        {
            case ValueComparison.SmallerThan:

                var maximum = filterItem.ReadValue(f => f.Maximum);
                
                return Expression.LessThanOrEqual(leftOperand, Expression.Constant(maximum));
            case ValueComparison.LargerThan:
                
                var minimum = filterItem.ReadValue(f => f.Minimum);
                
                return Expression.GreaterThanOrEqual(leftOperand, Expression.Constant(minimum));
            
            case ValueComparison.BetweenValues:
                var max = filterItem.ReadValue(f => f.Maximum);
                var min = filterItem.ReadValue(f => f.Minimum);

                return Expression.Add(
                    Expression.GreaterThanOrEqual(leftOperand,Expression.Constant(min)),
                    Expression.LessThanOrEqual(leftOperand,Expression.Constant(max)));
            case ValueComparison.Equal:
                // False Binary Expression
                BinaryExpression exp = Expression.Equal(Expression.Constant(false),Expression.Constant(true));

                var values = filterItem.ReadEqualValues();

                foreach (var value in values)
                {
                    exp = Expression.Or(exp, Expression.Equal(leftOperand, Expression.Constant(value)));
                }

                return exp;
        }
        return Expression.Equal(Expression.Constant(false),Expression.Constant(true));
    }

    public static object ReadValue(this FilterItem filter, Func<FilterItem, string> selector)
    {
        var stringValue = selector(filter);

        return ReadAsValue(stringValue, filter.ValueType);
    } 
    
    public static List<object> ReadEqualValues(this FilterItem filter)
    {
        var objectsRead = new List<object>();

        foreach (var stringValue in filter.EqualValues)
        {
            var value = ReadAsValue(stringValue, filter.ValueType);
            
            objectsRead.Add(value);
        }

        return objectsRead;
    }
    

    private static object ReadAsValue(string stringValue, Type type)
    {
        if (type == typeof(string))
        {
            return stringValue;
        }
        if (type == typeof(int))
        {
            return int.Parse(stringValue);
        }
        if (type == typeof(long))
        {
            return long.Parse(stringValue);
        }
        if (type == typeof(double))
        {
            return double.Parse(stringValue);
        }
        if (type == typeof(short))
        {
            return short.Parse(stringValue);
        }
        if (type == typeof(byte))
        {
            return byte.Parse(stringValue);
        }
        if (type == typeof(sbyte))
        {
            return sbyte.Parse(stringValue);
        }
        if (type == typeof(float))
        {
            return float.Parse(stringValue);
        }
        if (type == typeof(ulong))
        {
            return ulong.Parse(stringValue);
        }
        if (type == typeof(uint))
        {
            return uint.Parse(stringValue);
        }
        if (type == typeof(uint))
        {
            return uint.Parse(stringValue);
        }
        if (type == typeof(bool))
        {
            return bool.Parse(stringValue);
        }
        if (type == typeof(char))
        {
            return char.Parse(stringValue);
        }
        if (type == typeof(decimal))
        {
            return decimal.Parse(stringValue);
        }
        return stringValue;
    }
    
    private static Result<PropertyInfo> FindPropertyByKey(Type type, string key)
    {
        var normalizedKey = key.ToLower();

        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            if (property.Name.ToLower() == normalizedKey)
            {
                return new Result<PropertyInfo>(true, property);
            }
        }

        return new Result<PropertyInfo>().FailAndDefaultValue();
    }
}