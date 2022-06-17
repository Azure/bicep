targetScope = 'managementGroup'

param ownerPrincipalId string
//@[12:14]     "ownerPrincipalId": {

param contributorPrincipals array
//@[15:17]     "contributorPrincipals": {
param readerPrincipals array
//@[18:20]     "readerPrincipals": {

resource owner 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
//@[23:31]       "type": "Microsoft.Authorization/roleAssignments",
  name: guid('owner', ownerPrincipalId)
  properties: {
//@[27:30]       "properties": {
    principalId: ownerPrincipalId
//@[28:28]         "principalId": "[parameters('ownerPrincipalId')]",
    roleDefinitionId: '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
//@[29:29]         "roleDefinitionId": "8e3af657-a8ff-443c-a75c-2fe8c4bcb635"
  }
}

resource contributors 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for contributor in contributorPrincipals: {
//@[32:47]       "copy": {
  name: guid('contributor', contributor)
  properties: {
//@[40:43]       "properties": {
    principalId: contributor
//@[41:41]         "principalId": "[parameters('contributorPrincipals')[copyIndex()]]",
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[42:42]         "roleDefinitionId": "b24988ac-6180-42a0-ab88-20f7382dd24c"
  }
  dependsOn: [
    owner
  ]
}]

resource readers 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for reader in readerPrincipals: {
//@[48:64]       "copy": {
  name: guid('reader', reader)
  properties: {
//@[56:59]       "properties": {
    principalId: reader
//@[57:57]         "principalId": "[parameters('readerPrincipals')[copyIndex()]]",
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[58:58]         "roleDefinitionId": "b24988ac-6180-42a0-ab88-20f7382dd24c"
  }
  dependsOn: [
    owner
    contributors[0]
  ]
}]

resource single_mg 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[65:70]       "type": "Microsoft.Management/managementGroups",
  scope: tenant()
  name: 'one-mg'
}

// Blueprints are read-only at tenant Scope, but it's a convenient example to use to validate this.
resource tenant_blueprint 'Microsoft.Blueprint/blueprints@2018-11-01-preview' = {
//@[71:77]       "type": "Microsoft.Blueprint/blueprints",
  name: 'tenant-blueprint'
  properties: {}
//@[76:76]       "properties": {}
  scope: tenant()
}

resource mg_blueprint 'Microsoft.Blueprint/blueprints@2018-11-01-preview' = {
//@[78:83]       "type": "Microsoft.Blueprint/blueprints",
  name: 'mg-blueprint'
  properties: {}
//@[82:82]       "properties": {}
}

