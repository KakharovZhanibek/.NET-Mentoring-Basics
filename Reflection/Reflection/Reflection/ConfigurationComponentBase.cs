using System.Reflection;
using Reflection.Contracts;

namespace Reflection;

/// <summary>
/// Base class for components that support configuration binding via attributes.
/// LoadSettings / SaveSettings are generated at compile time by
/// Reflection.SourceGenerator for every partial subclass.
/// </summary>
public abstract class ConfigurationComponentBase
{
    private static readonly Dictionary<string, Contracts.IConfigurationProvider> _loadedProviders = new();
    private static bool _providersInitialized = false;

    protected ConfigurationComponentBase()
    {
        if (!_providersInitialized)
        {
            InitializeProviders();
            _providersInitialized = true;
        }
    }

    private static void InitializeProviders()
    {
        Console.WriteLine("\n=== Initializing Configuration Providers ===");

        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var pluginDirectory = Path.Combine(baseDirectory, "plugins");

        Console.WriteLine($"Plugin directory: {pluginDirectory}");

        var providers = PluginLoader.LoadProviders(pluginDirectory);
        foreach (var provider in providers)
            _loadedProviders[provider.Key] = provider.Value;

        Console.WriteLine($"Total providers loaded: {_loadedProviders.Count}");
        Console.WriteLine("=============================================\n");
    }

    /// <summary>
    /// Returns the provider registered under the given type name, or null.
    /// Called by the source-generated LoadSettings / SaveSettings methods.
    /// </summary>
    protected internal Contracts.IConfigurationProvider? GetProvider(string providerTypeName)
    {
        if (_loadedProviders.TryGetValue(providerTypeName, out var provider))
            return provider;

        Console.WriteLine(
            $"Warning: Provider '{providerTypeName}' not found. " +
            $"Available: {string.Join(", ", _loadedProviders.Keys)}");
        return null;
    }
}
