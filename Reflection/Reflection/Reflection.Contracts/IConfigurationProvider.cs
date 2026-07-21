namespace Reflection.Contracts;

/// <summary>
/// Base interface for configuration providers
/// </summary>
public interface IConfigurationProvider
{
    /// <summary>
    /// Gets the provider type name that this provider handles
    /// </summary>
    string ProviderTypeName { get; }

    /// <summary>
    /// Reads a setting value from the configuration source
    /// </summary>
    string? ReadSetting(string settingName);

    /// <summary>
    /// Writes a setting value to the configuration source
    /// </summary>
    void WriteSetting(string settingName, string value);
}
