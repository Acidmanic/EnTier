using System;
using System.Collections.Generic;
using System.Reflection;
using EnTier.DataAccess.InMemory;
using EnTier.Repositories;
using EnTier.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace EnTier.Fixture
{
    internal class FixtureExecuter
    {
        private readonly IFixtureResolver _fixtureResolver;
        private readonly IUnitOfWork _unitOfWork;

        public FixtureExecuter(IFixtureResolver fixtureResolver)
        {
            _fixtureResolver = fixtureResolver;
            
            IUnitOfWork unitOfWork;
            try
            {
                unitOfWork = _fixtureResolver.Resolve<IUnitOfWork>();
            }
            catch (Exception e)
            {
                unitOfWork = null;
            }

            if (unitOfWork == null)
            {
                unitOfWork = new InMemoryUnitOfWork();
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
                arguments[i] = _fixtureResolver.Resolve(parameters[i].ParameterType);
            }

            return constructorToUse.Invoke(arguments);
        }

        private void ExecuteSetup(object fixture, MethodInfo method)
        {
            var parameters = method.GetParameters();

            var arguments = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                arguments[i] = CreateRepositoryFor(parameters[i]);
            }

            method.Invoke(fixture, arguments);

            _unitOfWork.Complete();
        }

        private object CreateRepositoryFor(ParameterInfo parameter)
        {
            var unitOfWorkType = _unitOfWork.GetType();

            var parameterType = parameter.ParameterType;

            var storageType = parameterType.GetGenericArguments()[0];
            var idType = parameterType.GetGenericArguments()[1];

            var createRepositoryMethod = unitOfWorkType.GetMethod(nameof(IUnitOfWork.GetCrudRepository));

            if (createRepositoryMethod != null)
            {
                var genericMethod = createRepositoryMethod.MakeGenericMethod(storageType, idType);

                return genericMethod.Invoke(_unitOfWork, new object[] { });
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
            var repoType = typeof(ICrudRepository<,>);

            if (method.Name.Equals("Setup") && !method.IsStatic && !method.IsPrivate)
            {
                var parameters = method.GetParameters();

                foreach (var parameter in parameters)
                {
                    if (parameter.ParameterType.GetGenericTypeDefinition() != repoType)
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }
    }
}