using FileSystemVisitorConsoleApp.Filters.Base;

namespace FileSystemVisitorConsoleApp.Filters.Chain;

/// <summary>
/// Chain filter that checks if an item is a file (not a directory).
/// Can be chained with other filters to create a processing pipeline.
/// </summary>
public class FileTypeChainFilter : ChainFilterBase
{
    private readonly bool _filesOnly;
    
    /// <summary>
    /// Initializes a new instance of the FileTypeChainFilter class.
    /// </summary>
    /// <param name="filesOnly">True to accept only files; false to accept only directories.</param>
    public FileTypeChainFilter(bool filesOnly = true)
    {
        _filesOnly = filesOnly;
    }
    
    /// <summary>
    /// Processes the filter logic for file type checking.
    /// </summary>
    /// <param name="info">The file system item information.</param>
    /// <returns>True if the item matches the desired type; otherwise, false.</returns>
    protected override bool ProcessFilter(FileSystemInfo info)
    {
        bool isFile = info is FileInfo;
        return _filesOnly ? isFile : !isFile;
    }
    
    /// <summary>
    /// Gets the description of this filter.
    /// </summary>
    protected override string GetFilterDescription()
    {
        return _filesOnly ? "Type:File" : "Type:Directory";
    }
}
