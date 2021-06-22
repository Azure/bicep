// $1 = policyDefinition
// $2 = 'name'
// $3 = 'displayName'
// $4 = 'Custom'
// $5 = 'All'
// $6 = 'description'
// $7 = '0.1.0'
// $8 = 'category'
// $9 = 'source'
// $10 = parameterName
// $11 = 'String'
// $12 = 'defaultValue'
// $13 = 'displayName'
// $14 = 'description'
// $15 = allOf
// $16 = 'field'
// $17 = equals
// $18 = 'conditionValue'
// $19 = 'deployIfNotExists'
// $20 = '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
// $21 = 'type'
// $22 = 'field'
// $23 = equals
// $24 = 'conditionValue'
// $25 = 'incremental'
// $26 = 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
// $27 = '1.0.0.0'
// $28 = parameterName
// $29 = 'String'
// $30 = 'displayName'
// $31 = 'description'
// $32 = 'name'
// $33 = 'type'
// $34 = 'apiVersion'
// $35 = 'location'
// $36 = parameterName
// $37 = 'value'
targetScope = 'subscription'
resource policyDefinition 'Microsoft.Authorization/policyDefinitions@2020-09-01' = {
  name: 'name'
  properties: {
    displayName: 'displayName'
    policyType: 'Custom'
    mode: 'All'
    description: 'description'
    metadata: {
      version: '0.1.0'
      category: 'category'
      source: 'source'
    }
    parameters: {
      parameterName: {
        type: 'String'
        defaultValue: 'defaultValue'
        metadata: {
          displayName: 'displayName'
          description: 'description'
        }
      }
    }
    policyRule: {
      if: {
        allOf: [
          {
            field: 'field'
            equals: 'conditionValue'
          }
        ]
      }
      then: {
        effect: 'deployIfNotExists'
        details: {
          roleDefinitionIds: [
            '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
          ]
          type: 'type'
          existenceCondition: {
            allOf: [
              {
                field: 'field'
                equals: 'conditionValue'
              }
            ]
          }
          deployment: {
            properties: {
              mode: 'incremental'
              template: {
                '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'
                contentVersion: '1.0.0.0'
                parameters: {
                  parameterName: {
                    type: 'String'
                    metadata: {
                      displayName: 'displayName'
                      description: 'description'
                    }
                  }
                }
                variables: {}
                resources: [
                  {
                    name: 'name'
                    type: 'type'
                    apiVersion: 'apiVersion'
                    location: 'location'
                    properties: {}
                  }
                ]
              }
              parameters: {
                parameterName: {
                  value: 'value'
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

