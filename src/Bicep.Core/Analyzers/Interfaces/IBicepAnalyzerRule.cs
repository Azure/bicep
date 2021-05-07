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
        string DocumentationUri { get; }
        bool EnabledForEditing { get; }
        bool EnabledForCLI { get; }
        string RuleName { get; }

        void ConfigureRule(bool enabledOnChange, bool enabledForDocument, DiagnosticLevel level);
        IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model);
    }
}
