// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    public class LinterRuleTestsBase
    {
        protected void CompileAndTest(string ruleCode, string text, int expectedDiagnosticCount)
        {
            var compilationResult = CompilationHelper.Compile(text);

            var internalRuleErrors = compilationResult.Diagnostics.Where(d => d.Code == LinterRuleBase.FailedRuleCode).ToArray();
            Assert.AreEqual(0, internalRuleErrors.Count(), "There were internal linter rule errors");

            var ruleErrors = compilationResult.Diagnostics.Where(d => d.Code == ruleCode).ToArray();
            Assert.AreEqual(expectedDiagnosticCount, ruleErrors.Count());
        }
    }
}
