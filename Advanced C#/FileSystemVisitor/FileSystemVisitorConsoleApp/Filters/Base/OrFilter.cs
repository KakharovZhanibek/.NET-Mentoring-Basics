using FileSystemVisitorConsoleApp.Interfaces;

namespace FileSystemVisitorConsoleApp.Filters.Base;

/// <summary>
/// Combines multiple filters with OR logic (at least one must match).
/// </summary>
public class OrFilter : FileSystemFilterBase
{
    private readonly IEnumerable<IFileSystemFilter> _filters;
    
    public OrFilter(params IFileSystemFilter[] filters)
    {
        _filters = filters ?? throw new ArgumentNullException(nameof(filters));
    }
    
    public OrFilter(IEnumerable<IFileSystemFilter> filters)
    {
        _filters = filters ?? throw new ArgumentNullException(nameof(filters));
    }
    
    public override bool IsMatch(FileSystemInfo info)
    {
        return _filters.Any(f => f.IsMatch(info));
    }
    
    public override string Description =>
        $"({string.Join(" OR ", _filters.Select(f => f.Description))})";
}
