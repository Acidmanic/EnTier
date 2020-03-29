


using System;

namespace EnTier.DataAccess
{



    public class AttributeEagerScopeManager<Entity> : IDisposable
    where Entity:class
    {


        private readonly EagerScopeManager _attributesScope = null;


        public AttributeEagerScopeManager(IDisposable parent)
        {
            _attributesScope = new EagerAttributeProcessor()
                .MarkEagers<Entity>(parent);
        }

        public void Dispose()
        {
            if (_attributesScope!=null) _attributesScope.Dispose();
        }
    }
}