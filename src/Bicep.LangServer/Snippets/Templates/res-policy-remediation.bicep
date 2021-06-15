//Policy Remediation
resource ${1:policyRemediation} 'Microsoft.PolicyInsights/remediations@2019-07-01' = {
  name: ${2:'name'}
  properties: {
    policyAssignmentId: ${3:'policyAssignmentId'}
    policyDefinitionReferenceId: ${4:'policyDefinitionReferenceId'}
    resourceDiscoveryMode: ${5|'ExistingNonCompliant','ReEvaluateCompliance'|}
    filters: {
      locations: [
        ${6:'location'}
      ]
    }
  }
}
