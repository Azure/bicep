targetScope = 'managementGroup'

param ownerPrincipalId string
//@[6:22) Parameter ownerPrincipalId. Type: string. Declaration start char: 0, length: 29

param contributorPrincipals array
//@[6:27) Parameter contributorPrincipals. Type: array. Declaration start char: 0, length: 33
param readerPrincipals array
//@[6:22) Parameter readerPrincipals. Type: array. Declaration start char: 0, length: 28

resource owner 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
//@[9:14) Resource owner. Type: Microsoft.Authorization/roleAssignments@2020-04-01-preview. Declaration start char: 0, length: 242
  name: guid('owner', ownerPrincipalId)
  properties: {
    principalId: ownerPrincipalId
    roleDefinitionId: '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
  }
}

resource contributors 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for contributor in contributorPrincipals: {
//@[90:101) Local contributor. Type: any. Declaration start char: 90, length: 11
//@[9:21) Resource contributors. Type: Microsoft.Authorization/roleAssignments@2020-04-01-preview[]. Declaration start char: 0, length: 321
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
//@[85:91) Local reader. Type: any. Declaration start char: 85, length: 6
//@[9:16) Resource readers. Type: Microsoft.Authorization/roleAssignments@2020-04-01-preview[]. Declaration start char: 0, length: 312
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

resource single_mg 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[9:18) Resource single_mg. Type: Microsoft.Management/managementGroups@2020-05-01. Declaration start char: 0, length: 113
  scope: tenant()
  name: 'one-mg'
}

// Blueprints are read-only at tenant Scope, but it's a convenient example to use to validate this.
resource tenant_blueprint 'Microsoft.Blueprint/blueprints@2018-11-01-preview' = {
//@[9:25) Resource tenant_blueprint. Type: Microsoft.Blueprint/blueprints@2018-11-01-preview. Declaration start char: 0, length: 149
  name: 'tenant-blueprint'
  properties: {}
  scope: tenant()
}

resource mg_blueprint 'Microsoft.Blueprint/blueprints@2018-11-01-preview' = {
//@[9:21) Resource mg_blueprint. Type: Microsoft.Blueprint/blueprints@2018-11-01-preview. Declaration start char: 0, length: 122
  name: 'mg-blueprint'
  properties: {}
}

