using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Converter
{

    public class PropertyCannotReadException : Exception
    {

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class BinaryIgnoreAttribute : Attribute
    {

    }

    public class ObjectConverter
    {
        public static T ReadBytes<T>(Stream stream)
        {
            return (T)ReadBytes(typeof(T), stream);
        }

        public static object ReadBytes(Type type, Stream stream)
        {
            object obj = Activator.CreateInstance(type);
            var fieldInfos = type.GetFields(Serializer.BindingFlags).OrderBy(f => f.Name);
            var propertyInfos = type.GetProperties(Serializer.BindingFlags).OrderBy(f => f.Name);
            foreach(var field in fieldInfos)
            {
                if(field.GetCustomAttribute<BinaryIgnoreAttribute>() == null)
                {
                    field.SetValue(obj, Serializer.Deserialize(field.FieldType, stream));
                }
            }
            foreach(var propertyInfo in propertyInfos)
            {
                if(propertyInfo.GetCustomAttribute<BinaryIgnoreAttribute>() == null)
                {
                    if(propertyInfo.SetMethod != null)
                    {
                        if(propertyInfo.SetMethod.GetParameters().Length == 1)
                        {
                            object value = Serializer.Deserialize(propertyInfo.PropertyType, stream);

                            propertyInfo.SetValue(obj, value);
                        }
                    }
                }
            }
            return obj;
        }

        public static void WriteBytes<T>(T obj, Stream stream)
        {
            WriteBytes(typeof(T), obj, stream);
        }
        public static void WriteBytes(Type type, object obj, Stream stream)
        {
            var fieldInfos = type.GetFields(Serializer.BindingFlags).OrderBy(f => f.Name);
            var propertyInfos = type.GetProperties(Serializer.BindingFlags).OrderBy(f => f.Name);
            foreach(var field in fieldInfos)
            {
                if(field.GetCustomAttribute<BinaryIgnoreAttribute>() == null)
                {
                    Serializer.Serialize(field.FieldType, field.GetValue(obj), stream);
                }
            }
            foreach(var propertyInfo in propertyInfos)
            {
                if(propertyInfo.GetCustomAttribute<BinaryIgnoreAttribute>() == null)
                {
                    if(propertyInfo.GetMethod == null)
                    {
                        throw new PropertyCannotReadException();
                    }
                    if(propertyInfo.GetMethod.GetParameters().Length == 0)
                    {
                        Serializer.Serialize(propertyInfo.PropertyType, propertyInfo.GetValue(obj), stream);
                    }
                } 
            }
        }
    }
}
