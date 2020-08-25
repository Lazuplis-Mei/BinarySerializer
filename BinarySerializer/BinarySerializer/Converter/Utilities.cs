using System;
using System.Collections.Generic;
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
            Type tBase = typeof(TBase);
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

    }
}
