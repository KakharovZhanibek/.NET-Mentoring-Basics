using FileSystemVisitorConsoleApp.Events;
using FileSystemVisitorConsoleApp.Interfaces;

namespace FileSystemVisitorConsoleApp;

/// <summary>
/// A class that allows traversing a file system tree from a pre-defined folder.
/// Raises events during the traversal process.
/// </summary>
public class FileSystemVisitor : IFileSystemVisitor
{
    private readonly string _rootPath;
    private readonly Func<string, bool>? _pathFilter;
    private readonly Func<FileSystemInfo, bool>? _infoFilter;
    private readonly IFileSystemFilter? _filter;
    private bool _stopSearchRequested;

    /// <summary>
    /// Event raised during file system traversal with information about what happened.
    /// </summary>
    public event EventHandler<FileSystemVisitorEventArgs>? ItemProcessed;

    /// <summary>
    /// Initializes a new instance of FileSystemVisitor without filtering.
    /// </summary>
    /// <param name="rootPath">The root directory to start traversing from.</param>
    /// <exception cref="ArgumentNullException">Thrown when rootPath is null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
    public FileSystemVisitor(string rootPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
            throw new ArgumentNullException(nameof(rootPath));

        if (!Directory.Exists(rootPath))
            throw new DirectoryNotFoundException($"Directory not found: {rootPath}");

        _rootPath = rootPath;
        _pathFilter = null;
        _infoFilter = null;
        _filter = null;
        _stopSearchRequested = false;
    }

    /// <summary>
    /// Initializes a new instance of FileSystemVisitor with a path-based filter.
    /// </summary>
    /// <param name="rootPath">The root directory to start traversing from.</param>
    /// <param name="filter">A delegate that defines the filtering logic based on path. Returns true to include the item.</param>
    /// <exception cref="ArgumentNullException">Thrown when rootPath is null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
    public FileSystemVisitor(string rootPath, Func<string, bool> filter) : this(rootPath)
    {
        _pathFilter = filter;
    }

    /// <summary>
    /// Initializes a new instance of FileSystemVisitor with a FileSystemInfo-based filter.
    /// This allows filtering based on file attributes, dates, size, etc.
    /// </summary>
    /// <param name="rootPath">The root directory to start traversing from.</param>
    /// <param name="filter">A delegate that defines the filtering logic based on FileSystemInfo. Returns true to include the item.</param>
    /// <exception cref="ArgumentNullException">Thrown when rootPath is null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
    public FileSystemVisitor(string rootPath, Func<FileSystemInfo, bool> filter) : this(rootPath)
    {
        _infoFilter = filter;
    }

    /// <summary>
    /// Returns all found files and folders as a linear sequence using an iterator.
    /// </summary>
    /// <returns>An enumerable collection of file and folder paths.</returns>
    public IEnumerable<string> GetFileSystemItems()
    {
        _stopSearchRequested = false;
        
        // Raise Start event
        OnStart();

        foreach (var item in TraverseDirectory(_rootPath))
        {
            if (_stopSearchRequested)
                break;
            
            yield return item;
        }

        // Raise Finish event
        OnFinish();
    }

    /// <summary>
    /// Recursively traverses the directory tree and yields files and folders.
    /// </summary>
    /// <param name="currentPath">The current directory path to traverse.</param>
    /// <returns>An enumerable collection of file and folder paths.</returns>
    private IEnumerable<string> TraverseDirectory(string currentPath)
    {
        if (_stopSearchRequested)
            yield break;

        // Enumerate directories
        string[]? directories = null;
        try
        {
            directories = Directory.GetDirectories(currentPath);
        }
        catch (UnauthorizedAccessException)
        {
            // Skip directories we don't have access to
            yield break;
        }
        catch (DirectoryNotFoundException)
        {
            // Skip if directory no longer exists
            yield break;
        }

        if (directories != null)
        {
            foreach (var directory in directories)
            {
                if (_stopSearchRequested)
                    yield break;

                // Raise DirectoryFound event (before filtering)
                var dirFoundArgs = new FileSystemVisitorEventArgs(
                    $"Directory found: {directory}", 
                    FileSystemEventType.DirectoryFound, 
                    directory);
                OnItemProcessed(dirFoundArgs);

                if (dirFoundArgs.StopSearch)
                {
                    _stopSearchRequested = true;
                    yield break;
                }

                if (dirFoundArgs.ExcludeItem)
                    continue;

                // Apply filter if provided
                bool passesFilter = ApplyFilter(directory);
                
                if (passesFilter)
                {
                    // Raise FilteredDirectoryFound event (after filtering)
                    var filteredDirArgs = new FileSystemVisitorEventArgs(
                        $"Filtered directory found: {directory}", 
                        FileSystemEventType.FilteredDirectoryFound, 
                        directory);
                    OnItemProcessed(filteredDirArgs);

                    if (filteredDirArgs.StopSearch)
                    {
                        _stopSearchRequested = true;
                        yield break;
                    }

                    if (!filteredDirArgs.ExcludeItem)
                    {
                        yield return directory;
                    }
                }

                // Recursively traverse subdirectories
                foreach (var item in TraverseDirectory(directory))
                {
                    if (_stopSearchRequested)
                        yield break;
                    
                    yield return item;
                }
            }
        }

        // Enumerate files
        string[]? files = null;
        try
        {
            files = Directory.GetFiles(currentPath);
        }
        catch (UnauthorizedAccessException)
        {
            // Skip directories we don't have access to
            yield break;
        }
        catch (DirectoryNotFoundException)
        {
            // Skip if directory no longer exists
            yield break;
        }

        if (files != null)
        {
            foreach (var file in files)
            {
                if (_stopSearchRequested)
                    yield break;

                // Raise FileFound event (before filtering)
                var fileFoundArgs = new FileSystemVisitorEventArgs(
                    $"File found: {file}", 
                    FileSystemEventType.FileFound, 
                    file);
                OnItemProcessed(fileFoundArgs);

                if (fileFoundArgs.StopSearch)
                {
                    _stopSearchRequested = true;
                    yield break;
                }

                if (fileFoundArgs.ExcludeItem)
                    continue;

                // Apply filter if provided
                bool passesFilter = ApplyFilter(file);
                
                if (passesFilter)
                {
                    // Raise FilteredFileFound event (after filtering)
                    var filteredFileArgs = new FileSystemVisitorEventArgs(
                        $"Filtered file found: {file}", 
                        FileSystemEventType.FilteredFileFound, 
                        file);
                    OnItemProcessed(filteredFileArgs);

                    if (filteredFileArgs.StopSearch)
                    {
                        _stopSearchRequested = true;
                        yield break;
                    }

                    if (!filteredFileArgs.ExcludeItem)
                    {
                        yield return file;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Applies the appropriate filter (path-based, info-based, or IFileSystemFilter) to the item.
    /// </summary>
    /// <param name="itemPath">The path of the item to filter.</param>
    /// <returns>True if the item passes the filter, false otherwise.</returns>
    private bool ApplyFilter(string itemPath)
    {
        // No filter means include everything
        if (_pathFilter == null && _infoFilter == null && _filter == null)
            return true;

        // Apply path-based filter (highest priority for backward compatibility)
        if (_pathFilter != null)
            return _pathFilter(itemPath);

        // Apply info-based filter delegate
        if (_infoFilter != null)
        {
            try
            {
                FileSystemInfo info = File.Exists(itemPath) 
                    ? new FileInfo(itemPath) 
                    : new DirectoryInfo(itemPath);
                return _infoFilter(info);
            }
            catch
            {
                return false;
            }
        }

        // Apply IFileSystemFilter
        if (_filter != null)
        {
            try
            {
                FileSystemInfo info = File.Exists(itemPath) 
                    ? new FileInfo(itemPath) 
                    : new DirectoryInfo(itemPath);
                return _filter.IsMatch(info);
            }
            catch
            {
                return false;
            }
        }

        return true;
    }

    // Event raising methods
    
    /// <summary>
    /// Raises the Start event.
    /// </summary>
    protected virtual void OnStart()
    {
        var args = new FileSystemVisitorEventArgs("Search started", FileSystemEventType.Start);
        OnItemProcessed(args);
    }

    /// <summary>
    /// Raises the Finish event.
    /// </summary>
    protected virtual void OnFinish()
    {
        var args = new FileSystemVisitorEventArgs("Search finished", FileSystemEventType.Finish);
        OnItemProcessed(args);
    }

    /// <summary>
    /// Raises the ItemProcessed event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnItemProcessed(FileSystemVisitorEventArgs e)
    {
        ItemProcessed?.Invoke(this, e);
        
        if (e.StopSearch)
        {
            _stopSearchRequested = true;
        }
    }
}
