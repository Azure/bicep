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
    public class SecureParameterDefaultRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, int expectedDiagnosticCount)
        {
            base.CompileAndTest(SecureParameterDefaultRule.Code, text, expectedDiagnosticCount);
        }

        [DataRow(0, @"
@secure()
param password string
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(1, @"
@secure()
param param1 string = 'val1'
output sub int = sum
")]
        [DataRow(1, @"
@secure()
param param1 string
@secure()
param param2 string = 'val'
param param3 int
output sub int = sum
")]
        [DataRow(2, @"
@secure()
param param1 string = 'val'
@secure()
param param2 string = 'val'
param param3 int
output sub int = sum
")]
        [DataRow(2, @"
@secure()
param param1 string = 'val'
@secure()
param param2 string = 'val'
@secure()
param param3 int
output sub int = sum
")]
        [DataRow(3, @"
@secure()
param param1 string = 'val'
@secure()
param param2 string = 'val'
@secure()
param param3 int = 5
output sub int = sum
")]
        [DataTestMethod]
        public void TestRule(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

    }
}
