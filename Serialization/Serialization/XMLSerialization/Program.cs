using System.Xml.Serialization;
using XMLSerialization.Models;

const string filePath = "department.xml";

var department = new Department
{
    DepartmentName = "Engineering",
    Employees =
    [
        new Employee { EmployeeName = "Alice Johnson" },
        new Employee { EmployeeName = "Bob Smith" },
        new Employee { EmployeeName = "Carol White" }
    ]
};

var serializer = new XmlSerializer(typeof(Department));

// Serialize to file
using (var writeStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
{
    serializer.Serialize(writeStream, department);
}

string fileContents = File.ReadAllText(filePath);
Console.WriteLine("Serialized to file: " + filePath);
Console.WriteLine();
Console.WriteLine("File contents:");
Console.WriteLine(fileContents);

// Deserialize from file
Department deserialized;
using (var readStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
{
    deserialized = (Department)serializer.Deserialize(readStream)!;
}

Console.WriteLine("Deserialized object:");
Console.WriteLine(deserialized);
