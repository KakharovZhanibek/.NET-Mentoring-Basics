using System.Reflection;
using Reflection.Contracts;

namespace Reflection;

/// <summary>
/// Loads configuration provider plugins from assemblies
/// </summary>
public static class PluginLoader
{
    /// <summary>
    /// Loads all configuration providers from plugin assemblies in the specified directory
    /// </summary>
    /// <param name="pluginDirectory">Directory containing plugin DLLs</param>
    /// <returns>Dictionary of provider type names to provider instances</returns>
    public static Dictionary<string, IConfigurationProvider> LoadProviders(string pluginDirectory)
    {
        var providers = new Dictionary<string, IConfigurationProvider>();

        if (!Directory.Exists(pluginDirectory))
        {
            Console.WriteLine($"Warning: Plugin directory '{pluginDirectory}' does not exist.");
            return providers;
        }

        var pluginFiles = Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.TopDirectoryOnly);

        foreach (var pluginFile in pluginFiles)
        {
            try
            {
                Console.WriteLine($"Loading plugin from: {Path.GetFileName(pluginFile)}");
                
                var assembly = Assembly.LoadFrom(pluginFile);
                var providerTypes = assembly.GetTypes()
                    .Where(t => typeof(IConfigurationProvider).IsAssignableFrom(t) 
                        && !t.IsInterface 
                        && !t.IsAbstract);

                foreach (var type in providerTypes)
                {
                    try
                    {
                        // Create instance using parameterless constructor
                        var instance = Activator.CreateInstance(type) as IConfigurationProvider;
                        
                        if (instance != null)
                        {
                            providers[instance.ProviderTypeName] = instance;
                            Console.WriteLine($"  ? Loaded provider: {type.Name} (Type: {instance.ProviderTypeName})");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ? Failed to instantiate provider {type.Name}: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"    Inner exception: {ex.InnerException.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ? Failed to load assembly {Path.GetFileName(pluginFile)}: {ex.Message}");
            }
        }

        return providers;
    }
}
