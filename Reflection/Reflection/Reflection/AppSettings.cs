namespace Reflection;

/// <summary>
/// Sample component demonstrating the use of ConfigurationItemAttribute
/// </summary>
public class AppSettings : ConfigurationComponentBase
{
    [ConfigurationItem("MaxRetryCount", ConfigurationProviderType.File)]
    public int MaxRetryCount { get; set; }

    [ConfigurationItem("TimeoutDuration", ConfigurationProviderType.File)]
    public TimeSpan TimeoutDuration { get; set; }

    [ConfigurationItem("ServerUrl", ConfigurationProviderType.ConfigurationManager)]
    public string ServerUrl { get; set; } = string.Empty;

    [ConfigurationItem("ApiVersion", ConfigurationProviderType.ConfigurationManager)]
    public float ApiVersion { get; set; }

    /// <summary>
    /// Displays all settings
    /// </summary>
    public void DisplaySettings()
    {
        Console.WriteLine("\n=== Current Settings ===");
        Console.WriteLine($"MaxRetryCount: {MaxRetryCount}");
        Console.WriteLine($"TimeoutDuration: {TimeoutDuration}");
        Console.WriteLine($"ServerUrl: {ServerUrl}");
        Console.WriteLine($"ApiVersion: {ApiVersion}");
        Console.WriteLine("========================\n");
    }
}
