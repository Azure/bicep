// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    public class LinterRuleTestsBase
    {
        protected IBicepAnalyzerDiagnostic[] GetDiagnostics(string ruleCode, string text)
        {
            var compilationResult = CompilationHelper.Compile(text);

            var internalRuleErrors = compilationResult.Diagnostics.Where(d => d.Code == LinterAnalyzer.FailedRuleCode).ToArray();
            internalRuleErrors.Count().Should().Be(0, "There should never be linter FailedRuleCode errors");

            return compilationResult.Diagnostics.OfType<IBicepAnalyzerDiagnostic>().Where(d => d.Code == ruleCode).ToArray();
        }
    }
}
