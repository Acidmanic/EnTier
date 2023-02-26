using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.NamingConventions;
using Acidmanic.Utilities.Reflection.Dynamics;
using Acidmanic.Utilities.Results;
using EnTier.EventSourcing.Models;

namespace EnTier.EventSourcing
{
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


        public Result<MethodProfile> FindProfile(string name)
        {
            foreach (var item in MethodProfiles)
            {
                if (item.Key == name.ToLower())
                {
                    return new Result<MethodProfile>(true, item.Value);
                }
            }

            return new Result<MethodProfile>().FailAndDefaultValue();
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
                Name = methodName
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