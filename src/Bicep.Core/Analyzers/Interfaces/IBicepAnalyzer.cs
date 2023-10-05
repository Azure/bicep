// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;

namespace Bicep.Core.Analyzers.Interfaces
{
    public interface IBicepAnalyzer
    {
        IEnumerable<IBicepAnalyzerRule> GetRuleSet();
        IEnumerable<IDiagnostic> Analyze(SemanticModel model);
    }
}
