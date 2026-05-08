namespace FileSystemVisitorConsoleApp.Services;

/// <summary>
/// Service for collecting and validating user input for filter parameters.
/// </summary>
public static class ParameterInputService
{
    /// <summary>
    /// Prompts user for input and converts it to the specified type.
    /// </summary>
    public static object? GetParameterValue(Type parameterType, string prompt, bool isRequired)
    {
        while (true)
        {
            Console.Write($"{prompt} ");
            string? input = Console.ReadLine();

            // Handle empty input
            if (string.IsNullOrWhiteSpace(input))
            {
                if (!isRequired)
                    return GetDefaultValue(parameterType);
                    
                Console.WriteLine("This parameter is required. Please provide a value.");
                continue;
            }

            // Try to parse/convert the input
            try
            {
                return ConvertToType(input, parameterType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid input: {ex.Message}. Please try again.");
            }
        }
    }

    private static object? ConvertToType(string input, Type targetType)
    {
        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        return underlyingType switch
        {
            // String array (for extensions)
            Type t when t == typeof(string[]) => input
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray(),

            // Long
            Type t when t == typeof(long) => long.Parse(input),

            // DateTime
            Type t when t == typeof(DateTime) => DateTime.Parse(input),

            // Int
            Type t when t == typeof(int) => int.Parse(input),

            // Double
            Type t when t == typeof(double) => double.Parse(input),

            // Bool
            Type t when t == typeof(bool) => bool.Parse(input),

            // String
            Type t when t == typeof(string) => input,

            _ => throw new NotSupportedException($"Type {targetType.Name} is not supported for parameter conversion.")
        };
    }

    private static object? GetDefaultValue(Type type)
    {
        // For nullable types, return null
        if (Nullable.GetUnderlyingType(type) != null || !type.IsValueType)
            return null;

        // For value types, return their default value
        return Activator.CreateInstance(type);
    }
}
