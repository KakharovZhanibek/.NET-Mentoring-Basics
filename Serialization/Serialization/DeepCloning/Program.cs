using DeepCloning;
using DeepCloning.Models;

// ── Build the original Department ─────────────────────────────────────────
var original = new Department
{
    DepartmentName = "Engineering",
    Employees =
    [
        new Employee { EmployeeName = "Alice Johnson" },
        new Employee { EmployeeName = "Bob Smith" },
        new Employee { EmployeeName = "Carol White" }
    ]
};

Console.WriteLine("=== Original ===");
Console.WriteLine(original);

// ── Deep Clone ────────────────────────────────────────────────────────────
Department clone = original.DeepClone();

Console.WriteLine("\n=== Clone (before modifications) ===");
Console.WriteLine(clone);

// ── Prove independence: mutate the clone, original must stay unchanged ─────
clone.DepartmentName = "Research & Development";
clone.Employees[0].EmployeeName = "Diana Prince";
clone.Employees.Add(new Employee { EmployeeName = "Eve Torres" });

Console.WriteLine("\n=== Clone (after modifications) ===");
Console.WriteLine(clone);

Console.WriteLine("\n=== Original (must be unchanged) ===");
Console.WriteLine(original);

// ── Reference-equality checks ─────────────────────────────────────────────
Console.WriteLine("\n=== Independence checks ===");
Console.WriteLine($"Same Department reference  : {ReferenceEquals(original, clone)}");
Console.WriteLine($"Same Employees list ref    : {ReferenceEquals(original.Employees, clone.Employees)}");
Console.WriteLine($"Same Employee[0] reference : {ReferenceEquals(original.Employees[0], clone.Employees[0])}");
Console.WriteLine($"Original name unchanged    : {original.DepartmentName == "Engineering"}");
Console.WriteLine($"Original employee count    : {original.Employees.Count} (expected 3)");
