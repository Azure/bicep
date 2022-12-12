targetScope = 'managementGroup'

param ownerPrincipalId string
//@[line02->line11]     "ownerPrincipalId": {
//@[line02->line12]       "type": "string"
//@[line02->line13]     },

param contributorPrincipals array
//@[line04->line14]     "contributorPrincipals": {
//@[line04->line15]       "type": "array"
//@[line04->line16]     },
param readerPrincipals array
//@[line05->line17]     "readerPrincipals": {
//@[line05->line18]       "type": "array"
//@[line05->line19]     }

resource owner 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
//@[line07->line22]     {
//@[line07->line23]       "type": "Microsoft.Authorization/roleAssignments",
//@[line07->line24]       "apiVersion": "2020-04-01-preview",
//@[line07->line25]       "name": "[guid('owner', parameters('ownerPrincipalId'))]",
//@[line07->line30]     },
  name: guid('owner', ownerPrincipalId)
  properties: {
//@[line09->line26]       "properties": {
//@[line09->line29]       }
    principalId: ownerPrincipalId
//@[line10->line27]         "principalId": "[parameters('ownerPrincipalId')]",
    roleDefinitionId: '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
//@[line11->line28]         "roleDefinitionId": "8e3af657-a8ff-443c-a75c-2fe8c4bcb635"
  }
}

resource contributors 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for contributor in contributorPrincipals: {
//@[line15->line31]     {
//@[line15->line32]       "copy": {
//@[line15->line33]         "name": "contributors",
//@[line15->line34]         "count": "[length(parameters('contributorPrincipals'))]"
//@[line15->line35]       },
//@[line15->line36]       "type": "Microsoft.Authorization/roleAssignments",
//@[line15->line37]       "apiVersion": "2020-04-01-preview",
//@[line15->line38]       "name": "[guid('contributor', parameters('contributorPrincipals')[copyIndex()])]",
//@[line15->line43]       "dependsOn": [
//@[line15->line44]         "[extensionResourceId(managementGroup().id, 'Microsoft.Authorization/roleAssignments', guid('owner', parameters('ownerPrincipalId')))]"
//@[line15->line45]       ]
//@[line15->line46]     },
  name: guid('contributor', contributor)
  properties: {
//@[line17->line39]       "properties": {
//@[line17->line42]       },
    principalId: contributor
//@[line18->line40]         "principalId": "[parameters('contributorPrincipals')[copyIndex()]]",
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[line19->line41]         "roleDefinitionId": "b24988ac-6180-42a0-ab88-20f7382dd24c"
  }
  dependsOn: [
    owner
  ]
}]

resource readers 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for reader in readerPrincipals: {
//@[line26->line47]     {
//@[line26->line48]       "copy": {
//@[line26->line49]         "name": "readers",
//@[line26->line50]         "count": "[length(parameters('readerPrincipals'))]"
//@[line26->line51]       },
//@[line26->line52]       "type": "Microsoft.Authorization/roleAssignments",
//@[line26->line53]       "apiVersion": "2020-04-01-preview",
//@[line26->line54]       "name": "[guid('reader', parameters('readerPrincipals')[copyIndex()])]",
//@[line26->line59]       "dependsOn": [
//@[line26->line60]         "[extensionResourceId(managementGroup().id, 'Microsoft.Authorization/roleAssignments', guid('contributor', parameters('contributorPrincipals')[0]))]",
//@[line26->line61]         "[extensionResourceId(managementGroup().id, 'Microsoft.Authorization/roleAssignments', guid('owner', parameters('ownerPrincipalId')))]"
//@[line26->line62]       ]
//@[line26->line63]     },
  name: guid('reader', reader)
  properties: {
//@[line28->line55]       "properties": {
//@[line28->line58]       },
    principalId: reader
//@[line29->line56]         "principalId": "[parameters('readerPrincipals')[copyIndex()]]",
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[line30->line57]         "roleDefinitionId": "b24988ac-6180-42a0-ab88-20f7382dd24c"
  }
  dependsOn: [
    owner
    contributors[0]
  ]
}]

resource single_mg 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[line38->line64]     {
//@[line38->line65]       "type": "Microsoft.Management/managementGroups",
//@[line38->line66]       "apiVersion": "2020-05-01",
//@[line38->line67]       "scope": "/",
//@[line38->line68]       "name": "one-mg"
//@[line38->line69]     },
  scope: tenant()
  name: 'one-mg'
}

// Blueprints are read-only at tenant Scope, but it's a convenient example to use to validate this.
resource tenant_blueprint 'Microsoft.Blueprint/blueprints@2018-11-01-preview' = {
//@[line44->line70]     {
//@[line44->line71]       "type": "Microsoft.Blueprint/blueprints",
//@[line44->line72]       "apiVersion": "2018-11-01-preview",
//@[line44->line73]       "scope": "/",
//@[line44->line74]       "name": "tenant-blueprint",
//@[line44->line76]     },
  name: 'tenant-blueprint'
  properties: {}
//@[line46->line75]       "properties": {}
  scope: tenant()
}

resource mg_blueprint 'Microsoft.Blueprint/blueprints@2018-11-01-preview' = {
//@[line50->line77]     {
//@[line50->line78]       "type": "Microsoft.Blueprint/blueprints",
//@[line50->line79]       "apiVersion": "2018-11-01-preview",
//@[line50->line80]       "name": "mg-blueprint",
//@[line50->line82]     }
  name: 'mg-blueprint'
  properties: {}
//@[line52->line81]       "properties": {}
}

