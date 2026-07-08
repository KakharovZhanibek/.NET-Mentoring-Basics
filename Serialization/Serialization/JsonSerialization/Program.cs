// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using JsonSerialization.Models;

const string filePath = "department.json";

// ── JSON options: indented output + preserve custom property names ─────────
var options = new JsonSerializerOptions { WriteIndented = true };

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
string json = JsonSerializer.Serialize(department, options);
File.WriteAllText(filePath, json);
Console.WriteLine($"Serialized '{department.DepartmentName}' to '{filePath}'.");
Console.WriteLine("\nFile contents:");
Console.WriteLine(json);

// ── Deserialize from file ──────────────────────────────────────────────────
string jsonFromFile = File.ReadAllText(filePath);
var deserialized = JsonSerializer.Deserialize<Department>(jsonFromFile, options)!;
Console.WriteLine("\nDeserialized object:");
Console.WriteLine(deserialized);
