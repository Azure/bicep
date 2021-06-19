//Policy Exemption
resource ${1:policyExemption} 'Microsoft.Authorization/policyExemptions@2020-07-01-preview' = {
  name: ${2:'name'}
  properties: {
    policyAssignmentId: ${3:'policyAssignmentId'}
    policyDefinitionReferenceIds: [
      ${4:'policyDefinitionReferenceIds'}
    ]
    exemptionCategory: ${5|'Mitigated','Waiver'|}
    expiresOn: ${6:'expiresOn'}
    displayName: ${7:'displayName'}
    description: ${8:'description'}
    metadata: {
      version: ${9:'0.1.0'}
      source: ${10:'source'}
    }
  }
}
