using EnTier.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using EnTier.Utility;

namespace EnTier.Components
{
    class SearchForEfDbContextComponentProvider : IComponentProvider
    {
        public TInterface Provide<TInterface>(params object[] args)
        {
            var r = ReflectionService.Make();
            //TODO: Filter only those which contain a DbSet<Storage> property
            var dbConstructor = r.FindConstructor<DbContext>(t => r.Extends<DbContext>(t));

            if (!dbConstructor.IsNull)
            {
                var ret = new EntityFrameworkContext(dbConstructor.Construct());

                return (TInterface)(object)ret;

            }

            return default;
        }
    }
}
