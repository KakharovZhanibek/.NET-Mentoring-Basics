namespace FileSystemVisitorConsoleApp.Events;

/// <summary>
/// Types of file system visitor events.
/// </summary>
public enum FileSystemEventType
{
    /// <summary>
    /// Raised when the search starts.
    /// </summary>
    Start,
    
    /// <summary>
    /// Raised when the search finishes.
    /// </summary>
    Finish,
    
    /// <summary>
    /// Raised when a file is found (before filtering).
    /// </summary>
    FileFound,
    
    /// <summary>
    /// Raised when a directory is found (before filtering).
    /// </summary>
    DirectoryFound,
    
    /// <summary>
    /// Raised when a file passes the filter.
    /// </summary>
    FilteredFileFound,
    
    /// <summary>
    /// Raised when a directory passes the filter.
    /// </summary>
    FilteredDirectoryFound
}
