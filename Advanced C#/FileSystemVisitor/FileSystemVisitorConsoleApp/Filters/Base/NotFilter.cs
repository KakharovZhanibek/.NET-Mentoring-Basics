using FileSystemVisitorConsoleApp.Interfaces;

namespace FileSystemVisitorConsoleApp.Filters.Base;

/// <summary>
/// Inverts the result of another filter (NOT logic).
/// </summary>
public class NotFilter : FileSystemFilterBase
{
    private readonly IFileSystemFilter _filter;
    
    public NotFilter(IFileSystemFilter filter)
    {
        _filter = filter ?? throw new ArgumentNullException(nameof(filter));
    }
    
    public override bool IsMatch(FileSystemInfo info)
    {
        return !_filter.IsMatch(info);
    }
    
    public override string Description => $"NOT ({_filter.Description})";
}
