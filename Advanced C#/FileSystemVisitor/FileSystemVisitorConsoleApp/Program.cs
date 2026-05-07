using FileSystemVisitorConsoleApp;
using FileSystemVisitorConsoleApp.Factories;
using FileSystemVisitorConsoleApp.Interfaces;

Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
Console.WriteLine("║       FileSystemVisitor - Interactive Demo               ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

// Initialize FilterFactory for creating delegate/lambda algorithms
var filterFactory = new FilterFactory();

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
            ApplyMultipleFilters(targetDirectory, filterFactory);
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
        
        visitor.Start += (s, e) => Console.WriteLine("[Started search...]\n");
        visitor.FileFound += (s, e) => fileCount++;
        visitor.DirectoryFound += (s, e) => dirCount++;
        visitor.Finish += (s, e) => Console.WriteLine("\n[Search completed]");
        
        int displayCount = 0;
        foreach (var item in visitor.GetFileSystemItems())
        {
            string type = Directory.Exists(item) ? "[DIR]" : "[FILE]";
            Console.WriteLine($"{type} {item}");
            displayCount++;
        }
        
        Console.WriteLine($"\nTotal: {displayCount} items ({dirCount} directories, {fileCount} files)");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

/// <summary>
/// Applies multiple filters using delegate/lambda algorithms and event handlers.
/// </summary>
static void ApplyMultipleFilters(string directory, FilterFactory filterFactory)
{
    Console.WriteLine("═══ Apply Filters and Event Handlers ═══\n");
    
    var filters = new List<IFileSystemFilter>();
    string? abortTargetName = null;
    HashSet<string>? extensionsToExclude = null;
    bool addingFilters = true;
    
    while (addingFilters)
    {
        Console.WriteLine("\nAvailable options:");
        Console.WriteLine("1. File extension filter");
        Console.WriteLine("2. File size filter");
        Console.WriteLine("3. Creation date filter");
        Console.WriteLine("4. Modification date filter");
        Console.WriteLine("5. Files only filter");
        Console.WriteLine("6. Directories only filter");
        Console.WriteLine("7. Abort condition (stop at specific name)");
        Console.WriteLine("8. Exclude files by extension");
        Console.WriteLine("0. Done (apply filters and start search)");
        Console.Write("\nYour choice: ");
        
        string? choice = Console.ReadLine();
        Console.WriteLine();
        
        switch (choice)
        {
            case "1":
                var extFilter = GetExtensionFilter(filterFactory);
                if (extFilter != null) filters.Add(extFilter);
                break;
            case "2":
                var sizeFilter = GetSizeFilter(filterFactory);
                if (sizeFilter != null) filters.Add(sizeFilter);
                break;
            case "3":
                var createFilter = GetCreationDateFilter(filterFactory);
                if (createFilter != null) filters.Add(createFilter);
                break;
            case "4":
                var modifyFilter = GetModificationDateFilter(filterFactory);
                if (modifyFilter != null) filters.Add(modifyFilter);
                break;
            case "5":
                filters.Add(filterFactory.CreateFilesOnlyFilter());
                Console.WriteLine("✓ Added: Files only");
                break;
            case "6":
                filters.Add(filterFactory.CreateDirectoriesOnlyFilter());
                Console.WriteLine("✓ Added: Directories only");
                break;
            case "7":
                abortTargetName = GetAbortTargetName();
                break;
            case "8":
                extensionsToExclude = GetExtensionsToExclude();
                break;
            case "0":
                addingFilters = false;
                break;
            default:
                Console.WriteLine("Invalid choice.");
                break;
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
    ExecuteSearch(directory, filterFactory, filters, abortTargetName, extensionsToExclude);
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
    FilterFactory filterFactory, 
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
        var combinedFilter = filterFactory.CreateAndFilter(filters.ToArray());
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
            var combinedFilter = filterFactory.CreateAndFilter(filters.ToArray());
            Func<FileSystemInfo, bool> filterAlgorithm = combinedFilter.IsMatch;
            visitor = new FileSystemVisitor(directory, filterAlgorithm);
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
        
        visitor.Start += (s, e) => Console.WriteLine("[Started search...]\n");
        
        // Event handler for abort condition
        if (abortTargetName != null)
        {
            visitor.FileFound += (sender, args) =>
            {
                string fileName = Path.GetFileName(args.ItemPath);
                if (fileName.Equals(abortTargetName, StringComparison.OrdinalIgnoreCase))
                {
                    args.StopSearch = true;
                    foundTarget = true;
                    Console.WriteLine($"⛔ FOUND TARGET FILE: {args.ItemPath}");
                    Console.WriteLine("   Aborting search...\n");
                }
            };
            
            visitor.DirectoryFound += (sender, args) =>
            {
                string dirName = Path.GetFileName(args.ItemPath);
                if (dirName.Equals(abortTargetName, StringComparison.OrdinalIgnoreCase))
                {
                    args.StopSearch = true;
                    foundTarget = true;
                    Console.WriteLine($"⛔ FOUND TARGET DIRECTORY: {args.ItemPath}");
                    Console.WriteLine("   Aborting search...\n");
                }
            };
        }
        
        // Event handler for exclusions
        if (extensionsToExclude != null)
        {
            visitor.FileFound += (sender, args) =>
            {
                string extension = Path.GetExtension(args.ItemPath).ToLowerInvariant();
                if (extensionsToExclude.Contains(extension))
                {
                    args.ExcludeItem = true;
                    excludedCount++;
                    Console.WriteLine($"❌ EXCLUDED: {Path.GetFileName(args.ItemPath)} (extension: {extension})");
                }
            };
        }
        
        // Count all items
        visitor.FileFound += (s, e) => fileCount++;
        visitor.DirectoryFound += (s, e) => dirCount++;
        
        visitor.Finish += (s, e) => 
        {
            if (foundTarget)
                Console.WriteLine("\n[Search aborted - target found]");
            else
                Console.WriteLine("\n[Search completed]");
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
        Console.WriteLine($"\nTotal scanned: {fileCount} files, {dirCount} directories");
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

static IFileSystemFilter? GetExtensionFilter(FilterFactory filterFactory)
{
    Console.WriteLine("Enter file extension(s) separated by comma (e.g., .txt, .cs, .pdf):");
    string? input = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine("No extension provided.");
        return null;
    }
    
    var extensions = input.Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(e => e.Trim())
        .ToArray();
    
    var filter = filterFactory.CreateExtensionFilter(extensions);
    Console.WriteLine($"✓ Added: {filter.Description}");
    return filter;
}

static IFileSystemFilter? GetSizeFilter(FilterFactory filterFactory)
{
    Console.WriteLine("1. Larger than");
    Console.WriteLine("2. Smaller than");
    Console.WriteLine("3. Range");
    Console.Write("Choose size filter: ");
    
    string? choice = Console.ReadLine();
    
    IFileSystemFilter? filter = null;
    
    switch (choice)
    {
        case "1":
            Console.Write("Enter minimum size in KB: ");
            if (long.TryParse(Console.ReadLine(), out long minSize))
            {
                filter = filterFactory.CreateSizeFilter(minSize * 1024, null);
            }
            break;
        case "2":
            Console.Write("Enter maximum size in KB: ");
            if (long.TryParse(Console.ReadLine(), out long maxSize))
            {
                filter = filterFactory.CreateSizeFilter(null, maxSize * 1024);
            }
            break;
        case "3":
            Console.Write("Enter minimum size in KB: ");
            if (!long.TryParse(Console.ReadLine(), out long min))
            {
                Console.WriteLine("Invalid size.");
                return null;
            }
            Console.Write("Enter maximum size in KB: ");
            if (!long.TryParse(Console.ReadLine(), out long max))
            {
                Console.WriteLine("Invalid size.");
                return null;
            }
            filter = filterFactory.CreateSizeFilter(min * 1024, max * 1024);
            break;
    }
    
    if (filter != null)
    {
        Console.WriteLine($"✓ Added: {filter.Description}");
    }
    else
    {
        Console.WriteLine("Invalid input.");
    }
    
    return filter;
}

static IFileSystemFilter? GetCreationDateFilter(FilterFactory filterFactory)
{
    Console.WriteLine("1. Created in last N days");
    Console.WriteLine("2. Created after specific date");
    Console.WriteLine("3. Created before specific date");
    Console.WriteLine("4. Created today");
    Console.WriteLine("5. Created in date range");
    Console.Write("Choose creation date filter: ");
    
    string? choice = Console.ReadLine();
    
    IFileSystemFilter? filter = null;
    
    switch (choice)
    {
        case "1":
            Console.Write("Enter number of days: ");
            if (int.TryParse(Console.ReadLine(), out int days))
            {
                filter = filterFactory.CreateCreationDateFilter(DateTime.Now.AddDays(-days), null);
            }
            break;
        case "2":
            Console.Write("Enter date (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime afterDate))
            {
                filter = filterFactory.CreateCreationDateFilter(afterDate, null);
            }
            break;
        case "3":
            Console.Write("Enter date (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime beforeDate))
            {
                filter = filterFactory.CreateCreationDateFilter(null, beforeDate);
            }
            break;
        case "4":
            var today = DateTime.Today;
            filter = filterFactory.CreateCreationDateFilter(today, today.AddDays(1).AddTicks(-1));
            break;
        case "5":
            Console.Write("Enter start date (yyyy-MM-dd) or press Enter to skip: ");
            string? startInput = Console.ReadLine();
            DateTime? startDate = null;
            if (!string.IsNullOrWhiteSpace(startInput) && DateTime.TryParse(startInput, out DateTime parsedStart))
            {
                startDate = parsedStart;
            }
            
            Console.Write("Enter end date (yyyy-MM-dd) or press Enter to skip: ");
            string? endInput = Console.ReadLine();
            DateTime? endDate = null;
            if (!string.IsNullOrWhiteSpace(endInput) && DateTime.TryParse(endInput, out DateTime parsedEnd))
            {
                endDate = parsedEnd;
            }
            
            if (!startDate.HasValue && !endDate.HasValue)
            {
                Console.WriteLine("At least one date must be provided.");
                return null;
            }
            
            filter = filterFactory.CreateCreationDateFilter(startDate, endDate);
            break;
    }
    
    if (filter != null)
    {
        Console.WriteLine($"✓ Added: {filter.Description}");
    }
    else
    {
        Console.WriteLine("Invalid input.");
    }
    
    return filter;
}

static IFileSystemFilter? GetModificationDateFilter(FilterFactory filterFactory)
{
    Console.WriteLine("1. Modified in last N days");
    Console.WriteLine("2. Modified after specific date");
    Console.WriteLine("3. Modified before specific date");
    Console.WriteLine("4. Modified today");
    Console.WriteLine("5. Modified in date range");
    Console.Write("Choose modification date filter: ");
    
    string? choice = Console.ReadLine();
    
    IFileSystemFilter? filter = null;
    
    switch (choice)
    {
        case "1":
            Console.Write("Enter number of days: ");
            if (int.TryParse(Console.ReadLine(), out int days))
            {
                filter = filterFactory.CreateModificationDateFilter(DateTime.Now.AddDays(-days), null);
            }
            break;
        case "2":
            Console.Write("Enter date (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime afterDate))
            {
                filter = filterFactory.CreateModificationDateFilter(afterDate, null);
            }
            break;
        case "3":
            Console.Write("Enter date (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime beforeDate))
            {
                filter = filterFactory.CreateModificationDateFilter(null, beforeDate);
            }
            break;
        case "4":
            var today = DateTime.Today;
            filter = filterFactory.CreateModificationDateFilter(today, today.AddDays(1).AddTicks(-1));
            break;
        case "5":
            Console.Write("Enter start date (yyyy-MM-dd) or press Enter to skip: ");
            string? startInput = Console.ReadLine();
            DateTime? startDate = null;
            if (!string.IsNullOrWhiteSpace(startInput) && DateTime.TryParse(startInput, out DateTime parsedStart))
            {
                startDate = parsedStart;
            }
            
            Console.Write("Enter end date (yyyy-MM-dd) or press Enter to skip: ");
            string? endInput = Console.ReadLine();
            DateTime? endDate = null;
            if (!string.IsNullOrWhiteSpace(endInput) && DateTime.TryParse(endInput, out DateTime parsedEnd))
            {
                endDate = parsedEnd;
            }
            
            if (!startDate.HasValue && !endDate.HasValue)
            {
                Console.WriteLine("At least one date must be provided.");
                return null;
            }
            
            filter = filterFactory.CreateModificationDateFilter(startDate, endDate);
            break;
    }
    
    if (filter != null)
    {
        Console.WriteLine($"✓ Added: {filter.Description}");
    }
    else
    {
        Console.WriteLine("Invalid input.");
    }
    
    return filter;
}
