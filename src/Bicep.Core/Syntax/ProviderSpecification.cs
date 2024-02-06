// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public interface IResourceTypesProviderSpecification : ISymbolNameSource
{
    string NamespaceIdentifier { get; }
    string? Version { get; }
}

public record InlinedResourceTypesProviderSpecification(
    string NamespaceIdentifier,
    string Version,
    string UnexpandedArtifactAddress,
    bool IsValid,
    TextSpan Span) : IResourceTypesProviderSpecification;

public record ConfigurationManagedResourceTypesProviderSpecification(string NamespaceIdentifier, bool IsValid, TextSpan Span) : IResourceTypesProviderSpecification
{
    public string? Version => null;
};

public record LegacyProviderSpecification(string NamespaceIdentifier, string Version, bool IsValid, TextSpan Span) : IResourceTypesProviderSpecification;

public record ResourceTypesProviderSpecificationTrivia(TextSpan Span) : IResourceTypesProviderSpecification
{
    public string NamespaceIdentifier => LanguageConstants.ErrorName;
    public bool IsValid => false;
    public string? Version => null;
};

public static partial class ProviderSpecificationFactory
{
    // The setting below adds syntax highlighting for regex.
    // language=regex
    private const string NamePattern = "[a-zA-Z][a-zA-Z0-9]+";

    private const string BicepRegistryAddressPattern = @"br[:\/]\S+";

    // Regex copied from https://semver.org/.
    // language=regex
    private const string SemanticVersionPattern = @"(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?";

    [GeneratedRegex(@$"^(?<name>{NamePattern})$", RegexOptions.ECMAScript | RegexOptions.Compiled)]
    private static partial Regex ConfigManagedSpecificationPattern();

    [GeneratedRegex(@$"^(?<name>{NamePattern})@(?<version>{SemanticVersionPattern})$", RegexOptions.ECMAScript | RegexOptions.Compiled)]
    private static partial Regex LegacyBuiltInSpecificationPattern();

    [GeneratedRegex(@$"^(?<address>{BicepRegistryAddressPattern})@(?<version>{SemanticVersionPattern})$", RegexOptions.ECMAScript | RegexOptions.Compiled)]
    private static partial Regex InlinedSpecificationPattern();

    [GeneratedRegex(@"^\S*[:\/](?<name>\S+)$", RegexOptions.ECMAScript | RegexOptions.Compiled)]
    private static partial Regex RepositoryNamePattern();

    public static IResourceTypesProviderSpecification FromSyntax(SyntaxBase syntax)
     => syntax switch
     {
         StringSyntax stringSyntax when stringSyntax.TryGetLiteralValue() is { } value &&
                                       TryCreateFromStringSyntax(stringSyntax, value) is { } specification
             => specification,

         IdentifierSyntax identifierSyntax
             => new ConfigurationManagedResourceTypesProviderSpecification(
                 identifierSyntax.IdentifierName,
                 IsValid: true,
                 identifierSyntax.Span),

         _ => new ResourceTypesProviderSpecificationTrivia(syntax.Span)
     };

    private static IResourceTypesProviderSpecification? TryCreateFromStringSyntax(StringSyntax stringSyntax, string value)
    {
        if (LegacyBuiltInSpecificationPattern().Match(value) is { Success: true } legacyMatch)
        {
            var name = legacyMatch.Groups["name"].Value;
            var version = legacyMatch.Groups["version"].Value;
            var span = new TextSpan(stringSyntax.Span.Position + 1, name.Length);

            return new LegacyProviderSpecification(name, version, IsValid: true, span);
        }
        else if (ConfigManagedSpecificationPattern().Match(value) is { Success: true } builtInMatch)
        {
            var name = builtInMatch.Groups["name"].Value;
            var span = new TextSpan(stringSyntax.Span.Position + 1, name.Length);

            return new ConfigurationManagedResourceTypesProviderSpecification(name, IsValid: true, span);
        }
        else if (InlinedSpecificationPattern().Match(value) is { Success: true } registryMatch)
        {
            // NOTE(asilverman): The regex for the registry pattern is intentionally loose since it will be validated by the module resolver.
            var address = registryMatch.Groups["address"].Value;
            var version = registryMatch.Groups["version"].Value;

            var span = new TextSpan(stringSyntax.Span.Position + 1, address.Length);
            // NOTE(asilverman): I normalize the artifact address to the way we represent module addresses, see https://github.com/Azure/bicep/issues/12202
            var unexpandedArtifactAddress = $"{address}:{version}";
            var name = RepositoryNamePattern().Match(address).Groups["name"].Value;

            return new InlinedResourceTypesProviderSpecification(name, version, unexpandedArtifactAddress, IsValid: true, span);
        }
        return null;
    }
}

