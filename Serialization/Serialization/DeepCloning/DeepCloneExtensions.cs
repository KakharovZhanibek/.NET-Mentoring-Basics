using System.Text.Json;

namespace DeepCloning;

/// <summary>
/// Provides deep cloning of any serializable object graph via JSON
/// round-tripping with <see cref="System.Text.Json.JsonSerializer"/>.
///
/// Each call produces a completely independent object — no shared references
/// exist between the original and the clone.
/// </summary>
public static class DeepCloneExtensions
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = false,
        // Preserve the full type information for collections / polymorphic members.
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Returns a deep clone of <paramref name="source"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to clone.</typeparam>
    /// <param name="source">The object to clone. Must not be <see langword="null"/>.</param>
    /// <returns>A new, fully independent instance of <typeparamref name="T"/>.</returns>
    public static T DeepClone<T>(this T source) where T : notnull
    {
        // Serialize to JSON then immediately deserialize into a brand-new instance.
        string json = JsonSerializer.Serialize(source, _options);
        return JsonSerializer.Deserialize<T>(json, _options)!;
    }
}
