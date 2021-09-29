// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using System;
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

        Uri? Uri { get; }

        void Configure(AnalyzersConfiguration configuration);

        IEnumerable<IDiagnostic> Analyze(SemanticModel model);
    }
}
