using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Converter
{
    public class DateTimeConverter : BinaryConverter<DateTime>
    {
        public override DateTime ReadBytes(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            return DateTime.FromBinary(reader.ReadInt64());
        }
        public override void WriteBytes(DateTime obj, Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(obj.ToBinary());
        }
    }
    public class TimeSpanConverter : BinaryConverter<TimeSpan>
    {
        public override TimeSpan ReadBytes(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            return TimeSpan.FromTicks(reader.ReadInt64());
        }
        public override void WriteBytes(TimeSpan obj, Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(obj.Ticks);
        }
    }
}
