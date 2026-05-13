using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Interfaces;
using FileSystemVisitorConsoleApp.Metadata;

namespace FileSystemVisitorConsoleApp.Filters;

/// <summary>
/// Filters to show only files (excludes directories).
/// </summary>
public class FilesOnlyFilter : FileSystemFilterBase, IFilterMetadata
{
    public override bool IsMatch(FileSystemInfo info)
    {
        return info is FileInfo;
    }
    
    public override string Description => "Type: Files only";
    
    // IFilterMetadata implementation
    public static string FilterName => "Files Only";
    
    public static string FilterDescription => "Show only files";
    
    public static IFileSystemFilter? CreateFromUserInput()
    {
        // No user input needed for this filter
        return new FilesOnlyFilter();
    }
}
