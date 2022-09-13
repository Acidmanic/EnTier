using System;

namespace EnTier.Utility.IdGenerating
{
    public class IntegerIncrementer : IncrementerBase<int>,IIncrementer<Int32>
    {
        public override int Increment(int value)
        {
            return value + 1;
        }
    }

    public class LongIncrementer : IncrementerBase<long>,IIncrementer<Int64>
    {
        public override long Increment(long value)
        {
            return value + 1;
        }
    }

    public class ShortIncrementer : IncrementerBase<short>,IIncrementer<Int16>
    {
        public override short Increment(short value)
        {
            int iv = (int) value;

            iv++;
            if (iv > short.MaxValue)
            {
                iv = short.MaxValue;
            }

            if (iv < short.MinValue)
            {
                iv = short.MinValue;
            }

            return (short) iv;
        }
    }

    public class DoubleIncrementer : IncrementerBase<double>,IIncrementer<Double>
    {
        public override double Increment(double value)
        {
            return value + 1;
        }
    }

    public class FloatIncrementer : IncrementerBase<float>
    {
        public override float Increment(float value)
        {
            return value + 1;
        }
    }

    public class DecimalIncrementer : IncrementerBase<decimal>,IIncrementer<Decimal>
    {
        public override decimal Increment(decimal value)
        {
            return value + 1;
        }
    }

    public class ByteIncrementer : IncrementerBase<byte>,IIncrementer<Byte>
    {
        public override byte Increment(byte value)
        {
            int iv = (int) value;

            iv++;
            if (iv > byte.MaxValue)
            {
                iv = byte.MaxValue;
            }

            if (iv < byte.MinValue)
            {
                iv = byte.MinValue;
            }

            return (byte) iv;
        }
    }

    public class UIntIncrementer : IncrementerBase<uint>,IIncrementer<UInt32>
    {
        public override uint Increment(uint value)
        {
            return value + 1;
        }
    }

    public class ULongIncrementer : IncrementerBase<ulong>,IIncrementer<UInt64>
    {
        public override ulong Increment(ulong value)
        {
            return value + 1;
        }
    }
    
    public class UShortIncrementer : IncrementerBase<ushort>,IIncrementer<UInt16>
    {
        public override ushort Increment(ushort value)
        {
            int iv = (int) value;

            iv++;
            if (iv > ushort.MaxValue)
            {
                iv = ushort.MaxValue;
            }

            if (iv < ushort.MinValue)
            {
                iv = ushort.MinValue;
            }

            return (ushort) iv;
        }
    }

    public class SByteIncrementer : IncrementerBase<sbyte>,IIncrementer<SByte>
    {
        public override sbyte Increment(sbyte value)
        {
            int iv = (int) value;

            iv++;
            if (iv > sbyte.MaxValue)
            {
                iv = sbyte.MaxValue;
            }

            if (iv < sbyte.MinValue)
            {
                iv = sbyte.MinValue;
            }

            return (sbyte) iv;
        }
    }
    
    
}