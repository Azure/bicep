// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class MaxNumberOutputsRuleTests : LinterRuleTestsBase
    {
        [TestMethod]
        public void ParameterNameInFormattedMessage()
        {
            var ruleToTest = new MaxNumberOutputsRule();
            ruleToTest.GetMessage(1).Should().Be("Too many outputs. Number of outputs is limited to 1.");
        }

        private void CompileAndTest(string text, params string[] unusedParams)
        {
            AssertLinterRuleDiagnostics(MaxNumberOutputsRule.Code, text, onCompileErrors: OnCompileErrors.Ignore,  diags =>
            {
                if (unusedParams.Any())
                {
                    var rule = new MaxNumberOutputsRule();
                    string[] expectedMessages = unusedParams.Select(p => rule.GetMessage(MaxNumberOutputsRule.MaxNumber)).ToArray();
                    diags.Select(e => e.Message).Should().ContainInOrder(expectedMessages);
                }
                else
                {
                    diags.Should().BeEmpty();
                }
            });
        }

        [DataRow(@"
                    output o1 string
                    output o2 string
                    output o3 string
                    output o4 string
                    output o5 string
                    output o6 string
                    output o7 string
                    output o8 string
                    output o9 string
                    output o10 string
                    output o11 string
                    output o12 string
                    output o13 string
                    output o14 string
                    output o15 string
                    output o16 string
                    output o17 string
                    output o18 string
                    output o19 string
                    output o20 string
                    output o21 string
                    output o22 string
                    output o23 string
                    output o24 string
                    output o25 string
                    output o26 string
                    output o27 string
                    output o28 string
                    output o29 string
                    output o30 string
                    output o31 string
                    output o32 string
                    output o33 string
                    output o34 string
                    output o35 string
                    output o36 string
                    output o37 string
                    output o38 string
                    output o39 string
                    output o40 string
                    output o41 string
                    output o42 string
                    output o43 string
                    output o44 string
                    output o45 string
                    output o46 string
                    output o47 string
                    output o48 string
                    output o49 string
                    output o50 string
                    output o51 string
                    output o52 string
                    output o53 string
                    output o54 string
                    output o55 string
                    output o56 string
                    output o57 string
                    output o58 string
                    output o59 string
                    output o60 string
                    output o61 string
                    output o62 string
                    output o63 string
                    output o64 string
        ")]
        [DataRow(@"
            output o1 string
            output o2 string
            output o3 string
            output o4 string
            output o5 string
            output o6 string
            output o7 string
            output o8 string
            output o9 string
            output o10 string
            output o11 string
            output o12 string
            output o13 string
            output o14 string
            output o15 string
            output o16 string
            output o17 string
            output o18 string
            output o19 string
            output o20 string
            output o21 string
            output o22 string
            output o23 string
            output o24 string
            output o25 string
            output o26 string
            output o27 string
            output o28 string
            output o29 string
            output o30 string
            output o31 string
            output o32 string
            output o33 string
            output o34 string
            output o35 string
            output o36 string
            output o37 string
            output o38 string
            output o39 string
            output o40 string
            output o41 string
            output o42 string
            output o43 string
            output o44 string
            output o45 string
            output o46 string
            output o47 string
            output o48 string
            output o49 string
            output o50 string
            output o51 string
            output o52 string
            output o53 string
            output o54 string
            output o55 string
            output o56 string
            output o57 string
            output o58 string
            output o59 string
            output o60 string
            output o61 string
            output o62 string
            output o63 string
            output o64 string
            output o65 string
            ",
            "o1","o2","o3","o4","o5","o6","o7","o8","o9","o10","o11","o12","o13","o14","o15","o16","o17","o18","o19","o20","o21","o22","o23","o24","o25","o26","o27","o28","o29","o30","o31","o32","o33","o34","o35","o36","o37","o38","o39","o40","o41","o42","o43","o44","o45","o46","o47","o48","o49","o50","o51","o52","o53","o54","o55","o56","o57","o58","o59","o60","o61","o62","o63","o64","o65")]
        [DataTestMethod]
        public void TestRule(string text, params string[] unusedParams)
        {
            CompileAndTest(text, unusedParams);
        }
    }
}
