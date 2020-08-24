using BinarySerializer.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializerTest
{
    [TestClass]
    class ElementTypeConverterTest
    {
        [TestMethod]
        public void TestElementTypeConverter()
        {
            var elementTypeConverter = new ElementTypeConverter();
            Assert.IsTrue(elementTypeConverter.CanConvert(long.MaxValue.GetType()));
            Assert.IsTrue(elementTypeConverter.CanConvert(double.MaxValue.GetType()));
            Assert.IsTrue(elementTypeConverter.CanConvert(decimal.One.GetType()));

            Assert.IsFalse(elementTypeConverter.CanConvert(Encoding.Default.GetType()));
            Assert.IsFalse(elementTypeConverter.CanConvert(FieldAttributes.Public.GetType()));
            Assert.IsFalse(elementTypeConverter.CanConvert(typeof(void)));

            object pi = Math.PI;
            if(elementTypeConverter.CanConvert(pi.GetType()))
            {
                MemoryStream memory = new MemoryStream();
                elementTypeConverter.WriteBytes(pi, memory);
                memory.Seek(0, SeekOrigin.Begin);
                elementTypeConverter.ReadBytes(memory, out object pi2);
                Assert.AreEqual(pi, pi2);
            }
        }
    }
}
