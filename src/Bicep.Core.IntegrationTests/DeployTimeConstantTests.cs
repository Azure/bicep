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
  }
}]
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP178", DiagnosticLevel.Error, "This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. Properties of dnsZone which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP178", DiagnosticLevel.Error, "This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. Properties of storageAccounts which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP178", DiagnosticLevel.Error, "This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. Properties of aRecord which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP182", DiagnosticLevel.Error, "This expression is being used in the for-body of the variable \"foo\", which requires values that can be calculated at the start of the deployment. Properties of appPlan which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP182", DiagnosticLevel.Error, "This expression is being used in the for-body of the variable \"foo\", which requires values that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start (\"bar\" -> \"aRecord\"). Properties of aRecord which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP177", DiagnosticLevel.Error, "This expression is being used in the if-condition expression, which requires a value that can be calculated at the start of the deployment. Properties of aRecord which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"tags\" property of the \"Microsoft.Web/serverfarms\" type, which requires a value that can be calculated at the start of the deployment."),
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"tags\" property of the \"Microsoft.Web/serverfarms\" type, which requires a value that can be calculated at the start of the deployment. Properties of dnsZone which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
            });
        }
    }
}
