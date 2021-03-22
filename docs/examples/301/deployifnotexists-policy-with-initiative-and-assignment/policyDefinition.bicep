targetScope = 'subscription'

// PARAMETERS   
param actionGroupName string
param actionGroupRG string
param actionGroupId string
param metricAlertResourceNamespace string
param metricAlertName string
param metricAlertDimension1 string
param metricAlertDimension2 string
param metricAlertDimension3 string
param metricAlertDescription string
param metricAlertSeverity string
param metricAlertEnabled string
param metricAlertEvaluationFrequency string
param metricAlertWindowSize string
param metricAlertSensitivity string
param metricAlertOperator string
param metricAlertTimeAggregation string
param metricAlertCriterionType string
param metricAlertAutoMitigate string

// VARIABLES
var policyDefCategory = 'Custom'
var policySource = 'Bicep'

// OUTPUTS
output bicepExampleInitiativeId string = bicepExampleInitiative.id

// RESOURCES
resource bicepExampleDINEpolicy 'Microsoft.Authorization/policyDefinitions@2020-09-01' = {
  name: 'bicepExampleDINEpolicy'
  properties: {
    displayName: 'DINE metric alert to Load Balancer for dipAvailability'
    description: 'DeployIfNotExists a metric alert to Load Balancers for dipAvailability (Average Load Balancer health probe status per time duration)'
    policyType: 'Custom'
    mode: 'All'
    metadata: {
      category: policyDefCategory
      source: policySource
      version: '0.1.0'
    }
    parameters: {}
    policyRule: {
      if: {
        allOf: [
          {
            field: 'type'
            equals: metricAlertResourceNamespace
          }
          {
            field: 'Microsoft.Network/loadBalancers/sku.name'
            equals: 'Standard' // only Standard SKU support metric alerts
          }
        ]
      }
      then: {
        effect: 'deployIfNotExists'
        details: {
          roleDefinitionIds: [
            '/providers/microsoft.authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c' // contributor RBAC role for deployIfNotExists effect
          ]
          type: 'Microsoft.Insights/metricAlerts'
          existenceCondition: {
            allOf: [
              {
                field: 'Microsoft.Insights/metricAlerts/criteria.Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria.allOf[*].metricNamespace'
                equals: metricAlertResourceNamespace
              }
              {
                field: 'Microsoft.Insights/metricAlerts/criteria.Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria.allOf[*].metricName'
                equals: metricAlertName
              }
              {
                field: 'Microsoft.Insights/metricalerts/scopes[*]'
                equals: '[concat(subscription().id, \'/resourceGroups/\', resourceGroup().name, \'/providers/${metricAlertResourceNamespace}/\', field(\'fullName\'))]'
              }
            ]
          }
          deployment: {
            properties: {
              mode: 'incremental'
              template: {
                '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                contentVersion: '1.0.0.0'
                parameters: {
                  resourceName: {
                    type: 'String'
                    metadata: {
                      displayName: 'resourceName'
                      description: 'Name of the resource'
                    }
                  }
                  resourceId: {
                    type: 'String'
                    metadata: {
                      displayName: 'resourceId'
                      description: 'Resource ID of the resource emitting the metric that will be used for the comparison'
                    }
                  }
                  resourceLocation: {
                    type: 'String'
                    metadata: {
                      displayName: 'resourceLocation'
                      description: 'Location of the resource'
                    }
                  }
                  actionGroupName: {
                    type: 'String'
                    metadata: {
                      displayName: 'actionGroupName'
                      description: 'Name of the Action Group'
                    }
                  }
                  actionGroupRG: {
                    type: 'String'
                    metadata: {
                      displayName: 'actionGroupRG'
                      description: 'Resource Group containing the Action Group'
                    }
                  }
                  actionGroupId: {
                    type: 'String'
                    metadata: {
                      displayName: 'actionGroupId'
                      description: 'The ID of the action group that is triggered when the alert is activated or deactivated'
                    }
                  }
                }
                variables: {}
                resources: [
                  {
                    type: 'Microsoft.Insights/metricAlerts'
                    apiVersion: '2018-03-01'
                    name: '[concat(parameters(\'resourceName\'), \'-${metricAlertName}\')]'
                    location: 'global'
                    properties: {
                      description: metricAlertDescription
                      severity: metricAlertSeverity
                      enabled: metricAlertEnabled
                      scopes: [
                        '[parameters(\'resourceId\')]'
                      ]
                      evaluationFrequency: metricAlertEvaluationFrequency
                      windowSize: metricAlertWindowSize
                      criteria: {
                        allOf: [
                          {
                            alertSensitivity: metricAlertSensitivity
                            failingPeriods: {
                              numberOfEvaluationPeriods: '2'
                              minFailingPeriodsToAlert: '1'
                            }
                            name: 'Metric1'
                            metricNamespace: metricAlertResourceNamespace
                            metricName: metricAlertName
                            dimensions: [
                              {
                                name: metricAlertDimension1
                                operator: 'Include'
                                values: [
                                  '*'
                                ]
                              }
                              {
                                name: metricAlertDimension2
                                operator: 'Include'
                                values: [
                                  '*'
                                ]
                              }
                              {
                                name: metricAlertDimension3
                                operator: 'Include'
                                values: [
                                  '*'
                                ]
                              }
                            ]
                            operator: metricAlertOperator
                            timeAggregation: metricAlertTimeAggregation
                            criterionType: metricAlertCriterionType
                          }
                        ]
                        'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
                      }
                      autoMitigate: metricAlertAutoMitigate
                      targetResourceType: metricAlertResourceNamespace
                      targetResourceRegion: '[parameters(\'resourceLocation\')]'
                      actions: [
                        {
                          actionGroupId: actionGroupId
                          webHookProperties: {}
                        }
                      ]
                    }
                  }
                ]
              }
              parameters: {
                resourceName: {
                  value: '[field(\'name\')]'
                }
                resourceId: {
                  value: '[field(\'id\')]'
                }
                resourceLocation: {
                  value: '[field(\'location\')]'
                }
                actionGroupName: {
                  value: actionGroupName
                }
                actionGroupRG: {
                  value: actionGroupRG
                }
                actionGroupID: {
                  value: actionGroupId
                }
              }
            }
          }
        }
      }
    }
  }
}

resource bicepExampleInitiative 'Microsoft.Authorization/policySetDefinitions@2020-09-01' = {
  name: 'bicepExampleInitiative'
  properties: {
    policyType: 'Custom'
    displayName: 'Bicep Example Initiative'
    description: 'Bicep Example Initiative'
    metadata: {
      category: policyDefCategory
      source: policySource
      version: '0.1.0'
    }
    parameters: {}
    policyDefinitions: [
      {
        policyDefinitionId: bicepExampleDINEpolicy.id
        parameters: {}
      }
    ]
  }
}
