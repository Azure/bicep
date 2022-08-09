// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.ApiVersions;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using Bicep.Core.TypeSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Foundation;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    public partial class UseRecentApiVersionRuleTests
    {
        [TestClass]
        public class InReferenceFunctions
        {
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

        }

        [TestMethod]
        // https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Pass/NoApiVersion.json
        public void ArmTtk_NoApiVersion_Pass()
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

        //asdfg
        //[TestMethod]
        //public void ApiVersionInOutput_Fail()
        //{
        //    string bicep = @"
        //        output a object = reference(resourceId('Fake.DBforMySQL/servers', 'test'), '2417-12-01-preview')
        //    ";
        //    CompileAndTestWithFakeDateAndTypes(bicep,
        //        ResourceScope.ResourceGroup,
        //        FakeResourceTypes.ResourceScopeTypes,
        //        "2422-07-04",
        //        new string[] {
        //            "asdfg"
        //        });
        //}

        //asdfg
        //[TestMethod]
        //// https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Pass/latestStableApiOlderThan2Years.json
        //public void ArmTtk_LatestStableApiOlderThan2Years_Pass()
        //{
        //    string bicep = @"
        //        output a object = reference(resourceId('Fake.Web/connections', 'test'), '2416-06-01')
        //    ";
        //    CompileAndTestWithFakeDateAndTypes(bicep,
        //        ResourceScope.ResourceGroup,
        //        FakeResourceTypes.ResourceScopeTypes,
        //        "2422-07-04",
        //        new string[] {
        //            // pass
        //        });
        //}

        //asdfg
        //[TestMethod]
        //// https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/CaseInsenstiveApiVersion.json
        //public void ArmTtk_CaseInsensitiveApiVersion_Fail()
        //{
        //    string bicep = @"
        //        @description('The location in which the resources should be deployed.')
        //        param location string = resourceGroup().location

        //        var userAssignedIdentityName_var = 'msi'

        //        resource userAssignedIdentityName 'Fake.ManagedIdentity/userAssignedIdentities@2415-08-31-PREVIEW' = {
        //          name: userAssignedIdentityName_var
        //          location: location
        //        }

        //        output foo string = reference(userAssignedIdentityName.id, '2415-08-31-PREVIEW').principalId
        //    ";
        //    CompileAndTestWithFakeDateAndTypes(bicep,
        //        ResourceScope.ResourceGroup,
        //    FakeResourceTypes.ResourceScopeTypes,
        //    "2422-07-04",
        //        new string[] {
        //            // TTK result:
        //            //    [-] apiVersions Should Be Recent In Reference Functions(12 ms)
        //            //        reference(resourceId('Microsoft.ManagedIdentity/userAssignedIdentities', variables('userAssignedIdentityName')), '2015-08-31-PREVIEW')  uses a preview version(2015 - 08 - 31 - PREVIEW) and there are more recent
        //            //           versions available.Line: 15, Column: 24
        //            //        Valid Api Versions Microsoft.ManagedIdentity / userAssignedIdentities :                                           
        //            //        2018 - 11 - 30
        //            //        2022 - 01 - 31 - PREVIEW
        //            //        2021 - 09 - 30 - PREVIEW
        //            //        Api versions must be the latest or under 2 years old(730 days) - API version used by:
        //            //            reference(resourceId('Microsoft.ManagedIdentity/userAssignedIdentities', variables('userAssignedIdentityName')), '2015-08-31-PREVIEW')
        //            //        is 2535 days old Line: 15, Column: 24
        //            //        Valid Api Versions for Microsoft.ManagedIdentity / userAssignedIdentities :                                       
        //            //        2018 - 11 - 30
        //            //        2022 - 01 - 31 - PREVIEW
        //            "asdfg"
        //        });
        //}

        //asdfg
        //[TestMethod]
        //// https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/old-apiVersion-FullReference.json
        //public void ArmTtk_OldApiVersionFullReference_Fail()
        //{
        //    string bicep = @"
        //        output a object = reference(resourceId('Fake.Web/connections', 'test'), '2416-06-01')
        //    ";
        //    CompileAndTestWithFakeDateAndTypes(bicep,
        //        ResourceScope.ResourceGroup,
        //        FakeResourceTypes.ResourceScopeTypes,
        //        "2422-07-04",
        //        new string[] {
        //            "asdfg",
        //        });
        //}

        //asdfg
        //[TestMethod]
        //// https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/old-apiVersion-NestedResource.json
        //public void ArmTtk_OldApiVersionNestedReference_Fail()
        //{
        //    string bicep = @"
        //        output output1 object = reference(resourceId('Fake.Network/virtualNetworks/subnets', 'a', 'b'), '2415-06-15')
        //    ";
        //    CompileAndTestWithFakeDateAndTypes(bicep,
        //        ResourceScope.ResourceGroup,
        //        FakeResourceTypes.ResourceScopeTypes,
        //        "2422-07-04",
        //        new string[] {
        //            "asdfg",
        //        });
        //}

        //asdfg
        //[TestMethod]
        //// https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/old-apiVersion-in-List.json
        //public void ArmTtk_OldApiversionInList_Fail()
        //{
        //    string bicep = @"
        //        output output1 object = list(resourceId('Fake.Network/publicIPAddresses', 'test'), '2415-06-15')
        //    ";
        //    CompileAndTestWithFakeDateAndTypes(bicep,
        //        ResourceScope.ResourceGroup,
        //        FakeResourceTypes.ResourceScopeTypes,
        //        "2422-07-04",
        //        new string[] {
        //             // TTK results:
        //             // [-] apiVersions Should Be Recent In Reference Functions(24 ms)
        //             //       Api versions must be the latest or under 2 years old(730 days) - API version used by:
        //             //           list(resourceId('Microsoft.Network/publicIPAddresses', 'test'), '2015-06-15')
        //             //       is 2612 days old Line: 11, Column: 24
        //             //       Valid Api Versions for Microsoft.Network / publicIPAddresses :                                                    
        //             //       2022 - 01 - 01
        //             //       2022 - 01 - 01
        //             //       2021 - 12 - 01
        //             //       2021 - 08 - 01
        //             //       2021 - 06 - 01
        //             //       2021 - 05 - 01
        //             //       2021 - 04 - 01
        //             //       2021 - 03 - 01
        //             //       2021 - 02 - 01
        //             //       2021 - 01 - 01
        //             //       2020 - 11 - 01
        //            "asdfg",
        //        });
        //}

        //asdfg
        //[TestMethod]
        //// https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/old-apiVersion-in-ListAccountSas.json
        //public void ArmTtk_OldApiversionInListAccountSas_Fail()
        //{
        //    string bicep = @"
        //        var listAccountSasRequestContent = {
        //          signedServices: 'bfqt'
        //          signedPermission: 'rwdlacup'
        //          signedStart: '2021-06-02T00:00:00Z'
        //          signedExpiry: '2030-10-30T00:00:00Z'
        //          signedResourceTypes: 'sco'
        //        }

        //        #disable-next-line outputs-should-not-contain-secrets
        //        output output1 object = listAccountSas(resourceId('Fake.Network/publicIPAddresses', 'test'), '2415-06-15', listAccountSasRequestContent)
        //    ";
        //    CompileAndTestWithFakeDateAndTypes(bicep,
        //        ResourceScope.ResourceGroup,
        //        FakeResourceTypes.ResourceScopeTypes,
        //        "2422-07-04",
        //        new string[] {
        //            "asdfg",
        //        });
        //}

        //asdfg
        //[TestMethod]
        //// https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/old-apiVersion-in-ListKeys.json
        //public void ArmTtk_OldApiversionInListKeys_Fail()
        //{
        //    string bicep = @"
        //        #disable-next-line outputs-should-not-contain-secrets
        //        output output1 object = listKeys(resourceId('Fake.Network/publicIPAddresses', 'test'), '2415-06-15')
        //    ";
        //    CompileAndTestWithFakeDateAndTypes(bicep,
        //        ResourceScope.ResourceGroup,
        //        FakeResourceTypes.ResourceScopeTypes,
        //        "2422-07-04",
        //        new string[] {
        //            "asdfg",
        //        });
        //}

        //asdfg
        //[TestMethod]
        //// https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/old-apiVersion.json
        //public void ArmTtk_OldApiVersion_Fail()
        //{
        //    string bicep = @"
        //        output output1 object = reference(resourceId('Fake.Network/publicIPAddresses', 'test'), '2415-06-15')
        //    ";
        //    CompileAndTestWithFakeDateAndTypes(bicep,
        //        ResourceScope.ResourceGroup,
        //        FakeResourceTypes.ResourceScopeTypes,
        //        "2422-07-04",
        //        new string[] {
        //            "asdfg",
        //        });
        //}

        //asdfg
        //[TestMethod]
        //// https://github.com/Azure/arm-ttk/blob/master/unit-tests/apiVersions-Should-Be-Recent-In-Reference-Functions/Fail/preview-apiVersion-ReferencedByVariable.json
        //public void ArmTtk_PreviewApiVersionReferencedByVariable_Fail()
        //{
        //    string bicep = @"
        //        var sqlServerName_var = 'testSqlServer'

        //        resource sqlServerName 'Fake.DBforMySQL/servers@2417-12-01-preview' = {
        //          name: sqlServerName_var
        //        }

        //        output output1 object = listkeys(sqlServerName_var, '2417-12-01-preview')
        //    ";
        //    CompileAndTestWithFakeDateAndTypes(bicep,
        //        ResourceScope.ResourceGroup,
        //        FakeResourceTypes.ResourceScopeTypes,
        //        "2422-07-04",
        //        new string[] {
        //            "asdfg",
        //        });
        //}
    }
}
