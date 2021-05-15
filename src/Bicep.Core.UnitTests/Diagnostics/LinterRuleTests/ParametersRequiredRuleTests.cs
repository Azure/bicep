// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class ParametersRequiredRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, int expectedDiagnosticCount)
        {
            base.CompileAndTest(ParametersRequiredRule.Code, text, expectedDiagnosticCount);
        }

        [DataRow(0, @"
// No resources - rule not applied
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(0, @"
// No resources - rule not applied
param password string
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(1, @"
var sum = 1 + 3
output sub int = sum
resource createDevopsPipeline 'Microsoft.Unknown/whatever@2019-10-01-preview' = {
}
")]
        [DataRow(0, @"
param password string
var sum = 1 + 3
output sub int = sum
resource createDevopsPipeline 'Microsoft.Unknown/whatever@2019-10-01-preview' = {
}
")]
        [DataTestMethod]
        public void TestRule(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }
    }
}
