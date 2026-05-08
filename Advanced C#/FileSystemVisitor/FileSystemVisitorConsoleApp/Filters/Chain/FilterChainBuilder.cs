using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Interfaces;

namespace FileSystemVisitorConsoleApp.Filters.Chain;

/// <summary>
/// Builder class for creating filter chains using a fluent API.
/// </summary>
public class FilterChainBuilder
{
    private ChainFilterBase? _first;
    private ChainFilterBase? _current;
    
    /// <summary>
    /// Adds an extension filter to the chain.
    /// </summary>
    /// <param name="extension">The file extension to check.</param>
    /// <returns>The builder for fluent chaining.</returns>
    public FilterChainBuilder WithExtension(string extension)
    {
        AddFilter(new ExtensionChainFilter(extension));
        return this;
    }
    
    /// <summary>
    /// Adds a size filter to the chain.
    /// </summary>
    /// <param name="minSize">Minimum file size in bytes (null for no minimum).</param>
    /// <param name="maxSize">Maximum file size in bytes (null for no maximum).</param>
    /// <returns>The builder for fluent chaining.</returns>
    public FilterChainBuilder WithSize(long? minSize, long? maxSize)
    {
        AddFilter(new SizeChainFilter(minSize, maxSize));
        return this;
    }
    
    /// <summary>
    /// Adds a modification date filter to the chain.
    /// </summary>
    /// <param name="startDate">Start date (null for no start date).</param>
    /// <param name="endDate">End date (null for no end date).</param>
    /// <returns>The builder for fluent chaining.</returns>
    public FilterChainBuilder WithModificationDate(DateTime? startDate, DateTime? endDate)
    {
        AddFilter(new ModificationDateChainFilter(startDate, endDate));
        return this;
    }
    
    /// <summary>
    /// Adds a file type filter to the chain.
    /// </summary>
    /// <param name="filesOnly">True to accept only files; false to accept only directories.</param>
    /// <returns>The builder for fluent chaining.</returns>
    public FilterChainBuilder WithFileType(bool filesOnly = true)
    {
        AddFilter(new FileTypeChainFilter(filesOnly));
        return this;
    }
    
    /// <summary>
    /// Adds a custom filter to the chain.
    /// </summary>
    /// <param name="filter">The filter to add.</param>
    /// <returns>The builder for fluent chaining.</returns>
    public FilterChainBuilder WithCustomFilter(ChainFilterBase filter)
    {
        AddFilter(filter);
        return this;
    }
    
    /// <summary>
    /// Builds and returns the filter chain.
    /// </summary>
    /// <returns>The first filter in the chain, or null if no filters were added.</returns>
    public IFileSystemFilter? Build()
    {
        return _first;
    }
    
    /// <summary>
    /// Adds a filter to the chain.
    /// </summary>
    private void AddFilter(ChainFilterBase filter)
    {
        if (_first == null)
        {
            _first = filter;
            _current = filter;
        }
        else if (_current != null)
        {
            _current.SetNext(filter);
            _current = filter;
        }
    }
}
