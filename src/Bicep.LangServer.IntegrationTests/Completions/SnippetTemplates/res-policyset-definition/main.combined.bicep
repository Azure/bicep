// $1 = policySetDefinition
// $2 = 'name'
// $3 = 'displayName'
// $4 = 'Custom'
// $5 = 'description'
// $6 = '0.1.0'
// $7 = 'category'
// $8 = 'source'
// $9 = parameterName
// $10 = 'String'
// $11 = 'displayName'
// $12 = 'description'
// $13 = 'policyDefinitionId'
// $14 = 'policyDefinitionReferenceId'
// $15 = parameterName
// $16 = 'value'
// $17 = 'policyDefinitionId'
// $18 = 'policyDefinitionReferenceId'
// $19 = parameterName
// $20 = 'value'
targetScope = 'subscription'
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
// Insert snippet here

