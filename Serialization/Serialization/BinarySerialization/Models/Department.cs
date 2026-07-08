namespace BinarySerialization.Models;

public class Department
{
    public string DepartmentName { get; set; } = string.Empty;
    public List<Employee> Employees { get; set; } = [];

    public override string ToString()
    {
        var employees = string.Join(Environment.NewLine, Employees);
        return $"Department: {DepartmentName}{Environment.NewLine}{employees}";
    }
}
