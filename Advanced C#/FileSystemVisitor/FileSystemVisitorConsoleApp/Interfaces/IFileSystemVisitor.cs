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
    
    // Events
    event EventHandler? Start;
    event EventHandler? Finish;
    event EventHandler<ItemEventArgs>? FileFound;
    event EventHandler<ItemEventArgs>? DirectoryFound;
    event EventHandler<ItemEventArgs>? FilteredFileFound;
    event EventHandler<ItemEventArgs>? FilteredDirectoryFound;
}
