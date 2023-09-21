// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;

namespace Bicep.Core.Analyzers
{
    public class AnalyzerDiagnostic : Diagnostic, IDiagnostic
    {
        public AnalyzerDiagnostic(string analyzerName, TextSpan span, DiagnosticLevel level, string code, string message, Uri? documentationUri = null, DiagnosticStyling styling = DiagnosticStyling.Default)
            : base(span, level, code, message, documentationUri, styling, $"{LanguageConstants.LanguageId} {analyzerName}")
        {
        }
    }
}
