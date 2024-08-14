// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Analyzers.Linter
{
    public interface ILinterRulesProvider
    {
        ImmutableDictionary<string, (string diagnosticLevelConfigProperty, DiagnosticLevel defaultDiagnosticLevel)> GetLinterRules();

        IEnumerable<Type> GetRuleTypes();
    }
}
