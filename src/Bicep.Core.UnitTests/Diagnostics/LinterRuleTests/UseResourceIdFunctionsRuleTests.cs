// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CA1825 // Avoid zero-length array allocations

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class UseResourceIdFunctionsRuleTests : LinterRuleTestsBase
    {
        private static void CompileAndTest(string bicep, params string[] expectedMessages)
        {
            AssertLinterRuleDiagnostics(UseResourceIdFunctionsRule.Code, bicep, expectedMessages, new Options(OnCompileErrors.IncludeErrors, IncludePosition.LineNumberAndColumn));
        }

        private static void CompileAndTest(string bicep, Options options, params string[] expectedMessages)
        {
            AssertLinterRuleDiagnostics(UseResourceIdFunctionsRule.Code, bicep, expectedMessages, options);
        }

        [DataRow(@"
            @metadata({ Description: 'The name of the Virtual Network to Create' })
            param virtualNetworkName string

            @metadata({ Description: 'The address range of the new VNET in CIDR format' })
            param virtualNetworkAddressRange string = '10.0.0.0/16'

            @metadata({ Description: 'The name of the subnet created in the new VNET' })
            param subnetName string

            @metadata({ Description: 'The address range of the subnet created in the new VNET' })
            param subnetRange string = '10.0.0.0/24'

            @metadata({ Description: 'The DNS address(es) of the DNS Server(s) used by the VNET' })
            param DNSServerAddress array

            @metadata({ Description: 'The name of an existing NSG to associate with the subnet' })
            param NSGName string
            param location string

            resource virtualNetworkName_resource 'Microsoft.Network/virtualNetworks@2020-05-01' = {
              name: virtualNetworkName
              location: location
              properties: {
                addressSpace: {
                  addressPrefixes: [
                    virtualNetworkAddressRange
                  ]
                }
                dhcpOptions: {
                  dnsServers: DNSServerAddress
                }
                subnets: [
                  {
                    name: subnetName
                    properties: {
                      addressPrefix: subnetRange
                      networkSecurityGroup: {
                        id: '/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Network/networkSecurityGroups/${NSGName}'
                      }
                    }
                  }
                ]
              }
            }",
            new object[]
            {
                // TTK result:
                // Property: "id" must use one of the following expressions for an resourceId property:
                //  extensionResourceId,resourceId,subscriptionResourceId,tenantResourceId.,if,parameters,reference,variables,subscription,guid
                "[39:25] If property \"id\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId."
            },
            DisplayName = "virtualNetworkName fail"
        )]
        [DataRow(@"
            param NSGName string
            resource virtualNetworkName_resource 'Microsoft.Network/virtualNetworks@2022-01-01' = {
              name: 'virtualNetworkName'
              location: 'location'
              properties: {
                addressSpace: {
                  addressPrefixes: [
                    'virtualNetworkAddressRange'
                  ]
                }
                dhcpOptions: {
                  dnsServers: 'DNSServerAddress'
                }
                subnets: [
                  {
                    name: 'subnetName'
                    properties: {
                      addressPrefix: 'subnetRange'
                      networkSecurityGroup: {
                        foo: {
                          whatever: {
                            andmore: {
                              id: '/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Network/networkSecurityGroups/${NSGName}'
                            }
                          }
                        }
                      }
                    }
                  }
                ]
              }
            }",
            new object[]
            {
                "[24:31] If property \"id\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId."
            },
            DisplayName = "'id' should be analyzed anywhere in the resource tree"
        )]
        [DataRow(@"
              resource parent 'Microsoft.Web/sites@2022-03-01' = {
                  name: 'FAIL-top-level-invalid'
                  kind: 'app'
                  location: 'eastus'
                  properties: {
                    firstId: 'my/id'
                  }

                  resource child 'backups' = {
                    name: 'child'
                    properties: {
                      secondId: 'my/id'
                    }
                  }
                }",
            new object[]
            {
                "[7:21] If property \"firstId\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId.",
                "[13:23] If property \"secondId\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId.",
            },
            DisplayName = "'id' should be analyzed in nested resources"
        )]
        [DataRow(@"
            param id string = '/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Network/networkSecurityGroups/${'NSGName'}'
            ",
            new object[]
            {
            // pass
            },
            DisplayName = "'id' should not be analyzed in params")]
        [DataRow(@"
                output id object = {
                  id: {
                    id: '/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Network/networkSecurityGroups/${'NSGName'}'
                  }
                }   ",
            new object[]
            {
                // pass
            },
            DisplayName = "'id' should not be analyzed in outputs"
        )]
        [DataRow(@"
                var id = {
                  id: {
                    id: '/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Network/networkSecurityGroups/${'NSGName'}'
                  }
                }",
            new object[]
            {
                // pass
            },
            DisplayName = "'id' should not be analyzed in vars"
        )]
        [DataRow(@"
                module m1 'main.bicep' = {
                  name: 'm1'
                  params: {
                    id: '/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Network/networkSecurityGroups/${'NSGName'}'
                  }
                }",
            new object[]
            {
                 // pass
                 "[2:27] This module references itself, which is not allowed."
            },
            DisplayName = "'id' should not be analyzed in modules"
        )]
        [DataRow(@"
                resource r 'Microsoft.Web/sites@2022-03-01' = {
                  name: 'name'
                  kind: 'app'
                  location: 'eastus'
                  properties: {
                    id: 'my/id'
                  }
                }",
            new object[]
            {
                "[7:21] If property \"id\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId."
            },
            DisplayName = "Quoted property keys"
        )]
        [DataRow(@"
                        @metadata({ Description: 'The name of the Virtual Network to Create' })
                        param virtualNetworkName string

                        @metadata({ Description: 'The address range of the new VNET in CIDR format' })
                        param virtualNetworkAddressRange string = '10.0.0.0/16'

                        @metadata({ Description: 'The name of the subnet created in the new VNET' })
                        param subnetName string

                        @metadata({ Description: 'The address range of the subnet created in the new VNET' })
                        param subnetRange string = '10.0.0.0/24'

                        @metadata({ Description: 'The DNS address(es) of the DNS Server(s) used by the VNET' })
                        param DNSServerAddress array

                        @metadata({ Description: 'The name of an existing NSG to associate with the subnet' })
                        param NSGName string
                        param location string

                        resource virtualNetworkName_resource 'Microsoft.Network/virtualNetworks@2020-05-01' = {
                          name: virtualNetworkName
                          location: location
                          properties: {
                            addressSpace: {
                              addressPrefixes: [
                                virtualNetworkAddressRange
                              ]
                            }
                            dhcpOptions: {
                              dnsServers: DNSServerAddress
                            }
                            subnets: [
                              {
                                name: subnetName
                                properties: {
                                  addressPrefix: subnetRange
                                  networkSecurityGroup: {
                                    id: resourceId('Microsoft.Network/networkSecurityGroups', NSGName)
                                  }
                                }
                              }
                            ]
                          }
                        }
                    ",
                    new object[]
                    {
                        // Pass
                    },
            DisplayName = "virtualNetworkName")]
        [DataRow(
            @"
                var id = 'id'

                resource name 'Microsoft.Storage/storageAccounts@2021-09-01' = {
                  name: 'name'
                  properties: {
                    fail4: {
                      resourceId: true
                    }
                  }
                }",
            new object[]
            {
                // TTK complains, but Bicep will show an error for type mismatch, so we'll all values that aren't string
                // pass
            },
            DisplayName = "bool value - ignore"
            )]
        // from https://github.com/Azure/arm-ttk/blob/master/unit-tests/IDs-Should-Be-Derived-From-ResourceIDs/Fail/concat.json
        [DataRow(
            @"
                var id = 'id'

                resource name 'Microsoft.Storage/storageAccounts@2021-09-01' = {
                  name: 'name'
                  properties: {
                    fail4: {
                      resourceId: concat(id) //technically it works, but not a best practice
                    }
                  }
                }",
            new object[]
            {
                // TTK result:
                // Property: "resourceId" must use one of the following expressions for an resourceId property:
                // extensionResourceId,resourceId,subscriptionResourceId,tenantResourceId.,if,parameters,reference,variables,subscription,guid
                "[8:23] If property \"resourceId\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId."
            },
            DisplayName = "Fail/concat.json")]
        // from https://github.com/Azure/arm-ttk/blob/master/unit-tests/IDs-Should-Be-Derived-From-ResourceIDs/Fail/documentdb-sqldatabases-that-contains-invalid-id-property-should-fail.json
        [DataRow(
            @"
                resource dbAccount_dbName 'Microsoft.DocumentDB/databasail/documentdb-sqldatabases-that-contains-invalid-id-property-should-fes@2021-04-15' = {
                  name: 'dbAccount/dbName'
                  location: 'eastus'
                  tags: {
                  }
                  properties: {
                    resource: {
                      id: 'dbAccount/dbName' // this should pass
                    }
                    failId: 'this is not excluded from verification/should fail' // this should fail
                  }
                }",
            new object[]
            {
                // TTK result:
                // Property: "id" must use one of the following expressions for an resourceId property:
                //   extensionResourceId,resourceId,subscriptionResourceId,tenantResourceId.,if,parameters,reference,variables,subscription,guid
                "[11:21] If property \"failId\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId.",
            },
            DisplayName = "Fail/documentdb-sqldatabases-that-contains-invalid-id-property-should-fail.json")]
        // from https://github.com/Azure/arm-ttk/blob/master/unit-tests/IDs-Should-Be-Derived-From-ResourceIDs/Fail/empty-string.json
        [DataRow(
            @"
                resource name 'Microsoft.Storage/storageAccounts@2021-09-01' = {
                  name: 'name'
                  properties: {
                    fail4: {
                      id: ''
                    }
                  }
                }",
            new object[]
            {
                // TTK fails on this:
                //
                //    [-] IDs Should Be Derived From ResourceIDs(52 ms)
                //        Blank ID Property found:
                //id ParentObject       PropertyName JSONPath
                //----------------------------------
                //   {@{fail0 =}, $null}
                //id fail0.id

                // However, in the Bicep rule we're treating it the same as a literal without any forward slashes - if it doesn't look like an ID,
                // probably not intended to be, so to reduce likelihood of false positives, let it pass
            },
            DisplayName = "empty-string.json")]
        // from https://github.com/Azure/arm-ttk/blob/master/unit-tests/IDs-Should-Be-Derived-From-ResourceIDs/Fail/literal.json
        [DataRow(
            @"
                resource name 'Microsoft.Storage/storageAccounts@2021-09-01' = {
                  name: 'name'
                  properties: {
                    fail4: {
                      id: '/subscriptions/literal'
                    }
                  }
                }",
            new object[]
            {
                "[6:23] If property \"id\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId."
            },
            DisplayName = "Fail/literal.json")]
        [DataRow(
            @"
                resource name 'Microsoft.Storage/storageAccounts@2021-09-01' = {
                  name: 'name'
                  properties: {
                    fail4: {
                      id: 'subscriptions/literal'
                    }
                  }
                }",
            new object[]
            {
                "[6:23] If property \"id\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId."
            },
            DisplayName = "literal without any forward slashes - pass - if it doesn't look like an ID, probably not intended to be, so reduce likelihood of false positives")]
        [DataRow(
            @"
                var noslashes = 'hello'
                var noslashes2 = noslashes
                var slashes = 'hello/there'
                var slashes2 = slashes
                var blank = ''
                var blank2 = blank

                resource name 'a.b/c@2021-09-01' = {
                  name: 'name'
                  properties: {
                    pass1: {
                      blankPassId: blank2
                    }
                    pass2: {
                      noslashesPassId: noslashes2
                    }
                    fail1: {
                      slashesFailId: slashes2
                    }
                  }
                }",
            new object[]
            {
                "[19:23] If property \"slashesFailId\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId. Found nonconforming expression at slashesFailId -> slashes2 -> slashes"
            },
            DisplayName = "literal in variable")]
        [DataRow(
            @"
                param noslashes string = 'hello'
                param noslashes2 string = noslashes
                param slashes string = 'hello/there'
                param slashes2 string = slashes
                param blank string = ''
                param blank2 string = blank
                param nodefault string

                resource name 'a.b/c@2021-09-01' = {
                  name: 'name'
                  properties: {
                    pass1: {
                      blankPassId: blank2
                    }
                    pass2: {
                      noslashesPassId: noslashes2
                    }
                    fail3: {
                      slashesFailId: slashes2
                    }
                    pass4: {
                        nodefaultId: nodefault
                    }
                  }
                }",
            new object[]
            {
                // pass - all parameters are acceptable regardless of default value
            },
            DisplayName = "literal in param default value")]
        // from "Pass/IDs-In-AppSettings.json"
        [DataRow(@"
            var AppServiceName = 'value'
            var location = 'value'

            resource AppServiceName_appsettings 'Microsoft.Web/sites/config@2020-09-01' = {
              name: '${AppServiceName}/appsettings'
              location: location
              properties: {
                SOMEONES_ID: concat('495f6d91-cceb-4916-bf29-c07bea002443')
              }
            }",
            new object[]
            {
                // pass
            },
            DisplayName = "Pass/IDs-In-AppSettings.json")]
        // from https://github.com/Azure/arm-ttk/blob/master/unit-tests/IDs-Should-Be-Derived-From-ResourceIDs/Pass/IDs-In-KeyVault.json
        [DataRow(
        @"
            var certificateOrderName_var = 'certificateOrderName'
            var existingAppLocation = 'existingAppLocation'
            var existingKeyVaultId = 'existingKeyVault/id'

            // should pass - keyVaultId property name is an exception
            resource certificateOrderName 'Microsoft.Web/certificates@2015-08-01' = {
              name: certificateOrderName_var
              location: existingAppLocation
              properties: {
                keyVaultId: existingKeyVaultId
                keyVaultSecretName: certificateOrderName_var
              }
            }
            // should fail
            resource certificateOrderName2 'Microsoft.Web/certificates@2015-08-01' = {
              name: certificateOrderName_var
              location: existingAppLocation
              properties: {
                keyVaultShouldFailId: existingKeyVaultId // doesn't match keyVaultId
                keyVaultSecretName: certificateOrderName_var
              }
            }
            ",
            new object[]
            {
                "[20:17] If property \"keyVaultShouldFailId\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId. Found nonconforming expression at keyVaultShouldFailId -> existingKeyVaultId",
            },
            DisplayName = "pass/IDs-In-KeyVault.json")]
        // from https://github.com/Azure/arm-ttk/blob/master/unit-tests/IDs-Should-Be-Derived-From-ResourceIDs/Pass/IDs-In-Metadata.json
        [DataRow(@"
            var metadataVariable = {
              position: {
                x: 8
                y: 4
                colSpan: 7
                rowSpan: 3
              }
              metadata: {
                inputs: [
                  'removed inputs for brevity'
                ]
                type: 'Extension/Microsoft_OperationsManagementSuite_Workspace/PartType/LogsDashboardPart'
                settings: {
                  content: {
                    GridColumnsWidth: {
                      timestamp: '145px'
                      message: '194px'
                      customDimensions: '351px'
                      Type: '172px'
                      Details: '522px'
                      cycleId: concat('76/px')
                      totalStates: '96px'
                    }
                    Query: ' *removed beginning part of Query for brevity* | Project timestamp, message, cycleId, totalStates, processed\n'
                    PartTitle: 'Policy Processing runs'
                    PartSubTitle: 'Cycles'
                  }
                }
                filters: {
                  MsPortalFx_TimeRange: {
                    model: {
                      format: 'local'
                      granularity: 'auto'
                      relative: '24h'
                    }
                  }
                }
              }
            }",
            new object[]
            {
                // pass
            },
            DisplayName = "Pass/IDs-In-Metadata.json")]
        // from https://github.com/Azure/arm-ttk/blob/master/unit-tests/IDs-Should-Be-Derived-From-ResourceIDs/Pass/IDs-in-Backends.json
        [DataRow(
        @"// Pass/IDs-in-Backends.json
            var webServiceName = 'webServiceName'
            var apiManagementInstanceName = 'apiManagementInstanceName'

            resource ApiApp_webServiceName_backend 'Microsoft.ApiManagement/service/backends@2020-06-01-preview' = {
              name: 'ApiApp_${webServiceName}/backend'
              properties: {
                description: webServiceName
                resourceId: 'https://management.azure.com${resourceId('Microsoft.Web/sites', webServiceName)}'
                url: 'https://${reference(resourceId('Microsoft.Web/sites', webServiceName), '2020-09-01').defaultHostName}'
                protocol: 'http'
              }
            }",
            new object[]
            {
                // pass
            },
            DisplayName = "Pass/IDs-in-Backends")]
        // from https://github.com/Azure/arm-ttk/blob/master/unit-tests/IDs-Should-Be-Derived-From-ResourceIDs/Pass/IDs-in-WebTest-Locations.json
        [DataRow(
        @"
            var location1 = concat('location1')

            resource WebTest 'microsoft.insights/webtests@2015-05-01' = {
              name: 'WebTest'
              properties: {
                Locations: [
                  {
                    Id: location1
                  }
                  {
                    Id: concat('us-tx-sn1-azr')
                  }
                  {
                    Id: concat('us-il-ch1-azr')
                  }
                  {
                    Id: concat('us-va-ash-azr')
                  }
                  {
                    Id: concat('us-fl-mia-edge')
                  }
                ]
              }
            }",
            new object[]
            {
                // pass
            },
            DisplayName = "Pass/IDs-in-WebTest-Locations.json")]
        // from https://github.com/Azure/arm-ttk/blob/master/unit-tests/IDs-Should-Be-Derived-From-ResourceIDs/Pass/dashboard-that-contains-id-should-pass.json
        [DataRow(@"
                param dashboardName string = 'armttkDashboard'
                param appinsightName string

                resource dashboardName_resource 'Microsoft.Portal/dashboards@2020-09-01-preview' = {
                  name: dashboardName
                  location: 'westeurope'
                  tags: {
                    'hidden-title': 'arm-ttk dashboard'
                  }
                  properties: {
                    lenses: [
                      {
                        order: 0
                        parts: [
                          {
                            position: {
                              x: 0
                              y: 0
                              rowSpan: 4
                              colSpan: 10
                            }
                            metadata: {
                              inputs: [
                                {
                                  name: 'resourceTypeMode'
                                  isOptional: true
                                }
                                {
                                  name: 'ComponentId'
                                  isOptional: true
                                }
                                {
                                  name: 'Scope'
                                  value: {
                                    resourceIds: [
                                      resourceId('microsoft.insights/components', appinsightName)
                                    ]
                                  }
                                  isOptional: true
                                }
                                {
                                  name: 'PartId'
                                  value: '45be4c00-bc15-4b77-9134-dc091d4cff32'
                                  isOptional: true
                                }
                                {
                                  name: 'Version'
                                  value: '2.0'
                                  isOptional: true
                                }
                                {
                                  name: 'TimeRange'
                                  value: 'P1D'
                                  isOptional: true
                                }
                                {
                                  name: 'DashboardId'
                                  isOptional: true
                                }
                                {
                                  name: 'DraftRequestParameters'
                                  isOptional: true
                                }
                                {
                                  name: 'Query'
                                  value: 'traces\n| order by timestamp\n| extend ProcessId = tostring(customDimensions[""ProcessId""])\n| extend EventName = tostring(customDimensions[""EventName""])\n| project message, appId, ProcessId, EventName\n\n'
                                  isOptional: true
                                }
                                {
                                  name: 'ControlType'
                                  value: 'AnalyticsGrid'
                                  isOptional: true
                                }
                                {
                                  name: 'SpecificChart'
                                  isOptional: true
                                }
                                {
                                  name: 'PartTitle'
                                  value: 'Analytics'
                                  isOptional: true
                                }
                                {
                                  name: 'PartSubTitle'
                                  value: appinsightName
                                  isOptional: true
                                }
                                {
                                  name: 'Dimensions'
                                  isOptional: true
                                }
                                {
                                  name: 'LegendOptions'
                                  isOptional: true
                                }
                                {
                                  name: 'IsQueryContainTimeRange'
                                  value: false
                                  isOptional: true
                                }
                              ]
                              type: 'Extension/Microsoft_OperationsManagementSuite_Workspace/PartType/LogsDashboardPart'
                              settings: {
                                content: {
                                  GridColumnsWidth: {
                                    appId: concat('268px')
                                    message: '232px'
                                    ProcessId: concat('75px')
                                    EventName: '192px'
                                  }
                                }
                              }
                            }
                          }
                        ]
                      }
                    ]
                    metadata: {
                      model: {
                        timeRange: {
                          value: {
                            relative: {
                              duration: 24
                              timeUnit: 1
                            }
                          }
                          type: 'MsPortalFx.Composition.Configuration.ValueTypes.TimeRange'
                        }
                        filterLocale: {
                          value: 'en-us'
                        }
                        filters: {
                          value: {
                            MsPortalFx_TimeRange: {
                              model: {
                                format: 'utc'
                                granularity: 'auto'
                                relative: '24h'
                              }
                              displayCache: {
                                name: 'UTC Time'
                                value: 'Past 24 hours'
                              }
                              filteredPartIds: [
                                'StartboardPart-LogsDashboardPart-a92d5c95-132e-4e72-be32-cfd64bdf9124'
                              ]
                            }
                          }
                        }
                      }
                    }
                  }
                }",
            new object[]
            {
                // pass
            },
            DisplayName = "Pass/dashboard-that-contains-id-should-pass.json")]
        // from https://github.com/Azure/arm-ttk/blob/master/unit-tests/IDs-Should-Be-Derived-From-ResourceIDs/Pass/documentdb-sqldatabases-that-contains-resource-id-should-pass.json
        [DataRow(
            @"
            resource dbAccount_dbName 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-04-15' = {
              name: 'dbAccount/dbName'
              location: 'eastus'
              tags: {
              }
              properties: {
                resource: {
                  shouldPass1Id: 'db/id'
                }
              }
            }

            resource keys 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/clientEncryptionKeys@2022-05-15-preview' = {
              name: 'sql/db/keys'
              properties: {
                resource: {
                  shouldPass2Id: 'db/id'
                }
              }
            }
            ",
            new object[]
            {
                // pass
            },
            DisplayName = "Pass/documentdb-sqldatabases-that-contains-resource-id-should-pass.json")]
        // from https://github.com/Azure/arm-ttk/blob/master/unit-tests/IDs-Should-Be-Derived-From-ResourceIDs/Pass/exceptions-that-should-pass.json
        [DataRow(
        @"
            param storageAccountName string = 'name/id'
            param storageResourceGroupName string = 'name/id'
            param appGatewayBackendPool string = 'name-no-slashes'

            var id = guid('ok')

            resource somepropertyname 'Microsoft.CustomProviders/resourceProviders@2018-09-01-preview' = {
              name: 'somepropertyname'
              properties: {
                id: resourceId('Microsoft.Storage/storageAccounts', 'name')
                pass1: {
                  pass1id: resourceId('Microsoft.Network/applicationGateways/httpListeners', 'appGW', 'appGatewayHttpListener')
                }
                pass2: {
                  pass2id: appGatewayBackendPool
                }
                pass3: {
                  pass3id: id
                }
                pass4: {
                  pass4id: subscriptionResourceId('Microsoft.Storage/storageAccounts', storageAccountName)
                }
                pass5: {
                  pass5id: tenantResourceId('Microsoft.Storage/storageAccounts', storageAccountName)
                }
                pass6: {
                  pass6id: extensionResourceId(resourceId('Microsoft.Storage/storageAccounts', storageAccountName), 'Microsoft.Resources/type', 'resourceName')
                }
                pass7: {
                  pass7id: resourceId(storageResourceGroupName, 'Microsoft.Storage/storageAccounts', storageAccountName)
                }
                pass8: {
                  pass8id: (bool('true') ? resourceId(storageResourceGroupName, 'Microsoft.Storage/storageAccounts', storageAccountName) : json('null'))
                }
                exceptionList: {
                  appId: concat('some data value')
                  clientId: concat('some data value')
                  DataTypeId: concat('some data value')
                  defaultMenuItemId: concat('some data value')
                  keyVaultSecretId: concat('some data value')
                  keyId: concat('some data value')
                  objectId: concat('some data value')
                  menuId: concat('some data value')
                  policyDefinitionReferenceId: concat('some data value')
                  servicePrincipalClientId: concat('some data value')
                  StartingDeviceID: concat('some data value')
                  subscriptionId: concat('some data value')
                  SyntheticMonitorId: concat('some data value')
                  targetProtectionContainerId: concat('some data value')
                  targetWorkerSizeId: concat('some data value')
                  tenantId: concat('some data value')
                  timezoneId: concat('some data value')
                  vlanId: concat('some data value')
                  workerSizeId: concat('some data value')
                  detector: {
                    id: concat('someDetectorID')
                  }
                }
                tags: {
                  intagId: concat('some data')
                }
              }
            }

            resource service_backend 'Microsoft.ApiManagement/service/backends@2020-06-01-preview' = {
              name: concat('service/backend')
              properties: {
                description: 'desc'
                resourceId: 'https://management.azure.com${resourceId('Microsoft.Web/sites', 'web')}'
                url: 'https://${reference(resourceId('Microsoft.Web/sites', 'web'), '2020-09-01').defaultHostName}'
                protocol: 'http'
              }
            }

            // https://github.com/Azure/bicep/issues/8382
            resource keys 'Microsoft.ApiManagement/service/subscriptions@2021-08-01' = {
              name: 'apimName/testSubscription'
              properties: {
                ownerId: '/users/1' // in form /users/{userId}, so this property should be ignored
                scope: '/products/testProduct'
                displayName: 'Test Name'
                state: 'active'
                allowTracing: true
              }
            }
",
            new object[]
            {
                // pass
            },
            DisplayName = "Pass/exceptions-that-should-pass.json")]
        //// from https://github.com/Azure/arm-ttk/blob/master/unit-tests/IDs-Should-Be-Derived-From-ResourceIDs/Pass/logic-app-that-contain-id-should-pass.json
        [DataRow(
        @"
            resource mylogicapp 'Microsoft.Logic/workflows@2019-05-01' = {
              name: 'mylogicapp'
              properties: {
                definition: {
                  '$schema': 'https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#'
                  contentVersion: '1.0.0.0'
                  triggers: {
                    manual: {
                      correlation: {
                        clientTrackingId: concat('@{coalesce(triggerOutputs().headers?[\'id\'], guid())}')
                      }
                      emailId: concat('@body(\'Get_email_(V2)_-_Processing\')?[\'id\']')
                    }
                    somedataId: concat('@concat(parameters(\'firstName\'), parameters(\'lastName\'))')
                  }
                }
              }
            }",
            new object[]
            {
                // pass
            },
            DisplayName = "logic-app-that-contain-id-should-pass.json")]
        [DataRow(
        @"
            resource mylogicapp 'Microsoft.Logic/notWorkflows@2019-05-01' = {
              name: 'mylogicapp'
              properties: {
                guid: 'abc/def'   // ignore
                myGuid: 'abc/def' // ignore
                myUuid: 'abc/def' // ignore
                myUUID: 'abc/def' // ignore
                guId: 'abc/def'   // fail
              }
            }",
            new object[]
            {
                // guId: 'abc/def'   // not considered "guid", fail
                "[9:17] If property \"guId\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId."
            },
            DisplayName = "Ignore properties like \"UID\" and \"guid\" (but not 'guId')")]
        [DataRow(
        @"
            var idValue = concat('a/', 'b')

            resource res1 'Microsoft.CustomProviders/resourceProviders@2018-09-01-preview' = {
              name: 'res1'
              properties: {
                id: idValue
              }
            }",
            new object[]
            {
                "[7:17] If property \"id\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId. Found nonconforming expression at id -> idValue",
            },
            DisplayName = "resolved variable simple")]
        [DataRow(
        @"
            var idValue = concat('a/', 'b')
            var idValue2 = idValue

            resource res1 'Microsoft.CustomProviders/resourceProviders@2018-09-01-preview' = {
              name: 'res1'
              properties: {
                id: idValue2
              }
            }",
            new object[]
            {
                "[8:17] If property \"id\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId. Found nonconforming expression at id -> idValue2 -> idValue",
            },
        DisplayName = "resolved variable double")]
        [DataRow(
        @"
            var idValue = reference('a', 'b')
            var idValue2 = idValue

            resource res1 'Microsoft.CustomProviders/resourceProviders@2018-09-01-preview' = {
              name: 'res1'
              properties: {
                id: idValue
              }
            }"
        ,
        new object[]
        {
        },
            DisplayName = "resolved variable double - passing")]
        [DataRow(@"
            param idValue1 string
            param idValue2 string = concat('a/', 'b')
            param idValue3 string = concat('a', 'b')
            param idValue4 string = idValue3

            resource res1 'Microsoft.CustomProviders/resourceProviders@2018-09-01-preview' = {
              name: 'res1'
              properties: {
                firstId: idValue1
                secondId: idValue2
                thirdId: idValue3
                fourthId: idValue4
              }
            }",
            new object[]
            {
                // pass
            },
            DisplayName = "parameters should always pass")]
        [DataRow(@"
            param p1 string = concat('a/', 'b')
            param p2 string = p1
            var v3 = p2
            var p4 = v3

            resource res1 'Microsoft.CustomProviders/resourceProviders@2018-09-01-preview' = {
              name: 'res1'
              properties: {
                id: p4
              }
            }",
            new object[]
            {
                // pass
            },
        DisplayName = "variables defined from parameters")]
        [DataRow(@"
            param p1 string = resourceId('', '') // acceptable function call
            param p2 string = p1
            var v3 = p2
            var v4 = v3

            resource res1 'Microsoft.CustomProviders/resourceProviders@2018-09-01-preview' = {
              name: 'res1'
              properties: {
                id: v4
              }
            }"
        ,
        new object[]
        {
            // pass
            },
            DisplayName = "resolved parameter default values and variables - passing")]
        [DataRow(@"
            var storageAccountName = 'name/id'
            var storageResourceGroupName = 'name/id'
            var appGatewayBackendPool = 'name/id'

            var id = guid('ok') // acceptable function

            resource somepropertyname 'Microsoft.CustomProviders/resourceProviders@2018-09-01-preview' = {
              name: 'somepropertyname'
              properties: {
                id: resourceId('Microsoft.Storage/storageAccounts', 'name')
                pass1: {
                  pass1id: extensionResourceId('Microsoft.Network/applicationGateways/httpListeners', 'appGW', 'appGatewayHttpListener') // pass - resourceId() used
                }
                fail2: {
                  fail2id: appGatewayBackendPool // failed - variable -> string literal with forward slashes
                }
                pass3: {
                  pass3id: id // pass - guid() function used
                }
              }
            }",
            new object[]
            {
                "[16:19] If property \"fail2id\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId. Found nonconforming expression at fail2id -> appGatewayBackendPool",
            },
            DisplayName = "Resolved variables with string literals")]
        [DataRow(@"
                var v1 = v3
                var v2 = v1
                var v3 = v2

                resource res1 'Microsoft.CustomProviders/resourceProviders@2018-09-01-preview' = {
                  name: 'res1'
                  properties: {
                    id: v3
                  }
                }",
                new object[]
                {
                    "[2:26] The expression is involved in a cycle (\"v3\" -> \"v2\" -> \"v1\").",
                    "[3:26] The expression is involved in a cycle (\"v1\" -> \"v3\" -> \"v2\").",
                    "[4:26] The expression is involved in a cycle (\"v2\" -> \"v1\" -> \"v3\").",
                    "[9:25] The expression is involved in a cycle (\"v3\" -> \"v2\" -> \"v1\").",
                },
            DisplayName = "resolved variable cycle should not hang")]
        [DataRow(@"
                resource dashboardName_resource1 'Microsoft.Portal/dashboards@2020-09-01-preview' = {
                    location: 'location'
                    name: 'name1'
                    properties: {
                        pass1Id: 'abc/def'
                    }
                }
                resource dashboardName_resource2 'Microsoft.PORTAL/DASHBOARDS@2020-09-01-preview' = {
                    location: 'location'
                    name: 'name2'
                    properties: {
                        pass2Id: 'abc/def'
                    }
                }
                resource dashboardName_resource3 'microsoft.portal/dashboards@2020-09-01-preview' = {
                    location: 'location'
                    name: 'name3'
                    properties: {
                        pass3Id: 'abc/def'
                    }
                }
            ",
            new object[]
            {
                // pass
            },
            DisplayName = "Excluded resources should ignore casing")]
        [DataRow(@"
                param whatever bool

                resource dashboardName_resource1 'ms.something/dashboards@2020-09-01-preview' = {
                  location: 'location'
                  name: 'name1'
                  properties: {
                      id: whatever ? concat('', '') : concat('', '')
                  }
                }
               ",
            new object[]
            {
                // pass
            },
            DisplayName = "if")]
        [DataRow(@"
                resource res1 'Microsoft.CustomProviders/resourceProviders@2018-09-01-preview' = {
                  name: 'res1'
                  properties: {
                    resourceId: concat(resourceId('', ''))
                  }
                }"
            ,
            new object[]
            {
                "[5:21] If property \"resourceId\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId."
            },
            DisplayName = "resourceId has to be outermost function")]
        [DataRow(@"
                resource existing 'ms.something/else@2020-09-01-preview' existing = {
                  name: 'name1'
                }
                resource storageaccount 'ms.something/else@2021-02-01' = {
                  name: 'name'
                  properties: {
                    id: existing.id
                  }
                  location: 'location'
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ",
            new object[]
            {
                // pass
            },
            DisplayName = "symbolic name simple")]
        [DataRow(@"
                resource existing 'ms.something/else@2020-09-01-preview' existing = {
                  name: 'name1'
                }
                resource storageaccount 'ms.something/else@2021-02-01' = {
                  name: 'name'
                  properties: {
                    id: 'a${true ? 'b' : existing.id}' // passes as long as existing.id is anywhere in the expression
                  }
                  location: 'location'
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }",
            new object[]
            {
                // pass
            },
            DisplayName = "symbolic name deep")]
        [DataRow(@"
                resource existing 'ms.something/else@2020-09-01-preview' existing = {
                  name: 'name1'
                }
                resource storageaccount 'ms.something/else@2021-02-01' = {
                  name: 'name'
                  properties: {
                    id: 'a${true ? 'b' : existing}' // passes as long as existing is anywhere in the expression
                    twoid: 'a${true ? 'b' : existing.id}' // passes as long as existing is anywhere in the expression
                    threeid: 'a${true ? 'b' : existing.properties}' // passes as long as existing is anywhere in the expression
                  }
                  location: 'location'
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }",
            new object[]
            {
                // pass
            },
            DisplayName = "symbolic name - doesn't need to reference the id function - pass")]
        [DataRow(@"
/*
{
    ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
    ""contentVersion"": ""1.0.0.0"",
    ""parameters"": {
        ""connections_azuremonitorlogs_name"": {
            ""type"": ""string""
        },
        ""location"": {
            ""type"": ""string""
        },
        ""resourceTags"": {
            ""type"": ""object""
        },
        ""tenantId"": {
            ""type"": ""string""
        }
    },
    ""functions"": [],
    ""variables"": {},
    ""resources"": [
        {
            ""type"": ""Microsoft.Web/connections"",
            ""apiVersion"": ""2016-06-01"",
            ""name"": ""[parameters('connections_azuremonitorlogs_name')]"",
            ""location"": ""[parameters('location')]"",
            ""tags"": ""[parameters('resourceTags')]"",
            ""properties"": {
                ""displayName"": ""azuremonitorlogs"",
                ""statuses"": [
                    {
                        ""status"": ""Connected""
                    }
                ],
                ""nonSecretParameterValues"": {
                    ""token:TenantId"": ""[parameters('tenantId')]"",
                    ""token:grantType"": ""code""
                },
                ""api"": {
                    ""name"": ""[parameters('connections_azuremonitorlogs_name')]"",
                    ""displayName"": ""Azure Monitor Logs"",
                    ""description"": ""Use this connector to query your Azure Monitor Logs across Log Analytics workspace and Application Insights component, to list or visualize results."",
                    ""iconUri"": ""[concat('https://connectoricons-prod.azureedge.net/releases/v1.0.1501/1.0.1501.2507/', parameters('connections_azuremonitorlogs_name'), '/icon.png')]"",
                    ""brandColor"": ""#0072C6"",
                    ""id"": ""[concat('/subscriptions/<subscription_id_here>/providers/Microsoft.Web/locations/<region_here>/managedApis/', parameters('connections_azuremonitorlogs_name'))]"",
                    ""type"": ""Microsoft.Web/locations/managedApis""
                }
            }
        }
    ],
    ""outputs"": {}
}
*/
                @description('description')
                param connections_azuremonitorlogs_name string

                @description('description')
                param location string

                @description('description')
                param resourceTags object
                param tenantId string

                resource connections_azuremonitorlogs_name_resource 'Microsoft.Web/connections@2016-06-01' = {
                  name: connections_azuremonitorlogs_name
                  location: location
                  tags: resourceTags
                  properties: {
                    displayName: 'azuremonitorlogs'
                    statuses: [
                      {
                        status: 'Connected'
                      }
                    ]
                    nonSecretParameterValues: {
                      'token:TenantId': tenantId
                      'token:grantType': 'code'
                    }
                    api: {
                      name: connections_azuremonitorlogs_name
                      displayName: 'Azure Monitor Logs'
                      description: 'Use this connector to query your Azure Monitor Logs across Log Analytics workspace and Application Insights component, to list or visualize results.'
                      iconUri: 'https://connectoricons-prod.azureedge.net/releases/v1.0.1501/1.0.1501.2507/${connections_azuremonitorlogs_name}/icon.png'
                      brandColor: '#0072C6'
                      id: '/subscriptions/<subscription_id_here>/providers/Microsoft.Web/locations/<region_here>/managedApis/${connections_azuremonitorlogs_name}'

                      // correct:
                      //   id: subscriptionResourceId('Microsoft.Web/locations/managedApis', location, connections_azuremonitorlogs_name)

                      type: 'Microsoft.Web/locations/managedApis'
                    }
                  }
                }
                ",
            new object[]
            {
                // TTK results:
                // [-] IDs Should Be Derived From ResourceIDs
                // Property: "id" must use one of the following expressions for an resourceId property:
                //   extensionResourceId,resourceId,subscriptionResourceId,tenantResourceId,if,parameters,reference,variables,subscription,guid
                "[86:23] If property \"id\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId.",
            },
            DisplayName = "https://github.com/Azure/arm-ttk/issues/497")]
        [DataRow(@"
                resource storageaccount 'ms.something/else@2021-02-01' = {
                  name: 'name'
                  properties: {
                    id: concat(subscription().id,'/resourceGroups/',resourceGroup().name,  '/providers/Microsoft.ManagedIdentity/userAssignedIdentities?api-version=2018-11-30')
                  }
                  location: 'location'
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ",
            new object[]
            {
                "[5:21] If property \"id\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId."
            },
            DisplayName = "concat")]
        [DataRow(@"
                resource name 'a.b/c@2021-09-01' = [for (item, index) in range(0, 10): {
                  name: 'name'
                  properties: {
                    failid: 'bad/id'
                  }
                }]
            ",
            new object[]
            {
                "[5:21] If property \"failid\" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId."
            },
            DisplayName = "loops")]
        [DataRow(@"
                resource applicationGateway 'Microsoft.Network/applicationGateways@2020-11-01' = {
                  name: 'name'
                #disable-next-line no-loc-expr-outside-params
                  location: resourceGroup().location
                  properties: {
                    id: 'hello'
                    sku: {
                      name: 'Standard_Small'
                      tier: 'Standard'
                      capacity: 'capacity'
                    }
                    gatewayIPConfigurations: [
                      {
                        name: 'name'
                        properties: {
                          subnet: {
                            // Error: NSGName is not defined
                            id: '/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Network/networkSecurityGroups/${NSGName}'
                          }
                        }
                      }
                    ]
                  }
                }"
            ,
            new object[]
            {
            "[19:173] The name \"NSGName\" does not exist in the current context."
            },
            DisplayName = "undefined variable error")]
        [DataRow(@"
            param location string = resourceGroup().location
            param vnetIPPrefix string = '10.0.0.0/16'
            param containerInstancesSubnetIPPrefix string = '10.0.0.0/24'
            param applicationGatewaySubnetIPPrefix string = '10.0.1.0/24'

            var containerGroupNetworkProfileName = 'aci-networkprofile'
            var containerGroupNetworkProfileInterfaceName = 'eth0'
            var containerGroupNetworkProfileInterfaceIPConfigurationName = 'ipconfigprofile1'
            var vnetName = 'VNet'
            var containerGroupSubnetName = 'Containers'
            var applicationGatewaySubnetName = 'ApplicationGateway'
            var nsgName = 'MyNSG'

            resource containerGroupNetworkProfile 'Microsoft.Network/networkProfiles@2021-02-01' = {
              name: containerGroupNetworkProfileName
              location: location
              properties: {
                containerNetworkInterfaceConfigurations: [
                  {
                    name: containerGroupNetworkProfileInterfaceName
                    properties: {
                      ipConfigurations: [
                        {
                          name: containerGroupNetworkProfileInterfaceIPConfigurationName
                          properties: {
                            subnet: {
                              id: vnet::containerGroupSubnet.id
                            }
                          }
                        }
                      ]
                    }
                  }
                ]
              }
            }

            resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
              name: vnetName
              location: location
              properties: {
                addressSpace: {
                  addressPrefixes: [
                    vnetIPPrefix
                  ]
                }
                subnets: [
                  {
                    name: containerGroupSubnetName
                    properties: {
                      addressPrefix: containerInstancesSubnetIPPrefix
                      delegations: [
                        {
                          name: 'DelegationService'
                          properties: {
                            serviceName: 'Microsoft.ContainerInstance/containerGroups'
                          }
                        }
                      ]
                    }
                  }
                  {
                    name: applicationGatewaySubnetName
                    properties: {
                      addressPrefix: applicationGatewaySubnetIPPrefix
                      networkSecurityGroup: {
                        id: nsg.id
                      }
                    }
                  }
                ]
              }

              resource containerGroupSubnet 'subnets' existing = {
                name: containerGroupSubnetName
              }

              resource applicationGatewaySubnet 'subnets' existing = {
                name: applicationGatewaySubnetName
              }
            }

            resource nsg 'Microsoft.Network/networkSecurityGroups@2020-05-01' = {
              name: nsgName
              location: location
              properties: {
                securityRules: [
                  {
                    name: 'Allow_Front_Door_to_send_HTTP_traffic'
                    properties: {
                      protocol: 'Tcp'
                      sourcePortRange: '*'
                      destinationPortRanges: [
                        '80'
                        '443'
                      ]
                      sourceAddressPrefix: 'AzureFrontDoor.Backend'
                      destinationAddressPrefix: 'VirtualNetwork'
                      access: 'Allow'
                      priority: 120
                      direction: 'Inbound'
                    }
                  }

                  // Rules for Application Gateway as documented here: https://docs.microsoft.com/en-us/azure/application-gateway/application-gateway-faq
                  {
                    name: 'Allow_GWM'
                    properties: {
                      protocol: '*'
                      sourcePortRange: '*'
                      destinationPortRange: '65200-65535'
                      sourceAddressPrefix: 'GatewayManager'
                      destinationAddressPrefix: '*'
                      access: 'Allow'
                      priority: 100
                      direction: 'Inbound'
                    }
                  }
                  {
                    name: 'Allow_AzureLoadBalancer'
                    properties: {
                      protocol: '*'
                      sourcePortRange: '*'
                      destinationPortRange: '*'
                      sourceAddressPrefix: 'AzureLoadBalancer'
                      destinationAddressPrefix: '*'
                      access: 'Allow'
                      priority: 110
                      direction: 'Inbound'
                    }
                  }
                ]
              }
            }
            ",
            new object[]
            {
                // pass
            },
            DisplayName = "subnets")]
        [DataRow(@"
            param location string = resourceGroup().location
            param vnetIPPrefix string = '10.0.0.0/16'
            param containerInstancesSubnetIPPrefix string = '10.0.0.0/24'
            param applicationGatewaySubnetIPPrefix string = '10.0.1.0/24'

            var containerGroupNetworkProfileName = 'aci-networkprofile'
            var containerGroupNetworkProfileInterfaceName = 'eth0'
            var containerGroupNetworkProfileInterfaceIPConfigurationName = 'ipconfigprofile1'
            var vnetName = 'VNet'
            var containerGroupSubnetName = 'Containers'
            var applicationGatewaySubnetName = 'ApplicationGateway'
            var nsgName = 'MyNSG'

            resource containerGroupNetworkProfile 'Microsoft.Network/networkProfiles@2021-02-01' = {
              name: containerGroupNetworkProfileName
              location: location
              properties: {
                containerNetworkInterfaceConfigurations: [
                  {
                    name: containerGroupNetworkProfileInterfaceName
                    properties: {
                      ipConfigurations: [
                        {
                          name: containerGroupNetworkProfileInterfaceIPConfigurationName
                          properties: {
                            subnet: {
                              id: vnet::containerGroupSubnet.id
                            }
                          }
                        }
                      ]
                    }
                  }
                ]
              }
            }

            resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
              name: vnetName
              location: location
              properties: {
                addressSpace: {
                  addressPrefixes: [
                    vnetIPPrefix
                  ]
                }
                subnets: [
                  {
                    name: containerGroupSubnetName
                    properties: {
                      addressPrefix: containerInstancesSubnetIPPrefix
                      delegations: [
                        {
                          name: 'DelegationService'
                          properties: {
                            serviceName: 'Microsoft.ContainerInstance/containerGroups'
                          }
                        }
                      ]
                    }
                  }
                  {
                    name: applicationGatewaySubnetName
                    properties: {
                      addressPrefix: applicationGatewaySubnetIPPrefix
                      networkSecurityGroup: {
                        id: nsg.id
                      }
                    }
                  }
                ]
              }

              resource containerGroupSubnet 'subnets' existing = {
                name: containerGroupSubnetName
              }

              resource applicationGatewaySubnet 'subnets' existing = {
                name: applicationGatewaySubnetName
              }
            }

            resource nsg 'Microsoft.Network/networkSecurityGroups@2020-05-01' = {
              name: nsgName
              location: location
              properties: {
                securityRules: [
                  {
                    name: 'Allow_Front_Door_to_send_HTTP_traffic'
                    properties: {
                      protocol: 'Tcp'
                      sourcePortRange: '*'
                      destinationPortRanges: [
                        '80'
                        '443'
                      ]
                      sourceAddressPrefix: 'AzureFrontDoor.Backend'
                      destinationAddressPrefix: 'VirtualNetwork'
                      access: 'Allow'
                      priority: 120
                      direction: 'Inbound'
                    }
                  }

                  // Rules for Application Gateway as documented here: https://docs.microsoft.com/en-us/azure/application-gateway/application-gateway-faq
                  {
                    name: 'Allow_GWM'
                    properties: {
                      protocol: '*'
                      sourcePortRange: '*'
                      destinationPortRange: '65200-65535'
                      sourceAddressPrefix: 'GatewayManager'
                      destinationAddressPrefix: '*'
                      access: 'Allow'
                      priority: 100
                      direction: 'Inbound'
                    }
                  }
                  {
                    name: 'Allow_AzureLoadBalancer'
                    properties: {
                      protocol: '*'
                      sourcePortRange: '*'
                      destinationPortRange: '*'
                      sourceAddressPrefix: 'AzureLoadBalancer'
                      destinationAddressPrefix: '*'
                      access: 'Allow'
                      priority: 110
                      direction: 'Inbound'
                    }
                  }
                ]
              }
            }
            ",
            new object[]
            {
                // pass
            },
            DisplayName = "subnets")]
        [DataRow(@"
                param cosmosName string
                param cosmosAccountDatabaseScope string

                var devAadGroupIds = [
                  '25f41063-9986-4445-b0d5-e24b1a370d5e'
                  '621dc9d5-008d-4b72-bfdb-bed87e2a039a'
                ]

                @batchSize(1)
                resource developerRoleAssignments 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2022-05-15' = [for devId in devAadGroupIds: {
                  name: '${cosmosName}/${devId}'
                  properties: {
                    roleDefinitionId: '00000000-0000-0000-0000-000000000001' // Read Access
                    principalId: devId
                    scope: cosmosAccountDatabaseScope
                  }
                }]
            ",
            DisplayName = "Regress #8424"
        )]
        [DataTestMethod]
        public void Test(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, expectedMessages);
        }

        [TestMethod]
        public void AssumeModuleReferencesAreOkay()
        {
            // https://github.com/Azure/bicep/issues/8030

            string bicep = @"
                @description('Deployment Location')
                param location string = resourceGroup().location

                @description('Name of VMSS Cluster')
                param vmssName string

                @description('GameDev Sku')
                param vmssSku string = 'Standard_D4ds_v4'

                @description('GameDev Image Publisher')
                @allowed([
                  'microsoftcorporation1602274591143'
                  'azure-gaming'
                ])
                param vmssImgPublisher string = 'microsoftcorporation1602274591143'

                @description('GameDev Image Product Id')
                @allowed([
                  'game-dev-vm'
                ])
                param vmssImgProduct string = 'game-dev-vm'

                @description('GameDev Image Sku')
                @allowed([
                  'win10_no_engine_1_0'
                  'ws2019_no_engine_1_0'
                  'win10_unreal_4_27_2'
                  'ws2019_unreal_4_27_2'
                  'win10_unreal_5_0_1'
                  'ws2019_unreal_5_0_1'
                ])
                param vmssImgSku string = 'win10_unreal_4_27_2'

                @description('GameDev Image Product Id')
                param vmssImgVersion string = 'latest'

                @description('GameDev Disk Type')
                param vmssOsDiskType string = 'Premium_LRS'

                @description('VMSS Instance Count')
                @maxValue(100)
                @minValue(1)
                param vmssInstanceCount int = 1

                @description('Administrator Login for access')
                param administratorLogin string

                @description('Administrator Password for access')
                @secure()
                param passwordAdministratorLogin string

                @description('File Share Storage Account name')
                param fileShareStorageAccount string = ''

                @description('File Share Storage Account key')
                @secure()
                param fileShareStorageAccountKey string = ''

                @description('File Share name')
                param fileShareName string = ''

                @description('Perforce Port address')
                param p4Port string = ''

                @description('Perforce User')
                param p4Username string = ''

                @description('Perforce User password')
                @secure()
                param p4Password string = ''

                @description('Perforce Client Workspace')
                param p4Workspace string = ''

                @description('Perforce Stream')
                param p4Stream string = ''

                @description('Perforce Depot Client View mappings')
                param p4ClientViews string = ''

                @description('Incredibuild License Key')
                @secure()
                param ibLicenseKey string = ''

                @description('GDK Version')
                param gdkVersion string = 'June_2021_Update_4'

                @description('Use VM to sysprep an image from')
                param useVmToSysprepCustomImage bool = false

                @description('Remote Access technology')
                @allowed([
                  'RDP'
                  'Teradici'
                  'Parsec'
                ])
                param remoteAccessTechnology string = 'RDP'

                @description('Teradici Registration Key')
                @secure()
                param teradiciRegKey string = ''

                @description('Parsec Team ID')
                param parsecTeamId string = ''

                @description('Parsec Team Key')
                @secure()
                param parsecTeamKey string = ''

                @description('Parsec Hostname')
                param parsecHost string = ''

                @description('Parsec User Email')
                param parsecUserEmail string = ''

                @description('Parsec Is Guest Access')
                param parsecIsGuestAccess bool = false

                @description('Virtual Network Resource Name')
                param vnetName string = 'vnet-${vmssName}'

                @description('Virtual Network Subnet Name')
                param subnetName string = 'subnet${vmssName}'

                @description('Virtual Network Security Group Name')
                param networkSecurityGroupName string = 'nsg-${vmssName}'

                @description('Virtual Network Address Prefix')
                param vnetAddressPrefix string = '172.17.72.0/24' //Change as needed

                @description('Virtual Network Subnet Address Prefix')
                param subnetAddressPrefix string = '172.17.72.0/25' // 172.17.72.[0-128] is part of this subnet

                var customData = format('''
                fileShareStorageAccount={0}
                fileShareStorageAccountKey={1}
                fileShareName={2}
                p4Port={3}
                p4Username={4}
                p4Password={5}
                p4Workspace={6}
                p4Stream={7}
                p4ClientViews={8}
                ibLicenseKey={9}
                gdkVersion={10}
                useVmToSysprepCustomImage={11}
                remoteAccessTechnology={12}
                teradiciRegKey={13}
                parsecTeamId={14}
                parsecTeamKey={15}
                parsecHost={16}
                parsecUserEmail={17}
                parsecIsGuestAccess={18}
                deployedFromSolutionTemplate={19}
                ''', fileShareStorageAccount, fileShareStorageAccountKey, fileShareName, p4Port, p4Username, p4Password, p4Workspace, p4Stream, p4ClientViews, ibLicenseKey, gdkVersion, useVmToSysprepCustomImage, remoteAccessTechnology, teradiciRegKey, parsecTeamId, parsecTeamKey, parsecHost, parsecUserEmail, parsecIsGuestAccess, false)


                module vnet './nestedtemplates/virtualNetworks.bicep'  = {
                  name:                       vnetName
                  params: {
                    location:                 location
                    vnetName:                 vnetName
                    subnetName:               subnetName
                    networkSecurityGroupName: networkSecurityGroupName
                    vnetAddressPrefix:        vnetAddressPrefix
                    subnetAddressPrefix:      subnetAddressPrefix
                  }
                }

                resource vmss 'Microsoft.Compute/virtualMachineScaleSets@2021-04-01' = {
                  name: vmssName
                  location: location
                  sku: {
                    name:     vmssSku
                    tier:     'Standard'
                    capacity: vmssInstanceCount
                  }
                  plan: {
                    name:      vmssImgSku
                    publisher: vmssImgPublisher
                    product:   vmssImgProduct
                  }
                  properties: {
                    singlePlacementGroup: false
                    upgradePolicy: {
                      mode: 'Manual'
                    }
                    virtualMachineProfile: {
                      storageProfile: {
                        osDisk: {
                          createOption: 'FromImage'
                          caching: 'ReadWrite'
                          managedDisk: {
                            storageAccountType: vmssOsDiskType
                          }
                        }
                        imageReference: {
                          publisher: vmssImgPublisher
                          offer:     vmssImgProduct
                          sku:       vmssImgSku
                          version:   vmssImgVersion
	                }
                      }
                      networkProfile: {
                        networkInterfaceConfigurations: [
                          {
                            name: '${vmssName}Nic'
                            properties: {
                              primary: true
                              ipConfigurations: [
                                {
                                  name: '${vmssName}IpConfig'
                                  properties: {
                                    subnet: {
                                      id: vnet.outputs.subnetId
                                    }
                                  }
                                }
                              ]
                              networkSecurityGroup: {
                                id: vnet.outputs.nsgID
                              }
                            }
                          }
                        ]
                      }
                      osProfile: {
                        computerNamePrefix: vmssName
                        adminUsername:      administratorLogin
                        adminPassword:      passwordAdministratorLogin
	                customData:         base64(customData)
	                windowsConfiguration: {
                          provisionVMAgent: true
                        }
                      }
                      priority: 'Regular'
                    }
                    overprovision: false
                  }
                }

                output id   string = vmss.id
                output name string = vmss.name
            ";
            CompileAndTest(
                bicep,
                new Options(
                    AdditionalFiles: new[]
                    {
                        (
                        "nestedtemplates/virtualNetworks.bicep",
                        @"
                            param location string
                            param vnetName string
                            param subnetName string
                            param networkSecurityGroupName string
                            param vnetAddressPrefix string
                            param subnetAddressPrefix string

                            output subnetId string = 'id'
                            output nsgID string = 'name'
                        "
                        )
                    }),
                expectedMessages: new string[]
                {
                    // pass
                });
        }

        [TestMethod]
        public void Regress8410()
        {
            CompileAndTest(
                @"
                    targetScope = 'tenant'

                    param parTopLevelManagementGroupPrefix string
                    param parTopLevelManagementGroupDisplayName string
                    param parTopLevelManagementGroupParentId string

                    resource resTopLevelMg 'Microsoft.Management/managementGroups@2021-04-01' = {
                      name: parTopLevelManagementGroupPrefix
                      properties: {
                        displayName: parTopLevelManagementGroupDisplayName
                        details: {
                          parent: {
                    #disable-next-line BCP037
                            oneId: (empty(parTopLevelManagementGroupParentId) ? '/providers/Microsoft.Management/managementGroups/${tenant().tenantId}' : parTopLevelManagementGroupParentId)

                    #disable-next-line BCP037
                            twoId: empty(parTopLevelManagementGroupParentId) ? '/providers/Microsoft.Management/managementGroups/${tenant().tenantId}' : parTopLevelManagementGroupParentId
                          }
                        }
                      }
                    }
",
                expectedMessages: new string[]
                {
                    // asdfg
                });
        }
    }
}
