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
    /// 提供二进制对象转换器的基类
    /// </summary>
    public abstract class BinaryConverter
    {
        public abstract bool CanConvert(Type type);
        public abstract void WriteBytes(object obj, Stream stream);
        public abstract void ReadBytes(Stream stream, out object obj);
    }
    /// <summary>
    /// 提供二进制对象转换器的泛型基类
    /// </summary>
    /// <typeparam name="T">转换类型</typeparam>
    public abstract class BinaryConverter<T> : BinaryConverter
    {
        public sealed override bool CanConvert(Type type)
        {
            return type.IsOrBaseFrom<T>();
        }

        public sealed override void WriteBytes(object obj, Stream stream)
        {
            if(obj == null)
            {
                stream.WriteByte(0);
            }
            else
            {
                stream.WriteByte(1);
                WriteBytes((T)obj, stream);
            }
        }
        public sealed override void ReadBytes(Stream stream, out object obj)
        {
            obj = null;
            if(stream.ReadByte() != 0)
            {
                obj = ReadBytes(stream);
            }
        }
        public abstract void WriteBytes(T obj, Stream stream);
        public abstract T ReadBytes(Stream stream);
    }


    /// <summary>
    /// 用于指定泛型类型转换器的基类和类型参数数量
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class)]
    sealed class GenericConverterAttribute : Attribute
    {
        public int GenericTypeArgsCount { get; }
        public Type GenericType { get; }
        public GenericConverterAttribute(Type type)
        {
            if(type != null && type.IsGenericType)
            {
                GenericType = type;
                GenericTypeArgsCount = ((TypeInfo)type).GenericTypeParameters.Length;
            }
            else
            {
                throw new ArgumentException("必须为泛型类型");
            }
        }

        public bool CheckType(Type type, Type[] typeargs)
        {
            if(typeargs.Length == GenericTypeArgsCount)
            {
                try
                {
                    return GenericType.MakeGenericType(typeargs) == type;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }


    /// <summary>
    /// 提供针对泛型类型的转换器基类
    /// </summary>
    public abstract class GenericConverter : BinaryConverter
    {
        public Type GenericType { get; private set; }
        public Type[] TypeArgs { get; private set; }
        public Type CurrentType { get; private set; }
        public sealed override bool CanConvert(Type type)
        {
            var gattr = GetType().GetCustomAttribute<GenericConverterAttribute>();
            if(gattr != null)
            {
                GenericType = gattr.GenericType;
                TypeArgs = type.GenericTypeArguments;
                if(gattr.CheckType(type, TypeArgs))
                {
                    CurrentType = type;
                    return true;
                }
            }
            return false;
        }
    }
}
