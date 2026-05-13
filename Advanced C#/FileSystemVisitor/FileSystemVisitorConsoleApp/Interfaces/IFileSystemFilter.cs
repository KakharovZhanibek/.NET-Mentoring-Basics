namespace FileSystemVisitorConsoleApp.Interfaces;

/// <summary>
/// Defines a contract for file system filtering logic.
/// </summary>
public interface IFileSystemFilter
{
    /// <summary>
    /// Determines whether the specified file system item passes the filter.
    /// </summary>
    /// <param name="info">The file system item information.</param>
    /// <returns>True if the item passes the filter; otherwise, false.</returns>
    bool IsMatch(FileSystemInfo info);
    
    /// <summary>
    /// Gets a description of the filter for display purposes.
    /// </summary>
    string Description { get; }
}
