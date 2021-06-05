// PolicySet Definition
resource ${1:policySetDefinition} 'Microsoft.Authorization/policySetDefinitions@2020-09-01' = {
  name: ${2:'name'}
  properties: {
    displayName: ${3:'displayName'}
    policyType: ${4:'Custom'}
    description: ${5:'description'}
    metadata: {
      version: ${6:'0.1.0'}
      category: ${7:'category'}
      source: ${8:'source'}
    }
    parameters: {
      ${9:parameterName}: {
        type: ${10|'String','Array'|}
        metadata: {
          displayName: ${11:'displayName'}
          description: ${12:'description'}
        }
      }
    }
    policyDefinitions: [
      {
        policyDefinitionId: ${13:'policyDefinitionId'}
        policyDefinitionReferenceId: ${14:'policyDefinitionReferenceId'}
        parameters: {
          ${15:parameterName}: {
            value: ${16:'value'}
          }
        }
      }
      {
        policyDefinitionId: ${17:'policyDefinitionId'}
        policyDefinitionReferenceId: ${18:'policyDefinitionReferenceId'}
        parameters: {
          ${19:parameterName}: {
            value: ${20:'value'}
          }
        }
      }
    ]
  }
}
