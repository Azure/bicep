//Policy Remediation
resource /*${1:policyRemediation}*/ policyRemediation 'Microsoft.PolicyInsights/remediations@2019-07-01' = {
  name: /*${2:'name'}*/ 'name'
  properties: {
    policyAssignmentId: /*${3:'policyAssignmentId'}*/ 'policyAssignmentId'
    policyDefinitionReferenceId: /*${4:'policyDefinitionReferenceId'}*/ 'policyDefinitionReferenceId'
    resourceDiscoveryMode: /*${5|'ExistingNonCompliant','ReEvaluateCompliance'|}*/ 'ExistingNonCompliant'
    filters: {
      locations: [
        /*${6:location}*/'location'
      ]
    }
  }
}
