// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Interfaces
{
    /// <summary>
    /// Implementing IBicepAnalyzer Rule requires
    /// the implementing class to have a parameterless
    /// constructor which can be discoverd through
    /// reflection
    /// </summary>
    public interface IBicepAnalyzerRule
    {
        string AnalyzerName { get; }
        string Code { get; }
        string Description { get; }
        DiagnosticLevel DiagnosticLevel { get; }
        DiagnosticLabel? DiagnosticLabel { get; }
        string DocumentationUri { get; }
        string RuleName { get; }
        bool Enabled { get; }
        void ConfigureRule(DiagnosticLevel level);
        IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model);
    }
}
