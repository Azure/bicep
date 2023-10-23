// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.Syntax
{
    public class ImportSpecification : ISymbolNameSource
    {
        // The setting below adds syntax highlighting for regex.
        // language=regex
        private const string NamePattern = "[a-zA-Z][a-zA-Z0-9]+";

        private const string FromRegistryPattern = @"br[:\/]\S+";

        // Regex copied from https://semver.org/.
        // language=regex
        private const string SemanticVersionPattern = @"(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?";

        private static readonly Regex BuiltInSpecificationPattern = new(
            @$"^(?<name>{NamePattern})@(?<version>{SemanticVersionPattern})$",
            RegexOptions.ECMAScript | RegexOptions.Compiled);

        private static readonly Regex FromRegistrySpecificationPattern = new(
            @$"^(?<address>{FromRegistryPattern})@(?<version>{SemanticVersionPattern})$",
            RegexOptions.ECMAScript | RegexOptions.Compiled);

        private static readonly Regex RepositoryNamePattern = new(
            @"^\S*[:\/](?<name>az)$",
            RegexOptions.ECMAScript | RegexOptions.Compiled);

        private ImportSpecification(string unexpandedPath, string name, string version, bool isValid, TextSpan span)
        {
            UnexpandedPath = unexpandedPath;
            Name = name;
            Version = version;
            IsValid = isValid;
            Span = span;
        }

        private string UnexpandedPath { get; }

        public string Name { get; }

        public string Version { get; }

        public bool IsValid { get; }

        public TextSpan Span { get; }

        public static ImportSpecification From(SyntaxBase specificationSyntax)
        {
            if (specificationSyntax is StringSyntax stringSyntax && stringSyntax.TryGetLiteralValue() is { } value)
            {
                return CreateFromStringSyntax(stringSyntax, value);
            }

            return new ImportSpecification(
                LanguageConstants.ErrorName,
                LanguageConstants.ErrorName,
                LanguageConstants.ErrorName,
                false,
                specificationSyntax.Span);
        }

        public SyntaxBase ToPath()
        {
            if (!this.IsValid)
            {
                return new SkippedTriviaSyntax(this.Span, Enumerable.Empty<SyntaxBase>());
            }
            return SyntaxFactory.CreateStringLiteral(this.UnexpandedPath);
        }

        private static ImportSpecification CreateFromStringSyntax(StringSyntax stringSyntax, string value)
        {
            if (BuiltInSpecificationPattern.Match(value) is { } builtInMatch && builtInMatch.Success)
            {
                var name = builtInMatch.Groups["name"].Value;
                var span = new TextSpan(stringSyntax.Span.Position + 1, name.Length);
                var version = builtInMatch.Groups["version"].Value;
                // built-in providers (e.g. kubernetes@1.0.0 or sys@1.0.0) are allowed as long as the name is not 'az'
                return new(name, name, version, name != AzNamespaceType.BuiltInName, span);
            }

            if (FromRegistrySpecificationPattern.Match(value) is { } registryMatch && registryMatch.Success)
            {
                // NOTE(asilverman): The regex for the registry pattern is intentionally loose since it will be validated by the module resolver.
                var address = registryMatch.Groups["address"].Value;
                var version = registryMatch.Groups["version"].Value;

                var span = new TextSpan(stringSyntax.Span.Position + 1, address.Length);
                // NOTE(asilverman): I normalize the artifact address to the way we represent module addresses, see https://github.com/Azure/bicep/issues/12202
                var unexpandedArtifactAddress = $"{address}:{version}";
                var name = RepositoryNamePattern.Match(address).Groups["name"].Value;
                // NOTE(asilverman): Only a repo name of az is allowed for now. This shall be relaxed once we generalize dynamic type loading for other provider packages.
                return new(unexpandedArtifactAddress, name, version, name == AzNamespaceType.BuiltInName, span);
            }
            return new(
                 LanguageConstants.ErrorName,
                 LanguageConstants.ErrorName,
                 LanguageConstants.ErrorName,
                 false,
                 stringSyntax.Span);
        }
    }
}
