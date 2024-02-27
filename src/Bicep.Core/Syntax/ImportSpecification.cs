// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.Syntax
{
    public partial class ImportSpecification : ISymbolNameSource
    {
        // The setting below adds syntax highlighting for regex.
        // language=regex
        private const string NamePattern = "[a-zA-Z][a-zA-Z0-9]+";

        private const string BicepRegistryAddressPattern = @"br[:\/]\S+";

        // Regex copied from https://semver.org/.
        // language=regex
        private const string SemanticVersionPattern = @"(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?";

        [GeneratedRegex(@$"^(?<name>{NamePattern})@(?<version>{SemanticVersionPattern})$", RegexOptions.ECMAScript | RegexOptions.Compiled)]
        private static partial Regex BuiltInSpecificationPattern();

        [GeneratedRegex(@$"^(?<address>{BicepRegistryAddressPattern})@(?<version>{SemanticVersionPattern})$", RegexOptions.ECMAScript | RegexOptions.Compiled)]
        private static partial Regex FromRegistrySpecificationPattern();


        [GeneratedRegex(@"^\S*[:\/](?<name>\S+)$", RegexOptions.ECMAScript | RegexOptions.Compiled)]
        private static partial Regex RepositoryNamePattern();

        private ImportSpecification(string name, string version, string? bicepRegistryAddress, TextSpan span, bool isValid)
        {
            Name = name;
            Version = version;
            BicepRegistryAddress = bicepRegistryAddress;
            IsValid = isValid;
            Span = span;
        }

        public string Name { get; }

        public string Version { get; }

        public string? BicepRegistryAddress { get; }

        public bool IsValid { get; }

        public TextSpan Span { get; }

        public static ImportSpecification From(SyntaxBase specificationSyntax)
        {
            if (specificationSyntax is StringSyntax stringSyntax &&
                stringSyntax.TryGetLiteralValue() is { } value &&
                TryCreateFromStringSyntax(stringSyntax, value) is { } specification)
            {
                return specification;
            }

            return new(
                LanguageConstants.ErrorName,
                LanguageConstants.ErrorName,
                null,
                specificationSyntax.Span,
                isValid: false);
        }

        private static ImportSpecification? TryCreateFromStringSyntax(StringSyntax stringSyntax, string value)
        {
            if (BuiltInSpecificationPattern().Match(value) is { } builtInMatch && builtInMatch.Success)
            {
                var name = builtInMatch.Groups["name"].Value;
                var span = stringSyntax.GetInnerSpan();
                var version = builtInMatch.Groups["version"].Value;
                // built-in providers (e.g. kubernetes@1.0.0 or sys@1.0.0) are allowed as long as the name is not 'az'
                return new(name, version, null, span, isValid: name != AzNamespaceType.BuiltInName);
            }

            if (FromRegistrySpecificationPattern().Match(value) is { } registryMatch && registryMatch.Success)
            {
                // NOTE(asilverman): The regex for the registry pattern is intentionally loose since it will be validated by the module resolver.
                var address = registryMatch.Groups["address"].Value;
                var version = registryMatch.Groups["version"].Value;

                var span = stringSyntax.GetInnerSpan();
                // NOTE(asilverman): I normalize the artifact address to the way we represent module addresses, see https://github.com/Azure/bicep/issues/12202
                var unexpandedArtifactAddress = $"{address}:{version}";
                var name = RepositoryNamePattern().Match(address).Groups["name"].Value;

                return new(name, version, unexpandedArtifactAddress, span, isValid: true);
            }

            return null;
        }
    }
}
