// DEPLOYMENT SCOPE
targetScope = 'subscription'

// PARAMETERS
param bicepExampleInitiativeId string
param location string

// VARIABLES

// OUTPUTS

// RESOURCES
resource bicepExampleAssignment 'Microsoft.Authorization/policyAssignments@2020-09-01' = {
  name: 'bicepExampleAssignment'
  location: location
  identity:{
    type: 'SystemAssigned'
  }
  properties: {
    displayName: 'Bicep Example Assignment'
    description: 'Bicep Example Assignment'
    enforcementMode:'Default'
    metadata: {
      source: 'Bicep'
      version: '0.1.0'
    }
    policyDefinitionId: bicepExampleInitiativeId
    parameters: {}
    nonComplianceMessages: [
      {
        message: 'Resource is not compliant with a DeployIfNotExists policy'
      }
    ]
  }
}