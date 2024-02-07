// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;

namespace Bicep.Core.Analyzers
{
    public class AnalyzerFixableDiagnostic(string analyzerName, TextSpan span, DiagnosticLevel level,
        string code, string message, Uri? documentationUri,
        DiagnosticStyling styling, IEnumerable<CodeFix> codeFixes) : AnalyzerDiagnostic(analyzerName, span, level, code, message, documentationUri, styling), IBicepAnalyerFixableDiagnostic
    {
        public IEnumerable<CodeFix> Fixes { get; } = codeFixes;
    }
}
