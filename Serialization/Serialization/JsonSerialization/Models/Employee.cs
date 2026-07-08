using System.Text.Json.Serialization;

namespace JsonSerialization.Models;

public class Employee
{
    /// <summary>Serialized with a custom JSON property name.</summary>
    [JsonPropertyName("employee_name")]
    public string EmployeeName { get; set; } = string.Empty;

    public override string ToString() => $"    Employee: {EmployeeName}";
}
