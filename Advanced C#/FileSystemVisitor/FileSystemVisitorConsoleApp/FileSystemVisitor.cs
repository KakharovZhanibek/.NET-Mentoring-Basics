using FileSystemVisitorConsoleApp.Interfaces;

namespace FileSystemVisitorConsoleApp;

/// <summary>
/// Event arguments for file system item events.
/// </summary>
public class ItemEventArgs : EventArgs
{
    public string ItemPath { get; }
    public bool StopSearch { get; set; }
    public bool ExcludeItem { get; set; }

    public ItemEventArgs(string itemPath)
    {
        ItemPath = itemPath;
        StopSearch = false;
        ExcludeItem = false;
    }
}

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

    // Events
    public event EventHandler? Start;
    public event EventHandler? Finish;
    public event EventHandler<ItemEventArgs>? FileFound;
    public event EventHandler<ItemEventArgs>? DirectoryFound;
    public event EventHandler<ItemEventArgs>? FilteredFileFound;
    public event EventHandler<ItemEventArgs>? FilteredDirectoryFound;

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
                var dirFoundArgs = new ItemEventArgs(directory);
                OnDirectoryFound(dirFoundArgs);

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
                    var filteredDirArgs = new ItemEventArgs(directory);
                    OnFilteredDirectoryFound(filteredDirArgs);

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
                var fileFoundArgs = new ItemEventArgs(file);
                OnFileFound(fileFoundArgs);

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
                    var filteredFileArgs = new ItemEventArgs(file);
                    OnFilteredFileFound(filteredFileArgs);

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

    /// <summary>f
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
    protected virtual void OnStart()
    {
        Start?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnFinish()
    {
        Finish?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnFileFound(ItemEventArgs e)
    {
        FileFound?.Invoke(this, e);
    }

    protected virtual void OnDirectoryFound(ItemEventArgs e)
    {
        DirectoryFound?.Invoke(this, e);
    }

    protected virtual void OnFilteredFileFound(ItemEventArgs e)
    {
        FilteredFileFound?.Invoke(this, e);
    }

    protected virtual void OnFilteredDirectoryFound(ItemEventArgs e)
    {
        FilteredDirectoryFound?.Invoke(this, e);
    }
}
