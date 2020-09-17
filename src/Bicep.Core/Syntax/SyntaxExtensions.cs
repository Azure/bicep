// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public static class SyntaxExtensions
    {
        public static IList<Diagnostic> GetParseDiagnostics(this SyntaxBase syntax)
        {
            var diagnostics = new List<Diagnostic>();
            var parseErrorVisitor = new ParseDiagnosticsVisitor(diagnostics);
            parseErrorVisitor.Visit(syntax);

            return diagnostics;
        }

        public static bool HasParseErrors(this SyntaxBase syntax)
            => syntax.GetParseDiagnostics().Any(d => d.Level == DiagnosticLevel.Error);
    }
}

