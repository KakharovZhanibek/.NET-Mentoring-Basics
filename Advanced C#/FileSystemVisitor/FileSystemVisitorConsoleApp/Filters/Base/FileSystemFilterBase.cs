using FileSystemVisitorConsoleApp.Interfaces;

namespace FileSystemVisitorConsoleApp.Filters.Base;

/// <summary>
/// Abstract base class for file system filters.
/// </summary>
public abstract class FileSystemFilterBase : IFileSystemFilter
{
    /// <summary>
    /// Determines whether the specified file system item passes the filter.
    /// </summary>
    /// <param name="info">The file system item information.</param>
    /// <returns>True if the item passes the filter; otherwise, false.</returns>
    public abstract bool IsMatch(FileSystemInfo info);
    
    /// <summary>
    /// Gets a description of the filter for display purposes.
    /// </summary>
    public abstract string Description { get; }
}
