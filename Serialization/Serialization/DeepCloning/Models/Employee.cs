using System.Text.Json.Serialization;

namespace DeepCloning.Models;

public class Employee
{
    [JsonPropertyName("employee_name")]
    public string EmployeeName { get; set; } = string.Empty;

    public override string ToString() => $"    Employee: {EmployeeName}";
}
