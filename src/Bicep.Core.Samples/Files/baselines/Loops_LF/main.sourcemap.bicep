param name string
//@    "name": {
//@      "type": "string"
//@    },
param accounts array
//@    "accounts": {
//@      "type": "array"
//@    },
param index int
//@    "index": {
//@      "type": "int"
//@    }

// single resource
resource singleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@    {
//@      "type": "Microsoft.Storage/storageAccounts",
//@      "apiVersion": "2019-06-01",
//@      "name": "[format('{0}single-resource-name', parameters('name'))]",
//@    },
  name: '${name}single-resource-name'
  location: resourceGroup().location
//@      "location": "[resourceGroup().location]",
  kind: 'StorageV2'
//@      "kind": "StorageV2",
  sku: {
//@      "sku": {
//@      }
    name: 'Standard_LRS'
//@        "name": "Standard_LRS"
  }
}

// extension of single resource
resource singleResourceExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@    {
//@      "type": "Microsoft.Authorization/locks",
//@      "apiVersion": "2016-09-01",
//@      "scope": "[resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name')))]",
//@      "name": "single-resource-lock",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name')))]"
//@      ]
//@    },
  scope: singleResource
  name: 'single-resource-lock'
  properties: {
//@      "properties": {
//@      },
    level: 'CanNotDelete'
//@        "level": "CanNotDelete"
  }
}

// single resource cascade extension
resource singleResourceCascadeExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@    {
//@      "type": "Microsoft.Authorization/locks",
//@      "apiVersion": "2016-09-01",
//@      "scope": "[extensionResourceId(resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name'))), 'Microsoft.Authorization/locks', 'single-resource-lock')]",
//@      "name": "single-resource-cascade-extension",
//@      "dependsOn": [
//@        "[extensionResourceId(resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name'))), 'Microsoft.Authorization/locks', 'single-resource-lock')]"
//@      ]
//@    },
  scope: singleResourceExtension
  name: 'single-resource-cascade-extension'
  properties: {
//@      "properties": {
//@      },
    level: 'CanNotDelete'
//@        "level": "CanNotDelete"
  }
}

// resource collection
@batchSize(42)
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@    {
//@      "copy": {
//@        "name": "storageAccounts",
//@        "count": "[length(parameters('accounts'))]",
//@        "mode": "serial",
//@        "batchSize": 42
//@      },
//@      "type": "Microsoft.Storage/storageAccounts",
//@      "apiVersion": "2019-06-01",
//@      "name": "[format('{0}-collection-{1}', parameters('name'), parameters('accounts')[copyIndex()].name)]",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name')))]"
//@      ]
//@    },
  name: '${name}-collection-${account.name}'
  location: account.location
//@      "location": "[parameters('accounts')[copyIndex()].location]",
  kind: 'StorageV2'
//@      "kind": "StorageV2",
  sku: {
//@      "sku": {
//@      },
    name: 'Standard_LRS'
//@        "name": "Standard_LRS"
  }
  dependsOn: [
    singleResource
  ]
}]

// extension of a single resource in a collection
resource extendSingleResourceInCollection 'Microsoft.Authorization/locks@2016-09-01' = {
//@    {
//@      "type": "Microsoft.Authorization/locks",
//@      "apiVersion": "2016-09-01",
//@      "scope": "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[mod(parameters('index'), 2)].name))]",
//@      "name": "one-resource-collection-item-lock",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[mod(parameters('index'), 2)].name))]"
//@      ]
//@    },
  name: 'one-resource-collection-item-lock'
  properties: {
//@      "properties": {
//@      },
    level: 'ReadOnly'
//@        "level": "ReadOnly"
  }
  scope: storageAccounts[index % 2]
}

// collection of extensions
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@    {
//@      "copy": {
//@        "name": "extensionCollection",
//@        "count": "[length(range(0, 1))]"
//@      },
//@      "type": "Microsoft.Authorization/locks",
//@      "apiVersion": "2016-09-01",
//@      "scope": "[resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name')))]",
//@      "name": "[format('lock-{0}', range(0, 1)[copyIndex()])]",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name')))]"
//@      ]
//@    },
  name: 'lock-${i}'
  properties: {
//@      "properties": {
//@      },
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@        "level": "[if(equals(range(0, 1)[copyIndex()], 0), 'CanNotDelete', 'ReadOnly')]"
  }
  scope: singleResource
}]

// cascade extend the extension
@batchSize(1)
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@    {
//@      "copy": {
//@        "name": "lockTheLocks",
//@        "count": "[length(range(0, 1))]",
//@        "mode": "serial",
//@        "batchSize": 1
//@      },
//@      "type": "Microsoft.Authorization/locks",
//@      "apiVersion": "2016-09-01",
//@      "scope": "[extensionResourceId(resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name'))), 'Microsoft.Authorization/locks', format('lock-{0}', range(0, 1)[range(0, 1)[copyIndex()]]))]",
//@      "name": "[format('lock-the-lock-{0}', range(0, 1)[copyIndex()])]",
//@      "dependsOn": [
//@        "[extensionResourceId(resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name'))), 'Microsoft.Authorization/locks', format('lock-{0}', range(0, 1)[range(0, 1)[copyIndex()]]))]"
//@      ]
//@    },
  name: 'lock-the-lock-${i}'
  properties: {
//@      "properties": {
//@      },
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@        "level": "[if(equals(range(0, 1)[copyIndex()], 0), 'CanNotDelete', 'ReadOnly')]"
  }
  scope: extensionCollection[i]
}]

// special case property access
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
//@    "indexedCollectionBlobEndpoint": {
//@      "type": "string",
//@      "value": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name)), '2019-06-01').primaryEndpoints.blob]"
//@    },
output indexedCollectionName string = storageAccounts[index].name
//@    "indexedCollectionName": {
//@      "type": "string",
//@      "value": "[format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name)]"
//@    },
output indexedCollectionId string = storageAccounts[index].id
//@    "indexedCollectionId": {
//@      "type": "string",
//@      "value": "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name))]"
//@    },
output indexedCollectionType string = storageAccounts[index].type
//@    "indexedCollectionType": {
//@      "type": "string",
//@      "value": "Microsoft.Storage/storageAccounts"
//@    },
output indexedCollectionVersion string = storageAccounts[index].apiVersion
//@    "indexedCollectionVersion": {
//@      "type": "string",
//@      "value": "2019-06-01"
//@    },

// general case property access
output indexedCollectionIdentity object = storageAccounts[index].identity
//@    "indexedCollectionIdentity": {
//@      "type": "object",
//@      "value": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name)), '2019-06-01', 'full').identity]"
//@    },

// indexed access of two properties
output indexedEndpointPair object = {
//@    "indexedEndpointPair": {
//@      "type": "object",
//@      "value": {
//@      }
//@    },
  primary: storageAccounts[index].properties.primaryEndpoints.blob
//@        "primary": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name)), '2019-06-01').primaryEndpoints.blob]",
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
//@        "secondary": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[add(parameters('index'), 1)].name)), '2019-06-01').secondaryEndpoints.blob]"
}

// nested indexer?
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier
//@    "indexViaReference": {
//@      "type": "string",
//@      "value": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[int(reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name)), '2019-06-01').creationTime)].name)), '2019-06-01').accessTier]"
//@    },

// dependency on a resource collection
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@    {
//@      "copy": {
//@        "name": "storageAccounts2",
//@        "count": "[length(parameters('accounts'))]"
//@      },
//@      "type": "Microsoft.Storage/storageAccounts",
//@      "apiVersion": "2019-06-01",
//@      "name": "[format('{0}-collection-{1}', parameters('name'), parameters('accounts')[copyIndex()].name)]",
//@      "dependsOn": [
//@        "storageAccounts"
//@      ]
//@    },
  name: '${name}-collection-${account.name}'
  location: account.location
//@      "location": "[parameters('accounts')[copyIndex()].location]",
  kind: 'StorageV2'
//@      "kind": "StorageV2",
  sku: {
//@      "sku": {
//@      },
    name: 'Standard_LRS'
//@        "name": "Standard_LRS"
  }
  dependsOn: [
    storageAccounts
  ]
}]

// one-to-one paired dependencies
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@    {
//@      "copy": {
//@        "name": "firstSet",
//@        "count": "[length(range(0, length(parameters('accounts'))))]"
//@      },
//@      "type": "Microsoft.Storage/storageAccounts",
//@      "apiVersion": "2019-06-01",
//@      "name": "[format('{0}-set1-{1}', parameters('name'), range(0, length(parameters('accounts')))[copyIndex()])]",
//@    },
  name: '${name}-set1-${i}'
  location: resourceGroup().location
//@      "location": "[resourceGroup().location]",
  kind: 'StorageV2'
//@      "kind": "StorageV2",
  sku: {
//@      "sku": {
//@      }
    name: 'Standard_LRS'
//@        "name": "Standard_LRS"
  }
}]

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@    {
//@      "copy": {
//@        "name": "secondSet",
//@        "count": "[length(range(0, length(parameters('accounts'))))]"
//@      },
//@      "type": "Microsoft.Storage/storageAccounts",
//@      "apiVersion": "2019-06-01",
//@      "name": "[format('{0}-set2-{1}', parameters('name'), range(0, length(parameters('accounts')))[copyIndex()])]",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-set1-{1}', parameters('name'), range(0, length(parameters('accounts')))[range(0, length(parameters('accounts')))[copyIndex()]]))]"
//@      ]
//@    },
  name: '${name}-set2-${i}'
  location: resourceGroup().location
//@      "location": "[resourceGroup().location]",
  kind: 'StorageV2'
//@      "kind": "StorageV2",
  sku: {
//@      "sku": {
//@      },
    name: 'Standard_LRS'
//@        "name": "Standard_LRS"
  }
  dependsOn: [
    firstSet[i]
  ]
}]

// depending on collection and one resource in the collection optimizes the latter part away
resource anotherSingleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@    {
//@      "type": "Microsoft.Storage/storageAccounts",
//@      "apiVersion": "2019-06-01",
//@      "name": "[format('{0}single-resource-name', parameters('name'))]",
//@      "dependsOn": [
//@        "secondSet"
//@      ]
//@    },
  name: '${name}single-resource-name'
  location: resourceGroup().location
//@      "location": "[resourceGroup().location]",
  kind: 'StorageV2'
//@      "kind": "StorageV2",
  sku: {
//@      "sku": {
//@      },
    name: 'Standard_LRS'
//@        "name": "Standard_LRS"
  }
  dependsOn: [
    secondSet
    secondSet[0]
  ]
}

// vnets
var vnetConfigurations = [
//@    "vnetConfigurations": [
//@    ],
  {
//@      {
//@      },
    name: 'one'
//@        "name": "one",
    location: resourceGroup().location
//@        "location": "[resourceGroup().location]"
  }
  {
//@      {
//@      }
    name: 'two'
//@        "name": "two",
    location: 'westus'
//@        "location": "westus"
  }
]

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for vnetConfig in vnetConfigurations: {
//@    {
//@      "copy": {
//@        "name": "vnets",
//@        "count": "[length(variables('vnetConfigurations'))]"
//@      },
//@      "type": "Microsoft.Network/virtualNetworks",
//@      "apiVersion": "2020-06-01",
//@      "name": "[variables('vnetConfigurations')[copyIndex()].name]",
//@    },
  name: vnetConfig.name
  location: vnetConfig.location
//@      "location": "[variables('vnetConfigurations')[copyIndex()].location]"
}]

// implicit dependency on single resource from a resource collection
resource implicitDependencyOnSingleResourceByIndex 'Microsoft.Network/dnsZones@2018-05-01' = {
//@    {
//@      "type": "Microsoft.Network/dnsZones",
//@      "apiVersion": "2018-05-01",
//@      "name": "test",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[add(parameters('index'), 1)].name)]"
//@      ]
//@    },
  name: 'test'
  location: 'global'
//@      "location": "global",
  properties: {
//@      "properties": {
//@      },
    resolutionVirtualNetworks: [
//@        "resolutionVirtualNetworks": [
//@        ]
      {
//@          {
//@          }
        id: vnets[index+1].id
//@            "id": "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[add(parameters('index'), 1)].name)]"
      }
    ]
  }
}

// implicit and explicit dependency combined
resource combinedDependencies 'Microsoft.Network/dnsZones@2018-05-01' = {
//@    {
//@      "type": "Microsoft.Network/dnsZones",
//@      "apiVersion": "2018-05-01",
//@      "name": "test2",
//@      "dependsOn": [
//@        "vnets"
//@      ]
//@    },
  name: 'test2'
  location: 'global'
//@      "location": "global",
  properties: {
//@      "properties": {
//@      },
    resolutionVirtualNetworks: [
//@        "resolutionVirtualNetworks": [
//@        ]
      {
//@          {
//@          },
        id: vnets[index-1].id
//@            "id": "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[sub(parameters('index'), 1)].name)]"
      }
      {
//@          {
//@          }
        id: vnets[index * 2].id
//@            "id": "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[mul(parameters('index'), 2)].name)]"
      }
    ]
  }
  dependsOn: [
    vnets
  ]
}

// single module
module singleModule 'passthrough.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "14375999048727010492"
//@            }
//@          },
//@          "parameters": {
//@            "myInput": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "myOutput": {
//@              "type": "string",
//@              "value": "[parameters('myInput')]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'test'
//@      "name": "test",
  params: {
//@        "parameters": {
//@        },
    myInput: 'hello'
//@          "myInput": {
//@            "value": "hello"
//@          }
  }
}

var moduleSetup = [
//@    "moduleSetup": [
//@    ],
  'one'
//@      "one",
  'two'
//@      "two",
  'three'
//@      "three"
]

// module collection plus explicit dependency on single module
@sys.batchSize(3)
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@    {
//@      "copy": {
//@        "name": "moduleCollectionWithSingleDependency",
//@        "count": "[length(variables('moduleSetup'))]",
//@        "mode": "serial",
//@        "batchSize": 3
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "14375999048727010492"
//@            }
//@          },
//@          "parameters": {
//@            "myInput": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "myOutput": {
//@              "type": "string",
//@              "value": "[parameters('myInput')]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Resources/deployments', 'test')]",
//@        "[resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name')))]"
//@      ]
//@    },
  name: moduleName
//@      "name": "[variables('moduleSetup')[copyIndex()]]",
  params: {
//@        "parameters": {
//@        },
    myInput: 'in-${moduleName}'
//@          "myInput": {
//@            "value": "[format('in-{0}', variables('moduleSetup')[copyIndex()])]"
//@          }
  }
  dependsOn: [
    singleModule
    singleResource
  ]
}]

// another module collection with dependency on another module collection
module moduleCollectionWithCollectionDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@    {
//@      "copy": {
//@        "name": "moduleCollectionWithCollectionDependencies",
//@        "count": "[length(variables('moduleSetup'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "14375999048727010492"
//@            }
//@          },
//@          "parameters": {
//@            "myInput": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "myOutput": {
//@              "type": "string",
//@              "value": "[parameters('myInput')]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "moduleCollectionWithSingleDependency",
//@        "storageAccounts"
//@      ]
//@    },
  name: moduleName
//@      "name": "[variables('moduleSetup')[copyIndex()]]",
  params: {
//@        "parameters": {
//@        },
    myInput: 'in-${moduleName}'
//@          "myInput": {
//@            "value": "[format('in-{0}', variables('moduleSetup')[copyIndex()])]"
//@          }
  }
  dependsOn: [
    storageAccounts
    moduleCollectionWithSingleDependency
  ]
}]

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "14375999048727010492"
//@            }
//@          },
//@          "parameters": {
//@            "myInput": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "myOutput": {
//@              "type": "string",
//@              "value": "[parameters('myInput')]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Resources/deployments', variables('moduleSetup')[parameters('index')])]",
//@        "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[mul(parameters('index'), 3)].name))]",
//@        "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[sub(parameters('index'), 10)].name))]"
//@      ]
//@    },
  name: 'hello'
//@      "name": "hello",
  params: {
//@        "parameters": {
//@        },
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
//@          "myInput": {
//@            "value": "[concat(reference(resourceId('Microsoft.Resources/deployments', variables('moduleSetup')[parameters('index')]), '2025-04-01').outputs.myOutput.value, reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[mul(parameters('index'), 3)].name)), '2019-06-01').accessTier)]"
//@          }
  }
  dependsOn: [
    storageAccounts2[index - 10]
  ]
}

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@    {
//@      "copy": {
//@        "name": "moduleCollectionWithIndexedDependencies",
//@        "count": "[length(variables('moduleSetup'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "14375999048727010492"
//@            }
//@          },
//@          "parameters": {
//@            "myInput": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "myOutput": {
//@              "type": "string",
//@              "value": "[parameters('myInput')]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Resources/deployments', variables('moduleSetup')[parameters('index')])]",
//@        "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[mul(parameters('index'), 3)].name))]",
//@        "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[sub(parameters('index'), 9)].name))]"
//@      ]
//@    },
  name: moduleName
//@      "name": "[variables('moduleSetup')[copyIndex()]]",
  params: {
//@        "parameters": {
//@        },
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName}'
//@          "myInput": {
//@            "value": "[format('{0} - {1} - {2}', reference(resourceId('Microsoft.Resources/deployments', variables('moduleSetup')[parameters('index')]), '2025-04-01').outputs.myOutput.value, reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[mul(parameters('index'), 3)].name)), '2019-06-01').accessTier, variables('moduleSetup')[copyIndex()])]"
//@          }
  }
  dependsOn: [
    storageAccounts2[index - 9]
  ]
}]

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
//@    "indexedModulesName": {
//@      "type": "string",
//@      "value": "[variables('moduleSetup')[parameters('index')]]"
//@    },
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput
//@    "indexedModuleOutput": {
//@      "type": "string",
//@      "value": "[reference(resourceId('Microsoft.Resources/deployments', variables('moduleSetup')[mul(parameters('index'), 1)]), '2025-04-01').outputs.myOutput.value]"
//@    },

// resource collection
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for account in accounts: {
  name: '${name}-existing-${account.name}'
}]

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
//@    "existingIndexedResourceName": {
//@      "type": "string",
//@      "value": "[format('{0}-existing-{1}', parameters('name'), parameters('accounts')[mul(parameters('index'), 0)].name)]"
//@    },
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
//@    "existingIndexedResourceId": {
//@      "type": "string",
//@      "value": "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-existing-{1}', parameters('name'), parameters('accounts')[mul(parameters('index'), 1)].name))]"
//@    },
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
//@    "existingIndexedResourceType": {
//@      "type": "string",
//@      "value": "Microsoft.Storage/storageAccounts"
//@    },
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
//@    "existingIndexedResourceApiVersion": {
//@      "type": "string",
//@      "value": "2019-06-01"
//@    },
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
//@    "existingIndexedResourceLocation": {
//@      "type": "string",
//@      "value": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-existing-{1}', parameters('name'), parameters('accounts')[div(parameters('index'), 2)].name)), '2019-06-01', 'full').location]"
//@    },
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier
//@    "existingIndexedResourceAccessTier": {
//@      "type": "string",
//@      "value": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-existing-{1}', parameters('name'), parameters('accounts')[mod(parameters('index'), 3)].name)), '2019-06-01').accessTier]"
//@    },

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@    {
//@      "copy": {
//@        "name": "duplicatedNames",
//@        "count": "[length(createArray())]"
//@      },
//@      "type": "Microsoft.Network/dnsZones",
//@      "apiVersion": "2018-05-01",
//@      "name": "no loop variable",
//@    },
  name: 'no loop variable'
  location: 'eastus'
//@      "location": "eastus"
}]

// reference to a resource collection whose name expression does not reference any loop variables
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@    {
//@      "copy": {
//@        "name": "referenceToDuplicateNames",
//@        "count": "[length(createArray())]"
//@      },
//@      "type": "Microsoft.Network/dnsZones",
//@      "apiVersion": "2018-05-01",
//@      "name": "no loop variable 2",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Network/dnsZones', 'no loop variable')]"
//@      ]
//@    },
  name: 'no loop variable 2'
  location: 'eastus'
//@      "location": "eastus",
  dependsOn: [
    duplicatedNames[index]
  ]
}]

var regions = [
//@    "regions": [
//@    ]
  'eastus'
//@      "eastus",
  'westus'
//@      "westus"
]

module apim 'passthrough.bicep' = [for region in regions: {
//@    {
//@      "copy": {
//@        "name": "apim",
//@        "count": "[length(variables('regions'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "14375999048727010492"
//@            }
//@          },
//@          "parameters": {
//@            "myInput": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "myOutput": {
//@              "type": "string",
//@              "value": "[parameters('myInput')]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'apim-${region}-${name}'
//@      "name": "[format('apim-{0}-{1}', variables('regions')[copyIndex()], parameters('name'))]",
  params: {
//@        "parameters": {
//@        },
    myInput: region
//@          "myInput": {
//@            "value": "[variables('regions')[copyIndex()]]"
//@          }
  }
}]

resource propertyLoopDependencyOnModuleCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@    {
//@      "type": "Microsoft.Network/frontDoors",
//@      "apiVersion": "2020-05-01",
//@      "name": "[parameters('name')]",
//@      "dependsOn": [
//@        "apim"
//@      ]
//@    },
  name: name
  location: 'Global'
//@      "location": "Global",
  properties: {
//@      "properties": {
//@      },
    backendPools: [
//@        "backendPools": [
//@        ]
      {
//@          {
//@          }
        name: 'BackendAPIMs'
//@            "name": "BackendAPIMs",
        properties: {
//@            "properties": {
//@              "copy": [
//@              ]
//@            }
          backends: [for index in range(0, length(regions)): {
//@                {
//@                  "name": "backends",
//@                  "count": "[length(range(0, length(variables('regions'))))]",
//@                  "input": {
//@                  }
//@                }
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: apim[index].outputs.myOutput
//@                    "address": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex('backends')]], parameters('name'))), '2025-04-01').outputs.myOutput.value]",
            backendHostHeader: apim[index].outputs.myOutput
//@                    "backendHostHeader": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex('backends')]], parameters('name'))), '2025-04-01').outputs.myOutput.value]",
            httpPort: 80
//@                    "httpPort": 80,
            httpsPort: 443
//@                    "httpsPort": 443,
            priority: 1
//@                    "priority": 1,
            weight: 50
//@                    "weight": 50
          }]
        }
      }
    ]
  }
}

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(regions)): {
//@    {
//@      "copy": {
//@        "name": "indexedModuleCollectionDependency",
//@        "count": "[length(range(0, length(variables('regions'))))]"
//@      },
//@      "type": "Microsoft.Network/frontDoors",
//@      "apiVersion": "2020-05-01",
//@      "name": "[format('{0}-{1}', parameters('name'), range(0, length(variables('regions')))[copyIndex()])]",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex()]], parameters('name')))]",
//@        "[resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex()]], parameters('name')))]"
//@      ]
//@    },
  name: '${name}-${index}'
  location: 'Global'
//@      "location": "Global",
  properties: {
//@      "properties": {
//@      },
    backendPools: [
//@        "backendPools": [
//@        ]
      {
//@          {
//@          }
        name: 'BackendAPIMs'
//@            "name": "BackendAPIMs",
        properties: {
//@            "properties": {
//@            }
          backends: [
//@              "backends": [
//@              ]
            {
//@                {
//@                }
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: apim[index].outputs.myOutput
//@                  "address": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex()]], parameters('name'))), '2025-04-01').outputs.myOutput.value]",
              backendHostHeader: apim[index].outputs.myOutput
//@                  "backendHostHeader": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex()]], parameters('name'))), '2025-04-01').outputs.myOutput.value]",
              httpPort: 80
//@                  "httpPort": 80,
              httpsPort: 443
//@                  "httpsPort": 443,
              priority: 1
//@                  "priority": 1,
              weight: 50
//@                  "weight": 50
            }
          ]
        }
      }
    ]
  }
}]

resource propertyLoopDependencyOnResourceCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@    {
//@      "type": "Microsoft.Network/frontDoors",
//@      "apiVersion": "2020-05-01",
//@      "name": "[parameters('name')]",
//@      "dependsOn": [
//@        "storageAccounts"
//@      ]
//@    },
  name: name
  location: 'Global'
//@      "location": "Global",
  properties: {
//@      "properties": {
//@      },
    backendPools: [
//@        "backendPools": [
//@        ]
      {
//@          {
//@          }
        name: 'BackendAPIMs'
//@            "name": "BackendAPIMs",
        properties: {
//@            "properties": {
//@              "copy": [
//@              ]
//@            }
          backends: [for index in range(0, length(accounts)): {
//@                {
//@                  "name": "backends",
//@                  "count": "[length(range(0, length(parameters('accounts'))))]",
//@                  "input": {
//@                  }
//@                }
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@                    "address": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex('backends')]].name)), '2019-06-01').primaryEndpoints.internetEndpoints.web]",
            backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@                    "backendHostHeader": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex('backends')]].name)), '2019-06-01').primaryEndpoints.internetEndpoints.web]",
            httpPort: 80
//@                    "httpPort": 80,
            httpsPort: 443
//@                    "httpsPort": 443,
            priority: 1
//@                    "priority": 1,
            weight: 50
//@                    "weight": 50
          }]
        }
      }
    ]
  }
}

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(accounts)): {
//@    {
//@      "copy": {
//@        "name": "indexedResourceCollectionDependency",
//@        "count": "[length(range(0, length(parameters('accounts'))))]"
//@      },
//@      "type": "Microsoft.Network/frontDoors",
//@      "apiVersion": "2020-05-01",
//@      "name": "[format('{0}-{1}', parameters('name'), range(0, length(parameters('accounts')))[copyIndex()])]",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex()]].name))]",
//@        "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex()]].name))]"
//@      ]
//@    },
  name: '${name}-${index}'
  location: 'Global'
//@      "location": "Global",
  properties: {
//@      "properties": {
//@      },
    backendPools: [
//@        "backendPools": [
//@        ]
      {
//@          {
//@          }
        name: 'BackendAPIMs'
//@            "name": "BackendAPIMs",
        properties: {
//@            "properties": {
//@            }
          backends: [
//@              "backends": [
//@              ]
            {
//@                {
//@                }
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@                  "address": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex()]].name)), '2019-06-01').primaryEndpoints.internetEndpoints.web]",
              backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@                  "backendHostHeader": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex()]].name)), '2019-06-01').primaryEndpoints.internetEndpoints.web]",
              httpPort: 80
//@                  "httpPort": 80,
              httpsPort: 443
//@                  "httpsPort": 443,
              priority: 1
//@                  "priority": 1,
              weight: 50
//@                  "weight": 50
            }
          ]
        }
      }
    ]
  }
}]

resource filteredZones 'Microsoft.Network/dnsZones@2018-05-01' = [for i in range(0,10): if(i % 3 == 0) {
//@    {
//@      "copy": {
//@        "name": "filteredZones",
//@        "count": "[length(range(0, 10))]"
//@      },
//@      "condition": "[equals(mod(range(0, 10)[copyIndex()], 3), 0)]",
//@      "type": "Microsoft.Network/dnsZones",
//@      "apiVersion": "2018-05-01",
//@      "name": "[format('zone{0}', range(0, 10)[copyIndex()])]",
//@    },
  name: 'zone${i}'
  location: resourceGroup().location
//@      "location": "[resourceGroup().location]"
}]

module filteredModules 'passthrough.bicep' = [for i in range(0,6): if(i % 2 == 0) {
//@    {
//@      "copy": {
//@        "name": "filteredModules",
//@        "count": "[length(range(0, 6))]"
//@      },
//@      "condition": "[equals(mod(range(0, 6)[copyIndex()], 2), 0)]",
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "14375999048727010492"
//@            }
//@          },
//@          "parameters": {
//@            "myInput": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "myOutput": {
//@              "type": "string",
//@              "value": "[parameters('myInput')]"
//@            }
//@          }
//@        }
//@      }
//@    },
  name: 'stuff${i}'
//@      "name": "[format('stuff{0}', range(0, 6)[copyIndex()])]",
  params: {
//@        "parameters": {
//@        },
    myInput: 'script-${i}'
//@          "myInput": {
//@            "value": "[format('script-{0}', range(0, 6)[copyIndex()])]"
//@          }
  }
}]

resource filteredIndexedZones 'Microsoft.Network/dnsZones@2018-05-01' = [for (account, i) in accounts: if(account.enabled) {
//@    {
//@      "copy": {
//@        "name": "filteredIndexedZones",
//@        "count": "[length(parameters('accounts'))]"
//@      },
//@      "condition": "[parameters('accounts')[copyIndex()].enabled]",
//@      "type": "Microsoft.Network/dnsZones",
//@      "apiVersion": "2018-05-01",
//@      "name": "[format('indexedZone-{0}-{1}', parameters('accounts')[copyIndex()].name, copyIndex())]",
//@    },
  name: 'indexedZone-${account.name}-${i}'
  location: account.location
//@      "location": "[parameters('accounts')[copyIndex()].location]"
}]

output lastNameServers array = filteredIndexedZones[length(accounts) - 1].properties.nameServers
//@    "lastNameServers": {
//@      "type": "array",
//@      "value": "[reference(resourceId('Microsoft.Network/dnsZones', format('indexedZone-{0}-{1}', parameters('accounts')[sub(length(parameters('accounts')), 1)].name, sub(length(parameters('accounts')), 1))), '2018-05-01').nameServers]"
//@    },

module filteredIndexedModules 'passthrough.bicep' = [for (account, i) in accounts: if(account.enabled) {
//@    {
//@      "copy": {
//@        "name": "filteredIndexedModules",
//@        "count": "[length(parameters('accounts'))]"
//@      },
//@      "condition": "[parameters('accounts')[copyIndex()].enabled]",
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "14375999048727010492"
//@            }
//@          },
//@          "parameters": {
//@            "myInput": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "myOutput": {
//@              "type": "string",
//@              "value": "[parameters('myInput')]"
//@            }
//@          }
//@        }
//@      }
//@    }
  name: 'stuff-${i}'
//@      "name": "[format('stuff-{0}', copyIndex())]",
  params: {
//@        "parameters": {
//@        },
    myInput: 'script-${account.name}-${i}'
//@          "myInput": {
//@            "value": "[format('script-{0}-{1}', parameters('accounts')[copyIndex()].name, copyIndex())]"
//@          }
  }
}]

output lastModuleOutput string = filteredIndexedModules[length(accounts) - 1].outputs.myOutput
//@    "lastModuleOutput": {
//@      "type": "string",
//@      "value": "[reference(resourceId('Microsoft.Resources/deployments', format('stuff-{0}', sub(length(parameters('accounts')), 1))), '2025-04-01').outputs.myOutput.value]"
//@    }

