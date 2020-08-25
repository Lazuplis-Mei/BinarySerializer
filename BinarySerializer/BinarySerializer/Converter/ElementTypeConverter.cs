using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Converter
{
    /// <summary>
    /// 基础类型枚举
    /// </summary>
    public enum ElementType
    {
        NotElementType = -1,
        Boolean,
        Char,
        Int16,
        Int32,
        Int64,
        UInt16,
        UInt32,
        UInt64,
        Single,
        Double,
        Byte,
        SByte,
        Decimal
    }
    /// <summary>
    /// 基础类型转换器
    /// </summary>
    public class ElementTypeConverter : BinaryConverter
    {

        public static List<Type> ElementTypes = new List<Type>
        {
             typeof(bool),
             typeof(char),
             typeof(short),
             typeof(int),
             typeof(long),
             typeof(ushort),
             typeof(uint),
             typeof(ulong),
             typeof(float),
             typeof(double),
             typeof(byte),
             typeof(sbyte),
             typeof(decimal)
        };
        public static List<int> ElementTypesSize = new List<int>
        {
             sizeof(bool),
             sizeof(char),
             sizeof(short),
             sizeof(int),
             sizeof(long),
             sizeof(ushort),
             sizeof(uint),
             sizeof(ulong),
             sizeof(float),
             sizeof(double),
             sizeof(byte),
             sizeof(sbyte),
             sizeof(decimal)
        };
        public ElementType ElementType;
        public int SizeOfType;
        public override bool CanConvert(Type type)
        {
            ElementType = (ElementType)ElementTypes.IndexOf(type);
            if(ElementType != ElementType.NotElementType)
            {
                SizeOfType = ElementTypesSize[(int)ElementType];
                return true;
            }
            return false;
        }
        public override void ReadBytes(Stream stream, out object obj)
        {
            byte[] buffer = new byte[ElementTypesSize.Max()];
            stream.Read(buffer, 0, SizeOfType);
            switch(ElementType)
            {
                case ElementType.Boolean:
                    obj = BitConverter.ToBoolean(buffer, 0);
                    break;
                case ElementType.Char:
                    obj = BitConverter.ToChar(buffer, 0);
                    break;
                case ElementType.Int16:
                    obj = BitConverter.ToInt16(buffer, 0);
                    break;
                case ElementType.Int32:
                    obj = BitConverter.ToInt32(buffer, 0);
                    break;
                case ElementType.Int64:
                    obj = BitConverter.ToInt64(buffer, 0);
                    break;
                case ElementType.UInt16:
                    obj = BitConverter.ToUInt16(buffer, 0);
                    break;
                case ElementType.UInt32:
                    obj = BitConverter.ToUInt32(buffer, 0);
                    break;
                case ElementType.UInt64:
                    obj = BitConverter.ToUInt64(buffer, 0);
                    break;
                case ElementType.Single:
                    obj = BitConverter.ToSingle(buffer, 0);
                    break;
                case ElementType.Double:
                    obj = BitConverter.ToDouble(buffer, 0);
                    break;
                case ElementType.Byte:
                    obj = buffer[0];
                    break;
                case ElementType.SByte:
                    obj = (sbyte)buffer[0];
                    break;
                case ElementType.Decimal:
                    obj = new decimal(buffer.BytesToInts());
                    break;
                default:
                    throw new InvalidCastException();
            }
        }
        public override void WriteBytes(object obj, Stream stream)
        {
            byte[] buffer = null;
            switch(ElementType)
            {
                case ElementType.Boolean:
                    buffer = BitConverter.GetBytes((bool)obj);
                    break;
                case ElementType.Char:
                    buffer = BitConverter.GetBytes((char)obj);
                    break;
                case ElementType.Int16:
                    buffer = BitConverter.GetBytes((short)obj);
                    break;
                case ElementType.Int32:
                    buffer = BitConverter.GetBytes((int)obj);
                    break;
                case ElementType.Int64:
                    buffer = BitConverter.GetBytes((long)obj);
                    break;
                case ElementType.UInt16:
                    buffer = BitConverter.GetBytes((ushort)obj);
                    break;
                case ElementType.UInt32:
                    buffer = BitConverter.GetBytes((uint)obj);
                    break;
                case ElementType.UInt64:
                    buffer = BitConverter.GetBytes((ulong)obj);
                    break;
                case ElementType.Single:
                    buffer = BitConverter.GetBytes((float)obj);
                    break;
                case ElementType.Double:
                    buffer = BitConverter.GetBytes((double)obj);
                    break;
                case ElementType.Byte:
                    buffer = new byte[] { (byte)obj };
                    break;
                case ElementType.SByte:
                    buffer = new byte[] { (byte)(sbyte)obj };
                    break;
                case ElementType.Decimal:
                    buffer = decimal.GetBits(((decimal)obj)).IntsToBytes();
                    break;
            }
            stream.Write(buffer, 0, SizeOfType);
        }
    }
}
