using FileSystemVisitorConsoleApp.Interfaces;

namespace FileSystemVisitorConsoleApp.Filters.Base;

/// <summary>
/// Abstract base class for filters that implement the Chain of Responsibility pattern.
/// Each filter in the chain can process the request and optionally pass it to the next filter.
/// </summary>
public abstract class ChainFilterBase : FileSystemFilterBase
{
    /// <summary>
    /// The next filter in the chain.
    /// </summary>
    protected IFileSystemFilter? NextFilter { get; private set; }
    
    /// <summary>
    /// Sets the next filter in the chain.
    /// </summary>
    /// <param name="nextFilter">The next filter to process the request if this filter passes.</param>
    /// <returns>The next filter, allowing for fluent chaining.</returns>
    public IFileSystemFilter SetNext(IFileSystemFilter? nextFilter)
    {
        NextFilter = nextFilter;
        return nextFilter ?? this;
    }
    
    /// <summary>
    /// Determines whether the specified file system item passes the filter chain.
    /// Calls ProcessFilter to determine if this filter matches, then passes to next if needed.
    /// </summary>
    /// <param name="info">The file system item information.</param>
    /// <returns>True if the item passes the entire chain; otherwise, false.</returns>
    public override bool IsMatch(FileSystemInfo info)
    {
        // Process this filter's logic
        var result = ProcessFilter(info);
        
        // If this filter fails, short-circuit (don't continue chain)
        if (!result)
            return false;
        
        // If this filter passes and there's a next filter, delegate to it
        if (NextFilter != null)
            return NextFilter.IsMatch(info);
        
        // This filter passed and there's no next filter
        return true;
    }
    
    /// <summary>
    /// Processes the filter logic for this specific filter in the chain.
    /// </summary>
    /// <param name="info">The file system item information.</param>
    /// <returns>True if this filter passes; otherwise, false.</returns>
    protected abstract bool ProcessFilter(FileSystemInfo info);
    
    /// <summary>
    /// Gets a description of the filter chain.
    /// </summary>
    public override string Description
    {
        get
        {
            var currentDescription = GetFilterDescription();
            if (NextFilter != null)
                return $"{currentDescription} ? {NextFilter.Description}";
            return currentDescription;
        }
    }
    
    /// <summary>
    /// Gets the description of this specific filter in the chain.
    /// </summary>
    /// <returns>The filter description.</returns>
    protected abstract string GetFilterDescription();
}
