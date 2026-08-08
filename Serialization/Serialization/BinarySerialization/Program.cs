using BinarySerialization;
using BinarySerialization.Models;

const string filePath = "department.bin";

// ── Build a sample Department ──────────────────────────────────────────────
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

// ── Serialize to file ──────────────────────────────────────────────────────
using (var writeStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
{
    BinarySerializationHelper.Serialize(department, writeStream);
}
Console.WriteLine($"Serialized '{department.DepartmentName}' to '{filePath}'.");

// ── Deserialize from file ──────────────────────────────────────────────────
using (var readStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
{
    var deserialized = BinarySerializationHelper.Deserialize(readStream);
    Console.WriteLine("Deserialized object:");
    Console.WriteLine(deserialized);
}
