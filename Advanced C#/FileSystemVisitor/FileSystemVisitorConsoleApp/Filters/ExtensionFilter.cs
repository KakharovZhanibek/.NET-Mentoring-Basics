using FileSystemVisitorConsoleApp.Filters.Base;
using FileSystemVisitorConsoleApp.Interfaces;

namespace FileSystemVisitorConsoleApp.Filters;

/// <summary>
/// Filters files based on file extension.
/// </summary>
public class ExtensionFilter : FileSystemFilterBase, IFilterMetadata
{
    private readonly HashSet<string> _extensions;
    
    public ExtensionFilter(params string[] extensions)
    {
        if (extensions == null || extensions.Length == 0)
            throw new ArgumentException("At least one extension must be provided.", nameof(extensions));
            
        _extensions = extensions
            .Select(e => e.StartsWith(".") ? e : "." + e)
            .Select(e => e.ToLowerInvariant())
            .ToHashSet();
    }
    
    public override bool IsMatch(FileSystemInfo info)
    {
        if (info is not FileInfo)
            return false;
            
        return _extensions.Contains(info.Extension.ToLowerInvariant());
    }
    
    public override string Description =>
        $"Extension: {string.Join(", ", _extensions)}";
    
    // IFilterMetadata implementation
    public static string FilterName => "Extension Filter";
    
    public static string FilterDescription => "Filter files by extension(s)";
    
    public static IFileSystemFilter? CreateFromUserInput()
    {
        Console.Write("Enter file extension(s) separated by comma (e.g., .txt, .cs, .pdf): ");
        string? input = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("? No extensions provided.");
            return null;
        }
        
        var extensions = input
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(ext => ext.Trim())
            .ToArray();
        
        if (extensions.Length == 0)
        {
            Console.WriteLine("? No valid extensions provided.");
            return null;
        }
        
        return new ExtensionFilter(extensions);
    }
}
