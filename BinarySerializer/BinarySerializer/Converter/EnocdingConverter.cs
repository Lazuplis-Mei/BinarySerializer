using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Converter
{
    /// <summary>
    /// <see cref="Encoding"/>转换器
    /// </summary>
    public class EnocdingConverter : BinaryConverter<Encoding>
    {
        public override Encoding ReadBytes(Stream stream)
        {
            return Encoding.GetEncoding(stream.ReadInt32());
        }
        public override void WriteBytes(Encoding obj, Stream stream)
        {
            stream.WriteInt32(obj.CodePage);
        }
    }
}
