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
        public static ProgramSyntax Parse(string text) => new Core.Parser.Parser(text).Program();

        public static ProgramSyntax Parse(string text, Action<IEnumerable<Diagnostic>> onDiagnosticsFunc)
        {
            var program = Parse(text);
            var diagnostics = program.GetParseDiagnostics();
            onDiagnosticsFunc(diagnostics);

            return program;
        }

        public static SyntaxBase ParseExpression(string text) => new Core.Parser.Parser(text).Expression();
    }
}

