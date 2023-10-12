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

namespace Bicep.Core.Syntax
{
    public class ImportSpecification : ISymbolNameSource
    {
        // The setting below adds syntax highlighting for regex.
        // language=regex
        private const string SchemePattern = "br";

        // The setting below adds syntax highlighting for regex.
        // language=regex
        private const string NamePattern = "[a-zA-Z][a-zA-Z0-9]+";

        // Regex copied from https://semver.org/.
        // language=regex
        private const string SemanticVersionPattern = @"(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?";

        private static readonly Regex SpecificationWithAliasPattern = new(
            @$"^(?<scheme>{SchemePattern})/(?<registryAlias>{NamePattern}):(?<name>{NamePattern})@(?<version>{SemanticVersionPattern})$",
            RegexOptions.ECMAScript | RegexOptions.Compiled);
        private static readonly Regex SpecificationWithoutAliasPattern = new(
            @$"^(?<scheme>{SchemePattern}):(?<address>[a-zA-Z][\w\.-]+(:\d+)?(\/[a-zA-Z0-9]*)+)?@(?<version>{SemanticVersionPattern})$",
            RegexOptions.ECMAScript | RegexOptions.Compiled);
        private static readonly Regex BareSpecification = new(
            @$"^(?<name>{NamePattern})@(?<version>{SemanticVersionPattern})$");


        private ImportSpecification(string scheme, string unqualifiedAddress, string registryAlias, string name, string version, bool isValid, TextSpan span)
        {
            Scheme = scheme;
            UnqualifiedAddress = unqualifiedAddress;
            RegistryAlias = registryAlias;
            Name = name;
            Version = version;
            IsValid = isValid;
            Span = span;
        }

        public string Scheme { get; }
        private string UnqualifiedAddress { get; }
        public string RegistryAlias { get; }

        public string Name { get; }

        public string Version { get; }

        public bool IsValid { get; }

        public TextSpan Span { get; }

        public static ImportSpecification From(SyntaxBase specificationSyntax)
        {
            return specificationSyntax switch
            {
                StringSyntax stringSyntax when stringSyntax.TryGetLiteralValue() is { } value
                    => CreateFromStringSyntax(stringSyntax, value),
                SkippedTriviaSyntax trivia
                    => new ImportSpecification(
                        trivia.TriviaName,
                        trivia.TriviaName,
                        trivia.TriviaName,
                        trivia.TriviaName,
                        trivia.TriviaName,
                        false,
                        trivia.Span),
                _
                    => new ImportSpecification(
                        LanguageConstants.ErrorName,
                        LanguageConstants.ErrorName,
                        LanguageConstants.ErrorName,
                        LanguageConstants.ErrorName,
                        LanguageConstants.ErrorName,
                        false,
                        specificationSyntax.Span)
            };
        }

        public StringSyntax ToPath()
        {
            if (!this.IsValid)
            {
                throw new InvalidOperationException("Cannot convert invalid import specification to path.");
            }
            return SyntaxFactory.CreateStringLiteral(this.UnqualifiedAddress);
        }

        private static ImportSpecification CreateFromStringSyntax(StringSyntax stringSyntax, string value)
        {
            var matchBareSpecification = BareSpecification.Match(value);
            if (matchBareSpecification.Success)
            {
                var name = matchBareSpecification.Groups["name"].Value;
                var version = matchBareSpecification.Groups["version"].Value;
                if (name == "az")
                {
                    return new(
                        LanguageConstants.ErrorName,
                        LanguageConstants.ErrorName,
                        LanguageConstants.ErrorName,
                        LanguageConstants.ErrorName,
                        LanguageConstants.ErrorName,
                        false,
                        stringSyntax.Span);
                }
                var span = new TextSpan(stringSyntax.Span.Position + 1, name.Length);
                return new(string.Empty, string.Empty, string.Empty, name, version, true, span);
            }
            var matchSpecificationWithAlias = SpecificationWithAliasPattern.Match(value);
            if (matchSpecificationWithAlias.Success)
            {
                var scheme = matchSpecificationWithAlias.Groups["scheme"].Value;
                var alias = matchSpecificationWithAlias.Groups["registryAlias"].Value;
                var name = matchSpecificationWithAlias.Groups["name"].Value;
                var version = matchSpecificationWithAlias.Groups["version"].Value;

                var address = value.Replace('@', ':');

                var offset = value.Split('@')[0].Length;
                var span = new TextSpan(stringSyntax.Span.Position + 1, offset);

                return new(scheme, address, alias, name, version, true, span);

            }

            var matchSpecificationWithoutAlias = SpecificationWithoutAliasPattern.Match(value);
            if (matchSpecificationWithoutAlias.Success)
            {
                var scheme = matchSpecificationWithoutAlias.Groups["scheme"].Value;
                var version = matchSpecificationWithoutAlias.Groups["version"].Value;

                var address = value.Replace('@', ':');

                var offset = address.Length;
                var span = new TextSpan(stringSyntax.Span.Position + 1, offset);

                var name = address.Split('/')[^1].Split(':')[0];

                return new(scheme, address, string.Empty, name, version, true, span);
            }

            return new(
                LanguageConstants.ErrorName,
                LanguageConstants.ErrorName,
                LanguageConstants.ErrorName,
                LanguageConstants.ErrorName,
                LanguageConstants.ErrorName,
                false,
                stringSyntax.Span);
        }
    }
}
