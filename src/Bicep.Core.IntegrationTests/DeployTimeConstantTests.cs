// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class DeployTimeConstantTests
    {
        [TestMethod]
        public void DtcValidation_EntireResourceOrModuleAccessAtInvalidLocations_ProducesDiagnostics()
        {
            var result = CompilationHelper.Compile(@"
var foo = [for x in [
  dnsZone
  storageAccounts[0]
  dnsZone::aRecord
]: {
  value1: appPlan
  value2: [
    bar
  ]
}]

var bar = {
  value: dnsZone::aRecord
}

resource appPlan 'Microsoft.Web/serverfarms@2020-12-01' = if (dnsZone::aRecord != null) {
  name: 'appPlan'
  location: resourceGroup().location
  sku: {
    name: 'F1'
    capacity: 1
  }
}

resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'dnsZone'
  location: 'global'
  
  resource aRecord 'A' = {
    name: 'aRecord'
  }
}

resource storageAccounts 'Microsoft.Storage/storageAccounts@2021-02-01' = [for i in range(0, 2): {
  name: 'mystorage-${i}'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
    tier: 'Premium'
  }
}]
");
            result.Should().HaveDiagnostics(new[]
            {
                (UnusedVariableRule.Code, DiagnosticLevel.Warning, new UnusedVariableRule().GetMessage("foo")),
                ("BCP178", DiagnosticLevel.Error, "The for-expression must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of dnsZone are \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP178", DiagnosticLevel.Error, "The for-expression must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of storageAccounts are \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP178", DiagnosticLevel.Error, "The for-expression must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of aRecord are \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP182", DiagnosticLevel.Error, "The for-body of the variable \"foo\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of appPlan are \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP182", DiagnosticLevel.Error, "The for-body of the variable \"foo\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. You are referencing a variable which cannot be calculated in time (\"bar\" -> \"aRecord\"). Accessible properties of aRecord are \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP177", DiagnosticLevel.Error, "The if-condition expression must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of aRecord are \"apiVersion\", \"id\", \"name\", \"type\"."),
            });
        }

        [TestMethod]
        public void DtcValidation_RuntimeValueAsDtcPropertyKey_ProducesDiagnostics()
        {
            var result = CompilationHelper.Compile(@"
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: 'global'
}

resource appPlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'appPlan'
  location: resourceGroup().location
  tags: {
    '${listKeys('storage', '2020-01-01')}': 'value'
    '${dnsZone.etag}': 'value'
  }
  sku: {
    name: 'F1'
    capacity: 1
  }
}
");
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP120", DiagnosticLevel.Error, "The property \"tags\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated."),
                ("BCP120", DiagnosticLevel.Error, "The property \"tags\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of dnsZone are \"apiVersion\", \"id\", \"name\", \"type\"."),
            });
        }
    }
}
