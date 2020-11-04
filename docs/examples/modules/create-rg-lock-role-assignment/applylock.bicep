targetScope = 'resourceGroup'

param principalId string
param roleDefinitionId string
param roleAssignmentName string

resource lockResource 'Microsoft.Authorization/locks@2016-09-01' = {
  name: 'DontDelete'
  properties: {
    level: 'CanNotDelete'
    notes: 'Prevent deletion of the resourceGroup'
  }
}

resource assignmentResource 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(roleAssignmentName)
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roleDefinitionId)
    principalId: principalId
  }
}