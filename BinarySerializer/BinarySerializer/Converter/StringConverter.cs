using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static BinarySerializer.Serializer;

namespace BinarySerializer.Converter
{
    /// <summary>
    /// <see cref="string"/>转换器，默认编码为<see cref="DefaultEncoding"/>
    /// </summary>
    public class StringConverter : BinaryConverter<string>
    {
        public override string ReadBytes(Stream stream)
        {
            int len = stream.ReadInt32();
            byte[] bytes = stream.ReadBytes(len);
            return DefaultEncoding.GetString(bytes);
        }
        public override void WriteBytes(string obj, Stream stream)
        {
            byte[] bytes = DefaultEncoding.GetBytes(obj);
            stream.WriteInt32(bytes.Length);
            stream.WriteBytes(bytes);
        }
    }
}
