targetScope = 'managementGroup'

@maxLength(5)
param topLevelManagementGroupPrefix string

var scope = '/providers/Microsoft.Management/managementGroups/${topLevelManagementGroupPrefix}'
var policies = {
  policyDefinitions: [
    {
          properties: {
            Description: 'Deploys the diagnostic settings for Container Instances to stream to a Log Analytics workspace when any ACR which is missing this diagnostic settings is created or updated. The policy wil set the diagnostic with all metrics enabled.'
        DisplayName: 'Deploy Diagnostic Settings for Container Instances to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.ContainerInstance/containerGroups'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.ContainerInstance/containerGroups/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: []
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-ACI'
    }
    {
      Properties: {
        Description: 'Depoloys a default budget on subscriptions.'
        DisplayName: 'Deploy a default budget on subscriptions'
        Mode: 'All'
        Parameters: {
          amount: {
            type: 'string'
            defaultValue: '1000'
            metadata: {
              description: 'The total amount of cost or usage to track with the budget'
            }
          }
          timeGrain: {
            type: 'string'
            defaultValue: 'Monthly'
            allowedValues: [
              'Monthly'
              'Quarterly'
              'Annually'
              'BillingMonth'
              'BillingQuarter'
              'BillingAnnual'
            ]
            metadata: {
              description: 'The time covered by a budget. Tracking of the amount will be reset based on the time grain.'
            }
          }
          firstThreshold: {
            type: 'string'
            defaultValue: '90'
            metadata: {
              description: 'Threshold value associated with a notification. Notification is sent when the cost exceeded the threshold. It is always percent and has to be between 0 and 1000.'
            }
          }
          secondThreshold: {
            type: 'string'
            defaultValue: '100'
            metadata: {
              description: 'Threshold value associated with a notification. Notification is sent when the cost exceeded the threshold. It is always percent and has to be between 0 and 1000.'
            }
          }
          contactRoles: {
            type: 'array'
            defaultValue: [
              'Owner'
              'Contributor'
            ]
            metadata: {
              description: 'The list of contact RBAC roles, in an array, to send the budget notification to when the threshold is exceeded.'
            }
          }
          contactEmails: {
            type: 'array'
            defaultValue: []
            metadata: {
              description: 'The list of email addresses, in an array, to send the budget notification to when the threshold is exceeded.'
            }
          }
          contactGroups: {
            type: 'array'
            defaultValue: []
            metadata: {
              description: 'The list of action groups, in an array, to send the budget notification to when the threshold is exceeded. It accepts array of strings.'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Budget'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Resources/subscriptions'
              }
            ]
          }
          then: {
            effect: 'DeployIfNotExists'
            details: {
              type: 'Microsoft.Consumption/budgets'
              deploymentScope: 'Subscription'
              existenceScope: 'Subscription'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Consumption/budgets/amount'
                    equals: '[parameters(\'amount\')]'
                  }
                  {
                    field: 'Microsoft.Consumption/budgets/timeGrain'
                    equals: '[parameters(\'timeGrain\')]'
                  }
                  {
                    field: 'Microsoft.Consumption/budgets/category'
                    equals: 'Cost'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
              ]
              deployment: {
                location: 'northeurope'
                properties: {
                  mode: 'incremental'
                  parameters: {
                    amount: {
                      value: '[parameters(\'amount\')]'
                    }
                    timeGrain: {
                      value: '[parameters(\'timeGrain\')]'
                    }
                    firstThreshold: {
                      value: '[parameters(\'firstThreshold\')]'
                    }
                    secondThreshold: {
                      value: '[parameters(\'secondThreshold\')]'
                    }
                    contactEmails: {
                      value: '[parameters(\'contactEmails\')]'
                    }
                    contactRoles: {
                      value: '[parameters(\'contactRoles\')]'
                    }
                    contactGroups: {
                      value: '[parameters(\'contactGroups\')]'
                    }
                  }
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      amount: {
                        type: 'string'
                      }
                      timeGrain: {
                        type: 'string'
                      }
                      firstThreshold: {
                        type: 'string'
                      }
                      secondThreshold: {
                        type: 'string'
                      }
                      contactEmails: {
                        type: 'array'
                      }
                      contactRoles: {
                        type: 'array'
                      }
                      contactGroups: {
                        type: 'array'
                      }
                      startDate: {
                        type: 'string'
                        defaultValue: '[concat(utcNow(\'MM\'), \'/01/\', utcNow(\'yyyy\'))]'
                      }
                    }
                    resources: [
                      {
                        type: 'Microsoft.Consumption/budgets'
                        apiVersion: '2019-10-01'
                        name: 'default-sandbox-budget'
                        properties: {
                          timePeriod: {
                            startDate: '[parameters(\'startDate\')]'
                          }
                          timeGrain: '[parameters(\'timeGrain\')]'
                          amount: '[parameters(\'amount\')]'
                          category: 'Cost'
                          notifications: {
                            NotificationForExceededBudget1: {
                              enabled: true
                              operator: 'GreaterThan'
                              threshold: '[parameters(\'firstThreshold\')]'
                              contactEmails: '[parameters(\'contactEmails\')]'
                              contactRoles: '[parameters(\'contactRoles\')]'
                              contactGroups: '[parameters(\'contactGroups\')]'
                            }
                            NotificationForExceededBudget2: {
                              enabled: true
                              operator: 'GreaterThan'
                              threshold: '[parameters(\'secondThreshold\')]'
                              contactEmails: '[parameters(\'contactEmails\')]'
                              contactRoles: '[parameters(\'contactRoles\')]'
                              contactGroups: '[parameters(\'contactGroups\')]'
                            }
                          }
                        }
                      }
                    ]
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Budget'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Container Registry to stream to a Log Analytics workspace when any ACR which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics  enabled.'
        DisplayName: 'Deploy Diagnostic Settings for Container Registry to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.ContainerRegistry/registries'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.ContainerRegistry/registries/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'ContainerRegistryLoginEvents'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'ContainerRegistryRepositoryEvents'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-ACR'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Activity Log to stream to a Log Analytics workspace when any Activity Log which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with category enabled.'
        DisplayName: 'Deploy Diagnostic Settings for Activity Log to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Primary Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Resources/subscriptions'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              deploymentScope: 'Subscription'
              existenceScope: 'Subscription'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              deployment: {
                location: 'northeurope'
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      logAnalytics: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        name: 'subscriptionToLa'
                        type: 'Microsoft.Insights/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        location: 'Global'
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          logs: [
                            {
                              category: 'Administrative'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Security'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'ServiceHealth'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Alert'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Recommendation'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Policy'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Autoscale'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'ResourceHealth'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-ActivityLog'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Kubernetes Service to stream to a Log Analytics workspace when any Kubernetes Service which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled.'
        DisplayName: 'Deploy Diagnostic Settings for Kubernetes Service to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.ContainerService/managedClusters'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.ContainerService/managedClusters/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'kube-audit'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'kube-apiserver'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'kube-controller-manager'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'kube-scheduler'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'cluster-autoscaler'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'guard'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'kube-audit-admin'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-AKS'
    }
    {
      Properties: {
        Description: 'This policy deploys virtual network and peer to the hub'
        DisplayName: 'Deploys virtual network peering to hub'
        Mode: 'All'
        Parameters: {
          vNetName: {
            type: 'String'
            metadata: {
              displayName: 'vNetName'
              description: 'Name of the landing zone vNet'
            }
          }
          vNetRgName: {
            type: 'String'
            metadata: {
              displayName: 'vNetRgName'
              description: 'Name of the landing zone vNet RG'
            }
          }
          vNetLocation: {
            type: 'String'
            metadata: {
              displayName: 'vNetLocation'
              description: 'Location for the vNet'
            }
          }
          vNetCidrRange: {
            type: 'String'
            metadata: {
              displayName: 'vNetCidrRange'
              description: 'CIDR Range for the vNet'
            }
          }
          hubResourceId: {
            type: 'String'
            metadata: {
              displayName: 'hubResourceId'
              description: 'Resource ID for the HUB vNet'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Resources/subscriptions'
              }
            ]
          }
          then: {
            effect: 'deployIfNotExists'
            details: {
              type: 'Microsoft.Network/virtualNetworks'
              name: '[parameters(\'vNetName\')]'
              deploymentScope: 'Subscription'
              existenceScope: 'ResourceGroup'
              ResourceGroupName: '[parameters(\'vNetRgName\')]'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
              ]
              existenceCondition: {
                allOf: [
                  {
                    field: 'name'
                    like: '[parameters(\'vNetName\')]'
                  }
                  {
                    field: 'location'
                    equals: '[parameters(\'vNetLocation\')]'
                  }
                ]
              }
              deployment: {
                location: 'northeurope'
                properties: {
                  mode: 'incremental'
                  parameters: {
                    vNetRgName: {
                      value: '[parameters(\'vNetRgName\')]'
                    }
                    vNetName: {
                      value: '[parameters(\'vNetName\')]'
                    }
                    vNetLocation: {
                      value: '[parameters(\'vNetLocation\')]'
                    }
                    vNetCidrRange: {
                      value: '[parameters(\'vNetCidrRange\')]'
                    }
                    hubResourceId: {
                      value: '[parameters(\'hubResourceId\')]'
                    }
                  }
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      vNetRgName: {
                        type: 'string'
                      }
                      vNetName: {
                        type: 'string'
                      }
                      vNetLocation: {
                        type: 'string'
                      }
                      vNetCidrRange: {
                        type: 'string'
                      }
                      vNetPeerUseRemoteGateway: {
                        type: 'bool'
                        defaultValue: false
                      }
                      hubResourceId: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2020-06-01'
                        name: '[concat(\'es-lz-vnet-\',substring(uniqueString(subscription().id),0,6),\'-rg\')]'
                        location: '[parameters(\'vNetLocation\')]'
                        dependsOn: []
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            variables: {}
                            resources: [
                              {
                                type: 'Microsoft.Resources/resourceGroups'
                                apiVersion: '2020-06-01'
                                name: '[parameters(\'vNetRgName\')]'
                                location: '[parameters(\'vNetLocation\')]'
                                properties: {}
                              }
                              {
                                type: 'Microsoft.Resources/resourceGroups'
                                apiVersion: '2020-06-01'
                                name: 'NetworkWatcherRG'
                                location: '[parameters(\'vNetLocation\')]'
                                properties: {}
                              }
                            ]
                            outputs: {}
                          }
                        }
                      }
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2020-06-01'
                        name: '[concat(\'es-lz-vnet-\',substring(uniqueString(subscription().id),0,6))]'
                        dependsOn: [
                          '[concat(\'es-lz-vnet-\',substring(uniqueString(subscription().id),0,6),\'-rg\')]'
                        ]
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            variables: {}
                            resources: [
                              {
                                type: 'Microsoft.Network/virtualNetworks'
                                apiVersion: '2020-06-01'
                                name: '[parameters(\'vNetName\')]'
                                location: '[parameters(\'vNetLocation\')]'
                                dependsOn: []
                                properties: {
                                  addressSpace: {
                                    addressPrefixes: [
                                      '[parameters(\'vNetCidrRange\')]'
                                    ]
                                  }
                                }
                              }
                              {
                                type: 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings'
                                apiVersion: '2020-05-01'
                                name: '[concat(parameters(\'vNetName\'), \'/peerToHub\')]'
                                dependsOn: [
                                  '[parameters(\'vNetName\')]'
                                ]
                                properties: {
                                  remoteVirtualNetwork: {
                                    id: '[parameters(\'hubResourceId\')]'
                                  }
                                  allowVirtualNetworkAccess: true
                                  allowForwardedTraffic: true
                                  allowGatewayTransit: false
                                  useRemoteGateways: '[parameters(\'vNetPeerUseRemoteGateway\')]'
                                }
                              }
                              {
                                type: 'Microsoft.Resources/deployments'
                                apiVersion: '2020-06-01'
                                name: '[concat(\'es-lz-hub-\',substring(uniqueString(subscription().id),0,6),\'-peering\')]'
                                subscriptionId: '[split(parameters(\'hubResourceId\'),\'/\')[2]]'
                                resourceGroup: '[split(parameters(\'hubResourceId\'),\'/\')[4]]'
                                dependsOn: [
                                  '[parameters(\'vNetName\')]'
                                ]
                                properties: {
                                  mode: 'Incremental'
                                  expressionEvaluationOptions: {
                                    scope: 'inner'
                                  }
                                  template: {
                                    '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                                    contentVersion: '1.0.0.0'
                                    parameters: {
                                      remoteVirtualNetwork: {
                                        Type: 'string'
                                        defaultValue: false
                                      }
                                      hubName: {
                                        Type: 'string'
                                        defaultValue: false
                                      }
                                    }
                                    variables: {}
                                    resources: [
                                      {
                                        type: 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings'
                                        name: '[[concat(parameters(\'hubName\'),\'/\',last(split(parameters(\'remoteVirtualNetwork\'),\'/\')))]'
                                        apiVersion: '2020-05-01'
                                        properties: {
                                          allowVirtualNetworkAccess: true
                                          allowForwardedTraffic: true
                                          allowGatewayTransit: true
                                          useRemoteGateways: false
                                          remoteVirtualNetwork: {
                                            id: '[[parameters(\'remoteVirtualNetwork\')]'
                                          }
                                        }
                                      }
                                    ]
                                    outputs: {}
                                  }
                                  parameters: {
                                    remoteVirtualNetwork: {
                                      value: '[concat(subscription().id,\'/resourceGroups/\',parameters(\'vNetRgName\'), \'/providers/\',\'Microsoft.Network/virtualNetworks/\', parameters(\'vNetName\'))]'
                                    }
                                    hubName: {
                                      value: '[split(parameters(\'hubResourceId\'),\'/\')[8]]'
                                    }
                                  }
                                }
                              }
                            ]
                            outputs: {}
                          }
                        }
                        resourceGroup: '[parameters(\'vNetRgName\')]'
                      }
                    ]
                    outputs: {}
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-VNET-HubSpoke'
    }
    {
      properties: {
        Description: 'This policy denies  the creation of  Azure Kubernetes Service non-private clusters'
        DisplayName: 'Public network access on AKS API should be disabled'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Kubernetes'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.ContainerService/managedClusters'
              }
              {
                field: 'Microsoft.ContainerService/managedClusters/apiServerAccessProfile.enablePrivateCluster'
                notequals: 'true'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-PublicEndpoint-Aks'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Analysis Services to stream to a Log Analytics workspace when any Analysis Services which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Analysis Services to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.AnalysisServices/servers'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.AnalysisServices/servers/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'Engine'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Service'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-AnalysisService'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for API Management to stream to a Log Analytics workspace when any API Management which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for API Management to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.ApiManagement/service'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.ApiManagement/service/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'Gateway Requests'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                            {
                              category: 'Capacity'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                            {
                              category: 'EventHub Events'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                            {
                              category: 'Network Status'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'GatewayLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-APIMgmt'
    }
    {
      properties: {
        Description: 'This policy enables you to restrict that Application Gateways is always deployed with WAF enabled'
        DisplayName: 'Application Gateway should be deployed with WAF enabled'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Network/applicationGateways'
              }
              {
                field: 'Microsoft.Network/applicationGateways/sku.name'
                notequals: 'WAF_v2'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-AppGW-Without-WAF'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Application Gateway to stream to a Log Analytics workspace when any Application Gateway which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Application Gateway to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/applicationGateways'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Network/applicationGateways/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'ApplicationGatewayAccessLog'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'ApplicationGatewayPerformanceLog'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'ApplicationGatewayFirewallLog'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-ApplicationGateway'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for App Service Plan to stream to a Log Analytics workspace when any App Service Plan which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for App Service Plan to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Web/serverfarms'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Web/serverfarms/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: []
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-WebServerFarm'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Web App to stream to a Log Analytics workspace when any Web App which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for App Service to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Web/sites'
              }
              {
                value: '[field(\'kind\')]'
                notContains: 'functionapp'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Web/sites/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'AppServiceAntivirusScanAuditLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AppServiceHTTPLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AppServiceConsoleLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AppServiceHTTPLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AppServiceAppLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AppServiceFileAuditLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AppServiceAuditLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AppServiceIPSecAuditLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AppServicePlatformLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-Website'
    }
    {
      properties: {
        Description: 'Deploys  the Azure Defender settings in Azure Security Center for  the specific services.'
        DisplayName: 'Deploy Azure Defender settings in Azure Security Center.'
        Mode: 'Indexed'
        Parameters: {
          pricingTierVMs: {
            type: 'String'
            metadata: {
              displayName: 'pricingTierVMs'
              description: null
            }
            allowedValues: [
              'Standard'
              'Free'
            ]
            defaultValue: 'Standard'
          }
          pricingTierSqlServers: {
            type: 'String'
            metadata: {
              displayName: 'pricingTierSqlServers'
              description: null
            }
            allowedValues: [
              'Standard'
              'Free'
            ]
            defaultValue: 'Standard'
          }
          pricingTierAppServices: {
            type: 'String'
            metadata: {
              displayName: 'pricingTierAppServices'
              description: null
            }
            allowedValues: [
              'Standard'
              'Free'
            ]
            defaultValue: 'Standard'
          }
          pricingTierStorageAccounts: {
            type: 'String'
            metadata: {
              displayName: 'pricingTierStorageAccounts'
              description: null
            }
            allowedValues: [
              'Standard'
              'Free'
            ]
            defaultValue: 'Standard'
          }
          pricingTierContainerRegistry: {
            type: 'String'
            metadata: {
              displayName: 'pricingTierContainerRegistry'
              description: null
            }
            allowedValues: [
              'Standard'
              'Free'
            ]
            defaultValue: 'Standard'
          }
          pricingTierKeyVaults: {
            type: 'String'
            metadata: {
              displayName: 'pricingTierKeyVaults'
              description: null
            }
            allowedValues: [
              'Standard'
              'Free'
            ]
            defaultValue: 'Standard'
          }
          pricingTierKubernetesService: {
            type: 'String'
            metadata: {
              displayName: 'pricingTierKubernetesService'
              description: null
            }
            allowedValues: [
              'Standard'
              'Free'
            ]
            defaultValue: 'Standard'
          }
          pricingTierDns: {
            type: 'String'
            metadata: {
              displayName: 'pricingTierDns'
              description: null
            }
            allowedValues: [
              'Standard'
              'Free'
            ]
            defaultValue: 'Standard'
          }
          pricingTierArm: {
            type: 'String'
            metadata: {
              displayName: 'pricingTierArm'
              description: null
            }
            allowedValues: [
              'Standard'
              'Free'
            ]
            defaultValue: 'Standard'
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Security Center'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Resources/subscriptions'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Security/pricings'
              deploymentScope: 'subscription'
              existenceScope: 'subscription'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/fb1c8493-542b-48eb-b624-b4c8fea62acd'
              ]
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Security/pricings/pricingTier'
                    equals: 'Standard'
                  }
                  {
                    field: 'type'
                    equals: 'Microsoft.Security/pricings'
                  }
                ]
              }
              deployment: {
                location: 'northeurope'
                properties: {
                  mode: 'incremental'
                  parameters: {
                    pricingTierVMs: {
                      value: '[parameters(\'pricingTierVMs\')]'
                    }
                    pricingTierSqlServers: {
                      value: '[parameters(\'pricingTierSqlServers\')]'
                    }
                    pricingTierAppServices: {
                      value: '[parameters(\'pricingTierAppServices\')]'
                    }
                    pricingTierStorageAccounts: {
                      value: '[parameters(\'pricingTierStorageAccounts\')]'
                    }
                    pricingTierContainerRegistry: {
                      value: '[parameters(\'pricingTierContainerRegistry\')]'
                    }
                    pricingTierKeyVaults: {
                      value: '[parameters(\'pricingTierKeyVaults\')]'
                    }
                    pricingTierKubernetesService: {
                      value: '[parameters(\'pricingTierKubernetesService\')]'
                    }
                    pricingTierDns: {
                      value: '[parameters(\'pricingTierDns\')]'
                    }
                    pricingTierArm: {
                      value: '[parameters(\'pricingTierArm\')]'
                    }
                  }
                  template: {
                    '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      pricingTierVMs: {
                        type: 'string'
                        metadata: {
                          description: 'pricingTierVMs'
                        }
                      }
                      pricingTierSqlServers: {
                        type: 'string'
                        metadata: {
                          description: 'pricingTierSqlServers'
                        }
                      }
                      pricingTierAppServices: {
                        type: 'string'
                        metadata: {
                          description: 'pricingTierAppServices'
                        }
                      }
                      pricingTierStorageAccounts: {
                        type: 'string'
                        metadata: {
                          description: 'pricingTierStorageAccounts'
                        }
                      }
                      pricingTierContainerRegistry: {
                        type: 'string'
                        metadata: {
                          description: 'ContainerRegistry'
                        }
                      }
                      pricingTierKeyVaults: {
                        type: 'string'
                        metadata: {
                          description: 'KeyVaults'
                        }
                      }
                      pricingTierKubernetesService: {
                        type: 'string'
                        metadata: {
                          description: 'KubernetesService'
                        }
                      }
                      pricingTierDns: {
                        type: 'string'
                        metadata: {
                          description: 'KubernetesService'
                        }
                      }
                      pricingTierArm: {
                        type: 'string'
                        metadata: {
                          description: 'KubernetesService'
                        }
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Security/pricings'
                        apiVersion: '2018-06-01'
                        name: 'VirtualMachines'
                        properties: {
                          pricingTier: '[parameters(\'pricingTierVMs\')]'
                        }
                      }
                      {
                        type: 'Microsoft.Security/pricings'
                        apiVersion: '2018-06-01'
                        name: 'StorageAccounts'
                        dependsOn: [
                          '[concat(\'Microsoft.Security/pricings/VirtualMachines\')]'
                        ]
                        properties: {
                          pricingTier: '[parameters(\'pricingTierStorageAccounts\')]'
                        }
                      }
                      {
                        type: 'Microsoft.Security/pricings'
                        apiVersion: '2018-06-01'
                        name: 'AppServices'
                        dependsOn: [
                          '[concat(\'Microsoft.Security/pricings/StorageAccounts\')]'
                        ]
                        properties: {
                          pricingTier: '[parameters(\'pricingTierAppServices\')]'
                        }
                      }
                      {
                        type: 'Microsoft.Security/pricings'
                        apiVersion: '2018-06-01'
                        name: 'SqlServers'
                        dependsOn: [
                          '[concat(\'Microsoft.Security/pricings/AppServices\')]'
                        ]
                        properties: {
                          pricingTier: '[parameters(\'pricingTierSqlServers\')]'
                        }
                      }
                      {
                        type: 'Microsoft.Security/pricings'
                        apiVersion: '2018-06-01'
                        name: 'KeyVaults'
                        dependsOn: [
                          '[concat(\'Microsoft.Security/pricings/SqlServers\')]'
                        ]
                        properties: {
                          pricingTier: '[parameters(\'pricingTierKeyVaults\')]'
                        }
                      }
                      {
                        type: 'Microsoft.Security/pricings'
                        apiVersion: '2018-06-01'
                        name: 'KubernetesService'
                        dependsOn: [
                          '[concat(\'Microsoft.Security/pricings/KeyVaults\')]'
                        ]
                        properties: {
                          pricingTier: '[parameters(\'pricingTierKubernetesService\')]'
                        }
                      }
                      {
                        type: 'Microsoft.Security/pricings'
                        apiVersion: '2018-06-01'
                        name: 'ContainerRegistry'
                        dependsOn: [
                          '[concat(\'Microsoft.Security/pricings/KubernetesService\')]'
                        ]
                        properties: {
                          pricingTier: '[parameters(\'pricingTierContainerRegistry\')]'
                        }
                      }
                      {
                        type: 'Microsoft.Security/pricings'
                        apiVersion: '2018-06-01'
                        name: 'Dns'
                        dependsOn: [
                          '[concat(\'Microsoft.Security/pricings/ContainerRegistry\')]'
                        ]
                        properties: {
                          pricingTier: '[parameters(\'pricingTierDns\')]'
                        }
                      }
                      {
                        type: 'Microsoft.Security/pricings'
                        apiVersion: '2018-06-01'
                        name: 'Arm'
                        dependsOn: [
                          '[concat(\'Microsoft.Security/pricings/Dns\')]'
                        ]
                        properties: {
                          pricingTier: '[parameters(\'pricingTierArm\')]'
                        }
                      }
                    ]
                    outputs: {}
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-ASC-Standard'
    }
    {
      properties: {
        Description: 'This policy denies the creation of child resources on the Automation Account'
        DisplayName: 'No child resources in Automation Account'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Automation'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                in: [
                  'Microsoft.Automation/automationAccounts/runbooks'
                  'Microsoft.Automation/automationAccounts/variables'
                  'Microsoft.Automation/automationAccounts/modules'
                  'Microsoft.Automation/automationAccounts/credentials'
                  'Microsoft.Automation/automationAccounts/connections'
                  'Microsoft.Automation/automationAccount/certificates'
                ]
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-AA-child-resources'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Automation to stream to a Log Analytics workspace when any Automation which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Automation to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Automation/automationAccounts'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Automation/automationAccounts/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              timeGrain: null
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                enabled: false
                                days: 0
                              }
                            }
                          ]
                          logs: [
                            {
                              category: 'JobLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'JobStreams'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'DscNodeStatus'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-AA'
    }
    {
      properties: {
        displayName: 'RDP access from the Internet should be blocked'
        description: 'This policy denies any network security rule that allows RDP access from Internet'
        mode: 'All'
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        parameters: {
          effect: {
            type: 'String'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
          }
        }
        policyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Network/networkSecurityGroups/securityRules'
              }
              {
                allOf: [
                  {
                    field: 'Microsoft.Network/networkSecurityGroups/securityRules/access'
                    equals: 'Allow'
                  }
                  {
                    field: 'Microsoft.Network/networkSecurityGroups/securityRules/direction'
                    equals: 'Inbound'
                  }
                  {
                    anyOf: [
                      {
                        field: 'Microsoft.Network/networkSecurityGroups/securityRules/destinationPortRange'
                        equals: '*'
                      }
                      {
                        field: 'Microsoft.Network/networkSecurityGroups/securityRules/destinationPortRange'
                        equals: '3389'
                      }
                      {
                        value: '[if(and(not(empty(field(\'Microsoft.Network/networkSecurityGroups/securityRules/destinationPortRange\'))), contains(field(\'Microsoft.Network/networkSecurityGroups/securityRules/destinationPortRange\'),\'-\')), and(lessOrEquals(int(first(split(field(\'Microsoft.Network/networkSecurityGroups/securityRules/destinationPortRange\'), \'-\'))),3389),greaterOrEquals(int(last(split(field(\'Microsoft.Network/networkSecurityGroups/securityRules/destinationPortRange\'), \'-\'))),3389)), \'false\')]'
                        equals: 'true'
                      }
                      {
                        count: {
                          field: 'Microsoft.Network/networkSecurityGroups/securityRules/destinationPortRanges[*]'
                          where: {
                            value: '[if(and(not(empty(first(field(\'Microsoft.Network/networkSecurityGroups/securityRules/destinationPortRanges[*]\')))), contains(first(field(\'Microsoft.Network/networkSecurityGroups/securityRules/destinationPortRanges[*]\')),\'-\')), and(lessOrEquals(int(first(split(first(field(\'Microsoft.Network/networkSecurityGroups/securityRules/destinationPortRanges[*]\')), \'-\'))),3389),greaterOrEquals(int(last(split(first(field(\'Microsoft.Network/networkSecurityGroups/securityRules/destinationPortRanges[*]\')), \'-\'))),3389)) , \'false\')]'
                            equals: 'true'
                          }
                        }
                        greater: 0
                      }
                      {
                        not: {
                          field: 'Microsoft.Network/networkSecurityGroups/securityRules/destinationPortRanges[*]'
                          notEquals: '*'
                        }
                      }
                      {
                        not: {
                          field: 'Microsoft.Network/networkSecurityGroups/securityRules/destinationPortRanges[*]'
                          notEquals: '3389'
                        }
                      }
                    ]
                  }
                  {
                    anyOf: [
                      {
                        field: 'Microsoft.Network/networkSecurityGroups/securityRules/sourceAddressPrefix'
                        equals: '*'
                      }
                      {
                        field: 'Microsoft.Network/networkSecurityGroups/securityRules/sourceAddressPrefix'
                        equals: 'Internet'
                      }
                      {
                        not: {
                          field: 'Microsoft.Network/networkSecurityGroups/securityRules/sourceAddressPrefixes[*]'
                          notEquals: '*'
                        }
                      }
                      {
                        not: {
                          field: 'Microsoft.Network/networkSecurityGroups/securityRules/sourceAddressPrefixes[*]'
                          notEquals: 'Internet'
                        }
                      }
                    ]
                  }
                ]
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-RDP-From-Internet'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Batch to stream to a Log Analytics workspace when any Batch which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Batch to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Batch/batchAccounts'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Batch/batchAccounts/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'ServiceLog'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-Batch'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for CDN Endpoint to stream to a Log Analytics workspace when any CDN Endpoint which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for CDN Endpoint to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Cdn/profiles/endpoints'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Cdn/profiles/endpoints/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: []
                          logs: [
                            {
                              category: 'CoreAnalytics'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'fullName\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-CDNEndpoints'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Cognitive Services to stream to a Log Analytics workspace when any Cognitive Services which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Cognitive Services to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.CognitiveServices/accounts'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.CognitiveServices/accounts/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'Audit'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'RequestResponse'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Trace'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-CognitiveServices'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Cosmos DB to stream to a Log Analytics workspace when any Cosmos DB which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Cosmos DB to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.DocumentDB/databaseAccounts'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.DocumentDB/databaseAccounts/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'Requests'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'DataPlaneRequests'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'MongoRequests'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'QueryRuntimeStatistics'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'PartitionKeyStatistics'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'PartitionKeyRUConsumption'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'ControlPlaneRequests'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'CassandraRequests'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'GremlinRequests'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-CosmosDB'
    }
    {
      properties: {
        Description: 'This policy denies that  Cosmos database accounts  are created with out public network access is disabled.'
        DisplayName: 'Public network access should be disabled for CosmosDB'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'SQL'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.DocumentDB/databaseAccounts'
              }
              {
                field: 'Microsoft.DocumentDB/databaseAccounts/publicNetworkAccess'
                notequals: 'Disabled'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-PublicEndpoint-CosmosDB'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Databricks to stream to a Log Analytics workspace when any Databricks which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Databricks to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Databricks/workspaces'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Databricks/workspaces/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          logs: [
                            {
                              category: 'dbfs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'clusters'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'accounts'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'jobs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'notebook'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'ssh'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'workspace'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'secrets'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'sqlPermissions'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'instancePools'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-Databricks'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Data Factory to stream to a Log Analytics workspace when any Data Factory which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Data Factory to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.DataFactory/factories'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.DataFactory/factories/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'ActivityRuns'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'PipelineRuns'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'TriggerRuns'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'SSISPackageEventMessages'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'SSISPackageExecutableStatistics'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'SSISPackageEventMessageContext'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'SSISPackageExecutionComponentPhases'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'SSISPackageExecutionDataStatistics'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'SSISIntegrationRuntimeLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-DataFactory'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Data Lake Analytics to stream to a Log Analytics workspace when any Data Lake Analytics which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Data Lake Analytics to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.DataLakeAnalytics/accounts'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.DataLakeAnalytics/accounts/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'Audit'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Requests'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-DLAnalytics'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Azure Data Lake Store to stream to a Log Analytics workspace when anyAzure Data Lake Store which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Azure Data Lake Store to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.DataLakeStore/accounts'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.DataLakeStore/accounts/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'Audit'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Requests'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-DataLakeStore'
    }
    {
      properties: {
        Description: 'Deploys the configurations of a Private DNS Zone Group by a parameter for Storage-Blob Private Endpoint. Used enforce the configuration to a single Private DNS Zone. '
        DisplayName: 'Deploy DNS Zone Group for Storage-Blob Private Endpoint'
        Mode: 'Indexed'
        Parameters: {
          privateDnsZoneId: {
            type: 'String'
            metadata: {
              displayName: 'privateDnsZoneId'
              strongType: 'Microsoft.Network/privateDnsZones'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Network/privateEndpoints'
              }
              {
                count: {
                  field: 'Microsoft.Network/privateEndpoints/privateLinkServiceConnections[*].groupIds[*]'
                  where: {
                    field: 'Microsoft.Network/privateEndpoints/privateLinkServiceConnections[*].groupIds[*]'
                    equals: 'blob'
                  }
                }
                greaterOrEquals: 1
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/4d97b98b-1d4f-4787-a291-c67834d212e7'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      privateDnsZoneId: {
                        type: 'string'
                      }
                      privateEndpointName: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                    }
                    resources: [
                      {
                        name: '[concat(parameters(\'privateEndpointName\'), \'/deployedByPolicy\')]'
                        type: 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups'
                        apiVersion: '2020-03-01'
                        location: '[parameters(\'location\')]'
                        properties: {
                          privateDnsZoneConfigs: [
                            {
                              name: 'storageBlob-privateDnsZone'
                              properties: {
                                privateDnsZoneId: '[parameters(\'privateDnsZoneId\')]'
                              }
                            }
                          ]
                        }
                      }
                    ]
                  }
                  parameters: {
                    privateDnsZoneId: {
                      value: '[parameters(\'privateDnsZoneId\')]'
                    }
                    privateEndpointName: {
                      value: '[field(\'name\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-DNSZoneGroup-For-Blob-PrivateEndpoint'
    }
    {
      properties: {
        Description: 'Deploys the configurations of a Private DNS Zone Group by a parameter for Storage-File Private Endpoint. Used enforce the configuration to a single Private DNS Zone. '
        DisplayName: 'Deploy DNS  Zone Group for Storage-File Private Endpoint'
        Mode: 'Indexed'
        Parameters: {
          privateDnsZoneId: {
            type: 'String'
            metadata: {
              displayName: 'privateDnsZoneId'
              strongType: 'Microsoft.Network/privateDnsZones'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Network/privateEndpoints'
              }
              {
                count: {
                  field: 'Microsoft.Network/privateEndpoints/privateLinkServiceConnections[*].groupIds[*]'
                  where: {
                    field: 'Microsoft.Network/privateEndpoints/privateLinkServiceConnections[*].groupIds[*]'
                    equals: 'file'
                  }
                }
                greaterOrEquals: 1
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/4d97b98b-1d4f-4787-a291-c67834d212e7'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      privateDnsZoneId: {
                        type: 'string'
                      }
                      privateEndpointName: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                    }
                    resources: [
                      {
                        name: '[concat(parameters(\'privateEndpointName\'), \'/deployedByPolicy\')]'
                        type: 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups'
                        apiVersion: '2020-03-01'
                        location: '[parameters(\'location\')]'
                        properties: {
                          privateDnsZoneConfigs: [
                            {
                              name: 'storageFile-privateDnsZone'
                              properties: {
                                privateDnsZoneId: '[parameters(\'privateDnsZoneId\')]'
                              }
                            }
                          ]
                        }
                      }
                    ]
                  }
                  parameters: {
                    privateDnsZoneId: {
                      value: '[parameters(\'privateDnsZoneId\')]'
                    }
                    privateEndpointName: {
                      value: '[field(\'name\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-DNSZoneGroup-For-File-PrivateEndpoint'
    }
    {
      properties: {
        Description: 'Deploys the configurations of a Private DNS Zone Group by a parameter for Key Vault Private Endpoint. Used enforce the configuration to a single Private DNS Zone. '
        DisplayName: 'Deploy DNS  Zone Group for Key Vault Private Endpoint'
        Mode: 'Indexed'
        Parameters: {
          privateDnsZoneId: {
            type: 'String'
            metadata: {
              displayName: 'privateDnsZoneId'
              strongType: 'Microsoft.Network/privateDnsZones'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Network/privateEndpoints'
              }
              {
                count: {
                  field: 'Microsoft.Network/privateEndpoints/privateLinkServiceConnections[*].groupIds[*]'
                  where: {
                    field: 'Microsoft.Network/privateEndpoints/privateLinkServiceConnections[*].groupIds[*]'
                    equals: 'vault'
                  }
                }
                greaterOrEquals: 1
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/4d97b98b-1d4f-4787-a291-c67834d212e7'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      privateDnsZoneId: {
                        type: 'string'
                      }
                      privateEndpointName: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                    }
                    resources: [
                      {
                        name: '[concat(parameters(\'privateEndpointName\'), \'/deployedByPolicy\')]'
                        type: 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups'
                        apiVersion: '2020-03-01'
                        location: '[parameters(\'location\')]'
                        properties: {
                          privateDnsZoneConfigs: [
                            {
                              name: 'keyVault-privateDnsZone'
                              properties: {
                                privateDnsZoneId: '[parameters(\'privateDnsZoneId\')]'
                              }
                            }
                          ]
                        }
                      }
                    ]
                  }
                  parameters: {
                    privateDnsZoneId: {
                      value: '[parameters(\'privateDnsZoneId\')]'
                    }
                    privateEndpointName: {
                      value: '[field(\'name\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-DNSZoneGroup-For-KeyVault-PrivateEndpoint'
    }
    {
      properties: {
        Description: 'Deploys the configurations of a Private DNS Zone Group by a parameter for Storage-Queue Private Endpoint. Used enforce the configuration to a single Private DNS Zone. '
        DisplayName: 'Deploy DNS  Zone Group for Storage-Queue Private Endpoint'
        Mode: 'Indexed'
        Parameters: {
          privateDnsZoneId: {
            type: 'String'
            metadata: {
              displayName: 'privateDnsZoneId'
              strongType: 'Microsoft.Network/privateDnsZones'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Network/privateEndpoints'
              }
              {
                count: {
                  field: 'Microsoft.Network/privateEndpoints/privateLinkServiceConnections[*].groupIds[*]'
                  where: {
                    field: 'Microsoft.Network/privateEndpoints/privateLinkServiceConnections[*].groupIds[*]'
                    equals: 'queue'
                  }
                }
                greaterOrEquals: 1
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/4d97b98b-1d4f-4787-a291-c67834d212e7'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      privateDnsZoneId: {
                        type: 'string'
                      }
                      privateEndpointName: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                    }
                    resources: [
                      {
                        name: '[concat(parameters(\'privateEndpointName\'), \'/deployedByPolicy\')]'
                        type: 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups'
                        apiVersion: '2020-03-01'
                        location: '[parameters(\'location\')]'
                        properties: {
                          privateDnsZoneConfigs: [
                            {
                              name: 'storageQueue-privateDnsZone'
                              properties: {
                                privateDnsZoneId: '[parameters(\'privateDnsZoneId\')]'
                              }
                            }
                          ]
                        }
                      }
                    ]
                  }
                  parameters: {
                    privateDnsZoneId: {
                      value: '[parameters(\'privateDnsZoneId\')]'
                    }
                    privateEndpointName: {
                      value: '[field(\'name\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-DNSZoneGroup-For-Queue-PrivateEndpoint'
    }
    {
      properties: {
        Description: 'Deploys the configurations of a Private DNS Zone Group by a parameter for SQL Private Private Endpoint. Used enforce the configuration to a single Private DNS Zone. '
        DisplayName: 'Deploy DNS  Zone Group for SQL Private Endpoint'
        Mode: 'Indexed'
        Parameters: {
          privateDnsZoneId: {
            type: 'String'
            metadata: {
              displayName: 'privateDnsZoneId'
              strongType: 'Microsoft.Network/privateDnsZones'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Network/privateEndpoints'
              }
              {
                count: {
                  field: 'Microsoft.Network/privateEndpoints/privateLinkServiceConnections[*].groupIds[*]'
                  where: {
                    field: 'Microsoft.Network/privateEndpoints/privateLinkServiceConnections[*].groupIds[*]'
                    equals: 'sqlServer'
                  }
                }
                greaterOrEquals: 1
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/4d97b98b-1d4f-4787-a291-c67834d212e7'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      privateDnsZoneId: {
                        type: 'string'
                      }
                      privateEndpointName: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                    }
                    resources: [
                      {
                        name: '[concat(parameters(\'privateEndpointName\'), \'/deployedByPolicy\')]'
                        type: 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups'
                        apiVersion: '2020-03-01'
                        location: '[parameters(\'location\')]'
                        properties: {
                          privateDnsZoneConfigs: [
                            {
                              name: 'sqlServer-privateDnsZone'
                              properties: {
                                privateDnsZoneId: '[parameters(\'privateDnsZoneId\')]'
                              }
                            }
                          ]
                        }
                      }
                    ]
                  }
                  parameters: {
                    privateDnsZoneId: {
                      value: '[parameters(\'privateDnsZoneId\')]'
                    }
                    privateEndpointName: {
                      value: '[field(\'name\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-DNSZoneGroup-For-Sql-PrivateEndpoint'
    }
    {
      properties: {
        Description: 'Deploys the configurations of a Private DNS Zone Group by a parameter for Storage-Table Private Endpoint. Used enforce the configuration to a single Private DNS Zone. '
        DisplayName: 'Deploy DNS  Zone Group for Storage-Table Private Endpoint'
        Mode: 'Indexed'
        Parameters: {
          privateDnsZoneId: {
            type: 'String'
            metadata: {
              displayName: 'privateDnsZoneId'
              strongType: 'Microsoft.Network/privateDnsZones'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Network/privateEndpoints'
              }
              {
                count: {
                  field: 'Microsoft.Network/privateEndpoints/privateLinkServiceConnections[*].groupIds[*]'
                  where: {
                    field: 'Microsoft.Network/privateEndpoints/privateLinkServiceConnections[*].groupIds[*]'
                    equals: 'table'
                  }
                }
                greaterOrEquals: 1
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/4d97b98b-1d4f-4787-a291-c67834d212e7'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      privateDnsZoneId: {
                        type: 'string'
                      }
                      privateEndpointName: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                    }
                    resources: [
                      {
                        name: '[concat(parameters(\'privateEndpointName\'), \'/deployedByPolicy\')]'
                        type: 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups'
                        apiVersion: '2020-03-01'
                        location: '[parameters(\'location\')]'
                        properties: {
                          privateDnsZoneConfigs: [
                            {
                              name: 'storageTable-privateDnsZone'
                              properties: {
                                privateDnsZoneId: '[parameters(\'privateDnsZoneId\')]'
                              }
                            }
                          ]
                        }
                      }
                    ]
                  }
                  parameters: {
                    privateDnsZoneId: {
                      value: '[parameters(\'privateDnsZoneId\')]'
                    }
                    privateEndpointName: {
                      value: '[field(\'name\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-DNSZoneGroup-For-Table-PrivateEndpoint'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Event Grid subscriptions to stream to a Log Analytics workspace when any Event Grid subscriptions which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Event Grid subscriptions to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.EventGrid/eventSubscriptions'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.EventGrid/eventSubscriptions/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: []
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-EventGridSub'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Event Grid Topic to stream to a Log Analytics workspace when any Event Grid Topic which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Event Grid Topic to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.EventGrid/topics'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.EventGrid/topics/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'DeliveryFailures'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'PublishFailures'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-EventGridTopic'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Event Hubs to stream to a Log Analytics workspace when any Event Hubs which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Event Hubs to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.EventHub/namespaces'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.EventHub/namespaces/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'ArchiveLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'OperationalLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AutoScaleLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'KafkaCoordinatorLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'KafkaUserErrorLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'EventHubVNetConnectionEvent'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'CustomerManagedKeyUserLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-EventHub'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Event Grid System Topic to stream to a Log Analytics workspace when any Event Grid System Topic which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Event Grid System Topic to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.EventGrid/systemTopics'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.EventGrid/systemTopics/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'DeliveryFailures'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-EventGridSystemTopic'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for ExpressRoute to stream to a Log Analytics workspace when any ExpressRoute which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for ExpressRoute to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/expressRouteCircuits'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Network/expressRouteCircuits/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'PeeringRouteLog'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-ExpressRoute'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Firewall to stream to a Log Analytics workspace when any Firewall which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Firewall to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/azureFirewalls'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Network/azureFirewalls/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'AzureFirewallApplicationRule'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AzureFirewallNetworkRule'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AzureFirewallDnsProxy'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-Firewall'
    }
    {
      properties: {
        Description: 'Deploys Azure Firewall Manager policy in subscription where the policy is assigned.'
        DisplayName: 'Deploy Azure Firewall Manager policy in the subscription'
        Mode: 'Indexed'
        Parameters: {
          fwpolicy: {
            type: 'Object'
            metadata: {
              displayName: 'fwpolicy'
              description: 'Object describing Azure Firewall Policy'
            }
            defaultValue: {}
          }
          fwPolicyRegion: {
            type: 'String'
            metadata: {
              displayName: 'fwPolicyRegion'
              description: 'Select Azure region for Azure Firewall Policy'
              strongType: 'location'
            }
          }
          rgName: {
            type: 'String'
            metadata: {
              displayName: 'rgName'
              description: 'Provide name for resource group.'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Resources/subscriptions'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Network/firewallPolicies'
              deploymentScope: 'Subscription'
              existenceScope: 'ResourceGroup'
              resourceGroupName: '[parameters(\'rgName\')]'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
              ]
              deployment: {
                location: 'northeurope'
                properties: {
                  mode: 'incremental'
                  parameters: {
                    rgName: {
                      value: '[parameters(\'rgName\')]'
                    }
                    fwPolicy: {
                      value: '[parameters(\'fwPolicy\')]'
                    }
                    fwPolicyRegion: {
                      value: '[parameters(\'fwPolicyRegion\')]'
                    }
                  }
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      rgName: {
                        type: 'string'
                      }
                      fwPolicy: {
                        type: 'object'
                      }
                      fwPolicyRegion: {
                        type: 'string'
                      }
                    }
                    resources: [
                      {
                        type: 'Microsoft.Resources/resourceGroups'
                        apiVersion: '2018-05-01'
                        name: '[parameters(\'rgName\')]'
                        location: '[deployment().location]'
                        properties: {}
                      }
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2018-05-01'
                        name: 'fwpolicies'
                        resourceGroup: '[parameters(\'rgName\')]'
                        dependsOn: [
                          '[resourceId(\'Microsoft.Resources/resourceGroups/\', parameters(\'rgName\'))]'
                        ]
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            variables: {}
                            resources: [
                              {
                                type: 'Microsoft.Network/firewallPolicies'
                                apiVersion: '2019-09-01'
                                name: '[parameters(\'fwpolicy\').firewallPolicyName]'
                                location: '[parameters(\'fwpolicy\').location]'
                                dependsOn: []
                                tags: {}
                                properties: {}
                                resources: [
                                  {
                                    type: 'ruleGroups'
                                    apiVersion: '2019-09-01'
                                    name: '[parameters(\'fwpolicy\').ruleGroups.name]'
                                    dependsOn: [
                                      '[resourceId(\'Microsoft.Network/firewallPolicies\',parameters(\'fwpolicy\').firewallPolicyName)]'
                                    ]
                                    properties: {
                                      priority: '[parameters(\'fwpolicy\').ruleGroups.properties.priority]'
                                      rules: '[parameters(\'fwpolicy\').ruleGroups.properties.rules]'
                                    }
                                  }
                                ]
                              }
                            ]
                            outputs: {}
                          }
                        }
                      }
                    ]
                    outputs: {}
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-FirewallPolicy'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Front Door to stream to a Log Analytics workspace when any Front Door which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Front Door to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/frontDoors'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Network/frontDoors/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'FrontdoorAccessLog'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'FrontdoorWebApplicationFirewallLog'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-FrontDoor'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Azure Function App to stream to a Log Analytics workspace when any function app which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Azure Function App to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Web/sites'
              }
              {
                value: '[field(\'kind\')]'
                notEquals: 'app'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Web/sites/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'FunctionAppLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-Function'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for HDInsight to stream to a Log Analytics workspace when any HDInsight which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for HDInsight to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.HDInsight/clusters'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.HDInsight/clusters/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: []
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-HDInsight'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for IoT Hub to stream to a Log Analytics workspace when any IoT Hub which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for IoT Hub to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Devices/IotHubs'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Devices/IotHubs/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'Connections'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'DeviceTelemetry'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'C2DCommands'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'DeviceIdentityOperations'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'FileUploadOperations'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Routes'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'D2CTwinOperations'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'C2DTwinOperations'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'TwinQueries'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'JobsOperations'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'DirectMethods'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'DistributedTracing'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Configurations'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'DeviceStreams'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-iotHub'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Key Vault to stream to a Log Analytics workspace when any Key Vault which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Key Vault to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.KeyVault/vaults'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              name: 'setByPolicy'
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.KeyVault/vaults/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'AuditEvent'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-KeyVault'
    }
    {
      properties: {
        Description: 'This policy denies creation of Key Vaults with IP Firewall exposed to all public endpoints'
        DisplayName: 'Public network access should be disabled for KeyVault'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Key Vault'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.KeyVault/vaults'
              }
              {
                field: 'Microsoft.KeyVault/vaults/networkAcls.defaultAction'
                notequals: 'Deny'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-PublicEndpoint-KeyVault'
    }
    {
      properties: {
        Description: 'This policy enables you to ensure when a Key Vault is created with out soft delete enabled it will be added.'
        DisplayName: 'KeyVault SoftDelete should be enabled'
        Mode: 'Indexed'
        Parameters: {}
        metadata: {
          version: '1.0.0'
          category: 'Key Vault'
        }
        PolicyRule: {
          if: {
            anyOf: [
              {
                allOf: [
                  {
                    field: 'type'
                    equals: 'Microsoft.KeyVault/vaults'
                  }
                  {
                    field: 'Microsoft.KeyVault/vaults/enableSoftDelete'
                    notEquals: false
                  }
                ]
              }
            ]
          }
          then: {
            effect: 'append'
            details: [
              {
                field: 'Microsoft.KeyVault/vaults/enableSoftDelete'
                value: true
              }
            ]
          }
        }
      }
      name: 'Append-KV-SoftDelete'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Load Balancer to stream to a Log Analytics workspace when any Load Balancer which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Load Balancer to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/loadBalancers'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Network/loadBalancers/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              timeGrain: null
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                enabled: false
                                days: 0
                              }
                            }
                          ]
                          logs: [
                            {
                              category: 'LoadBalancerAlertEvent'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'LoadBalancerProbeHealthStatus'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-LoadBalancer'
    }
    {
      properties: {
        Description: 'Deploy the configurations to the Log Analytics in the subscription. This includes a list of solutions like update, automation etc and  enables the vminsight counters. '
        DisplayName: 'Deploy the configurations to the Log Analytics in the subscription'
        Mode: 'Indexed'
        Parameters: {
          workspaceName: {
            type: 'String'
            metadata: {
              displayName: 'workspaceName'
              description: 'Provide name of existing Log Analytics workspace'
            }
          }
          workspaceRegion: {
            type: 'String'
            metadata: {
              displayName: 'workspaceRegion'
              description: 'Select region of existing Log Analytics workspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.OperationalInsights/workspaces'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.OperationalInsights/workspaces'
              deploymentScope: 'resourceGroup'
              existenceScope: 'Subscription'
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              existenceCondition: {
                allOf: [
                  {
                    field: 'name'
                    like: '[parameters(\'workspaceName\')]'
                  }
                  {
                    field: 'location'
                    equals: '[parameters(\'workspaceRegion\')]'
                  }
                ]
              }
              deployment: {
                properties: {
                  mode: 'incremental'
                  parameters: {
                    workspaceName: {
                      value: '[parameters(\'workspaceName\')]'
                    }
                    workspaceRegion: {
                      value: '[parameters(\'workspaceRegion\')]'
                    }
                  }
                  template: {
                    '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      workspaceName: {
                        type: 'string'
                      }
                      workspaceRegion: {
                        type: 'string'
                      }
                    }
                    variables: {
                      vmInsightsPerfCounters: {
                        windowsArray: [
                          {
                            armName: 'counter1'
                            objectName: 'LogicalDisk'
                            counterName: '% Free Space'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                          {
                            armName: 'counter2'
                            objectName: 'LogicalDisk'
                            counterName: 'Avg. Disk sec/Read'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                          {
                            armName: 'counter3'
                            objectName: 'LogicalDisk'
                            counterName: 'Avg. Disk sec/Transfer'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                          {
                            armName: 'counter4'
                            objectName: 'LogicalDisk'
                            counterName: 'Avg. Disk sec/Write'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                          {
                            armName: 'counter5'
                            objectName: 'LogicalDisk'
                            counterName: 'Disk Read Bytes/sec'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                          {
                            armName: 'counter6'
                            objectName: 'LogicalDisk'
                            counterName: 'Disk Reads/sec'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                          {
                            armName: 'counter7'
                            objectName: 'LogicalDisk'
                            counterName: 'Disk Transfers/sec'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                          {
                            armName: 'counter8'
                            objectName: 'LogicalDisk'
                            counterName: 'Disk Write Bytes/sec'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                          {
                            armName: 'counter9'
                            objectName: 'LogicalDisk'
                            counterName: 'Disk Writes/sec'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                          {
                            armName: 'counter10'
                            objectName: 'LogicalDisk'
                            counterName: 'Free Megabytes'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                          {
                            armName: 'counter11'
                            objectName: 'Memory'
                            counterName: 'Available MBytes'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                          {
                            armName: 'counter12'
                            objectName: 'Network Adapter'
                            counterName: 'Bytes Received/sec'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                          {
                            armName: 'counter13'
                            objectName: 'Network Adapter'
                            counterName: 'Bytes Sent/sec'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                          {
                            armName: 'counter14'
                            objectName: 'Processor'
                            counterName: '% Processor Time'
                            instanceName: '*'
                            intervalSeconds: 10
                          }
                        ]
                        linuxDiskArray: [
                          {
                            counterName: '% Used Inodes'
                          }
                          {
                            counterName: 'Free Megabytes'
                          }
                          {
                            counterName: '% Used Space'
                          }
                          {
                            counterName: 'Disk Transfers/sec'
                          }
                          {
                            counterName: 'Disk Reads/sec'
                          }
                          {
                            counterName: 'Disk writes/sec'
                          }
                        ]
                        linuxDiskObject: {
                          armResourceName: 'Disk'
                          objectName: 'Logical Disk'
                          instanceName: '*'
                          intervalSeconds: 10
                        }
                        linuxMemoryArray: [
                          {
                            counterName: 'Available MBytes Memory'
                          }
                        ]
                        linuxMemoryObject: {
                          armResourceName: 'Memory'
                          objectName: 'Memory'
                          instanceName: '*'
                          intervalSeconds: 10
                        }
                        linuxNetworkArray: [
                          {
                            counterName: 'Total Bytes Received'
                          }
                          {
                            counterName: 'Total Bytes Transmitted'
                          }
                        ]
                        linuxNetworkObject: {
                          armResourceName: 'Network'
                          objectName: 'Network'
                          instanceName: '*'
                          intervalSeconds: 10
                        }
                        linuxCpuArray: [
                          {
                            counterName: '% Processor Time'
                          }
                        ]
                        linuxCpuObject: {
                          armResourceName: 'Processor'
                          objectName: 'Processor'
                          instanceName: '*'
                          intervalSeconds: 10
                        }
                      }
                      batch1: {
                        solutions: [
                          {
                            name: '[concat(\'Security\', \'(\', parameters(\'workspaceName\'), \')\')]'
                            marketplaceName: 'Security'
                          }
                          {
                            name: '[concat(\'AgentHealthAssessment\', \'(\', parameters(\'workspaceName\'), \')\')]'
                            marketplaceName: 'AgentHealthAssessment'
                          }
                          {
                            name: '[concat(\'ChangeTracking\', \'(\', parameters(\'workspaceName\'), \')\')]'
                            marketplaceName: 'ChangeTracking'
                          }
                          {
                            name: '[concat(\'Updates\', \'(\', parameters(\'workspaceName\'), \')\')]'
                            marketplaceName: 'Updates'
                          }
                          {
                            name: '[concat(\'AzureActivity\', \'(\', parameters(\'workspaceName\'), \')\')]'
                            marketplaceName: 'AzureActivity'
                          }
                          {
                            name: '[concat(\'AzureAutomation\', \'(\', parameters(\'workspaceName\'), \')\')]'
                            marketplaceName: 'AzureAutomation'
                          }
                          {
                            name: '[concat(\'ADAssessment\', \'(\', parameters(\'workspaceName\'), \')\')]'
                            marketplaceName: 'ADAssessment'
                          }
                          {
                            name: '[concat(\'SQLAssessment\', \'(\', parameters(\'workspaceName\'), \')\')]'
                            marketplaceName: 'SQLAssessment'
                          }
                          {
                            name: '[concat(\'VMInsights\', \'(\', parameters(\'workspaceName\'), \')\')]'
                            marketplaceName: 'VMInsights'
                          }
                          {
                            name: '[concat(\'ServiceMap\', \'(\', parameters(\'workspaceName\'), \')\')]'
                            marketplaceName: 'ServiceMap'
                          }
                          {
                            name: '[concat(\'SecurityInsights\', \'(\', parameters(\'workspaceName\'), \')\')]'
                            marketplaceName: 'SecurityInsights'
                          }
                        ]
                      }
                    }
                    resources: [
                      {
                        apiVersion: '2015-11-01-preview'
                        type: 'Microsoft.OperationalInsights/workspaces/datasources'
                        name: '[concat(parameters(\'workspaceName\'), \'/LinuxPerfCollection\')]'
                        kind: 'LinuxPerformanceCollection'
                        properties: {
                          state: 'Enabled'
                        }
                      }
                      {
                        apiVersion: '2015-11-01-preview'
                        type: 'Microsoft.OperationalInsights/workspaces/dataSources'
                        name: '[concat(parameters(\'workspaceName\'), \'/\', variables(\'vmInsightsPerfCounters\').linuxDiskObject.armResourceName)]'
                        kind: 'LinuxPerformanceObject'
                        properties: {
                          performanceCounters: '[variables(\'vmInsightsPerfCounters\').linuxDiskArray]'
                          objectName: '[variables(\'vmInsightsPerfCounters\').linuxDiskObject.objectName]'
                          instanceName: '[variables(\'vmInsightsPerfCounters\').linuxDiskObject.instanceName]'
                          intervalSeconds: '[variables(\'vmInsightsPerfCounters\').linuxDiskObject.intervalSeconds]'
                        }
                      }
                      {
                        apiVersion: '2015-11-01-preview'
                        type: 'Microsoft.OperationalInsights/workspaces/dataSources'
                        name: '[concat(parameters(\'workspaceName\'), \'/\', variables(\'vmInsightsPerfCounters\').linuxMemoryObject.armResourceName)]'
                        kind: 'LinuxPerformanceObject'
                        properties: {
                          performanceCounters: '[variables(\'vmInsightsPerfCounters\').linuxMemoryArray]'
                          objectName: '[variables(\'vmInsightsPerfCounters\').linuxMemoryObject.objectName]'
                          instanceName: '[variables(\'vmInsightsPerfCounters\').linuxMemoryObject.instanceName]'
                          intervalSeconds: '[variables(\'vmInsightsPerfCounters\').linuxMemoryObject.intervalSeconds]'
                        }
                      }
                      {
                        apiVersion: '2015-11-01-preview'
                        type: 'Microsoft.OperationalInsights/workspaces/dataSources'
                        name: '[concat(parameters(\'workspaceName\'), \'/\', variables(\'vmInsightsPerfCounters\').linuxCpuObject.armResourceName)]'
                        kind: 'LinuxPerformanceObject'
                        properties: {
                          performanceCounters: '[variables(\'vmInsightsPerfCounters\').linuxCpuArray]'
                          objectName: '[variables(\'vmInsightsPerfCounters\').linuxCpuObject.objectName]'
                          instanceName: '[variables(\'vmInsightsPerfCounters\').linuxCpuObject.instanceName]'
                          intervalSeconds: '[variables(\'vmInsightsPerfCounters\').linuxCpuObject.intervalSeconds]'
                        }
                      }
                      {
                        apiVersion: '2015-11-01-preview'
                        type: 'Microsoft.OperationalInsights/workspaces/dataSources'
                        name: '[concat(parameters(\'workspaceName\'), \'/\', variables(\'vmInsightsPerfCounters\').linuxNetworkObject.armResourceName)]'
                        kind: 'LinuxPerformanceObject'
                        properties: {
                          performanceCounters: '[variables(\'vmInsightsPerfCounters\').linuxNetworkArray]'
                          objectName: '[variables(\'vmInsightsPerfCounters\').linuxNetworkObject.objectName]'
                          instanceName: '[variables(\'vmInsightsPerfCounters\').linuxNetworkObject.instanceName]'
                          intervalSeconds: '[variables(\'vmInsightsPerfCounters\').linuxNetworkObject.intervalSeconds]'
                        }
                      }
                      {
                        apiVersion: '2015-11-01-preview'
                        type: 'Microsoft.OperationalInsights/workspaces/dataSources'
                        name: '[concat(parameters(\'workspaceName\'), \'/\', variables(\'vmInsightsPerfCounters\').windowsArray[copyIndex()].armName)]'
                        kind: 'WindowsPerformanceCounter'
                        copy: {
                          name: 'counterCopy'
                          count: '[length(variables(\'vmInsightsPerfCounters\').windowsArray)]'
                        }
                        properties: {
                          objectName: '[variables(\'vmInsightsPerfCounters\').windowsArray[copyIndex()].objectName]'
                          instanceName: '[variables(\'vmInsightsPerfCounters\').windowsArray[copyIndex()].instanceName]'
                          intervalSeconds: '[variables(\'vmInsightsPerfCounters\').windowsArray[copyIndex()].intervalSeconds]'
                          counterName: '[variables(\'vmInsightsPerfCounters\').windowsArray[copyIndex()].counterName]'
                        }
                      }
                      {
                        apiVersion: '2015-11-01-preview'
                        type: 'Microsoft.OperationsManagement/solutions'
                        name: '[concat(variables(\'batch1\').solutions[copyIndex()].Name)]'
                        location: '[parameters(\'workspaceRegion\')]'
                        copy: {
                          name: 'solutionCopy'
                          count: '[length(variables(\'batch1\').solutions)]'
                        }
                        properties: {
                          workspaceResourceId: '[resourceId(\'Microsoft.OperationalInsights/workspaces/\', parameters(\'workspaceName\'))]'
                        }
                        plan: {
                          name: '[variables(\'batch1\').solutions[copyIndex()].name]'
                          product: '[concat(\'OMSGallery/\', variables(\'batch1\').solutions[copyIndex()].marketplaceName)]'
                          promotionCode: ''
                          publisher: 'Microsoft'
                        }
                      }
                    ]
                    outputs: {}
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-LA-Config'
    }
    {
      properties: {
        Description: 'Deploys Log Analytics and Automation account to the subscription where the policy is assigned.'
        DisplayName: 'Deploy the Log Analytics in the subscription'
        Mode: 'Indexed'
        Parameters: {
          workspaceName: {
            type: 'String'
            metadata: {
              displayName: 'workspaceName'
              description: 'Provide name for log analytics workspace'
            }
          }
          automationAccountName: {
            type: 'String'
            metadata: {
              displayName: 'automationAccountName'
              description: 'Provide name for automation account'
            }
          }
          workspaceRegion: {
            type: 'String'
            metadata: {
              displayName: 'workspaceRegion'
              description: 'Select Azure region for Log Analytics'
            }
          }
          automationRegion: {
            type: 'String'
            metadata: {
              displayName: 'automationRegion'
              description: 'Select Azure region for Automation account'
            }
          }
          retentionInDays: {
            type: 'string'
            defaultValue: '30'
            metadata: {
              displayName: 'Data retention'
              description: 'Select data retention (days) for Log Analytics.'
            }
          }
          rgName: {
            type: 'String'
            metadata: {
              displayName: 'rgName'
              description: 'Provide name for resource group.'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Resources/subscriptions'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.OperationalInsights/workspaces'
              deploymentScope: 'Subscription'
              existenceScope: 'Subscription'
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              existenceCondition: {
                allOf: [
                  {
                    field: 'name'
                    like: '[parameters(\'workspaceName\')]'
                  }
                ]
              }
              deployment: {
                location: 'northeurope'
                properties: {
                  mode: 'incremental'
                  parameters: {
                    rgName: {
                      value: '[parameters(\'rgName\')]'
                    }
                    retentionInDays: {
                      value: '[parameters(\'retentionInDays\')]'
                    }
                    workspaceName: {
                      value: '[parameters(\'workspaceName\')]'
                    }
                    workspaceRegion: {
                      value: '[parameters(\'workspaceRegion\')]'
                    }
                    automationAccountName: {
                      value: '[parameters(\'automationAccountName\')]'
                    }
                    automationRegion: {
                      value: '[parameters(\'automationRegion\')]'
                    }
                  }
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      rgName: {
                        type: 'string'
                      }
                      workspaceName: {
                        type: 'string'
                      }
                      workspaceRegion: {
                        type: 'string'
                      }
                      automationAccountName: {
                        type: 'string'
                      }
                      automationRegion: {
                        type: 'string'
                      }
                      retentionInDays: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Resources/resourceGroups'
                        apiVersion: '2018-05-01'
                        name: '[parameters(\'rgName\')]'
                        location: '[deployment().location]'
                        properties: {}
                      }
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2018-05-01'
                        name: 'log-analytics'
                        resourceGroup: '[parameters(\'rgName\')]'
                        dependsOn: [
                          '[resourceId(\'Microsoft.Resources/resourceGroups/\', parameters(\'rgName\'))]'
                        ]
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            variables: {}
                            resources: [
                              {
                                apiversion: '2015-10-31'
                                location: '[parameters(\'AutomationRegion\')]'
                                name: '[parameters(\'AutomationAccountName\')]'
                                type: 'Microsoft.Automation/automationAccounts'
                                comments: 'Automation account for '
                                properties: {
                                  sku: {
                                    name: 'OMS'
                                  }
                                }
                              }
                              {
                                apiVersion: '2017-03-15-preview'
                                location: '[parameters(\'workspaceRegion\')]'
                                name: '[parameters(\'workspaceName\')]'
                                type: 'Microsoft.OperationalInsights/workspaces'
                                properties: {
                                  sku: {
                                    name: 'pernode'
                                  }
                                  enableLogAccessUsingOnlyResourcePermissions: true
                                  retentionInDays: '[int(parameters(\'retentionInDays\'))]'
                                }
                                resources: [
                                  {
                                    name: 'Automation'
                                    type: 'linkedServices'
                                    apiVersion: '2015-11-01-preview'
                                    dependsOn: [
                                      '[resourceId(\'Microsoft.OperationalInsights/workspaces/\', parameters(\'workspaceName\'))]'
                                      '[resourceId(\'Microsoft.Automation/automationAccounts/\', parameters(\'AutomationAccountName\'))]'
                                    ]
                                    properties: {
                                      resourceId: '[concat(subscription().id, \'/resourceGroups/\', parameters(\'rgName\'), \'/providers/Microsoft.Automation/automationAccounts/\', parameters(\'AutomationAccountName\'))]'
                                    }
                                  }
                                ]
                              }
                            ]
                            outputs: {}
                          }
                        }
                      }
                    ]
                    outputs: {}
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Log-Analytics'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Logic Apps integration service environment to stream to a Log Analytics workspace when any Logic Apps integration service environment which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Logic Apps integration service environment to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Logic/integrationAccounts'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Logic/integrationAccounts/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: []
                          logs: [
                            {
                              category: 'IntegrationAccountTrackingEvents'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-LogicAppsISE'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Logic Apps Workflow runtimeto stream to a Log Analytics workspace when any Logic Apps Workflow runtime which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Logic Apps Workflow runtime to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Logic/workflows'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Logic/workflows/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'WorkflowRuntime'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-LogicAppsWF'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for MariaDB to stream to a Log Analytics workspace when any MariaDB  which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for MariaDB to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.DBforMariaDB/servers'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.DBforMariaDB/servers/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'MySqlSlowLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'MySqlAuditLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-MariaDB'
    }
    {
      properties: {
        Description: 'This policy denies the creation of Maria DB accounts with exposed public endpoints'
        DisplayName: 'Public network access should be disabled for MariaDB'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'SQL'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.DBforMariaDB/servers'
              }
              {
                field: 'Microsoft.DBforMariaDB/servers/publicNetworkAccess'
                notequals: 'Disabled'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-PublicEndpoint-MariaDB'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Machine Learning workspace to stream to a Log Analytics workspace when any Machine Learning workspace which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Machine Learning workspace to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.MachineLearningServices/workspaces'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.MachineLearningServices/workspaces/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'Run'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                            {
                              category: 'Model'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: true
                              }
                            }
                            {
                              category: 'Quota'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                            {
                              category: 'Resource'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'AmlComputeClusterEvent'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AmlComputeClusterNodeEvent'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AmlComputeJobEvent'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AmlComputeCpuGpuUtilization'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AmlRunStatusChangedEvent'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-MlWorkspace'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Database for MySQL to stream to a Log Analytics workspace when any Database for MySQL which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Database for MySQL to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.DBforMySQL/servers'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.DBforMySQL/servers/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'MySqlSlowLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'MySqlAuditLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-MySQL'
    }
    {
      properties: {
        Description: 'This policy denies creation of MySql DB accounts with exposed public endpoints'
        DisplayName: 'Public network access should be disabled for MySQL'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'SQL'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.DBforMySQL/servers'
              }
              {
                field: 'Microsoft.DBforMySQL/servers/publicNetworkAccess'
                notequals: 'Disabled'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-PublicEndpoint-MySQL'
    }
    {
      properties: {
        Description: 'Deploys an Azure DDoS Protection Standard plan'
        DisplayName: 'Deploy an Azure DDoS Protection Standard plan'
        Mode: 'Indexed'
        Parameters: {
          ddosName: {
            type: 'String'
            metadata: {
              displayName: 'ddosName'
              description: 'Name of the Virtual WAN'
            }
          }
          ddosRegion: {
            type: 'String'
            metadata: {
              displayName: 'ddosRegion'
              description: 'Select Azure region for Virtual WAN'
              strongType: 'location'
            }
          }
          rgName: {
            type: 'String'
            metadata: {
              displayName: 'rgName'
              description: 'Provide name for resource group.'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Resources/subscriptions'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Network/ddosProtectionPlans'
              deploymentScope: 'Subscription'
              existenceScope: 'ResourceGroup'
              resourceGroupName: '[parameters(\'rgName\')]'
              name: '[parameters(\'ddosName\')]'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/4d97b98b-1d4f-4787-a291-c67834d212e7'
              ]
              deployment: {
                location: 'northeurope'
                properties: {
                  mode: 'incremental'
                  parameters: {
                    rgName: {
                      value: '[parameters(\'rgName\')]'
                    }
                    ddosname: {
                      value: '[parameters(\'ddosname\')]'
                    }
                    ddosregion: {
                      value: '[parameters(\'ddosRegion\')]'
                    }
                  }
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      rgName: {
                        type: 'string'
                      }
                      ddosname: {
                        type: 'string'
                      }
                      ddosRegion: {
                        type: 'string'
                      }
                    }
                    resources: [
                      {
                        type: 'Microsoft.Resources/resourceGroups'
                        apiVersion: '2018-05-01'
                        name: '[parameters(\'rgName\')]'
                        location: '[deployment().location]'
                        properties: {}
                      }
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2018-05-01'
                        name: 'ddosprotection'
                        resourceGroup: '[parameters(\'rgName\')]'
                        dependsOn: [
                          '[resourceId(\'Microsoft.Resources/resourceGroups/\', parameters(\'rgName\'))]'
                        ]
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            resources: [
                              {
                                type: 'Microsoft.Network/ddosProtectionPlans'
                                apiVersion: '2019-12-01'
                                name: '[parameters(\'ddosName\')]'
                                location: '[parameters(\'ddosRegion\')]'
                                properties: {}
                              }
                            ]
                            outputs: {}
                          }
                        }
                      }
                    ]
                    outputs: {}
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-DDoSProtection'
    }
    {
      properties: {
        Description: 'This policy denies the creation of vNet Peerings under the assigned scope.'
        DisplayName: 'Deny vNet peering '
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings'
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-VNet-Peering'
    }
    {
      properties: {
        Description: 'This policy denies the creation of a private DNS in the current scope, used in combination with policies that create centralized private DNS in connectivity subscription'
        DisplayName: 'Deny the creation of private DNS'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/privateDnsZones'
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-Private-DNS-Zones'
    }
    {
      properties: {
        Description: 'This policy denies creation of Public IPs under the assigned scope.'
        DisplayName: 'Deny the creation of public IP'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/publicIPAddresses'
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-PublicIP'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Network Interfaces to stream to a Log Analytics workspace when any Network Interfaces which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Network Interfaces to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/networkInterfaces'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Network/networkInterfaces/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              timeGrain: null
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                enabled: false
                                days: 0
                              }
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-NIC'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Public IP addresses to stream to a Log Analytics workspace when any Public IP addresses which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Public IP addresses to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/publicIPAddresses'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Network/publicIPAddresses/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              timeGrain: null
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                enabled: false
                                days: 0
                              }
                            }
                          ]
                          logs: [
                            {
                              category: 'DDoSProtectionNotifications'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'DDoSMitigationFlowLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'DDoSMitigationReports'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-PublicIP'
    }
    {
      properties: {
        Description: 'Deploys NSG flow logs and traffic analytics to a storageaccountid with a specfied retention period.'
        DisplayName: 'Deploys NSG flow logs and traffic analytics'
        Mode: 'Indexed'
        Parameters: {
          retention: {
            type: 'Integer'
            metadata: {
              displayName: 'Retention'
            }
            defaultValue: 5
          }
          storageAccountResourceId: {
            type: 'String'
            metadata: {
              displayName: 'Storage Account Resource Id'
              strongType: 'Microsoft.Storage/storageAccounts'
            }
          }
          trafficAnalyticsInterval: {
            type: 'Integer'
            metadata: {
              displayName: 'Traffic Analytics processing interval mins (10/60)'
            }
            defaultValue: 60
          }
          flowAnalyticsEnabled: {
            type: 'Boolean'
            metadata: {
              displayName: 'Enable Traffic Analytics'
            }
            defaultValue: false
          }
          logAnalytics: {
            type: 'String'
            metadata: {
              strongType: 'omsWorkspace'
              displayName: 'Resource ID of Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
            }
            defaultValue: ''
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/networkSecurityGroups'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Network/networkWatchers/flowLogs'
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              resourceGroupName: 'NetworkWatcherRG'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Network/networkWatchers/flowLogs/enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Network/networkWatchers/flowLogs/flowAnalyticsConfiguration.networkWatcherFlowAnalyticsConfiguration.enabled'
                    equals: '[parameters(\'flowAnalyticsEnabled\')]'
                  }
                ]
              }
              deployment: {
                properties: {
                  mode: 'incremental'
                  parameters: {
                    networkSecurityGroupName: {
                      value: '[field(\'name\')]'
                    }
                    resourceGroupName: {
                      value: '[resourceGroup().name]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    storageAccountResourceId: {
                      value: '[parameters(\'storageAccountResourceId\')]'
                    }
                    retention: {
                      value: '[parameters(\'retention\')]'
                    }
                    flowAnalyticsEnabled: {
                      value: '[parameters(\'flowAnalyticsEnabled\')]'
                    }
                    trafficAnalyticsInterval: {
                      value: '[parameters(\'trafficAnalyticsInterval\')]'
                    }
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                  }
                  template: {
                    '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      networkSecurityGroupName: {
                        type: 'string'
                      }
                      resourceGroupName: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      storageAccountResourceId: {
                        type: 'string'
                      }
                      retention: {
                        type: 'int'
                      }
                      flowAnalyticsEnabled: {
                        type: 'bool'
                      }
                      trafficAnalyticsInterval: {
                        type: 'int'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Network/networkWatchers/flowLogs'
                        apiVersion: '2020-05-01'
                        name: '[take(concat(\'NetworkWatcher_\', toLower(parameters(\'location\')),  \'/\', parameters(\'networkSecurityGroupName\'), \'-\', parameters(\'resourceGroupName\'), \'-flowlog\' ), 80)]'
                        location: '[parameters(\'location\')]'
                        properties: {
                          targetResourceId: '[resourceId(parameters(\'resourceGroupName\'), \'Microsoft.Network/networkSecurityGroups\', parameters(\'networkSecurityGroupName\'))]'
                          storageId: '[parameters(\'storageAccountResourceId\')]'
                          enabled: true
                          retentionPolicy: {
                            enabled: true
                            days: '[parameters(\'retention\')]'
                          }
                          format: {
                            type: 'JSON'
                            version: 2
                          }
                          flowAnalyticsConfiguration: {
                            networkWatcherFlowAnalyticsConfiguration: {
                              enabled: '[bool(parameters(\'flowAnalyticsEnabled\'))]'
                              trafficAnalyticsInterval: '[parameters(\'trafficAnalyticsInterval\')]'
                              workspaceId: '[if(not(empty(parameters(\'logAnalytics\'))), reference(parameters(\'logAnalytics\'), \'2020-03-01-preview\', \'Full\').properties.customerId, json(\'null\')) ]'
                              workspaceRegion: '[if(not(empty(parameters(\'logAnalytics\'))), reference(parameters(\'logAnalytics\'), \'2020-03-01-preview\', \'Full\').location, json(\'null\')) ]'
                              workspaceResourceId: '[if(not(empty(parameters(\'logAnalytics\'))), parameters(\'logAnalytics\'), json(\'null\'))]'
                            }
                          }
                        }
                      }
                    ]
                    outputs: {}
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Nsg-FlowLogs'
    }
    {
      properties: {
        Description: 'This policy denies the creation of a subsnet with out an Network Security Group. NSG help to protect traffic across subnet-level.'
        DisplayName: 'Subnets should have a Network Security Group '
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Network/virtualNetworks/subnets'
              }
              {
                field: 'Microsoft.Network/virtualNetworks/subnets/networkSecurityGroup.id'
                exists: 'false'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-Subnet-Without-Nsg'
    }
    {
      properties: {
        displayName: 'Subnets should have a User Defined Route'
        policyType: 'Custom'
        mode: 'Indexed'
        description: 'This policy denies the creation of a subsnet with out a User Defined Route.'
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        parameters: {
          effect: {
            type: 'String'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
          }
        }
        policyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Network/virtualNetworks/subnets'
              }
              {
                field: 'Microsoft.Network/virtualNetworks/subnets/routeTable.id'
                exists: 'false'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-Subnet-Without-Udr'
    }
    {
      Properties: {
        Description: 'This policy denies the creation of vNet Peerings outside of the same subscriptions under the assigned scope.'
        DisplayName: 'Deny vNet peering cross subscription.'
        Mode: 'Indexed'
        metadata: {
          version: '1.0.0.0'
          category: 'Network'
        }
        Parameters: {
          effect: {
            type: 'String'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
          }
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings'
              }
              {
                field: 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings/remoteVirtualNetwork.id'
                notcontains: '[subscription().id]'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-VNET-Peer-Cross-Sub'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Network Security Groups to stream to a Log Analytics workspace when any Network Security Groups which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Network Security Groups to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/networkSecurityGroups'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Network/networkSecurityGroups/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: []
                          logs: [
                            {
                              category: 'NetworkSecurityGroupEvent'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'NetworkSecurityGroupRuleCounter'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-NetworkSecurityGroups'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Database for PostgreSQL to stream to a Log Analytics workspace when any Database for PostgreSQL which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Database for PostgreSQL to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.DBforPostgreSQL/servers'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.DBforPostgreSQL/servers/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'PostgreSQLLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'QueryStoreRuntimeStatistics'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'QueryStoreWaitStatistics'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-PostgreSQL'
    }
    {
      properties: {
        Description: 'This policy denies creation of Postgre SQL DB accounts with exposed public endpoints'
        DisplayName: 'Public network access should be disabled for PostgreSql'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'SQL'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.DBforPostgreSQL/servers'
              }
              {
                field: 'Microsoft.DBforPostgreSQL/servers/publicNetworkAccess'
                notequals: 'Disabled'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-PublicEndpoint-PostgreSql'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Power BI Embedded to stream to a Log Analytics workspace when any Power BI Embedded which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Power BI Embedded to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.PowerBIDedicated/capacities'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.PowerBIDedicated/capacities/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'Engine'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-PowerBIEmbedded'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Recovery Services vaults to stream to a Log Analytics workspace when any Recovery Services vaults which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Recovery Services vaults to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.RecoveryServices/vaults'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allof: [
                  {
                    count: {
                      field: 'Microsoft.Insights/diagnosticSettings/logs[*]'
                      where: {
                        allof: [
                          {
                            field: 'Microsoft.Insights/diagnosticSettings/logs[*].Category'
                            in: [
                              'CoreAzureBackup'
                              'AddonAzureBackupJobs'
                              'AddonAzureBackupAlerts'
                              'AddonAzureBackupPolicy'
                              'AddonAzureBackupStorage'
                              'AddonAzureBackupProtectedInstance'
                              'AzureBackupReport'
                            ]
                          }
                          {
                            field: 'Microsoft.Insights/diagnosticSettings/logs[*].Enabled'
                            equals: 'True'
                          }
                        ]
                      }
                    }
                    Equals: 7
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logAnalyticsDestinationType'
                    equals: 'Dedicated'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.RecoveryServices/vaults/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          logAnalyticsDestinationType: 'Dedicated'
                          metrics: []
                          logs: [
                            {
                              category: 'CoreAzureBackup'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AddonAzureBackupAlerts'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AddonAzureBackupJobs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AddonAzureBackupPolicy'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AddonAzureBackupProtectedInstance'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AddonAzureBackupStorage'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AzureBackupReport'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-RecoveryVault'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Redis Cache to stream to a Log Analytics workspace when any Redis Cache which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Redis Cache to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Cache/redis'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Cache/redis/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: []
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-RedisCache'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Relay to stream to a Log Analytics workspace when any Relay which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Relay to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Relay/namespaces'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Relay/namespaces/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'HybridConnectionsEvent'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-Relay'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Search Services to stream to a Log Analytics workspace when any Search Services which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Search Services to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Search/searchServices'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Search/searchServices/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'OperationLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-SearchServices'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for ServiceBus to stream to a Log Analytics workspace when any ServiceBus which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Service Bus namespaces  to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.ServiceBus/namespaces'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.ServiceBus/namespaces/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'OperationalLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-ServiceBus'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for SignalR to stream to a Log Analytics workspace when any SignalR which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for SignalR to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.SignalRService/SignalR'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.SignalRService/SignalR/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'AllLogs'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-SignalR'
    }
    {
      properties: {
        Description: 'Deploy auditing settings to SQL Database when it not exist in the deployment'
        DisplayName: 'Deploy SQL database auditing settings'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'SQL'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Sql/servers/databases'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Sql/servers/databases/auditingSettings'
              name: 'default'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Sql/servers/databases/auditingSettings/state'
                    equals: 'enabled'
                  }
                  {
                    field: 'Microsoft.Sql/servers/databases/auditingSettings/isAzureMonitorTargetEnabled'
                    equals: 'true'
                  }
                ]
              }
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      location: {
                        type: 'string'
                      }
                      sqlServerName: {
                        type: 'string'
                      }
                      sqlServerDataBaseName: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        name: '[concat( parameters(\'sqlServerName\'),\'/\',parameters(\'sqlServerDataBaseName\'),\'/default\')]'
                        type: 'Microsoft.Sql/servers/databases/auditingSettings'
                        apiVersion: '2017-03-01-preview'
                        properties: {
                          state: 'enabled'
                          auditActionsAndGroups: [
                            'BATCH_COMPLETED_GROUP'
                            'DATABASE_OBJECT_CHANGE_GROUP'
                            'SCHEMA_OBJECT_CHANGE_GROUP'
                            'BACKUP_RESTORE_GROUP'
                            'APPLICATION_ROLE_CHANGE_PASSWORD_GROUP'
                            'DATABASE_PRINCIPAL_CHANGE_GROUP'
                            'DATABASE_PRINCIPAL_IMPERSONATION_GROUP'
                            'DATABASE_ROLE_MEMBER_CHANGE_GROUP'
                            'USER_CHANGE_PASSWORD_GROUP'
                            'DATABASE_OBJECT_OWNERSHIP_CHANGE_GROUP'
                            'DATABASE_OBJECT_PERMISSION_CHANGE_GROUP'
                            'DATABASE_PERMISSION_CHANGE_GROUP'
                            'SCHEMA_OBJECT_PERMISSION_CHANGE_GROUP'
                            'SUCCESSFUL_DATABASE_AUTHENTICATION_GROUP'
                            'FAILED_DATABASE_AUTHENTICATION_GROUP'
                          ]
                          isAzureMonitorTargetEnabled: true
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    location: {
                      value: '[field(\'location\')]'
                    }
                    sqlServerName: {
                      value: '[first(split(field(\'fullname\'),\'/\'))]'
                    }
                    sqlServerDataBaseName: {
                      value: '[field(\'name\')]'
                    }
                  }
                }
              }
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/056cd41c-7e88-42e1-933e-88ba6a50c9c3'
              ]
            }
          }
        }
      }
      name: 'Deploy-Sql-AuditingSettings'
    }
    {
      properties: {
        Description: 'Deploy the Transparent Data Encryption when it is not enabled in the deployment'
        DisplayName: 'Deploy SQL Database Transparent Data Encryption '
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'SQL'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Sql/servers/databases'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Sql/servers/databases/transparentDataEncryption'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Sql/transparentDataEncryption.status'
                    equals: 'Enabled'
                  }
                ]
              }
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      location: {
                        type: 'string'
                      }
                      sqlServerName: {
                        type: 'string'
                      }
                      sqlServerDataBaseName: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        name: '[concat( parameters(\'sqlServerName\'),\'/\',parameters(\'sqlServerDataBaseName\'),\'/current\')]'
                        type: 'Microsoft.Sql/servers/databases/transparentDataEncryption'
                        apiVersion: '2014-04-01'
                        properties: {
                          status: 'Enabled'
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    location: {
                      value: '[field(\'location\')]'
                    }
                    sqlServerName: {
                      value: '[first(split(field(\'fullname\'),\'/\'))]'
                    }
                    sqlServerDataBaseName: {
                      value: '[field(\'name\')]'
                    }
                  }
                }
              }
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/056cd41c-7e88-42e1-933e-88ba6a50c9c3'
              ]
            }
          }
        }
      }
      name: 'Deploy-Sql-Tde'
    }
    {
      properties: {
        Description: 'Deploy the security Alert Policies configuration with email admin accounts when it not exist in current configuration'
        DisplayName: 'Deploy SQL Database security Alert Policies configuration with email admin accounts'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'SQL'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Sql/servers/databases'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Sql/servers/databases/securityAlertPolicies'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Sql/servers/databases/securityAlertPolicies/state'
                    equals: 'Enabled'
                  }
                ]
              }
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      location: {
                        type: 'string'
                      }
                      sqlServerName: {
                        type: 'string'
                      }
                      sqlServerDataBaseName: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        name: '[concat(parameters(\'sqlServerName\'),\'/\',parameters(\'sqlServerDataBaseName\'),\'/default\')]'
                        type: 'Microsoft.Sql/servers/databases/securityAlertPolicies'
                        apiVersion: '2018-06-01-preview'
                        properties: {
                          state: 'Enabled'
                          disabledAlerts: [
                            ''
                          ]
                          emailAddresses: [
                            'admin@contoso.com'
                          ]
                          emailAccountAdmins: true
                          storageEndpoint: null
                          storageAccountAccessKey: ''
                          retentionDays: 0
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    location: {
                      value: '[field(\'location\')]'
                    }
                    sqlServerName: {
                      value: '[first(split(field(\'fullname\'),\'/\'))]'
                    }
                    sqlServerDataBaseName: {
                      value: '[field(\'name\')]'
                    }
                  }
                }
              }
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/056cd41c-7e88-42e1-933e-88ba6a50c9c3'
              ]
            }
          }
        }
      }
      name: 'Deploy-Sql-SecurityAlertPolicies'
    }
    {
      properties: {
        Description: 'Deploy SQL Database vulnerability Assessments when it not exist in the deployment. To the specific  storage account in the parameters'
        DisplayName: 'Deploy SQL Database vulnerability Assessments'
        Mode: 'Indexed'
        Parameters: {
          vulnerabilityAssessmentsEmail: {
            type: 'String'
            metadata: {
              description: 'The email address to send alerts'
              displayName: 'The email address to send alerts'
            }
          }
          vulnerabilityAssessmentsStorageID: {
            type: 'String'
            metadata: {
              description: 'The storage account to store assessments'
              displayName: 'The storage account to store assessments'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'SQL'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Sql/servers/databases'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Sql/servers/databases/vulnerabilityAssessments'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Sql/servers/databases/vulnerabilityAssessments/recurringScans.emails'
                    equals: '[parameters(\'vulnerabilityAssessmentsEmail\')]'
                  }
                  {
                    field: 'Microsoft.Sql/servers/databases/vulnerabilityAssessments/recurringScans.isEnabled'
                    equals: true
                  }
                ]
              }
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      location: {
                        type: 'string'
                      }
                      sqlServerName: {
                        type: 'string'
                      }
                      sqlServerDataBaseName: {
                        type: 'string'
                      }
                      vulnerabilityAssessmentsEmail: {
                        type: 'string'
                      }
                      vulnerabilityAssessmentsStorageID: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        name: '[concat(parameters(\'sqlServerName\'),\'/\',parameters(\'sqlServerDataBaseName\'),\'/default\')]'
                        type: 'Microsoft.Sql/servers/databases/vulnerabilityAssessments'
                        apiVersion: '2017-03-01-preview'
                        properties: {
                          storageContainerPath: '[concat(\'https://\', last( split(parameters(\'vulnerabilityAssessmentsStorageID\') ,  \'/\') ) , \'.blob.core.windows.net/vulneraabilitylogs\')]'
                          storageAccountAccessKey: '[listkeys(parameters(\'vulnerabilityAssessmentsStorageID\'), providers(\'Microsoft.Storage\', \'storageAccounts\').apiVersions[0]).keys[0].value]'
                          recurringScans: {
                            isEnabled: true
                            emailSubscriptionAdmins: false
                            emails: [
                              '[parameters(\'vulnerabilityAssessmentsEmail\')]'
                            ]
                          }
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    location: {
                      value: '[field(\'location\')]'
                    }
                    sqlServerName: {
                      value: '[first(split(field(\'fullname\'),\'/\'))]'
                    }
                    sqlServerDataBaseName: {
                      value: '[field(\'name\')]'
                    }
                    vulnerabilityAssessmentsEmail: {
                      value: '[parameters(\'vulnerabilityAssessmentsEmail\')]'
                    }
                    vulnerabilityAssessmentsStorageID: {
                      value: '[parameters(\'vulnerabilityAssessmentsStorageID\')]'
                    }
                  }
                }
              }
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/056cd41c-7e88-42e1-933e-88ba6a50c9c3'
                '/providers/Microsoft.Authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
              ]
            }
          }
        }
      }
      name: 'Deploy-Sql-vulnerabilityAssessments'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for SQL Databases to stream to a Log Analytics workspace when any SQL Databases  which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for SQL Databases  to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Sql/servers/databases'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Sql/servers/databases/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'SQLInsights'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'AutomaticTuning'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'DevOpsOperationsAudit'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'QueryStoreRuntimeStatistics'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'QueryStoreWaitStatistics'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Errors'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'DatabaseWaitStatistics'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Timeouts'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Blocks'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Deadlocks'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'SQLSecurityAuditEvents'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'fullName\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-SQLDBs'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for SQL Elastic Pools to stream to a Log Analytics workspace when any SQL Elastic Pools which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for SQL Elastic Pools to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Sql/servers/elasticPools'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Sql/servers/elasticPools/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: []
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'fullName\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-SQLElasticPools'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for SQL Managed Instances to stream to a Log Analytics workspace when any SQL Managed Instances which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for SQL Managed Instances to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Sql/managedInstances'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Sql/managedInstances/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          logs: [
                            {
                              category: 'ResourceUsageStats'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'SQLSecurityAuditEvents'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'DevOpsOperationsAudit'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-SQLMI'
    }
    {
      properties: {
        Description: 'This policy denies creation of Sql servers with exposed public endpoints'
        DisplayName: 'Public network access on Azure SQL Database should be disabled'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'SQL'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Sql/servers'
              }
              {
                field: 'Microsoft.Sql/servers/publicNetworkAccess'
                notequals: 'Disabled'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-PublicEndpoint-Sql'
    }
    {
      properties: {
        Description: 'This policy denies creation of storage accounts with IP Firewall exposed to all public endpoints'
        DisplayName: 'Public network access onStorage accounts should be disabled'
        Mode: 'Indexed'
        Parameters: {
          effect: {
            type: 'String'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            defaultValue: 'Deny'
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Storage'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Storage/storageAccounts'
              }
              {
                field: 'Microsoft.Storage/storageAccounts/networkAcls.defaultAction'
                notequals: 'Deny'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
          }
        }
      }
      name: 'Deny-PublicEndpoint-Storage'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Stream Analytics to stream to a Log Analytics workspace when any Stream Analytics which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Stream Analytics to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.StreamAnalytics/streamingjobs'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.StreamAnalytics/streamingjobs/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'Execution'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Authoring'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-StreamAnalytics'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Time Series Insights to stream to a Log Analytics workspace when any Time Series Insights which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Time Series Insights to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.TimeSeriesInsights/environments'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.TimeSeriesInsights/environments/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'Ingress'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'Management'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-TimeSeriesInsights'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Traffic Manager to stream to a Log Analytics workspace when any Traffic Manager which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Traffic Manager to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/trafficManagerProfiles'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Network/trafficManagerProfiles/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'ProbeHealthStatusEvents'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-TrafficManager'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Virtual Machines to stream to a Log Analytics workspace when any Virtual Machines which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Virtual Machines to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Compute/virtualMachines'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Compute/virtualMachines/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                enabled: false
                                days: 0
                              }
                            }
                          ]
                          logs: []
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-VM'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Virtual Network to stream to a Log Analytics workspace when any Virtual Network which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Virtual Network to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/virtualNetworks'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Network/virtualNetworks/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                enabled: false
                                days: 0
                              }
                            }
                          ]
                          logs: [
                            {
                              category: 'VMProtectionAlerts'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-VirtualNetwork'
    }
    {
      properties: {
        Description: 'Deploys Virtual Network to be used as hub virtual network in desired region in the subscription where this policy is assigned.'
        DisplayName: 'Deploy Virtual Network to be used as hub virtual network in desired region'
        Mode: 'Indexed'
        Parameters: {
          hubName: {
            type: 'String'
            metadata: {
              displayName: 'hubName'
              description: 'Name of the Hub'
            }
          }
          HUB: {
            type: 'Object'
            metadata: {
              displayName: 'HUB'
              description: 'Object describing HUB'
            }
          }
          vpngw: {
            type: 'Object'
            metadata: {
              displayName: 'vpngw'
              description: 'Object describing VPN gateway'
            }
            defaultValue: {}
          }
          ergw: {
            type: 'Object'
            metadata: {
              displayName: 'ergw'
              description: 'Object describing ExpressRoute gateway'
            }
            defaultValue: {}
          }
          azfw: {
            type: 'Object'
            metadata: {
              displayName: 'ergw'
              description: 'Object describing ExpressRoute gateway'
            }
            defaultValue: {}
          }
          rgName: {
            type: 'String'
            metadata: {
              displayName: 'rgName'
              description: 'Provide name for resource group.'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Resources/subscriptions'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Network/virtualNetworks'
              name: '[parameters(\'hubName\')]'
              deploymentScope: 'Subscription'
              existenceScope: 'ResourceGroup'
              ResourceGroupName: '[parameters(\'rgName\')]'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/4d97b98b-1d4f-4787-a291-c67834d212e7'
              ]
              deployment: {
                location: 'northeurope'
                properties: {
                  mode: 'incremental'
                  parameters: {
                    rgName: {
                      value: '[parameters(\'rgName\')]'
                    }
                    hubName: {
                      value: '[parameters(\'hubName\')]'
                    }
                    HUB: {
                      value: '[parameters(\'HUB\')]'
                    }
                    vpngw: {
                      value: '[parameters(\'vpngw\')]'
                    }
                    ergw: {
                      value: '[parameters(\'ergw\')]'
                    }
                    azfw: {
                      value: '[parameters(\'azfw\')]'
                    }
                  }
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      hubName: {
                        type: 'string'
                        metadata: {
                          description: 'Name of the HUB'
                        }
                      }
                      HUB: {
                        type: 'object'
                        metadata: {
                          description: 'Object describing HUB'
                        }
                      }
                      vpngw: {
                        type: 'object'
                        defaultValue: {}
                        metadata: {
                          description: 'Object describing VPN gateway'
                        }
                      }
                      ergw: {
                        type: 'object'
                        defaultValue: {}
                        metadata: {
                          description: 'Object describing ExpressRoute gateway'
                        }
                      }
                      azfw: {
                        type: 'object'
                        defaultValue: {}
                        metadata: {
                          description: 'Object describing the Azure Firewall'
                        }
                      }
                      rgName: {
                        type: 'String'
                        metadata: {
                          displayName: 'rgName'
                          description: 'Provide name for resource group.'
                        }
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Resources/resourceGroups'
                        apiVersion: '2020-06-01'
                        name: '[parameters(\'rgName\')]'
                        location: '[deployment().location]'
                        properties: {}
                      }
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2020-06-01'
                        name: '[concat(parameters(\'hubName\'),\'-\', parameters(\'HUB\').location)]'
                        resourceGroup: '[parameters(\'rgName\')]'
                        dependsOn: [
                          '[resourceId(\'Microsoft.Resources/resourceGroups/\', parameters(\'rgName\'))]'
                        ]
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'https: //schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            variables: {}
                            resources: [
                              {
                                name: '[parameters(\'hubName\')]'
                                type: 'Microsoft.Network/virtualNetworks'
                                apiVersion: '2020-04-01'
                                location: '[parameters(\'HUB\').location]'
                                properties: {
                                  addressSpace: {
                                    addressPrefixes: [
                                      '[parameters(\'HUB\').addressPrefix]'
                                    ]
                                  }
                                  subnets: [
                                    {
                                      name: 'Infrastructure'
                                      properties: {
                                        addressPrefix: '[if(not(empty(parameters(\'HUB\').subnets.infra)),parameters(\'HUB\').subnets.infra, json(\'null\'))]'
                                      }
                                    }
                                    {
                                      name: 'AzureFirewallSubnet'
                                      properties: {
                                        addressPrefix: '[if(not(empty(parameters(\'HUB\').subnets.azfw)),parameters(\'HUB\').subnets.azfw, json(\'null\'))]'
                                      }
                                    }
                                    {
                                      name: 'GatewaySubnet'
                                      properties: {
                                        addressPrefix: '[if(not(empty(parameters(\'HUB\').subnets.gw)),parameters(\'HUB\').subnets.gw, json(\'null\'))]'
                                      }
                                    }
                                  ]
                                }
                              }
                            ]
                          }
                        }
                      }
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2020-06-01'
                        condition: '[greater(length(parameters(\'vpngw\')),0)]'
                        resourceGroup: '[parameters(\'rgName\')]'
                        dependsOn: [
                          '[concat(parameters(\'hubName\'),\'-\', parameters(\'HUB\').location)]'
                        ]
                        name: '[concat(parameters(\'hubName\'),\'-vpngw\')]'
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            variables: {}
                            resources: [
                              {
                                apiVersion: '2020-05-01'
                                type: 'Microsoft.Network/publicIpAddresses'
                                location: '[parameters(\'HUB\').location]'
                                name: '[concat(parameters(\'vpngw\').name,\'-pip\')]'
                                properties: {
                                  publicIPAllocationMethod: 'Dynamic'
                                }
                                tags: {}
                              }
                              {
                                apiVersion: '2020-05-01'
                                name: '[parameters(\'vpngw\').name]'
                                type: 'Microsoft.Network/virtualNetworkGateways'
                                location: '[parameters(\'HUB\').location]'
                                dependsOn: [
                                  '[concat(\'Microsoft.Network/publicIPAddresses/\', parameters(\'vpngw\').name,\'-pip\')]'
                                ]
                                tags: {}
                                properties: {
                                  gatewayType: 'Vpn'
                                  vpnType: '[parameters(\'vpngw\').vpnType]'
                                  ipConfigurations: [
                                    {
                                      name: 'default'
                                      properties: {
                                        privateIPAllocationMethod: 'Dynamic'
                                        subnet: {
                                          id: '[concat(subscription().id,\'/resourceGroups/\',parameters(\'rgName\'), \'/providers\',\'/Microsoft.Network/virtualNetworks/\', parameters(\'hubName\'),\'/subnets/GatewaySubnet\')]'
                                        }
                                        publicIpAddress: {
                                          id: '[concat(subscription().id,\'/resourceGroups/\',parameters(\'rgName\'), \'/providers\',\'/Microsoft.Network/publicIPAddresses/\', parameters(\'vpngw\').name,\'-pip\')]'
                                        }
                                      }
                                    }
                                  ]
                                  sku: {
                                    name: '[parameters(\'vpngw\').sku]'
                                    tier: '[parameters(\'vpngw\').sku]'
                                  }
                                }
                              }
                            ]
                          }
                        }
                      }
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2020-06-01'
                        condition: '[greater(length(parameters(\'ergw\')),0)]'
                        resourceGroup: '[parameters(\'rgName\')]'
                        dependsOn: [
                          '[concat(parameters(\'hubName\'),\'-\', parameters(\'HUB\').location)]'
                        ]
                        name: '[concat(parameters(\'hubName\'),\'-ergw\')]'
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            variables: {}
                            resources: [
                              {
                                apiVersion: '2020-05-01'
                                type: 'Microsoft.Network/publicIpAddresses'
                                location: '[parameters(\'HUB\').location]'
                                name: '[concat(parameters(\'ergw\').name,\'-pip\')]'
                                properties: {
                                  publicIPAllocationMethod: 'Dynamic'
                                }
                                tags: {}
                              }
                              {
                                apiVersion: '2020-05-01'
                                name: '[parameters(\'ergw\').name]'
                                type: 'Microsoft.Network/virtualNetworkGateways'
                                location: '[parameters(\'HUB\').location]'
                                dependsOn: [
                                  '[concat(\'Microsoft.Network/publicIPAddresses/\', parameters(\'ergw\').name,\'-pip\')]'
                                ]
                                tags: {}
                                properties: {
                                  gatewayType: 'ExpressRoute'
                                  ipConfigurations: [
                                    {
                                      name: 'default'
                                      properties: {
                                        privateIPAllocationMethod: 'Dynamic'
                                        subnet: {
                                          id: '[concat(subscription().id,\'/resourceGroups/\',parameters(\'rgName\'), \'/providers\',\'/Microsoft.Network/virtualNetworks/\', parameters(\'hubName\'),\'/subnets/GatewaySubnet\')]'
                                        }
                                        publicIpAddress: {
                                          id: '[concat(subscription().id,\'/resourceGroups/\',parameters(\'rgName\'), \'/providers\',\'/Microsoft.Network/publicIPAddresses/\', parameters(\'ergw\').name,\'-pip\')]'
                                        }
                                      }
                                    }
                                  ]
                                  sku: {
                                    name: '[parameters(\'ergw\').sku]'
                                    tier: '[parameters(\'ergw\').sku]'
                                  }
                                }
                              }
                            ]
                          }
                        }
                      }
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2020-06-01'
                        condition: '[greater(length(parameters(\'azfw\')),0)]'
                        name: '[concat(parameters(\'hubName\'),\'-azfw\')]'
                        resourceGroup: '[parameters(\'rgName\')]'
                        dependsOn: [
                          '[concat(parameters(\'hubName\'),\'-\', parameters(\'HUB\').location)]'
                        ]
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            variables: {}
                            resources: [
                              {
                                apiVersion: '2020-05-01'
                                type: 'Microsoft.Network/publicIpAddresses'
                                name: '[concat(parameters(\'azfw\').name,\'-pip\')]'
                                location: '[parameters(\'azfw\').location]'
                                sku: {
                                  name: 'Standard'
                                }
                                zones: '[if(contains(parameters(\'azfw\'),\'pipZones\'),parameters(\'azfw\').pipZones,json(\'null\'))]'
                                properties: {
                                  publicIPAllocationMethod: 'Static'
                                }
                                tags: {}
                              }
                              {
                                apiVersion: '2020-05-01'
                                type: 'Microsoft.Network/azureFirewalls'
                                name: '[parameters(\'azfw\').name]'
                                location: '[parameters(\'azfw\').location]'
                                zones: '[if(contains(parameters(\'azfw\'),\'fwZones\'),parameters(\'azfw\').fwZones,json(\'null\'))]'
                                dependsOn: [
                                  '[concat(parameters(\'azfw\').name,\'-pip\')]'
                                ]
                                properties: {
                                  threatIntelMode: '[parameters(\'azfw\').threatIntelMode]'
                                  additionalProperties: '[if(contains(parameters(\'azfw\'),\'additionalProperties\'),parameters(\'azfw\').additionalProperties,json(\'null\'))]'
                                  sku: '[if(contains(parameters(\'azfw\'),\'sku\'),parameters(\'azfw\').sku,json(\'null\'))]'
                                  ipConfigurations: [
                                    {
                                      name: '[concat(parameters(\'azfw\').name,\'-pip\')]'
                                      properties: {
                                        subnet: {
                                          id: '[concat(subscription().id,\'/resourceGroups/\',parameters(\'rgName\'), \'/providers\',\'/Microsoft.Network/virtualNetworks/\', parameters(\'hubName\'),\'/subnets/AzureFirewallSubnet\')]'
                                        }
                                        publicIPAddress: {
                                          id: '[concat(subscription().id,\'/resourceGroups/\',parameters(\'rgName\'), \'/providers\',\'/Microsoft.Network/publicIPAddresses/\', parameters(\'azfw\').name,\'-pip\')]'
                                        }
                                      }
                                    }
                                  ]
                                  firewallPolicy: '[if(contains(parameters(\'azfw\'),\'firewallPolicy\'),parameters(\'azfw\').firewallPolicy,json(\'null\'))]'
                                }
                                tags: {}
                              }
                            ]
                          }
                        }
                      }
                    ]
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-HUB'
    }
    {
      properties: {
        Description: 'Deploy spoke network with configuration to hub network based on ipam configuration object'
        DisplayName: 'Deploy spoke network with configuration to hub network based on ipam configuration object'
        Mode: 'Indexed'
        Parameters: {
          ipam: {
            type: 'Array'
            metadata: {
              displayName: 'ipam'
              description: null
            }
            defaultValue: []
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Resources/subscriptions'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Resources/resourceGroups'
              deploymentScope: 'Subscription'
              existenceScope: 'Subscription'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/4d97b98b-1d4f-4787-a291-c67834d212e7'
              ]
              existenceCondition: {
                allOf: [
                  {
                    field: 'type'
                    equals: 'Microsoft.Resources/subscriptions/resourceGroups'
                  }
                  {
                    field: 'name'
                    like: '[concat(subscription().displayName, \'-network\')]'
                  }
                ]
              }
              deployment: {
                location: 'northeurope'
                properties: {
                  mode: 'incremental'
                  parameters: {
                    ipam: {
                      value: '[parameters(\'ipam\')]'
                      defaultValue: []
                    }
                  }
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      ipam: {
                        defaultValue: [
                          {
                            name: 'bu1-weu-msx3-vNet1'
                            location: 'westeurope'
                            virtualNetworks: {
                              properties: {
                                addressSpace: {
                                  addressPrefixes: [
                                    '10.51.217.0/24'
                                  ]
                                }
                              }
                            }
                            networkSecurityGroups: {
                              properties: {
                                securityRules: []
                              }
                            }
                            routeTables: {
                              properties: {
                                routes: []
                              }
                            }
                            hubVirtualNetworkConnection: {
                              vWanVhubResourceId: '/subscriptions/99c2838f-a548-4884-a6e2-38c1f8fb4c0b/resourceGroups/contoso-global-vwan/providers/Microsoft.Network/virtualHubs/contoso-vhub-weu'
                              properties: {
                                allowHubToRemoteVnetTransit: true
                                allowRemoteVnetToUseHubVnetGateways: false
                                enableInternetSecurity: true
                              }
                            }
                          }
                        ]
                        type: 'Array'
                      }
                    }
                    variables: {
                      vNetRgName: '[concat(subscription().displayName, \'-network\')]'
                      vNetName: '[concat(subscription().displayName, \'-vNet\')]'
                      vNetSubId: '[subscription().subscriptionId]'
                    }
                    resources: [
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2020-06-01'
                        name: '[concat(\'es-ipam-\',subscription().displayName,\'-RG-\',copyIndex())]'
                        location: '[parameters(\'ipam\')[copyIndex()].location]'
                        dependsOn: []
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            variables: {}
                            resources: [
                              {
                                type: 'Microsoft.Resources/resourceGroups'
                                apiVersion: '2020-06-01'
                                name: '[variables(\'vNetRgName\')]'
                                location: '[parameters(\'ipam\')[copyIndex()].location]'
                                properties: {}
                              }
                              {
                                type: 'Microsoft.Resources/resourceGroups'
                                apiVersion: '2020-06-01'
                                name: 'NetworkWatcherRG'
                                location: '[parameters(\'ipam\')[copyIndex()].location]'
                                properties: {}
                              }
                            ]
                            outputs: {}
                          }
                        }
                        copy: {
                          name: 'ipam-rg-loop'
                          count: '[length(parameters(\'ipam\'))]'
                        }
                        condition: '[if(and(not(empty(parameters(\'ipam\'))), equals(toLower(parameters(\'ipam\')[copyIndex()].name),toLower(variables(\'vNetName\')))),bool(\'true\'),bool(\'false\'))]'
                      }
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2020-06-01'
                        name: '[concat(\'es-ipam-\',subscription().displayName,\'-nsg-udr-vnet-hub-vwan-peering-\',copyIndex())]'
                        dependsOn: [
                          '[concat(\'es-ipam-\',subscription().displayName,\'-RG-\',copyIndex())]'
                        ]
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            variables: {}
                            resources: [
                              {
                                condition: '[contains(parameters(\'ipam\')[copyIndex()],\'networkSecurityGroups\')]'
                                apiVersion: '2020-05-01'
                                type: 'Microsoft.Network/networkSecurityGroups'
                                name: '[concat(subscription().displayName, \'-nsg\')]'
                                location: '[parameters(\'ipam\')[copyIndex()].location]'
                                properties: '[if(contains(parameters(\'ipam\')[copyIndex()],\'networkSecurityGroups\'),parameters(\'ipam\')[copyIndex()].networkSecurityGroups.properties,json(\'null\'))]'
                              }
                              {
                                condition: '[contains(parameters(\'ipam\')[copyIndex()],\'routeTables\')]'
                                apiVersion: '2020-05-01'
                                type: 'Microsoft.Network/routeTables'
                                name: '[concat(subscription().displayName, \'-udr\')]'
                                location: '[parameters(\'ipam\')[copyIndex()].location]'
                                properties: '[if(contains(parameters(\'ipam\')[copyIndex()],\'routeTables\'),parameters(\'ipam\')[copyIndex()].routeTables.properties,json(\'null\'))]'
                              }
                              {
                                condition: '[contains(parameters(\'ipam\')[copyIndex()],\'virtualNetworks\')]'
                                type: 'Microsoft.Network/virtualNetworks'
                                apiVersion: '2020-05-01'
                                name: '[concat(subscription().displayName, \'-vnet\')]'
                                location: '[parameters(\'ipam\')[copyIndex()].location]'
                                dependsOn: [
                                  '[concat(subscription().displayName, \'-nsg\')]'
                                  '[concat(subscription().displayName, \'-udr\')]'
                                ]
                                properties: '[if(contains(parameters(\'ipam\')[copyIndex()],\'virtualNetworks\'),parameters(\'ipam\')[copyIndex()].virtualNetworks.properties,json(\'null\'))]'
                              }
                              {
                                condition: '[contains(parameters(\'ipam\')[copyIndex()],\'virtualNetworkPeerings\')]'
                                type: 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings'
                                apiVersion: '2020-05-01'
                                name: '[concat(variables(\'vNetName\'), \'/peerToHub\')]'
                                dependsOn: [
                                  '[concat(subscription().displayName, \'-vnet\')]'
                                ]
                                properties: '[if(contains(parameters(\'ipam\')[copyIndex()],\'virtualNetworkPeerings\'),parameters(\'ipam\')[copyIndex()].virtualNetworkPeerings.properties,json(\'null\'))]'
                              }
                              {
                                condition: '[and(contains(parameters(\'ipam\')[copyIndex()],\'virtualNetworks\'),contains(parameters(\'ipam\')[copyIndex()],\'hubVirtualNetworkConnection\'),contains(parameters(\'ipam\')[copyIndex()].hubVirtualNetworkConnection,\'vWanVhubResourceId\'))]'
                                type: 'Microsoft.Resources/deployments'
                                apiVersion: '2020-06-01'
                                name: '[concat(\'es-ipam-vWan-\',subscription().displayName,\'-peering-\',copyIndex())]'
                                subscriptionId: '[if(and(contains(parameters(\'ipam\')[copyIndex()],\'hubVirtualNetworkConnection\'),contains(parameters(\'ipam\')[copyIndex()].hubVirtualNetworkConnection,\'vWanVhubResourceId\')),split(parameters(\'ipam\')[copyIndex()].hubVirtualNetworkConnection.vWanVhubResourceId,\'/\')[2],json(\'null\'))]'
                                resourceGroup: '[if(and(contains(parameters(\'ipam\')[copyIndex()],\'hubVirtualNetworkConnection\'),contains(parameters(\'ipam\')[copyIndex()].hubVirtualNetworkConnection,\'vWanVhubResourceId\')),split(parameters(\'ipam\')[copyIndex()].hubVirtualNetworkConnection.vWanVhubResourceId,\'/\')[4],json(\'null\'))]'
                                dependsOn: [
                                  '[concat(subscription().displayName, \'-vnet\')]'
                                ]
                                properties: {
                                  mode: 'Incremental'
                                  expressionEvaluationOptions: {
                                    scope: 'inner'
                                  }
                                  template: {
                                    '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                                    contentVersion: '1.0.0.0'
                                    parameters: {
                                      remoteVirtualNetwork: {
                                        type: 'string'
                                      }
                                      vWanVhubName: {
                                        Type: 'string'
                                        defaultValue: ''
                                      }
                                      allowHubToRemoteVnetTransit: {
                                        Type: 'bool'
                                        defaultValue: true
                                      }
                                      allowRemoteVnetToUseHubVnetGateways: {
                                        Type: 'bool'
                                        defaultValue: false
                                      }
                                      enableInternetSecurity: {
                                        Type: 'bool'
                                        defaultValue: true
                                      }
                                    }
                                    variables: {}
                                    resources: [
                                      {
                                        type: 'Microsoft.Network/virtualHubs/hubVirtualNetworkConnections'
                                        apiVersion: '2020-05-01'
                                        name: '[[concat(parameters(\'vWanVhubName\'),\'/\',last(split(parameters(\'remoteVirtualNetwork\'),\'/\')))]'
                                        properties: {
                                          remoteVirtualNetwork: {
                                            id: '[[parameters(\'remoteVirtualNetwork\')]'
                                          }
                                          allowHubToRemoteVnetTransit: '[[parameters(\'allowHubToRemoteVnetTransit\')]'
                                          allowRemoteVnetToUseHubVnetGateways: '[[parameters(\'allowRemoteVnetToUseHubVnetGateways\')]'
                                          enableInternetSecurity: '[[parameters(\'enableInternetSecurity\')]'
                                        }
                                      }
                                    ]
                                    outputs: {}
                                  }
                                  parameters: {
                                    remoteVirtualNetwork: {
                                      value: '[concat(subscription().id,\'/resourceGroups/\',variables(\'vNetRgName\'), \'/providers/\',\'Microsoft.Network/virtualNetworks/\', concat(subscription().displayName, \'-vnet\'))]'
                                    }
                                    vWanVhubName: {
                                      value: '[if(and(contains(parameters(\'ipam\')[copyIndex()],\'hubVirtualNetworkConnection\'),contains(parameters(\'ipam\')[copyIndex()].hubVirtualNetworkConnection,\'vWanVhubResourceId\')),split(parameters(\'ipam\')[copyIndex()].hubVirtualNetworkConnection.vWanVhubResourceId,\'/\')[8],json(\'null\'))]'
                                    }
                                    allowHubToRemoteVnetTransit: {
                                      value: '[if(and(contains(parameters(\'ipam\')[copyIndex()],\'hubVirtualNetworkConnection\'),contains(parameters(\'ipam\')[copyIndex()].hubVirtualNetworkConnection,\'vWanVhubResourceId\')),parameters(\'ipam\')[copyIndex()].hubVirtualNetworkConnection.properties.allowHubToRemoteVnetTransit,json(\'null\'))]'
                                    }
                                    allowRemoteVnetToUseHubVnetGateways: {
                                      value: '[if(and(contains(parameters(\'ipam\')[copyIndex()],\'hubVirtualNetworkConnection\'),contains(parameters(\'ipam\')[copyIndex()].hubVirtualNetworkConnection,\'vWanVhubResourceId\')),parameters(\'ipam\')[copyIndex()].hubVirtualNetworkConnection.properties.allowRemoteVnetToUseHubVnetGateways,json(\'null\'))]'
                                    }
                                    enableInternetSecurity: {
                                      value: '[if(and(contains(parameters(\'ipam\')[copyIndex()],\'hubVirtualNetworkConnection\'),contains(parameters(\'ipam\')[copyIndex()].hubVirtualNetworkConnection,\'vWanVhubResourceId\')),parameters(\'ipam\')[copyIndex()].hubVirtualNetworkConnection.properties.enableInternetSecurity,json(\'null\'))]'
                                    }
                                  }
                                }
                              }
                              {
                                condition: '[and(contains(parameters(\'ipam\')[copyIndex()],\'virtualNetworks\'),contains(parameters(\'ipam\')[copyIndex()],\'virtualNetworkPeerings\'),contains(parameters(\'ipam\')[copyIndex()].virtualNetworkPeerings.properties.remoteVirtualNetwork,\'id\'))]'
                                type: 'Microsoft.Resources/deployments'
                                apiVersion: '2020-06-01'
                                name: '[concat(\'es-ipam-hub-\',subscription().displayName,\'-peering-\',copyIndex())]'
                                subscriptionId: '[if(and(contains(parameters(\'ipam\')[copyIndex()],\'virtualNetworkPeerings\'),contains(parameters(\'ipam\')[copyIndex()].virtualNetworkPeerings.properties.remoteVirtualNetwork,\'id\')),split(parameters(\'ipam\')[copyIndex()].virtualNetworkPeerings.properties.remoteVirtualNetwork.id,\'/\')[2],json(\'null\'))]'
                                resourceGroup: '[if(and(contains(parameters(\'ipam\')[copyIndex()],\'virtualNetworkPeerings\'),contains(parameters(\'ipam\')[copyIndex()].virtualNetworkPeerings.properties.remoteVirtualNetwork,\'id\')),split(parameters(\'ipam\')[copyIndex()].virtualNetworkPeerings.properties.remoteVirtualNetwork.id,\'/\')[4],json(\'null\'))]'
                                dependsOn: [
                                  '[concat(subscription().displayName, \'-vnet\')]'
                                ]
                                properties: {
                                  mode: 'Incremental'
                                  expressionEvaluationOptions: {
                                    scope: 'inner'
                                  }
                                  template: {
                                    '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                                    contentVersion: '1.0.0.0'
                                    parameters: {
                                      remoteVirtualNetwork: {
                                        Type: 'string'
                                        defaultValue: false
                                      }
                                      hubName: {
                                        Type: 'string'
                                        defaultValue: false
                                      }
                                    }
                                    variables: {}
                                    resources: [
                                      {
                                        type: 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings'
                                        name: '[[concat(parameters(\'hubName\'),\'/\',last(split(parameters(\'remoteVirtualNetwork\'),\'/\')))]'
                                        apiVersion: '2020-05-01'
                                        properties: {
                                          allowVirtualNetworkAccess: true
                                          allowForwardedTraffic: true
                                          allowGatewayTransit: true
                                          useRemoteGateways: false
                                          remoteVirtualNetwork: {
                                            id: '[[parameters(\'remoteVirtualNetwork\')]'
                                          }
                                        }
                                      }
                                    ]
                                    outputs: {}
                                  }
                                  parameters: {
                                    remoteVirtualNetwork: {
                                      value: '[concat(subscription().id,\'/resourceGroups/\',variables(\'vNetRgName\'), \'/providers/\',\'Microsoft.Network/virtualNetworks/\', concat(subscription().displayName, \'-vnet\'))]'
                                    }
                                    hubName: {
                                      value: '[if(and(contains(parameters(\'ipam\')[copyIndex()],\'virtualNetworkPeerings\'),contains(parameters(\'ipam\')[copyIndex()].virtualNetworkPeerings.properties.remoteVirtualNetwork,\'id\')),split(parameters(\'ipam\')[copyIndex()].virtualNetworkPeerings.properties.remoteVirtualNetwork.id,\'/\')[8],json(\'null\'))]'
                                    }
                                  }
                                }
                              }
                            ]
                            outputs: {}
                          }
                        }
                        resourceGroup: '[variables(\'vNetRgName\')]'
                        copy: {
                          name: 'ipam-loop'
                          count: '[length(parameters(\'ipam\'))]'
                        }
                        condition: '[if(and(not(empty(parameters(\'ipam\'))), equals(toLower(parameters(\'ipam\')[copyIndex()].name),toLower(variables(\'vNetName\')))),bool(\'true\'),bool(\'false\'))]'
                      }
                    ]
                    outputs: {
                      ipam: {
                        condition: '[bool(\'true\')]'
                        type: 'Int'
                        value: '[length(parameters(\'ipam\'))]'
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-vNet'
    }
    {
      properties: {
        Description: 'Deploy the Virtual WAN in the specific region.'
        DisplayName: 'Deploy the Virtual WAN in the specific region'
        Mode: 'Indexed'
        Parameters: {
          vwanname: {
            type: 'String'
            metadata: {
              displayName: 'vwanname'
              description: 'Name of the Virtual WAN'
            }
          }
          vwanRegion: {
            type: 'String'
            metadata: {
              displayName: 'vwanRegion'
              description: 'Select Azure region for Virtual WAN'
              strongType: 'location'
            }
          }
          rgName: {
            type: 'String'
            metadata: {
              displayName: 'rgName'
              description: 'Provide name for resource group.'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Resources/subscriptions'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Network/virtualWans'
              deploymentScope: 'Subscription'
              existenceScope: 'ResourceGroup'
              name: '[parameters(\'vwanname\')]'
              resourceGroupName: '[parameters(\'rgName\')]'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
              ]
              deployment: {
                location: 'northeurope'
                properties: {
                  mode: 'incremental'
                  parameters: {
                    rgName: {
                      value: '[parameters(\'rgName\')]'
                    }
                    vwanname: {
                      value: '[parameters(\'vwanname\')]'
                    }
                    vwanRegion: {
                      value: '[parameters(\'vwanRegion\')]'
                    }
                  }
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      rgName: {
                        type: 'string'
                      }
                      vwanname: {
                        type: 'string'
                      }
                      vwanRegion: {
                        type: 'string'
                      }
                    }
                    variables: {
                      vwansku: 'Standard'
                    }
                    resources: [
                      {
                        type: 'Microsoft.Resources/resourceGroups'
                        apiVersion: '2018-05-01'
                        name: '[parameters(\'rgName\')]'
                        location: '[deployment().location]'
                        properties: {}
                      }
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2018-05-01'
                        name: 'vwan'
                        resourceGroup: '[parameters(\'rgName\')]'
                        dependsOn: [
                          '[resourceId(\'Microsoft.Resources/resourceGroups/\', parameters(\'rgName\'))]'
                        ]
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            resources: [
                              {
                                type: 'Microsoft.Network/virtualWans'
                                apiVersion: '2020-05-01'
                                location: '[parameters(\'vwanRegion\')]'
                                name: '[parameters(\'vwanname\')]'
                                properties: {
                                  virtualHubs: []
                                  vpnSites: []
                                  type: '[variables(\'vwansku\')]'
                                }
                              }
                            ]
                            outputs: {}
                          }
                        }
                      }
                    ]
                    outputs: {}
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-vWAN'
    }
    {
      properties: {
        Description: 'Deploy Virtual Hub network with Virtual Wan and Gateway and Firewall configured in the desired region. '
        DisplayName: 'Deploy Virtual Hub network with Virtual Wan and Gateway and Firewall configured.'
        Mode: 'Indexed'
        Parameters: {
          vwanname: {
            type: 'String'
            metadata: {
              displayName: 'vwanname'
              description: 'Name of the Virtual WAN'
            }
          }
          vHubName: {
            type: 'String'
            metadata: {
              displayName: 'vHubName'
              description: 'Name of the vHUB'
            }
            defaultValue: ''
          }
          vHUB: {
            type: 'Object'
            metadata: {
              displayName: 'vHUB'
              description: 'Object describing Virtual WAN vHUB'
            }
          }
          vpngw: {
            type: 'Object'
            metadata: {
              displayName: 'vpngw'
              description: 'Object describing VPN gateway'
            }
            defaultValue: {}
          }
          ergw: {
            type: 'Object'
            metadata: {
              displayName: 'ergw'
              description: 'Object describing ExpressRoute gateway'
            }
            defaultValue: {}
          }
          azfw: {
            type: 'Object'
            metadata: {
              displayName: 'azfw'
              description: 'Object describing the Azure Firewall in vHUB'
            }
            defaultValue: {}
          }
          rgName: {
            type: 'String'
            metadata: {
              displayName: 'rgName'
              description: 'Provide name for resource group.'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Resources/subscriptions'
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Network/virtualHubs'
              name: '[parameters(\'vHubName\')]'
              deploymentScope: 'Subscription'
              existenceScope: 'ResourceGroup'
              ResourceGroupName: '[parameters(\'rgName\')]'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
              ]
              deployment: {
                location: 'northeurope'
                properties: {
                  mode: 'incremental'
                  parameters: {
                    rgName: {
                      value: '[parameters(\'rgName\')]'
                    }
                    vwanname: {
                      value: '[parameters(\'vwanname\')]'
                    }
                    vHUB: {
                      value: '[parameters(\'vHUB\')]'
                    }
                    vpngw: {
                      value: '[parameters(\'vpngw\')]'
                    }
                    ergw: {
                      value: '[parameters(\'ergw\')]'
                    }
                    azfw: {
                      value: '[parameters(\'azfw\')]'
                    }
                    vHUBName: {
                      value: '[parameters(\'vHUBName\')]'
                    }
                  }
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      vwanname: {
                        type: 'string'
                        metadata: {
                          description: 'Name of the Virtual WAN'
                        }
                      }
                      vHUB: {
                        type: 'object'
                        metadata: {
                          description: 'Object describing Virtual WAN vHUB'
                        }
                      }
                      vpngw: {
                        type: 'object'
                        defaultValue: {}
                        metadata: {
                          description: 'Object describing VPN gateway'
                        }
                      }
                      ergw: {
                        type: 'object'
                        defaultValue: {}
                        metadata: {
                          description: 'Object describing ExpressRoute gateway'
                        }
                      }
                      azfw: {
                        type: 'object'
                        defaultValue: {}
                        metadata: {
                          description: 'Object describing the Azure Firewall in vHUB'
                        }
                      }
                      rgName: {
                        type: 'String'
                        metadata: {
                          displayName: 'rgName'
                          description: 'Provide name for resource group.'
                        }
                      }
                      vHUBName: {
                        type: 'String'
                        metadata: {
                          displayName: 'vHUBName'
                          description: 'Name of the vHUB'
                        }
                      }
                    }
                    variables: {
                      vhubsku: 'Standard'
                      vwanresourceid: '[concat(subscription().id,\'/resourceGroups/\',parameters(\'rgName\'),\'/providers/Microsoft.Network/virtualWans/\',parameters(\'vwanname\'))]'
                      vwanhub: '[concat(subscription().id,\'/resourceGroups/\',parameters(\'rgName\'),\'/providers/Microsoft.Network/virtualHubs/\',parameters(\'vHUBName\'))]'
                    }
                    resources: [
                      {
                        type: 'Microsoft.Resources/resourceGroups'
                        apiVersion: '2018-05-01'
                        name: '[parameters(\'rgName\')]'
                        location: '[deployment().location]'
                        properties: {}
                      }
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2018-05-01'
                        name: '[concat(\'vHUBdeploy-\',parameters(\'vHUB\').location)]'
                        resourceGroup: '[parameters(\'rgName\')]'
                        dependsOn: [
                          '[resourceId(\'Microsoft.Resources/resourceGroups/\', parameters(\'rgName\'))]'
                        ]
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            variables: {}
                            resources: [
                              {
                                type: 'Microsoft.Network/virtualHubs'
                                apiVersion: '2020-05-01'
                                location: '[parameters(\'vHUB\').location]'
                                name: '[parameters(\'vHUBname\')]'
                                properties: {
                                  virtualWan: {
                                    id: '[variables(\'vwanresourceid\')]'
                                  }
                                  addressPrefix: '[parameters(\'vHUB\').addressPrefix]'
                                  sku: '[variables(\'vhubsku\')]'
                                }
                              }
                            ]
                          }
                        }
                      }
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2018-05-01'
                        condition: '[greater(length(parameters(\'vpngw\')),0)]'
                        resourceGroup: '[parameters(\'rgName\')]'
                        dependsOn: [
                          '[concat(\'vHUBdeploy-\',parameters(\'vHUB\').location)]'
                        ]
                        name: '[concat(parameters(\'vHUBName\'),\'-vpngw\')]'
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            variables: {}
                            resources: [
                              {
                                type: 'Microsoft.Network/vpnGateways'
                                apiVersion: '2020-05-01'
                                location: '[parameters(\'vHUB\').location]'
                                name: '[parameters(\'vpngw\').name]'
                                properties: {
                                  virtualHub: {
                                    id: '[variables(\'vwanhub\')]'
                                  }
                                  bgpSettings: '[parameters(\'vpngw\').bgpSettings]'
                                  vpnGatewayScaleUnit: '[parameters(\'vpngw\').vpnGatewayScaleUnit]'
                                }
                              }
                            ]
                          }
                        }
                      }
                      {
                        type: 'Microsoft.Resources/deployments'
                        apiVersion: '2018-05-01'
                        condition: '[greater(length(parameters(\'ergw\')),0)]'
                        resourceGroup: '[parameters(\'rgName\')]'
                        dependsOn: [
                          '[concat(\'vHUBdeploy-\',parameters(\'vHUB\').location)]'
                        ]
                        name: '[concat(parameters(\'vHUBName\'),\'-ergw\')]'
                        properties: {
                          mode: 'Incremental'
                          template: {
                            '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                            contentVersion: '1.0.0.0'
                            parameters: {}
                            variables: {}
                            resources: [
                              {
                                type: 'Microsoft.Network/expressRouteGateways'
                                apiVersion: '2020-05-01'
                                location: '[parameters(\'vHUB\').location]'
                                name: '[parameters(\'ergw\').name]'
                                properties: {
                                  virtualHub: {
                                    id: '[variables(\'vwanhub\')]'
                                  }
                                  autoScaleConfiguration: '[parameters(\'ergw\').autoScaleConfiguration]'
                                }
                              }
                            ]
                          }
                        }
                      }
                    ]
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-vHUB'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for Virtual Machine Scale Sets  to stream to a Log Analytics workspace when any Virtual Machine Scale Sets  which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
        DisplayName: 'Deploy Diagnostic Settings for Virtual Machine Scale Sets to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Compute/virtualMachineScaleSets'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Compute/virtualMachineScaleSets/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                enabled: false
                                days: 0
                              }
                            }
                          ]
                          logs: []
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-VMSS'
    }
    {
      properties: {
        Description: 'Deploys the diagnostic settings for VPN Gateway to stream to a Log Analytics workspace when any VPN Gateway which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled.'
        DisplayName: 'Deploy Diagnostic Settings for VPN Gateway to Log Analytics workspace'
        Mode: 'Indexed'
        Parameters: {
          logAnalytics: {
            type: 'String'
            metadata: {
              displayName: 'Log Analytics workspace'
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              strongType: 'omsWorkspace'
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          metricsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable metrics'
              description: 'Whether to enable metrics stream to the Log Analytics workspace - True or False'
            }
          }
          logsEnabled: {
            type: 'string'
            defaultValue: 'True'
            allowedValues: [
              'True'
              'False'
            ]
            metadata: {
              displayName: 'Enable logs'
              description: 'Whether to enable logs stream to the Log Analytics workspace - True or False'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyRule: {
          if: {
            field: 'type'
            equals: 'Microsoft.Network/virtualNetworkGateways'
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Insights/diagnosticSettings'
              name: 'setByPolicy'
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/logs.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/metrics.enabled'
                    equals: 'true'
                  }
                  {
                    field: 'Microsoft.Insights/diagnosticSettings/workspaceId'
                    equals: '[parameters(\'logAnalytics\')]'
                  }
                ]
              }
              roleDefinitionIds: [
                '/providers/microsoft.authorization/roleDefinitions/749f88d5-cbae-40b8-bcfc-e573ddc772fa'
                '/providers/microsoft.authorization/roleDefinitions/92aaf0da-9dab-42b6-94a3-d43ce8d16293'
              ]
              deployment: {
                properties: {
                  mode: 'incremental'
                  template: {
                    '$schema': 'http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      resourceName: {
                        type: 'string'
                      }
                      logAnalytics: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      profileName: {
                        type: 'string'
                      }
                      metricsEnabled: {
                        type: 'string'
                      }
                      logsEnabled: {
                        type: 'string'
                      }
                    }
                    variables: {}
                    resources: [
                      {
                        type: 'Microsoft.Network/virtualNetworkGateways/providers/diagnosticSettings'
                        apiVersion: '2017-05-01-preview'
                        name: '[concat(parameters(\'resourceName\'), \'/\', \'Microsoft.Insights/\', parameters(\'profileName\'))]'
                        location: '[parameters(\'location\')]'
                        dependsOn: []
                        properties: {
                          workspaceId: '[parameters(\'logAnalytics\')]'
                          metrics: [
                            {
                              category: 'AllMetrics'
                              enabled: '[parameters(\'metricsEnabled\')]'
                              retentionPolicy: {
                                days: 0
                                enabled: false
                              }
                              timeGrain: null
                            }
                          ]
                          logs: [
                            {
                              category: 'GatewayDiagnosticLog'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'IKEDiagnosticLog'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'P2SDiagnosticLog'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'RouteDiagnosticLog'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'RouteDiagnosticLog'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                            {
                              category: 'TunnelDiagnosticLog'
                              enabled: '[parameters(\'logsEnabled\')]'
                            }
                          ]
                        }
                      }
                    ]
                    outputs: {}
                  }
                  parameters: {
                    logAnalytics: {
                      value: '[parameters(\'logAnalytics\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    resourceName: {
                      value: '[field(\'name\')]'
                    }
                    profileName: {
                      value: '[parameters(\'profileName\')]'
                    }
                    metricsEnabled: {
                      value: '[parameters(\'metricsEnabled\')]'
                    }
                    logsEnabled: {
                      value: '[parameters(\'logsEnabled\')]'
                    }
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Diagnostics-VNetGW'
    }
    {
      properties: {
        Description: 'Deploy Windows Domain Join Extension with keyvault configuration when the extension does not exist on a given windows Virtual Machine'
        DisplayName: 'Deploy Windows Domain Join Extension with keyvault configuration'
        Mode: 'Indexed'
        Parameters: {
          domainUsername: {
            type: 'String'
            metadata: {
              displayName: 'domainUsername'
              description: null
            }
          }
          domainPassword: {
            type: 'String'
            metadata: {
              displayName: 'domainPassword'
              description: null
            }
          }
          domainFQDN: {
            type: 'String'
            metadata: {
              displayName: 'domainFQDN'
              description: null
            }
          }
          domainOUPath: {
            type: 'String'
            metadata: {
              displayName: 'domainOUPath'
              description: null
            }
          }
          keyVaultResourceId: {
            type: 'String'
            metadata: {
              displayName: 'keyVaultResourceId'
              description: null
            }
          }
          effect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Effect'
              description: 'Enable or disable the execution of the policy'
            }
          }
        }
        metadata: {
          version: '1.0.0'
          category: 'Guest Configuration'
        }
        PolicyRule: {
          if: {
            allOf: [
              {
                field: 'type'
                equals: 'Microsoft.Compute/virtualMachines'
              }
              {
                field: 'Microsoft.Compute/imagePublisher'
                equals: 'MicrosoftWindowsServer'
              }
              {
                field: 'Microsoft.Compute/imageOffer'
                equals: 'WindowsServer'
              }
              {
                field: 'Microsoft.Compute/imageSKU'
                in: [
                  '2008-R2-SP1'
                  '2008-R2-SP1-smalldisk'
                  '2008-R2-SP1-zhcn'
                  '2012-Datacenter'
                  '2012-datacenter-gensecond'
                  '2012-Datacenter-smalldisk'
                  '2012-datacenter-smalldisk-g2'
                  '2012-Datacenter-zhcn'
                  '2012-datacenter-zhcn-g2'
                  '2012-R2-Datacenter'
                  '2012-r2-datacenter-gensecond'
                  '2012-R2-Datacenter-smalldisk'
                  '2012-r2-datacenter-smalldisk-g2'
                  '2012-R2-Datacenter-zhcn'
                  '2012-r2-datacenter-zhcn-g2'
                  '2016-Datacenter'
                  '2016-datacenter-gensecond'
                  '2016-datacenter-gs'
                  '2016-Datacenter-Server-Core'
                  '2016-datacenter-server-core-g2'
                  '2016-Datacenter-Server-Core-smalldisk'
                  '2016-datacenter-server-core-smalldisk-g2'
                  '2016-Datacenter-smalldisk'
                  '2016-datacenter-smalldisk-g2'
                  '2016-Datacenter-with-Containers'
                  '2016-datacenter-with-containers-g2'
                  '2016-Datacenter-with-RDSH'
                  '2016-Datacenter-zhcn'
                  '2016-datacenter-zhcn-g2'
                  '2019-Datacenter'
                  '2019-Datacenter-Core'
                  '2019-datacenter-core-g2'
                  '2019-Datacenter-Core-smalldisk'
                  '2019-datacenter-core-smalldisk-g2'
                  '2019-Datacenter-Core-with-Containers'
                  '2019-datacenter-core-with-containers-g2'
                  '2019-Datacenter-Core-with-Containers-smalldisk'
                  '2019-datacenter-core-with-containers-smalldisk-g2'
                  '2019-datacenter-gensecond'
                  '2019-datacenter-gs'
                  '2019-Datacenter-smalldisk'
                  '2019-datacenter-smalldisk-g2'
                  '2019-Datacenter-with-Containers'
                  '2019-datacenter-with-containers-g2'
                  '2019-Datacenter-with-Containers-smalldisk'
                  '2019-datacenter-with-containers-smalldisk-g2'
                  '2019-Datacenter-zhcn'
                  '2019-datacenter-zhcn-g2'
                  'Datacenter-Core-1803-with-Containers-smalldisk'
                  'datacenter-core-1803-with-containers-smalldisk-g2'
                  'Datacenter-Core-1809-with-Containers-smalldisk'
                  'datacenter-core-1809-with-containers-smalldisk-g2'
                  'Datacenter-Core-1903-with-Containers-smalldisk'
                  'datacenter-core-1903-with-containers-smalldisk-g2'
                  'datacenter-core-1909-with-containers-smalldisk'
                  'datacenter-core-1909-with-containers-smalldisk-g1'
                  'datacenter-core-1909-with-containers-smalldisk-g2'
                ]
              }
            ]
          }
          then: {
            effect: '[parameters(\'effect\')]'
            details: {
              type: 'Microsoft.Compute/virtualMachines/extensions'
              roleDefinitionIds: [
                '/providers/Microsoft.Authorization/roleDefinitions/9980e02c-c2be-4d73-94e8-173b1dc7cf3c'
              ]
              existenceCondition: {
                allOf: [
                  {
                    field: 'Microsoft.Compute/virtualMachines/extensions/type'
                    equals: 'JsonADDomainExtension'
                  }
                  {
                    field: 'Microsoft.Compute/virtualMachines/extensions/publisher'
                    equals: 'Microsoft.Compute'
                  }
                ]
              }
              deployment: {
                properties: {
                  mode: 'incremental'
                  parameters: {
                    vmName: {
                      value: '[field(\'name\')]'
                    }
                    location: {
                      value: '[field(\'location\')]'
                    }
                    domainUsername: {
                      reference: {
                        keyVault: {
                          id: '[parameters(\'keyVaultResourceId\')]'
                        }
                        secretName: '[parameters(\'domainUsername\')]'
                      }
                    }
                    domainPassword: {
                      reference: {
                        keyVault: {
                          id: '[parameters(\'keyVaultResourceId\')]'
                        }
                        secretName: '[parameters(\'domainPassword\')]'
                      }
                    }
                    domainOUPath: {
                      value: '[parameters(\'domainOUPath\')]'
                    }
                    domainFQDN: {
                      value: '[parameters(\'domainFQDN\')]'
                    }
                    keyVaultResourceId: {
                      value: '[parameters(\'keyVaultResourceId\')]'
                    }
                  }
                  template: {
                    '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    parameters: {
                      vmName: {
                        type: 'string'
                      }
                      location: {
                        type: 'string'
                      }
                      domainUsername: {
                        type: 'string'
                      }
                      domainPassword: {
                        type: 'securestring'
                      }
                      domainFQDN: {
                        type: 'string'
                      }
                      domainOUPath: {
                        type: 'string'
                      }
                      keyVaultResourceId: {
                        type: 'string'
                      }
                    }
                    variables: {
                      domainJoinOptions: 3
                      vmName: '[parameters(\'vmName\')]'
                    }
                    resources: [
                      {
                        apiVersion: '2015-06-15'
                        type: 'Microsoft.Compute/virtualMachines/extensions'
                        name: '[concat(variables(\'vmName\'),\'/joindomain\')]'
                        location: '[resourceGroup().location]'
                        properties: {
                          publisher: 'Microsoft.Compute'
                          type: 'JsonADDomainExtension'
                          typeHandlerVersion: '1.3'
                          autoUpgradeMinorVersion: true
                          settings: {
                            Name: '[parameters(\'domainFQDN\')]'
                            User: '[parameters(\'domainUserName\')]'
                            Restart: 'true'
                            Options: '[variables(\'domainJoinOptions\')]'
                            OUPath: '[parameters(\'domainOUPath\')]'
                          }
                          protectedSettings: {
                            Password: '[parameters(\'domainPassword\')]'
                          }
                        }
                      }
                    ]
                    outputs: {}
                  }
                }
              }
            }
          }
        }
      }
      name: 'Deploy-Windows-DomainJoin'
    }
  ]
}
var initiatives = {
  policySetDefinitions: [
    {
      properties: {
        Description: 'This policy set deploys the configurations of application Azure resources to forward diagnostic logs and metrics to an Azure Log Analytics workspace. See the list of policies of the services that are included '
        DisplayName: 'Deploy Diagnostic Settings to Azure Services'
        Parameters: {
          logAnalytics: {
            metadata: {
              description: 'Select Log Analytics workspace from dropdown list. If this workspace is outside of the scope of the assignment you must manually grant \'Log Analytics Contributor\' permissions (or similar) to the policy assignment\'s principal ID.'
              displayName: 'Log Analytics workspace'
              strongType: 'omsWorkspace'
            }
            type: 'String'
          }
          profileName: {
            type: 'string'
            defaultValue: 'setbypolicy'
            metadata: {
              displayName: 'Profile name'
              description: 'The diagnostic settings profile name'
            }
          }
          ACILogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Container Instances to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Container Instances to stream to a Log Analytics workspace when any ACR which is missing this diagnostic settings is created or updated. The policy wil set the diagnostic with all metrics enabled.'
            }
          }
          ACRLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Container Registry to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Container Registry to stream to a Log Analytics workspace when any ACR which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics  enabled.'
            }
          }
          AKSLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Kubernetes Service to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Kubernetes Service to stream to a Log Analytics workspace when any Kubernetes Service which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled.'
            }
          }
          AnalysisServiceLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Analysis Services to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Analysis Services to stream to a Log Analytics workspace when any Analysis Services which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          APIMgmtLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for API Management to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for API Management to stream to a Log Analytics workspace when any API Management which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          ApplicationGatewayLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Application Gateway to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Application Gateway to stream to a Log Analytics workspace when any Application Gateway which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          AutomationLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Automation to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Automation to stream to a Log Analytics workspace when any Automation which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          BatchLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Batch to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Batch to stream to a Log Analytics workspace when any Batch which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          CDNEndpointsLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for CDN Endpoint to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for CDN Endpoint to stream to a Log Analytics workspace when any CDN Endpoint which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          CognitiveServicesLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Cognitive Services to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Cognitive Services to stream to a Log Analytics workspace when any Cognitive Services which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          CosmosLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Cosmos DB to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Cosmos DB to stream to a Log Analytics workspace when any Cosmos DB which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          DatabricksLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Databricks to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Databricks to stream to a Log Analytics workspace when any Databricks which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          DataFactoryLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Data Factory to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Data Factory to stream to a Log Analytics workspace when any Data Factory which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          DataLakeStoreLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Azure Data Lake Store to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Azure Data Lake Store to stream to a Log Analytics workspace when anyAzure Data Lake Store which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          DataLakeAnalyticsLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Data Lake Analytics to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Data Lake Analytics to stream to a Log Analytics workspace when any Data Lake Analytics which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          EventGridSubLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Event Grid subscriptions to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Event Grid subscriptions to stream to a Log Analytics workspace when any Event Grid subscriptions which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          EventGridTopicLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Event Grid Topic to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Event Grid Topic to stream to a Log Analytics workspace when any Event Grid Topic which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          EventHubLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Event Hubs to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Event Hubs to stream to a Log Analytics workspace when any Event Hubs which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          EventSystemTopicLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Event Grid System Topic to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Event Grid System Topic to stream to a Log Analytics workspace when any Event Grid System Topic which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          ExpressRouteLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for ExpressRoute to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for ExpressRoute to stream to a Log Analytics workspace when any ExpressRoute which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          FirewallLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Firewall to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Firewall to stream to a Log Analytics workspace when any Firewall which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          FrontDoorLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Front Door to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Front Door to stream to a Log Analytics workspace when any Front Door which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          FunctionAppLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Azure Function App to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Azure Function App to stream to a Log Analytics workspace when any function app which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          HDInsightLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for HDInsight to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for HDInsight to stream to a Log Analytics workspace when any HDInsight which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          IotHubLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for IoT Hub to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for IoT Hub to stream to a Log Analytics workspace when any IoT Hub which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          KeyVaultLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Key Vault to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Key Vault to stream to a Log Analytics workspace when any Key Vault which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          LoadBalancerLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Load Balancer to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Load Balancer to stream to a Log Analytics workspace when any Load Balancer which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          LogicAppsISELogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Logic Apps integration service environment to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Logic Apps integration service environment to stream to a Log Analytics workspace when any Logic Apps integration service environment which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          LogicAppsWFLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Logic Apps Workflow runtime to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Logic Apps Workflow runtimeto stream to a Log Analytics workspace when any Logic Apps Workflow runtime which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          MariaDBLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for MariaDB to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for MariaDB to stream to a Log Analytics workspace when any MariaDB  which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          MlWorkspaceLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Machine Learning workspace to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Machine Learning workspace to stream to a Log Analytics workspace when any Machine Learning workspace which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          MySQLLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Database for MySQL to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Database for MySQL to stream to a Log Analytics workspace when any Database for MySQL which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          NetworkSecurityGroupsLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Network Security Groups to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Network Security Groups to stream to a Log Analytics workspace when any Network Security Groups which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          NetworkNICLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Network Interfaces to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Network Interfaces to stream to a Log Analytics workspace when any Network Interfaces which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          PostgreSQLLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Database for PostgreSQL to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Database for PostgreSQL to stream to a Log Analytics workspace when any Database for PostgreSQL which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          PowerBIEmbeddedLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Power BI Embedded to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Power BI Embedded to stream to a Log Analytics workspace when any Power BI Embedded which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          NetworkPublicIPNicLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Public IP addresses to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Public IP addresses to stream to a Log Analytics workspace when any Public IP addresses which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          RecoveryVaultLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Recovery Services vaults to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Recovery Services vaults to stream to a Log Analytics workspace when any Recovery Services vaults which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          RedisCacheLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Redis Cache to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Redis Cache to stream to a Log Analytics workspace when any Redis Cache which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          RelayLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Relay to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Relay to stream to a Log Analytics workspace when any Relay which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          SearchServicesLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Search Services to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Search Services to stream to a Log Analytics workspace when any Search Services which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          ServiceBusLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Service Bus namespaces  to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for ServiceBus to stream to a Log Analytics workspace when any ServiceBus which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          SignalRLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for SignalR to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for SignalR to stream to a Log Analytics workspace when any SignalR which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          SQLDBsLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for SQL Databases  to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for SQL Databases to stream to a Log Analytics workspace when any SQL Databases  which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          SQLElasticPoolsLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for SQL Elastic Pools to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for SQL Elastic Pools to stream to a Log Analytics workspace when any SQL Elastic Pools which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          SQLMLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for SQL Managed Instances to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for SQL Managed Instances to stream to a Log Analytics workspace when any SQL Managed Instances which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          StreamAnalyticsLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Stream Analytics to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Stream Analytics to stream to a Log Analytics workspace when any Stream Analytics which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          TimeSeriesInsightsLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Time Series Insights to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Time Series Insights to stream to a Log Analytics workspace when any Time Series Insights which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          TrafficManagerLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Traffic Manager to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Traffic Manager to stream to a Log Analytics workspace when any Traffic Manager which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          VirtualNetworkLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Virtual Network to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Virtual Network to stream to a Log Analytics workspace when any Virtual Network which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          VirtualMachinesLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Virtual Machines to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Virtual Machines to stream to a Log Analytics workspace when any Virtual Machines which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          VMSSLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for Virtual Machine Scale Sets to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Virtual Machine Scale Sets  to stream to a Log Analytics workspace when any Virtual Machine Scale Sets  which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          VNetGWLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for VPN Gateway to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for VPN Gateway to stream to a Log Analytics workspace when any VPN Gateway which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled.'
            }
          }
          AppServiceLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for App Service Plan to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for App Service Plan to stream to a Log Analytics workspace when any App Service Plan which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
          AppServiceWebappLogAnalyticsEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy Diagnostic Settings for App Service to Log Analytics workspace'
              description: 'Deploys the diagnostic settings for Web App to stream to a Log Analytics workspace when any Web App which is missing this diagnostic settings is created or updated. The policy wil  set the diagnostic with all metrics and category enabled'
            }
          }
        }
        PolicyDefinitionGroups: null
        metadata: {
          version: '1.0.0'
          category: 'Monitoring'
        }
        PolicyDefinitions: [
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-ACI'
            policyDefinitionReferenceId: 'ACIDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'ACILogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-ACR'
            policyDefinitionReferenceId: 'ACRDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'ACRLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-AKS'
            policyDefinitionReferenceId: 'AKSDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'AKSLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-AnalysisService'
            policyDefinitionReferenceId: 'AnalysisServiceDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'AnalysisServiceLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-APIMgmt'
            policyDefinitionReferenceId: 'APIMgmtDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'APIMgmtLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-ApplicationGateway'
            policyDefinitionReferenceId: 'ApplicationGatewayDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'ApplicationGatewayLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-AA'
            policyDefinitionReferenceId: 'AutomationDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'AutomationLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-Batch'
            policyDefinitionReferenceId: 'BatchDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'BatchLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-CDNEndpoints'
            policyDefinitionReferenceId: 'CDNEndpointsDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'CDNEndpointsLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-CognitiveServices'
            policyDefinitionReferenceId: 'CognitiveServicesDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'CognitiveServicesLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-CosmosDB'
            policyDefinitionReferenceId: 'CosmosDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'CosmosLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-Databricks'
            policyDefinitionReferenceId: 'DatabricksDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'DatabricksLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-DataFactory'
            policyDefinitionReferenceId: 'DataFactoryDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'DataFactoryLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-DataLakeStore'
            policyDefinitionReferenceId: 'DataLakeStoreDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'DataLakeStoreLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-DLAnalytics'
            policyDefinitionReferenceId: 'DataLakeAnalyticsDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'DataLakeAnalyticsLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-EventGridSub'
            policyDefinitionReferenceId: 'EventGridSubDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'EventGridSubLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-EventGridTopic'
            policyDefinitionReferenceId: 'EventGridTopicDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'EventGridTopicLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-EventHub'
            policyDefinitionReferenceId: 'EventHubDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'EventHubLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-EventGridSystemTopic'
            policyDefinitionReferenceId: 'EventSystemTopicDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'EventSystemTopicLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-ExpressRoute'
            policyDefinitionReferenceId: 'ExpressRouteDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'ExpressRouteLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-Firewall'
            policyDefinitionReferenceId: 'FirewallDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'FirewallLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-FrontDoor'
            policyDefinitionReferenceId: 'FrontDoorDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'FrontDoorLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-Function'
            policyDefinitionReferenceId: 'FunctionAppDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'FunctionAppLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-HDInsight'
            policyDefinitionReferenceId: 'HDInsightDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'HDInsightLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-iotHub'
            policyDefinitionReferenceId: 'IotHubDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'IotHubLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-KeyVault'
            policyDefinitionReferenceId: 'KeyVaultDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'KeyVaultLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-LoadBalancer'
            policyDefinitionReferenceId: 'LoadBalancerDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'LoadBalancerLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-LogicAppsISE'
            policyDefinitionReferenceId: 'LogicAppsISEDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'LogicAppsISELogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-LogicAppsWF'
            policyDefinitionReferenceId: 'LogicAppsWFDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'LogicAppsWFLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-MariaDB'
            policyDefinitionReferenceId: 'MariaDBDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'MariaDBLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-MlWorkspace'
            policyDefinitionReferenceId: 'MlWorkspaceDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'MlWorkspaceLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-MySQL'
            policyDefinitionReferenceId: 'MySQLDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'MySQLLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-NetworkSecurityGroups'
            policyDefinitionReferenceId: 'NetworkSecurityGroupsDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'NetworkSecurityGroupsLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-NIC'
            policyDefinitionReferenceId: 'NetworkNICDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'NetworkNICLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-PostgreSQL'
            policyDefinitionReferenceId: 'PostgreSQLDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'PostgreSQLLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-PowerBIEmbedded'
            policyDefinitionReferenceId: 'PowerBIEmbeddedDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'PowerBIEmbeddedLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-PublicIP'
            policyDefinitionReferenceId: 'NetworkPublicIPNicDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'NetworkPublicIPNicLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-RecoveryVault'
            policyDefinitionReferenceId: 'RecoveryVaultDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'RecoveryVaultLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-RedisCache'
            policyDefinitionReferenceId: 'RedisCacheDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'RedisCacheLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-Relay'
            policyDefinitionReferenceId: 'RelayDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'RelayLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-SearchServices'
            policyDefinitionReferenceId: 'SearchServicesDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'SearchServicesLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-ServiceBus'
            policyDefinitionReferenceId: 'ServiceBusDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'ServiceBusLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-SignalR'
            policyDefinitionReferenceId: 'SignalRDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'SignalRLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-SQLDBs'
            policyDefinitionReferenceId: 'SQLDBsDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'SQLDBsLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-SQLElasticPools'
            policyDefinitionReferenceId: 'SQLElasticPoolsDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'SQLElasticPoolsLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-SQLMI'
            policyDefinitionReferenceId: 'SQLMDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'SQLMLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-StreamAnalytics'
            policyDefinitionReferenceId: 'StreamAnalyticsDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'StreamAnalyticsLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-TimeSeriesInsights'
            policyDefinitionReferenceId: 'TimeSeriesInsightsDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'TimeSeriesInsightsLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-TrafficManager'
            policyDefinitionReferenceId: 'TrafficManagerDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'TrafficManagerLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-VirtualNetwork'
            policyDefinitionReferenceId: 'VirtualNetworkDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'VirtualNetworkLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-VM'
            policyDefinitionReferenceId: 'VirtualMachinesDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'VirtualMachinesLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-VMSS'
            policyDefinitionReferenceId: 'VMSSDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'VMSSLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-VNetGW'
            policyDefinitionReferenceId: 'VNetGWDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'VNetGWLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-WebServerFarm'
            policyDefinitionReferenceId: 'AppServiceDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'AppServiceLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Diagnostics-Website'
            policyDefinitionReferenceId: 'AppServiceWebappDeployDiagnosticLogDeployLogAnalytics'
            parameters: {
              logAnalytics: {
                value: '[parameters(\'logAnalytics\')]'
              }
              effect: {
                value: '[parameters(\'AppServiceWebappLogAnalyticsEffect\')]'
              }
              profileName: {
                value: '[parameters(\'profileName\')]'
              }
            }
          }
        ]
      }
      type: null
      name: 'Deploy-Diag-LogAnalytics'
    }
    {
      properties: {
        Description: 'This policy denies creation of Azure PAAS services with exposed public endpoints.  This policy set includes the policy for the following services KeyVault, Storage accounts, AKS, Cosmos, SQL Servers, MariaDB, MySQL and Postgress. '
        DisplayName: 'Public network access should be disabled for PAAS services'
        Parameters: {
          CosmosPublicIpDenyEffect: {
            type: 'string'
            defaultValue: 'Deny'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            metadata: {
              displayName: 'Public network access should be disabled for CosmosDB'
              description: 'This policy denies that  Cosmos database accounts  are created with out public network access is disabled.'
            }
          }
          MariaDBPublicIpDenyEffect: {
            type: 'string'
            defaultValue: 'Deny'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            metadata: {
              displayName: 'Public network access should be disabled for MariaDB'
              description: 'This policy denies the creation of Maria DB accounts with exposed public endpoints'
            }
          }
          MySQLPublicIpDenyEffect: {
            type: 'string'
            defaultValue: 'Deny'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            metadata: {
              displayName: 'Public network access should be disabled for MySQL'
              description: 'This policy denies creation of MySql DB accounts with exposed public endpoints'
            }
          }
          PostgreSQLPublicIpDenyEffect: {
            type: 'string'
            defaultValue: 'Deny'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            metadata: {
              displayName: 'Public network access should be disabled for PostgreSql'
              description: 'This policy denies creation of Postgre SQL DB accounts with exposed public endpoints'
            }
          }
          KeyVaultPublicIpDenyEffect: {
            type: 'string'
            defaultValue: 'Deny'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            metadata: {
              displayName: 'Public network access should be disabled for KeyVault'
              description: 'This policy denies creation of Key Vaults with IP Firewall exposed to all public endpoints'
            }
          }
          SqlServerPublicIpDenyEffect: {
            type: 'string'
            defaultValue: 'Deny'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            metadata: {
              displayName: 'Public network access on Azure SQL Database should be disabled'
              description: 'This policy denies creation of Sql servers with exposed public endpoints'
            }
          }
          StoragePublicIpDenyEffect: {
            type: 'string'
            defaultValue: 'Deny'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            metadata: {
              displayName: 'Public network access onStorage accounts should be disabled'
              description: 'This policy denies creation of storage accounts with IP Firewall exposed to all public endpoints'
            }
          }
          AKSPublicIpDenyEffect: {
            type: 'string'
            defaultValue: 'Deny'
            allowedValues: [
              'Audit'
              'Deny'
              'Disabled'
            ]
            metadata: {
              displayName: 'Public network access on AKS API should be disabled'
              description: 'This policy denies  the creation of  Azure Kubernetes Service non-private clusters'
            }
          }
        }
        PolicyDefinitionGroups: null
        metadata: {
          version: '1.0.0'
          category: 'Network'
        }
        PolicyDefinitions: [
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deny-PublicEndpoint-CosmosDB'
            policyDefinitionReferenceId: 'CosmosDenyPaasPublicIP'
            parameters: {
              effect: {
                value: '[parameters(\'CosmosPublicIpDenyEffect\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deny-PublicEndpoint-MariaDB'
            policyDefinitionReferenceId: 'MariaDBDenyPaasPublicIP'
            parameters: {
              effect: {
                value: '[parameters(\'MariaDBPublicIpDenyEffect\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deny-PublicEndpoint-MySQL'
            policyDefinitionReferenceId: 'MySQLDenyPaasPublicIP'
            parameters: {
              effect: {
                value: '[parameters(\'MySQLPublicIpDenyEffect\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deny-PublicEndpoint-PostgreSql'
            policyDefinitionReferenceId: 'PostgreSQLDenyPaasPublicIP'
            parameters: {
              effect: {
                value: '[parameters(\'PostgreSQLPublicIpDenyEffect\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deny-PublicEndpoint-KeyVault'
            policyDefinitionReferenceId: 'KeyVaultDenyPaasPublicIP'
            parameters: {
              effect: {
                value: '[parameters(\'KeyVaultPublicIpDenyEffect\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deny-PublicEndpoint-Sql'
            policyDefinitionReferenceId: 'SqlServerDenyPaasPublicIP'
            parameters: {
              effect: {
                value: '[parameters(\'SqlServerPublicIpDenyEffect\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deny-PublicEndpoint-Storage'
            policyDefinitionReferenceId: 'StorageDenyPaasPublicIP'
            parameters: {
              effect: {
                value: '[parameters(\'StoragePublicIpDenyEffect\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deny-PublicEndpoint-Aks'
            policyDefinitionReferenceId: 'AKSDenyPaasPublicIP'
            parameters: {
              effect: {
                value: '[parameters(\'AKSPublicIpDenyEffect\')]'
              }
            }
          }
        ]
      }
      type: null
      name: 'Deny-PublicEndpoints'
    }
    {
      properties: {
        Description: 'Deploy auditing, Alert, TDE and SQL vulnerability to SQL Databases when it not exist in the deployment'
        DisplayName: 'Deploy SQL Database built-in SQL security configuration'
        Parameters: {
          vulnerabilityAssessmentsEmail: {
            metadata: {
              description: 'The email address to send alerts'
              displayName: 'The email address to send alerts'
            }
            type: 'String'
          }
          vulnerabilityAssessmentsStorageID: {
            metadata: {
              description: 'The storage account ID to store assessments'
              displayName: 'The storage account ID to store assessments'
            }
            type: 'String'
          }
          SqlDbTdeDeploySqlSecurityEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy SQL Database Transparent Data Encryption '
              description: 'Deploy the Transparent Data Encryption when it is not enabled in the deployment'
            }
          }
          SqlDbSecurityAlertPoliciesDeploySqlSecurityEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy SQL Database security Alert Policies configuration with email admin accounts'
              description: 'Deploy the security Alert Policies configuration with email admin accounts when it not exist in current configuration'
            }
          }
          SqlDbAuditingSettingsDeploySqlSecurityEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy SQL database auditing settings'
              description: 'Deploy auditing settings to SQL Database when it not exist in the deployment'
            }
          }
          SqlDbVulnerabilityAssessmentsDeploySqlSecurityEffect: {
            type: 'string'
            defaultValue: 'DeployIfNotExists'
            allowedValues: [
              'DeployIfNotExists'
              'Disabled'
            ]
            metadata: {
              displayName: 'Deploy SQL Database vulnerability Assessments'
              description: 'Deploy SQL Database vulnerability Assessments when it not exist in the deployment. To the specific  storage account in the parameters'
            }
          }
        }
        PolicyDefinitionGroups: null
        metadata: {
          version: '1.0.0'
          category: 'SQL'
        }
        PolicyDefinitions: [
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Sql-Tde'
            policyDefinitionReferenceId: 'SqlDbTdeDeploySqlSecurity'
            parameters: {
              effect: {
                value: '[parameters(\'SqlDbTdeDeploySqlSecurityEffect\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Sql-SecurityAlertPolicies'
            policyDefinitionReferenceId: 'SqlDbSecurityAlertPoliciesDeploySqlSecurity'
            parameters: {
              effect: {
                value: '[parameters(\'SqlDbSecurityAlertPoliciesDeploySqlSecurityEffect\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Sql-AuditingSettings'
            policyDefinitionReferenceId: 'SqlDbAuditingSettingsDeploySqlSecurity'
            parameters: {
              effect: {
                value: '[parameters(\'SqlDbAuditingSettingsDeploySqlSecurityEffect\')]'
              }
            }
          }
          {
            policyDefinitionId: '${scope}/providers/Microsoft.Authorization/policyDefinitions/Deploy-Sql-vulnerabilityAssessments'
            policyDefinitionReferenceId: 'SqlDbVulnerabilityAssessmentsDeploySqlSecurity'
            parameters: {
              effect: {
                value: '[parameters(\'SqlDbVulnerabilityAssessmentsDeploySqlSecurityEffect\')]'
              }
              vulnerabilityAssessmentsEmail: {
                value: '[parameters(\'vulnerabilityAssessmentsEmail\')]'
              }
              vulnerabilityAssessmentsStorageID: {
                value: '[parameters(\'vulnerabilityAssessmentsStorageID\')]'
              }
            }
          }
        ]
      }
      type: null
      name: 'Deploy-Sql-Security'
    }
  ]
}

resource policies_policyDefinitions_0_name 'Microsoft.Authorization/policyDefinitions@2019-09-01' = {
  name: policies.policyDefinitions[0].name
  properties: {
    displayName: policies.policyDefinitions[0].properties.displayName
    description: policies.policyDefinitions[0].properties.description
    mode: 'All'
    policyType: 'Custom'
    parameters: policies.policyDefinitions[0].properties.parameters
    policyRule: policies.policyDefinitions[0].properties.policyRule
    metadata: policies.policyDefinitions[0].properties.metadata
  }
}

resource initiatives_policySetDefinitions_0_name 'Microsoft.Authorization/policySetDefinitions@2019-09-01' = {
  name: initiatives.policySetDefinitions[0].name
  properties: {
    displayName: initiatives.policySetDefinitions[0].properties.displayName
    description: initiatives.policySetDefinitions[0].properties.description
    parameters: initiatives.policySetDefinitions[0].properties.parameters
    policyDefinitions: initiatives.policySetDefinitions[0].properties.policyDefinitions
    metadata: initiatives.policySetDefinitions[0].properties.metadata
  }
}