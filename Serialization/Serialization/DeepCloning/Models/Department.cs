using System.Text.Json.Serialization;

namespace DeepCloning.Models;

public class Department
{
    [JsonPropertyName("department_name")]
    public string DepartmentName { get; set; } = string.Empty;

    [JsonPropertyName("employees")]
    public List<Employee> Employees { get; set; } = [];

    public override string ToString()
    {
        var employees = string.Join(Environment.NewLine, Employees);
        return $"Department: {DepartmentName}{Environment.NewLine}{employees}";
    }
}
