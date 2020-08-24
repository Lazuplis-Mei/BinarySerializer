using System;
using System.Text;
using BinarySerializer;
using BinarySerializer.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializerTest
{
    class Test1Class1
    {
        public int X;
    }

    class Test1Class2
    {
        public Test1Class1 A;
        public Test1Class1 B;
    }


    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var test1Class2 = new Test1Class2();
            test1Class2.A = new Test1Class1() { X = 1 };
            test1Class2.B = null;

            var test1Class22 = Serializer.Deserialize<Test1Class2>(Serializer.Serialize(test1Class2));

            Assert.AreEqual(test1Class2.A.X, test1Class22.A.X);
            Assert.IsNull(test1Class22.B);
        }

        [TestMethod]
        public void TestMethod2()
        {
            object x = 100;
            byte[] y = (byte[])Serializer.Deserialize<object>(Serializer.Serialize<object>(x));

            Assert.AreEqual(4, y.Length);
            Assert.AreEqual(100, y[0]);


            x = new object();
            object z = Serializer.Deserialize<object>(Serializer.Serialize(x));//Serialize<object> is the same
            Assert.AreEqual(z.GetType(), typeof(object));
        }

        [TestMethod]
        public void TestMethod3()
        {
            var x = (1, "one");
            var y = Serializer.Deserialize<(int,string)>(Serializer.Serialize(x));
            Assert.AreEqual(x, y);
        }

        [TestMethod]
        public void TestMethod4()
        {
            var x = Encoding.UTF8;
            var y = Serializer.Deserialize<Encoding>(Serializer.Serialize(x));
            Assert.AreEqual(x, y);
        }

        [TestMethod]
        public void TestMethod5()
        {
            var x = new TestBinaryIgnore();
            x.Ignore = 100;
            x.NotIgnore = 200;

            byte[] buffer = Serializer.Serialize(x);

            var y = Serializer.Deserialize<TestBinaryIgnore>(buffer);

            Assert.AreEqual(0, y.Ignore);
            Assert.AreEqual(200, y.NotIgnore);

            Assert.AreEqual(8, buffer.Length);
        }
    }
    class TestBinaryIgnore
    {
        [BinaryIgnore]
        public int Ignore;
        public int NotIgnore;
    }
}
