using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Reflection
{
    public class PropertyWrapper<T>
    {


        private Func<T> _read = () => default;
        private Action<T> _write = (value) => { };



        public T Value { get; set; }

        public PropertyWrapper()
        {

        }

        public PropertyWrapper(PropertyInfo propertyInfo,object obj)
        {
            if (propertyInfo.CanRead)
            {
                _read = () =>
                {
                    try
                    {
                        return (T)propertyInfo.GetValue(obj);
                    }
                    catch (Exception) { }
                    return default;
                };
            }
            if (propertyInfo.CanWrite)
            {
                _write = (value) =>
                {
                    try
                    {
                        propertyInfo.SetValue(obj, value);
                    }
                    catch (Exception) { }
                };
            }
        }

        public PropertyWrapper(Func<T> read, Action<T> write)
        {
            _read = read;

            _write = write;
        }

    }
}
