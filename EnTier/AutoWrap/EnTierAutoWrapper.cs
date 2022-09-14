using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;

namespace EnTier.AutoWrap
{
    public class EnTierAutoWrapper<TModel>
    {

        private readonly Type _ownerType;
        private readonly bool _disabled;
        
        
        
        public EnTierAutoWrapper(Type ownerType)
        {
            _ownerType = ownerType;
            _disabled = false;
        }

        public EnTierAutoWrapper(object owner)
        {
            if (owner == null)
            {
                _disabled = true;
            }
            else
            {
                _ownerType = owner.GetType();    
            }
        }
    
        public object WrapIfNeeded(IEnumerable<TModel> data)
        {
            if (_disabled)
            {
                return data;
            }
            return WrapIfNeeded(data, _ownerType);
        }
        
        public  object WrapIfNeeded(IEnumerable<TModel> data,Type ownerType)
        {
            
            var aw = AutoWrap.IsEnabled(ownerType);

            if (aw.Enabled)
            {
                var wrapper = aw.AutoName
                    ? new EnumerableDynamicWrapper<TModel>()
                    : new EnumerableDynamicWrapper<TModel>(aw.Name);

                return wrapper.Wrap(data);
            }
            return data;
        }
    }
}