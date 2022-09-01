// $1 = policyAssignment
// $2 = 'name'
// $3 = location
// $4 = 'SystemAssigned'
// $5 = 'displayName'
// $6 = 'description'
// $7 = 'Default'
// $8 = 'source'
// $9 = '0.1.0'
// $10 = 'policyDefinitionId'
// $11 = parameterName
// $12 = 'value'
// $13 = 'message'
// $14 = 'message'
// $15 = 'policyDefinitionReferenceId'

param location string

resource policyAssignment 'Microsoft.Authorization/policyAssignments@2020-09-01' = {
  name: 'name'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    displayName: 'displayName'
    description: 'description'
    enforcementMode: 'Default'
    metadata: {
      source: 'source'
      version: '0.1.0'
    }
    policyDefinitionId: 'policyDefinitionId'
    parameters: {
      parameterName: {
        value: 'value'
      }
    }
    nonComplianceMessages: [
      {
        message: 'message'
      }
      {
        message: 'message'
        policyDefinitionReferenceId: 'policyDefinitionReferenceId'
      }
    ]
  }
}
// Insert snippet here

