// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using FluentAssertions;

namespace Bicep.Core.UnitTests.Utils
{
    public static class ParserHelper
    {
        public static ProgramSyntax Parse(string text)
        {
            var parser = new Parser(text);

            return parser.Program();
        }

        public static SyntaxBase ParseExpression(string text, ExpressionFlags expressionFlags = ExpressionFlags.AllowComplexLiterals) => new Parser(text).Expression(expressionFlags);

        public static (string file, IReadOnlyList<int> cursors) GetFileWithCursors(string fileWithCursors)
        {
            var bicepFile = fileWithCursors.Replace("|", "");

            var cursors = new List<int>();
            for (var i = 0; i < fileWithCursors.Length; i++)
            {
                if (fileWithCursors[i] == '|')
                {
                    cursors.Add(i - cursors.Count);
                }
            }

            return (bicepFile, cursors);
        }

        public static (string file, int cursor) GetFileWithSingleCursor(string fileWithCursors)
        {
            var (file, cursors) = GetFileWithCursors(fileWithCursors);
            cursors.Should().HaveCount(1);

            return (file, cursors.Single());
        }
    }
}

