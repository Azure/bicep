resource policyDefinition 'Microsoft.Authorization/policyDefinitions@2020-09-01' = {
//@[83:2173) [BCP135 (Error)] Scope "resourceGroup" is not valid for this resource type. Permitted scopes: "managementGroup", "subscription". (CodeDescription: none) |{\r\n  name: 'name'\r\n  properties: {\r\n    displayName: 'displayName'\r\n    policyType: 'Custom'\r\n    mode: 'All'\r\n    description: 'description'\r\n    metadata: {\r\n      version: '0.1.0'\r\n      category: 'category'\r\n      source: 'source'\r\n    }\r\n    parameters: {\r\n      parameterName: {\r\n        type: 'String'\r\n        defaultValue: 'defaultValue'\r\n        metadata: {\r\n          displayName: 'displayName'\r\n          description: 'description'\r\n        }\r\n      }\r\n    }\r\n    policyRule: {\r\n      if: {\r\n        allOf: [\r\n          {\r\n            field: 'field'\r\n            equals: 'conditionValue'\r\n          }\r\n        ]\r\n      }\r\n      then: {\r\n        effect: 'deployIfNotExists'\r\n        details: {\r\n          roleDefinitionIds: [\r\n            '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'\r\n          ]\r\n          type: 'type'\r\n          existenceCondition: {\r\n            allOf: [\r\n              {\r\n                field: 'field'\r\n                equals: 'conditionValue'\r\n              }\r\n            ]\r\n          }\r\n          deployment: {\r\n            properties: {\r\n              mode: 'incremental'\r\n              template: {\r\n                '$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#'\r\n                contentVersion: '1.0.0.0'\r\n                parameters: {\r\n                  parameterName: {\r\n                    type: 'String'\r\n                    metadata: {\r\n                      displayName: 'displayName'\r\n                      description: 'description'\r\n                    }\r\n                  }\r\n                }\r\n                variables: {}\r\n                resources: [\r\n                  {\r\n                    name: 'name'\r\n                    type: 'type'\r\n                    apiVersion: 'apiVersion'\r\n                    location: 'location'\r\n                    properties: {}\r\n                  }\r\n                ]\r\n              }\r\n              parameters: {\r\n                parameterName: {\r\n                  value: 'value'\r\n                }\r\n        |
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

