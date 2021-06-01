targetScope = 'subscription'
resource policySetDefinition 'Microsoft.Authorization/policySetDefinitions@2020-09-01' = {
//@[89:1023) [BCP135 (Error)] Scope "resourceGroup" is not valid for this resource type. Permitted scopes: "managementGroup", "subscription". (CodeDescription: none) |{\r\n  name: 'name'\r\n  properties: {\r\n    displayName: 'displayName'\r\n    policyType: 'Custom'\r\n    description: 'description'\r\n    metadata: {\r\n      version: '0.1.0'\r\n      category: 'category'\r\n      source: 'source'\r\n    }\r\n    parameters: {\r\n      parameterName: {\r\n        type: 'String'\r\n        metadata: {\r\n          displayName: 'displayName'\r\n          description: 'description'\r\n        }\r\n      }\r\n    }\r\n    policyDefinitions: [\r\n      {\r\n        policyDefinitionId: 'policyDefinitionId'\r\n        policyDefinitionReferenceId: 'policyDefinitionReferenceId'\r\n        parameters: {\r\n          parameterName: {\r\n            value: 'value'\r\n          }\r\n        }\r\n      }\r\n      {\r\n        policyDefinitionId: 'policyDefinitionId'\r\n        policyDefinitionReferenceId: 'policyDefinitionReferenceId'\r\n        parameters: {\r\n          parameterName: {\r\n            value: 'value'\r\n          }\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}|
  name: 'name'
  properties: {
    displayName: 'displayName'
    policyType: 'Custom'
    description: 'description'
    metadata: {
      version: '0.1.0'
      category: 'category'
      source: 'source'
    }
    parameters: {
      parameterName: {
        type: 'String'
        metadata: {
          displayName: 'displayName'
          description: 'description'
        }
      }
    }
    policyDefinitions: [
      {
        policyDefinitionId: 'policyDefinitionId'
        policyDefinitionReferenceId: 'policyDefinitionReferenceId'
        parameters: {
          parameterName: {
            value: 'value'
          }
        }
      }
      {
        policyDefinitionId: 'policyDefinitionId'
        policyDefinitionReferenceId: 'policyDefinitionReferenceId'
        parameters: {
          parameterName: {
            value: 'value'
          }
        }
      }
    ]
  }
}

