using System;
using BinarySerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializerTest
{
    enum FiveElements
    {
        Gold = 4,
        Wood = 1,
        Water = 2,
        Fire = 5,
        Earth = 3
    }

    class TwoElements
    {
        public FiveElements Left;
        public FiveElements Right;
        public int Combine()
        {
            return (int)Left * 10 + (int)Right;
        }
    }

    [TestClass]
    public class EnumTest
    {
        [TestMethod]
        public void TestEnum()
        {
            FiveElements elements = FiveElements.Wood;
            FiveElements elements2 = Serializer.Deserialize<FiveElements>(Serializer.Serialize(elements));
            Assert.AreEqual(elements, elements2);


            TwoElements twoElements = new TwoElements() { Left = FiveElements.Fire, Right = FiveElements.Water };
            byte[] bytes = Serializer.Serialize(twoElements);
            TwoElements twoElements2 = Serializer.Deserialize<TwoElements>(bytes);

            Assert.AreEqual(twoElements.Combine(), twoElements2.Combine());

        }
    }
}
