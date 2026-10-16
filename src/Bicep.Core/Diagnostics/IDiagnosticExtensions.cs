// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Analyzers;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.Diagnostics
{
    public static class IDiagnosticExtensions
    {
        public static bool CanBeSuppressed(this IDiagnostic diagnostic)
            => IsLinterRaised(diagnostic) || !IsError(diagnostic);

        public static bool IsSuppressedAt(this IDiagnostic diagnostic, BicepSourceFile sourceFile)
            => diagnostic.CanBeSuppressed()
            && sourceFile.DisabledDiagnosticsCache.IsDisabledAtPosition(diagnostic.Code, diagnostic.Span.Position);

        public static bool IsLinterRaised(this IDiagnostic diagnostic)
            => diagnostic.Source is DiagnosticSource.CoreLinter or DiagnosticSource.PolicyLinter;

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
