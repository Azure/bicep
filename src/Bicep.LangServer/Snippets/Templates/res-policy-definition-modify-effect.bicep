// Policy Definition
resource ${1:policyDefinition} 'Microsoft.Authorization/policyDefinitions@2020-09-01' = {
  name: ${2:'name'}
  properties: {
    displayName: ${3:'displayName'}
    policyType: ${4:'Custom'}
    mode: ${5|'All','Indexed'|}
    description: ${6:'description'}
    metadata: {
      version: ${7:'0.1.0'}
      category: ${8:'category'}
      source: ${9:'source'}
    }
    parameters: {
      ${10:parameterName}: {
        type: ${11|'String','Array'|}
        defaultValue: ${12:'defaultValue'}
        metadata: {
          displayName: ${13:'displayName'}
          description: ${14:'description'}
        }
      }
    }
    policyRule: {
      if: {
        ${15|allOf,anyOf|}: [
          {
            field: ${16:'field'}
            ${17|equals,notEquals,like,notLike,match,matchInsensitively,notMatch,notMatchInsensitively,contains,notContains,in,notIn,containsKey,notContainsKey,less,lessOrEquals,greater,greaterOrEquals,exists|}: ${18:'conditionValue'}
          }
        ]
      }
      then: {
        effect: ${19|'Audit','AuditIfNotExists','Deny','DeployIfNotExists','Disabled','Modify'|}
        details: {
          roleDefinitionIds: [
            ${20:'roleDefinitionIds'}
          ]
          operations: [
            {
              operation: ${21|'add','addOrReplace'|}
              field: ${22:'field'}
              value: ${23:'value'}
            }
          ]
        }
      }
    }
  }
}
