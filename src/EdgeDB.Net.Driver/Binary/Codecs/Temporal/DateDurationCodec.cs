using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeDB.Binary.Codecs
{
    internal sealed class DateDurationCodec : BaseTemporalCodec<DataTypes.DateDuration>
    {
        public DateDurationCodec()
        {
            AddConverter(From, To);
        }

        public override DataTypes.DateDuration Deserialize(ref PacketReader reader, CodecContext context)
        {
            reader.Skip(sizeof(long));
            var days = reader.ReadInt32();
            var months = reader.ReadInt32();

            return new(days, months);
        }
        
        public override void Serialize(ref PacketWriter writer, DataTypes.DateDuration value, CodecContext context)
        {
            writer.Write(0L);
            writer.Write(value.Days);
            writer.Write(value.Months);
        }

        private DataTypes.DateDuration From(ref TimeSpan value)
            => new(value);

        private TimeSpan To(ref DataTypes.DateDuration value)
            => value.TimeSpan;
    }
}
