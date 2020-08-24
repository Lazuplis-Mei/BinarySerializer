using System;
using BinarySerializer;
using BinarySerializer.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializerTest
{
    class TestMemberReadWriteClass
    {
        public int X;
        private int x;
        public readonly int Y;
        private readonly int y;
        public int PropX { get; set; }
        private int propX { get; set; }


        public int PropY { get; private set; }
        private int propY { get; set; }

    }


    [TestClass]
    public class MemberReadWriteTest
    {
        [TestMethod]
        public void TestMemberReadWriteOnlyPublic()
        {
            Serializer.OnlyPublicMember = true;
            var testclass = new TestMemberReadWriteClass();
            testclass.X = 1;
            testclass.StrongSetValue("Y", 11);
            testclass.PropX = 111;
            testclass.StrongSetValue("PropY", 1111);
            testclass.StrongSetValue("x", 2);
            testclass.StrongSetValue("y", 22);
            testclass.StrongSetValue("propX", 222);
            testclass.StrongSetValue("propY", 2222);


            var testclass2 = Serializer.Deserialize<TestMemberReadWriteClass>(Serializer.Serialize(testclass));

            Assert.AreEqual(testclass.X, testclass2.X);
            Assert.AreEqual(testclass.Y, testclass2.Y);
            Assert.AreEqual(testclass.PropX, testclass2.PropX);
            Assert.AreEqual(testclass.PropY, testclass2.PropY);

            Assert.AreEqual(0, testclass2.StrongGetValue<int>("x"));
            Assert.AreEqual(0, testclass2.StrongGetValue<int>("y"));
            Assert.AreEqual(0, testclass2.StrongGetValue<int>("propX"));
            Assert.AreEqual(0, testclass2.StrongGetValue<int>("propY"));

        }

        [TestMethod]
        public void TestMemberReadWriteAll()
        {
            Serializer.OnlyPublicMember = false;
            var testclass = new TestMemberReadWriteClass();
            testclass.X = 1;
            testclass.StrongSetValue("Y", 11);
            testclass.PropX = 111;
            testclass.StrongSetValue("PropY", 1111);
            testclass.StrongSetValue("x", 2);
            testclass.StrongSetValue("y", 22);
            testclass.StrongSetValue("propX", 222);
            testclass.StrongSetValue("propY", 2222);


            var testclass2 = Serializer.Deserialize<TestMemberReadWriteClass>(Serializer.Serialize(testclass));

            Assert.AreEqual(testclass.X, testclass2.X);
            Assert.AreEqual(testclass.Y, testclass2.Y);
            Assert.AreEqual(testclass.PropX, testclass2.PropX);
            Assert.AreEqual(testclass.PropY, testclass2.PropY);

            Assert.AreEqual(testclass.StrongGetValue<int>("x"), testclass2.StrongGetValue<int>("x"));
            Assert.AreEqual(testclass.StrongGetValue<int>("y"), testclass2.StrongGetValue<int>("y"));
            Assert.AreEqual(testclass.StrongGetValue<int>("propX"), testclass2.StrongGetValue<int>("propX"));
            Assert.AreEqual(testclass.StrongGetValue<int>("propY"), testclass2.StrongGetValue<int>("propY"));

        }
    }
}
