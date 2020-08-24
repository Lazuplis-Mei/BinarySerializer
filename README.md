> example

```cs
class Student
{
    public string Name;
    public int Id;
}

byte[] buffer = Serializer.Serialize(new Student() {Name = "Name001", Id = 1000 });
Student stu = Serializer.Deserialize<Student>(buffer);
//now stu.Name is Name001 and stu.Id is 1000
```

> ignoreattribute

```cs
class Student
{
    public string Name;
    [BinaryIgnore]
    public int Id;
}

byte[] buffer = Serializer.Serialize(new Student() {Name = "Name001", Id = 1000 });
Student stu = Serializer.Deserialize<Student>(buffer);
//now stu.Name is Name001 and stu.Id is default 0
```

> add assembly to find type

```cs
class Student
{
    public string Name;
    public int Id;
}

byte[] buffer = Serializer.Serialize(typeof(Student));
Type stuType = Serializer.Deserialize<Type>(buffer);
//this will cause a TypeNotFoundException

Serializer.AddAssembly(Assembly.GetExecutingAssembly());
byte[] buffer = Serializer.Serialize(typeof(Student));
Type stuType = Serializer.Deserialize<Type>(buffer);
//this will work
```

> get private member

```cs
class Student
{
    public string Name;
    private int Id;
    public Student()
    {
        //default ctor is require
    }
    public Student(string name,int id)
    {
        Name = name;
        Id = id;
    }
}

byte[] buffer = Serializer.Serialize(new Student("Name001", 1000));
Student stu = Serializer.Deserialize<Student>(buffer);
//now stu.Name is Name001 and stu.Id is default 0

Serializer.OnlyPublicMember = false;
byte[] buffer = Serializer.Serialize(new Student("Name001", 1000));
Student stu = Serializer.Deserialize<Student>(buffer);
//now stu.Name is Name001 and stu.Id is 1000
```
> add a custom converter(like EnocdingConverter)

```cs

//implement BinaryConverter<T> or BinaryConverter
class EnocdingConverter : BinaryConverter<Encoding>
{
    public override Encoding ReadBytes(Stream stream)
    {
        if(stream.ReadByte() != 0)
        {
            BinaryReader reader = new BinaryReader(stream);
            return Encoding.GetEncoding(reader.ReadInt32());
        }
        return null;
    }
    public override void WriteBytes(Encoding obj, Stream stream)
    {
        if(obj == null)
        {
            stream.WriteByte(0);
        }
        else
        {
            stream.WriteByte(1);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(obj.CodePage);
        }
    }
}

//add to converters
Serializer.AddConverter<EnocdingConverter>();
```
