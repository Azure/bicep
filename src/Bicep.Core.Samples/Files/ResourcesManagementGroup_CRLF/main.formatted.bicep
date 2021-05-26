targetScope = 'managementGroup'

param ownerPrincipalId string

param contributorPrincipals array
param readerPrincipals array

resource owner 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid('owner', ownerPrincipalId)
  properties: {
    principalId: ownerPrincipalId
    roleDefinitionId: '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
  }
}

resource contributors 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for contributor in contributorPrincipals: {
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
  scope: tenant()
  name: 'one-mg'
}

// Blueprints are read-only at tenant Scope, but it's a convenient example to use to validate this.
resource tenant_blueprint 'Microsoft.Blueprint/blueprints@2018-11-01-preview' = {
  name: 'tenant-blueprint'
  properties: {}
  scope: tenant()
}

resource mg_blueprint 'Microsoft.Blueprint/blueprints@2018-11-01-preview' = {
  name: 'mg-blueprint'
  properties: {}
}
