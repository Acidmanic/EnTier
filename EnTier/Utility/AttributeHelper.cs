using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EnTier.Utility
{
    class AttributeHelper
    {
        /// <summary>
        /// This method returns all instances of attributes that has been assigned to the call chain of caller method
        /// </summary>
        /// <typeparam name="T">Type of The attribute to look up</typeparam>
        /// <returns>Any found attribute object of given type</returns>
        public List<T> DeliveredAttributes<T>()
            where T : Attribute
        {
            var attributes = new List<T>();

            var stack = new StackTrace();

            var frames = stack.GetFrames();

            if (frames != null)
            {
                foreach (var frame in frames)
                {
                    var frameAttributes = frame.GetMethod().GetCustomAttributes(true);

                    foreach (var frameAttribute in frameAttributes)
                    {
                        if (frameAttribute is T attribute)
                        {
                            attributes.Add(attribute);
                        }
                    }
                }
            }
            
            return attributes;
        }
    }
}