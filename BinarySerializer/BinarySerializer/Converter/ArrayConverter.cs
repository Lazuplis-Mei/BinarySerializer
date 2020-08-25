using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Converter
{
    /// <summary>
    /// <see cref="Array"/>，<see cref="List{T}"/>，<see cref="IEnumerable{T}"/>转换器
    /// 其中<see cref="IEnumerable{T}"/>的实例为<see cref="List{T}"/>
    /// </summary>
    public class ArrayConverter : BinaryConverter
    {
        public Type InternalType;
        public bool IsArray;
        public override bool CanConvert(Type type)
        {
            if(type.IsArray)
            {
                InternalType = type.GetElementType();
                IsArray = true;
                return true;
            }
            else if(type.IsGenericType)
            {
                InternalType = type.GenericTypeArguments[0];
                if(typeof(List<>).MakeGenericType(InternalType) == type)
                {
                    return true;
                }
                else if(typeof(IEnumerable<>).MakeGenericType(InternalType) == type)
                {
                    return true;
                }
            }
            return false;
        }
        public override void ReadBytes(Stream stream, out object obj)
        {
            BinaryReader reader = new BinaryReader(stream);
            int len = reader.ReadInt32();
            if(len != -1)
            {
                Type type = typeof(List<>).MakeGenericType(InternalType);
                object list = Activator.CreateInstance(type);
                var addMethod = type.GetMethod("Add");
                for(int i = 0; i < len; i++)
                {
                    addMethod.Invoke(list, new[] { Serializer.Deserialize(InternalType, stream) });
                }
                obj = list;
                if(IsArray)
                {
                    obj = type.GetMethod("ToArray").Invoke(list, null);
                }
                return;
            }
            obj = null;
        }
        public override void WriteBytes(object obj, Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            if(obj == null)
            {
                writer.Write(-1);
            }
            else
            {
                IEnumerable<object> array = ((System.Collections.IEnumerable)obj).Cast<object>();
                writer.Write(array.Count());
                foreach(var item in array)
                {
                    Serializer.Serialize(InternalType, item, stream);
                }
            }
        }
    }
}
