using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Converter
{
    /// <summary>
    /// 提供二进制对象转换器的基类
    /// </summary>
    public abstract class BinaryConverter
    {
        public abstract bool CanConvert(Type type);
        public abstract void WriteBytes(object obj, Stream stream);
        public abstract void ReadBytes(Stream stream, out object obj);
    }
    /// <summary>
    /// 提供二进制对象转换器的泛型基类
    /// </summary>
    /// <typeparam name="T">转换类型</typeparam>
    public abstract class BinaryConverter<T> : BinaryConverter
    {
        public sealed override bool CanConvert(Type type)
        {
            
            return type.IsOrBaseFrom<T>();
        }

        public sealed override void WriteBytes(object obj, Stream stream)
        {
            WriteBytes((T)obj, stream);
        }
        public sealed override void ReadBytes(Stream stream, out object obj)
        {
            obj = ReadBytes(stream);
        }
        public abstract void WriteBytes(T obj, Stream stream);
        public abstract T ReadBytes(Stream stream);
    }
}
