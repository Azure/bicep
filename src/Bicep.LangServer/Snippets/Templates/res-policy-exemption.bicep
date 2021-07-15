//Policy Exemption
resource /*${1:policyExemption}*/policyExemption 'Microsoft.Authorization/policyExemptions@2020-07-01-preview' = {
  name: /*${2:'name'}*/'name'
  properties: {
    policyAssignmentId: /*${3:'policyAssignmentId'}*/'policyAssignmentId'
    policyDefinitionReferenceIds: [
      /*${4:'policyDefinitionReferenceIds'}*/'policyDefinitionReferenceIds'
    ]
    exemptionCategory: /*${5|'Mitigated','Waiver'|}*/'Mitigated'
    expiresOn: /*${6:'expiresOn'}*/'expiresOn'
    displayName: /*${7:'displayName'}*/'displayName'
    description: /*${8:'description'}*/'description'
    metadata: {
      version: /*${9:'0.1.0'}*/'0.1.0'
      source: /*${10:'source'}*/'source'
    }
  }
}
