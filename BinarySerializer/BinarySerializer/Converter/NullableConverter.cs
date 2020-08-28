using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static BinarySerializer.Serializer;

namespace BinarySerializer.Converter
{
    /// <summary>
    /// 可空类型<see cref="Nullable{T}"/>转换器
    /// </summary>
    [GenericConverter(typeof(Nullable<>))]
    public class NullableConverter : GenericConverter
    {
        
        public override void ReadBytes(Stream stream, out object obj)
        {
            obj = null;
            if(stream.ReadByte() != 0)
            {
                obj = Deserialize(TypeArgs[0], stream);
            }
        }
        public override void WriteBytes(object obj, Stream stream)
        {
            if(obj == null)
            {
                stream.WriteByte(0);
            }
            else
            {
                stream.WriteByte(1);
                Serialize(obj, stream);
            }
        }
    }
}
