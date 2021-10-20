// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Analyzers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Analyzers.Linter;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class IDiagnosticExtensions
    {
        public static IEnumerable<IDiagnostic> ExcludingLinterDiagnostics(this IEnumerable<IDiagnostic> diagnostics, params string[] codes)
        {
            diagnostics.Should().NotContainDiagnostic(LinterAnalyzer.FailedRuleCode, "Should never get LinterAnalyzer.FailedRuleCode");
            return diagnostics.Where(d => d is not AnalyzerDiagnostic);
        }

        public static IEnumerable<IDiagnostic> ExcludingCode(this IEnumerable<IDiagnostic> diagnostics, params string[] codes)
        {
            return diagnostics.Where(x => !codes.Contains(x.Code));
        }

        public static IEnumerable<IDiagnostic> ExcludingMissingTypes(this IEnumerable<IDiagnostic> diagnostics)
        {
            return diagnostics.ExcludingCode("BCP081");
        }
    }
}
