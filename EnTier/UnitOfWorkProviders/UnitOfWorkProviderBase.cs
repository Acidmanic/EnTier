




using Repository;
using Utility;

namespace Providers
{
    public abstract class UnitOfWorkProviderBase : IProvider<IUnitOfWork>
    {
        public abstract IUnitOfWork Create();

        protected IUnitOfWork FindUnitOfWork(params object[] args){

                var reflection = ReflectionService.Make();

                var constructor = reflection
                    .FilterRemoveImplementers<IEnTierGeneric>()
                    .FindConstructor<IUnitOfWork>(args);

                if(!constructor.IsNull){

                    var ret =  constructor.Construct();

                    return ret;
                }

                return null;
            }

    }
}