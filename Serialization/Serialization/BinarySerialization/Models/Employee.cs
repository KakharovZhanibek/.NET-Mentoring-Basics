namespace BinarySerialization.Models;

public class Employee
{
    public string EmployeeName { get; set; } = string.Empty;

    public override string ToString() => $"    Employee: {EmployeeName}";
}
