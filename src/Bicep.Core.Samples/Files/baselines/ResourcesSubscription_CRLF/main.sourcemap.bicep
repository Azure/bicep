targetScope = 'subscription'

param ownerPrincipalId string
//@    "ownerPrincipalId": {
//@      "type": "string"
//@    },

param contributorPrincipals array
//@    "contributorPrincipals": {
//@      "type": "array"
//@    },
param readerPrincipals array
//@    "readerPrincipals": {
//@      "type": "array"
//@    }

resource owner 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
//@    {
//@      "type": "Microsoft.Authorization/roleAssignments",
//@      "apiVersion": "2020-04-01-preview",
//@      "name": "[guid('owner', parameters('ownerPrincipalId'))]",
//@    },
  name: guid('owner', ownerPrincipalId)
  properties: {
//@      "properties": {
//@      }
    principalId: ownerPrincipalId
//@        "principalId": "[parameters('ownerPrincipalId')]",
    roleDefinitionId: '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
//@        "roleDefinitionId": "8e3af657-a8ff-443c-a75c-2fe8c4bcb635"
  }
}

resource contributors 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for contributor in contributorPrincipals: {
//@    {
//@      "copy": {
//@        "name": "contributors",
//@        "count": "[length(parameters('contributorPrincipals'))]"
//@      },
//@      "type": "Microsoft.Authorization/roleAssignments",
//@      "apiVersion": "2020-04-01-preview",
//@      "name": "[guid('contributor', parameters('contributorPrincipals')[copyIndex()])]",
//@      "dependsOn": [
//@        "[subscriptionResourceId('Microsoft.Authorization/roleAssignments', guid('owner', parameters('ownerPrincipalId')))]"
//@      ]
//@    },
  name: guid('contributor', contributor)
  properties: {
//@      "properties": {
//@      },
    principalId: contributor
//@        "principalId": "[parameters('contributorPrincipals')[copyIndex()]]",
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@        "roleDefinitionId": "b24988ac-6180-42a0-ab88-20f7382dd24c"
  }
  dependsOn: [
    owner
  ]
}]

resource readers 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for reader in readerPrincipals: {
//@    {
//@      "copy": {
//@        "name": "readers",
//@        "count": "[length(parameters('readerPrincipals'))]"
//@      },
//@      "type": "Microsoft.Authorization/roleAssignments",
//@      "apiVersion": "2020-04-01-preview",
//@      "name": "[guid('reader', parameters('readerPrincipals')[copyIndex()])]",
//@      "dependsOn": [
//@        "[subscriptionResourceId('Microsoft.Authorization/roleAssignments', guid('contributor', parameters('contributorPrincipals')[0]))]",
//@        "[subscriptionResourceId('Microsoft.Authorization/roleAssignments', guid('owner', parameters('ownerPrincipalId')))]"
//@      ]
//@    }
  name: guid('reader', reader)
  properties: {
//@      "properties": {
//@      },
    principalId: reader
//@        "principalId": "[parameters('readerPrincipals')[copyIndex()]]",
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@        "roleDefinitionId": "b24988ac-6180-42a0-ab88-20f7382dd24c"
  }
  dependsOn: [
    owner
    contributors[0]
  ]
}]

