using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializerTest
{

    class TestGenericClass<T>
    {
        public T X;
    }


    [TestClass]
    public class GenericTest
    {
        [TestMethod]
        public void TestGeneric()
        {

            var test = new TestGenericClass<int>();
            test.X = 100;
            var test2 = Serializer.Deserialize<TestGenericClass<int>>(Serializer.Serialize(test));
            Assert.AreEqual(100, test2.X);
        }

        [TestMethod]
        public void TestGeneric2()
        {
            var pair = new KeyValuePair<int, string>(1, "one");
            var pair2 = Serializer.Deserialize<KeyValuePair<int, string>>(Serializer.Serialize(pair));
            Assert.AreEqual(1, pair2.Key);
            Assert.AreEqual("one", pair2.Value);
        }
        [TestMethod]
        public void TestGeneric3()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");
            dict.Add(3, "three");
            var dict2 = Serializer.Deserialize<Dictionary<int, string>>(Serializer.Serialize(dict));

            Assert.AreEqual(dict.Count, dict2.Count);
            for(int i = 0; i < dict.Keys.Count; i++)
            {
                var key = dict.Keys.ToArray()[i];
                var key2 = dict2.Keys.ToArray()[i];
                Assert.AreEqual(key, key2);
                Assert.AreEqual(dict[key], dict2[key2]);
            }

        }
    }
}
