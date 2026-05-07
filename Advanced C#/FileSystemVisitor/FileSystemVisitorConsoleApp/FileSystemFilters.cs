namespace FileSystemVisitorConsoleApp;

/// <summary>
/// Provides predefined filters for FileSystemVisitor.
/// </summary>
public static class FileSystemFilters
{
    /// <summary>
    /// Creates a filter for files larger than specified size.
    /// </summary>
    /// <param name="sizeInBytes">Minimum file size in bytes.</param>
    /// <returns>Filter function.</returns>
    public static Func<FileSystemInfo, bool> FilesLargerThan(long sizeInBytes)
    {
        return info => info is FileInfo fileInfo && fileInfo.Length > sizeInBytes;
    }

    /// <summary>
    /// Creates a filter for files smaller than specified size.
    /// </summary>
    /// <param name="sizeInBytes">Maximum file size in bytes.</param>
    /// <returns>Filter function.</returns>
    public static Func<FileSystemInfo, bool> FilesSmallerThan(long sizeInBytes)
    {
        return info => info is FileInfo fileInfo && fileInfo.Length < sizeInBytes;
    }

    /// <summary>
    /// Creates a filter for items within a creation date range.
    /// If startDate is null, filters items created before or on endDate.
    /// If endDate is null, filters items created after or on startDate.
    /// If both dates are provided, filters items created between them (inclusive).
    /// </summary>
    /// <param name="startDate">Start date (null for no lower bound).</param>
    /// <param name="endDate">End date (null for no upper bound).</param>
    /// <returns>Filter function.</returns>
    public static Func<FileSystemInfo, bool> CreatedInRange(DateTime? startDate, DateTime? endDate)
    {
        return info =>
        {
            if (startDate.HasValue && info.CreationTime < startDate.Value)
                return false;
            if (endDate.HasValue && info.CreationTime > endDate.Value)
                return false;
            return true;
        };
    }

    /// <summary>
    /// Creates a filter for items within a modification date range.
    /// If startDate is null, filters items modified before or on endDate.
    /// If endDate is null, filters items modified after or on startDate.
    /// If both dates are provided, filters items modified between them (inclusive).
    /// </summary>
    /// <param name="startDate">Start date (null for no lower bound).</param>
    /// <param name="endDate">End date (null for no upper bound).</param>
    /// <returns>Filter function.</returns>
    public static Func<FileSystemInfo, bool> ModifiedInRange(DateTime? startDate, DateTime? endDate)
    {
        return info =>
        {
            if (startDate.HasValue && info.LastWriteTime < startDate.Value)
                return false;
            if (endDate.HasValue && info.LastWriteTime > endDate.Value)
                return false;
            return true;
        };
    }

    /// <summary>
    /// Creates a filter for files with any of the specified extensions.
    /// </summary>
    /// <param name="extensions">List of file extensions.</param>
    /// <returns>Filter function.</returns>
    public static Func<FileSystemInfo, bool> WithExtensions(params string[] extensions)
    {
        var normalizedExtensions = extensions
            .Select(e => e.StartsWith(".") ? e : "." + e)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        return info => info is FileInfo && normalizedExtensions.Contains(info.Extension);
    }

    /// <summary>
    /// Creates a filter for files only (excludes directories).
    /// </summary>
    public static Func<FileSystemInfo, bool> FilesOnly()
    {
        return info => info is FileInfo;
    }

    /// <summary>
    /// Creates a filter for directories only (excludes files).
    /// </summary>
    public static Func<FileSystemInfo, bool> DirectoriesOnly()
    {
        return info => info is DirectoryInfo;
    }

    /// <summary>
    /// Combines multiple filters with AND logic.
    /// </summary>
    /// <param name="filters">Filters to combine.</param>
    /// <returns>Combined filter function.</returns>
    public static Func<FileSystemInfo, bool> And(params Func<FileSystemInfo, bool>[] filters)
    {
        return info => filters.All(filter => filter(info));
    }

    /// <summary>
    /// Combines multiple filters with OR logic.
    /// </summary>
    /// <param name="filters">Filters to combine.</param>
    /// <returns>Combined filter function.</returns>
    public static Func<FileSystemInfo, bool> Or(params Func<FileSystemInfo, bool>[] filters)
    {
        return info => filters.Any(filter => filter(info));
    }

    /// <summary>
    /// Negates a filter.
    /// </summary>
    /// <param name="filter">Filter to negate.</param>
    /// <returns>Negated filter function.</returns>
    public static Func<FileSystemInfo, bool> Not(Func<FileSystemInfo, bool> filter)
    {
        return info => !filter(info);
    }

    /// <summary>
    /// Creates a filter for files within a size range.
    /// </summary>
    /// <param name="minSize">Minimum size in bytes.</param>
    /// <param name="maxSize">Maximum size in bytes.</param>
    /// <returns>Filter function.</returns>
    public static Func<FileSystemInfo, bool> FileSizeRange(long minSize, long maxSize)
    {
        return info => info is FileInfo fileInfo && 
                      fileInfo.Length >= minSize && 
                      fileInfo.Length <= maxSize;
    }
}
