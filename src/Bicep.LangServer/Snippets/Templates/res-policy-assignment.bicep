// Policy Assignment
resource /*${1:policyAssignment}*/ policyAssignment 'Microsoft.Authorization/policyAssignments@2020-09-01' = {
  name: /*${2:'name'}*/ 'name'
  location: location
  identity: {
    type: /*${3|'SystemAssigned','None'|}*/ 'SystemAssigned'
  }
  properties: {
    displayName: /*${4:'displayName'}*/ 'displayName'
    description: /*${5:'description'}*/ 'description'
    enforcementMode: /*${6|'Default','DoNotEnforce'|}*/ 'Default'
    metadata: {
      source: /*${7:'source'}*/ 'source'
      version: /*${8:'0.1.0'}*/ '0.1.0'
    }
    policyDefinitionId: /*${9:'policyDefinitionId'}*/ 'policyDefinitionId'
    parameters: {
      /*${10:parameterName}*/'parameterName': {
        value: /*${11:'value'}*/ 'value'
      }
    }
    nonComplianceMessages: [
      {
        message: /*${12:'message'}*/ 'message'
      }
      {
        message: /*${13:'message'}*/ 'message'
        policyDefinitionReferenceId: /*${14:'policyDefinitionReferenceId'}*/ 'policyDefinitionReferenceId'
      }
    ]
  }
}
