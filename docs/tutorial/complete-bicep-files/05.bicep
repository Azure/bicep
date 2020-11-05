targetScope = 'subscription'

module stg './storage.bicep' = {
  name: 'storageDeploy'
  scope: resourceGroup('brittle-hollow') // this will target another resource group in the same subscription
  params: {
    namePrefix: 'contoso'
  }
}

var objectId = 'cf024e4c-f790-45eb-a992-5218c39bde1a' // change this AAD object ID. This is specific to the microsoft tenant
var contributor = 'b24988ac-6180-42a0-ab88-20f7382dd24c'
resource rbac 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(subscription().id, objectId, contributor)
  properties: {
    roleDefinitionId: contributor
    principalId: objectId
  }
}

output storageName string = stg.outputs.computedStorageName