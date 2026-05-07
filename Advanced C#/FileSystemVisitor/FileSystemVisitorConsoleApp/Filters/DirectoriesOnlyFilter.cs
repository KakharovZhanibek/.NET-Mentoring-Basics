using FileSystemVisitorConsoleApp.Filters.Base;

namespace FileSystemVisitorConsoleApp.Filters;

/// <summary>
/// Filters to show only directories (excludes files).
/// </summary>
public class DirectoriesOnlyFilter : FileSystemFilterBase
{
    public override bool IsMatch(FileSystemInfo info)
    {
        return info is DirectoryInfo;
    }
    
    public override string Description => "Directories only";
}
