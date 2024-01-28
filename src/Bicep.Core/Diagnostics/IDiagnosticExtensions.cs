// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Analyzers;

namespace Bicep.Core.Diagnostics
{
    public static class IDiagnosticExtensions
    {
        public static bool CanBeSuppressed(this IDiagnostic diagnostic)
        {
            return !(diagnostic.Level == DiagnosticLevel.Error && diagnostic is not AnalyzerDiagnostic);
        }

        public static ErrorDiagnostic? AsErrorDiagnostic(this IDiagnostic diagnostic)
        {
            if (diagnostic is ErrorDiagnostic alreadyErrorDiagnostic)
            {
                return alreadyErrorDiagnostic;
            }

            return diagnostic.Level switch
            {
                DiagnosticLevel.Error => diagnostic switch
                {
                    FixableDiagnostic fixable => new FixableErrorDiagnostic(fixable.Span, fixable.Code, fixable.Message, fixable.Uri, fixable.Styling, fixable.Fixes.First(), fixable.Fixes.Skip(1).ToArray()),
                    _ => new ErrorDiagnostic(diagnostic.Span, diagnostic.Code, diagnostic.Message, diagnostic.Uri, diagnostic.Styling),
                },
                _ => null,
            };
        }

        public static Diagnostic WithMaximumDiagnosticLevel(this Diagnostic diagnostic, DiagnosticLevel maximumDiagnosticLevel)
        {
            if (diagnostic.Level > maximumDiagnosticLevel)
            {
                return diagnostic switch
                {
                    FixableErrorDiagnostic fixable => new FixableDiagnostic(
                        fixable.Span,
                        maximumDiagnosticLevel,
                        fixable.Code,
                        fixable.Message,
                        fixable.Uri,
                        fixable.Styling,
                        fixable.Fixes.First(),
                        fixable.Fixes.Skip(1).ToArray()),
                    FixableDiagnostic fixable => new FixableDiagnostic(
                        fixable.Span,
                        maximumDiagnosticLevel,
                        fixable.Code,
                        fixable.Message,
                        fixable.Uri,
                        fixable.Styling,
                        fixable.Fixes.First(),
                        fixable.Fixes.Skip(1).ToArray()),
                    _ => new Diagnostic(
                        diagnostic.Span,
                        maximumDiagnosticLevel,
                        diagnostic.Code,
                        diagnostic.Message,
                        diagnostic.Uri,
                        diagnostic.Styling,
                        diagnostic.Source),
                };
            }

            return diagnostic;
        }
    }
}
