using System;
using System.Linq;
using System.Reflection;
using EnTier.Results;
using EnTier.Utility;

namespace EnTier.AutoWrap
{
    public  class AutoWrap
    {

        public bool Enabled { get; private set; }
        
        public bool AutoName { get; private set; }
        
        public string Name { get; private set; }
        
        private AutoWrap()
        {
            
        }

        public static AutoWrap IsEnabled(object obj)
        {
            if (obj == null)
            {
                return new AutoWrap
                {
                    Enabled = false,
                    Name = null,
                    AutoName = false
                };
            }

            return IsEnabled(obj.GetType());
        }
        
        
        public static AutoWrap IsEnabled<T>()
        {
            return IsEnabled(typeof(T));
        }

        public static AutoWrap IsEnabled(Type type)
        {

            var autoWrap = IsEnabled();

            if (autoWrap.Enabled)
            {
                return autoWrap;
            }
            
            var classAttribute =
                type.GetCustomAttributes<AutoWrapAttribute>()
                    .LastOrDefault();

            if (classAttribute != null)
            {
                return new AutoWrap
                {
                    Enabled = true,
                    Name = classAttribute.Name,
                    AutoName = classAttribute.AutoName
                };
            }
            return new AutoWrap
            {
                Enabled = false,
                Name = null,
                AutoName = false
            };
        }

        public static AutoWrap IsEnabled()
        {
            var delivered = 
                new AttributeHelper().DeliveredAttributes<AutoWrapAttribute>()
                    .LastOrDefault();
            
            if (delivered != null)
            {
                return new AutoWrap
                {
                    Enabled = true,
                    Name = delivered.Name,
                    AutoName = delivered.AutoName
                };
            }
            return new AutoWrap
            {
                Enabled = false,
                Name = null,
                AutoName = false
            };
        }
    }
}