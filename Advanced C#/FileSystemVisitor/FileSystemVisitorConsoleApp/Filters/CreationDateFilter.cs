using FileSystemVisitorConsoleApp.Filters.Base;

namespace FileSystemVisitorConsoleApp.Filters;

/// <summary>
/// Filters items based on creation date.
/// </summary>
public class CreationDateFilter : FileSystemFilterBase
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
}
