using FileSystemVisitorConsoleApp.Events;

namespace FileSystemVisitorConsoleApp.Interfaces;

/// <summary>
/// Defines a contract for file system traversal.
/// </summary>
public interface IFileSystemVisitor
{
    /// <summary>
    /// Gets all file system items that match the configured filters.
    /// </summary>
    /// <returns>An enumerable collection of file system item paths.</returns>
    IEnumerable<string> GetFileSystemItems();
    
    /// <summary>
    /// Event raised during file system traversal with information about what happened.
    /// </summary>
    event EventHandler<FileSystemVisitorEventArgs>? ItemProcessed;
}
