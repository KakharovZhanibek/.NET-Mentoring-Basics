using FileSystemVisitorConsoleApp.Interfaces;

namespace FileSystemVisitorConsoleApp.Metadata;

/// <summary>
/// Metadata describing a filter type and how to create it from user input.
/// </summary>
public record FilterMetadata(
    string Name,
    string Description,
    Type FilterType,
    FilterParameter[] Parameters,
    Func<object?[], IFileSystemFilter> Factory)
{
    /// <summary>
    /// Creates a filter instance using the provided parameter values.
    /// </summary>
    public IFileSystemFilter CreateFilter(object?[] parameterValues)
    {
        if (parameterValues.Length != Parameters.Length)
            throw new ArgumentException($"Expected {Parameters.Length} parameters but got {parameterValues.Length}");
            
        return Factory(parameterValues);
    }
}
