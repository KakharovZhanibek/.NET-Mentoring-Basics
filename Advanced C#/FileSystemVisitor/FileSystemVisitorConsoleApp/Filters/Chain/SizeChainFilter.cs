using FileSystemVisitorConsoleApp.Filters.Base;

namespace FileSystemVisitorConsoleApp.Filters.Chain;

/// <summary>
/// Chain filter that checks if a file meets size criteria.
/// Can be chained with other filters to create a processing pipeline.
/// </summary>
public class SizeChainFilter : ChainFilterBase
{
    private readonly long? _minSize;
    private readonly long? _maxSize;
    
    /// <summary>
    /// Initializes a new instance of the SizeChainFilter class.
    /// </summary>
    /// <param name="minSize">Minimum file size in bytes (null for no minimum).</param>
    /// <param name="maxSize">Maximum file size in bytes (null for no maximum).</param>
    public SizeChainFilter(long? minSize, long? maxSize)
    {
        if (minSize.HasValue && maxSize.HasValue && minSize.Value > maxSize.Value)
            throw new ArgumentException("Minimum size cannot be greater than maximum size.");
        
        _minSize = minSize;
        _maxSize = maxSize;
    }
    
    /// <summary>
    /// Processes the filter logic for size checking.
    /// </summary>
    /// <param name="info">The file system item information.</param>
    /// <returns>True if the item meets the size criteria; otherwise, false.</returns>
    protected override bool ProcessFilter(FileSystemInfo info)
    {
        // Directories always pass through to next filter
        if (info is not FileInfo file)
            return true;
        
        if (_minSize.HasValue && file.Length < _minSize.Value)
            return false;
        
        if (_maxSize.HasValue && file.Length > _maxSize.Value)
            return false;
        
        return true;
    }
    
    /// <summary>
    /// Gets the description of this filter.
    /// </summary>
    protected override string GetFilterDescription()
    {
        if (_minSize.HasValue && _maxSize.HasValue)
            return $"Size:[{_minSize.Value / 1024}KB-{_maxSize.Value / 1024}KB]";
        if (_minSize.HasValue)
            return $"Size:?{_minSize.Value / 1024}KB";
        if (_maxSize.HasValue)
            return $"Size:?{_maxSize.Value / 1024}KB";
        return "Size:Any";
    }
}
