using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Interfaces;
using FileSystemVisitorConsoleApp.Metadata;

namespace FileSystemVisitorConsoleApp.Filters;

/// <summary>
/// Filters to show only directories (excludes files).
/// </summary>
public class DirectoriesOnlyFilter : FileSystemFilterBase, IFilterMetadata
{
    public override bool IsMatch(FileSystemInfo info)
    {
        return info is DirectoryInfo;
    }
    
    public override string Description => "Type: Directories only";
    
    // IFilterMetadata implementation
    public static string FilterName => "Directories Only";
    
    public static string FilterDescription => "Show only directories";
    
    public static IFileSystemFilter? CreateFromUserInput()
    {
        // No user input needed for this filter
        return new DirectoriesOnlyFilter();
    }
}
