using BinarySerializer.Converter;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer
{

    /// <summary>
    /// 提供二进制序列化的实现
    /// </summary>
    public class Serializer
    {
        static readonly List<Type> _converters = new List<Type>();

        /// <summary>
        /// 是否仅对<see langword="public"/>成员进行序列化
        /// </summary>
        public static bool OnlyPublicMember;
        /// <summary>
        /// 是否在调用<see cref="Serialize(Type, object, Stream)"/>时自动添加对应类型的程序集
        /// </summary>
        public static bool AddAssemblyWhenSerializing;
        /// <summary>
        /// 表示<see cref="StringConverter"/>序列化<see cref="string"/>时使用的编码
        /// </summary>
        public static Encoding DefaultEncoding;

        /// <summary>
        /// 加载的程序集对应的模块，用于<see cref="TypeConverter"/>反序列化类型
        /// </summary>
        public static readonly List<Module> Modules = new List<Module>();

        /// <summary>
        /// 表示当前获取成员使用的<see cref="BindingFlags"/>
        /// </summary>
        public static BindingFlags BindingFlags => BindingFlags.Instance | BindingFlags.Public |
            (OnlyPublicMember ? BindingFlags.Default : BindingFlags.NonPublic);

        /// <summary>
        /// 添加一个自定义的<see cref="BinaryConverter"/>，用于转换特定的类型
        /// </summary>
        /// <typeparam name="T">表示一个派生自<see cref="BinaryConverter"/>的类型</typeparam>
        public static void AddConverter<T>()where T : BinaryConverter
        {
            if(!_converters.Contains(typeof(T)))
            {
                _converters.Insert(0, typeof(T));
            }
        }

        /// <summary>
        /// 移除一个自定义的<see cref="BinaryConverter"/>，如果确实包含
        /// </summary>
        /// <typeparam name="T">表示一个派生自<see cref="BinaryConverter"/>的类型</typeparam>
        public static void RemoveConverter<T>() where T : BinaryConverter
        {
            _converters.Remove(typeof(T));
        }

        /// <summary>
        /// 添加对应类型的程序集，用于<see cref="TypeConverter"/>反序列化类型
        /// </summary>
        /// <param name="assembly">程序集</param>
        public static void AddAssembly(Assembly assembly)
        {
            Module module = assembly.Modules.First();
            if(!Modules.Contains(module))
            {
                Modules.Insert(0, module);
            }
        }


        /// <summary>
        /// 移除指定的程序集
        /// </summary>
        /// <param name="assembly">程序集</param>
        public static void RemoveAssembly(Assembly assembly)
        {
            Modules.Remove(assembly.Modules.First());
        }


        static Serializer()
        {
            OnlyPublicMember = true;
            DefaultEncoding = Encoding.Default;

            AddConverter<ElementTypeConverter>();
            AddConverter<NullableConverter>();
            AddConverter<TypeConverter>();
            AddConverter<StringConverter>();
            AddConverter<ArrayConverter>();
            AddConverter<DictionaryConverter>();
            AddConverter<KeyValuePairConverter>();
            AddConverter<DateTimeConverter>();
            AddConverter<DateTimeOffsetConverter>();
            AddConverter<TimeSpanConverter>();
            AddConverter<EnocdingConverter>();
            AddConverter<HashSetConverter>();

            AddAssembly(typeof(int).Assembly);
        }

        /// <summary>
        /// 序列化一个对象，仅当类型不为<see langword="null"/>时，类型自动获取
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="compress">是否进行压缩</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PropertyCannotReadException"/>
        /// <returns>字节数组</returns>
        public static byte[] Serialize(object obj, bool compress = false)
        {
            if(obj == null)
            {
                throw new ArgumentNullException();
            }
            MemoryStream stream = new MemoryStream();
            Serialize(obj, stream);
            if(compress)
            {
                var memory = new MemoryStream();
                GZipStream zipStream = new GZipStream(memory, CompressionMode.Compress, true);
                zipStream.Write(stream.ToArray(), 0, (int)stream.Length);
                zipStream.Close();
                return memory.ToArray();
            }
            return stream.ToArray();
        }
        /// <summary>
        /// 序列化一个指定类型的对象，<see cref="Nullable{T}"/>类型需要显式类型参数
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="compress">是否进行压缩</param>
        /// <exception cref="PropertyCannotReadException"/>
        /// <returns>字节数组</returns>
        public static byte[] Serialize<T>(T obj, bool compress = false)
        {
            MemoryStream stream = new MemoryStream();
            Serialize(obj, stream);
            if(compress)
            {
                var memory = new MemoryStream();
                GZipStream zipStream = new GZipStream(memory, CompressionMode.Compress, true);
                zipStream.Write(stream.ToArray(), 0, (int)stream.Length);
                zipStream.Close();
                return memory.ToArray();
            }
            return stream.ToArray();
        }

        /// <summary>
        /// 序列化一个对象，仅当类型不为<see langword="null"/>时，类型自动获取
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="stream">输出流</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PropertyCannotReadException"/>
        public static void Serialize(object obj, Stream stream)
        {
            if(obj == null)
            {
                throw new ArgumentNullException();
            }
            Serialize(obj.GetType(), obj, stream);
        }

        /// <summary>
        /// 序列化一个指定类型的对象，<see cref="Nullable{T}"/>类型需要显式类型参数
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="stream">输出流</param>
        /// <exception cref="PropertyCannotReadException"/>
        public static void Serialize<T>(T obj, Stream stream)
        {
            Serialize(typeof(T), obj, stream);
        }

        /// <summary>
        /// 序列化一个指定类型的对象
        /// </summary>
        /// <param name="type">要序列化的对象类型</param>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="stream">输出流</param>
        /// <exception cref="PropertyCannotReadException"/>
        public static void Serialize(Type type, object obj, Stream stream)
        {
            if(AddAssemblyWhenSerializing)
            {
                AddAssembly(type.Assembly);
            }

            if(type.GetInterfaces().Contains(typeof(IBinarySerializable)))
            {
                var binarySerializable = (IBinarySerializable)obj;
                binarySerializable.Serialize(stream);
                return;
            }
            foreach(var converter in _converters)
            {
                var binaryConverter = (BinaryConverter)Activator.CreateInstance(converter);
                if(binaryConverter.CanConvert(type))
                {
                    binaryConverter.WriteBytes(obj, stream);
                    return;
                }
            }
            if(obj == null)
            {
                stream.WriteInt32(-1);
            }
            else
            {
                int pos = (int)stream.Position;
                stream.WriteInt32(0);
                if(type.BaseType != null)
                {
                    ObjectConverter.WriteBytes(type, obj, stream);
                }
                else
                {
                    type = obj.GetType();
                    if(AddAssemblyWhenSerializing)
                    {
                        AddAssembly(type.Assembly);
                    }
                    foreach(var converter in _converters)
                    {
                        var binaryConverter = (BinaryConverter)Activator.CreateInstance(converter);
                        if(binaryConverter.CanConvert(type))
                        {
                            binaryConverter.WriteBytes(obj, stream);
                            break;
                        }
                    }
                }
                int dpos = (int)stream.Position;
                stream.Position = pos;
                stream.WriteInt32(dpos - pos - 4);
                stream.Position = dpos;
            }
        }

        /// <summary>
        /// 反序列化为指定类型的对象
        /// </summary>
        /// <typeparam name="T">反序列化的结果类型</typeparam>
        /// <param name="bytes">字节数组</param>
        /// <param name="decompress">是否解压缩</param>
        /// <exception cref="TypeNotFoundException"/>
        public static T Deserialize<T>(byte[] bytes, bool decompress = false)
        {
            MemoryStream stream = new MemoryStream(bytes);
            if(decompress)
            {
                GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress);
                var memory = new MemoryStream();
                zipStream.CopyTo(memory);
                zipStream.Close();
                memory.Position = 0;
                return Deserialize<T>(memory);
            }
            return Deserialize<T>(stream);
        }

        /// <summary>
        /// 反序列化为指定类型的对象
        /// </summary>
        /// <param name="type">反序列化的结果类型</param>
        /// <param name="bytes">字节数组</param>
        /// <param name="decompress">是否解压缩</param>
        /// <exception cref="TypeNotFoundException"/>
        public static object Deserialize(Type type, byte[] bytes, bool decompress = false)
        {
            MemoryStream stream = new MemoryStream(bytes);
            if(decompress)
            {
                GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress);
                var memory = new MemoryStream();
                zipStream.CopyTo(memory);
                zipStream.Close();
                memory.Position = 0;
                return Deserialize(type, memory);
            }
            return Deserialize(type, stream);
        }

        /// <summary>
        /// 反序列化为指定类型的对象
        /// </summary>
        /// <typeparam name="T">反序列化的结果类型</typeparam>
        /// <param name="stream">输入流</param>
        /// <exception cref="TypeNotFoundException"/>
        public static T Deserialize<T>(Stream stream)
        {
            return (T)Deserialize(typeof(T), stream);
        }

        /// <summary>
        /// 反序列化为指定类型的对象
        /// </summary>
        /// <param name="type">反序列化的结果类型</param>
        /// <param name="stream">输入流</param>
        /// <exception cref="TypeNotFoundException"/>
        public static object Deserialize(Type type, Stream stream)
        {
            if(type.GetInterfaces().Contains(typeof(IBinarySerializable)))
            {
                var binarySerializable = (IBinarySerializable)ObjectConverter.CreateInstance(type);
                return binarySerializable.Deserialize(stream);
            }
            foreach(var converter in _converters)
            {
                var binaryConverter = (BinaryConverter)Activator.CreateInstance(converter);
                if(binaryConverter.CanConvert(type))
                {
                    binaryConverter.ReadBytes(stream, out object result);
                    return result;
                }
            }
            int len = stream.ReadInt32();
            if(len != -1)
            {
                if(type.BaseType != null)
                {
                    return ObjectConverter.ReadBytes(type, stream);
                }
                if(len == 0)
                {
                    return new object();
                }
                return stream.ReadBytes(len);
            }
            return null;
        }

    }
}
