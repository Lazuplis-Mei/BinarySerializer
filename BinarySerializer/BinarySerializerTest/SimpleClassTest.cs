using System;
using BinarySerializer;
using BinarySerializer.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializerTest
{
    class SimpleClass
    {
        public int X;
        public long Y;
    }


    [TestClass]
    public class SimpleClassTest
    {
        [TestMethod]
        public void TestSimpleClass()
        {
            SimpleClass testClass = new SimpleClass() { X = 10, Y = 100 };
            byte[] bytes = Serializer.Serialize(testClass);
            SimpleClass testClass1 = Serializer.Deserialize<SimpleClass>(bytes);

            Assert.AreEqual(testClass.X, testClass1.X);
            Assert.AreEqual(testClass.Y, testClass1.Y);
        }

        [TestMethod]
        public void TestSimpleClass2()
        {
            Student stu = new Student("Name001", 1000);
            Student stu2 = Serializer.Deserialize<Student>(Serializer.Serialize(stu));

            Assert.AreEqual(stu.Name, stu2.Name);
            Assert.AreEqual(stu.Id, stu2.Id);
        }
    }

    class Student
    {
        public string Name;
        public int Id;
        public Student()
        {
            
        }
        public Student(string name, int id)
        {
            Name = name;
            Id = id;
        }
    }

}
