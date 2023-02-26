using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Acidmanic.Utilities.NamingConventions;
using Acidmanic.Utilities.Reflection.Dynamics;
using Acidmanic.Utilities.Results;
using EnTier.EventSourcing.Attributes;
using EnTier.EventSourcing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace EnTier.EventSourcing
{
    /// <summary>
    /// This class profiles all methods in an aggregate class, and provides indexing for these methods so they can be
    /// dynamically listed and invoked.
    /// </summary>
    /// <typeparam name="TAggregate">Type of aggregate class to be indexed</typeparam>
    public class AggregateIndex<TAggregate> : AggregateIndex
    {
        public AggregateIndex() : base(typeof(TAggregate))
        {
        }
    }

    public class AggregateIndex
    {
        public Dictionary<string, MethodProfile> MethodProfiles { get; } = new Dictionary<string, MethodProfile>();

        public Type StreamIdType { get; }

        public Result<ConventionDescriptor> ModerateMethodNames { get; set; } =
            new Result<ConventionDescriptor>(true, ConventionDescriptor.Standard.Kebab);


        public AggregateIndex(Type type)
        {
            var methods = type.GetMethods()
                .Where(m => !m.IsAbstract && m.IsPublic
                                          && !m.IsStatic && !m.IsGenericMethodDefinition
                                          && !m.IsSpecialName && m.DeclaringType == type);

            foreach (var method in methods)
            {
                var profile = CreateProfile(method);

                MethodProfiles.Add(profile.Name.ToLower(), profile);
            }

            var getStreamId = type.GetProperty("StreamId");

            if (getStreamId == null)
            {
                throw new Exception("Invalid Aggregate Type");
            }

            StreamIdType = getStreamId.PropertyType;
        }


        public Result<MethodProfile> FindProfile(string name,bool byStreamId)
        {
            foreach (var item in MethodProfiles)
            {
                if (item.Key == name.ToLower() && item.Value.NeedsStreamId==byStreamId)
                {
                    return new Result<MethodProfile>(true, item.Value);
                }
            }

            return new Result<MethodProfile>().FailAndDefaultValue();
        }


        private bool IsHidden(MethodInfo method)
        {
            return method.GetCustomAttribute<ApiInvisibleAttribute>() != null;
        }


        private HttpMethod GetHttpMethod(MethodInfo method)
        {
            var httpMethodAttribute = method.GetCustomAttribute<HttpMethodAttribute>();

            if (httpMethodAttribute is HttpGetAttribute)
            {
                return HttpMethod.Get;
            }

            if (httpMethodAttribute is HttpDeleteAttribute)
            {
                return HttpMethod.Delete;
            }

            if (httpMethodAttribute is HttpPutAttribute)
            {
                return HttpMethod.Put;
            }

            return HttpMethod.Post;
        }


        private MethodProfile CreateProfile(MethodInfo method)
        {
            var methodName = method.Name;

            if (ModerateMethodNames)
            {
                var namingConvention = new NamingConvention();

                methodName = namingConvention.Convert(methodName, ModerateMethodNames);
            }

            var profile = new MethodProfile
            {
                Method = method,
                Name = methodName,
                HttpMethod = GetHttpMethod(method),
                NeedsStreamId = method.GetCustomAttribute<NoStreamIdApi>() == null
            };

            var parameters = method.GetParameters();

            var typeBuilder = new ModelBuilder(method.Name + "Parameters");

            foreach (var parameter in parameters)
            {
                typeBuilder.AddProperty(parameter.Name, parameter.ParameterType);
            }

            profile.ModelType = typeBuilder.Build();

            return profile;
        }
    }
}