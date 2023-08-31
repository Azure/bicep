// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using System.Text.RegularExpressions;

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

        private static readonly Regex SpecificationPattern = new(
            @$"^(?<name>{NamePattern})@(?<version>{SemanticVersionPattern})$",
            RegexOptions.ECMAScript | RegexOptions.Compiled);

        private ImportSpecification(string name, string version, bool isValid, TextSpan span)
        {
            Name = name;
            Version = version;
            IsValid = isValid;
            Span = span;
        }

        public string Name { get; }

        public string Version { get; }

        public bool IsValid { get; }

        public TextSpan Span { get; }

        public static ImportSpecification From(SyntaxBase specificationSyntax)
        {
            switch (specificationSyntax)
            {
                case StringSyntax stringSyntax when stringSyntax.TryGetLiteralValue() is { } value:
                    var (name, version, isValid) = Parse(value);
                    var span = isValid ? new TextSpan(stringSyntax.Span.Position + 1, name.Length) : stringSyntax.Span;

                    return new ImportSpecification(name, version, isValid, span);

                case SkippedTriviaSyntax trivia:
                    return new ImportSpecification(trivia.TriviaName, trivia.TriviaName, false, trivia.Span);

                default:
                    return new ImportSpecification(LanguageConstants.ErrorName, LanguageConstants.ErrorName, false, specificationSyntax.Span);
            }
        }

        private static (string Name, string Version, bool IsValid) Parse(string value)
        {
            var match = SpecificationPattern.Match(value);

            if (!match.Success)
            {
                return (LanguageConstants.ErrorName, LanguageConstants.ErrorName, false);
            }

            var name = match.Groups["name"].Value;
            var version = match.Groups["version"].Value;

            return (name, version, true);
        }
    }
}
