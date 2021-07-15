// Policy Assignment
resource /*${1:policyAssignment}*/policyAssignment 'Microsoft.Authorization/policyAssignments@2020-09-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:'location'}*/'location'
  identity: {
    type: /*${4|'SystemAssigned','None'|}*/'SystemAssigned'
  }
  properties: {
    displayName: /*${5:'displayName'}*/'displayName'
    description: /*${6:'description'}*/'description'
    enforcementMode: /*${7|'Default','DoNotEnforce'|}*/'Default'
    metadata: {
      source: /*${8:'source'}*/'source'
      version: /*${9:'0.1.0'}*/'0.1.0'
    }
    policyDefinitionId: /*${10:'policyDefinitionId'}*/'policyDefinitionId'
    parameters: {
      /*${11:parameterName}*/'parameterName': {
        value: /*${12:'value'}*/'value'
      }
    }
    nonComplianceMessages: [
      {
        message: /*${13:'message'}*/'message'
      }
      {
        message: /*${14:'message'}*/'message'
        policyDefinitionReferenceId: /*${15:'policyDefinitionReferenceId'}*/'policyDefinitionReferenceId'
      }
    ]
  }
}
