using System.Globalization;
using System.Reflection;
using Reflection.Contracts;

namespace Reflection;

/// <summary>
/// Base class for components that support configuration binding via attributes
/// </summary>
public abstract class ConfigurationComponentBase
{
    private static readonly Dictionary<string, Contracts.IConfigurationProvider> _loadedProviders = new();
    private static bool _providersInitialized = false;

    /// <summary>
    /// Initializes a new instance of ConfigurationComponentBase
    /// </summary>
    protected ConfigurationComponentBase()
    {
        if (!_providersInitialized)
        {
            InitializeProviders();
            _providersInitialized = true;
        }
    }

    /// <summary>
    /// Initializes configuration providers by loading them from plugin assemblies
    /// </summary>
    private static void InitializeProviders()
    {
        Console.WriteLine("\n=== Initializing Configuration Providers ===");
        
        // Determine the plugins directory (in the output directory)
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var pluginDirectory = Path.Combine(baseDirectory, "plugins");
        
        Console.WriteLine($"Plugin directory: {pluginDirectory}");
        
        var providers = PluginLoader.LoadProviders(pluginDirectory);
        
        foreach (var provider in providers)
        {
            _loadedProviders[provider.Key] = provider.Value;
        }
        
        Console.WriteLine($"Total providers loaded: {_loadedProviders.Count}");
        Console.WriteLine("=============================================\n");
    }

    /// <summary>
    /// Gets a provider by its type name
    /// </summary>
    private Contracts.IConfigurationProvider? GetProvider(ConfigurationProviderType providerType)
    {
        var providerTypeName = providerType.ToString();
        
        if (_loadedProviders.TryGetValue(providerTypeName, out var provider))
        {
            return provider;
        }
        
        Console.WriteLine($"Warning: Provider '{providerTypeName}' not found. Available providers: {string.Join(", ", _loadedProviders.Keys)}");
        return null;
    }

    /// <summary>
    /// Loads settings from the configured providers into properties marked with ConfigurationItemAttribute
    /// </summary>
    public void LoadSettings()
    {
        var properties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.GetCustomAttribute<ConfigurationItemAttribute>() != null);

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<ConfigurationItemAttribute>()!;
            var provider = GetProvider(attribute.ProviderType);
            
            if (provider == null)
            {
                Console.WriteLine($"Cannot load setting '{attribute.SettingName}' - provider not available");
                continue;
            }
            
            var stringValue = provider.ReadSetting(attribute.SettingName);
            
            if (stringValue != null)
            {
                try
                {
                    var convertedValue = ConvertFromString(stringValue, property.PropertyType);
                    property.SetValue(this, convertedValue);
                    Console.WriteLine($"Loaded setting '{attribute.SettingName}' = '{stringValue}' into property '{property.Name}'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error converting setting '{attribute.SettingName}' to type {property.PropertyType.Name}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Setting '{attribute.SettingName}' not found in {attribute.ProviderType} provider");
            }
        }
    }

    /// <summary>
    /// Saves settings from properties marked with ConfigurationItemAttribute to the configured providers
    /// </summary>
    public void SaveSettings()
    {
        var properties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetCustomAttribute<ConfigurationItemAttribute>() != null);

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<ConfigurationItemAttribute>()!;
            var provider = GetProvider(attribute.ProviderType);
            
            if (provider == null)
            {
                Console.WriteLine($"Cannot save setting '{attribute.SettingName}' - provider not available");
                continue;
            }
            
            var value = property.GetValue(this);
            
            if (value != null)
            {
                try
                {
                    var stringValue = ConvertToString(value);
                    provider.WriteSetting(attribute.SettingName, stringValue);
                    Console.WriteLine($"Saved setting '{attribute.SettingName}' = '{stringValue}' from property '{property.Name}'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving setting '{attribute.SettingName}': {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Converts a string value to the specified type
    /// </summary>
    private object? ConvertFromString(string value, Type targetType)
    {
        if (targetType == typeof(string))
            return value;

        if (targetType == typeof(int))
            return int.Parse(value, CultureInfo.InvariantCulture);

        if (targetType == typeof(float))
            return float.Parse(value, CultureInfo.InvariantCulture);

        if (targetType == typeof(TimeSpan))
            return TimeSpan.Parse(value, CultureInfo.InvariantCulture);

        throw new NotSupportedException($"Type {targetType.Name} is not supported for configuration binding");
    }

    /// <summary>
    /// Converts an object value to its string representation
    /// </summary>
    private string ConvertToString(object value)
    {
        if (value is IFormattable formattable)
            return formattable.ToString(null, CultureInfo.InvariantCulture);

        return value.ToString() ?? string.Empty;
    }
}
