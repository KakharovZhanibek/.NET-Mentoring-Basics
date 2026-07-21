// Polyfill required so C# records / init-only setters compile against netstandard2.0.
// The type is shipped in .NET 5+ BCL but must be declared manually for older targets.
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
