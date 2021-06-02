// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using System;

namespace Bicep.Core.Analyzers
{
    public class AnalyzerDiagnostic : Diagnostic, IDiagnostic
    {
        public AnalyzerDiagnostic(string analyzerName, TextSpan span, DiagnosticLevel level, string code, string message, Uri? documentationUri, DiagnosticLabel? label = default)
            : base(span, level, code, message, documentationUri, label)
        {
            this.Source = $"{LanguageConstants.LanguageId} {analyzerName}";
        }
    }
}
