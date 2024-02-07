// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Diagnostics
{
    // roughly equivalent to the 'SyntaxDiagnosticInfo' class in Roslyn
    public class ErrorDiagnostic(TextSpan span, string code, string message, Uri? documentationUri = null, DiagnosticStyling styling = DiagnosticStyling.Default) : Diagnostic(span, DiagnosticLevel.Error, code, message, documentationUri, styling)
    {
        public ErrorDiagnostic WithSpan(TextSpan newSpan)
            => new(newSpan, Code, Message);
    }
}
