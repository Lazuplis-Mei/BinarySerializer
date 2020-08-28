using System;
using BinarySerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializerTest
{
    [TestClass]
    public class CompressTest
    {
        [TestMethod]
        public void TestCompress()
        {
            string str = "aaaaaabbbbbbccccccdddddd";
            str += str;
            str += str;
            str += str;
            var bytes = Serializer.Serialize(str, true);
            string str2 = Serializer.Deserialize<string>(bytes, true);
            Assert.AreEqual(str, str2);
        }
    }
}
