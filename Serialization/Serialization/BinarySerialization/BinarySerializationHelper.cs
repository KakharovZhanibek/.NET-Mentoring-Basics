using BinarySerialization.Models;

namespace BinarySerialization;

/// <summary>
/// Provides manual binary serialization using <see cref="BinaryWriter"/> and <see cref="BinaryReader"/>.
/// BinaryFormatter was removed in .NET 9, so we write each field explicitly.
/// </summary>
public static class BinarySerializationHelper
{
    public static void Serialize(Department department, Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        writer.Write(department.DepartmentName);
        writer.Write(department.Employees.Count);

        foreach (var employee in department.Employees)
            writer.Write(employee.EmployeeName);
    }

    public static Department Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        var department = new Department
        {
            DepartmentName = reader.ReadString()
        };

        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            department.Employees.Add(new Employee
            {
                EmployeeName = reader.ReadString()
            });
        }

        return department;
    }
}
