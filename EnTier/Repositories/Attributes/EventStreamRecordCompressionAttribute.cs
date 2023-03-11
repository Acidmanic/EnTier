using System;
using System.IO.Compression;
using Acidmanic.Utilities;

namespace EnTier.Repositories.Attributes
{
    public class EventStreamRecordCompressionAttribute : Attribute
    {
        public EventStreamRecordCompressionAttribute(Compressions compression,
            CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            Compression = compression;
            CompressionLevel = compressionLevel;
        }

        public Compressions Compression { get; }

        public CompressionLevel CompressionLevel { get; }
    }
}