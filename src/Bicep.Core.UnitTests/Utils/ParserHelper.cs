// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.UnitTests.Utils
{
    public static class ParserHelper
    {
        public static ProgramSyntax Parse(string text)
        {
            var parser = new Core.Parser.Parser(text);

            return parser.Program();
        }

        public static SyntaxBase ParseExpression(string text, bool allowComplexLiterals = true) => new Core.Parser.Parser(text).Expression(allowComplexLiterals);
    }
}

