// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    public class LinterRuleTestsBase
    {
        protected IBicepAnalyzerDiagnostic[] GetDiagnostics(string ruleCode, string text)
        {
            var compilationResult = CompilationHelper.Compile(text);

            var internalRuleErrors = compilationResult.Diagnostics.Where(d => d.Code == LinterRuleBase.FailedRuleCode).ToArray();
            Assert.AreEqual(0, internalRuleErrors.Count(), "There were internal linter rule errors");

            return compilationResult.Diagnostics.OfType<IBicepAnalyzerDiagnostic>().Where(d => d.Code == ruleCode).ToArray();
        }
    }
}
