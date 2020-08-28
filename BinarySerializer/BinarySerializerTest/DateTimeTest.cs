using System;
using BinarySerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializerTest
{
    [TestClass]
    public class DateTimeTest
    {
        [TestMethod]
        public void TestDateTime()
        {
            DateTime x = DateTime.Now;
            DateTime y = Serializer.Deserialize<DateTime>(Serializer.Serialize(x));

            Assert.AreEqual(x, y);

            x = DateTime.UtcNow;
            y = Serializer.Deserialize<DateTime>(Serializer.Serialize(x));

            Assert.AreEqual(x, y);
        }

        [TestMethod]
        public void TestTimeSpan()
        {
            TimeSpan x = TimeSpan.FromSeconds(100);
            TimeSpan y = Serializer.Deserialize<TimeSpan>(Serializer.Serialize(x));
            
            Assert.AreEqual(x, y);
        }

        [TestMethod]
        public void TestDateTimeOffset()
        {
            DateTimeOffset x = DateTime.Now;
            DateTimeOffset y = Serializer.Deserialize<DateTimeOffset>(Serializer.Serialize(x));

            Assert.AreEqual(x, y);
        }
    }
}
