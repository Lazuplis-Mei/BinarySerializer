using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static BinarySerializer.Serializer;

namespace BinarySerializer.Converter
{
    /// <summary>
    /// 属性不可读
    /// </summary>
    public class PropertyCannotReadException : Exception
    {

    }

    /// <summary>
    /// 找不到指定类型可调用的构造器
    /// </summary>
    public class ConstructorNotFoundException : Exception
    {

    }

    /// <summary>
    /// 该特性可用于修饰字段和属性，指示它们不被序列化
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BinaryIgnoreAttribute : Attribute
    {

    }

    /// <summary>
    /// 对象转换器
    /// </summary>
    public class ObjectConverter
    {
        public static T ReadBytes<T>(Stream stream)
        {
            return (T)ReadBytes(typeof(T), stream);
        }

        public static object ReadBytes(Type type, Stream stream)
        {
            object obj = CreateInstance(type);
            var fieldInfos = type.GetFields(Serializer.BindingFlags).OrderBy(f => f.Name);
            var propertyInfos = type.GetProperties(Serializer.BindingFlags).OrderBy(f => f.Name);
            foreach(var field in fieldInfos)
            {
                if(field.GetCustomAttribute<BinaryIgnoreAttribute>() == null)
                {
                    field.SetValue(obj, Deserialize(field.FieldType, stream));
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
                            object value = Deserialize(propertyInfo.PropertyType, stream);

                            propertyInfo.SetValue(obj, value);
                        }
                    }
                }
            }
            return obj;
        }

        public static object CreateInstance(Type type)
        {
            if(type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            object obj;
            var constructor = type.GetConstructor(new Type[0]);
            if(constructor == null)
            {

                var constructors = type.GetConstructors(Serializer.BindingFlags | 
                    System.Reflection.BindingFlags.NonPublic).Where(
                    c => c.GetCustomAttribute<BinaryConstructorAttribute>() != null);
                if(constructors.Count() == 0)
                {
                    throw new ConstructorNotFoundException();
                }
                constructor = constructors.First();
                var binaryConstructorAttribute = constructor.GetCustomAttribute<BinaryConstructorAttribute>();
                var paramInfos = constructor.GetParameters();
                List<object> args = new List<object>();
                for(int i = 0; i < paramInfos.Length; i++)
                {
                    if(i < binaryConstructorAttribute.Args.Length)
                    {
                        args.Add(binaryConstructorAttribute.Args[i]);
                    }
                    else
                    {
                        var paramtype = paramInfos[i].ParameterType;
                        if(paramtype.IsValueType)
                        {
                            args.Add(Activator.CreateInstance(paramtype));
                        }
                        else
                        {
                            args.Add(null);
                        }
                    }
                }
                obj = constructor.Invoke(args.ToArray());
            }
            else
            {
                obj = constructor.Invoke(new object[0]);
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
                    Serialize(field.FieldType, field.GetValue(obj), stream);
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
                        Serialize(propertyInfo.PropertyType, propertyInfo.GetValue(obj), stream);
                    }
                } 
            }
        }
    }
}
