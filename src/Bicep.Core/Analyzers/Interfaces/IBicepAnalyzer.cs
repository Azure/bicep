// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Configuration;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;

namespace Bicep.Core.Analyzers.Interfaces
{
    public interface IBicepAnalyzer
    {
        string AnalyzerName { get; }

        bool IsEnabled(AnalyzersConfiguration configuration);

        IEnumerable<IBicepAnalyzerRule> GetRuleSet();
        IEnumerable<IDiagnostic> Analyze(SemanticModel model);
    }
}
