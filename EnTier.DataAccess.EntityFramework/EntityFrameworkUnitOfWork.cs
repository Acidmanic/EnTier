using System;
using System.Linq;
using System.Reflection;
using EnTier.Repositories;
using EnTier.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework
{
    public class EntityFrameworkUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly Func<Type, object> GetDbSetByType;

        public EntityFrameworkUnitOfWork(DbContext context)
        {
            _context = context;


            var contextType = typeof(DbContext);

            var getDbSetMethod = contextType.GetMethod(nameof(DbContext.Set), new Type[] { });

            GetDbSetByType = entityType =>
            {
                var method = getDbSetMethod?.MakeGenericMethod(new Type[] { entityType});

                return method?.Invoke(_context, new object[] { });
            };
        }


        public ICrudRepository<TStorage, TId> GetCrudRepository<TStorage, TId>() where TStorage : class, new()
        {
            var dbSet = _context.Set<TStorage>();

            return new EntityFrameWorkCrudRepository<TStorage, TId>(dbSet);
        }


        public TCustomCrudRepository GetCrudRepository<TStorage, TId, TCustomCrudRepository>()
            where TStorage : class, new()
            where TCustomCrudRepository : ICrudRepository<TStorage, TId>
        {
            var repoType = UnitOfWorkRepositoryConfigurations.GetInstance().GetRepositoryType<TCustomCrudRepository>();

            if (repoType == null)
            {
                throw new Exception(
                    "You should register your custom repository in your startup class, using applicationBuilder.");
            }

            var repoConstructor = GetConstructor(repoType);

            if (repoConstructor == null)
            {
                throw new Exception("Your repository type should have" +
                                    " a constructor with (0 to any number " +
                                    "of) DbSet<TDalModel> arguments.");
            }

            object[] arguments = ProvideDbSetArgs(repoConstructor);

            var repository = repoConstructor.Invoke(arguments);

            return (TCustomCrudRepository) repository;
        }

        private object[] ProvideDbSetArgs(ConstructorInfo repoConstructor)
        {
            var dbSetTypes = repoConstructor.GetParameters()
                .Select(p => p.ParameterType).ToList();

            var parameterValues = new object[dbSetTypes.Count];

            for (int i = 0; i < parameterValues.Length; i++)
            {
                var modelTypes = dbSetTypes[i].GenericTypeArguments;

                if (modelTypes == null || modelTypes.Length != 1)
                {
                    throw new Exception(dbSetTypes[i].FullName +
                                        " is not acceptable for a CrudRepository " +
                                        "constructor argument.");
                }

                object dbSet = GetDbSetByType(modelTypes[0]);

                parameterValues[i] = dbSet;
            }

            return parameterValues;
        }

        private ConstructorInfo GetConstructor(Type repoType)
        {
            var constructors = repoType.GetConstructors();

            var dbSetOnlyConstructors = constructors.Where(c => IsDbSetOnly(c.GetParameters()));

            var setOnlyConstructors = dbSetOnlyConstructors as ConstructorInfo[] ?? dbSetOnlyConstructors.ToArray();

            if (setOnlyConstructors.Length > 0)
            {
                var theConstructor = setOnlyConstructors[0];

                int longest = theConstructor.GetParameters().Length;

                foreach (var c in setOnlyConstructors)
                {
                    var len = c.GetParameters().Length;

                    if (len > longest)
                    {
                        longest = len;

                        theConstructor = c;
                    }
                }

                return theConstructor;
            }

            return null;
        }

        private bool IsDbSetOnly(ParameterInfo[] parameters)
        {
            var dbSetType = typeof(DbSet<>);

            foreach (var parameter in parameters)
            {
                var type = parameter.ParameterType;

                if (!ExtendsGeneric(type,dbSetType))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ExtendsGeneric(Type type, Type genericParent)
        {
            Type current = type;

            while (current != null)
            {
                if (current.GetGenericTypeDefinition() == genericParent)
                {
                    return true;
                }

                current = current.BaseType;
            }
            return false;
        }

        public void Complete()
        {
            _context.SaveChanges();
        }
    }
}