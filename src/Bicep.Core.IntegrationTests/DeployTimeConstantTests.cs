// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
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

resource vnet 'Microsoft.Network/virtualNetworks@2022-07-01' existing = {
  name: uniqueString(appPlan.properties.workerTierName)

  resource subnet0 'subnets' = {
    name: uniqueString(appPlan.properties.workerTierName)
    properties: {
      addressPrefix: '10.0.2.0/24'
    }
  }

  resource subnet1 'subnets' existing = {
    name: uniqueString(appPlan.properties.workerTierName)
  }
}

resource subnet2 'Microsoft.Network/virtualNetworks/subnets@2022-07-01' = {
  name: 'subnet2'
  parent: vnet
  properties: {
    addressPrefix: '10.0.2.0/24'
  }
}

resource diag 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview'  = {
  name: 'diag'
  scope: vnet::subnet1
}
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP178", DiagnosticLevel.Error, "This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. Properties of dnsZone which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP178", DiagnosticLevel.Error, "This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. Properties of storageAccounts which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP178", DiagnosticLevel.Error, "This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. Properties of aRecord which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP182", DiagnosticLevel.Error, "This expression is being used in the for-body of the variable \"foo\", which requires values that can be calculated at the start of the deployment. Properties of appPlan which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP182", DiagnosticLevel.Error, "This expression is being used in the for-body of the variable \"foo\", which requires values that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start (\"bar\" -> \"aRecord\"). Properties of aRecord which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP177", DiagnosticLevel.Error, "This expression is being used in the if-condition expression, which requires a value that can be calculated at the start of the deployment. Properties of aRecord which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP120", DiagnosticLevel.Error, @"This expression is being used in an assignment to the ""name"" property of the ""Microsoft.Network/virtualNetworks/subnets"" type, which requires a value that can be calculated at the start of the deployment. Properties of appPlan which can be calculated at the start include ""apiVersion"", ""id"", ""name"", ""type""."),
                ("BCP120", DiagnosticLevel.Error, @"This expression is being used in an assignment to the ""parent"" property of the ""Microsoft.Network/virtualNetworks/subnets"" type, which requires a value that can be calculated at the start of the deployment. Properties of vnet which can be calculated at the start include ""apiVersion"", ""id"", ""type""."),
                ("BCP120", DiagnosticLevel.Error, @"This expression is being used in an assignment to the ""scope"" property of the ""Microsoft.Insights/diagnosticSettings"" type, which requires a value that can be calculated at the start of the deployment. Properties of subnet1 which can be calculated at the start include ""apiVersion"", ""id"", ""type""."),
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

        [TestMethod]
        public void DtcValidation_RuntimeValue_ForBodyExpression_ProducesDiagnostics()
        {
            StringBuilder textSb = new(@"
resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'foo'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}
resource foos 'Microsoft.Storage/storageAccounts@2022-09-01' = [for i in range(0, 2): {
  name: 'foo-${i}'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}]
param strParam string = 'id'
var zeroIndex = 0
var otherIndex = zeroIndex + 2
var idAccessor = 'id'
var idAccessor2 = idAccessor
var idAccessorInterpolated = '${idAccessor}'
var propertiesAccessor = 'properties'
var accessTierAccessor = 'accessTier'
");

            var arrayAccessorExps = new[] { "0", "i", "i + 2", "zeroIndex", "otherIndex" };

            // iterate the ok cases
            var okCase = 0;
            var okAccessExps = new[] { ".id", "['id']", "[idAccessor]", "[idAccessor2]" };

            foreach (var okAccessExp in okAccessExps)
            {
                textSb.AppendLine($"var ok{++okCase} = [for i in range(0, 2): foo{okAccessExp}]");
                textSb.AppendLine($@"var ok{++okCase} = [for i in range(0, 2): {{
                  prop: foo{okAccessExp}
                }}]");

                foreach (var arrAccessorExp in arrayAccessorExps)
                {
                    textSb.AppendLine($"var ok{++okCase} = [for i in range(0, 2): foos[{arrAccessorExp}]{okAccessExp}]");
                    textSb.AppendLine($@"var ok{++okCase} = [for i in range(0, 2): {{
  prop: foos[{arrAccessorExp}]{okAccessExp}
}}]");
                }
            }
            okCase.Should().Be(okAccessExps.Length * 2 + arrayAccessorExps.Length * okAccessExps.Length * 2);

            // iterate the bad cases
            var badCase = 0;
            var badAccessExps = new[]
            {
                ".properties", ".properties.accessTier",
                "['properties']", "['properties']['accessTier']",
                "[propertiesAccessor]", "[propertiesAccessor][accessTierAccessor]",
                "[strParam]",
                "[idAccessorInterpolated]"
            };

            var expectedDiagnostics = new List<(string, DiagnosticLevel, string)>();
            void AddExpectedDtcDiagnostic(int badVariableNumber, string variableName)
            {
                expectedDiagnostics.Add(("BCP182", DiagnosticLevel.Error, $"This expression is being used in the for-body of the variable \"bad{badVariableNumber}\", which requires values that can be calculated at the start of the deployment. Properties of {variableName} which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."));
            }

            foreach (var badAccessExp in badAccessExps)
            {
                textSb.AppendLine($"var bad{++badCase} = [for i in range(0, 2): foo{badAccessExp}]");
                AddExpectedDtcDiagnostic(badCase, "foo");
                textSb.AppendLine($@"var bad{++badCase} = [for i in range(0, 2): {{
                  prop: foo{badAccessExp}
                }}]");
                AddExpectedDtcDiagnostic(badCase, "foo");

                foreach (var arrAccessorExp in arrayAccessorExps)
                {
                    textSb.AppendLine($"var bad{++badCase} = [for i in range(0, 2): foos[{arrAccessorExp}]{badAccessExp}]");
                    AddExpectedDtcDiagnostic(badCase, "foos");
                    textSb.AppendLine($@"var bad{++badCase} = [for i in range(0, 2): {{
  prop: foos[{arrAccessorExp}]{badAccessExp}
}}]");
                    AddExpectedDtcDiagnostic(badCase, "foos");
                }
            }
            expectedDiagnostics.Should().HaveCount(badAccessExps.Length * 2 + arrayAccessorExps.Length * badAccessExps.Length * 2);

            var finalText = textSb.ToString();
            var result = CompilationHelper.Compile(finalText);

            var filteredDiagnostics = result.WithFilteredDiagnostics(d => d.Level == DiagnosticLevel.Error);
            filteredDiagnostics.Should().HaveDiagnostics(expectedDiagnostics);
        }
    }
}
