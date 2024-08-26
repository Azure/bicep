// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Analyzers;

namespace Bicep.Core.Diagnostics
{
    public static class IDiagnosticExtensions
    {
        public static bool CanBeSuppressed(this IDiagnostic diagnostic)
            => IsLinterRaised(diagnostic) || !IsError(diagnostic);

        public static bool IsLinterRaised(this IDiagnostic diagnostic)
            => diagnostic.Source == DiagnosticSource.CoreLinter;

        public static bool IsError(this IDiagnostic diagnostic)
            => diagnostic.Level == DiagnosticLevel.Error;

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
