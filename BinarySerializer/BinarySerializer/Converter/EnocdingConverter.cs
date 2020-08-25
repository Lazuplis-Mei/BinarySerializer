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
            if(stream.ReadByte() != 0)
            {
                BinaryReader reader = new BinaryReader(stream);
                return Encoding.GetEncoding(reader.ReadInt32());
            }
            return null;
        }
        public override void WriteBytes(Encoding obj, Stream stream)
        {
            if(obj == null)
            {
                stream.WriteByte(0);
            }
            else
            {
                stream.WriteByte(1);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(obj.CodePage);
            }
        }
    }
}
