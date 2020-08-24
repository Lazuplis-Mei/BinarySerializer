using BinarySerializer.Converter;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer
{

    public class Serializer
    {
        static readonly List<Type> _converters = new List<Type>();

        public static bool OnlyPublicMember;
        public static bool AddAssemblyWhenSerializing;
        public static Encoding DefaultEncoding;

        public static readonly List<Module> Modules = new List<Module>();

        public static BindingFlags BindingFlags => BindingFlags.Instance | BindingFlags.Public |
            (OnlyPublicMember ? BindingFlags.Default : BindingFlags.NonPublic);

        public static void AddConverter<T>()where T : BinaryConverter
        {
            if(!_converters.Contains(typeof(T)))
            {
                _converters.Insert(0, typeof(T));
            }
        }

        public static void RemoveConverter<T>() where T : BinaryConverter
        {
            _converters.Remove(typeof(T));
        }

        public static void AddAssembly(Assembly assembly)
        {
            Module module = assembly.Modules.First();
            if(!Modules.Contains(module))
            {
                Modules.Insert(0, module);
            }
        }

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
            AddConverter<TimeSpanConverter>();
            AddConverter<EnocdingConverter>();

            AddAssembly(typeof(int).Assembly);
        }

        public static byte[] Serialize(object obj)
        {
            if(obj == null)
            {
                throw new ArgumentNullException();
            }
            MemoryStream stream = new MemoryStream();
            Serialize(obj, stream);
            return stream.ToArray();
        }
        public static byte[] Serialize<T>(T obj)
        {
            MemoryStream stream = new MemoryStream();
            Serialize(obj, stream);
            return stream.ToArray();
        }

        public static void Serialize(object obj, Stream stream)
        {
            if(obj == null)
            {
                throw new ArgumentNullException();
            }
            Serialize(obj.GetType(), obj, stream);
        }


        public static void Serialize<T>(T obj, Stream stream)
        {
            Serialize(typeof(T), obj, stream);
        }

        public static void Serialize(Type type, object obj, Stream stream)
        {
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
                    return;
                }
            }
            BinaryWriter Writer = new BinaryWriter(stream);
            if(obj == null)
            {
                Writer.Write(-1);
            }
            else
            {
                int pos = (int)stream.Position;
                Writer.Write(0);
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
                Writer.Write(dpos - pos - 4);
                stream.Position = dpos;
            }
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            return Deserialize<T>(stream);
        }

        public static object Deserialize(Type type, byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            return Deserialize(type, stream);
        }

        public static T Deserialize<T>(Stream stream)
        {
            return (T)Deserialize(typeof(T), stream);
        }

        public static object Deserialize(Type type, Stream stream)
        {
            foreach(var converter in _converters)
            {
                var binaryConverter = (BinaryConverter)Activator.CreateInstance(converter);
                if(binaryConverter.CanConvert(type))
                {
                    binaryConverter.ReadBytes(stream, out object result);
                    return result;
                }
            }
            BinaryReader reader = new BinaryReader(stream);
            int len = reader.ReadInt32();
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
                return reader.ReadBytes(len);
            }
            return null;
        }

    }
}
