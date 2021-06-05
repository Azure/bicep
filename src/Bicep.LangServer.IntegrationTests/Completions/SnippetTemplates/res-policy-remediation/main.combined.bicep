// $1 = policyRemediation
// $2 = 'name'
// $3 = 'policyAssignmentId'
// $4 = 'policyDefinitionReferenceId'
// $5 = 'ExistingNonCompliant'
// $6 = 'location'

resource policyRemediation 'Microsoft.PolicyInsights/remediations@2019-07-01' = {
  name: 'name'
  properties: {
    policyAssignmentId: 'policyAssignmentId'
    policyDefinitionReferenceId: 'policyDefinitionReferenceId'
    resourceDiscoveryMode: 'ExistingNonCompliant'
    filters: {
      locations: [
        'location'
      ]
    }
  }
}
// Insert snippet here

