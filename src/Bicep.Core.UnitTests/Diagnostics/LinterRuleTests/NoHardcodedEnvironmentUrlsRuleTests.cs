// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

// TODO: Test with different configs
namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class NoHardcodedEnvironmentUrlsRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, int expectedDiagnosticCount)
        {
            using (new AssertionScope($"linter errors for this code:\n{text}\n"))
            {
                var errors = GetDiagnostics(NoHardcodedEnvironmentUrlsRule.Code, text);
                errors.Should().HaveCount(expectedDiagnosticCount);
            }
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
        [DataRow(1, @"
param param1 string
var location = 'http://management.core.windows.net'
output sub int = sum
")]
        [DataRow(1, @"
param param1 string
var location = 'https://management.core.windows.net'
output sub int = sum
")]
        [DataRow(1, @"
param param1 string
var location = 'http://MANAGEMENT.core.windows.net'
output sub int = sum
")]
        [DataRow(1, @"
param param1 string
var location = 'http://MANAGEMENT.core.windows.net'
output sub int = sum
")]
        [DataRow(1, @"
resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'name'
  location: resourceGroup().location
  sku: {
    name: 'azuredatalakestore.net'
    capacity: 1
  }
}
")]
        [DataRow(1, @"
resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'name'
  location: resourceGroup().location
  sku: {
    name: 'azuredatalakestore.net'
    capacity: 1
  }
}
")]
        [DataTestMethod]
        public void Simple(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow(1, @"
param p1 string
param p2 string
var a = '${p1}azuredatalakestore.net${p2}'
")]
        [DataRow(1, @"
param p1 string
param p2 string
var a = '${p1}azuredatalakestore.net${p2}'
")]
        [DataTestMethod]
        public void InsideStringInterpolation(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow(1, @"
var a = 'azuredatalakestore.net' + 1
")]
        [DataRow(1, @"
param p1 string
param p2 string
var a = concat('${p1}${'azuredatalakestore.net'}${p2}', 'foo')
")]
        [DataTestMethod]
        public void InsideExpressions(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }
    }
}

