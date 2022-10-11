// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Navigation;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

#pragma warning disable CA1825 // Avoid zero-length array allocations

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    public partial class UseRecentApiVersionRuleTests
    {
        [TestClass]
        public class InReferenceFunctions
        {
            private record ExpectedFunctionInfo(string FunctionCall, string? ResourceType, string? ApiVerion);
            private static readonly ApiVersionProvider apiVersionProvider;

            static InReferenceFunctions()
            {
                // Test with the linter using the fake resource types from FakeResourceTypes (to guard against failures due to Azure changes)
                // Note: The compiler does not know about these fake types, only the linter.
                apiVersionProvider = new ApiVersionProvider(BicepTestConstants.Features, BicepTestConstants.NamespaceProvider);
                apiVersionProvider.InjectTypeReferences(ResourceScope.ResourceGroup, FakeResourceTypes.GetFakeResourceTypeReferences(FakeResourceTypes.ResourceScopeTypes));
            }

            private static void TestGetFunctionCallInfo(string bicep, string expectedFunctionCall, string? expectedResourceType, string? expectedApiVerion)
            {
                ExpectedFunctionInfo typedExpected = new(expectedFunctionCall, expectedResourceType, expectedApiVerion);

                var result = CompilationHelper.Compile(
                    new ServiceBuilder().WithApiVersionProvider(apiVersionProvider),
                    bicep);
                using (new AssertionScope().WithFullSource(result.BicepFile))
                {
                    var actual = UseRecentApiVersionRule.GetFunctionCallInfos(result.Compilation.GetEntrypointSemanticModel());
                    actual.Should().HaveCount(1, "Expecting a single function call per test");
                    var typedActual = new ExpectedFunctionInfo(
                            actual.First().FunctionCallSyntax.ToText(),
                            actual.First().ResourceType,
                            actual.First().ApiVersion?.Formatted);

                    typedActual.Should().BeEquivalentTo(typedExpected);
                }
            }

            [DataRow(
                @"
                    output a object = reference()
                ",
                "reference()",
                null,
                null
            )]
            [DataRow(
                @"
                    output a object = reference('2417-12-01-preview')
                ",
                "reference('2417-12-01-preview')",
                null,
                null
            )]
            [DataRow(
                @"
                    output a object = reference('Fake.DBforMySQL/servers', '417-12-01-preview')
                ",
                "reference('Fake.DBforMySQL/servers', '417-12-01-preview')",
                "Fake.DBforMySQL/servers",
                null
            )]
            [DataRow(
                @"
                    output a object = reference('Fake.DBforMySQL/servers', '2417-12-01-preview')
                ",
                "reference('Fake.DBforMySQL/servers', '2417-12-01-preview')",
                "Fake.DBforMySQL/servers",
                "2417-12-01-preview"
            )]
            [DataRow(
                @"
                    output a object = reference('Fake.DBforMySQL-servers', '2417-12-01-preview')
                ",
                "reference('Fake.DBforMySQL-servers', '2417-12-01-preview')",
                null,
                "2417-12-01-preview"
            )]
            [DataRow(
                @"
                    param apiversion string
                    output a object = reference('Fake.DBforMySQL/servers', apiversion)
                ",
                "reference('Fake.DBforMySQL/servers', apiversion)",
                "Fake.DBforMySQL/servers",
                null
            )]
            [DataRow(
                @"
                    param apiversion string = '2022-01-01'
                    output a object = reference('Fake.DBforMySQL/servers', apiversion)
                ",
                "reference('Fake.DBforMySQL/servers', apiversion)",
                "Fake.DBforMySQL/servers",
                "2022-01-01"
            )]
            [DataRow(
                @"
                    var apiversion = '2022-01-01'
                    output a object = reference('Fake.DBforMySQL/servers', apiversion)
                ",
                "reference('Fake.DBforMySQL/servers', apiversion)",
                "Fake.DBforMySQL/servers",
                "2022-01-01"
            )]
            [DataRow(
                @"
                    param p string = '2022-01-01'
                    var apiversion = p
                    output a object = reference('Fake.DBforMySQL/servers', apiversion)
                ",
                "reference('Fake.DBforMySQL/servers', apiversion)",
                "Fake.DBforMySQL/servers",
                "2022-01-01"
            )]
            [DataRow(
                @"
                    param p2 string = 'Fake.DBforMySQL/servers'
                    var resType = p2
                    param p1 string = '2022-01-01'
                    var apiversion = p1
                    output a object = reference(resType, apiversion)
                ",
                "reference(resType, apiversion)",
                "Fake.DBforMySQL/servers",
                "2022-01-01"
            )]
            [DataRow(
                @"
                    param p2 string = 'Fake.DBforMySQL-servers'
                    var resType = p2
                    param p1 string = '2022-01-01'
                    var apiversion = p1
                    output a object = reference(resType, apiversion)
                ",
                "reference(resType, apiversion)",
                null, // not valid
                "2022-01-01"
            )]
            [DataRow(
                @"
                    output o1 string = reference('Fake.Resources/deployments/SettingUpVirtualNetwork', '2015-01-01').outputs.dbSubnetRef.value
                ",
                "reference('Fake.Resources/deployments/SettingUpVirtualNetwork', '2015-01-01')",
                "Fake.Resources/deployments",
                "2015-01-01"
            )]
            [DataRow(
                @"
                    output o1 string = reference(resourceId('Fake.Resources/deployments/SettingUpVirtualNetwork'), '2015-01-01').outputs.dbSubnetRef.value
                ",
                "reference(resourceId('Fake.Resources/deployments/SettingUpVirtualNetwork'), '2015-01-01')",
                "Fake.Resources/deployments",
                "2015-01-01"
            )]
            [DataTestMethod]
            public void GetFunctionCallInfo_Reference_NoResourceId(string bicep, string expectedFunctionCall, string? expectedResourceType, string? expectedApiVerion)
            {
                TestGetFunctionCallInfo(bicep, expectedFunctionCall, expectedResourceType, expectedApiVerion);
            }

            [DataRow(
                @"
                    var lbPublicIPName = 'lbPublicIPName'
                    output o string = reference(resourceId('Microsoft.Network/publicIPAddresses', lbPublicIPName),'2020-08-01').dnsSettings.fqdn
                ",
                "reference(resourceId('Microsoft.Network/publicIPAddresses', lbPublicIPName), '2020-08-01')",
                "Microsoft.Network/publicIPAddresses",
                "2020-08-01"
            )]
            [DataRow(
                @"
                    output a object = reference(resourceId('Fake.DBforMySQL/servers', 'test'))
                ",
                "reference(resourceId('Fake.DBforMySQL/servers', 'test'))",
                "Fake.DBforMySQL/servers",
                null
            )]
            [DataRow(
                @"
                    output a object = reference(resourceId('Fake.DBforMySQL/servers'), '2022-01-01')
                ",
                "reference(resourceId('Fake.DBforMySQL/servers'), '2022-01-01')",
                "Fake.DBforMySQL/servers",
                "2022-01-01"
            )]
            [DataRow(
                @"
                    output a object = reference(resourceId('FakeDBforMySQL/servers'), '2022-01-01')
                ",
                "reference(resourceId('FakeDBforMySQL/servers'), '2022-01-01')",
                null, // not valid
                "2022-01-01"
            )]
            [DataRow(
                @"
                    output a object = reference(resourceId('Fake.DBforMySQL'), '2022-01-01')
                ",
                "reference(resourceId('Fake.DBforMySQL'), '2022-01-01')",
                null, // not valid
                "2022-01-01"
            )]
            [DataRow(
                @"
                    output a object = reference(resourceId('Fake.DBforMySQL/servers/whatever'), '2022-01-01')
                ",
                "reference(resourceId('Fake.DBforMySQL/servers/whatever'), '2022-01-01')",
                "Fake.DBforMySQL/servers",
                "2022-01-01"
            )]
            [DataRow(
                @"
                    output output1 object = reference(resourceId('Fake.Network/virtualNetworks/subnets', 'a', 'b'), '2415-06-15')
                ",
                "reference(resourceId('Fake.Network/virtualNetworks/subnets', 'a', 'b'), '2415-06-15')",
                "Fake.Network/virtualNetworks/subnets",
                "2415-06-15"
            )]
            [DataTestMethod]
            public void GetFunctionCallInfo_Reference_ResourceId(string bicep, string expectedFunctionCall, string? expectedResourceType, string? expectedApiVerion)
            {
                TestGetFunctionCallInfo(bicep, expectedFunctionCall, expectedResourceType, expectedApiVerion);
            }

            [DataRow(
                @"
                    output output1 object = list(resourceId('Fake.Network/publicIPAddresses', 'test'), '2415-06-15')
                ",
                "list(resourceId('Fake.Network/publicIPAddresses', 'test'), '2415-06-15')",
                "Fake.Network/publicIPAddresses",
                "2415-06-15"
            )]
            [DataRow(
                @"
                    var listAccountSasRequestContent = {
                      signedServices: 'bfqt'
                      signedPermission: 'rwdlacup'
                      signedStart: '2021-06-02T00:00:00Z'
                      signedExpiry: '2030-10-30T00:00:00Z'
                      signedResourceTypes: 'sco'
                    }
                    output output1 object = listAccountSas(resourceId('Fake.Network/publicIPAddresses', 'test'), '2415-06-15', listAccountSasRequestContent)
                ",
                "listAccountSas(resourceId('Fake.Network/publicIPAddresses', 'test'), '2415-06-15', listAccountSasRequestContent)",
                "Fake.Network/publicIPAddresses",
                "2415-06-15"
            )]
            [DataTestMethod]
            public void GetFunctionCallInfo_List_ResourceId(string bicep, string expectedFunctionCall, string? expectedResourceType, string? expectedApiVerion)
            {
                TestGetFunctionCallInfo(bicep, expectedFunctionCall, expectedResourceType, expectedApiVerion);
            }

            [DataRow(
                @"
                    resource sqlServerName 'Fake.DBforMySQL/servers@2417-12-01-preview' = {
                      name: 'testSqlServer'
                    }

                    output output1 object = listkeys('testSqlServer', '2417-12-01-preview')
                ",
                "listkeys('testSqlServer', '2417-12-01-preview')",
                "Fake.DBforMySQL/servers",
                "2417-12-01-preview",
                DisplayName = "string literal"
            )]
            [DataRow(
                @"
                    resource sqlServer 'Fake.DBforMySQL/servers@2417-12-01-preview' = {
                      name: sqlServerName
                    }
                    resource sqlServer2 'Fake.DBforMySQL/servers@2417-12-01-preview' = {
                      name: sqlServerName2
                    }

                    output output1 object = listkeys(sqlServerName, '2417-12-01-preview')
                    var sqlServerName = '${sqlServerName_var}-hello'
                    var sqlServerName2 = '${sqlServerName_var}-hello there'
                ",
                "listkeys(sqlServerName, '2417-12-01-preview')",
                "Fake.DBforMySQL/servers",
                "2417-12-01-preview",
                DisplayName = "variable"
            )]
            [DataRow(
                @"
                    var sqlServerName_var = 'testSqlServerName'

                    resource sqlServerName 'Fake.DBforMySQL/servers@2417-12-01-preview' = {
                      name: sqlServerName_var
                    }

                    output output1 object = listkeys('testSqlServerName', '2417-12-01-preview')
                ",
                "listkeys('testSqlServerName', '2417-12-01-preview')",
                "Fake.DBforMySQL/servers",
                "2417-12-01-preview",
                DisplayName = "string literal vs variable with same value"
            )]
            [DataRow(
                @"
                    var sqlServerName_var = 'testSqlServerName'

                    resource sqlServerName 'Fake.DBforMySQL/servers@2417-12-01-preview' = {
                      name: 'testSqlServerName'
                    }

                    output output1 object = listkeys(sqlServerName_var, '2417-12-01-preview')
                ",
                "listkeys(sqlServerName_var, '2417-12-01-preview')",
                "Fake.DBforMySQL/servers",
                "2417-12-01-preview",
                DisplayName = "variable vs string literal with same value"
            )]
            [DataRow(
                @"
                    var sqlServerName_var = 'testSqlServerName'

                    resource sqlServerName 'Fake.DBforMySQL/servers@2417-12-01-preview' = {
                      name: '${sqlServerName_var}-hello'
                    }

                    output output1 object = listkeys('${sqlServerName_var}-hello', '2417-12-01-preview')
                ",
                "listkeys('${sqlServerName_var}-hello', '2417-12-01-preview')",
                "Fake.DBforMySQL/servers",
                "2417-12-01-preview",
                DisplayName = "string expression"
            )]
            [DataTestMethod]
            public void GetFunctionCallInfo_UsingNameOfResource(string bicep, string expectedFunctionCall, string? expectedResourceType, string? expectedApiVerion)
            {
                TestGetFunctionCallInfo(bicep, expectedFunctionCall, expectedResourceType, expectedApiVerion);
            }

            [DataRow(
                @"
                    param location string = resourceGroup().location
                    var userAssignedIdentityName_var = 'msi'
                    resource userAssignedIdentityName 'Fake.ManagedIdentity/userAssignedIdentities@2415-08-31-PREVIEW' = {
                      name: userAssignedIdentityName_var
                      location: location
                    }
                    output foo string = reference(userAssignedIdentityName.id, '2415-08-31-PREVIEW').principalId
                ",
                "reference(userAssignedIdentityName.id, '2415-08-31-PREVIEW')",
                "Fake.ManagedIdentity/userAssignedIdentities",
                "2415-08-31-preview",
                DisplayName = "ApiVersion is in call"
            )]
            [DataRow(
                @"
                    param location string = resourceGroup().location
                    var userAssignedIdentityName_var = 'msi'
                    resource userAssignedIdentityName 'Fake.ManagedIdentity/userAssignedIdentities@2415-08-31-PREVIEW' = {
                      name: userAssignedIdentityName_var
                      location: location
                    }
                    output foo string = reference(userAssignedIdentityName.id).principalId
                ",
                "reference(userAssignedIdentityName.id)",
                "Fake.ManagedIdentity/userAssignedIdentities",
                null,
                DisplayName = "ApiVersion not in call"
            )]
            // CONSIDER: Not currently supported: fully replacing variables in the expression (userAssignedIdentityNameId -> userAssignedIdentityName.id) when evaluating reference()
            //[DataRow(
            //    @"
            //        param location string = resourceGroup().location
            //        var userAssignedIdentityName_var = 'msi'
            //        var userAssignedIdentityNameId = userAssignedIdentityName.id
            //        resource userAssignedIdentityName 'Fake.ManagedIdentity/userAssignedIdentities@2415-08-31-PREVIEW' = {
            //          name: userAssignedIdentityName_var
            //          location: location
            //        }
            //        output foo string = reference(userAssignedIdentityNameId, '2415-08-31-preview').principalId
            //    ",
            //    "reference(userAssignedIdentityName.id, '2415-08-31-preview')",
            //    "Fake.ManagedIdentity/userAssignedIdentities",
            //    "2415-08-31-preview",
            //    DisplayName = "Symbolic reference id through variable"
            //)]
            [DataTestMethod]
            public void GetFunctionCallInfo_UsingResourceSymbolicReferenceId(string bicep, string expectedFunctionCall, string? expectedResourceType, string? expectedApiVerion)
            {
                TestGetFunctionCallInfo(bicep, expectedFunctionCall, expectedResourceType, expectedApiVerion);
            }

            [DataRow(
                @"
                    output o1 string = reference(resourceId('Fake.Resources/deployments/SettingUpVirtualNetwork', 'a/b/c'), '2415-01-01').outputs.dbSubnetRef.value
                ",
                "reference(resourceId('Fake.Resources/deployments/SettingUpVirtualNetwork', 'a/b/c'), '2415-01-01')",
                "Fake.Resources/deployments",
                "2415-01-01"
            )]
            [DataRow(
                @"
                    output o1 string = reference(resourceId('ffffffff-ffff-ffff-ffff-ffffffffffff', 'Fake.Resources/deployments', 'SettingUpVirtualNetwork', 'a/b/c'), '2415-01-01').outputs.dbSubnetRef.value
                ",
                "reference(resourceId('ffffffff-ffff-ffff-ffff-ffffffffffff', 'Fake.Resources/deployments', 'SettingUpVirtualNetwork', 'a/b/c'), '2415-01-01')",
                "Fake.Resources/deployments",
                "2415-01-01"
            )]
            [DataRow(
                @"
                    output o2 string = reference(resourceId('Fake.Compute/virtualMachineScaleSets', 'virtualMachines/runCommands', 'a/b/c'), '2420-06-01').outputs.dbSubnetRef.value
                ",
                "reference(resourceId('Fake.Compute/virtualMachineScaleSets', 'virtualMachines/runCommands', 'a/b/c'), '2420-06-01')",
                "Fake.Compute/virtualMachineScaleSets/virtualMachines/runCommands",
                "2420-06-01"
            )]
            [DataRow(
                @"
                    var runCommands1 = 'Fake.Compute/virtualMachineScaleSets'
                    var runCommands2 = 'virtualMachines/runCommands/z'
                    output o3 string = reference(resourceId(runCommands1, runCommands2, 'a/b/c'), '2420-06-01').outputs.dbSubnetRef.value
                ",
                "reference(resourceId(runCommands1, runCommands2, 'a/b/c'), '2420-06-01')",
                "Fake.Compute/virtualMachineScaleSets/virtualMachines/runCommands",
                "2420-06-01"
            )]
            [DataTestMethod]
            public void GetFunctionCallInfo_PartsBeyondResourceType(string bicep, string expectedFunctionCall, string? expectedResourceType, string? expectedApiVerion)
            {
                TestGetFunctionCallInfo(bicep, expectedFunctionCall, expectedResourceType, expectedApiVerion);
            }

            [DataRow(
                @"
                    var runCommands1 = 'Fake.Compute/virtualMachineScaleSets'
                    var runCommands2 = 'virtualMachines/runCommands/z'
                    output o3 string = reference(resourceId('ffffffff-ffff-ffff-ffff-ffffffffffff', runCommands1, runCommands2, 'a/b/c'), '2420-06-01').outputs.dbSubnetRef.value
                ",
                "reference(resourceId('ffffffff-ffff-ffff-ffff-ffffffffffff', runCommands1, runCommands2, 'a/b/c'), '2420-06-01')",
                "Fake.Compute/virtualMachineScaleSets/virtualMachines/runCommands",
                "2420-06-01"
            )]
            [DataRow(
                @"
                    var runCommands1 = 'Fake.Compute/virtualMachineScaleSets'
                    var runCommands2 = 'virtualMachines/runCommands/z'
                    output o3 string = reference(resourceId('ffffffff-ffff-ffff-ffff-ffffffffffff', resourceGroup().id, runCommands1, runCommands2, 'a/b/c'), '2420-06-01').outputs.dbSubnetRef.value
                ",
                "reference(resourceId('ffffffff-ffff-ffff-ffff-ffffffffffff', resourceGroup().id, runCommands1, runCommands2, 'a/b/c'), '2420-06-01')",
                "Fake.Compute/virtualMachineScaleSets/virtualMachines/runCommands",
                "2420-06-01"
            )]
            [DataRow(
                @"
                    var runCommands1 = 'Fake.Compute/virtualMachineScaleSets'
                    var runCommands2 = 'virtualMachines/runCommands/z'
                    var array = []
                    output o3 string = reference(resourceId(array, 'ffffffff-ffff-ffff-ffff-ffffffffffff', runCommands1, runCommands2, 'a/b/c'), '2420-06-01').outputs.dbSubnetRef.value
                ",
                "reference(resourceId(array, 'ffffffff-ffff-ffff-ffff-ffffffffffff', runCommands1, runCommands2, 'a/b/c'), '2420-06-01')",
                "Fake.Compute/virtualMachineScaleSets/virtualMachines/runCommands",
                "2420-06-01"
            )]
            [DataRow(
                @"
                    var runCommands1 = 'Fake.Compute/virtualMachineScaleSets'
                    var runCommands2 = 'virtualMachines/runCommands/z'
                    var array = []
                    output o3 string = reference(resourceId(array, 'ffffffff-ffff-ffff-ffff-ffffffffffff', runCommands1, array, runCommands2, 'a/b/c'), '2420-06-01').outputs.dbSubnetRef.value
                ",
                "reference(resourceId(array, 'ffffffff-ffff-ffff-ffff-ffffffffffff', runCommands1, array, runCommands2, 'a/b/c'), '2420-06-01')",
                "Fake.Compute/virtualMachineScaleSets",
                "2420-06-01"
            )]
            [DataRow(
                @"
                    var runCommands1 = 'Fake.Compute/virtualMachineScaleSets'
                    var runCommands2 = 'virtualMachines/runCommands/z'
                    var array = []
                    output o3 string = reference(resourceId(array, 'ffffffff-ffff-ffff-ffff-ffffffffffff', runCommands1, , runCommands2, 'a/b/c'), '2420-06-01').outputs.dbSubnetRef.value
                ",
                "reference(resourceId(array, 'ffffffff-ffff-ffff-ffff-ffffffffffff', runCommands1, , runCommands2, 'a/b/c'), '2420-06-01')",
                "Fake.Compute/virtualMachineScaleSets",
                "2420-06-01"
            )]
            [DataRow(
                @"
                    var runCommands1 = 'Fake.Compute/virtualMachineScaleSets'
                    var runCommands2 = 'virtualMachines/runCommands/z'
                    var array = []
                    output o3 string = reference(resourceId(, 'ffffffff-ffff-ffff-ffff-ffffffffffff', runCommands1, , runCommands2, 'a/b/c'), '2420-06-01').outputs.dbSubnetRef.value
                ",
                "reference(resourceId(, 'ffffffff-ffff-ffff-ffff-ffffffffffff', runCommands1, , runCommands2, 'a/b/c'), '2420-06-01')",
                "Fake.Compute/virtualMachineScaleSets",
                "2420-06-01"
            )]
            [DataTestMethod]
            public void GetFunctionCallInfo_OptionalSubscriptionIdResourceIdArguments(string bicep, string expectedFunctionCall, string? expectedResourceType, string? expectedApiVerion)
            {
                TestGetFunctionCallInfo(bicep, expectedFunctionCall, expectedResourceType, expectedApiVerion);
            }

            [TestMethod]
            // https://raw.githubusercontent.com/Azure/arm-ttk/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Pass/ApiInPolicyRuleTemplate.json
            public void ArmTtk_ApiInPolicyRuleTemplate_Pass()
            {
                string bicep = @"
                    targetScope = 'managementGroup'
                    resource diagnostics_aa_deploy_policy 'Microsoft.Authorization/policyDefinitions@2021-06-01' = {
                      name: 'diagnostics-aa-deploy-policy'
                      properties: {
                        parameters: {
                          profileName: {
                            type: 'string'
                            metadata: {
                              displayName: 'Profile name'
                              description: ' '
                            }
                            defaultValue: 'setbypolicy_logAnalytics'
                          }
                          logAnalytics: {
                            type: 'string'
                            metadata: {
                              displayName: ' '
                              description: ' '
                              strongType: ''
                              assignPermissions: true
                            }
                          }
                        }
                        policyType: 'Custom'
                        policyRule: {
                          if: {
                            field: 'type'
                            equals: 'Microsoft.Automation/automationAccounts'
                          }
                          then: {
                            effect: 'deployIfNotExists'
                            details: {
                              type: 'Microsoft.Insights/diagnosticSettings'
                              roleDefinitionIds: [
                                '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
                              ]
                              existenceCondition: {
                                allOf: [
                                  {
                                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                                    equals: 'True'
                                  }
                                  {
                                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                                    equals: 'True'
                                  }
                                  {
                                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                                    matchInsensitively: '[parameters(\'logAnalytics\')]'
                                  }
                                ]
                              }
                              deployment: {
                                properties: {
                                  mode: 'incremental'
                                  template: {
                                    '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                                    contentVersion: '1.0.0.0'
                                    parameters: {
                                      profileName: {
                                        type: 'string'
                                      }
                                      logAnalytics: {
                                        type: 'string'
                                      }
                                      resourceName: {
                                        type: 'string'
                                      }
                                    }
                                    resources: [
                                      {
                                        name: '[parameters(\'profileName\')]'
                                        type: 'Microsoft.Insights/diagnosticSettings'
                                        apiVersion: '2021-05-01-preview'
                                        scope: '[resourceId(\'Microsoft.Automation/automationAccounts\', parameters(\'resourceName\'))]'
                                        properties: {
                                          workspaceId: '[parameters(\'logAnalytics\')]'
                                          metrics: [
                                            {
                                              category: 'AllMetrics'
                                              enabled: true
                                              retentionPolicy: {
                                                enabled: false
                                                days: 0
                                              }
                                            }
                                          ]
                                          logs: [
                                            {
                                              category: 'JobLogs'
                                              enabled: true
                                            }
                                            {
                                              category: 'JobStreams'
                                              enabled: true
                                            }
                                            {
                                              category: 'DscNodeStatus'
                                              enabled: true
                                            }
                                          ]
                                        }
                                      }
                                    ]
                                  }
                                  parameters: {
                                    profileName: {
                                      value: '[parameters(\'profileName\')]'
                                    }
                                    logAnalytics: {
                                      value: '[parameters(\'logAnalytics\')]'
                                    }
                                    resourceName: {
                                      value: '[field(\'name\')]'
                                    }
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] { });
            }

            [TestMethod]
            // https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Pass/NoApiVersion.json
            public void ArmTtk_NoApiVersion_Ignored()
            {
                string bicep = @"
                output a object = reference(resourceId('Fake.DBforMySQL/servers', 'test'))
            ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        // pass
                    });
            }

            [TestMethod]
            public void ReferenceFunction_NoArgs_Ignored()
            {
                string bicep = @"
                    output a object = reference()
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        // Compiler error
                        "[2] Expected 1 to 3 arguments, but got 0."
                    });
            }

            [TestMethod]
            public void ReferenceFunction_IgnoreApiVersionInFirstArg_Ignored()
            {
                string bicep = @"
                    output a object = reference('2417-12-01-preview')
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                    });
            }

            [TestMethod]
            public void ReferenceFunction_InvalidApiVersion_Ignored()
            {
                string bicep = @"
                    output a object = reference('Fake.DBforMySQL/servers', '417-12-01-preview')
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                    });
            }

            [TestMethod]
            public void ReferenceFunction_ApiVersionNotAStringLiteral_Ignored()
            {
                string bicep = @"
                    param apiversion string
                    output a object = reference('Fake.DBforMySQL/servers', apiversion)
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                    });
            }

            [TestMethod]
            public void ReferenceFunction_ResourceTypeAndApiVersionInParamDefault_RuleApplies()
            {
                string bicep = @"
                    param resourceType string = 'Fake.DBforMySQL/servers'
                    param apiversion string = '2000-01-01'
                    output a object = reference(resourceType, apiversion)
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                new string[] {
                    "[4] Could not find apiVersion 2000-01-01 for Fake.DBforMySQL/servers. Acceptable versions: 2417-12-01"
                });
            }

            [TestMethod]
            public void ReferenceFunction_ApiVersionInVariable_RuleApplies()
            {
                string bicep = @"
                    var apiversion = '2417-12-01-preview'
                    output a object = reference('Fake.DBforMySQL/servers', apiversion)
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        "[3] Use more recent API version for 'Fake.DBforMySQL/servers'. '2417-12-01-preview' is 1676 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2417-12-01",
                    });
            }

            [TestMethod]
            public void ReferenceFunction_ResourceTypeNotFound_Fail()
            {
                string bicep = @"
                    var apiversion = '2417-12-01-preview'
                    var resourceType = 'Fake.DBforMySQLOrWhatever/servers'
                    output a object = reference(resourceType, apiversion)
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                         "[4] Could not find resource type \"Fake.DBforMySQLOrWhatever/servers\". Did you mean \"Fake.DBForMySql/flexibleServers\"?",
                    });
            }

            [TestMethod]
            public void ReferenceFunction_ApiVersionNotFound_Fail()
            {
                string bicep = @"
                    var apiversion = '2417-12-02'
                    var resourceType = 'Fake.DBforMySQL/servers'
                    output a object = reference(resourceType, apiversion)
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        "[4] Could not find apiVersion 2417-12-02 for Fake.DBforMySQL/servers. Acceptable versions: 2417-12-01",
                    });
            }

            [TestMethod]
            public void ReferenceFunction_ApiVersionNotValid_Ignore()
            {
                string bicep = @"
                    var apiversion = '2417-12-01-preview-really'
                    var resourceType = 'Fake.DBforMySQL/servers'
                    output a object = reference(resourceType, apiversion)
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        // ignore (pass)
                    });
            }

            [TestMethod]
            public void ReferenceFunction_NoResourceType_Ignore()
            {
                string bicep = @"
                    output a object = reference(resourceId('Fake.DBforMySQL.servers', 'test'), '2001-01-01')
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        // ignore (pass)
                    });
            }

            [TestMethod]
            public void ReferenceFunction_NoApiVersion_Ignore()
            {
                string bicep = @"
                    output a object = reference(resourceId('Fake.DBforMySQL/servers', 'test'), '')
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        // ignore (pass)
                    });
            }

            [TestMethod]
            public void ApiVersionInOutput_RuleApplies()
            {
                string bicep = @"
                    output a object = reference(resourceId('Fake.DBforMySQL/servers', 'test'), '2417-12-01-preview')
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                new string[] {
                    "[2] Use more recent API version for 'Fake.DBforMySQL/servers'. '2417-12-01-preview' is 1676 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2417-12-01",
                });
            }

            [TestMethod]
            // https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Pass/latestStableApiOlderThan2Years.json
            public void ArmTtk_LatestStableApiOlderThan2Years_Pass()
            {
                string bicep = @"
                    output pass object = reference(resourceId('Fake.Web/connections', 'test'), '2416-06-01')
                    output fail object = reference(resourceId('Fake.Web/connections', 'test'), '2406-06-01')
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        "[3] Could not find apiVersion 2406-06-01 for Fake.Web/connections. Acceptable versions: 2416-06-01"
                    });
            }

            [TestMethod]
            // https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/CaseInsenstiveApiVersion.json
            public void ArmTtk_CaseInsensitiveApiVersion_Fail()
            {
                string bicep = @"
                    @description('The location in which the resources should be deployed.')
                    param location string = resourceGroup().location

                    var userAssignedIdentityName_var = 'msi'

                    resource userAssignedIdentityName 'Fake.ManagedIdentity/userAssignedIdentities@2415-08-31-PREVIEW' = {
                      name: userAssignedIdentityName_var
                      location: location
                    }

                    output foo string = reference(userAssignedIdentityName.id, '2415-08-31-PREVIEW').principalId
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                FakeResourceTypes.ResourceScopeTypes,
                "2422-07-04",
                    new string[] {
                        // TTK result:
                        //    [-] apiVersions Should Be Recent In Reference Functions(12 ms)
                        //        reference(resourceId('Microsoft.ManagedIdentity/userAssignedIdentities', variables('userAssignedIdentityName')), '2015-08-31-PREVIEW')  uses a preview version(2015 - 08 - 31 - PREVIEW) and there are more recent
                        //           versions available.Line: 15, Column: 24
                        //        Valid Api Versions Microsoft.ManagedIdentity / userAssignedIdentities :
                        //        2018 - 11 - 30
                        //        2022 - 01 - 31 - PREVIEW
                        //        2021 - 09 - 30 - PREVIEW
                        //        Api versions must be the latest or under 2 years old(730 days) - API version used by:
                        //            reference(resourceId('Microsoft.ManagedIdentity/userAssignedIdentities', variables('userAssignedIdentityName')), '2015-08-31-PREVIEW')
                        //        is 2535 days old Line: 15, Column: 24
                        //        Valid Api Versions for Microsoft.ManagedIdentity / userAssignedIdentities :
                        //        2018 - 11 - 30
                        //        2022 - 01 - 31 - PREVIEW
                        "[7] Use more recent API version for 'Fake.ManagedIdentity/userAssignedIdentities'. '2415-08-31-preview' is 2499 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2418-11-30",
                        "[12] Use more recent API version for 'Fake.ManagedIdentity/userAssignedIdentities'. '2415-08-31-preview' is 2499 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2418-11-30",
                    });
            }

            [TestMethod]
            // https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/old-apiVersion-FullReference.json
            public void ArmTtk_OldApiVersionFullReference_Fail()
            {
                string bicep = @"
                    output a object = reference(resourceId('Fake.Web/connections', 'test'), '2415-08-01-preview')
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        "[2] Use more recent API version for 'Fake.Web/connections'. '2415-08-01-preview' is 2529 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2416-06-01",
                    });
            }

            [TestMethod]
            // https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/old-apiVersion-NestedResource.json
            public void ArmTtk_OldApiVersionNestedReference_Fail()
            {
                string bicep = @"
                    output output1 object = reference(resourceId('Fake.Network/virtualNetworks/subnets', 'a', 'b'), '2415-06-15')
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        "[2] Use more recent API version for 'Fake.Network/virtualNetworks/subnets'. '2415-06-15' is 2576 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2420-11-01, 2420-08-01",
                    });
            }

            [TestMethod]
            // https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/old-apiVersion-in-List.json
            public void ArmTtk_OldApiversionInList_Fail()
            {
                string bicep = @"
                    output output1 object = list(resourceId('Fake.Network/publicIPAddresses', 'test'), '2415-06-15')
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                         // TTK results:
                         // [-] apiVersions Should Be Recent In Reference Functions(24 ms)
                         //       Api versions must be the latest or under 2 years old(730 days) - API version used by:
                         //           list(resourceId('Microsoft.Network/publicIPAddresses', 'test'), '2015-06-15')
                         //       is 2612 days old Line: 11, Column: 24
                         //       Valid Api Versions for Microsoft.Network/publicIPAddresses :
                         //       2022 - 01 - 01
                         //       2022 - 01 - 01
                         //       2021 - 12 - 01
                         //       2021 - 08 - 01
                         //       2021 - 06 - 01
                         //       2021 - 05 - 01
                         //       2021 - 04 - 01
                         //       2021 - 03 - 01
                         //       2021 - 02 - 01
                         //       2021 - 01 - 01
                         //       2020 - 11 - 01
                        "[2] Use more recent API version for 'Fake.Network/publicIPAddresses'. '2415-06-15' is 2576 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2420-11-01, 2420-08-01",
                    });
            }

            [TestMethod]
            // https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/old-apiVersion-in-ListAccountSas.json
            public void ArmTtk_OldApiversionInListAccountSas_Fail()
            {
                string bicep = @"
                    var listAccountSasRequestContent = {
                      signedServices: 'bfqt'
                      signedPermission: 'rwdlacup'
                      signedStart: '2021-06-02T00:00:00Z'
                      signedExpiry: '2030-10-30T00:00:00Z'
                      signedResourceTypes: 'sco'
                    }

                    #disable-next-line outputs-should-not-contain-secrets
                    output output1 object = listAccountSas(resourceId('Fake.Network/publicIPAddresses', 'test'), '2415-06-15', listAccountSasRequestContent)
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                         "[11] Use more recent API version for 'Fake.Network/publicIPAddresses'. '2415-06-15' is 2576 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2420-11-01, 2420-08-01",
                    });
            }

            [TestMethod]
            // https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/old-apiVersion-in-ListKeys.json
            public void ArmTtk_OldApiversionInListKeys_Fail()
            {
                string bicep = @"
                    #disable-next-line outputs-should-not-contain-secrets
                    output output1 object = listKeys(resourceId('Fake.Network/publicIPAddresses', 'test'), '2415-06-15')
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        "[3] Use more recent API version for 'Fake.Network/publicIPAddresses'. '2415-06-15' is 2576 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2420-11-01, 2420-08-01"
                    });
            }

            [TestMethod]
            // https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/old-apiVersion.json
            public void ArmTtk_OldApiVersion_Fail()
            {
                string bicep = @"
                    output output1 object = reference(resourceId('Fake.Network/publicIPAddresses', 'test'), '2415-06-15')
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        "[2] Use more recent API version for 'Fake.Network/publicIPAddresses'. '2415-06-15' is 2576 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2420-11-01, 2420-08-01"
                    });
            }

            [TestMethod]
            // https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/preview-apiVersion-ReferencedByVariable.json
            public void ArmTtk_PreviewApiVersionReferencedByVariable_Fail()
            {
                string bicep = @"
                    var sqlServerName_var = 'testSqlServer'

                    resource sqlServerName 'Fake.DBforMySQL/servers@2417-12-01' = {
                      name: sqlServerName_var
                    }

                    output output1 object = listkeys(sqlServerName_var, '2417-12-01-preview')
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        "[8] Use more recent API version for 'Fake.DBforMySQL/servers'. '2417-12-01-preview' is 1676 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2417-12-01",
                    });
            }

            [TestMethod]
            public void ReferenceFunction_ResourceIdLiteral_ApiVersionLiteral()
            {
                string bicep = @"
                    var lbPublicIPName = 'lbPublicIPName'
                    output o string = reference(resourceId('Microsoft.Network/publicIPAddresses', lbPublicIPName),'2020-08-01').dnsSettings.fqdn
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                FakeResourceTypes.ResourceScopeTypes,
                "2422-07-04",
                    new string[] {
                        // TTK result:
                        //[-] apiVersions Should Be Recent In Reference Functions(57 ms)
                        //    Api versions must be the latest or under 2 years old(730 days) - API version used by:
                        //        reference(resourceId('Microsoft.Network/publicIPAddresses', variables('lbPublicIPName')), '2020-08-01')
                        //    is 742 days old Line: 499, Column: 18
                        "[3] Could not find resource type \"Microsoft.Network/publicIPAddresses\"."
                    });
            }

            [TestMethod]
            public void ReferenceFunction_ResourceIdLiteral_ApiVersionLiteral_SpellingSuggestion()
            {
                string bicep = @"
                    var lbPublicIPName = 'lbPublicIPName'
                    output o string = reference(resourceId('Fake.Network/publicIPAddress', lbPublicIPName),'2020-08-01').dnsSettings.fqdn
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                FakeResourceTypes.ResourceScopeTypes,
                "2422-07-04",
                    new string[] {
                        "[3] Could not find resource type \"Fake.Network/publicIPAddress\". Did you mean \"Fake.Network/publicIPAddresses\"?"
                    });
            }

            [TestMethod]
            public void Reference_ExtraPartsBeyondResourceType()
            {
                string bicep = @"
                    output o string = reference(resourceId('Fake.Compute/virtualMachineScaleSets', 'virtualMachines/runCommands', 'a/b/c'), '2420-06-01').outputs.dbSubnetRef.value

                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                new string[] {
                    "[2] Use more recent API version for 'Fake.Compute/virtualMachineScaleSets/virtualMachines/runCommands'. '2420-06-01' is 763 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2421-07-01, 2421-04-01, 2421-03-01, 2420-12-01",
                });
            }

            [TestMethod]
            public void ArmTtk_Reference_ResourceId_ResourceTypeIn2ndParameter_Fail()
            {
                string bicep = @"
                    param vmName string = 'linux-zulu'
                    param storageAccountName string = 'storage${uniqueString(resourceGroup().id)}'
                    param storageAccountResourceGroupName string = resourceGroup().name
                    param location string = resourceGroup().location

                    resource vmName_resource 'Fake.Compute/virtualMachines@2421-07-01' = {
                      name: vmName
                      location: location
                      properties: {
                        diagnosticsProfile: {
                          bootDiagnostics: {
                            enabled: true
                            storageUri: reference(resourceId(storageAccountResourceGroupName, 'Fake.Storage/storageAccounts/', storageAccountName), '2420-12-01').primaryEndpoints.blob
                          }
                        }
                      }
                    }
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                FakeResourceTypes.ResourceScopeTypes,
                "2422-07-04",
                    new string[] {
                    // TTK result:
                    //    Api versions must be the latest or under 2 years old (730 days) - API version used by:
                    //        reference(resourceId(parameters('storageAccountResourceGroupName'), 'Microsoft.Storage/storageAccounts/', parameters('storageAccountName')), '2019-06-01')
                    //    is 1180 days old Line: 39, Column: 29
                    //    Valid Api Versions for Microsoft.Storage/storageAccounts/ :
                    //    2021-09-01
                    //    2021-09-01
                    //    2021-08-01
                    //    2021-06-01
                    //    2021-05-01
                    //    2021-04-01
                    //    2021-02-01
                    //    2021-01-01
                    "[14] Could not find apiVersion 2420-12-01 for Fake.Storage/storageAccounts. Acceptable versions: 2421-06-01, 2421-04-01, 2421-02-01, 2421-01-01",
                    });
            }

            [TestMethod]
            public void Reference_ResourceName()
            {
                string bicep = @"
                    output output string = reference('lbPublicIPName', '2020-01-01').outputs.dbSubnetRef.value

                    resource lbPublicIPName 'Fake.Network/publicIPAddresses@2020-11-01' = {
                      name: 'lbPublicIPName'
                    #disable-next-line no-hardcoded-location
                      location: 'location'
                      properties: {
                        publicIPAllocationMethod: 'Static'
                        dnsSettings: {
                          domainNameLabel: 'dnsName'
                        }
                      }
                    }
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                FakeResourceTypes.ResourceScopeTypes,
                "2422-07-04",
                    new string[] {
                        "[2] Could not find apiVersion 2020-01-01 for Fake.Network/publicIPAddresses. Acceptable versions: 2420-11-01, 2420-08-01", "[4] Could not find apiVersion 2020-11-01 for Fake.Network/publicIPAddresses. Acceptable versions: 2420-11-01, 2420-08-01",
                    });
            }

            [TestMethod]
            public void Reference_ResourceId()
            {
                string bicep = @"
                    param location string = resourceGroup().location
                    var userAssignedIdentityName_var = 'msi'
                    resource userAssignedIdentityName 'Fake.ManagedIdentity/userAssignedIdentities@2415-08-31-PREVIEW' = {
                      name: userAssignedIdentityName_var
                      location: location
                    }
                    output foo string = reference(userAssignedIdentityName.id, '2415-08-31-PREVIEW').principalId
                ";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                FakeResourceTypes.ResourceScopeTypes,
                "2422-07-04",
                    new string[] {
                        "[4] Use more recent API version for 'Fake.ManagedIdentity/userAssignedIdentities'. '2415-08-31-preview' is 2499 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2418-11-30",
                        "[8] Use more recent API version for 'Fake.ManagedIdentity/userAssignedIdentities'. '2415-08-31-preview' is 2499 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2418-11-30",
                    });
            }
        }
    }
}
