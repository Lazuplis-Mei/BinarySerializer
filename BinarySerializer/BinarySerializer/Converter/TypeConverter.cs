using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Converter
{

    public class TypeNotFoundException : Exception
    {

    }

    public class TypeConverter : BinaryConverter<Type>
    {
        public override Type ReadBytes(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            int flag = stream.ReadByte();
            if(flag != 0)
            {
                int token = reader.ReadInt32();
                Guid guid = new Guid(reader.ReadBytes(16));
                foreach(var module in Serializer.Modules)
                {
                    try
                    {
                        Type type = module.ResolveType(token);
                        if(type.GUID == guid)
                        {
                            if(flag == 1)
                            {
                                return type;
                            }
                            else if(flag == 2)
                            {
                                return type.MakeArrayType();
                            }
                            else if(flag == 3)
                            {
                                int len = stream.ReadByte();
                                Type[] types = new Type[len];
                                for(int i = 0; i < len; i++)
                                {
                                    types[i] = Serializer.Deserialize<Type>(stream);
                                }
                                return type.MakeGenericType(types);
                            }
                        }
                    }
                    catch(Exception)
                    {
                        continue;
                    }
                }
                throw new TypeNotFoundException();
            }
            return null;
        }
        public override void WriteBytes(Type obj, Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            if(obj == null)
            {
                stream.WriteByte(0);
            }
            else
            {
                if(obj.IsArray)
                {
                    stream.WriteByte(2);
                    obj = obj.GetElementType();
                    writer.Write(obj.MetadataToken);
                    writer.Write(obj.GUID.ToByteArray());
                }
                else if(obj.IsGenericType)
                {
                    stream.WriteByte(3);
                    Type[] types = obj.GetGenericArguments();
                    obj = obj.GetGenericTypeDefinition();
                    writer.Write(obj.MetadataToken);
                    writer.Write(obj.GUID.ToByteArray());
                    stream.WriteByte((byte)types.Length);
                    foreach(var item in types)
                    {
                        Serializer.Serialize(item, stream);
                    }
                }
                else
                {
                    stream.WriteByte(1);
                    writer.Write(obj.MetadataToken);
                    writer.Write(obj.GUID.ToByteArray());
                }
            }
        }
    }
}
