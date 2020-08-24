using System;
using BinarySerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializerTest
{
    class SimpleClass
    {
        public int X;
        public long Y;
    }


    [TestClass]
    public class SimpleClassTest
    {
        [TestMethod]
        public void TestSimpleClass()
        {
            SimpleClass testClass = new SimpleClass() { X = 10, Y = 100 };
            byte[] bytes = Serializer.Serialize(testClass);
            SimpleClass testClass1 = Serializer.Deserialize<SimpleClass>(bytes);

            Assert.AreEqual(testClass.X, testClass1.X);
            Assert.AreEqual(testClass.Y, testClass1.Y);
        }
    }
}
