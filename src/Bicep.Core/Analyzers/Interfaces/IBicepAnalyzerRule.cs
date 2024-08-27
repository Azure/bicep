// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Analyzers.Interfaces
{
    /// <summary>
    /// Implementing IBicepAnalyzer Rule requires
    /// the implementing class to have a parameterless
    /// constructor which can be discovered through
    /// reflection
    /// </summary>
    /// <remarks>Do not rename or move this type to a different namespace.
    /// We are using a source generator that requires the fully qualified type name of this interface to not change.</remarks>
    public interface IBicepAnalyzerRule
    {
        string Code { get; }

        string Description { get; }

        DiagnosticLevel DefaultDiagnosticLevel { get; }

        DiagnosticStyling DiagnosticStyling { get; }
        Uri? Uri { get; }

        IEnumerable<IDiagnostic> Analyze(SemanticModel model, IServiceProvider serviceProvider);
    }
}
