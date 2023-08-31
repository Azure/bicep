// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers;
using Bicep.Core.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class IDiagnosticExtensions
    {
        public static IEnumerable<IDiagnostic> ExcludingLinterDiagnostics(this IEnumerable<IDiagnostic> diagnostics)
        {
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

        public static IEnumerable<IDiagnostic> OnlyIncludingErrorDiagnostics(this IEnumerable<IDiagnostic> diagnostics)
            => diagnostics.Where(d => d.Level == DiagnosticLevel.Error);
    }
}
