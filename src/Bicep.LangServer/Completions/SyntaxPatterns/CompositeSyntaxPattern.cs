// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Completions.SyntaxPatterns
{
    public class CompositeSyntaxPattern
    {
        private readonly IEnumerable<SyntaxPattern> patterns;

        private CompositeSyntaxPattern(IEnumerable<SyntaxPattern> patterns)
        {
            this.patterns = patterns;
        }

        public static CompositeSyntaxPattern Create(params string[] textWithCursorData) =>
            new(AugmentData(textWithCursorData).Select(textWithCursor => SyntaxPattern.Create(textWithCursor)));

        public static CompositeSyntaxPattern Create(Func<Parser, SyntaxBase> syntaxFactory, params string[] textWithCursorData) =>
            new(AugmentData(textWithCursorData).Select(textWithCursor => SyntaxPattern.Create(syntaxFactory, textWithCursor)));

        public bool TailMatch(ImmutableArray<SyntaxBase> nodes)
        {
            foreach (var pattern in patterns)
            {
                if (pattern.TailMatch(nodes))
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<string> AugmentData(IEnumerable<string> textWithCursorData)
        {
            foreach (var textWithCursor in textWithCursorData)
            {
                yield return textWithCursor;

                if (textWithCursor.EndsWith('|'))
                {
                    yield return $"{textWithCursor}\n";
                }
            }
        }

    }
}
