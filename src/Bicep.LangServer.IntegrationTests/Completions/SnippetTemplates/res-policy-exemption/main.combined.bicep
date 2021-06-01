resource policyExemption 'Microsoft.Authorization/policyExemptions@2020-07-01-preview' = {
  name: 'name'
  properties: {
    policyAssignmentId: 'policyAssignmentId'
    policyDefinitionReferenceIds: [
      'policyDefinitionReferenceIds'
    ]
    exemptionCategory: 'Waiver'
    expiresOn: 'expiresOn'
    displayName: 'displayName'
    description: 'description'
    metadata: {
      version: '0.1.0'
      source: 'source'
    }
  }
}
