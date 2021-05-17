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
        [DataRow(3, @"
// Syntax errors
resource abc 'Microsoft.AAD/domainServices@2021-03-01'
param
param p1
param p2 =
        ")]
        [DataTestMethod]
        public void TestRule(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow(0, @"
@minLength(3)
@maxLength(11)
param namePrefix string
param location string = resourceGroup().location

module stgModule './storageAccount.bicep' = {
  name: 'storageDeploy'
  params: {
    storagePrefix: namePrefix
    location: location
  }
}

output storageEndpoint object = stgModule.outputs.storageEndpoint
")]
        [DataTestMethod]
        public void Modules(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow(0, @"
param location string

resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = if (false) {
  name: 'myZone'
  location: location
}
")]
        [DataTestMethod]
        public void Conditions(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }
    }
}
