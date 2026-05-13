using FileSystemVisitorConsoleApp.Interfaces;

namespace FileSystemVisitorConsoleApp.Filters.Base;

/// <summary>
/// Combines multiple filters with AND logic (all must match).
/// </summary>
public class AndFilter : FileSystemFilterBase
{
    private readonly IEnumerable<IFileSystemFilter> _filters;
    
    public AndFilter(params IFileSystemFilter[] filters)
    {
        _filters = filters ?? throw new ArgumentNullException(nameof(filters));
    }
    
    public AndFilter(IEnumerable<IFileSystemFilter> filters)
    {
        _filters = filters ?? throw new ArgumentNullException(nameof(filters));
    }
    
    public override bool IsMatch(FileSystemInfo info)
    {
        return _filters.All(f => f.IsMatch(info));
    }
    
    public override string Description =>
        $"({string.Join(" AND ", _filters.Select(f => f.Description))})";
}
