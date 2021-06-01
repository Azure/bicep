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

