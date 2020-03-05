



using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace DataAccess
{

    public partial class GenericDatabaseUnit:UnitOfDataAccessBase{

        private Dictionary<Type,Object> _dbSets ;

        private Dictionary<Type,Type> _repositoryTypes;

        public GenericDatabaseUnit(DbContext context):base(context)
        {
            _dbSets = ListAllDbSets(context);

            _repositoryTypes = ListAllRepositoryTypes();

        }

        private Dictionary<Type, Type> ListAllRepositoryTypes()
        {

            var  ret = new Dictionary<Type, Type>();

            var assembly = Assembly.GetCallingAssembly();

            var types = assembly.GetTypes();

            foreach(var type in types){

                var entity = GetEntityTypeFromImplementedIRepository(type);

                if(entity != null){
                    ret.Add(entity,type);
                }
            }

            return ret;
        }

        private Type GetEntityTypeFromImplementedIRepository(Type type)
        {



            if (type.IsInterface || type.IsAbstract){
                return null;
            }
            var parent = type;

            while(parent != null){
                var interfaces = parent.GetInterfaces();

                foreach(var ifc in interfaces){
                    if (ifc.IsGenericType){
                        if (ifc.GetGenericTypeDefinition() == typeof(IRepository<>)){
                            return ifc.GetGenericArguments()[0];
                        }
                    }
                }

                parent = parent.BaseType;
            }

            return null;
        }

        private Dictionary<Type, object> ListAllDbSets(DbContext context)
        {
            var ret = new Dictionary<Type, object>();

            var contextType = context.GetType();

            var properties = contextType.GetProperties();

            foreach(var prop in properties){
                var propType = prop.PropertyType;

                if(propType.IsGenericType){
                    if( propType.GetGenericTypeDefinition() == typeof(DbSet<>)){

                        var entity = propType.GetGenericArguments()[0];

                        var dbset = prop.GetValue(context);

                        ret.Add(entity,dbset);
                    }

                }
            }

            return ret;
        }

        public Trepository GetRepository<Trepository,Entity>() where Trepository : IRepository<Entity> where Entity : class
        {

            var enType = typeof(Entity);

            var dbset = (DbSet<Entity>)_dbSets[enType];

            var repoType = _repositoryTypes[enType];

            var repository = CreateInstance<Trepository>(repoType,dbset);

            return repository;

        }

        public IRepository<Entity> CreateRepository<Entity>() where Entity : class {
            
            var enType = typeof(Entity);

            var dbset = (DbSet<Entity>)_dbSets[enType];

            var ret = new GenericRepository<Entity>(dbset);

            return ret;
        }

        private Tcast CreateInstance<Tcast>(Type type, object argumet)
        {
            var constructor = type.GetConstructor(new Type[]{argumet.GetType()});

            var ret = constructor.Invoke(new object[]{argumet});

            return (Tcast) ret;
        }
    }


    
}