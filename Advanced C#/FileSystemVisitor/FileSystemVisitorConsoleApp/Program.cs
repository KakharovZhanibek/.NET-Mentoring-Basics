using FileSystemVisitorConsoleApp;
using FileSystemVisitorConsoleApp.Events;
using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Interfaces;
using FileSystemVisitorConsoleApp.Metadata;
using FileSystemVisitorConsoleApp.Services;

Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
Console.WriteLine("║       FileSystemVisitor - Interactive Demo               ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

// Get directory path from user
string targetDirectory = GetDirectoryFromUser();

// Main menu loop
bool running = true;
while (running)
{
    Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
    Console.WriteLine($"║ Current Directory: {targetDirectory.PadRight(38)}║");
    Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
    
    DisplayMainMenu();
    
    string? choice = Console.ReadLine();
    Console.WriteLine();
    
    switch (choice)
    {
        case "1":
            ShowAllItems(targetDirectory);
            break;
        case "2":
            ApplyMultipleFilters(targetDirectory);
            break;
        case "3":
            targetDirectory = GetDirectoryFromUser();
            break;
        case "0":
            running = false;
            Console.WriteLine("Goodbye!");
            break;
        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }
    
    if (running && choice != "3")
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}

// ============================================================================
// HELPER METHODS
// ============================================================================

static string GetDirectoryFromUser()
{
    while (true)
    {
        Console.WriteLine("Enter the directory path to traverse (or press Enter to use current directory):");
        string? userInput = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(userInput))
        {
            string currentDir = Directory.GetCurrentDirectory();
            Console.WriteLine($"Using current directory: {currentDir}\n");
            return currentDir;
        }
        
        if (Directory.Exists(userInput))
        {
            Console.WriteLine($"Directory found: {userInput}\n");
            return userInput;
        }
        
        Console.WriteLine($"Error: Directory '{userInput}' does not exist. Please try again.\n");
    }
}

static void DisplayMainMenu()
{
    Console.WriteLine("\n┌───────────────────────────────────────────────────────────┐");
    Console.WriteLine("│ Choose option:                                            │");
    Console.WriteLine("├───────────────────────────────────────────────────────────┤");
    Console.WriteLine("│ 1. Show all files and folders (no filter)                 │");
    Console.WriteLine("│ 2. Apply filters (can combine multiple)                   │");
    Console.WriteLine("│ 3. Change directory                                       │");
    Console.WriteLine("│ 0. Exit                                                   │");
    Console.WriteLine("└───────────────────────────────────────────────────────────┘");
    Console.Write("Your choice: ");
}

static void ShowAllItems(string directory)
{
    Console.WriteLine("═══ All Files and Folders ═══\n");
    
    try
    {
        var visitor = new FileSystemVisitor(directory);
        
        int fileCount = 0;
        int dirCount = 0;
        int filteredFileCount = 0;
        int filteredDirCount = 0;
        
        // Using the new single event with message
        visitor.ItemProcessed += (s, e) =>
        {
            switch (e.EventType)
            {
                case FileSystemEventType.Start:
                    Console.WriteLine($"[{e.Message}]\n");
                    break;
                case FileSystemEventType.Finish:
                    Console.WriteLine($"\n[{e.Message}]");
                    break;
                case FileSystemEventType.FileFound:
                    fileCount++;
                    break;
                case FileSystemEventType.DirectoryFound:
                    dirCount++;
                    break;
                case FileSystemEventType.FilteredFileFound:
                    filteredFileCount++;
                    break;
                case FileSystemEventType.FilteredDirectoryFound:
                    filteredDirCount++;
                    break;
            }
        };
        
        int displayCount = 0;
        foreach (var item in visitor.GetFileSystemItems())
        {
            string type = Directory.Exists(item) ? "[DIR]" : "[FILE]";
            Console.WriteLine($"{type} {item}");
            displayCount++;
        }
        
        Console.WriteLine($"\nTotal found: {fileCount} files, {dirCount} directories");
        Console.WriteLine($"Passed filter: {filteredFileCount} files, {filteredDirCount} directories");
        Console.WriteLine($"Final results: {displayCount} items");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

/// <summary>
/// Applies multiple filters using a metadata-driven approach.
/// </summary>
static void ApplyMultipleFilters(string directory)
{
    Console.WriteLine("═══ Apply Filters and Event Handlers ═══\n");
    
    var filters = new List<IFileSystemFilter>();
    string? abortTargetName = null;
    HashSet<string>? extensionsToExclude = null;
    bool addingFilters = true;
    
    while (addingFilters)
    {
        // Dynamically render filter options from registry
        Console.WriteLine("\nAvailable filters:");
        var availableFilters = FilterRegistry.AvailableFilters;
        
        for (int i = 0; i < availableFilters.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {availableFilters[i].Name} - {availableFilters[i].Description}");
        }
        
        Console.WriteLine($"{availableFilters.Length + 1}. Abort condition (stop at specific name)");
        Console.WriteLine($"{availableFilters.Length + 2}. Exclude files by extension");
        Console.WriteLine("0. Done (apply filters and start search)");
        Console.Write("\nYour choice: ");
        
        string? choice = Console.ReadLine();
        Console.WriteLine();
        
        if (choice == "0")
        {
            addingFilters = false;
            continue;
        }
        
        // Handle filter selection
        if (int.TryParse(choice, out int filterIndex) && filterIndex >= 1 && filterIndex <= availableFilters.Length)
        {
            var selectedFilter = availableFilters[filterIndex - 1];
            var filter = CreateFilterFromMetadata(selectedFilter);
            
            if (filter != null)
            {
                filters.Add(filter);
                Console.WriteLine($"✓ Added: {filter.Description}");
            }
        }
        // Handle special options (abort, exclude)
        else if (filterIndex == availableFilters.Length + 1)
        {
            abortTargetName = GetAbortTargetName();
        }
        else if (filterIndex == availableFilters.Length + 2)
        {
            extensionsToExclude = GetExtensionsToExclude();
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }
        
        if (addingFilters && choice != "0")
        {
            Console.WriteLine($"\nCurrent settings:");
            Console.WriteLine($"  - Filters: {filters.Count}");
            Console.WriteLine($"  - Abort condition: {(abortTargetName != null ? $"Stop when '{abortTargetName}' found" : "None")}");
            Console.WriteLine($"  - Exclusions: {(extensionsToExclude != null ? string.Join(", ", extensionsToExclude) : "None")}");
            Console.Write("\nAdd another option? (y/n): ");
            string? continueChoice = Console.ReadLine();
            if (continueChoice?.ToLower() != "y")
            {
                addingFilters = false;
            }
        }
    }
    
    // Execute search with configured options
    ExecuteSearch(directory, filters, abortTargetName, extensionsToExclude);
}

/// <summary>
/// Creates a filter from metadata by collecting parameters from user.
/// </summary>
static IFileSystemFilter? CreateFilterFromMetadata(FilterMetadata metadata)
{
    try
    {
        Console.WriteLine($"\nConfiguring: {metadata.Name}");
        
        // If no parameters required, create immediately
        if (metadata.Parameters.Length == 0)
        {
            return metadata.CreateFilter([]);
        }
        
        // Collect parameter values
        var parameterValues = new object?[metadata.Parameters.Length];
        
        for (int i = 0; i < metadata.Parameters.Length; i++)
        {
            var param = metadata.Parameters[i];
            parameterValues[i] = ParameterInputService.GetParameterValue(
                param.ParameterType,
                param.Prompt,
                param.IsRequired);
        }
        
        // Validate that at least one parameter is provided for optional filters
        if (metadata.Parameters.All(p => !p.IsRequired) && parameterValues.All(v => v == null))
        {
            Console.WriteLine("At least one parameter must be provided.");
            return null;
        }
        
        // Create the filter
        return metadata.CreateFilter(parameterValues);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating filter: {ex.Message}");
        return null;
    }
}

/// <summary>
/// Gets the target name for abort condition from user.
/// </summary>
static string? GetAbortTargetName()
{
    Console.WriteLine("Enter the file or folder name to stop search when found:");
    Console.Write("> ");
    string? targetName = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(targetName))
    {
        Console.WriteLine("No name provided. Abort condition not set.");
        return null;
    }
    
    Console.WriteLine($"✓ Abort condition set: Stop when '{targetName}' is found");
    return targetName;
}

/// <summary>
/// Gets the extensions to exclude from user.
/// </summary>
static HashSet<string>? GetExtensionsToExclude()
{
    Console.WriteLine("Enter file extensions to exclude (comma-separated, e.g., .log,.tmp,.bak):");
    Console.Write("> ");
    string? extensionsInput = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(extensionsInput))
    {
        Console.WriteLine("No extensions provided. Exclusion not set.");
        return null;
    }
    
    var extensions = extensionsInput
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(ext => ext.Trim().ToLowerInvariant())
        .Select(ext => ext.StartsWith(".") ? ext : "." + ext)
        .ToHashSet();
    
    Console.WriteLine($"✓ Exclusion set: Files with extensions {string.Join(", ", extensions)} will be excluded");
    return extensions;
}

/// <summary>
/// Executes the search with all configured filters and event handlers.
/// </summary>
static void ExecuteSearch(
    string directory, 
    List<IFileSystemFilter> filters, 
    string? abortTargetName, 
    HashSet<string>? extensionsToExclude)
{
    if (filters.Count == 0 && abortTargetName == null && extensionsToExclude == null)
    {
        Console.WriteLine("\nNo filters or event handlers configured. Showing all items.");
        ShowAllItems(directory);
        return;
    }
    
    Console.WriteLine($"\n═══ Starting Search ═══\n");
    
    if (filters.Count > 0)
    {
        var combinedFilter = new AndFilter(filters);
        Console.WriteLine($"Filters: {combinedFilter.Description}");
    }
    if (abortTargetName != null)
    {
        Console.WriteLine($"Abort: Will stop when '{abortTargetName}' is found");
    }
    if (extensionsToExclude != null)
    {
        Console.WriteLine($"Exclusions: {string.Join(", ", extensionsToExclude)}");
    }
    Console.WriteLine();
    
    try
    {
        // Create visitor with filter if any
        FileSystemVisitor visitor;
        if (filters.Count > 0)
        {
            var combinedFilter = new AndFilter(filters);
            visitor = new FileSystemVisitor(directory, combinedFilter.IsMatch);
        }
        else
        {
            visitor = new FileSystemVisitor(directory);
        }
        
        // Track statistics
        int fileCount = 0;
        int dirCount = 0;
        int excludedCount = 0;
        bool foundTarget = false;
        long totalSize = 0;
        int filteredFileCount = 0;
        int filteredDirCount = 0;
        
        // Using the new single event for all operations
        visitor.ItemProcessed += (sender, args) =>
        {
            switch (args.EventType)
            {
                case FileSystemEventType.Start:
                    Console.WriteLine($"[{args.Message}]\n");
                    break;
                    
                case FileSystemEventType.Finish:
                    if (foundTarget)
                        Console.WriteLine($"\n[Search aborted - target found]");
                    else
                        Console.WriteLine($"\n[{args.Message}]");
                    break;
                    
                case FileSystemEventType.FileFound:
                    fileCount++;
                    
                    // Handle abort condition
                    if (abortTargetName != null && args.ItemPath != null)
                    {
                        string fileName = Path.GetFileName(args.ItemPath);
                        if (fileName.Equals(abortTargetName, StringComparison.OrdinalIgnoreCase))
                        {
                            args.StopSearch = true;
                            foundTarget = true;
                            Console.WriteLine($"⛔ FOUND TARGET FILE: {args.ItemPath}");
                            Console.WriteLine("   Aborting search...\n");
                        }
                    }
                    
                    // Handle exclusions
                    if (extensionsToExclude != null && args.ItemPath != null)
                    {
                        string extension = Path.GetExtension(args.ItemPath).ToLowerInvariant();
                        if (extensionsToExclude.Contains(extension))
                        {
                            args.ExcludeItem = true;
                            excludedCount++;
                            Console.WriteLine($"❌ EXCLUDED: {Path.GetFileName(args.ItemPath)} (extension: {extension})");
                        }
                    }
                    break;
                    
                case FileSystemEventType.DirectoryFound:
                    dirCount++;
                    
                    // Handle abort condition
                    if (abortTargetName != null && args.ItemPath != null)
                    {
                        string dirName = Path.GetFileName(args.ItemPath);
                        if (dirName.Equals(abortTargetName, StringComparison.OrdinalIgnoreCase))
                        {
                            args.StopSearch = true;
                            foundTarget = true;
                            Console.WriteLine($"⛔ FOUND TARGET DIRECTORY: {args.ItemPath}");
                            Console.WriteLine("   Aborting search...\n");
                        }
                    }
                    break;
                    
                case FileSystemEventType.FilteredFileFound:
                    filteredFileCount++;
                    break;
                    
                case FileSystemEventType.FilteredDirectoryFound:
                    filteredDirCount++;
                    break;
            }
        };
        
        // Display results
        int displayCount = 0;
        foreach (var item in visitor.GetFileSystemItems())
        {
            if (File.Exists(item))
            {
                var fileInfo = new FileInfo(item);
                totalSize += fileInfo.Length;
                Console.WriteLine($"✓ [FILE] {Path.GetFileName(item)} - {fileInfo.Length / 1024.0:F2} KB");
            }
            else
            {
                Console.WriteLine($"✓ [DIR]  {item}");
            }
            displayCount++;
        }
        
        // Summary
        Console.WriteLine($"\nTotal found: {fileCount} files, {dirCount} directories");
        if ($"{directory}".Contains(".."))
        {
            Console.WriteLine($"[WARNING] The directory path contains relative segments (..): {directory}");
        }
        if (filters.Count > 0)
        {
            Console.WriteLine($"Passed filter: {filteredFileCount} files, {filteredDirCount} directories");
        }
        if (excludedCount > 0)
        {
            Console.WriteLine($"Excluded: {excludedCount} files");
        }
        Console.WriteLine($"Final results: {displayCount} items");
        if (totalSize > 0)
        {
            Console.WriteLine($"Total size: {totalSize / (1024.0 * 1024.0):F2} MB");
        }
        
        if (foundTarget)
        {
            Console.WriteLine($"\nStatus: Search was aborted when '{abortTargetName}' was found.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
