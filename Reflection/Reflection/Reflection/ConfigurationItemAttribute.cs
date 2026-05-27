namespace Reflection;

/// <summary>
/// Specifies the configuration provider type
/// </summary>
public enum ConfigurationProviderType
{
    File,
    ConfigurationManager
}

/// <summary>
/// Attribute to mark properties that should be bound to configuration values
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ConfigurationItemAttribute : Attribute
{
    /// <summary>
    /// Gets the setting name in the configuration source
    /// </summary>
    public string SettingName { get; }

    /// <summary>
    /// Gets the provider type to use for reading/writing the configuration
    /// </summary>
    public ConfigurationProviderType ProviderType { get; }

    /// <summary>
    /// Initializes a new instance of the ConfigurationItemAttribute
    /// </summary>
    /// <param name="settingName">The name of the setting in the configuration source</param>
    /// <param name="providerType">The type of configuration provider to use</param>
    public ConfigurationItemAttribute(string settingName, ConfigurationProviderType providerType)
    {
        if (string.IsNullOrWhiteSpace(settingName))
            throw new ArgumentException("Setting name cannot be null or empty", nameof(settingName));

        SettingName = settingName;
        ProviderType = providerType;
    }
}
