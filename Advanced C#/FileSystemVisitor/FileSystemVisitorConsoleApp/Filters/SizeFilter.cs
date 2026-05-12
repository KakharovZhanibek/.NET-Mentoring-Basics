using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Interfaces;
using FileSystemVisitorConsoleApp.Metadata;

namespace FileSystemVisitorConsoleApp.Filters;

/// <summary>
/// Filters files based on size criteria.
/// </summary>
public class SizeFilter : FileSystemFilterBase, IFilterMetadata
{
    private readonly long? _minSize;
    private readonly long? _maxSize;
    
    public SizeFilter(long? minSize, long? maxSize)
    {
        if (minSize.HasValue && maxSize.HasValue && minSize.Value > maxSize.Value)
            throw new ArgumentException("Minimum size cannot be greater than maximum size.");
            
        _minSize = minSize;
        _maxSize = maxSize;
    }
    
    public override bool IsMatch(FileSystemInfo info)
    {
        if (info is not FileInfo file)
            return false;
            
        if (_minSize.HasValue && file.Length < _minSize.Value)
            return false;
            
        if (_maxSize.HasValue && file.Length > _maxSize.Value)
            return false;
            
        return true;
    }
    
    public override string Description
    {
        get
        {
            if (_minSize.HasValue && _maxSize.HasValue)
                return $"Size: {_minSize.Value / 1024}KB - {_maxSize.Value / 1024}KB";
            if (_minSize.HasValue)
                return $"Size: > {_minSize.Value / 1024}KB";
            if (_maxSize.HasValue)
                return $"Size: < {_maxSize.Value / 1024}KB";
            return "Size: Any";
        }
    }
    
    // IFilterMetadata implementation
    public static string FilterName => "Size Filter";
    
    public static string FilterDescription => "Filter files by size";
    
    public static IFileSystemFilter? CreateFromUserInput()
    {
        Console.Write("Enter minimum size in KB (or press Enter to skip): ");
        string? minInput = Console.ReadLine();
        
        Console.Write("Enter maximum size in KB (or press Enter to skip): ");
        string? maxInput = Console.ReadLine();
        
        long? minSize = null;
        long? maxSize = null;
        
        if (!string.IsNullOrWhiteSpace(minInput))
        {
            if (long.TryParse(minInput, out long min))
            {
                minSize = min * 1024; // Convert KB to bytes
            }
            else
            {
                Console.WriteLine("? Invalid minimum size. Must be a number.");
                return null;
            }
        }
        
        if (!string.IsNullOrWhiteSpace(maxInput))
        {
            if (long.TryParse(maxInput, out long max))
            {
                maxSize = max * 1024; // Convert KB to bytes
            }
            else
            {
                Console.WriteLine("? Invalid maximum size. Must be a number.");
                return null;
            }
        }
        
        if (!minSize.HasValue && !maxSize.HasValue)
        {
            Console.WriteLine("? At least one size constraint must be provided.");
            return null;
        }
        
        return new SizeFilter(minSize, maxSize);
    }
}
