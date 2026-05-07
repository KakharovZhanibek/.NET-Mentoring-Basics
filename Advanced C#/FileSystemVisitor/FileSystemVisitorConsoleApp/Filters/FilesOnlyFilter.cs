using FileSystemVisitorConsoleApp.Filters.Base;

namespace FileSystemVisitorConsoleApp.Filters;

/// <summary>
/// Filters to show only files (excludes directories).
/// </summary>
public class FilesOnlyFilter : FileSystemFilterBase
{
    public override bool IsMatch(FileSystemInfo info)
    {
        return info is FileInfo;
    }
    
    public override string Description => "Files only";
}
