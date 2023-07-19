targetScope = 'subscription'

param ownerPrincipalId string
//@[06:022) Parameter ownerPrincipalId. Type: string. Declaration start char: 0, length: 29

param contributorPrincipals array
//@[06:027) Parameter contributorPrincipals. Type: array. Declaration start char: 0, length: 33
param readerPrincipals array
//@[06:022) Parameter readerPrincipals. Type: array. Declaration start char: 0, length: 28

resource owner 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
//@[09:014) Resource owner. Type: Microsoft.Authorization/roleAssignments@2020-04-01-preview. Declaration start char: 0, length: 242
  name: guid('owner', ownerPrincipalId)
  properties: {
    principalId: ownerPrincipalId
    roleDefinitionId: '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
  }
}

resource contributors 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for contributor in contributorPrincipals: {
//@[90:101) Local contributor. Type: any. Declaration start char: 90, length: 11
//@[09:021) Resource contributors. Type: Microsoft.Authorization/roleAssignments@2020-04-01-preview[]. Declaration start char: 0, length: 321
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
//@[85:091) Local reader. Type: any. Declaration start char: 85, length: 6
//@[09:016) Resource readers. Type: Microsoft.Authorization/roleAssignments@2020-04-01-preview[]. Declaration start char: 0, length: 312
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

