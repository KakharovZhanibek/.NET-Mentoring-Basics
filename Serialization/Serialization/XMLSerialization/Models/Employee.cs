using System.Xml.Serialization;

namespace XMLSerialization.Models;

[XmlType("Employee")]
public class Employee
{
    /// <summary>Serialized as an XML attribute instead of a child element.</summary>
    [XmlAttribute("name")]
    public string EmployeeName { get; set; } = string.Empty;

    public override string ToString() => $"    Employee: {EmployeeName}";
}
