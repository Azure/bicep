// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class SecureParamsInNestedDeploymentsRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string bicep, string[] expectedMessages)
        {
            AssertLinterRuleDiagnostics(
                SecureParamsInNestedDeploymentsRule.Code,
                bicep,
                expectedMessages,
                new Options(
                    OnCompileErrors: OnCompileErrors.IncludeErrorsAndWarnings,
                    IncludePosition: IncludePosition.LineNumber));
        }

        [TestMethod]
        public void SecureParams_AndListKeys_ButExplicitlyInnerScope_Pass()
        {
            // https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-test-cases#use-inner-scope-for-nested-deployment-secure-parameters
            CompileAndTest(@"
                 @secure()
                    param stgAccountName string

                    resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                      name: 'nested'
                      properties: {
                        mode: 'Incremental'
                        expressionEvaluationOptions: {
                          scope: 'inner'
                        }
                        template: {
                          '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                          contentVersion: '1.0.0.0'
                          resources: [
                            {
                              name: stgAccountName
                              type: 'Microsoft.Storage/storageAccounts'
                              apiVersion: '2021-04-01'
                              location: format('{0}', listKeys('someResourceId', '2020-01-01'))
                              kind: 'StorageV2'
                              sku: {
                                name: 'Premium_LRS'
                                tier: 'Premium'
                              }
                            }
                          ]
                        }
                      }
                    }",
                new string[]
                {
                });
        }

        [TestMethod]
        public void SecureParams_AndListKeys_ButExplicitlyInnerScope_CaseInsensitive_Pass()
        {
            // https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-test-cases#use-inner-scope-for-nested-deployment-secure-parameters
            CompileAndTest(@"
                 @secure()
                    param stgAccountName string

                    resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                      name: 'nested'
                      properties: {
                        mode: 'Incremental'
                        expressionEvaluationOptions: {
                          scope: 'INNER'
                        }
                        template: {
                          '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                          contentVersion: '1.0.0.0'
                          resources: [
                            {
                              name: stgAccountName
                              type: 'Microsoft.Storage/storageAccounts'
                              apiVersion: '2021-04-01'
                              #disable-next-line no-loc-expr-outside-params
                              location: format('{0}', listKeys('someResourceId', '2020-01-01'))
                              kind: 'StorageV2'
                              sku: {
                                name: 'Premium_LRS'
                                tier: 'Premium'
                              }
                            }
                          ]
                        }
                      }
                    }",
                new string[]
                {
                });
        }

        [TestMethod]
        public void ExplicitlyOuterScope_ButNoSecureParams_OrListKeys_Pass()
        {
            // https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-test-cases#use-inner-scope-for-nested-deployment-secure-parameters
            CompileAndTest(@"
                    param stgAccountName string

                    resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                      name: 'nested'
                      properties: {
                        mode: 'Incremental'
                        expressionEvaluationOptions: {
                          scope: 'outer'
                        }
                        template: {
                          '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                          contentVersion: '1.0.0.0'
                          resources: [
                            {
                              name: stgAccountName
                              type: 'Microsoft.Storage/storageAccounts'
                              apiVersion: '2021-04-01'
                              #disable-next-line no-loc-expr-outside-params
                              location: resourceGroup().location
                              kind: 'StorageV2'
                              sku: {
                                name: 'Premium_LRS'
                                tier: 'Premium'
                              }
                            }
                          ]
                        }
                      }
                    }",
                new string[]
                {
                });
        }

        [TestMethod]
        public void ImplicitlyOuterScope_ButNoSecureParams_OrListKeys_Pass()
        {
            // https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-test-cases#use-inner-scope-for-nested-deployment-secure-parameters
            CompileAndTest(@"
                    param stgAccountName string

                    resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                      name: 'nested'
                      properties: {
                        // Scope defaults to outer
                        mode: 'Incremental'
                        template: {
                          '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                          contentVersion: '1.0.0.0'
                          resources: [
                            {
                              name: stgAccountName
                              type: 'Microsoft.Storage/storageAccounts'
                              apiVersion: '2021-04-01'
                              #disable-next-line no-loc-expr-outside-params
                              location: resourceGroup().location
                              kind: 'StorageV2'
                              sku: {
                                name: 'Premium_LRS'
                                tier: 'Premium'
                              }
                            }
                          ]
                        }
                      }
                    }",
                new string[]
                {
                });
        }

        [TestMethod]
        public void ExplicitlyOuterScope_AndSecureParams_Fail()
        {
            /* TTK result:
              [-] Secure Params In Nested Deployments
                  Microsoft.Resources/deployments/nested is an outer scope nested deployment that contains a secureString type parameter: "stgAccountName"
            */
            // https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-test-cases#use-inner-scope-for-nested-deployment-secure-parameters
            CompileAndTest(@"
                    @secure()
                    param stgAccountName2 string

                    resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                      name: 'nested'
                      properties: {
                        mode: 'Incremental'
                        expressionEvaluationOptions: {
                          scope: 'outer'
                        }
                        template: {
                          '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                          contentVersion: '1.0.0.0'
                          resources: [
                            {
                              name: stgAccountName2
                              type: 'Microsoft.Storage/storageAccounts'
                              apiVersion: '2021-04-01'
                              #disable-next-line no-loc-expr-outside-params
                              location: resourceGroup().location
                              kind: 'StorageV2'
                              sku: {
                                name: 'Premium_LRS'
                                tier: 'Premium'
                              }
                            }
                          ]
                        }
                      }
                    }
                ",
                new[]
                {
                    "[5] 'nested' is an outer scoped nested deployment that accesses secure string parameters ('stgAccountName2'), which could expose their values in deployment history. Either set the deployment's properties.expressionEvaluationOptions.scope to 'inner' or use a Bicep module instead."
                });
        }

        [TestMethod]
        public void ImplicitlyOuterScope_BecauseScopeMissing_AndSecureParam_Fail()
        {
            // https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-test-cases#use-inner-scope-for-nested-deployment-secure-parameters
            CompileAndTest(@"
                    @secure()
                    param stgAccountName2 string

                    resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                      name: 'nested'
                      properties: {
                        mode: 'Incremental'
                            expressionEvaluationOptions: {
                        }
                        template: {
                          '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                          contentVersion: '1.0.0.0'
                          resources: [
                            {
                              name: stgAccountName2
                              type: 'Microsoft.Storage/storageAccounts'
                              apiVersion: '2021-04-01'
                              #disable-next-line no-loc-expr-outside-params
                              location: resourceGroup().location
                              kind: 'StorageV2'
                              sku: {
                                name: 'Premium_LRS'
                                tier: 'Premium'
                              }
                            }
                          ]
                        }
                      }
                    }
                ",
                new[]
                {
                    "[5] 'nested' is an outer scoped nested deployment that accesses secure string parameters ('stgAccountName2'), which could expose their values in deployment history. Either set the deployment's properties.expressionEvaluationOptions.scope to 'inner' or use a Bicep module instead."
                });
        }


        [TestMethod]
        public void ImplicitlyOuterScope_BecauseEvalOptionsMissing_AndSecureParam_Fail()
        {
            /* TTK result:

                    [-] Secure Params In Nested Deployments
                        Microsoft.Resources/deployments/nested is an outer scope nested deployment that contains a secureString type parameter: "stgAccountName"

            */
            // https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-test-cases#use-inner-scope-for-nested-deployment-secure-parameters
            CompileAndTest(@"
                @secure()
                #disable-next-line secure-parameter-default
                param stgAccountName string

                resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                    name: 'nested'
                    properties: {
                        mode: 'Incremental'
                        template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            resources: [
                                {
                                    name: stgAccountName
                                    type: 'Microsoft.Storage/storageAccounts'
                                    apiVersion: '2021-04-01'
                                    #disable-next-line no-loc-expr-outside-params
                                    location: resourceGroup().location
                                    kind: 'StorageV2'
                                    sku: {
                                        name: 'Premium_LRS'
                                        tier: 'Premium'
                                    }
                                }
                            ]
                        }
                    }
                }",
                new[]
                {
                    "[6] 'nested' is an outer scoped nested deployment that accesses secure string parameters ('stgAccountName'), which could expose their values in deployment history. Either set the deployment's properties.expressionEvaluationOptions.scope to 'inner' or use a Bicep module instead."
                });
        }

        [TestMethod]
        public void ImplicitlyOuterScope_BecausePropertiesMissing_AndSecureParams_Ignore()
        {
            // https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-test-cases#use-inner-scope-for-nested-deployment-secure-parameters
            CompileAndTest(@"
                @secure()
                #disable-next-line secure-parameter-default
                param stgAccountName string

                resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                    name: 'nested'
                    propertiesWhoopsMisspelled: {
                        mode: 'Incremental'
                        template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            resources: [
                                {
                                    name: stgAccountName
                                    type: 'Microsoft.Storage/storageAccounts'
                                    apiVersion: '2021-04-01'
                                    #disable-next-line no-loc-expr-outside-params
                                    location: resourceGroup().location
                                    kind: 'StorageV2'
                                    sku: {
                                        name: 'Premium_LRS'
                                        tier: 'Premium'
                                    }
                                }
                            ]
                        }
                    }
                }",
                 new string[]
                 {
                     "[6] The specified \"resource\" declaration is missing the following required properties: \"properties\". If this is an inaccuracy in the documentation, please report it to the Bicep Team.",
                     "[8] The property \"propertiesWhoopsMisspelled\" is not allowed on objects of type \"Microsoft.Resources/deployments\". Permissible properties include \"dependsOn\", \"location\", \"properties\", \"resourceGroup\", \"scope\", \"subscriptionId\", \"tags\". If this is an inaccuracy in the documentation, please report it to the Bicep Team.",
                 });
        }

        [TestMethod]
        public void TemplateLink_AndSecureParams_Pass()
        {
            // https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-test-cases#use-inner-scope-for-nested-deployment-secure-parameters
            CompileAndTest(@"
                    @secure()
                    param abc string

                    resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                      name: 'nested'
                      properties: {
                        mode: 'Incremental'
                        expressionEvaluationOptions: {
                          scope: 'outer'
                        }
                        templateLink: {
                          uri: 'https://microsoft.com/${abc}'
                        }
                      }
                    }",
                 new string[]
                 {
                 });
        }

        [TestMethod]
        public void ExplicitlyOuter_WithQuotedProperties_AndSecureParams_Fail()
        {
            // https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-test-cases#use-inner-scope-for-nested-deployment-secure-parameters
            CompileAndTest(@"
                    @secure()
                    param stgAccountName string

                    resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                      'name': 'nested'
                      'properties': {
                        'mode': 'Complete'
                        'expressionEvaluationOptions': {
                          'scope': 'outer'
                        }
                        'template': {
                          '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                          'contentVersion': '1.0.0.0'
                          'resources': [
                            {
                              'name': stgAccountName
                              'type': 'Microsoft.Storage/storageAccounts'
                              'apiVersion': '2021-04-01'
                              #disable-next-line no-loc-expr-outside-params
                              'location': resourceGroup().location
                              'kind': 'StorageV2'
                              'sku': {
                                'name': 'Premium_LRS'
                                'tier': 'Premium'
                              }
                            }
                          ]
                        }
                      }
                    }
                ",
                new[]
                {
                    "[5] 'nested' is an outer scoped nested deployment that accesses secure string parameters ('stgAccountName'), which could expose their values in deployment history. Either set the deployment's properties.expressionEvaluationOptions.scope to 'inner' or use a Bicep module instead.",
                });
        }

        [TestMethod]
        public void NotDeployment_Pass()
        {
            // https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-test-cases#use-inner-scope-for-nested-deployment-secure-parameters
            CompileAndTest(@"
                @secure()
                #disable-next-line secure-parameter-default
                param stgAccountName string

                resource nested 'Microsoft.Resources/notDeployments@2021-04-01' = {
                    name: 'nested'
                    properties: {
                        mode: 'Incremental'
                        template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            resources: [
                                {
                                    name: stgAccountName
                                    type: 'Microsoft.Storage/storageAccounts'
                                    apiVersion: '2021-04-01'
                                    #disable-next-line no-loc-expr-outside-params
                                    location: resourceGroup().location
                                    kind: 'StorageV2'
                                    sku: {
                                        name: 'Premium_LRS'
                                        tier: 'Premium'
                                    }
                                }
                            ]
                        }
                    }
                }",
                new string[]
                {
                    "[6] Resource type \"Microsoft.Resources/notDeployments@2021-04-01\" does not have types available."
                });
        }

        [TestMethod]
        public void SecureParams_ButNoneReferencedInDeployment_Pass()
        {
            CompileAndTest(@"
                        @secure()
                        param stgAccountName string
                        @secure()
                        param stgAccountName2 string

                        #disable-next-line outputs-should-not-contain-secrets
                        output s string = '${stgAccountName}${stgAccountName2}'

                        resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                          name: 'nested'
                          properties: {
                            mode: 'Incremental'
                            template: {
                              '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                              contentVersion: '1.0.0.0'
                              resources: [
                                {
                                  name: 'hello'
                                  type: 'Microsoft.Storage/storageAccounts'
                                  apiVersion: '2021-04-01'
                                  #disable-next-line no-loc-expr-outside-params
                                  location: resourceGroup().location
                                  kind: 'StorageV2'
                                  sku: {
                                    name: 'Premium_LRS'
                                    tier: 'Premium'
                                  }
                                }
                              ]
                            }
                          }
                        }
                        ",
                new string[]
                {
                });
        }

        [TestMethod]
        public void SecureParams_ButOnlySomeReferencedInDeployment_Pass()
        {
            CompileAndTest(@"
                        @secure()
                        param stgAccountName string
                        @secure()
                        param stgAccountName2 string
                        @secure()
                        param stgAccountName3 object

                        #disable-next-line outputs-should-not-contain-secrets
                        output s string = '${stgAccountName}${stgAccountName2}'

                        resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                          name: 'nested'
                          properties: {
                            mode: 'Incremental'
                            template: {
                              '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                              contentVersion: '1.0.0.0'
                              resources: [
                                {
                                  name: '${stgAccountName}${stgAccountName2}'
                                  type: 'Microsoft.Storage/storageAccounts'
                                  apiVersion: '2021-04-01'
                                  #disable-next-line no-loc-expr-outside-params
                                  location: resourceGroup().location
                                  kind: 'StorageV2'
                                  sku: {
                                    name: 'Premium_LRS'
                                    tier: 'Premium'
                                  }
                                }
                              ]
                            }
                          }
                        }
                        ",
                new string[]
                {
                    "[12] 'nested' is an outer scoped nested deployment that accesses secure string parameters ('stgAccountName', 'stgAccountName2'), which could expose their values in deployment history. Either set the deployment's properties.expressionEvaluationOptions.scope to 'inner' or use a Bicep module instead.",
                });
        }

        [TestMethod]
        public void ListKeys_Fail()
        {
            CompileAndTest(@"
                    /*
                        [-] Secure Params In Nested Deployments
                            Microsoft.Resources/deployments/nested is an outer scope nested deployment that contains a list*() function: , listKeys(
                    */
                    resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                      name: 'nested'
                      properties: {
                        mode: 'Complete'
                        template: {
                          '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                          contentVersion: '1.0.0.0'
                          variables: {}
                          resources: [
                            {
                              name: 'outerImplicit'
                              type: 'Microsoft.Network/networkSecurityGroups'
                              apiVersion: '2019-11-01'
                              location: '[resourceGroup().location]'
                              properties: {
                                securityRules: [
                                  {
                                    name: 'outerImplicit'
                                    properties: {
                                      description: format('{0}', listKeys('someResourceId', '2020-01-01'))
                                    }
                                  }
                                ]
                              }
                            }
                          ]
                        }
                      }
                    }                       
                ",
                new string[]
                {
                    "[6] 'nested' is an outer scoped nested deployment that calls a list* function (\"listKeys('someResourceId', '2020-01-01')\"), which could expose sensitive values in deployment history. Either set the deployment's properties.expressionEvaluationOptions.scope to 'inner' or use a Bicep module instead.",
                });
        }

        [TestMethod]
        public void AzListKeys_Fail()
        {
            CompileAndTest(@"
                    /*
                        [-] Secure Params In Nested Deployments
                            Microsoft.Resources/deployments/nested is an outer scope nested deployment that contains a list*() function: , listKeys(
                    */
                    resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                      name: 'nested'
                      properties: {
                        mode: 'Complete'
                        template: {
                          '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                          contentVersion: '1.0.0.0'
                          variables: {}
                          resources: [
                            {
                              name: 'outerImplicit'
                              type: 'Microsoft.Network/networkSecurityGroups'
                              apiVersion: '2019-11-01'
                              location: '[resourceGroup().location]'
                              properties: {
                                securityRules: [
                                  {
                                    name: 'outerImplicit'
                                    properties: {
                                      description: format('{0}', az.listKeys('someResourceId', '2020-01-01'))
                                    }
                                  }
                                ]
                              }
                            }
                          ]
                        }
                      }
                    }                       
                ",
                new string[]
                {
                    "[6] 'nested' is an outer scoped nested deployment that calls a list* function (\"az.listKeys('someResourceId', '2020-01-01')\"), which could expose sensitive values in deployment history. Either set the deployment's properties.expressionEvaluationOptions.scope to 'inner' or use a Bicep module instead.",
                });
        }

        [TestMethod]
        public void FooListKeys_Ignore()
        {
            CompileAndTest(@"
                    resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                      name: 'nested'
                      properties: {
                        mode: 'Complete'
                        template: {
                          '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                          contentVersion: '1.0.0.0'
                          variables: {}
                          resources: [
                            {
                              name: 'outerImplicit'
                              type: 'Microsoft.Network/networkSecurityGroups'
                              apiVersion: '2019-11-01'
                              location: '[resourceGroup().location]'
                              properties: {
                                securityRules: [
                                  {
                                    name: 'outerImplicit'
                                    properties: {
                                      description: format('{0}', foo.listKeys('someResourceId', '2020-01-01'))
                                    }
                                  }
                                ]
                              }
                            }
                          ]
                        }
                      }
                    }                       
                ",
                new string[]
                {
                    "[21] The name \"foo\" does not exist in the current context.",
                });
        }

        [TestMethod]
        public void IsDeployment_CaseInsensitive_Fail()
        {
            // https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-test-cases#use-inner-scope-for-nested-deployment-secure-parameters
            CompileAndTest(@"
                 @secure()
                #disable-next-line secure-parameter-default
                param stgAccountName string

                resource nested 'Microsoft.Resources/DEPLOYMENTS@2021-04-01' = {
                    name: 'nested'
                    properties: {
                        mode: 'Incremental'
                        template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            resources: [
                                {
                                    name: stgAccountName
                                    type: 'Microsoft.Storage/storageAccounts'
                                    apiVersion: '2021-04-01'
                                    #disable-next-line no-loc-expr-outside-params
                                    location: resourceGroup().location
                                    kind: 'StorageV2'
                                    sku: {
                                        name: 'Premium_LRS'
                                        tier: 'Premium'
                                    }
                                }
                            ]
                        }
                    }
                }",
                new[]
                {
                   "[6] 'nested' is an outer scoped nested deployment that accesses secure string parameters ('stgAccountName'), which could expose their values in deployment history. Either set the deployment's properties.expressionEvaluationOptions.scope to 'inner' or use a Bicep module instead.", 
                });
        }

        [TestMethod]
        public void ListKeys_InLoop_Fail()
        {
            CompileAndTest(@"
                    resource nested 'Microsoft.Resources/deployments@2021-04-01' = [for i in range(1, 10): {
                      name: 'nested${i}'
                      properties: {
                        mode: 'Complete'
                        template: {
                          '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                          contentVersion: '1.0.0.0'
                          variables: {}
                          resources: [
                            {
                              name: 'outerImplicit'
                              type: 'Microsoft.Network/networkSecurityGroups'
                              apiVersion: '2019-11-01'
                              location: '[resourceGroup().location]'
                              properties: {
                                securityRules: [
                                  {
                                    name: 'outerImplicit'
                                    properties: {
                                      description: format('{0}', listKeys('someResourceId', '2020-01-01'))
                                    }
                                  }
                                ]
                              }
                            }
                          ]
                        }
                      }
                    }]                       
                ",
                new string[]
                {
                    "[2] 'nested' is an outer scoped nested deployment that calls a list* function (\"listKeys('someResourceId', '2020-01-01')\"), which could expose sensitive values in deployment history. Either set the deployment's properties.expressionEvaluationOptions.scope to 'inner' or use a Bicep module instead.",
                });
        }
    }
}
