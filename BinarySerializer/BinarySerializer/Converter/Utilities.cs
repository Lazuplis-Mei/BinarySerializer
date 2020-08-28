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
    /// 几个功能函数
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// 表示类型是否是<see langword="TBase"/>或继承自<see langword="TBase"/>
        /// </summary>
        /// <typeparam name="TBase">基类型</typeparam>
        /// <param name="type">类型</param>
        public static bool IsOrBaseFrom<TBase>(this Type type)
        {
            return type.IsOrBaseFrom(typeof(TBase));
        }

        /// <summary>
        /// 表示类型是否是<see langword="TBase"/>或继承自<see langword="tBase"/>
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="tBase">基类型</param>
        public static bool IsOrBaseFrom(this Type type, Type tBase)
        {
            while(type.BaseType != null)
            {
                if(type == tBase)
                {
                    return true;
                }
                type = type.BaseType;
            }
            return tBase.BaseType == null;
        }

        /// <summary>
        /// 将<see langword="byte[]"/>转换成<see langword="int[]"/>，长度不够将会在末尾补0
        /// </summary>
        /// <param name="bytes">字节数组</param>
        public static int[] BytesToInts(this byte[] bytes)
        {
            int re = 4 - bytes.Length % 4;
            byte[] fullbytes = bytes;
            if(re != 4)
            {
                fullbytes = new byte[bytes.Length + re];
                bytes.CopyTo(fullbytes, 0);
            }
            int[] bits = new int[fullbytes.Length / 4];
            for(int i = 0; i < bits.Length; i++)
            {
                bits[i] = BitConverter.ToInt32(fullbytes, i * 4);
            }
            return bits;   
        }
        /// <summary>
        /// 将<see langword="int[]"/>转换成<see langword="byte[]"/>
        /// </summary>
        /// <param name="ints">int数组</param>
        public static byte[] IntsToBytes(this int[] ints)
        {                  
            byte[] bytes = new byte[ints.Length * 4];
            for(int i = 0; i < ints.Length; i++)
            {              
                byte[] buffer = BitConverter.GetBytes(ints[i]);
                buffer.CopyTo(bytes, i * 4);
            }
            return bytes;
        }

        /// <summary>
        /// 通过反射获取<see langword="private"/>字段或属性的值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="obj">要获取值的对象</param>
        /// <param name="memberName">字段或属性的名字</param>
        /// <exception cref="PropertyCannotReadException"/>
        public static T StrongGetValue<T>(this object obj,string memberName)
        {
            Type type = obj.GetType();
            var field = type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if(field == null)
            {
                var property = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if(property.GetMethod == null)
                {
                    throw new PropertyCannotReadException();
                }
                return (T)property.GetValue(obj);
            }
            return (T)field.GetValue(obj);
        }

        /// <summary>
        /// 通过反射修改<see langword="private"/>或<see langword="readonly"/>的字段或<see langword="private"/>属性的值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="obj">要修改的对象</param>
        /// <param name="memberName">字段或属性的名字</param>
        /// <param name="value">修改的值</param>
        public static void StrongSetValue<T>(this object obj, string memberName,T value)
        {
            Type type = obj.GetType();
            var field = type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if(field == null)
            {
                var property = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if(property.SetMethod != null)
                {
                    property.SetValue(obj, value);
                }
            }
            else
            {
                field.SetValue(obj, value);
            }
        }


        public static object Invoke(this MethodInfo method, object obj, params object[] args)
        {
            return method.Invoke(obj, args);
        }

        /// <summary>
        /// 读取流中的一个整数
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static int ReadInt32(this Stream stream)
        {
            return stream.ReadByte() | (stream.ReadByte() << 8) | (stream.ReadByte() << 16) | (stream.ReadByte() << 24);
        }

        /// <summary>
        /// 读取流中的字节数组
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(this Stream stream, int len)
        {
            byte[] result = new byte[len];
            stream.Read(result, 0, len);
            return result;
        }

        /// <summary>
        /// 向流中写入字节数组
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static void WriteBytes(this Stream stream, byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 读取流中的一个长整数
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static long ReadInt64(this Stream stream)
        {
            return (uint)stream.ReadInt32() | ((long)stream.ReadInt32() << 32);
        }

        /// <summary>
        /// 向流中写入一个整数
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static void WriteInt32(this Stream stream, int num)
        {
            stream.Write(BitConverter.GetBytes(num), 0, 4);
        }

        /// <summary>
        /// 向流中写入一个长整数
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static void WriteInt64(this Stream stream, long num)
        {
            stream.Write(BitConverter.GetBytes(num), 0, 8);
        }
    }
    
}
