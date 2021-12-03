// Policy Definition
resource /*${1:policyDefinition}*/ policyDefinition 'Microsoft.Authorization/policyDefinitions@2020-09-01' = {
  name: /*${2:'name'}*/ 'name'
  properties: {
    displayName: /*${3:'displayName'}*/ 'displayName'
    policyType: /*${4:'Custom'}*/ 'Custom'
    mode: /*${5|'All','Indexed'|}*/ 'All'
    description: /*${6:'description'}*/ 'description'
    metadata: {
      version: /*${7:'0.1.0'}*/ '0.1.0'
      category: /*${8:'category'}*/ 'category'
      source: /*${9:'source'}*/ 'source'
    }
    parameters: {
      /*${10:parameterName}*/'parameterName': {
        type: /*${11|'String','Array'|}*/ 'String'
        defaultValue: /*${12:'defaultValue'}*/ 'defaultValue'
        metadata: {
          displayName: /*${13:'displayName'}*/ 'displayName'
          description: /*${14:'description'}*/ 'description'
        }
      }
    }
    policyRule: {
      if: {
        /*${15|allOf,anyOf|}*/allOf: [
          {
            field: /*${16:'field'}*/ 'field'
            /*${17|equals,notEquals,like,notLike,match,matchInsensitively,notMatch,notMatchInsensitively,contains,notContains,in,notIn,containsKey,notContainsKey,less,lessOrEquals,greater,greaterOrEquals,exists|}*/equals: /*${18:'conditionValue'}*/ 'conditionValue'
          }
        ]
      }
      then: {
        effect: /*${19|'Audit','AuditIfNotExists','Deny','DeployIfNotExists','Disabled','Modify'|}*/ 'Audit'
        details: {
          roleDefinitionIds: [
            /*${20:'roleDefinitionIds'}*/'roleDefinitionIds'
          ]
          type: /*${21:'type'}*/ 'type'
          existenceCondition: {
            allOf: [
              {
                field: /*${22:'field'}*/ 'field'
                /*${23|equals,notEquals,like,notLike,match,matchInsensitively,notMatch,notMatchInsensitively,contains,notContains,in,notIn,containsKey,notContainsKey,less,lessOrEquals,greater,greaterOrEquals,exists|}*/equals: /*${24:'conditionValue'}*/ 'conditionValue'
              }
            ]
          }
          deployment: {
            properties: {
              mode: /*${25|'incremental','complete'|}*/ 'incremental'
              template: {
                '$schema': /*${26:'schema'}*/ 'schema'
                contentVersion: /*${27:'1.0.0.0'}*/ '1.0.0.0'
                parameters: {
                  /*${28:parameterName}*/parameterName: {
                    type: /*${29|'String','Array'|}*/ 'String'
                    metadata: {
                      displayName: /*${30:'displayName'}*/ 'displayName'
                      description: /*${31:'description'}*/ 'description'
                    }
                  }
                }
                variables: {}
                resources: [
                  {
                    name: /*${32:'name'}*/ 'name'
                    type: /*${33:'type'}*/ 'type'
                    apiVersion: /*${34:'apiVersion'}*/ 'apiVersion'
                    location: /*${35:location}*/ 'location'
                    properties: {}
                  }
                ]
              }
              parameters: {
                /*${36:parameterName}*/'parameterName': {
                  value: /*${37:'value'}*/ 'value'
                }
              }
            }
          }
        }
      }
    }
  }
}
