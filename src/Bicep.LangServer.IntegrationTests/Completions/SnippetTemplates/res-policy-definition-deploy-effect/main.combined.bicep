// $1 = policyDefinition
// $2 = 'name'
// $4 = 'displayName'
// $5 = 'Custom'
// $6 = 'All'
// $7 = 'description'
// $8 = '0.1.0'
// $9 = 'category'
// $10 = 'source'
// $11 = parameterName
// $12 = 'String'
// $13 = 'defaultValue'
// $14 = 'displayName'
// $15 = 'description'
// $16 = allOf
// $17 = 'field'
// $18 = equals
// $19 = 'conditionValue'
// $20 = 'deployIfNotExists'
// $21 = '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
// $22 = 'type'
// $23 = 'field'
// $24 = equals
// $25 = 'conditionValue'
// $26 = 'incremental'
// $27 = 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
// $28 = '1.0.0.0'
// $29 = parameterName
// $30 = 'String'
// $31 = 'displayName'
// $32 = 'description'
// $33 = 'name'
// $34 = 'type'
// $35 = 'apiVersion'
// $36 = parameterName
// $37 = 'value'

targetScope = 'subscription'

resource policyDefinition 'Microsoft.Authorization/policyDefinitions@2020-09-01' = {
  name: 'name'
  properties: {
    displayName: 
//@[17:17) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
    policyType: 'displayName'
    mode: 'Custom'
    description: 'All'
    metadata: {
      version: 'description'
      category: '0.1.0'
      source: 'category'
    }
    parameters: {
      'source': {
        type: parameterName
//@[14:27) [BCP057 (Error)] The name "parameterName" does not exist in the current context. (CodeDescription: none) |parameterName|
        defaultValue: 'String'
        metadata: {
          displayName: 'defaultValue'
          description: 'displayName'
        }
      }
    }
    policyRule: {
      if: {
        'description': [
          {
            field: allOf
//@[12:17) [BCP025 (Error)] The property "field" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |field|
//@[19:24) [BCP057 (Error)] The name "allOf" does not exist in the current context. (CodeDescription: none) |allOf|
            'field': equals
//@[12:19) [BCP025 (Error)] The property "field" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |'field'|
//@[21:27) [BCP069 (Error)] The function "equals" is not supported. Use the "==" operator instead. (CodeDescription: none) |equals|
          }
        ]
      }
      then: {
        effect: 'conditionValue'
        details: {
          roleDefinitionIds: [
            'deployIfNotExists'
          ]
          type: '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
          existenceCondition: {
            allOf: [
              {
                field: 'type'
//@[16:21) [BCP025 (Error)] The property "field" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |field|
                'field': equals 'conditionValue'
//@[16:23) [BCP025 (Error)] The property "field" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |'field'|
//@[25:31) [BCP069 (Error)] The function "equals" is not supported. Use the "==" operator instead. (CodeDescription: none) |equals|
//@[32:48) [BCP019 (Error)] Expected a new line character at this location. (CodeDescription: none) |'conditionValue'|
              }
            ]
          }
          deployment: {
            properties: {
              mode: 'conditionValue'
              template: {
                '$schema': 'incremental'
                contentVersion: 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                parameters: {
                  '1.0.0.0': {
                    type: parameterName
//@[26:39) [BCP057 (Error)] The name "parameterName" does not exist in the current context. (CodeDescription: none) |parameterName|
                    metadata: {
                      displayName: 'String'
                      description: 'displayName'
                    }
                  }
                }
                variables: {}
                resources: [
                  {
                    name: 'description'
                    type: 'name'
                    apiVersion: 'type'
                    location: 'apiVersion'
                    properties: {}
                  }
                ]
              }
              parameters: {
                parameterName: {
                  value: 
//@[25:25) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
                }
              }
            }
          }
        }
      }
    }
  }
}
// Insert snippet here

