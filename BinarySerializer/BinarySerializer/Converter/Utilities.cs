using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Converter
{
    public static class Utilities
    {
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
