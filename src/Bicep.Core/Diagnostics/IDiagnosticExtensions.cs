// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Analyzers;

namespace Bicep.Core.Diagnostics
{
    public static class IDiagnosticExtensions
    {
        public static bool CanBeSuppressed(this IDiagnostic diagnostic)
        {
            return diagnostic.Source == DiagnosticSource.Linter || diagnostic.Level != DiagnosticLevel.Error;
        }

        public static Diagnostic WithMaximumDiagnosticLevel(this Diagnostic diagnostic, DiagnosticLevel maximumDiagnosticLevel)
        {
            if (diagnostic.Level > maximumDiagnosticLevel)
            {
                return diagnostic with { Level = maximumDiagnosticLevel };
            }

            return diagnostic;
        }
    }
}
