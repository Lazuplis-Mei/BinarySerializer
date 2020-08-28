using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using static BinarySerializer.Serializer;

namespace BinarySerializer.Converter
{
    /// <summary>
    /// 找不到指定的类型，请参考<see cref="AddAssembly(Assembly)"/>
    /// </summary>
    public class TypeNotFoundException : Exception
    {

    }

    /// <summary>
    /// <see cref="Type"/>转换器
    /// </summary>
    public class TypeConverter : BinaryConverter<Type>
    {
        public override Type ReadBytes(Stream stream)
        {
            int flag = stream.ReadByte();
            int token = stream.ReadInt32();
            Guid guid = new Guid(stream.ReadBytes(16));
            foreach(var module in Modules)
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
                                types[i] = Deserialize<Type>(stream);
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
        public override void WriteBytes(Type obj, Stream stream)
        {
            if(obj.IsArray)
            {
                stream.WriteByte(2);
                obj = obj.GetElementType();
                stream.WriteInt32(obj.MetadataToken);
                stream.WriteBytes(obj.GUID.ToByteArray());
            }
            else if(obj.IsGenericType)
            {
                stream.WriteByte(3);
                Type[] types = obj.GetGenericArguments();
                obj = obj.GetGenericTypeDefinition();
                stream.WriteInt32(obj.MetadataToken);
                stream.WriteBytes(obj.GUID.ToByteArray());
                stream.WriteByte((byte)types.Length);
                foreach(var item in types)
                {
                    Serializer.Serialize(item, stream);
                }
            }
            else
            {
                stream.WriteByte(1);
                stream.WriteInt32(obj.MetadataToken);
                stream.WriteBytes(obj.GUID.ToByteArray());
            }
        }
    }
}
