// Test file for roleDefinition function
targetScope = 'subscription'

param principalId string = '00000000-0000-0000-0000-000000000000'

// Test the roleDefinition function
var contributorRole = az.roleDefinition('Contributor')
var ownerRole = az.roleDefinition('Owner')
var readerRole = az.roleDefinition('Reader')

// Use in role assignment with a simpler name calculation
resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(principalId, 'contributor')
  properties: {
    roleDefinitionId: contributorRole.id
    principalId: principalId
  }
}

// Output the role definitions for testing
output contributorRoleId string = contributorRole.id
output contributorRoleDefinitionId string = contributorRole.roleDefinitionId
output ownerRoleId string = ownerRole.id
output ownerRoleDefinitionId string = ownerRole.roleDefinitionId
output readerRoleId string = readerRole.id
output readerRoleDefinitionId string = readerRole.roleDefinitionId
