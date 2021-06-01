resource policyAssignment 'Microsoft.Authorization/policyAssignments@2020-09-01' = {
  name: 'name'
  location: 'location'
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
