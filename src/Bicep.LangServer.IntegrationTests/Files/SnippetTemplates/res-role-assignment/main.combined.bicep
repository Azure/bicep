// $1 = roleAssignment
// $2 = 'name'
// $3 = 'roleDefinitionId'
// $4 = 'principalId'
// $5 = 'ServicePrincipal'

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: 'name'
  properties: {
    roleDefinitionId: 'roleDefinitionId'
    principalId: 'principalId'
    principalType: 'ServicePrincipal'
  }
}
// Insert snippet here

