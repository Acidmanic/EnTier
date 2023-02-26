using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Acidmanic.Utilities.Results;
using EnTier.Reflection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EnTier.Extensions;

public static class HttpRequestExtensions
{



    public static Result<string> ReadLastPathSegment(this HttpRequest request)
    {
        var requestPath = request.Path;

        if (requestPath.HasValue)
        {
            if (requestPath.HasValue)
            {
                var path = requestPath.ToString();

                var terminalSegment = path.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

                if (!string.IsNullOrWhiteSpace(terminalSegment))
                {
                    return new Result<string>(true, terminalSegment);
                }
            }
        }

        return new Result<string>().FailAndDefaultValue();
    }

    public static async Task<string> ReadContentsAsString(this HttpRequest request)
    {
        var stream = request.Body;
        
        var reader = new StreamReader(stream);

        var content = await reader.ReadToEndAsync();

        return content;
    }
    
    public static Dictionary<string,string> GetUrlParameters(this HttpRequest request)
    {
        var parameters = new Dictionary<string, string>();

        foreach (var item in request.Query)
        {
            var key = item.Key;

            var value = item.Value.LastOrDefault();

            if (value != null)
            {
                if (parameters.ContainsKey(key))
                {
                    parameters.Remove(key);
                }
                parameters.Add(key,value);
            }
        }

        return parameters;
    }
    
    public static Dictionary<string,string> GetFormParameters(this HttpRequest request)
    {
        var parameters = new Dictionary<string, string>();

        foreach (var item in request.Form)
        {
            var key = item.Key;

            var value = item.Value.LastOrDefault();

            if (value != null)
            {
                if (parameters.ContainsKey(key))
                {
                    parameters.Remove(key);
                }
                parameters.Add(key,value);
            }
        }

        return parameters;
    }

    public static async Task<T> ReadJsonPayload<T>(this HttpRequest request) where T : class
    {
        return await request.ReadJsonPayload(typeof(T)) as T;
    }
    
    public static async Task<object> ReadJsonPayload(this HttpRequest request,Type type = null)
    {
        var json = await request.ReadContentsAsString();

        if (!string.IsNullOrWhiteSpace(json))
        {
            return DeserializeJsonOrDefault(json, type);
        }

        return null;
    }

    private static object DeserializeJsonOrDefault(string json, Type type = null)
    {
        try
        {
            if (type == null)
            {
                return JsonConvert.DeserializeObject(json);
            }
            else
            {
                return JsonConvert.DeserializeObject(json, type);
            }
        }
        catch (Exception _)
        {
        }

        return null;
    }
    
    public static async Task<MethodExecutionResult> ExecuteMethod(this HttpRequest request,object owner, MethodInfo method,Type payloadType = null)
    {
        var additionalData = new List<Dictionary<string, string>>();
        
        if (request.HasFormContentType)
        {
            var formData = request.GetFormParameters();
            
            additionalData.Add(formData);
        }

        if (request.Query.Count > 0)
        {
            var queryData = request.GetUrlParameters();
            
            additionalData.Add(queryData);
        }

        object payloadObject = null;
        
        if (request.ContentType == "application/json")
        {
            payloadObject = await request.ReadJsonPayload(payloadType);
        }

        var execute = new MethodExecute();

        var executionResult = execute.Execute(owner, method, payloadObject, additionalData.ToArray());

        return executionResult;
    }
    
    
}