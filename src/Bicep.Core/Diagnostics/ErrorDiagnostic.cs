// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Parser;

namespace Bicep.Core.Diagnostics
{
    // roughly equivalent to the 'SyntaxDiagnosticInfo' class in Roslyn
    public class ErrorDiagnostic : Diagnostic
    {
        public ErrorDiagnostic(TextSpan span, string code, string message)
            : base(span, DiagnosticLevel.Error, code, message)
        {
        }

        public ErrorDiagnostic WithSpan(TextSpan newSpan)
            => new ErrorDiagnostic(newSpan, Code, Message);

        public static bool IsCyclicExpressionError(ErrorDiagnostic diagnostic) => 
            string.Equals(diagnostic.Code, DiagnosticBuilder.BCP061CyclicExpressionCode, StringComparison.OrdinalIgnoreCase);
    }
}
