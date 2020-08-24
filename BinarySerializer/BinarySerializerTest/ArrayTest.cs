using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializerTest
{
    [TestClass]
    public class ArrayTest
    {
        [TestMethod]
        public void TestArray()
        {
            int[] arr = { 1, 2, 3, 4, 5 };
            int[] arr2 = Serializer.Deserialize<int[]>(Serializer.Serialize(arr));
            Assert.AreEqual(arr.Length, arr2.Length);
            for(int i = 0; i < arr.Length; i++)
            {
                Assert.AreEqual(arr[i], arr2[i]);
            }

            arr = null;
            arr2 = Serializer.Deserialize<int[]>(Serializer.Serialize(arr));
            Assert.IsNull(arr2);
        }

        [TestMethod]
        public void TestList()
        {
            List<int> list = new List<int> { 1, 2, 3, 4, 5 };
            List<int> list2 = Serializer.Deserialize<List<int>>(Serializer.Serialize(list));
            Assert.AreEqual(list.Count, list2.Count);
            for(int i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(list[i], list2[i]);
            }
        }

        [TestMethod]
        public void TestIEnumerable()
        {
            IEnumerable<int> enumeralbe1 = new List<int> { 1, 2, 3, 4, 5 };
            var enumeralbe2 = Serializer.Deserialize<IEnumerable<int>>(Serializer.Serialize(enumeralbe1));
            Assert.AreEqual(enumeralbe1.Count(), enumeralbe2.Count());
            for(int i = 0; i < enumeralbe1.Count(); i++)
            {
                Assert.AreEqual(enumeralbe1.ElementAt(i), enumeralbe2.ElementAt(i));
            }
            Assert.AreEqual(typeof(List<int>), enumeralbe2.GetType());

        }
    }
}
