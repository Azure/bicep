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
        private const string NamePattern = "[a-zA-Z][a-zA-Z0-9]+";

        // Regex copied from https://semver.org/.
        // language=regex
        private const string SemanticVersionPattern = @"(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?";

        private static readonly Regex BareSpecification = new(
            @$"(?<name>{NamePattern})@(?<version>{SemanticVersionPattern})$");


        private ImportSpecification(string unqualifiedAddress, string name, string version, bool isValid, TextSpan span)
        {
            UnqualifiedAddress = unqualifiedAddress;
            Name = name;
            Version = version;
            IsValid = isValid;
            Span = span;
        }

        private string UnqualifiedAddress { get; }

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
                        false,
                        trivia.Span),
                _
                    => new ImportSpecification(
                        LanguageConstants.ErrorName,
                        LanguageConstants.ErrorName,
                        LanguageConstants.ErrorName,
                        false,
                        specificationSyntax.Span)
            };
        }

        public SyntaxBase ToPath()
        {
            if (!this.IsValid)
            {
                return new SkippedTriviaSyntax(this.Span, Enumerable.Empty<SyntaxBase>());
            }
            return SyntaxFactory.CreateStringLiteral(this.UnqualifiedAddress);
        }

        private static ImportSpecification CreateFromStringSyntax(StringSyntax stringSyntax, string value)
        {
            var match = BareSpecification.Match(value);
            if (!match.Success)
            {
                return new(
                 LanguageConstants.ErrorName,
                 LanguageConstants.ErrorName,
                 LanguageConstants.ErrorName,
                 false,
                 stringSyntax.Span);
            }

            var name = match.Groups["name"].Value;
            var version = match.Groups["version"].Value;
            var parts = value.Split('@');
            var artifactAddress = parts[0];
            
            var span = new TextSpan(stringSyntax.Span.Position + 1, artifactAddress.Length);

            return new(string.Join(':', parts), name, version, artifactAddress != "az", span);
        }
    }
}
