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
// $19 = 'Audit'
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
        effect: 'Audit'
      }
    }
  }
}
// Insert snippet here

