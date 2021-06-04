// $1 = policyExemption
// $2 = 'name'
// $3 = 'policyAssignmentId'
// $4 = 'policyDefinitionReferenceIds'
// $5 = 'Waiver'
// $6 = 'expiresOn'
// $7 = 'displayName'
// $8 = 'description'
// $9 = '0.1.0'
// $10 = 'source'

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
// Insert snippet here

