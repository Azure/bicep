// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Interfaces
{
    public interface IBicepAnalyzerRule
    {
        string AnalyzerName { get; }
        string Code { get; }
        string Description { get; }
        DiagnosticLevel DiagnosticLevel { get; }
        string DocumentationUri { get; }
        bool EnabledForEdits { get; }
        bool EnabledForCLI { get; }
        string RuleName { get; }

        void ConfigureRule(bool enabledOnChange, bool enabledForDocument, DiagnosticLevel level);
        IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model);
    }
}
