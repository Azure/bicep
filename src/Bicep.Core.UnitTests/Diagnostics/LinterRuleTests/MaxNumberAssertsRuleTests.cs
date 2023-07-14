// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class MaxNumberAssertsRuleTests : LinterRuleTestsBase
    {
        [TestMethod]
        public void ParameterNameInFormattedMessage()
        {
            var ruleToTest = new MaxNumberAssertsRule();
            ruleToTest.GetMessage(1).Should().Be("Too many predeployment conditions. Number of 'assert' statements is limited to 1.");
        }

        private void CompileAndTest(string text, bool tooManyAsserts)
        {
            AssertLinterRuleDiagnostics(MaxNumberAssertsRule.Code, text, diags =>
            {
                if (tooManyAsserts)
                {
                    var rule = new MaxNumberAssertsRule();
                    diags.Should().ContainSingleDiagnostic("max-asserts", DiagnosticLevel.Error, rule.GetMessage(MaxNumberAssertsRule.MaxNumber));
                }
                else
                {
                    diags.Should().BeEmpty();
                }
            },
            new Options(OnCompileErrors.Ignore));
        }

        [DataRow(@"
            assert a1 = true
            assert a2 = true
            assert a3 = true
            assert a4 = true
            assert a5 = true
            assert a6 = true
            assert a7 = true
            assert a8 = true
            assert a9 = true
            assert a10 = true
            assert a11 = true
            assert a12 = true
            assert a13 = true
            assert a14 = true
            assert a15 = true
            assert a16 = true
            assert a17 = true
            assert a18 = true
            assert a19 = true
            assert a20 = true
            assert a21 = true
            assert a22 = true
            assert a23 = true
            assert a24 = true
            assert a25 = true
            assert a26 = true
            assert a27 = true
            assert a28 = true
            assert a29 = true
            assert a30 = true
            assert a31 = true
            assert a32 = true",
            false
        )]

        [DataRow(@"
            assert a1 = true
            assert a2 = true
            assert a3 = true
            assert a4 = true
            assert a5 = true
            assert a6 = true
            assert a7 = true
            assert a8 = true
            assert a9 = true
            assert a10 = true
            assert a11 = true
            assert a12 = true
            assert a13 = true
            assert a14 = true
            assert a15 = true
            assert a16 = true
            assert a17 = true
            assert a18 = true
            assert a19 = true
            assert a20 = true
            assert a21 = true
            assert a22 = true
            assert a23 = true
            assert a24 = true
            assert a25 = true
            assert a26 = true
            assert a27 = true
            assert a28 = true
            assert a29 = true
            assert a30 = true
            assert a31 = true
            assert a32 = true
            assert a33 = true",
            true
        )]

        [DataTestMethod]
        public void TestRule(string text, bool tooManyAsserts)
        {
            CompileAndTest(text, tooManyAsserts);
        }
    }
}
