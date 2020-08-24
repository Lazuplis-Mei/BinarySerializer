using System;
using System.Collections.Generic;
using System.Reflection;
using BinarySerializer;
using BinarySerializer.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializerTest
{

    class TypeTestClass
    {

    }

    [TestClass]
    public class TypeTest
    {
        [TestMethod]
        public void TestType()
        {
            Type type = typeof(int);
            Type type1 = Serializer.Deserialize<Type>(Serializer.Serialize(type));
            Assert.AreEqual(type, type1);
        }
        [TestMethod]
        public void TestType1()
        {
            Type type = null;
            Type type1 = Serializer.Deserialize<Type>(Serializer.Serialize(type));
            Assert.IsNull(type1);
        }

        [TestMethod]
        public void TestType2()
        {
            Assert.ThrowsException<TypeNotFoundException>(() => Serializer.Deserialize<Type>(Serializer.Serialize(typeof(TypeTestClass))));
        }

        [TestMethod]
        public void TestType3()
        {
            Type type = typeof(TypeTestClass);
            Serializer.AddAssembly(Assembly.GetExecutingAssembly());
            Type type1 = Serializer.Deserialize<Type>(Serializer.Serialize(type));
            Assert.AreEqual(type, type1);
        }

        [TestMethod]
        public void TestType4()
        {
            Serializer.AddAssemblyWhenSerializing = true;
            Type type = typeof(TypeTestClass);
            Type type1 = Serializer.Deserialize<Type>(Serializer.Serialize(type));
            Assert.AreEqual(type, type1);
            Serializer.AddAssemblyWhenSerializing = false;
        }

        [TestMethod]
        public void TestType5()
        {
            Type type = typeof(KeyValuePair<int, string>);
            Type type1 = Serializer.Deserialize<Type>(Serializer.Serialize(type));
            Assert.AreEqual(type, type1);
        }

    }
}
