// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

// TODO: Test with different configs
namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class EnvironmentUrlHardcodedRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, int expectedDiagnosticCount)
        {
            var errors = GetDiagnostics(EnvironmentUrlHardcodedRule.Code, text);
            errors.Should().HaveCount(expectedDiagnosticCount);
        }

        [DataRow(0, @"
param password string
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(0, @"
param param1 string
var location = 'somehost.com'
output sub int = sum
")]
        [DataRow(1, @"
param param1 string
var location = 'management.core.windows.net'
output sub int = sum
")]
        [DataTestMethod]
        public void TestRule(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }
    }
}
