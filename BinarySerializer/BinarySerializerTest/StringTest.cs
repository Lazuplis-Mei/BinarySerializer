using System;
using BinarySerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializerTest
{
    [TestClass]
    public class StringTest
    {
        [TestMethod]
        public void TestString()
        {
            string str = Serializer.Deserialize<string>(Serializer.Serialize("abcdefg"));
            Assert.AreEqual("abcdefg", str);
        }

        [TestMethod]
        public void TestEmptyString()
        {
            string str = Serializer.Deserialize<string>(Serializer.Serialize(string.Empty));
            Assert.AreEqual(string.Empty, str);
        }

        [TestMethod]
        public void TestNullString()
        {
            string str = Serializer.Deserialize<string>(Serializer.Serialize<string>(null));
            Assert.AreEqual(null, str);
        }
    }
}
