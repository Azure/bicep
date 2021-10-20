// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions.Execution;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class AssertionScopeExtensions
    {
        /// <summary>
        /// Prints the program syntax with line numbers and a cursor if a test fails in the given assertion scope.
        /// </summary>
        public static AssertionScope WithVisualCursor(this AssertionScope assertionScope, BicepFile bicepFile, IPositionable cursorPosition)
            => WithAnnotatedSource(
                assertionScope,
                bicepFile,
                "cursor info",
                new PrintHelper.Annotation(cursorPosition.Span, "cursor").AsEnumerable());

        /// <summary>
        /// Prints the program syntax with line numbers and diagnostics if a test fails in the given assertion scope.
        /// </summary>
        public static AssertionScope WithVisualDiagnostics(this AssertionScope assertionScope, BicepFile bicepFile, IEnumerable<IDiagnostic> diagnostics)
            => WithAnnotatedSource(
                assertionScope,
                bicepFile,
                "diagnostics",
                diagnostics.Select(x => new PrintHelper.Annotation(x.Span, $"[{x.Code} ({x.Level})] {x.Message}")));

        /// <summary>
        /// Prints the entire program syntax with line numbers if a test fails in the given assertion scope.
        /// </summary>
        public static AssertionScope WithFullSource(this AssertionScope assertionScope, BicepFile bicepFile)
        {
            assertionScope.AddReportable(
                "source",
                () => PrintHelper.PrintFullSource(bicepFile, 1, true));

            return assertionScope;
        }

        public static AssertionScope WithAnnotatedSource(AssertionScope assertionScope, BicepFile bicepFile, string contextName, IEnumerable<PrintHelper.Annotation> annotations)
        {
            assertionScope.AddReportable(
                contextName,
                () => PrintHelper.PrintWithAnnotations(bicepFile, annotations, 1, true));

            return assertionScope;
        }
    }
}
