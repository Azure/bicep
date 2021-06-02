// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Parsing;

namespace Bicep.Core.Diagnostics
{
    // roughly equivalent to the 'SyntaxDiagnosticInfo' class in Roslyn
    public class ErrorDiagnostic : Diagnostic
    {
        public ErrorDiagnostic(TextSpan span, string code, string message, Uri? documentationUri = null, DiagnosticLabel? label = null)
            : base(span, DiagnosticLevel.Error, code, message, documentationUri, label)
        {
        }

        public ErrorDiagnostic WithSpan(TextSpan newSpan)
            => new ErrorDiagnostic(newSpan, Code, Message);
    }
}
