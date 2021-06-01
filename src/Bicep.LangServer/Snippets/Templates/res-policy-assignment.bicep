// Policy Assignment
resource ${1:policyAssignment} 'Microsoft.Authorization/policyAssignments@2020-09-01' = {
  name: ${2:'name'}
  location: ${3:'location'}
  identity: {
    type: ${4|'SystemAssigned','None'|}
  }
  properties: {
    displayName: ${5:'displayName'}
    description: ${6:'description'}
    enforcementMode: ${7|'Default','DoNotEnforce'|}
    metadata: {
      source: ${8:'source'}
      version: ${9:'0.1.0'}
    }
    policyDefinitionId: ${10:'policyDefinitionId'}
    parameters: {
      ${11:parameterName}: {
        value: ${12:'value'}
      }
    }
    nonComplianceMessages: [
      {
        message: ${13:'message'}
      }
      {
        message: ${14:'message'}
        policyDefinitionReferenceId: ${15:'policyDefinitionReferenceId'}
      }
    ]
  }
}
