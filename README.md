> example

```cs
class Student
{
    public string Name;
    public int Id;
}

byte[] buffer = Serializer.Serialize(new Student() {Name = "Name001", Id = 1000 });
Student stu = Serializer.Deserialize<Student>(buffer);
//stu.Name == "Name001" && stu.Id == 1000
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
//stu.Name == "Name001" && stu.Id == default
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
//TypeNotFoundException

Serializer.AddAssembly(Assembly.GetExecutingAssembly());//AddAssembly
byte[] buffer = Serializer.Serialize(typeof(Student));
Type stuType = Serializer.Deserialize<Type>(buffer);
```

> get private member

```cs
class Student
{
    public string Name;
    private int Id;
    public Student()
    {
        //default ctor is required
    }
    public Student(string name,int id)
    {
        Name = name;
        Id = id;
    }
}

byte[] buffer = Serializer.Serialize(new Student("Name001", 1000));
Student stu = Serializer.Deserialize<Student>(buffer);
//stu.Name == Name001 && stu.Id == default

Serializer.OnlyPublicMember = false;
byte[] buffer = Serializer.Serialize(new Student("Name001", 1000));
Student stu = Serializer.Deserialize<Student>(buffer);
//stu.Name == Name001 && stu.Id == 1000
```
> add a custom converter(like EnocdingConverter)

```cs

//implement BinaryConverter<T>
class EnocdingConverter : BinaryConverter<Encoding>
{
    public override Encoding ReadBytes(Stream stream)
    {
        return Encoding.GetEncoding(stream.ReadInt32());
    }
    public override void WriteBytes(Encoding obj, Stream stream)
    {
        stream.WriteInt32(obj.CodePage);
    }
}

//AddConverter
Serializer.AddConverter<EnocdingConverter>();
```
