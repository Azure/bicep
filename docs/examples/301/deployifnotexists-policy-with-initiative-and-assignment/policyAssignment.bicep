targetScope = 'subscription'

// PARAMETERS
param bicepExampleInitiativeId string
param assignmentIdentityLocation string
param assignmentEnforcementMode string

// RESOURCES
resource bicepExampleAssignment 'Microsoft.Authorization/policyAssignments@2020-09-01' = {
  name: 'bicepExampleAssignment'
  location: assignmentIdentityLocation
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    displayName: 'Bicep Example Assignment'
    description: 'Bicep Example Assignment'
    enforcementMode: assignmentEnforcementMode
    metadata: {
      source: 'Bicep'
      version: '0.1.0'
    }
    policyDefinitionId: bicepExampleInitiativeId
    nonComplianceMessages: [
      {
        message: 'Resource is not compliant with a DeployIfNotExists policy'
      }
    ]
  }
}
