// DEPLOYMENT SCOPE
targetScope = 'subscription'

// PARAMETERS
param resourceGroupName string = 'BicepExampleRG'
param location string = 'australiaeast'
param actionGroupName string = 'BicepExampleAG'

// VARIABLES

// OUTPUTS

// RESOURCES
module rg './resourceGroup.bicep' = {
  scope: subscription()
  name: 'resourceGroup'
  params: {
    resourceGroupName: resourceGroupName
    location: location
  }
}

module ag './actionGroup.bicep' = {
  scope: resourceGroup(resourceGroupName)
  name: 'actionGroup'
  params: {
  actionGroupName: actionGroupName
  actionGroupEnabled: true
  actionGroupShortName: 'azspgcln'
  actionGroupEmailName: 'jloudon'
  actionGroupEmail: 'jesse.loudon@lab3.com.au'
  actionGroupAlertSchema: true
  }
  dependsOn:[
    rg
  ]
}

module policy './policyDefinition.bicep' = {
  scope: subscription()
  name: 'policy'
  params: {
  actionGroupName: ag.outputs.actionGroupName
  actionGroupRG: resourceGroupName
  actionGroupId: ag.outputs.actionGroupId
  metricAlertResourceNamespace: 'Microsoft.Network/loadBalancers'
  metricAlertName: 'DipAvailability'
  metricAlertDimension1: 'ProtocolType'
  metricAlertDimension2: 'FrontendIPAddress'
  metricAlertDimension3: 'BackendIPAddress'
  metricAlertDescription: 'Average Load Balancer health probe status per time duration'
  metricAlertSeverity: '2'
  metricAlertEnabled: 'true'
  metricAlertEvaluationFrequency: 'PT15M'
  metricAlertWindowSize: 'PT1H'
  metricAlertSensitivity: 'Medium'
  metricAlertOperator: 'LessThan'
  metricAlertTimeAggregation: 'Average'
  metricAlertCriterionType: 'DynamicThresholdCriterion'
  metricAlertAutoMitigate: 'true'
  }
}

module assignment './policyAssignment.bicep' = {
  scope: subscription()
  name: 'assignment'
  params: {
  location: location
  bicepExampleInitiativeId: policy.outputs.bicepExampleInitiativeId
  }
  dependsOn: [
    policy
  ]
}