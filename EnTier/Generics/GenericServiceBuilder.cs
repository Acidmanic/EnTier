


using System;
using Plugging;
using Service;

namespace Generics{



    public class GenericServiceBuilder : IGenericBuilder<IService>
    {


        public object Build<TStorage,TDomain,Tid>()
        where TStorage:class
        {
            var mapper = EnTierApplication.Resolver.Resolve<IObjectMapper>();

            return new GenericService<TStorage,TDomain,Tid>(mapper);
        }

    }
}