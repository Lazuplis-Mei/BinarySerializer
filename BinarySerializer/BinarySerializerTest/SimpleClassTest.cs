using System;
using System.IO;
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
        [TestMethod]
        public void TestSimpleClass3()
        {
            Person p = new Person("Name001");
            Person p2 = Serializer.Deserialize<Person>(Serializer.Serialize(p));

            Assert.AreEqual(p.Name, p2.Name);
        }
    }

    class Student
    {
        public string Name;
        public int Id;
        [BinaryConstructor]
        public Student(string name, int id)
        {
            Name = name;
            Id = id;
        }

    }

    class Person: IBinarySerializable
    {
        public string Name { get; }
        [BinaryConstructor]
        public Person(string name)
        {
            Name = name;
        }

        public void Serialize(Stream stream)
        {
            Serializer.Serialize(Name, stream);
        }
        public object Deserialize(Stream stream)
        {
            return new Person(Serializer.Deserialize<string>(stream));
        }
    }

}
