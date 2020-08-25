using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Converter
{
    /// <summary>
    /// <see cref="string"/>转换器，默认编码为<see cref="Serializer.DefaultEncoding"/>
    /// </summary>
    public class StringConverter : BinaryConverter<string>
    {
        public override string ReadBytes(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            int len = reader.ReadInt32();
            if(len != -1)
            {
                byte[] bytes = reader.ReadBytes(len);
                return Serializer.DefaultEncoding.GetString(bytes);
            }
            return null;
        }
        public override void WriteBytes(string obj, Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            if(obj == null)
            {
                writer.Write(-1);
            }
            else
            {
                byte[] bytes = Serializer.DefaultEncoding.GetBytes(obj);
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
        }
    }
}
