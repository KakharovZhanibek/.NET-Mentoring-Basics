using System.Reflection;
using FileSystemVisitorConsoleApp.Interfaces;
using FileSystemVisitorConsoleApp.Metadata;

namespace FileSystemVisitorConsoleApp.Services;

/// <summary>
/// Discovers filters that implement IFilterMetadata using reflection.
/// </summary>
public static class FilterDiscovery
{
    private static FilterMetadata[]? _cachedFilters;
    
    /// <summary>
    /// Gets all available filters with their metadata by discovering types that implement IFilterMetadata.
    /// </summary>
    public static FilterMetadata[] DiscoverFilters()
    {
        if (_cachedFilters != null)
            return _cachedFilters;
        
        var filterMetadataList = new List<FilterMetadata>();
        
        // Get all types in the current assembly
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();
        
        foreach (var type in types)
        {
            // Check if type implements IFilterMetadata
            if (type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(IFilterMetadata)))
            {
                try
                {
                    // Get static properties using reflection
                    var filterNameProp = type.GetProperty("FilterName", BindingFlags.Public | BindingFlags.Static);
                    var filterDescProp = type.GetProperty("FilterDescription", BindingFlags.Public | BindingFlags.Static);
                    var createMethod = type.GetMethod("CreateFromUserInput", BindingFlags.Public | BindingFlags.Static);
                    
                    if (filterNameProp == null || filterDescProp == null || createMethod == null)
                        continue;
                    
                    var filterName = filterNameProp.GetValue(null) as string;
                    var filterDescription = filterDescProp.GetValue(null) as string;
                    
                    if (filterName == null || filterDescription == null)
                        continue;
                    
                    // Create the factory delegate that calls CreateFromUserInput
                    var factory = new Func<object?[], IFileSystemFilter>(args =>
                    {
                        // CreateFromUserInput doesn't take parameters, call it directly
                        return (IFileSystemFilter)createMethod.Invoke(null, null)!;
                    });
                    
                    // Create FilterMetadata with empty parameters (not used anymore)
                    var metadata = new FilterMetadata(
                        Name: filterName,
                        Description: filterDescription,
                        FilterType: type,
                        Parameters: [], // Empty - each filter handles its own input
                        Factory: factory);
                    
                    filterMetadataList.Add(metadata);
                }
                catch
                {
                    // Skip types that can't be processed
                    continue;
                }
            }
        }
        
        _cachedFilters = [.. filterMetadataList.OrderBy(f => f.Name)];
        return _cachedFilters;
    }
    
    /// <summary>
    /// Clears the cached filters, forcing rediscovery on next call.
    /// </summary>
    public static void ClearCache()
    {
        _cachedFilters = null;
    }
}
