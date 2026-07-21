using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflection.SourceGenerator;

/// <summary>
/// Describes a single property decorated with [ConfigurationItem].
/// Carried through the pipeline as a plain-data record so it is safely
/// usable inside RegisterSourceOutput (which runs on a background thread).
/// </summary>
internal sealed record PropertyModel(
    string PropertyName,
    string PropertyType,
    string SettingName,
    string ProviderType          // "File" or "ConfigurationManager"
);

/// <summary>
/// Describes a class that should have LoadSettings / SaveSettings generated.
/// </summary>
internal sealed record ClassModel(
    string Namespace,
    string ClassName,
    IReadOnlyList<PropertyModel> Properties
);

/// <summary>
/// Incremental source generator that inspects every class annotated with
/// [ConfigurationItem] on its properties and emits type-safe, reflection-free
/// LoadSettings / SaveSettings partial-method implementations.
/// </summary>
[Generator]
public sealed class ConfigurationComponentGenerator : IIncrementalGenerator
{
    // Fully-qualified name of the attribute we look for
    private const string AttributeFqn = "Reflection.ConfigurationItemAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // ------------------------------------------------------------------
        // 1. Collect every ClassDeclarationSyntax that has at least one
        //    property carrying [ConfigurationItem].
        // ------------------------------------------------------------------
        IncrementalValuesProvider<ClassModel?> classModels =
            context.SyntaxProvider
                   .CreateSyntaxProvider(
                       predicate: static (node, _) => IsClassWithAttributes(node),
                       transform: static (ctx, _) => TransformClass(ctx))
                   .Where(static model => model is not null);

        // ------------------------------------------------------------------
        // 2. For every collected model, emit a source file.
        // ------------------------------------------------------------------
        context.RegisterSourceOutput(classModels, static (spc, model) =>
        {
            if (model is null) return;
            spc.AddSource($"{model.ClassName}.g.cs", SourceText.From(Emit(model), Encoding.UTF8));
        });
    }

    // -----------------------------------------------------------------------
    // Predicate: fast syntactic check – we only want class declarations.
    // -----------------------------------------------------------------------
    private static bool IsClassWithAttributes(SyntaxNode node)
        => node is ClassDeclarationSyntax cls && cls.Members.Count > 0;

    // -----------------------------------------------------------------------
    // Transform: run the full semantic analysis and build a ClassModel.
    // -----------------------------------------------------------------------
    private static ClassModel? TransformClass(GeneratorSyntaxContext ctx)
    {
        var classDecl = (ClassDeclarationSyntax)ctx.Node;
        var classSymbol = ctx.SemanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
        if (classSymbol is null) return null;

        // Only process classes that inherit (directly or indirectly) from
        // ConfigurationComponentBase
        if (!InheritsFromConfigurationComponentBase(classSymbol)) return null;

        // Must be declared partial so we can add members to it
        bool isPartial = classDecl.Modifiers.Any(
            m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword));
        if (!isPartial) return null;

        // Look for the attribute type in the compilation
        var attributeSymbol = ctx.SemanticModel.Compilation
                                 .GetTypeByMetadataName(AttributeFqn);
        if (attributeSymbol is null) return null;

        var properties = new List<PropertyModel>();

        foreach (var member in classSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            foreach (var attr in member.GetAttributes())
            {
                if (!SymbolEqualityComparer.Default.Equals(
                        attr.AttributeClass, attributeSymbol))
                    continue;

                if (attr.ConstructorArguments.Length < 2) continue;

                var settingName = attr.ConstructorArguments[0].Value as string;
                // The second argument is the ConfigurationProviderType enum value
                var providerEnumValue = attr.ConstructorArguments[1].Value;

                if (settingName is null || providerEnumValue is null) continue;

                // ConfigurationProviderType.File == 0, ConfigurationManager == 1
                var providerType = (int)providerEnumValue == 0
                    ? "File"
                    : "ConfigurationManager";

                properties.Add(new PropertyModel(
                    member.Name,
                    GetFullTypeName(member.Type),
                    settingName,
                    providerType));

                break; // only one ConfigurationItem per property
            }
        }

        if (properties.Count == 0) return null;

        var ns = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : classSymbol.ContainingNamespace.ToDisplayString();

        return new ClassModel(ns, classSymbol.Name, properties);
    }

    // -----------------------------------------------------------------------
    // Walk up the base-type chain looking for ConfigurationComponentBase.
    // -----------------------------------------------------------------------
    private static bool InheritsFromConfigurationComponentBase(INamedTypeSymbol symbol)
    {
        var current = symbol.BaseType;
        while (current is not null)
        {
            if (current.Name == "ConfigurationComponentBase") return true;
            current = current.BaseType;
        }
        return false;
    }

    // -----------------------------------------------------------------------
    // Return a C#-usable type name for a property.
    // -----------------------------------------------------------------------
    private static string GetFullTypeName(ITypeSymbol type)
    {
        // Use the special-form name so we get "int", "float", "string", etc.
        return type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    // -----------------------------------------------------------------------
    // Code emission
    // -----------------------------------------------------------------------
    private static string Emit(ClassModel model)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Globalization;");
        sb.AppendLine();

        bool hasNamespace = !string.IsNullOrEmpty(model.Namespace);
        if (hasNamespace)
        {
            sb.AppendLine($"namespace {model.Namespace};");
            sb.AppendLine();
        }

        sb.AppendLine($"partial class {model.ClassName}");
        sb.AppendLine("{");

        // ---- LoadSettings -----------------------------------------------
        sb.AppendLine("    /// <summary>Generated: loads all [ConfigurationItem] properties.</summary>");
        sb.AppendLine("    public void LoadSettings()");
        sb.AppendLine("    {");

        foreach (var prop in model.Properties)
        {
            sb.AppendLine($"        // {prop.PropertyName} <- \"{prop.SettingName}\" via {prop.ProviderType} provider");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var __provider = GetProvider(\"{prop.ProviderType}\");");
            sb.AppendLine($"            if (__provider is not null)");
            sb.AppendLine($"            {{");
            sb.AppendLine($"                var __raw = __provider.ReadSetting(\"{prop.SettingName}\");");
            sb.AppendLine($"                if (__raw is not null)");
            sb.AppendLine($"                    {prop.PropertyName} = {GenerateParseExpression(prop.PropertyType, "__raw")};");
            sb.AppendLine($"            }}");
            sb.AppendLine($"        }}");
        }

        sb.AppendLine("    }");
        sb.AppendLine();

        // ---- SaveSettings -----------------------------------------------
        sb.AppendLine("    /// <summary>Generated: saves all [ConfigurationItem] properties.</summary>");
        sb.AppendLine("    public void SaveSettings()");
        sb.AppendLine("    {");

        foreach (var prop in model.Properties)
        {
            sb.AppendLine($"        // {prop.PropertyName} -> \"{prop.SettingName}\" via {prop.ProviderType} provider");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var __provider = GetProvider(\"{prop.ProviderType}\");");
            sb.AppendLine($"            if (__provider is not null)");
            sb.AppendLine($"                __provider.WriteSetting(\"{prop.SettingName}\", {GenerateToStringExpression(prop.PropertyType, prop.PropertyName)});");
            sb.AppendLine($"        }}");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    // -----------------------------------------------------------------------
    // Generates the parse expression for each supported type.
    // -----------------------------------------------------------------------
    private static string GenerateParseExpression(string fqType, string rawVar)
        => fqType switch
        {
            "int" or "global::System.Int32"
                => $"int.Parse({rawVar}, System.Globalization.CultureInfo.InvariantCulture)",
            "float" or "global::System.Single"
                => $"float.Parse({rawVar}, System.Globalization.CultureInfo.InvariantCulture)",
            "double" or "global::System.Double"
                => $"double.Parse({rawVar}, System.Globalization.CultureInfo.InvariantCulture)",
            "bool" or "global::System.Boolean"
                => $"bool.Parse({rawVar})",
            "long" or "global::System.Int64"
                => $"long.Parse({rawVar}, System.Globalization.CultureInfo.InvariantCulture)",
            "System.TimeSpan" or "global::System.TimeSpan"
                => $"System.TimeSpan.Parse({rawVar}, System.Globalization.CultureInfo.InvariantCulture)",
            "string" or "global::System.String"
                => rawVar,
            _
                => $"({fqType}){rawVar} /* unsupported type — manual conversion needed */"
        };

    // -----------------------------------------------------------------------
    // Generates the to-string expression for each supported type.
    // -----------------------------------------------------------------------
    private static string GenerateToStringExpression(string fqType, string propName)
        => fqType switch
        {
            "string" or "global::System.String"
                => propName,
            "System.TimeSpan" or "global::System.TimeSpan"
                => $"{propName}.ToString(null, System.Globalization.CultureInfo.InvariantCulture)",
            _
                // IFormattable covers int, float, double, long, etc.
                => $"((System.IFormattable){propName}).ToString(null, System.Globalization.CultureInfo.InvariantCulture)"
        };
}
