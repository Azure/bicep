// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Diagnostics
{
    // roughly equivalent to the 'SyntaxDiagnosticInfo' class in Roslyn
    public class Diagnostic : IPositionable
    {
        public Diagnostic(TextSpan span, DiagnosticLevel level, string code, string message, DiagnosticLabel? label = null)
        {
            Span = span;
            Level = level;
            Code = code;
            Message = message;
            Label = label;
        }

        public TextSpan Span { get; }

        public DiagnosticLevel Level { get; }

        public DiagnosticLabel? Label { get; }

        public string Code { get; }

        public string Message { get; }
    }
}
