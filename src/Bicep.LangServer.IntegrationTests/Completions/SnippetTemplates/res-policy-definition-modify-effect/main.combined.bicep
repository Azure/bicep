resource policyDefinition 'Microsoft.Authorization/policyDefinitions@2020-09-01' = {
//@[83:1132) [BCP135 (Error)] Scope "resourceGroup" is not valid for this resource type. Permitted scopes: "managementGroup", "subscription". (CodeDescription: none) |{\r\n  name: 'name'\r\n  properties: {\r\n    displayName: 'displayName'\r\n    policyType: 'Custom'\r\n    mode: 'Indexed'\r\n    description: 'description'\r\n    metadata: {\r\n      version: '0.1.0'\r\n      category: 'category'\r\n      source: 'source'\r\n    }\r\n    parameters: {\r\n      parameterName: {\r\n        type: 'String'\r\n        defaultValue: 'defaultValue'\r\n        metadata: {\r\n          displayName: 'displayName'\r\n          description: 'description'\r\n        }\r\n      }\r\n    }\r\n    policyRule: {\r\n      if: {\r\n        allOf: [\r\n          {\r\n            field: 'field'\r\n            equals: 'conditionValue'\r\n          }\r\n        ]\r\n      }\r\n      then: {\r\n        effect: 'Modify'\r\n        details: {\r\n          roleDefinitionIds: [\r\n            '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'\r\n          ]\r\n          operations: [\r\n            {\r\n              operation: 'addOrReplace'\r\n              field: 'field'\r\n              value: 'value'\r\n            }\r\n          ]\r\n        }\r\n      }\r\n    }\r\n  }\r\n}|
  name: 'name'
  properties: {
    displayName: 'displayName'
    policyType: 'Custom'
    mode: 'Indexed'
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
        effect: 'Modify'
        details: {
          roleDefinitionIds: [
            '/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
          ]
          operations: [
            {
              operation: 'addOrReplace'
              field: 'field'
              value: 'value'
            }
          ]
        }
      }
    }
  }
}

