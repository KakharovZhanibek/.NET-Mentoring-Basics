using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Interfaces;

namespace FileSystemVisitorConsoleApp.Filters;

/// <summary>
/// Filters items based on creation date.
/// </summary>
public class CreationDateFilter : FileSystemFilterBase, IFilterMetadata
{
    private readonly DateTime? _startDate;
    private readonly DateTime? _endDate;
    
    public CreationDateFilter(DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            throw new ArgumentException("Start date cannot be after end date.");
            
        _startDate = startDate;
        _endDate = endDate;
    }
    
    public override bool IsMatch(FileSystemInfo info)
    {
        if (_startDate.HasValue && info.CreationTime < _startDate.Value)
            return false;
            
        if (_endDate.HasValue && info.CreationTime > _endDate.Value)
            return false;
            
        return true;
    }
    
    public override string Description
    {
        get
        {
            if (_startDate.HasValue && _endDate.HasValue)
                return $"Created: {_startDate.Value:yyyy-MM-dd} to {_endDate.Value:yyyy-MM-dd}";
            if (_startDate.HasValue)
                return $"Created: after {_startDate.Value:yyyy-MM-dd}";
            if (_endDate.HasValue)
                return $"Created: before {_endDate.Value:yyyy-MM-dd}";
            return "Created: Any time";
        }
    }
    
    // IFilterMetadata implementation
    public static string FilterName => "Creation Date Filter";
    
    public static string FilterDescription => "Filter by creation date";
    
    public static IFileSystemFilter? CreateFromUserInput()
    {
        Console.Write("Enter start date (yyyy-MM-dd) or press Enter to skip: ");
        string? startInput = Console.ReadLine();
        
        Console.Write("Enter end date (yyyy-MM-dd) or press Enter to skip: ");
        string? endInput = Console.ReadLine();
        
        DateTime? startDate = null;
        DateTime? endDate = null;
        
        if (!string.IsNullOrWhiteSpace(startInput))
        {
            if (DateTime.TryParse(startInput, out DateTime start))
            {
                startDate = start;
            }
            else
            {
                Console.WriteLine("? Invalid start date format. Use yyyy-MM-dd.");
                return null;
            }
        }
        
        if (!string.IsNullOrWhiteSpace(endInput))
        {
            if (DateTime.TryParse(endInput, out DateTime end))
            {
                endDate = end;
            }
            else
            {
                Console.WriteLine("? Invalid end date format. Use yyyy-MM-dd.");
                return null;
            }
        }
        
        if (!startDate.HasValue && !endDate.HasValue)
        {
            Console.WriteLine("? At least one date must be provided.");
            return null;
        }
        
        return new CreationDateFilter(startDate, endDate);
    }
}
