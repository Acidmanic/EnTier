using System;
using System.Collections.Generic;
using System.Reflection;
using EnTier.Extensions;

namespace EnTier.Reflection;

public class MethodExecute
{
    public MethodExecutionResult Execute(object owner, MethodInfo method,
        object dataByObject,
        params Dictionary<string, string>[] dataByDictionary)
    {
        var mergedData = new Dictionary<string, object>();

        PutDataInto(mergedData, dataByObject);

        foreach (var dictionary in dataByDictionary)
        {
            PutDataInto(mergedData, dictionary, method);
        }

        return Execute(owner, method, mergedData);
    }


    public MethodExecutionResult Execute(object owner, MethodInfo method, Dictionary<string, object> data)
    {
        var parameters = method.GetParameters();

        var parameterValues = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var name = parameters[i].Name;

            if (name != null)
            {
                if (data.ContainsKey(name))
                {
                    var value = data[name];

                    if (value != null && parameters[i].ParameterType.IsInstanceOfType(value))
                    {
                        parameterValues[i] = value;
                    }
                }
            }
        }

        var executionResult = Execute(method, owner, parameterValues);

        return executionResult;
    }

    public MethodExecutionResult Execute(MethodInfo method, object owner, object[] parameterValues)
    {
        var doesReturnValue = method.ReturnType == typeof(void);

        try
        {

            var returnValue = method.Invoke(owner, parameterValues);
            
            return new MethodExecutionResult
            {
                Exception = null,
                Successful = true,
                ReturnsValue = doesReturnValue,
                ReturnType = method.ReturnType,
                ReturnValue = returnValue
            };
        }
        catch (Exception e)
        {
            return new MethodExecutionResult
            {
                Exception = e.InnerMostException(),
                Successful = false,
                ReturnsValue = doesReturnValue,
                ReturnType = method.ReturnType,
                ReturnValue = null
            };
        }
    }

    public void PutDataInto(Dictionary<string, object> fieldsByName, Dictionary<string, string> dataStrings,
        MethodInfo targetMethod)
    {
        var parametersByName = GetParametersByName(targetMethod);

        foreach (var item in dataStrings)
        {
            var name = item.Key;

            if (parametersByName.ContainsKey(name))
            {
                var type = parametersByName[name].ParameterType;

                var value = ParseOrDefault(type, item.Value);

                MergeValueIn(fieldsByName, name, value);
            }
        }
    }


    public void PutDataInto(Dictionary<string, object> fieldsByName, object dataObject)
    {
        if (dataObject == null)
        {
            return;
        }

        var properties = dataObject.GetType().GetProperties();

        foreach (var property in properties)
        {
            var name = property.Name;

            var value = ReadOrDefault(dataObject, property);

            MergeValueIn(fieldsByName, name, value);
        }
    }

    public void MergeValueIn(Dictionary<string, object> fieldsByName, string name, object value)
    {
        if (fieldsByName.ContainsKey(name))
        {
            var existing = fieldsByName[name];

            if (existing == null)
            {
                fieldsByName.Remove(name);

                fieldsByName.Add(name, value);
            }
        }
        else
        {
            fieldsByName.Add(name, value);
        }
    }

    private object ReadOrDefault(object dataObject, PropertyInfo property)
    {
        try
        {
            return property.GetValue(dataObject);
        }
        catch (Exception e)
        {
        }

        return null;
    }

    private object ParseOrDefault(Type type, string stringValue)
    {
        if (type == typeof(string))
        {
            return stringValue;
        }

        var parseMethod = type.GetMethod("Parse", new[] { typeof(string) });

        if (parseMethod != null)
        {
            try
            {
                var value = parseMethod.Invoke(null, new object[] { stringValue });

                return value;
            }
            finally
            {
            }
        }


        return null;
    }


    private Dictionary<string, PropertyInfo> GetPropertiesByName(Type type)
    {
        var propertiesByName = new Dictionary<string, PropertyInfo>();

        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            propertiesByName.Add(property.Name, property);
        }

        return propertiesByName;
    }

    private Dictionary<string, ParameterInfo> GetParametersByName(MethodInfo method)
    {
        var parameters = method.GetParameters();

        var parametersByName = new Dictionary<string, ParameterInfo>();

        foreach (var parameter in parameters)
        {
            if (parameter.Name != null)
            {
                parametersByName.Add(parameter.Name, parameter);
            }
        }

        return parametersByName;
    }
}