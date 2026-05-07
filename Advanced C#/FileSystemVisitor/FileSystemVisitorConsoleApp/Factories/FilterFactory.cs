using FileSystemVisitorConsoleApp.Filters;
using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Interfaces;

namespace FileSystemVisitorConsoleApp.Factories;

/// <summary>
/// Factory for creating file system filters.
/// </summary>
public class FilterFactory
{
    /// <summary>
    /// Creates a size filter.
    /// </summary>
    public IFileSystemFilter CreateSizeFilter(long? minSize, long? maxSize)
    {
        return new SizeFilter(minSize, maxSize);
    }
    
    /// <summary>
    /// Creates an extension filter.
    /// </summary>
    public IFileSystemFilter CreateExtensionFilter(params string[] extensions)
    {
        return new ExtensionFilter(extensions);
    }
    
    /// <summary>
    /// Creates a creation date filter.
    /// </summary>
    public IFileSystemFilter CreateCreationDateFilter(DateTime? startDate, DateTime? endDate)
    {
        return new CreationDateFilter(startDate, endDate);
    }
    
    /// <summary>
    /// Creates a modification date filter.
    /// </summary>
    public IFileSystemFilter CreateModificationDateFilter(DateTime? startDate, DateTime? endDate)
    {
        return new ModificationDateFilter(startDate, endDate);
    }
    
    /// <summary>
    /// Creates a files-only filter.
    /// </summary>
    public IFileSystemFilter CreateFilesOnlyFilter()
    {
        return new FilesOnlyFilter();
    }
    
    /// <summary>
    /// Creates a directories-only filter.
    /// </summary>
    public IFileSystemFilter CreateDirectoriesOnlyFilter()
    {
        return new DirectoriesOnlyFilter();
    }
    
    /// <summary>
    /// Creates an AND composite filter.
    /// </summary>
    public IFileSystemFilter CreateAndFilter(params IFileSystemFilter[] filters)
    {
        return new AndFilter(filters);
    }
    
    /// <summary>
    /// Creates an OR composite filter.
    /// </summary>
    public IFileSystemFilter CreateOrFilter(params IFileSystemFilter[] filters)
    {
        return new OrFilter(filters);
    }
    
    /// <summary>
    /// Creates a NOT filter.
    /// </summary>
    public IFileSystemFilter CreateNotFilter(IFileSystemFilter filter)
    {
        return new NotFilter(filter);
    }
}
