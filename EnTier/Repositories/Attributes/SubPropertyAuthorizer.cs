using System;
using System.Collections.Generic;

namespace EnTier.Repositories.Attributes
{
    public class SubPropertyAuthorizer
    {
        private readonly bool _allowUnDecided;
        private readonly bool _preventationSuperceedsAllowing;
        private readonly List<Type> _keeping;
        private readonly List<Type> _stripping;

        public SubPropertyAuthorizer(List<DataInsertionAttribute> insertionAttributes)
        {
            _allowUnDecided = false;
            _preventationSuperceedsAllowing = true;
            _keeping = new List<Type>();
            _stripping = new List<Type>();

            foreach (var attribute in insertionAttributes)
            {
                if (attribute is KeepAllPropertiesAttribute)
                {
                    //Allow All
                    _allowUnDecided = true;
                }

                if (attribute is StripAllPropertiesAttribute)
                {
                    // Strip All
                    _allowUnDecided = false;
                }

                if (attribute is KeepPropertyAttribute keeper)
                {
                    _keeping.AddRange(keeper.TypesList);
                }

                if (attribute is StripPropertyAttribute stripper) // No, Not like that!
                {
                    _stripping.AddRange(stripper.TypesList);
                }
            }
        }


        public bool IsAllowed(Type type)
        {
            bool kept = IsListed(type, _keeping);
            bool stripped = IsListed(type, _stripping);

            if (kept && stripped)
            {
                return !_preventationSuperceedsAllowing;
            }

            if (!kept && !stripped)
            {
                return _allowUnDecided;
            }

            return kept;
        }

        private bool IsListed(Type t, List<Type> list)
        {
            foreach (var type in list)
            {
                if (type == t)
                {
                    return true;
                }
            }

            return false;
        }
    }
}