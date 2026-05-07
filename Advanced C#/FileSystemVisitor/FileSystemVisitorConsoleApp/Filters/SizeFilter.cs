using FileSystemVisitorConsoleApp.Filters.Base;

namespace FileSystemVisitorConsoleApp.Filters;

/// <summary>
/// Filters files based on size criteria.
/// </summary>
public class SizeFilter : FileSystemFilterBase
{
    private readonly long? _minSize;
    private readonly long? _maxSize;
    
    public SizeFilter(long? minSize, long? maxSize)
    {
        if (minSize.HasValue && maxSize.HasValue && minSize.Value > maxSize.Value)
            throw new ArgumentException("Minimum size cannot be greater than maximum size.");
            
        _minSize = minSize;
        _maxSize = maxSize;
    }
    
    public override bool IsMatch(FileSystemInfo info)
    {
        if (info is not FileInfo file)
            return false;
            
        if (_minSize.HasValue && file.Length < _minSize.Value)
            return false;
            
        if (_maxSize.HasValue && file.Length > _maxSize.Value)
            return false;
            
        return true;
    }
    
    public override string Description
    {
        get
        {
            if (_minSize.HasValue && _maxSize.HasValue)
                return $"Size: {_minSize.Value / 1024}KB - {_maxSize.Value / 1024}KB";
            if (_minSize.HasValue)
                return $"Size: > {_minSize.Value / 1024}KB";
            if (_maxSize.HasValue)
                return $"Size: < {_maxSize.Value / 1024}KB";
            return "Size: Any";
        }
    }
}
