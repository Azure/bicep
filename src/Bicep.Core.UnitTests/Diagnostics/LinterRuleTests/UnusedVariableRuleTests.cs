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
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class UnusedVariableRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, int expectedDiagnosticCount)
        {
            using (new AssertionScope($"linter errors for this code:\n{text}\n"))
            {
                var errors = GetDiagnostics(UnusedVariableRule.Code, text);
                errors.Should().HaveCount(expectedDiagnosticCount);
            }
        }

        [DataRow(1, @"
var password = 'hello'
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(3, @"
var var1 = 'var1'
var var2 = 'var2'
var var3 = 'var3'
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(2, @"
var var1 = 'var1'
var var2 = 4
var var3 = resourceGroup().location
var sum = 1 + 3
output sub int = sum + var2
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
        [DataTestMethod]
        public void TestRule(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow(2, @"
// Syntax errors
var string =
var a string
resource abc 'Microsoft.AAD/domainServices@2021-03-01'
        ")]
        [DataTestMethod]
        public void SyntaxErrors(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }
    }
}
