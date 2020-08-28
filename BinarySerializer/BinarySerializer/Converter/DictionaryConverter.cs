using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static BinarySerializer.Serializer;

namespace BinarySerializer.Converter
{

    /// <summary>
    /// <see cref="KeyValuePair{TKey, TValue}"/>转换器
    /// </summary>
    [GenericConverter(typeof(KeyValuePair<,>))]
    public class KeyValuePairConverter : GenericConverter
    {
        public override void ReadBytes(Stream stream, out object obj)
        {
            obj = null;
            if(stream.ReadByte() != 0)
            {
                object key = Deserialize(TypeArgs[0], stream);
                object value = Deserialize(TypeArgs[1], stream);
                obj = Activator.CreateInstance(CurrentType, key, value);
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
                Serialize(TypeArgs[0], CurrentType.GetProperty("Key").GetValue(obj), stream);
                Serialize(TypeArgs[1], CurrentType.GetProperty("Value").GetValue(obj), stream);
            }
        }
    }


    /// <summary>
    /// <see cref="Dictionary{TKey, TValue}"/>转换器
    /// </summary>
    [GenericConverter(typeof(Dictionary<,>))]
    public class DictionaryConverter : GenericConverter
    {
        public override void ReadBytes(Stream stream, out object obj)
        {
            obj = null;
            if(stream.ReadByte() != 0)
            {
                var keys = (Array)Deserialize(TypeArgs[0].MakeArrayType(), stream);
                var values = (Array)Deserialize(TypeArgs[1].MakeArrayType(), stream);
                obj = Activator.CreateInstance(CurrentType);
                MethodInfo Add = CurrentType.GetMethod("Add");
                for(int i = 0; i < keys.Length; i++)
                {
                    Add.Invoke(obj, keys.GetValue(i), values.GetValue(i));
                }
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
                Serialize(TypeArgs[0].MakeArrayType(), CurrentType.GetProperty("Keys").GetValue(obj), stream);
                Serialize(TypeArgs[1].MakeArrayType(), CurrentType.GetProperty("Values").GetValue(obj), stream);
            }
        }
    }
}
