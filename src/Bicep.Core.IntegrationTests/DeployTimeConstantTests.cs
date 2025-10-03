// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class DeployTimeConstantTests
    {
        private static ServiceBuilder Services => new();

        private static string GetDtcValidationResourceBaseline()
        {
            return @"
resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'foo'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'

  resource fooChild 'fileServices' = {
    name: 'default'
  }
}
resource foos 'Microsoft.Storage/storageAccounts@2022-09-01' = [for i in range(0, 2): {
  name: 'foo-${i}'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}]
resource existingFoo 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'existingFoo'
}
";
        }

        [TestMethod]
        public void DtcValidation_EntireResourceOrModuleAccessAtInvalidLocations_ProducesDiagnostics()
        {
            var result = CompilationHelper.Compile(Services.WithFeatureOverrides(new(ResourceTypedParamsAndOutputsEnabled: true)), @"
param ident resource 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30'

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
  name: uniqueString(appPlan!.properties.workerTierName)

  resource subnet0 'subnets' = {
    name: uniqueString(appPlan!.properties.workerTierName)
    properties: {
      addressPrefix: '10.0.2.0/24'
    }
  }

  resource subnet1 'subnets' existing = {
    name: uniqueString(appPlan!.properties.workerTierName)
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

resource assignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: ident.properties.principalId
  properties: {
    roleDefinitionId: 'a'
    principalId: 'a'
  }
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
                ("BCP120", DiagnosticLevel.Error, @"This expression is being used in an assignment to the ""parent"" property of the ""Microsoft.Network/virtualNetworks/subnets"" type, which requires a value that can be calculated at the start of the deployment. Properties of vnet which can be calculated at the start include ""apiVersion"", ""type""."),
                ("BCP120", DiagnosticLevel.Error, @"This expression is being used in an assignment to the ""scope"" property of the ""Microsoft.Insights/diagnosticSettings"" type, which requires a value that can be calculated at the start of the deployment. Properties of subnet1 which can be calculated at the start include ""apiVersion"", ""type""."),
                ("BCP120", DiagnosticLevel.Error, @"This expression is being used in an assignment to the ""name"" property of the ""Microsoft.Authorization/roleAssignments"" type, which requires a value that can be calculated at the start of the deployment. Properties of ident which can be calculated at the start include ""apiVersion"", ""id"", ""name"", ""type""."),
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
        public void DtcValidation_VarForBodyExpression_Ok()
        {
            StringBuilder textSb = new(GetDtcValidationResourceBaseline());
            textSb.Append(
                @"
param strParam string = 'id'
var idAccessor = 'id'
var strArray = ['id', 'properties']
var indirect = {
  prop: foo.id
}
"
            );

            textSb.AppendLine(
                @"
var okVarForBody1 = [for i in range(0, 2): foo.id]
var okVarForBody2 = [for i in range(0, 2): foo['id']]
var okVarForBody3 = [for i in range(0, 2): {
  prop: foo[idAccessor]
}]
var okVarForBody4 = [for i in range(0, 2): foo[strArray[0]]]
var okVarForBody5 = [for i in range(0, 2): foos[0].id]
var okVarForBody6 = [for i in range(0, 2): foos[i].name]
var okVarForBody7 = [for i in range(0, 2): foos[i]['name']]
var okVarForBody8 = [for i in range(0, 2): {
  '${foos[i].name}': foo.id
}]
var okVarForBody9 = [for i in range(0, 2): foos[i + 2]['${'name'}']]
var okVarForBody10 = [for i in range(0, 2): indirect.prop]
"
            );

            var finalText = textSb.ToString();
            var result = CompilationHelper.Compile(finalText);

            var filteredDiagnostics = result.WithFilteredDiagnostics(d => d.IsError());
            filteredDiagnostics.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void DtcValidation_RuntimeValue_VarForBodyExpression_ProducesDiagnostics()
        {
            StringBuilder textSb = new(GetDtcValidationResourceBaseline());
            textSb.Append(
                @"
param cond bool = false
param strParam string = 'id'
var propertiesAccessor = 'properties'
var strArray = ['id', 'properties']
var indirect = {
  prop: foo.properties
}
"
            );

            var expectedDiagnostics = new List<(string, DiagnosticLevel, string)>();
            void AddExpectedDtcDiagnostic(string varNameOfForBody, string resourceVarName, string? violatingPropertyName = null)
            {
                expectedDiagnostics.Add(("BCP182", DiagnosticLevel.Error, $"This expression is being used in the for-body of the variable \"{varNameOfForBody}\", which requires values that can be calculated at the start of the deployment.{(violatingPropertyName != null ? $" The property \"{violatingPropertyName}\" of {resourceVarName} cannot be calculated at the start." : "")} Properties of {resourceVarName} which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."));
            }

            void AddExpectedIndirectDtcDiagnostic(string varNameOfForBody, string resourceVarName, string dtcVariablePath, string? violatingPropertyName)
            {
                expectedDiagnostics.Add(("BCP182", DiagnosticLevel.Error, $"This expression is being used in the for-body of the variable \"{varNameOfForBody}\", which requires values that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start {dtcVariablePath}.{(violatingPropertyName != null ? $" The property \"{violatingPropertyName}\" of {resourceVarName} cannot be calculated at the start." : "")} Properties of {resourceVarName} which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."));
            }

            textSb.AppendLine("var badVarForBody1 = [for i in range(0, 2): foo.properties]");
            AddExpectedDtcDiagnostic("badVarForBody1", "foo", "properties");

            textSb.AppendLine("var badVarForBody2 = [for i in range(0, 2): foo['properties']]");
            AddExpectedDtcDiagnostic("badVarForBody2", "foo", "properties");

            textSb.AppendLine("var badVarForBody3 = [for i in range(0, 2): { prop: foo[propertiesAccessor].accessTier }]");
            AddExpectedDtcDiagnostic("badVarForBody3", "foo", "properties");

            textSb.AppendLine("var badVarForBody4 = [for i in range(0, 2): foo::fooChild.properties]");
            AddExpectedDtcDiagnostic("badVarForBody4", "fooChild", "properties");

            textSb.AppendLine("var badVarForBody5 = [for i in range(0, 2): foo[strParam]]");
            AddExpectedDtcDiagnostic("badVarForBody5", "foo");

            textSb.AppendLine("var badVarForBody6 = [for i in range(0, 2): foos[i].properties]");
            AddExpectedDtcDiagnostic("badVarForBody6", "foos", "properties");

            textSb.AppendLine("var badVarForBody7 = [for i in range(0, 2): foos[i + 2]['${'properties'}']]");
            AddExpectedDtcDiagnostic("badVarForBody7", "foos", "properties");

            textSb.AppendLine("var badVarForBody8 = [for i in range(0, 2): foo[strArray[1]]]");
            AddExpectedDtcDiagnostic("badVarForBody8", "foo", "properties");

            textSb.AppendLine("var badVarForBody9 = [for i in range(0, 2): indirect.prop]");
            AddExpectedIndirectDtcDiagnostic("badVarForBody9", "foo", "(\"indirect\" -> \"foo\")", "properties");

            textSb.AppendLine(@"var badVarForBody10 = [for i in range(0, 2): {
  '${foos[i].properties.accessTier}': true
}]");
            AddExpectedDtcDiagnostic("badVarForBody10", "foos", "properties");

            textSb.AppendLine("var badVarForBody11 = [for i in range(0, 2): foo[cond ? 'properties' : 'extendedLocation']]");
            AddExpectedDtcDiagnostic("badVarForBody11", "foo", "extendedLocation");
            AddExpectedDtcDiagnostic("badVarForBody11", "foo", "properties");

            var finalText = textSb.ToString();
            var result = CompilationHelper.Compile(finalText);

            var filteredDiagnostics = result.WithFilteredDiagnostics(d => d.IsError());
            filteredDiagnostics.Should().HaveDiagnostics(expectedDiagnostics);
        }

        [TestMethod]
        public void DtcValidation_DirectResourceAccessInTopLevelProperties_NotAllowed()
        {
            var result = CompilationHelper.Compile("""
                param location string

                resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
                  name: 'myIdentity'
                  location: location
                }

                resource storage 'Microsoft.Storage/storageAccounts@2022-09-01' = {
                  name: 'myStorage'
                  kind: 'StorageV2'
                  location: userAssignedIdentity
                  sku: {
                    name: 'Standard_LRS'
                  }
                  identity: {
                    type: 'UserAssigned'
                    userAssignedIdentities: userAssignedIdentity
                  }
                  properties: {
                    accessTier: 'Hot'
                  }
                }
                """);

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP036", DiagnosticLevel.Warning, @"The property ""location"" expected a value of type ""string"" but the provided value is of type ""Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31"". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."),
                ("BCP120", DiagnosticLevel.Error, @"This expression is being used in an assignment to the ""location"" property of the ""Microsoft.Storage/storageAccounts"" type, which requires a value that can be calculated at the start of the deployment. Properties of userAssignedIdentity which can be calculated at the start include ""apiVersion"", ""id"", ""name"", ""type""."),
                ("BCP120", DiagnosticLevel.Error, @"This expression is being used in an assignment to the ""identity"" property of the ""Microsoft.Storage/storageAccounts"" type, which requires a value that can be calculated at the start of the deployment. Properties of userAssignedIdentity which can be calculated at the start include ""apiVersion"", ""id"", ""name"", ""type""."),
            });
        }
    }
}
