targetScope = 'subscription'

param ownerPrincipalId string
//@[11:13]     "ownerPrincipalId": {

param contributorPrincipals array
//@[14:16]     "contributorPrincipals": {
param readerPrincipals array
//@[17:19]     "readerPrincipals": {

resource owner 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
//@[22:30]       "type": "Microsoft.Authorization/roleAssignments",
  name: guid('owner', ownerPrincipalId)
  properties: {
    principalId: ownerPrincipalId
    roleDefinitionId: '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
  }
}

resource contributors 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for contributor in contributorPrincipals: {
//@[31:46]       "copy": {
  name: guid('contributor', contributor)
  properties: {
    principalId: contributor
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
  }
  dependsOn: [
    owner
  ]
}]

resource readers 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for reader in readerPrincipals: {
//@[47:63]       "copy": {
  name: guid('reader', reader)
  properties: {
    principalId: reader
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
  }
  dependsOn: [
    owner
    contributors[0]
  ]
}]

