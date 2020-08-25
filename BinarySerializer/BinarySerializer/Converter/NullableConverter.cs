using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Converter
{
    /// <summary>
    /// 可空类型<see cref="Nullable{T}"/>转换器
    /// </summary>
    public class NullableConverter : BinaryConverter
    {
        public Type InternalType;
        public override bool CanConvert(Type type)
        {
            if(type.IsGenericType)
            {
                InternalType = type.GenericTypeArguments[0];
                return typeof(Nullable<>).MakeGenericType(InternalType) == type;
            }
            return false;
        }
        public override void ReadBytes(Stream stream, out object obj)
        {
            obj = null;
            if(stream.ReadByte() != 0)
            {
                obj = Serializer.Deserialize(InternalType, stream);
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
                Serializer.Serialize(obj, stream);
            }
        }
    }
}
