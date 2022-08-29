// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.UnitTests.Utils
{
    public static class ParamsParserHelper
    {
        public static ProgramSyntax ParamsParse(string text)
        {
            var parser = new ParamsParser(text);

            return parser.Program();
        }

        public static SyntaxBase ParseExpression(string text, ExpressionFlags expressionFlags = ExpressionFlags.AllowComplexLiterals) => new Parser(text).Expression(expressionFlags);

        public static (string file, IReadOnlyList<int> cursors) GetFileWithCursors(string fileWithCursors)
        {
            var bicepParamFile = fileWithCursors.Replace("|", "");

            var cursors = new List<int>();
            for (var i = 0; i < fileWithCursors.Length; i++)
            {
                if (fileWithCursors[i] == '|')
                {
                    cursors.Add(i - cursors.Count);
                }
            }

            return (bicepParamFile, cursors);
        }
    }
}


