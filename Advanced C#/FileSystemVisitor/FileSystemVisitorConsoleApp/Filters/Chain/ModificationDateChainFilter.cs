using FileSystemVisitorConsoleApp.Filters.Base;

namespace FileSystemVisitorConsoleApp.Filters.Chain;

/// <summary>
/// Chain filter that checks if a file meets date modification criteria.
/// Can be chained with other filters to create a processing pipeline.
/// </summary>
public class ModificationDateChainFilter : ChainFilterBase
{
    private readonly DateTime? _startDate;
    private readonly DateTime? _endDate;
    
    /// <summary>
    /// Initializes a new instance of the ModificationDateChainFilter class.
    /// </summary>
    /// <param name="startDate">Start date (null for no start date).</param>
    /// <param name="endDate">End date (null for no end date).</param>
    public ModificationDateChainFilter(DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            throw new ArgumentException("Start date cannot be after end date.");
        
        _startDate = startDate;
        _endDate = endDate;
    }
    
    /// <summary>
    /// Processes the filter logic for modification date checking.
    /// </summary>
    /// <param name="info">The file system item information.</param>
    /// <returns>True if the item meets the date criteria; otherwise, false.</returns>
    protected override bool ProcessFilter(FileSystemInfo info)
    {
        var modifiedDate = info.LastWriteTime;
        
        if (_startDate.HasValue && modifiedDate < _startDate.Value)
            return false;
        
        if (_endDate.HasValue && modifiedDate > _endDate.Value)
            return false;
        
        return true;
    }
    
    /// <summary>
    /// Gets the description of this filter.
    /// </summary>
    protected override string GetFilterDescription()
    {
        if (_startDate.HasValue && _endDate.HasValue)
            return $"Modified:[{_startDate.Value:yyyy-MM-dd} to {_endDate.Value:yyyy-MM-dd}]";
        if (_startDate.HasValue)
            return $"Modified:?{_startDate.Value:yyyy-MM-dd}";
        if (_endDate.HasValue)
            return $"Modified:?{_endDate.Value:yyyy-MM-dd}";
        return "Modified:Any";
    }
}
