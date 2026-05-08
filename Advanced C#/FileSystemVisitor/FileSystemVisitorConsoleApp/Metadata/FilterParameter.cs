namespace FileSystemVisitorConsoleApp.Metadata;

/// <summary>
/// Represents a parameter required to create a filter.
/// </summary>
public record FilterParameter(
    string Name,
    Type ParameterType,
    string Prompt,
    bool IsRequired = true,
    object? DefaultValue = null);
