// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Bicep.Core.Syntax;

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

    [GeneratedRegex(@$"^(?<name>{NamePattern})@(?<version>{SemanticVersionPattern})$", RegexOptions.ECMAScript | RegexOptions.Compiled)]
    private static partial Regex LegacyBuiltInSpecificationPattern();

    [GeneratedRegex(@$"^(?<address>{BicepRegistryAddressPattern})@(?<version>{SemanticVersionPattern})$", RegexOptions.ECMAScript | RegexOptions.Compiled)]
    private static partial Regex InlinedSpecificationPattern();

    [GeneratedRegex(@"^\S*[:\/](?<name>\S+)$", RegexOptions.ECMAScript | RegexOptions.Compiled)]
    private static partial Regex RepositoryNamePattern();

    public static IProviderSpecification CreateProviderSpecification(SyntaxBase syntax)
     => syntax switch
     {
         IdentifierSyntax identifierSyntax => new ConfigurationManagedProviderSpecification(identifierSyntax.IdentifierName, IsValid: true, identifierSyntax.Span),
         StringSyntax stringSyntax when stringSyntax.TryGetLiteralValue() is { } value && TryCreateFromStringSyntax(stringSyntax, value) is { } specification => specification,
         _ => new ProviderSpecificationTrivia(syntax.Span)
     };

    private static IProviderSpecification? TryCreateFromStringSyntax(StringSyntax stringSyntax, string value)
    {
        var legacyMatch = LegacyBuiltInSpecificationPattern().Match(value);
        if (legacyMatch.Success)
        {
            return new LegacyProviderSpecification(
                legacyMatch.Groups["name"].Value,
                legacyMatch.Groups["version"].Value,
                IsValid: true,
                stringSyntax.GetInnerSpan());
        }

        var registryMatch = InlinedSpecificationPattern().Match(value);
        if (!registryMatch.Success)
        {
            return null;
        }

        // The regex for the registry pattern is intentionally loose since it will be validated by the module resolver.
        var address = registryMatch.Groups["address"].Value;
        var version = registryMatch.Groups["version"].Value;

        var span = stringSyntax.GetInnerSpan();
        // We normalize the artifact address to the way we represent module addresses, see https://github.com/Azure/bicep/issues/12202
        var unexpandedArtifactAddress = $"{address}:{version}";
        var name = RepositoryNamePattern().Match(address).Groups["name"].Value;

        return new InlinedProviderSpecification(name, version, unexpandedArtifactAddress, IsValid: true, span);
    }
}

