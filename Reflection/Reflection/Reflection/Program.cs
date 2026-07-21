using Reflection;

Console.WriteLine("=== Configuration Attribute Demo ===\n");

// Create an instance of AppSettings
var settings = new AppSettings();

// Demo 1: Set initial values and save them
Console.WriteLine("Demo 1: Saving initial settings...");
settings.MaxRetryCount = 5;
settings.TimeoutDuration = TimeSpan.FromSeconds(30);
settings.ServerUrl = "https://api.example.com";
settings.ApiVersion = 2.5f;

settings.DisplaySettings();
settings.SaveSettings();

Console.WriteLine("\n" + new string('-', 50) + "\n");

// Demo 2: Create a new instance and load settings
Console.WriteLine("Demo 2: Loading settings into a new instance...");
var loadedSettings = new AppSettings();
Console.WriteLine("Before loading:");
loadedSettings.DisplaySettings();

loadedSettings.LoadSettings();

Console.WriteLine("After loading:");
loadedSettings.DisplaySettings();

Console.WriteLine(new string('-', 50) + "\n");

// Demo 3: Modify and save again
Console.WriteLine("Demo 3: Modifying and saving settings...");
loadedSettings.MaxRetryCount = 10;
loadedSettings.TimeoutDuration = TimeSpan.FromMinutes(1);
loadedSettings.ServerUrl = "https://api.updated.com";
loadedSettings.ApiVersion = 3.0f;

loadedSettings.DisplaySettings();
loadedSettings.SaveSettings();

Console.WriteLine("\n" + new string('-', 50) + "\n");

// Demo 4: Verify persistence by loading one more time
Console.WriteLine("Demo 4: Verifying persistence...");
var finalSettings = new AppSettings();
finalSettings.LoadSettings();
finalSettings.DisplaySettings();

Console.WriteLine("=== Demo Complete ===");
Console.WriteLine("\nNote: Configuration files are stored in the project root directory:");
Console.WriteLine("  - settings.json (File provider)");
Console.WriteLine("  - appsettings.json (ConfigurationManager provider)");
