using FileSystemVisitorConsoleApp.Filters.Base;

namespace FileSystemVisitorConsoleApp.Filters;

/// <summary>
/// Filters files based on file extension.
/// </summary>
public class ExtensionFilter : FileSystemFilterBase
{
    private readonly HashSet<string> _extensions;
    
    public ExtensionFilter(params string[] extensions)
    {
        if (extensions == null || extensions.Length == 0)
            throw new ArgumentException("At least one extension must be provided.", nameof(extensions));
            
        _extensions = extensions
            .Select(e => e.StartsWith(".") ? e : "." + e)
            .Select(e => e.ToLowerInvariant())
            .ToHashSet();
    }
    
    public override bool IsMatch(FileSystemInfo info)
    {
        if (info is not FileInfo)
            return false;
            
        return _extensions.Contains(info.Extension.ToLowerInvariant());
    }
    
    public override string Description =>
        $"Extension: {string.Join(", ", _extensions)}";
}
