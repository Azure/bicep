// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions.Execution;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class AssertionScopeExtensions
    {
        /// <summary>
        /// Prints the program syntax with line numbers and a cursor if a test fails in the given assertion scope.
        /// </summary>
        public static AssertionScope WithVisualCursor(this AssertionScope assertionScope, SyntaxTree syntaxTree, IPositionable cursorPosition)
            => WithAnnotatedSource(
                assertionScope,
                syntaxTree,
                "cursor info",
                new PrintHelper.Annotation(cursorPosition.Span, "cursor").AsEnumerable());

        /// <summary>
        /// Prints the program syntax with line numbers and diagnostics if a test fails in the given assertion scope.
        /// </summary>
        public static AssertionScope WithVisualDiagnostics(this AssertionScope assertionScope, SyntaxTree syntaxTree, IEnumerable<Diagnostic> diagnostics)
            => WithAnnotatedSource(
                assertionScope,
                syntaxTree,
                "diagnostics",
                diagnostics.Select(x => new PrintHelper.Annotation(x.Span, $"[{x.Code} ({x.Level})] {x.Message}")));

        private static AssertionScope WithAnnotatedSource(AssertionScope assertionScope, SyntaxTree syntaxTree, string contextName, IEnumerable<PrintHelper.Annotation> annotations)
        {
            // TODO: figure out how to set this only on failure, rather than always calculating it
            assertionScope.AddReportable(
                contextName,
                PrintHelper.PrintWithAnnotations(syntaxTree, annotations, 1, true));

            return assertionScope;
        }
    }
}
