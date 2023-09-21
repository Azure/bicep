// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;

namespace Bicep.Core.Analyzers
{
    public class AnalyzerFixableDiagnostic : AnalyzerDiagnostic, IBicepAnalyerFixableDiagnostic
    {
        public AnalyzerFixableDiagnostic(string analyzerName, TextSpan span, DiagnosticLevel level,
            string code, string message, Uri? documentationUri,
            DiagnosticStyling styling, IEnumerable<CodeFix> codeFixes)
            : base(analyzerName, span, level, code, message, documentationUri, styling)
        {
            this.Fixes = codeFixes;
        }

        public IEnumerable<CodeFix> Fixes { get; }
    }
}
