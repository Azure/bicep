// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.Completions.SyntaxPatterns
{
    public class CompositeSyntaxPattern
    {
        private readonly IEnumerable<SyntaxPattern> patterns;

        private CompositeSyntaxPattern(IEnumerable<SyntaxPattern> patterns)
        {
            this.patterns = patterns;
        }

        public static CompositeSyntaxPattern Create(char cursor, params string[] textWithCursorData) =>
            new(AugmentData(cursor, textWithCursorData).Select(textWithCursor => SyntaxPattern.Create(cursor, textWithCursor)));

        public static CompositeSyntaxPattern Create(Func<Parser, SyntaxBase> syntaxFactory, char cursor, params string[] textWithCursorData) =>
            new(AugmentData(cursor, textWithCursorData).Select(textWithCursor => SyntaxPattern.Create(syntaxFactory, cursor, textWithCursor)));

        public bool TailMatch(SyntaxPattern other)
        {
            foreach (var pattern in patterns)
            {
                if (pattern.TailMatch(other))
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<string> AugmentData(char cursor, IEnumerable<string> textWithCursorData)
        {
            foreach (var textWithCursor in textWithCursorData)
            {
                yield return textWithCursor;

                if (textWithCursor.EndsWith(cursor))
                {
                    yield return $"{textWithCursor}\n";
                }
            }
        }

    }
}
