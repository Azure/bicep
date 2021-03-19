// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;

namespace Bicep.Core.Analyzers
{
    internal class AnalyzerDiagnostic : Diagnostic, IBicepAnalyzerDiagnostic
    {
        internal AnalyzerDiagnostic(string analyzerName, TextSpan span, DiagnosticLevel level, string code, string message, DiagnosticLabel? label = default)
            : base(span, level, code, message, label)
        {
            this.AnalyzerName = analyzerName;
        }

        public string AnalyzerName { get; }
    }
}
