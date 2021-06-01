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
//@[19:100) [no-hardcoded-env-urls (Warning)] Environment URLs should not be hardcoded. Use the environment() function to ensure compatibility across clouds. Found this disallowed host: "management.azure.com" (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-env-urls)) | 'incremental'\r\n              template: {\r\n                '$schema': 'https://sc|
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

