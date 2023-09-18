// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class AutomaticOSImageUpgradeRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text)
        {
            AssertLinterRuleDiagnostics(AutomaticOSImageUpgradeRule.Code, text, diags =>
            {
                diags.Should().BeEmpty();
            });
        }

        [DataRow("""
param location string = resourceGroup().location

resource testVmScaleSet 'Microsoft.Compute/virtualMachineScaleSets@2022-11-01' = {
    location: location
    name: 'myScaleSet'
    properties: {
        orchestrationMode: 'Uniform' // Flexible does not support SF extensions
        zoneBalance: true
        singlePlacementGroup: true
        overprovision: false
        upgradePolicy: {
            mode: 'Automatic'
            automaticOSUpgradePolicy: {
                enableAutomaticOSUpgrade: false
            }
        }
    }
}
""")]
        [DataRow("""
param location string = resourceGroup().location

resource testVmScaleSet 'Microsoft.Compute/virtualMachineScaleSets@2022-11-01' = {
    location: location
    name: 'myScaleSet'
    properties: {
        orchestrationMode: 'Uniform' // Flexible does not support SF extensions
        zoneBalance: true
        singlePlacementGroup: true
        overprovision: false

    }
}
""")]
        [DataTestMethod]
        public void TestRule(string text)
        {
            CompileAndTest(text);
        }
    }
}
