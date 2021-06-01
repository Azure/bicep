resource policySetDefinition 'Microsoft.Authorization/policySetDefinitions@2020-09-01' = {
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

