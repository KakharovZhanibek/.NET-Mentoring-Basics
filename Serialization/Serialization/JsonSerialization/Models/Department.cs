using System.Text.Json.Serialization;

namespace JsonSerialization.Models;

public class Department
{
    /// <summary>Serialized with a custom JSON property name.</summary>
    [JsonPropertyName("department_name")]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>Null list is serialized as an empty JSON array.</summary>
    [JsonPropertyName("employees")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Employee> Employees { get; set; } = [];

    public override string ToString()
    {
        var employees = string.Join(Environment.NewLine, Employees);
        return $"Department: {DepartmentName}{Environment.NewLine}{employees}";
    }
}
