using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static BinarySerializer.Serializer;

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
            obj = null;
            int len = stream.ReadInt32();
            if(len != -1)
            {
                Type type = typeof(List<>).MakeGenericType(InternalType);
                object list = Activator.CreateInstance(type);
                var Add = type.GetMethod("Add");
                for(int i = 0; i < len; i++)
                {
                    Add.Invoke(list, Deserialize(InternalType, stream));
                }
                obj = list;
                if(IsArray)
                {
                    obj = type.GetMethod("ToArray").Invoke(list);
                }
            }
        }
        public override void WriteBytes(object obj, Stream stream)
        {
            if(obj == null)
            {
                stream.WriteInt32(-1);
            }
            else
            {
                IEnumerable<object> array = ((IEnumerable)obj).Cast<object>();
                stream.WriteInt32(array.Count());
                foreach(var item in array)
                {
                    Serialize(InternalType, item, stream);
                }
            }
        }
    }

    [GenericConverter(typeof(HashSet<>))]
    public class HashSetConverter : GenericConverter
    {
        public override void ReadBytes(Stream stream, out object obj)
        {
            obj = null;
            int len = stream.ReadInt32();
            if(len != -1)
            {
                object hashset = Activator.CreateInstance(CurrentType);
                for(int i = 0; i < len; i++)
                {
                    var Add = CurrentType.GetMethod("Add");
                    Add.Invoke(hashset, Deserialize(TypeArgs[0], stream));
                }
                obj = hashset;
            }
        }
        public override void WriteBytes(object obj, Stream stream)
        {
            if(obj == null)
            {
                stream.WriteInt32(-1);
            }
            else
            {
                IEnumerable<object> hashset = ((IEnumerable)obj).Cast<object>();
                stream.WriteInt32(hashset.Count());
                foreach(var item in hashset)
                {
                    Serialize(TypeArgs[0], item, stream);
                }
            }

        }
    }
}
