using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Converter
{
    /// <summary>
    /// <see cref="DateTime"/>转换器
    /// </summary>
    public class DateTimeConverter : BinaryConverter<DateTime>
    {
        public override DateTime ReadBytes(Stream stream)
        {
            return DateTime.FromBinary(stream.ReadInt64());
        }
        public override void WriteBytes(DateTime obj, Stream stream)
        {
            stream.WriteInt64(obj.ToBinary());
        }
    }

    /// <summary>
    /// <see cref="DateTimeOffset"/>转换器
    /// </summary>
    public class DateTimeOffsetConverter : BinaryConverter<DateTimeOffset>
    {
        public override DateTimeOffset ReadBytes(Stream stream)
        {
            return DateTimeOffset.FromFileTime(stream.ReadInt64());
        }
        public override void WriteBytes(DateTimeOffset obj, Stream stream)
        {
            stream.WriteInt64(obj.ToFileTime());
        }
    }

    /// <summary>
    /// <see cref="TimeSpan"/>转换器
    /// </summary>
    public class TimeSpanConverter : BinaryConverter<TimeSpan>
    {
        public override TimeSpan ReadBytes(Stream stream)
        {
            return TimeSpan.FromTicks(stream.ReadInt64());
        }
        public override void WriteBytes(TimeSpan obj, Stream stream)
        {
            stream.WriteInt64(obj.Ticks);
        }
    }
}
