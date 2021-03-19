// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Interfaces
{
    public interface IBicepAnalyzer
    {
        public IEnumerable<IBicepAnalyzerRule> GetRuleSet();
        public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model);
    }
}
