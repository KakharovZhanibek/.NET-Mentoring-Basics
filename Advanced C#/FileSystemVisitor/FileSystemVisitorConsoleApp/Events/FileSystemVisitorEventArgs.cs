namespace FileSystemVisitorConsoleApp.Events;

/// <summary>
/// Event arguments for file system visitor events.
/// </summary>
public class FileSystemVisitorEventArgs : EventArgs
{
    /// <summary>
    /// Gets the message describing what happened.
    /// </summary>
    public string Message { get; }
    
    /// <summary>
    /// Gets the type of event that occurred.
    /// </summary>
    public FileSystemEventType EventType { get; }
    
    /// <summary>
    /// Gets the item path (null for Start/Finish events).
    /// </summary>
    public string? ItemPath { get; }
    
    /// <summary>
    /// Gets or sets whether to stop the search.
    /// </summary>
    public bool StopSearch { get; set; }
    
    /// <summary>
    /// Gets or sets whether to exclude this item from results.
    /// </summary>
    public bool ExcludeItem { get; set; }

    /// <summary>
    /// Initializes a new instance of FileSystemVisitorEventArgs.
    /// </summary>
    /// <param name="message">A descriptive message about what happened.</param>
    /// <param name="eventType">The type of event that occurred.</param>
    /// <param name="itemPath">The path of the item (optional, null for Start/Finish events).</param>
    public FileSystemVisitorEventArgs(string message, FileSystemEventType eventType, string? itemPath = null)
    {
        Message = message;
        EventType = eventType;
        ItemPath = itemPath;
        StopSearch = false;
        ExcludeItem = false;
    }
}
