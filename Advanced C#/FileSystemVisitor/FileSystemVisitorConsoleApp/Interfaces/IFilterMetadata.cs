namespace FileSystemVisitorConsoleApp.Interfaces;

/// <summary>
/// Interface for filters that provide metadata about themselves.
/// </summary>
public interface IFilterMetadata
{
    /// <summary>
    /// Gets the display name of the filter.
    /// </summary>
    static abstract string FilterName { get; }
    
    /// <summary>
    /// Gets the description of what the filter does.
    /// </summary>
    static abstract string FilterDescription { get; }
    
    /// <summary>
    /// Prompts the user for input and creates a configured filter instance.
    /// Each filter implements its own custom logic for collecting parameters.
    /// </summary>
    /// <returns>A configured filter instance, or null if user cancels or provides invalid input.</returns>
    static abstract IFileSystemFilter? CreateFromUserInput();
}
