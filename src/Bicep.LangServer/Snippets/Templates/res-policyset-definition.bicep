// PolicySet Definition
resource /*${1:policySetDefinition}*/policySetDefinition 'Microsoft.Authorization/policySetDefinitions@2020-09-01' = {
  name: /*${2:'name'}*/'name'
  properties: {
    displayName: /*${3:'displayName'}*/'displayName'
    policyType: /*${4:'Custom'}*/'Custom'
    description: /*${5:'description'}*/'description'
    metadata: {
      version: /*${6:'0.1.0'}*/'0.1.0'
      category: /*${7:'category'}*/'category'
      source: /*${8:'source'}*/'source'
    }
    parameters: {
      /*${9:parameterName}*/'parameterName': {
        type: /*${10|'String','Array'|}*/'String'
        metadata: {
          displayName: /*${11:'displayName'}*/'displayName'
          description: /*${12:'description'}*/'description'
        }
      }
    }
    policyDefinitions: [
      {
        policyDefinitionId: /*${13:'policyDefinitionId'}*/'policyDefinitionId'
        policyDefinitionReferenceId: /*${14:'policyDefinitionReferenceId'}*/'policyDefinitionReferenceId'
        parameters: {
          /*${15:parameterName}*/'parameterName': {
            value: /*${16:'value'}*/'value'
          }
        }
      }
      {
        policyDefinitionId: /*${17:'policyDefinitionId'}*/'policyDefinitionId'
        policyDefinitionReferenceId: /*${18:'policyDefinitionReferenceId'}*/'policyDefinitionReferenceId'
        parameters: {
          /*${19:parameterName}*/'parameterName': {
            value: /*${20:'value'}*/'value'
          }
        }
      }
    ]
  }
}
