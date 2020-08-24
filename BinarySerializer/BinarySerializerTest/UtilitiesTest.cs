using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinarySerializer.Converter;
using System.Text;
using System.Reflection;
using System.IO;
using BinarySerializer;

namespace BinarySerializerTest
{
    [TestClass]
    public class UtilitiesTest
    {
        [TestMethod]
        public void TestBaseFrom()
        {
            Assert.IsTrue(Encoding.Default.GetType().IsOrBaseFrom<Encoding>());
        }
        [TestMethod]
        public void TestIntsXBytes()
        {
            int[] buffer = { 123, 456, 789, 101112 };
            Assert.AreEqual(buffer[2], buffer.IntsToBytes().BytesToInts()[2]);
        }

    }

}
