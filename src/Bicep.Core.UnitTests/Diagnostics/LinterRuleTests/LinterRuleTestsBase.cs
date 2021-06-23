// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    public class LinterRuleTestsBase
    {
        virtual protected void CompileAndTest(string ruleCode, string text, int expectedDiagnosticCount)
        {
            using (new AssertionScope($"linter errors for this code:\n{text}\n"))
            {
                var errors = GetDiagnostics(ruleCode, text);
                errors.Should().HaveCount(expectedDiagnosticCount);
            }
        }

        protected IDiagnostic[] GetDiagnostics(string ruleCode, string text)
        {
            var compilationResult = CompilationHelper.Compile(text);
            var internalRuleErrors = compilationResult.Diagnostics.Where(d => d.Code == LinterAnalyzer.FailedRuleCode).ToArray();
            internalRuleErrors.Count().Should().Be(0, "There should never be linter FailedRuleCode errors");

            return compilationResult.Diagnostics.OfType<IDiagnostic>().Where(d => d.Code == ruleCode).ToArray();
        }
    }
}
