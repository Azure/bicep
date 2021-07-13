// Policy Definition
resource /*${1:policyDefinition}*/policyDefinition 'Microsoft.Authorization/policyDefinitions@2020-09-01' = {
  name: /*${2:'name'}*/'name'
  properties: {
    displayName: /*${3:'displayName'}*/'displayName'
    policyType: /*${4:'Custom'}*/'Custom'
    mode: /*${5|'All','Indexed'|}*/'Indexed'
    description: /*${6:'description'}*/'description'
    metadata: {
      version: /*${7:'0.1.0'}*/'0.1.0'
      category: /*${8:'category'}*/'category'
      source: /*${9:'source'}*/'source'
    }
    parameters: {
      /*${10:parameterName}*/parameterName: {
        type: /*${11|'String','Array'|}*/'String'
        defaultValue: /*${12:'defaultValue'}*/'defaultValue'
        metadata: {
          displayName: /*${13:'displayName'}*/'displayName'
          description: /*${14:'description'}*/'description'
        }
      }
    }
    policyRule: {
      if: {
        /*${15|allOf,anyOf|}*/allOf: [
          {
            field: /*${16:'field'}*/'field'
            /*${17|equals,notEquals,like,notLike,match,matchInsensitively,notMatch,notMatchInsensitively,contains,notContains,in,notIn,containsKey,notContainsKey,less,lessOrEquals,greater,greaterOrEquals,exists|}*/equals: /*${18:'conditionValue'}*/'conditionValue'
          }
        ]
      }
      then: {
        effect: /*${19|'Audit','AuditIfNotExists','Deny','DeployIfNotExists','Disabled','Modify'|}*/'Audit'
        details: {
          roleDefinitionIds: [
            /*${20:'roleDefinitionIds'}*/'roleDefinitionIds'
          ]
          operations: [
            {
              operation: /*${21|'add','addOrReplace'|}*/'add'
              field: /*${22:'field'}*/'field'
              value: /*${23:'value'}*/'value'
            }
          ]
        }
      }
    }
  }
}
