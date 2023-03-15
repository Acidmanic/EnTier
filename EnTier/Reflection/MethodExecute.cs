using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Results;
using EnTier.Extensions;

namespace EnTier.Reflection
{
    public class MethodExecute
    {
        public MethodExecutionResult ExecuteStatic(Type type, string methodName, params object[] parameters)
        {
            var values = parameters.Where(p => p != null).ToArray();
            
            var types = values.Select(p => p.GetType()).ToArray();
            
            var method = type.GetMethods()
                .FirstOrDefault(m => m.IsStatic && MatchByParameterTypes(m, types));

            if (method != null)
            {
                var result = Execute(method, (object) null,values);

                return result;
            }

            return new MethodExecutionResult()
            {
                Exception = new Exception($"Method {methodName} has not been found in type: {type.Name}."),
                Successful = false,
                ReturnsValue = false,
                ReturnType = typeof(object),
                ReturnValue = null
            };
            
        }

        public MethodExecutionResult ExecuteStatic(Type type, string methodName,
            Dictionary<string, string> parameters)
        {
            var method = type.GetMethods()
                .FirstOrDefault(m => m.IsStatic && MatchByParameterNames(m, parameters.Keys));

            if (method != null)
            {
                var mergedData = new Dictionary<string, object>();

                PutDataInto(mergedData, parameters, method);

                var result = Execute(null, method, mergedData);

                return result;
            }

            return new MethodExecutionResult()
            {
                Exception = new Exception($"Method {methodName} has not been found in type: {type.Name}."),
                Successful = false,
                ReturnsValue = false,
                ReturnType = typeof(object),
                ReturnValue = null
            };
        }

        public MethodExecutionResult Execute(object owner, string methodName, Dictionary<string, string> parameters)
        {
            if (owner != null)
            {
                var type = owner.GetType();

                var method = type.GetMethods().FirstOrDefault(m => MatchByParameterNames(m, parameters.Keys));

                if (method != null)
                {
                    var mergedData = new Dictionary<string, object>();

                    PutDataInto(mergedData, parameters, method);

                    var result = Execute(owner, method, mergedData);

                    return result;
                }
            }

            return new MethodExecutionResult()
            {
                Exception = new Exception($"Method {methodName} has not been found in object: {owner}."),
                Successful = false,
                ReturnsValue = false,
                ReturnType = typeof(object),
                ReturnValue = null
            };
        }

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


        private MethodEvaluation GetMethodStatus(MethodInfo method)
        {
            var evaluation = new MethodEvaluation();

            var methodReturnType = method.ReturnType;

            evaluation.SyncReturnType = methodReturnType;

            evaluation.IsAsyncFunction = TypeCheck.IsSpecificOf(methodReturnType, typeof(Task<>));

            evaluation.IsAsyncVoid = !evaluation.IsAsyncFunction && Extends(typeof(Task), methodReturnType);

            evaluation.IsSyncVoid = methodReturnType == typeof(void);

            if (evaluation.IsAsyncFunction)
            {
                evaluation.AsyncReturnType = methodReturnType.GetGenericArguments()[0];

                evaluation.ResultProperty = methodReturnType.GetProperty("Result");
            }

            return evaluation;
        }

        private bool Extends(Type baseType, Type derived)
        {
            var b = derived;

            while (b!=null)
            {
                if (b == baseType)
                {
                    return true;
                }

                b = b.BaseType;
            }

            return false;
        }


        public MethodExecutionResult Execute(MethodInfo method, object owner, object[] parameterValues)
        {
            var evaluation = GetMethodStatus(method);

            try
            {
                object returnedValue = method.Invoke(owner, parameterValues);

                if (evaluation.IsAsync)
                {
                    if (evaluation.IsAsyncFunction)
                    {
                        returnedValue = evaluation.ResultProperty.GetValue(returnedValue);
                    }
                    else if (returnedValue is Task task)
                    {
                        task.Wait();

                        returnedValue = null;
                    }
                }

                return new MethodExecutionResult
                {
                    Exception = null,
                    Successful = true,
                    ReturnsValue = evaluation.IsFunction,
                    ReturnType = evaluation.OverAllReturnValue,
                    ReturnValue = returnedValue
                };
            }
            catch (Exception e)
            {
                return new MethodExecutionResult
                {
                    Exception = e.InnerMostException(),
                    Successful = false,
                    ReturnsValue = evaluation.IsFunction,
                    ReturnType = evaluation.OverAllReturnValue,
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

        private bool MatchByParameterNames(MethodInfo method, ICollection<string> parameterNames)
        {
            var parameters = method.GetParameters();

            if (parameters.Length == parameterNames.Count)
            {
                var names = parameterNames.Select(n => n.ToLower()).ToList();

                return parameters.All(p => names.Contains(p.Name.ToLower()));
            }

            return false;
        }

        private bool MatchByParameterTypes(MethodInfo method, ICollection<Type> parameterTypes)
        {
            var parameters = method.GetParameters();

            if (parameters.Length == parameterTypes.Count)
            {
                return parameters.All(p => parameterTypes.Any(t => p.ParameterType == t));
            }

            return false;
        }
    }
}