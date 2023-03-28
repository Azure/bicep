// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
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
        public void DtcValidation_RuntimeValue_ForBodyExpression_Resource_ProducesDiagnostics()
        {
            const string text = @"
resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'foo'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

var ok1 = [for i in range(0, 2): foo.id]
var ok2 = [for i in range(0, 2): {
  id: foo.id
}]
var bad1 = [for i in range(0, 2): foo.properties]
var bad2 = [for i in range(0, 2): foo.properties.accessTier]
var bad3 = [for i in range(0, 2): {
  accessTier: foo.properties.accessTier
}]
";
            var result = CompilationHelper.Compile(text);

            const int badCount = 3;
            var expectedDiagnostics = Enumerable
                .Range(1, badCount)
                .Select(i => ("BCP182", DiagnosticLevel.Error, $"This expression is being used in the for-body of the variable \"bad{i}\", which requires values that can be calculated at the start of the deployment. Properties of foo which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."));

            result.WithFilteredDiagnostics(d => d.Level == DiagnosticLevel.Error).Should().HaveDiagnostics(expectedDiagnostics);
        }

        [TestMethod]
        public void DtcValidation_RuntimeValue_ForBodyExpression_ResourceCollection_ProducesDiagnostics()
        {
            StringBuilder textSb = new(@"
resource foos 'Microsoft.Storage/storageAccounts@2022-09-01' = [for i in range(0, 2): {
  name: 'foo-${i}'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}]
");

            var arrayAccessors = new[] { "0", "i", "i + 2" };

            // iterate the ok cases
            var okNumber = 0;
            var okCollectionAccessExpressions = new[] { ".id" };

            foreach (var arrayAccessor in arrayAccessors)
            {
                foreach (var okExp in okCollectionAccessExpressions)
                {
                    textSb.AppendLine($"var ok{++okNumber} = [for i in range(0, 2): foos[{arrayAccessor}]{okExp}]");
                    textSb.AppendLine($@"var ok{++okNumber} = [for i in range(0, 2): {{
  name: foos[{arrayAccessor}]{okExp}
}}]");
                }
            }
            okNumber.Should().Be(arrayAccessors.Length * okCollectionAccessExpressions.Length * 2);

            // iterate the bad cases
            var badNumber = 0;
            var badCollectionAccessExpressions = new[] { ".properties", ".properties.accessTier" };

            foreach (var arrayAccessor in arrayAccessors)
            {
                foreach (var badExp in badCollectionAccessExpressions)
                {
                    textSb.AppendLine($"var bad{++badNumber} = [for i in range(0, 2): foos[{arrayAccessor}]{badExp}]");
                    textSb.AppendLine($@"var bad{++badNumber} = [for i in range(0, 2): {{
  name: foos[{arrayAccessor}]{badExp}
}}]");
                }
            }
            badNumber.Should().Be(arrayAccessors.Length * badCollectionAccessExpressions.Length * 2);

            var finalText = textSb.ToString();
            var result = CompilationHelper.Compile(finalText);

            var expectedDiagnostics = Enumerable
                .Range(1, badNumber)
                .Select(i => ("BCP182", DiagnosticLevel.Error, $"This expression is being used in the for-body of the variable \"bad{i}\", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."));

            result.WithFilteredDiagnostics(d => d.Level == DiagnosticLevel.Error).Should().HaveDiagnostics(expectedDiagnostics);
        }
    }
}
