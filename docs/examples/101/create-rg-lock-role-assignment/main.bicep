targetScope = 'subscription'

param rgName string
param rgLocation string
param principalId string
param roleDefinitionId string = 'b24988ac-6180-42a0-ab88-20f7382dd24c' // default is contributor
param roleAssignmentName string = guid(principalId, roleDefinitionId, rgName)

resource newRg 'Microsoft.Resources/resourceGroups@2019-10-01' = {
  name: rgName
  location: rgLocation
  properties: {}
}

module applyLock './applylock.bicep' = {
  name: 'applyLock'
  scope: resourceGroup(newRg.name)
  params: {
    principalId: principalId
    roleDefinitionId: roleDefinitionId
    roleAssignmentName: roleAssignmentName
  }
}
