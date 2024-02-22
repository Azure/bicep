// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Providers;

namespace Bicep.Core.TypeSystem.Providers;

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

    public static IProviderSpecification CreateProviderSpecification(SyntaxBase syntax)
     => syntax switch
     {
         StringSyntax stringSyntax when stringSyntax.TryGetLiteralValue() is { } value &&
                                       TryCreateFromStringSyntax(stringSyntax, value) is { } specification
             => specification,

         IdentifierSyntax identifierSyntax
             => new ConfigurationManagedProviderSpecification(
                 identifierSyntax.IdentifierName,
                 IsValid: true,
                 identifierSyntax.Span),

         _ => new ProviderSpecificationTrivia(syntax.Span)
     };

    private static IProviderSpecification? TryCreateFromStringSyntax(StringSyntax stringSyntax, string value)
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

            return new ConfigurationManagedProviderSpecification(name, IsValid: true, span);
        }
        else if (InlinedSpecificationPattern().Match(value) is { Success: true } registryMatch)
        {
            // The regex for the registry pattern is intentionally loose since it will be validated by the module resolver.
            var address = registryMatch.Groups["address"].Value;
            var version = registryMatch.Groups["version"].Value;

            var span = new TextSpan(stringSyntax.Span.Position + 1, address.Length);
            // We normalize the artifact address to the way we represent module addresses, see https://github.com/Azure/bicep/issues/12202
            var unexpandedArtifactAddress = $"{address}:{version}";
            var name = RepositoryNamePattern().Match(address).Groups["name"].Value;

            return new InlinedProviderSpecification(name, version, unexpandedArtifactAddress, IsValid: true, span);
        }
        return null;
    }
}

