// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    public class LinterRuleTestsBase
    {
        protected void CompileAndTest(string ruleCode, string text, int expectedDiagnosticCount)
            => AssertRuleCodeDiagnostics(ruleCode, text, diags => diags.Should().HaveCount(expectedDiagnosticCount));

        protected void AssertRuleCodeDiagnostics(string ruleCode, string text, Action<IEnumerable<IDiagnostic>> assertAction)
            => DoWithDiagnosticAnnotations(
                text,
                diag => diag.Code == ruleCode,
                assertAction);

        private void DoWithDiagnosticAnnotations(string text, Func<IDiagnostic, bool> filterFunc, Action<IEnumerable<IDiagnostic>> assertAction)
        {
            var result = CompilationHelper.Compile(text);

            result.Should().NotHaveDiagnosticsWithCodes(new [] { LinterAnalyzer.FailedRuleCode }, "There should never be linter FailedRuleCode errors");

            DiagnosticAssertions.DoWithDiagnosticAnnotations(
                result.Compilation.SourceFileGrouping.EntryPoint,
                result.Diagnostics.Where(filterFunc),
                assertAction);
        }
    }
}
