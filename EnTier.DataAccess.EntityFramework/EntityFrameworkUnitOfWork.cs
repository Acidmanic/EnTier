using System;
using System.Reflection;
using EnTier.Repositories;
using EnTier.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework
{
    public class EntityFrameworkUnitOfWork : UnitOfWorkBase
    {
        private readonly DbContext _context;
        private readonly Func<Type, object> _getDbSetByType;

        public EntityFrameworkUnitOfWork(DbContext context)
        {
            _context = context;


            var contextType = typeof(DbContext);

            var getDbSetMethod = contextType.GetMethod(nameof(DbContext.Set), new Type[] { });

            _getDbSetByType = entityType =>
            {
                var method = getDbSetMethod?.MakeGenericMethod(new Type[] { entityType});

                return method?.Invoke(_context, new object[] { });
            };
        }
        
        protected override ICrudRepository<TStorage, TId> CreateDefaultCrudRepository<TStorage, TId>()
        {
            var dbSet = _context.Set<TStorage>();

            return new EntityFrameWorkCrudRepository<TStorage, TId>(dbSet);
        }


        protected override bool IsConstructorAcceptable(ConstructorInfo constructor)
        {
            return IsDbSetOnly(constructor.GetParameters());
        }

        protected override object ProvideConstructorParameter(Type parameterType)
        {
            var genericArguments = parameterType.GenericTypeArguments;

            if (genericArguments == null || genericArguments.Length != 1)
            {
                throw new Exception("Custom repositories can only accept " +
                                    "parameters of type DbSet<TStorage>");
            }
            
            return _getDbSetByType(genericArguments[0]);
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

        public override void Complete()
        {
            _context.SaveChanges();
        }
    }
}