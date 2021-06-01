targetScope = 'subscription'
resource policyDefinition 'Microsoft.Authorization/policyDefinitions@2020-09-01' = {
  name: 'name'
  properties: {
    displayName: 'displayName'
    policyType: 'Custom'
    mode: 'Indexed'
    description: 'description'
    metadata: {
      version: '0.1.0'
      category: 'category'
      source: 'sourceRepoName'
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
            field: 'fieldName'
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
              field: 'fieldName'
              value: 'value'
            }
          ]
        }
      }
    }
  }
}
