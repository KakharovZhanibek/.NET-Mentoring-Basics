using System.Text.Json;
using Reflection.Contracts;

namespace Reflection.Plugins.FileProvider;

/// <summary>
/// Configuration provider that reads/writes settings from/to a JSON file
/// </summary>
public class FileConfigurationProvider : IConfigurationProvider
{
    private readonly string _filePath;
    private Dictionary<string, string> _settings;

    public string ProviderTypeName => "File";

    /// <summary>
    /// Initializes a new instance of FileConfigurationProvider with default file path
    /// </summary>
    public FileConfigurationProvider() : this("settings.json")
    {
    }

    /// <summary>
    /// Initializes a new instance of FileConfigurationProvider
    /// </summary>
    /// <param name="filePath">Path to the configuration file</param>
    public FileConfigurationProvider(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        
        // Make the path absolute if it's relative
        if (!Path.IsPathRooted(_filePath))
        {
            // Option 3: Store in project root (development approach)
            // Navigate from bin\Debug\net9.0 up to project root
            var projectRoot = GetProjectRoot();
            _filePath = Path.Combine(projectRoot, _filePath);
        }
        
        _settings = new Dictionary<string, string>();
        LoadFile();
    }

    /// <summary>
    /// Gets the project root directory by walking up from the base directory
    /// </summary>
    private static string GetProjectRoot()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var directory = new DirectoryInfo(baseDirectory);
        
        // Walk up from bin\Debug\net9.0 to project root
        // Typically: bin\Debug\net9.0 -> bin\Debug -> bin -> ProjectRoot
        if (directory.Parent?.Parent?.Parent != null)
        {
            return directory.Parent.Parent.Parent.FullName;
        }
        
        // Fallback to base directory if structure is different
        return baseDirectory;
    }

    private void LoadFile()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                _settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json) 
                    ?? new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load configuration file '{_filePath}': {ex.Message}");
                _settings = new Dictionary<string, string>();
            }
        }
    }

    private void SaveFile()
    {
        try
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Could not save configuration file '{_filePath}': {ex.Message}");
        }
    }

    public string? ReadSetting(string settingName)
    {
        return _settings.TryGetValue(settingName, out var value) ? value : null;
    }

    public void WriteSetting(string settingName, string value)
    {
        _settings[settingName] = value;
        SaveFile();
    }
}
