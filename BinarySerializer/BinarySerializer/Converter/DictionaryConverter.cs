using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Converter
{

    /// <summary>
    /// <see cref="KeyValuePair{TKey, TValue}"/>转换器
    /// </summary>
    public class KeyValuePairConverter : BinaryConverter
    {
        public Type TKey;
        public Type TValue;
        public PropertyInfo PropKey;
        public PropertyInfo PropValue;
        public override bool CanConvert(Type type)
        {
            if(type.IsGenericType)
            {
                Type[] types = type.GetGenericArguments();
                if(types.Length == 2)
                {
                    TKey = types[0];
                    TValue = types[1];
                    Type currentType = typeof(KeyValuePair<,>).MakeGenericType(types);
                    if(currentType == type)
                    {
                        PropKey = currentType.GetProperty("Key");
                        PropValue = currentType.GetProperty("Value");
                        return true;
                    }
                }
            }
            return false;
        }
        public override void ReadBytes(Stream stream, out object obj)
        {
            if(stream.ReadByte() != 0)
            {
                object key = Serializer.Deserialize(TKey, stream);
                object value = Serializer.Deserialize(TValue, stream);
                obj = Activator.CreateInstance(PropKey.DeclaringType, key, value);
                return;
            }
            obj = null;
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
                Serializer.Serialize(TKey, PropKey.GetValue(obj), stream);
                Serializer.Serialize(TValue, PropValue.GetValue(obj), stream);
            }
        }
    }
    /// <summary>
    /// <see cref="Dictionary{TKey, TValue}"/>转换器
    /// </summary>
    public class DictionaryConverter : BinaryConverter
    {
        public Type TKey;
        public Type TValue;
        public PropertyInfo PropKeys;
        public PropertyInfo PropValues;
        public override bool CanConvert(Type type)
        {
            if(type.IsGenericType)
            {
                Type[] types = type.GetGenericArguments();
                if(types.Length == 2)
                {
                    TKey = types[0];
                    TValue = types[1];
                    Type currentType = typeof(Dictionary<,>).MakeGenericType(types);
                    if(currentType == type)
                    {
                        PropKeys = currentType.GetProperty("Keys");
                        PropValues = currentType.GetProperty("Values");
                        return true;
                    }
                }
            }
            return false;
        }
        public override void ReadBytes(Stream stream, out object obj)
        {
            if(stream.ReadByte() != 0)
            {
                var keys = (Array)Serializer.Deserialize(TKey.MakeArrayType(), stream);
                var values = (Array)Serializer.Deserialize(TValue.MakeArrayType(), stream);
                obj = Activator.CreateInstance(PropKeys.DeclaringType);
                MethodInfo add = PropKeys.DeclaringType.GetMethod("Add");
                for(int i = 0; i < keys.Length; i++)
                {
                    add.Invoke(obj, new[] { keys.GetValue(i), values.GetValue(i) });
                }
                return;
            }
            obj = null;
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
                Serializer.Serialize(TKey.MakeArrayType(), PropKeys.GetValue(obj), stream);
                Serializer.Serialize(TValue.MakeArrayType(), PropValues.GetValue(obj), stream);
            }
        }
    }
}
