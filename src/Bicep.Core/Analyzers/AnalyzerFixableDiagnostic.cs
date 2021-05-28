// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using System;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers
{
    public class AnalyzerFixableDiagnostic : AnalyzerDiagnostic, IBicepAnalyerFixableDiagnostic
    {
        public AnalyzerFixableDiagnostic(string analyzerName, TextSpan span, DiagnosticLevel level,
            string code, string message, Uri? documentationUri,
            DiagnosticLabel? label, IEnumerable<CodeFix> codeFixes)
            : base(analyzerName, span, level, code, message, documentationUri, label)
        {
            this.Fixes = codeFixes;
        }

        public IEnumerable<CodeFix> Fixes { get; }
    }
}
