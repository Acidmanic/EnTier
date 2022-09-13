using System.Collections.Generic;

namespace EnTier.Utility.IdGenerating
{
    public class IncrementerFactory
    {
        private static readonly List<IIncrementer> _incrementers = new List<IIncrementer>
        {
            new IntegerIncrementer(),
            new LongIncrementer(),
            new ShortIncrementer(),
            new DoubleIncrementer(),
            new DoubleIncrementer(),
            new FloatIncrementer(),
            new DecimalIncrementer(),
            new ByteIncrementer(),
            new UIntIncrementer(),
            new ULongIncrementer(),
            new UShortIncrementer(),
            new SByteIncrementer()
        };

        public IncrementerFactory()
        {
        }

        public IIncrementer<TId> Make<TId>()
        {
            foreach (var incrementer in _incrementers)
            {
                if (incrementer is IIncrementer<TId> implemented)
                {
                    return implemented;
                }
            }

            return null;
        }
    }
}