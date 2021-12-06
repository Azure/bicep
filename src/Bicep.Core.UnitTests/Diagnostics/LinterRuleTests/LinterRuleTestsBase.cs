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
        public enum OnCompileErrors
        {
            Fail, // Assertion fails if there are compiler errors
            Ignore,
        }

        public record ExpectedCodeFix
        {
            public string Description;
            public string ReplacementText;

            public ExpectedCodeFix(string description, string replacementText)
            {
                this.Description = description;
                this.ReplacementText = replacementText;
            }
        }

        protected void AssertLinterRuleDiagnostics(string ruleCode, string bicepText, string[] expectedMessagesForCode, OnCompileErrors onCompileErrors = OnCompileErrors.Fail)
        {
            AssertLinterRuleDiagnostics(ruleCode, bicepText, onCompileErrors, diags =>
            {
                diags.Select(d => d.Message).Should().BeEquivalentTo(expectedMessagesForCode);
            });
        }

        protected void AssertLinterRuleDiagnostics(string ruleCode, string bicepText, int expectedDiagnosticCountForCode, OnCompileErrors onCompileErrors = OnCompileErrors.Fail)
        {
            AssertLinterRuleDiagnostics(ruleCode, bicepText, onCompileErrors, diags =>
            {
                diags.Should().HaveCount(expectedDiagnosticCountForCode);
            });
        }

        protected void AssertLinterRuleDiagnostics(string ruleCode, string bicepText, Action<IEnumerable<IDiagnostic>> assertAction)
        {
            AssertLinterRuleDiagnostics(ruleCode, bicepText, OnCompileErrors.Fail, assertAction);
        }

        protected void AssertLinterRuleDiagnostics(string ruleCode, string bicepText, OnCompileErrors onCompileErrors, Action<IEnumerable<IDiagnostic>> assertAction)
        {
            RunWithDiagnosticAnnotations(
                  bicepText,
                  diag => diag.Code == ruleCode || (onCompileErrors == OnCompileErrors.Fail && diag.Level == DiagnosticLevel.Error),
                  onCompileErrors,
                  assertAction);
        }

        private void RunWithDiagnosticAnnotations(string bicepText, Func<IDiagnostic, bool> filterFunc, OnCompileErrors onCompileErrors, Action<IEnumerable<IDiagnostic>> assertAction)
        {
            var result = CompilationHelper.Compile(bicepText);
            using (new AssertionScope().WithFullSource(result.BicepFile))
            {
                result.Should().NotHaveDiagnosticsWithCodes(new[] { LinterAnalyzer.FailedRuleCode }, "There should never be linter FailedRuleCode errors");

                if (onCompileErrors == OnCompileErrors.Fail)
                {
                    var compileErrors = result.Diagnostics.Where(d => d.Level == DiagnosticLevel.Error);
                    DiagnosticAssertions.DoWithDiagnosticAnnotations(
                    result.Compilation.SourceFileGrouping.EntryPoint,
                    compileErrors,
                    diags => diags.Should().HaveCount(0));
                }

                IDiagnostic[] diagnosticsMatchingCode = result.Diagnostics.Where(filterFunc).ToArray();
                DiagnosticAssertions.DoWithDiagnosticAnnotations(
                    result.Compilation.SourceFileGrouping.EntryPoint,
                    result.Diagnostics.Where(filterFunc),
                    assertAction);
            }
        }

        protected void ExpectPass(string bicepText, OnCompileErrors onCompileErrors = OnCompileErrors.Fail)
        {
            AssertLinterRuleDiagnostics(NoHardcodedLocationRule.Code, bicepText, new string[] { }, onCompileErrors);
        }

        protected void ExpectPass(string bicepText, string module1Text, OnCompileErrors onCompileErrors = OnCompileErrors.Fail)
        {
            AssertLinterRuleDiagnostics(NoHardcodedLocationRule.Code, bicepText, new string[] { }, onCompileErrors);
        }

        protected void ExpectFail(string bicepText, string expectedMessage, OnCompileErrors onCompileErrors = OnCompileErrors.Fail)
        {
            AssertLinterRuleDiagnostics(NoHardcodedLocationRule.Code, bicepText, new string[] { expectedMessage }, onCompileErrors);
        }

        protected void ExpectFailWithFix(string bicepText, string expectedMessage, ExpectedCodeFix expectedFix, OnCompileErrors onCompileErrors = OnCompileErrors.Fail)
        {
            AssertLinterRuleDiagnostics(
              NoHardcodedLocationRule.Code,
              bicepText,
              diagnostics =>
              {
                  diagnostics.Should().HaveCount(1);
                  diagnostics.Should().HaveFixableDiagnostics(new[] {
                    (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, expectedMessage, expectedFix.Description, expectedFix.ReplacementText)
                  }); ;
              }
            );
        }
    }
}
