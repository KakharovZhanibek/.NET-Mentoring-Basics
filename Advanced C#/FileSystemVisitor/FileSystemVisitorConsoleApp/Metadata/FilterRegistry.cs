using FileSystemVisitorConsoleApp.Filters;
using FileSystemVisitorConsoleApp.Interfaces;

namespace FileSystemVisitorConsoleApp.Metadata;

/// <summary>
/// Registry of available filters with their metadata.
/// </summary>
public static class FilterRegistry
{
    /// <summary>
    /// Gets all available filter metadata.
    /// </summary>
    public static FilterMetadata[] AvailableFilters { get; } = 
    [
        // Extension Filter
        new FilterMetadata(
            Name: "Extension Filter",
            Description: "Filter files by extension(s)",
            FilterType: typeof(ExtensionFilter),
            Parameters: 
            [
                new FilterParameter(
                    Name: "extensions",
                    ParameterType: typeof(string[]),
                    Prompt: "Enter file extension(s) separated by comma (e.g., .txt, .cs, .pdf):",
                    IsRequired: true)
            ],
            Factory: args => new ExtensionFilter((string[])args[0]!)),

        // Size Filter
        new FilterMetadata(
            Name: "Size Filter",
            Description: "Filter files by size",
            FilterType: typeof(SizeFilter),
            Parameters:
            [
                new FilterParameter(
                    Name: "minSize",
                    ParameterType: typeof(long?),
                    Prompt: "Enter minimum size in KB (or press Enter to skip):",
                    IsRequired: false),
                new FilterParameter(
                    Name: "maxSize",
                    ParameterType: typeof(long?),
                    Prompt: "Enter maximum size in KB (or press Enter to skip):",
                    IsRequired: false)
            ],
            Factory: args => new SizeFilter(
                (args[0] as long?) * 1024,  // Convert KB to bytes
                (args[1] as long?) * 1024)),

        // Creation Date Filter
        new FilterMetadata(
            Name: "Creation Date Filter",
            Description: "Filter by creation date",
            FilterType: typeof(CreationDateFilter),
            Parameters:
            [
                new FilterParameter(
                    Name: "startDate",
                    ParameterType: typeof(DateTime?),
                    Prompt: "Enter start date (yyyy-MM-dd) or press Enter to skip:",
                    IsRequired: false),
                new FilterParameter(
                    Name: "endDate",
                    ParameterType: typeof(DateTime?),
                    Prompt: "Enter end date (yyyy-MM-dd) or press Enter to skip:",
                    IsRequired: false)
            ],
            Factory: args => new CreationDateFilter(
                args[0] as DateTime?,
                args[1] as DateTime?)),

        // Modification Date Filter
        new FilterMetadata(
            Name: "Modification Date Filter",
            Description: "Filter by modification date",
            FilterType: typeof(ModificationDateFilter),
            Parameters:
            [
                new FilterParameter(
                    Name: "startDate",
                    ParameterType: typeof(DateTime?),
                    Prompt: "Enter start date (yyyy-MM-dd) or press Enter to skip:",
                    IsRequired: false),
                new FilterParameter(
                    Name: "endDate",
                    ParameterType: typeof(DateTime?),
                    Prompt: "Enter end date (yyyy-MM-dd) or press Enter to skip:",
                    IsRequired: false)
            ],
            Factory: args => new ModificationDateFilter(
                args[0] as DateTime?,
                args[1] as DateTime?)),

        // Files Only Filter
        new FilterMetadata(
            Name: "Files Only",
            Description: "Show only files",
            FilterType: typeof(FilesOnlyFilter),
            Parameters: [],
            Factory: args => new FilesOnlyFilter()),

        // Directories Only Filter
        new FilterMetadata(
            Name: "Directories Only",
            Description: "Show only directories",
            FilterType: typeof(DirectoriesOnlyFilter),
            Parameters: [],
            Factory: args => new DirectoriesOnlyFilter())
    ];
}
