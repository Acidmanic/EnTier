using System;
using System.Collections.Generic;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Results;
using EnTier.DataAccess.InMemory;
using EnTier.Repositories;
using EnTier.Services;
using EnTier.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace EnTier.Fixture
{
    internal class FixtureExecuter
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EnTierEssence _essence;
        public ILogger Logger { get; private set; }

        public FixtureExecuter(EnTierEssence essence)
        {
            _essence = essence;

            Logger = essence.Logger;

            var unitOfWork = essence.UnitOfWork;

            if (unitOfWork == null)
            {
                unitOfWork = new InMemoryUnitOfWork(essence);
            }

            _unitOfWork = unitOfWork;
        }

        public void Execute<TFixture>()
        {
            var fixtureType = typeof(TFixture);

            if (fixtureType.IsAbstract || fixtureType.IsInterface)
            {
                return;
            }

            var fixture = CreateFixtureObject(fixtureType);

            var setupMethods = ExtractFixtureSetupMethods(fixtureType);

            setupMethods.ForEach(method => ExecuteSetup(fixture, method));
        }

        private object CreateFixtureObject(Type fixtureType)
        {
            var constructors = fixtureType.GetConstructors();

            ConstructorInfo constructorToUse = null;
            var maxArguments = -1;

            foreach (var constructor in constructors)
            {
                if (constructor.IsPublic)
                {
                    var numberOfParameters = constructor.GetParameters().Length;

                    if (numberOfParameters > maxArguments)
                    {
                        constructorToUse = constructor;

                        maxArguments = numberOfParameters;
                    }
                }
            }

            // ReSharper disable once PossibleNullReferenceException
            var parameters = constructorToUse.GetParameters();

            var arguments = new object[maxArguments];

            for (int i = 0; i < maxArguments; i++)
            {
                arguments[i] = _essence.Resolver.Resolve(parameters[i].ParameterType);
            }

            return constructorToUse.Invoke(arguments);
        }

        private void ExecuteSetup(object fixture, MethodInfo method)
        {
            var parameters = method.GetParameters();

            var arguments = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                arguments[i] = CreateParameterValue(parameters[i]);
            }

            method.Invoke(fixture, arguments);

            _unitOfWork.Complete();
        }

        private object CreateParameterValue(ParameterInfo parameter)
        {
            var unitOfWorkType = _unitOfWork.GetType();

            var parameterType = parameter.ParameterType;

            if (parameterType.IsGenericType)
            {
                var parameterGenericType = parameterType.GetGenericTypeDefinition();

                Type storageType, idType;

                if (parameterGenericType == typeof(ICrudService<,>))
                {
                    storageType = parameterType.GetGenericArguments()[0];
                    
                    idType = parameterType.GetGenericArguments()[1];

                    var serviceType = typeof(CrudService<,,,>);

                    var serviceSpecificType = serviceType.MakeGenericType(storageType, storageType, idType, idType);

                    var constructor = serviceSpecificType.GetConstructor(new Type[] { typeof(EnTierEssence) });

                    var serviceInstance = constructor?.Invoke(new object[] { _essence });

                    return serviceInstance;
                }

                if (parameterGenericType == typeof(ICrudRepository<,>))
                {
                    storageType = parameterType.GetGenericArguments()[0];

                    idType = parameterType.GetGenericArguments()[1];

                    var createRepositoryMethod = GetBuiltinCrudRepositoryCreatorMethod(unitOfWorkType);

                    if (createRepositoryMethod != null)
                    {
                        var genericMethod = createRepositoryMethod.MakeGenericMethod(storageType, idType);

                        return genericMethod.Invoke(_unitOfWork, new object[] { });
                    }
                }
            }
            else
            {
                if (parameterType == typeof(IUnitOfWork))
                {
                    return _unitOfWork;
                }

                if (parameterType == typeof(ILogger))
                {
                    return _essence.Logger;
                }
            }

            return null;
        }

        private MethodInfo GetBuiltinCrudRepositoryCreatorMethod(Type unitOfWorkType)
        {
            var allMethods = unitOfWorkType.GetMethods();

            var methodName = nameof(IUnitOfWork.GetCrudRepository);

            foreach (var method in allMethods)
            {
                if (method.Name.Equals(methodName))
                {
                    if (method.ReturnType.GenericTypeArguments.Length == 2)
                    {
                        return method;
                    }
                }
            }

            return null;
        }

        private List<MethodInfo> ExtractFixtureSetupMethods(Type fixtureType)
        {
            var fixtureSetupMethods = new List<MethodInfo>();

            if (fixtureType.IsAbstract || fixtureType.IsInterface)
            {
                return fixtureSetupMethods;
            }

            var methods = fixtureType.GetMethods();

            foreach (var method in methods)
            {
                if (IsFixtureSetupMethod(method))
                {
                    fixtureSetupMethods.Add(method);
                }
            }

            return fixtureSetupMethods;
        }

        private bool IsFixtureSetupMethod(MethodInfo method)
        {
            if (method.Name.Equals("Setup") && !method.IsStatic && !method.IsPrivate)
            {
                var parameters = method.GetParameters();

                foreach (var parameter in parameters)
                {
                    if (!IsInjectable(parameter.ParameterType))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }


        private bool IsInjectable(Type type)
        {
            var injectableTypes = new Type[]
            {
                typeof(ICrudRepository<,>),
                typeof(ICrudService<,>)
            };

            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                foreach (var injectableType in injectableTypes)
                {
                    if (injectableType == genericType)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return type == typeof(IUnitOfWork) || type==typeof(ILogger);
            }

            return false;
        }
    }
}