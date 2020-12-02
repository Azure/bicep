targetScope = 'subscription'

param principalId string
var roleName = 'Bicep Custom Role demo'

resource definition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' = {
  name: guid(roleName)
  properties: {
    roleName: roleName
    description: 'Custom role create with bicep'
    permissions: [
      {
        actions: [
          '*/read'
        ]
      }
    ]
    assignableScopes: [
      subscription().id
    ]
  }
}

resource assignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(roleName, principalId, subscription().subscriptionId)
  properties: {
    roleDefinitionId: definition.id
    principalId: principalId
  }
}
