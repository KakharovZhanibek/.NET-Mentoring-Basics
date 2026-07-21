using Microsoft.Extensions.Configuration;
using System.Text.Json;
using IConfigurationProvider = Reflection.Contracts.IConfigurationProvider;

namespace Reflection.Plugins.ConfigurationManagerProvider;

/// <summary>
/// Configuration provider that reads/writes settings using Microsoft.Extensions.Configuration
/// </summary>
public class ConfigurationManagerConfigurationProvider : IConfigurationProvider
{
    private readonly string _filePath;
    private IConfiguration _configuration;

    public string ProviderTypeName => "ConfigurationManager";

    /// <summary>
    /// Initializes a new instance of ConfigurationManagerConfigurationProvider with default file path
    /// </summary>
    public ConfigurationManagerConfigurationProvider() : this("appsettings.json")
    {
    }

    /// <summary>
    /// Initializes a new instance of ConfigurationManagerConfigurationProvider
    /// </summary>
    /// <param name="filePath">Path to the appsettings.json file</param>
    public ConfigurationManagerConfigurationProvider(string filePath)
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
        
        _configuration = null!;
        LoadConfiguration();
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

    private void LoadConfiguration()
    {
        var directory = Path.GetDirectoryName(_filePath);
        var fileName = Path.GetFileName(_filePath);
        
        // Ensure we have an absolute path for SetBasePath
        var basePath = string.IsNullOrEmpty(directory) 
            ? AppDomain.CurrentDomain.BaseDirectory 
            : directory;
        
        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile(fileName, optional: true, reloadOnChange: false);

        _configuration = builder.Build();
    }

    public string? ReadSetting(string settingName)
    {
        return _configuration[settingName];
    }

    public void WriteSetting(string settingName, string value)
    {
        // For writing, we need to modify the JSON file directly
        var fileInfo = new FileInfo(_filePath);
        
        if (!fileInfo.Exists)
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // Create a new file with the setting
            File.WriteAllText(_filePath, $"{{\n  \"{settingName}\": \"{value}\"\n}}");
        }
        else
        {
            // Read existing JSON
            var json = File.ReadAllText(_filePath);
            var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(json) 
                ?? new Dictionary<string, object>();
            
            settings[settingName] = value;
            
            // Write back
            var updatedJson = JsonSerializer.Serialize(settings, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(_filePath, updatedJson);
        }

        // Reload configuration
        LoadConfiguration();
    }
}
