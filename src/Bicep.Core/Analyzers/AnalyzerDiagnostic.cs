// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;

namespace Bicep.Core.Analyzers
{
    public class AnalyzerDiagnostic(string analyzerName, TextSpan span, DiagnosticLevel level, string code, string message, Uri? documentationUri = null, DiagnosticStyling styling = DiagnosticStyling.Default) : Diagnostic(span, level, code, message, documentationUri, styling, $"{LanguageConstants.LanguageId} {analyzerName}"), IDiagnostic
    {
    }
}
