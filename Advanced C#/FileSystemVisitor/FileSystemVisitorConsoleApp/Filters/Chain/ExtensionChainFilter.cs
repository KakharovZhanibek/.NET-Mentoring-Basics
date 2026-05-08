using FileSystemVisitorConsoleApp.Filters.Base;

namespace FileSystemVisitorConsoleApp.Filters.Chain;

/// <summary>
/// Chain filter that checks if a file has a specific extension.
/// Can be chained with other filters to create a processing pipeline.
/// </summary>
public class ExtensionChainFilter : ChainFilterBase
{
    private readonly string _extension;
    
    /// <summary>
    /// Initializes a new instance of the ExtensionChainFilter class.
    /// </summary>
    /// <param name="extension">The file extension to check (with or without leading dot).</param>
    public ExtensionChainFilter(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            throw new ArgumentException("Extension cannot be null or empty.", nameof(extension));
        
        _extension = extension.StartsWith(".") ? extension : "." + extension;
        _extension = _extension.ToLowerInvariant();
    }
    
    /// <summary>
    /// Processes the filter logic for this extension check.
    /// </summary>
    /// <param name="info">The file system item information.</param>
    /// <returns>True if the item matches this extension; otherwise, false.</returns>
    protected override bool ProcessFilter(FileSystemInfo info)
    {
        // Directories always pass through to next filter
        if (info is DirectoryInfo)
            return true;
        
        return info.Extension.ToLowerInvariant() == _extension;
    }
    
    /// <summary>
    /// Gets the description of this filter.
    /// </summary>
    protected override string GetFilterDescription()
    {
        return $"Extension={_extension}";
    }
}
