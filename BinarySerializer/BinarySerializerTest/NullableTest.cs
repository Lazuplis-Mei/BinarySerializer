using System;
using System.Reflection;
using BinarySerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializerTest
{

    class TestNullableClass
    {
        public double? X;
        public int? Y;
    }

    [TestClass]
    public class NullableTest
    {
        [TestMethod]
        public void TestNullable()
        {
            int? x = 10;
            int? y = Serializer.Deserialize<int?>(Serializer.Serialize(x));

            Assert.AreEqual(x, y);

            Assert.AreEqual(null, Serializer.Deserialize<int?>(Serializer.Serialize<int?>(null)));
        }

        [TestMethod]
        public void TestNullable2()
        {
            var testNullableClass = new TestNullableClass() { X = 2.5};
            var testNullableClass2 = Serializer.Deserialize<TestNullableClass>(Serializer.Serialize(testNullableClass));

            Assert.AreEqual(testNullableClass.X, testNullableClass2.X);
            Assert.AreEqual(testNullableClass.Y, testNullableClass2.Y);
        }

    }
}
