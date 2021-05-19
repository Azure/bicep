
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class UnnecessaryDependsOnRuleTests : LinterRuleTestsBase
    {
        private void ExpectPass(string text)
        {
            using (new AssertionScope($"linter errors for this code:\n{text}\n"))
            {
                var errors = GetDiagnostics(InterpolateNotConcatRule.Code, text);
                errors.Should().HaveCount(0, $"Expecting linter rule to pass. Text: {text}");
            }
        }

        private void ExpectDiagnosticWithFix(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, new string[] { expectedFix });
        }

        private void ExpectDiagnosticWithFix(string text, string[] expectedFixes)
        {
            using (new AssertionScope($"linter errors for this code:\n{text}\n"))
            {
                var errors = GetDiagnostics(InterpolateNotConcatRule.Code, text);
                errors.Should().HaveCount(expectedFixes.Length, $"expected one fix per testcase.  Text: {text}");

                errors.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.Should().HaveCount(1);
                errors.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.First().Replacements.Should().HaveCount(1);
                var a = errors.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.SelectMany(f => f.Replacements.SelectMany(r => r.Text));
            }
        }
    }
}
