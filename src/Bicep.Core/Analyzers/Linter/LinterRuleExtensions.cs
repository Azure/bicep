// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Analyzers.Linter
{
    public static class LinterRuleExtensions
    {
        public static bool IsEnabled(this IBicepAnalyzerRule rule)
            => rule.DiagnosticLevel != DiagnosticLevel.Off;
    }
}
