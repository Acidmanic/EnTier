using System;

namespace EnTier.EventStore.WebView
{
    public static class StringParseExtensions
    {
        
        
        public static object ParseOrDefault(this string stringValue, Type type)
        {
            if (type == typeof(string))
            {
                return stringValue;
            }

            var parseMethod = type.GetMethod("Parse", new[] { typeof(string) });

            if (parseMethod != null)
            {
                try
                {
                    var value = parseMethod.Invoke(null, new object[] { stringValue });

                    return value;
                }
                finally
                {
                }
            }


            return null;
        }
    }
}