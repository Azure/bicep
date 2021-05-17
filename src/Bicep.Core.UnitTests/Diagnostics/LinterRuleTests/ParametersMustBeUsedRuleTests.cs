// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class ParametersMustBeUsedRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, int expectedDiagnosticCount)
        {
            base.CompileAndTest(ParametersMustBeUsedRule.Code, text, expectedDiagnosticCount);
        }

        [DataRow(1, @"
param password string
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(3, @"
param param1 string
param param2 string
param param3 string
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(2, @"
param param1 string
param param2 int = 4
param param3 string
var sum = 1 + 3
output sub int = sum + param2
")]
        [DataRow(0, @"
param param2 int = 4
var sum = 1 + 3
output sub int = sum + param2
")]
        [DataRow(0, @"
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(0, @"
// Syntax error
resource abc 'Microsoft.AAD/domainServices@2021-03-01'
        ")]
        [DataTestMethod]
        public void TestRule(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }
    }
}
