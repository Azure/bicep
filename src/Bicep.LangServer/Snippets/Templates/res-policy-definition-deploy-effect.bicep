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
          type: ${21:'type'}
          existenceCondition: {
            allOf: [
              {
                field: ${22:'field'}
                ${23|equals,notEquals,like,notLike,match,matchInsensitively,notMatch,notMatchInsensitively,contains,notContains,in,notIn,containsKey,notContainsKey,less,lessOrEquals,greater,greaterOrEquals,exists|}: ${24:'conditionValue'}
              }
            ]
          }
          deployment: {
            properties: {
              mode: ${25|'incremental','complete'|}
              template: {
                '$schema': ${26:'schema'}
                contentVersion: ${27:'1.0.0.0'}
                parameters: {
                  ${28:parameterName}: {
                    type: ${29|'String','Array'|}
                    metadata: {
                      displayName: ${30:'displayName'}
                      description: ${31:'description'}
                    }
                  }
                }
                variables: {}
                resources: [
                  {
                    name: ${32:'name'}
                    type: ${33:'type'}
                    apiVersion: ${34:'apiVersion'}
                    location: ${35:'location'}
                    properties: {}
                  }
                ]
              }
              parameters: {
                ${36:parameterName}: {
                  value: ${37:'value'}
                }
              }
            }
          }
        }
      }
    }
  }
}
