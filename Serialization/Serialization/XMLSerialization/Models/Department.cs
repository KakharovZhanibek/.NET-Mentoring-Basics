using System.Xml.Serialization;

namespace XMLSerialization.Models;

[XmlRoot("Department")]
public class Department
{
    /// <summary>Serialized as an XML attribute instead of a child element.</summary>
    [XmlAttribute("name")]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>Each employee is wrapped in an &lt;Employees&gt; element.</summary>
    [XmlArray("Employees")]
    [XmlArrayItem("Employee")]
    public List<Employee> Employees { get; set; } = [];

    public override string ToString()
    {
        var employees = string.Join(Environment.NewLine, Employees);
        return $"Department: {DepartmentName}{Environment.NewLine}{employees}";
    }
}
