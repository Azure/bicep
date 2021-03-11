targetScope = 'subscription'

// PARAMETERS
param resourceGroupName string = 'BicepExampleRG'
param resourceGrouplocation string = 'australiaeast'
param actionGroupName string = 'BicepExampleAG'
param actionGroupEnabled bool = true
param actionGroupShortName string = 'bicepag'
param actionGroupEmailName string = 'jloudon'
param actionGroupEmail string = 'jesse.loudon@lab3.com.au'
param actionGroupAlertSchema bool = true
param metricAlertResourceNamespace string = 'Microsoft.Network/loadBalancers'
param metricAlertName string = 'DipAvailability'
param metricAlertDimension1 string = 'ProtocolType'
param metricAlertDimension2 string = 'FrontendIPAddress'
param metricAlertDimension3 string = 'BackendIPAddress'
param metricAlertDescription string = 'Average Load Balancer health probe status per time duration'
param metricAlertSeverity string = '2'
param metricAlertEnabled string = 'true'
param metricAlertEvaluationFrequency string = 'PT15M'
param metricAlertWindowSize string = 'PT1H'
param metricAlertSensitivity string = 'Medium'
param metricAlertOperator string = 'LessThan'
param metricAlertTimeAggregation string = 'Average'
param metricAlertCriterionType string = 'DynamicThresholdCriterion'
param metricAlertAutoMitigate string = 'true'
param assignmentEnforcementMode string = 'Default'

// RESOURCES
resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: resourceGroupName
  location: resourceGrouplocation
}

module ag './actionGroup.bicep' = {
  scope: rg
  name: 'actionGroup'
  params: {
    actionGroupName: actionGroupName
    actionGroupEnabled: actionGroupEnabled
    actionGroupShortName: actionGroupShortName
    actionGroupEmailName: actionGroupEmailName
    actionGroupEmail: actionGroupEmail
    actionGroupAlertSchema: actionGroupAlertSchema
  }
}

module policy './policyDefinition.bicep' = {
  name: 'policy'
  params: {
    actionGroupName: ag.outputs.actionGroupName
    actionGroupRG: resourceGroupName
    actionGroupId: ag.outputs.actionGroupId
    metricAlertResourceNamespace: metricAlertResourceNamespace
    metricAlertName: metricAlertName
    metricAlertDimension1: metricAlertDimension1
    metricAlertDimension2: metricAlertDimension2
    metricAlertDimension3: metricAlertDimension3
    metricAlertDescription: metricAlertDescription
    metricAlertSeverity: metricAlertSeverity
    metricAlertEnabled: metricAlertEnabled
    metricAlertEvaluationFrequency: metricAlertEvaluationFrequency
    metricAlertWindowSize: metricAlertWindowSize
    metricAlertSensitivity: metricAlertSensitivity
    metricAlertOperator: metricAlertOperator
    metricAlertTimeAggregation: metricAlertTimeAggregation
    metricAlertCriterionType: metricAlertCriterionType
    metricAlertAutoMitigate: metricAlertAutoMitigate
  }
}

module assignment './policyAssignment.bicep' = {
  name: 'assignment'
  params: {
    bicepExampleInitiativeId: policy.outputs.bicepExampleInitiativeId
    assignmentIdentityLocation: resourceGrouplocation
    assignmentEnforcementMode: assignmentEnforcementMode
  }
}
