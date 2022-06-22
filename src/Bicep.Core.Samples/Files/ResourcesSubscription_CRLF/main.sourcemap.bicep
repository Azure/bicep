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
//@[26:29]       "properties": {
    principalId: ownerPrincipalId
//@[27:27]         "principalId": "[parameters('ownerPrincipalId')]",
    roleDefinitionId: '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
//@[28:28]         "roleDefinitionId": "8e3af657-a8ff-443c-a75c-2fe8c4bcb635"
  }
}

resource contributors 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for contributor in contributorPrincipals: {
//@[31:46]       "copy": {
  name: guid('contributor', contributor)
  properties: {
//@[39:42]       "properties": {
    principalId: contributor
//@[40:40]         "principalId": "[parameters('contributorPrincipals')[copyIndex()]]",
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[41:41]         "roleDefinitionId": "b24988ac-6180-42a0-ab88-20f7382dd24c"
  }
  dependsOn: [
    owner
  ]
}]

resource readers 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for reader in readerPrincipals: {
//@[47:63]       "copy": {
  name: guid('reader', reader)
  properties: {
//@[55:58]       "properties": {
    principalId: reader
//@[56:56]         "principalId": "[parameters('readerPrincipals')[copyIndex()]]",
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[57:57]         "roleDefinitionId": "b24988ac-6180-42a0-ab88-20f7382dd24c"
  }
  dependsOn: [
    owner
    contributors[0]
  ]
}]

