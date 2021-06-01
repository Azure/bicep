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
            'roleDefinitionIds'
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
