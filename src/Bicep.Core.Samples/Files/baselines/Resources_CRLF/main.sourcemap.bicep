
@sys.description('this is basicStorage')
//@        "description": "this is basicStorage"
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@    {
//@      "type": "Microsoft.Storage/storageAccounts",
//@      "apiVersion": "2019-06-01",
//@      "name": "basicblobs",
//@      "metadata": {
//@      }
//@    },
  name: 'basicblobs'
  location: 'westus'
//@      "location": "westus",
  kind: 'BlobStorage'
//@      "kind": "BlobStorage",
  sku: {
//@      "sku": {
//@      },
    name: 'Standard_GRS'
//@        "name": "Standard_GRS"
  }
}

@sys.description('this is dnsZone')
//@        "description": "this is dnsZone"
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
//@    {
//@      "type": "Microsoft.Network/dnsZones",
//@      "apiVersion": "2018-05-01",
//@      "name": "myZone",
//@      "metadata": {
//@      }
//@    },
  name: 'myZone'
  location: 'global'
//@      "location": "global",
}

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@    {
//@      "type": "Microsoft.Storage/storageAccounts",
//@      "apiVersion": "2017-10-01",
//@      "name": "myencryptedone",
//@    },
  name: 'myencryptedone'
  location: 'eastus2'
//@      "location": "eastus2",
  properties: {
//@      "properties": {
//@      },
    supportsHttpsTrafficOnly: true
//@        "supportsHttpsTrafficOnly": true,
    accessTier: 'Hot'
//@        "accessTier": "Hot",
    encryption: {
//@        "encryption": {
//@        }
      keySource: 'Microsoft.Storage'
//@          "keySource": "Microsoft.Storage",
      services: {
//@          "services": {
//@          }
        blob: {
//@            "blob": {
//@            },
          enabled: true
//@              "enabled": true
        }
        file: {
//@            "file": {
//@            }
          enabled: true
//@              "enabled": true
        }
      }
    }
  }
  kind: 'StorageV2'
//@      "kind": "StorageV2",
  sku: {
//@      "sku": {
//@      }
    name: 'Standard_LRS'
//@        "name": "Standard_LRS"
  }
}

resource withExpressions 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@    {
//@      "type": "Microsoft.Storage/storageAccounts",
//@      "apiVersion": "2017-10-01",
//@      "name": "myencryptedone2",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Storage/storageAccounts', 'myencryptedone')]"
//@      ]
//@    },
  name: 'myencryptedone2'
  location: 'eastus2'
//@      "location": "eastus2",
  properties: {
//@      "properties": {
//@      },
    supportsHttpsTrafficOnly: !false
//@        "supportsHttpsTrafficOnly": "[not(false())]",
    accessTier: true ? 'Hot' : 'Cold'
//@        "accessTier": "[if(true(), 'Hot', 'Cold')]",
    encryption: {
//@        "encryption": {
//@        }
      keySource: 'Microsoft.Storage'
//@          "keySource": "Microsoft.Storage",
      services: {
//@          "services": {
//@          }
        blob: {
//@            "blob": {
//@            },
          enabled: true || false
//@              "enabled": "[or(true(), false())]"
        }
        file: {
//@            "file": {
//@            }
          enabled: true
//@              "enabled": true
        }
      }
    }
  }
  kind: 'StorageV2'
//@      "kind": "StorageV2",
  sku: {
//@      "sku": {
//@      },
    name: 'Standard_LRS'
//@        "name": "Standard_LRS"
  }
  dependsOn: [
    myStorageAccount
  ]
}

param applicationName string = 'to-do-app${uniqueString(resourceGroup().id)}'
//@    "applicationName": {
//@      "type": "string",
//@      "defaultValue": "[format('to-do-app{0}', uniqueString(resourceGroup().id))]"
//@    },
var hostingPlanName = applicationName // why not just use the param directly?
//@    "hostingPlanName": "[parameters('applicationName')]",

param appServicePlanTier string
//@    "appServicePlanTier": {
//@      "type": "string"
//@    },
param appServicePlanInstances int
//@    "appServicePlanInstances": {
//@      "type": "int"
//@    },

var location = resourceGroup().location
//@    "location": "[resourceGroup().location]",

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
//@    {
//@      "type": "Microsoft.Web/serverfarms",
//@      "apiVersion": "2019-08-01",
//@      "name": "[variables('hostingPlanName')]",
//@    },
  // dependsOn: resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName)
  name: hostingPlanName
  location: location
//@      "location": "[variables('location')]",
  sku: {
//@      "sku": {
//@      },
    name: appServicePlanTier
//@        "name": "[parameters('appServicePlanTier')]",
    capacity: appServicePlanInstances
//@        "capacity": "[parameters('appServicePlanInstances')]"
  }
  properties: {
//@      "properties": {
//@      }
    name: hostingPlanName // just hostingPlanName results in an error
//@        "name": "[variables('hostingPlanName')]"
  }
}

var cosmosDbResourceId = resourceId('Microsoft.DocumentDB/databaseAccounts',
//@    "cosmosDbResourceId": "[resourceId('Microsoft.DocumentDB/databaseAccounts', parameters('cosmosDb').account)]",
// comment
cosmosDb.account)
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint

// this variable is not accessed anywhere in this template and depends on a run-time reference
// it should not be present at all in the template output as there is nowhere logical to put it
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint

param webSiteName string
//@    "webSiteName": {
//@      "type": "string"
//@    },
param cosmosDb object
//@    "cosmosDb": {
//@      "type": "object"
//@    },
resource site 'Microsoft.Web/sites@2019-08-01' = {
//@    {
//@      "type": "Microsoft.Web/sites",
//@      "apiVersion": "2019-08-01",
//@      "name": "[parameters('webSiteName')]",
//@    },
  name: webSiteName
  location: location
//@      "location": "[variables('location')]",
  properties: {
//@      "properties": {
//@      }
    // not yet supported // serverFarmId: farm.id
    siteConfig: {
//@        "siteConfig": {
//@        }
      appSettings: [
//@          "appSettings": [
//@          ]
        {
//@            {
//@            },
          name: 'CosmosDb:Account'
//@              "name": "CosmosDb:Account",
          value: reference(cosmosDbResourceId).documentEndpoint
//@              "value": "[reference(variables('cosmosDbResourceId')).documentEndpoint]"
        }
        {
//@            {
//@            },
          name: 'CosmosDb:Key'
//@              "name": "CosmosDb:Key",
          value: listKeys(cosmosDbResourceId, '2020-04-01').primaryMasterKey
//@              "value": "[listKeys(variables('cosmosDbResourceId'), '2020-04-01').primaryMasterKey]"
        }
        {
//@            {
//@            },
          name: 'CosmosDb:DatabaseName'
//@              "name": "CosmosDb:DatabaseName",
          value: cosmosDb.databaseName
//@              "value": "[parameters('cosmosDb').databaseName]"
        }
        {
//@            {
//@            }
          name: 'CosmosDb:ContainerName'
//@              "name": "CosmosDb:ContainerName",
          value: cosmosDb.containerName
//@              "value": "[parameters('cosmosDb').containerName]"
        }
      ]
    }
  }
}

var _siteApiVersion = site.apiVersion
//@    "_siteApiVersion": "2019-08-01",
var _siteType = site.type
//@    "_siteType": "Microsoft.Web/sites",

output siteApiVersion string = site.apiVersion
//@    "siteApiVersion": {
//@      "type": "string",
//@      "value": "2019-08-01"
//@    },
output siteType string = site.type
//@    "siteType": {
//@      "type": "string",
//@      "value": "Microsoft.Web/sites"
//@    },

resource nested 'Microsoft.Resources/deployments@2019-10-01' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2019-10-01",
//@      "name": "nestedTemplate1",
//@    },
  name: 'nestedTemplate1'
  properties: {
//@      "properties": {
//@      }
    mode: 'Incremental'
//@        "mode": "Incremental",
    template: {
//@        "template": {
//@        }
      // string key value
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
      contentVersion: '1.0.0.0'
//@          "contentVersion": "1.0.0.0",
      resources: [
//@          "resources": []
      ]
    }
  }
}

// should be able to access the read only properties
resource accessingReadOnlyProperties 'Microsoft.Foo/foos@2019-10-01' = {
//@    {
//@      "type": "Microsoft.Foo/foos",
//@      "apiVersion": "2019-10-01",
//@      "name": "nestedTemplate1",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Resources/deployments', 'nestedTemplate1')]"
//@      ]
//@    },
  name: 'nestedTemplate1'
  properties: {
//@      "properties": {
//@      },
    otherId: nested.id
//@        "otherId": "[resourceId('Microsoft.Resources/deployments', 'nestedTemplate1')]",
    otherName: nested.name
//@        "otherName": "nestedTemplate1",
    otherVersion: nested.apiVersion
//@        "otherVersion": "2019-10-01",
    otherType: nested.type
//@        "otherType": "Microsoft.Resources/deployments",

    otherThings: nested.properties.mode
//@        "otherThings": "[reference(resourceId('Microsoft.Resources/deployments', 'nestedTemplate1'), '2019-10-01').mode]"
  }
}

resource resourceA 'My.Rp/typeA@2020-01-01' = {
//@    {
//@      "type": "My.Rp/typeA",
//@      "apiVersion": "2020-01-01",
//@      "name": "resourceA"
//@    },
  name: 'resourceA'
}

resource resourceB 'My.Rp/typeA/typeB@2020-01-01' = {
//@    {
//@      "type": "My.Rp/typeA/typeB",
//@      "apiVersion": "2020-01-01",
//@      "name": "[format('{0}/resourceB', 'resourceA')]",
//@      "dependsOn": [
//@        "[resourceId('My.Rp/typeA', 'resourceA')]"
//@      ]
//@    },
  name: '${resourceA.name}/resourceB'
}

resource resourceC 'My.Rp/typeA/typeB@2020-01-01' = {
//@    {
//@      "type": "My.Rp/typeA/typeB",
//@      "apiVersion": "2020-01-01",
//@      "name": "[format('{0}/resourceC', 'resourceA')]",
//@      "dependsOn": [
//@        "[resourceId('My.Rp/typeA', 'resourceA')]",
//@        "[resourceId('My.Rp/typeA/typeB', split(format('{0}/resourceB', 'resourceA'), '/')[0], split(format('{0}/resourceB', 'resourceA'), '/')[1])]"
//@      ]
//@    },
  name: '${resourceA.name}/resourceC'
  properties: {
//@      "properties": {
//@      },
    aId: resourceA.id
//@        "aId": "[resourceId('My.Rp/typeA', 'resourceA')]",
    aType: resourceA.type
//@        "aType": "My.Rp/typeA",
    aName: resourceA.name
//@        "aName": "resourceA",
    aApiVersion: resourceA.apiVersion
//@        "aApiVersion": "2020-01-01",
    bProperties: resourceB.properties
//@        "bProperties": "[reference(resourceId('My.Rp/typeA/typeB', split(format('{0}/resourceB', 'resourceA'), '/')[0], split(format('{0}/resourceB', 'resourceA'), '/')[1]), '2020-01-01')]"
  }
}

var varARuntime = {
//@          {
//@          }
  bId: resourceB.id
//@            "bId": "[resourceId('My.Rp/typeA/typeB', split(format('{0}/resourceB', 'resourceA'), '/')[0], split(format('{0}/resourceB', 'resourceA'), '/')[1])]",
  bType: resourceB.type
//@            "bType": "My.Rp/typeA/typeB",
  bName: resourceB.name
//@            "bName": "[format('{0}/resourceB', 'resourceA')]",
  bApiVersion: resourceB.apiVersion
//@            "bApiVersion": "2020-01-01",
  aKind: resourceA.kind
//@            "aKind": "[reference(resourceId('My.Rp/typeA', 'resourceA'), '2020-01-01', 'full').kind]"
}

var varBRuntime = [
//@        "runtime": [
//@        ],
  varARuntime
]

var resourceCRef = {
//@    "resourceCRef": {
//@    },
  id: resourceC.id
//@      "id": "[resourceId('My.Rp/typeA/typeB', split(format('{0}/resourceC', 'resourceA'), '/')[0], split(format('{0}/resourceC', 'resourceA'), '/')[1])]"
}
var setResourceCRef = true
//@    "setResourceCRef": true,

resource resourceD 'My.Rp/typeD@2020-01-01' = {
//@    {
//@      "type": "My.Rp/typeD",
//@      "apiVersion": "2020-01-01",
//@      "name": "constant",
//@      "dependsOn": [
//@        "[resourceId('My.Rp/typeA', 'resourceA')]",
//@        "[resourceId('My.Rp/typeA/typeB', split(format('{0}/resourceB', 'resourceA'), '/')[0], split(format('{0}/resourceB', 'resourceA'), '/')[1])]",
//@        "[resourceId('My.Rp/typeA/typeB', split(format('{0}/resourceC', 'resourceA'), '/')[0], split(format('{0}/resourceC', 'resourceA'), '/')[1])]"
//@      ]
//@    },
  name: 'constant'
  properties: {
//@      "properties": {
//@      },
    runtime: varBRuntime
    // repro for https://github.com/Azure/bicep/issues/316
    repro316: setResourceCRef ? resourceCRef : null
//@        "repro316": "[if(variables('setResourceCRef'), variables('resourceCRef'), null())]"
  }
}

var myInterpKey = 'abc'
//@    "myInterpKey": "abc",
resource resourceWithInterp 'My.Rp/interp@2020-01-01' = {
//@    {
//@      "type": "My.Rp/interp",
//@      "apiVersion": "2020-01-01",
//@      "name": "interpTest",
//@    },
  name: 'interpTest'
  properties: {
//@      "properties": {
//@      }
    '${myInterpKey}': 1
//@        "[format('{0}', variables('myInterpKey'))]": 1,
    'abc${myInterpKey}def': 2
//@        "[format('abc{0}def', variables('myInterpKey'))]": 2,
    '${myInterpKey}abc${myInterpKey}': 3
//@        "[format('{0}abc{1}', variables('myInterpKey'), variables('myInterpKey'))]": 3
  }
}

resource resourceWithEscaping 'My.Rp/mockResource@2020-01-01' = {
//@    {
//@      "type": "My.Rp/mockResource",
//@      "apiVersion": "2020-01-01",
//@      "name": "test",
//@    },
  name: 'test'
  properties: {
//@      "properties": {
//@      }
    // both key and value should be escaped in template output
    '[resourceGroup().location]': '[resourceGroup().location]'
//@        "[[resourceGroup().location]": "[[resourceGroup().location]"
  }
}

param shouldDeployVm bool = true
//@    "shouldDeployVm": {
//@      "type": "bool",
//@      "defaultValue": true
//@    }

@sys.description('this is vmWithCondition')
//@        "description": "this is vmWithCondition"
resource vmWithCondition 'Microsoft.Compute/virtualMachines@2020-06-01' = if (shouldDeployVm) {
//@    {
//@      "condition": "[parameters('shouldDeployVm')]",
//@      "type": "Microsoft.Compute/virtualMachines",
//@      "apiVersion": "2020-06-01",
//@      "name": "vmName",
//@      "metadata": {
//@      }
//@    },
  name: 'vmName'
  location: 'westus'
//@      "location": "westus",
  properties: {
//@      "properties": {
//@      },
    osProfile: {
//@        "osProfile": {
//@        }
      windowsConfiguration: {
//@          "windowsConfiguration": {
//@          }
        enableAutomaticUpdates: true
//@            "enableAutomaticUpdates": true
      }
    }
  }
}

@sys.description('this is another vmWithCondition')
//@        "description": "this is another vmWithCondition"
resource vmWithCondition2 'Microsoft.Compute/virtualMachines@2020-06-01' =
//@    {
//@      "type": "Microsoft.Compute/virtualMachines",
//@      "apiVersion": "2020-06-01",
//@      "name": "vmName2",
//@      "metadata": {
//@      }
//@    },
                    if (shouldDeployVm) {
//@      "condition": "[parameters('shouldDeployVm')]",
  name: 'vmName2'
  location: 'westus'
//@      "location": "westus",
  properties: {
//@      "properties": {
//@      },
    osProfile: {
//@        "osProfile": {
//@        }
      windowsConfiguration: {
//@          "windowsConfiguration": {
//@          }
        enableAutomaticUpdates: true
//@            "enableAutomaticUpdates": true
      }
    }
  }
}

resource extension1 'My.Rp/extensionResource@2020-12-01' = {
//@    {
//@      "type": "My.Rp/extensionResource",
//@      "apiVersion": "2020-12-01",
//@      "scope": "[resourceId('Microsoft.Compute/virtualMachines', 'vmName')]",
//@      "name": "extension",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Compute/virtualMachines', 'vmName')]"
//@      ]
//@    },
  name: 'extension'
  scope: vmWithCondition
}

resource extension2 'My.Rp/extensionResource@2020-12-01' = {
//@    {
//@      "type": "My.Rp/extensionResource",
//@      "apiVersion": "2020-12-01",
//@      "scope": "[extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension')]",
//@      "name": "extension",
//@      "dependsOn": [
//@        "[extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension')]"
//@      ]
//@    },
  name: 'extension'
  scope: extension1
}

resource extensionDependencies 'My.Rp/mockResource@2020-01-01' = {
//@    {
//@      "type": "My.Rp/mockResource",
//@      "apiVersion": "2020-01-01",
//@      "name": "extensionDependencies",
//@      "dependsOn": [
//@        "[extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension')]",
//@        "[extensionResourceId(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), 'My.Rp/extensionResource', 'extension')]",
//@        "[resourceId('Microsoft.Compute/virtualMachines', 'vmName')]"
//@      ]
//@    },
  name: 'extensionDependencies'
  properties: {
//@      "properties": {
//@      },
    res1: vmWithCondition.id
//@        "res1": "[resourceId('Microsoft.Compute/virtualMachines', 'vmName')]",
    res1runtime: vmWithCondition.properties.something
//@        "res1runtime": "[reference(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), '2020-06-01').something]",
    res2: extension1.id
//@        "res2": "[extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension')]",
    res2runtime: extension1.properties.something
//@        "res2runtime": "[reference(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), '2020-12-01').something]",
    res3: extension2.id
//@        "res3": "[extensionResourceId(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), 'My.Rp/extensionResource', 'extension')]",
    res3runtime: extension2.properties.something
//@        "res3runtime": "[reference(extensionResourceId(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), 'My.Rp/extensionResource', 'extension'), '2020-12-01').something]"
  }
}

@sys.description('this is existing1')
resource existing1 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
  name: 'existing1'
  scope: extension1
}

resource existing2 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
  name: 'existing2'
  scope: existing1
}

resource extension3 'My.Rp/extensionResource@2020-12-01' = {
//@    {
//@      "type": "My.Rp/extensionResource",
//@      "apiVersion": "2020-12-01",
//@      "scope": "[extensionResourceId(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), 'Mock.Rp/existingExtensionResource', 'existing1')]",
//@      "name": "extension3",
//@      "dependsOn": [
//@        "[extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension')]"
//@      ]
//@    },
  name: 'extension3'
  scope: existing1
}

/*
  valid loop cases
*/
var storageAccounts = [
//@    "storageAccounts": [
//@    ],
  {
//@      {
//@      },
    name: 'one'
//@        "name": "one",
    location: 'eastus2'
//@        "location": "eastus2"
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

// just a storage account loop
@sys.description('this is just a storage account loop')
//@        "description": "this is just a storage account loop"
resource storageResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@    {
//@      "copy": {
//@        "name": "storageResources",
//@        "count": "[length(variables('storageAccounts'))]"
//@      },
//@      "type": "Microsoft.Storage/storageAccounts",
//@      "apiVersion": "2019-06-01",
//@      "name": "[variables('storageAccounts')[copyIndex()].name]",
//@      "metadata": {
//@      }
//@    },
  name: account.name
  location: account.location
//@      "location": "[variables('storageAccounts')[copyIndex()].location]",
  sku: {
//@      "sku": {
//@      },
    name: 'Standard_LRS'
//@        "name": "Standard_LRS"
  }
  kind: 'StorageV2'
//@      "kind": "StorageV2",
}]

// storage account loop with index
@sys.description('this is just a storage account loop with index')
//@        "description": "this is just a storage account loop with index"
resource storageResourcesWithIndex 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, i) in storageAccounts: {
//@    {
//@      "copy": {
//@        "name": "storageResourcesWithIndex",
//@        "count": "[length(variables('storageAccounts'))]"
//@      },
//@      "type": "Microsoft.Storage/storageAccounts",
//@      "apiVersion": "2019-06-01",
//@      "name": "[format('{0}{1}', variables('storageAccounts')[copyIndex()].name, copyIndex())]",
//@      "metadata": {
//@      }
//@    },
  name: '${account.name}${i}'
  location: account.location
//@      "location": "[variables('storageAccounts')[copyIndex()].location]",
  sku: {
//@      "sku": {
//@      },
    name: 'Standard_LRS'
//@        "name": "Standard_LRS"
  }
  kind: 'StorageV2'
//@      "kind": "StorageV2",
}]

// basic nested loop
@sys.description('this is just a basic nested loop')
//@        "description": "this is just a basic nested loop"
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@    {
//@      "copy": {
//@        "name": "vnet",
//@        "count": "[length(range(0, 3))]"
//@      },
//@      "type": "Microsoft.Network/virtualNetworks",
//@      "apiVersion": "2020-06-01",
//@      "name": "[format('vnet-{0}', range(0, 3)[copyIndex()])]",
//@      "metadata": {
//@      }
//@    },
  name: 'vnet-${i}'
  properties: {
//@      "properties": {
//@        "copy": [
//@        ]
//@      },
    subnets: [for j in range(0, 4): {
//@          {
//@            "name": "subnets",
//@            "count": "[length(range(0, 4))]",
//@            "input": {
//@            }
//@          }
      // #completionTest(0,1,2,3,4,5) -> subnetIdAndProperties

      // #completionTest(6) -> subnetIdAndPropertiesNoColon
      name: 'subnet-${i}-${j}'
//@              "name": "[format('subnet-{0}-{1}', range(0, 3)[copyIndex()], range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

// duplicate identifiers within the loop are allowed
resource duplicateIdentifiersWithinLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@    {
//@      "copy": {
//@        "name": "duplicateIdentifiersWithinLoop",
//@        "count": "[length(range(0, 3))]"
//@      },
//@      "type": "Microsoft.Network/virtualNetworks",
//@      "apiVersion": "2020-06-01",
//@      "name": "[format('vnet-{0}', range(0, 3)[copyIndex()])]",
//@    },
  name: 'vnet-${i}'
  properties: {
//@      "properties": {
//@        "copy": [
//@        ]
//@      }
    subnets: [for i in range(0, 4): {
//@          {
//@            "name": "subnets",
//@            "count": "[length(range(0, 4))]",
//@            "input": {
//@            }
//@          }
      name: 'subnet-${i}-${i}'
//@              "name": "[format('subnet-{0}-{1}', range(0, 4)[copyIndex('subnets')], range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

// duplicate identifiers in global and single loop scope are allowed (inner variable hides the outer)
var canHaveDuplicatesAcrossScopes = 'hello'
//@    "canHaveDuplicatesAcrossScopes": "hello",
resource duplicateInGlobalAndOneLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for canHaveDuplicatesAcrossScopes in range(0, 3): {
//@    {
//@      "copy": {
//@        "name": "duplicateInGlobalAndOneLoop",
//@        "count": "[length(range(0, 3))]"
//@      },
//@      "type": "Microsoft.Network/virtualNetworks",
//@      "apiVersion": "2020-06-01",
//@      "name": "[format('vnet-{0}', range(0, 3)[copyIndex()])]",
//@    },
  name: 'vnet-${canHaveDuplicatesAcrossScopes}'
  properties: {
//@      "properties": {
//@        "copy": [
//@        ]
//@      }
    subnets: [for i in range(0, 4): {
//@          {
//@            "name": "subnets",
//@            "count": "[length(range(0, 4))]",
//@            "input": {
//@            }
//@          }
      name: 'subnet-${i}-${i}'
//@              "name": "[format('subnet-{0}-{1}', range(0, 4)[copyIndex('subnets')], range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

// duplicate in global and multiple loop scopes are allowed (inner hides the outer)
var duplicatesEverywhere = 'hello'
//@    "duplicatesEverywhere": "hello",
resource duplicateInGlobalAndTwoLoops 'Microsoft.Network/virtualNetworks@2020-06-01' = [for duplicatesEverywhere in range(0, 3): {
//@    {
//@      "copy": {
//@        "name": "duplicateInGlobalAndTwoLoops",
//@        "count": "[length(range(0, 3))]"
//@      },
//@      "type": "Microsoft.Network/virtualNetworks",
//@      "apiVersion": "2020-06-01",
//@      "name": "[format('vnet-{0}', range(0, 3)[copyIndex()])]",
//@    },
  name: 'vnet-${duplicatesEverywhere}'
  properties: {
//@      "properties": {
//@        "copy": [
//@        ]
//@      }
    subnets: [for duplicatesEverywhere in range(0, 4): {
//@          {
//@            "name": "subnets",
//@            "count": "[length(range(0, 4))]",
//@            "input": {
//@            }
//@          }
      name: 'subnet-${duplicatesEverywhere}'
//@              "name": "[format('subnet-{0}', range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

/*
  Scope values created via array access on a resource collection
*/
resource dnsZones 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in range(0,4): {
//@    {
//@      "copy": {
//@        "name": "dnsZones",
//@        "count": "[length(range(0, 4))]"
//@      },
//@      "type": "Microsoft.Network/dnsZones",
//@      "apiVersion": "2018-05-01",
//@      "name": "[format('zone{0}', range(0, 4)[copyIndex()])]",
//@    },
  name: 'zone${zone}'
  location: 'global'
//@      "location": "global"
}]

resource locksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for lock in range(0,2): {
//@    {
//@      "copy": {
//@        "name": "locksOnZones",
//@        "count": "[length(range(0, 2))]"
//@      },
//@      "type": "Microsoft.Authorization/locks",
//@      "apiVersion": "2016-09-01",
//@      "scope": "[resourceId('Microsoft.Network/dnsZones', format('zone{0}', range(0, 4)[range(0, 2)[copyIndex()]]))]",
//@      "name": "[format('lock{0}', range(0, 2)[copyIndex()])]",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Network/dnsZones', format('zone{0}', range(0, 4)[range(0, 2)[copyIndex()]]))]"
//@      ]
//@    },
  name: 'lock${lock}'
  properties: {
//@      "properties": {
//@      },
    level: 'CanNotDelete'
//@        "level": "CanNotDelete"
  }
  scope: dnsZones[lock]
}]

resource moreLocksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for (lock, i) in range(0,3): {
//@    {
//@      "copy": {
//@        "name": "moreLocksOnZones",
//@        "count": "[length(range(0, 3))]"
//@      },
//@      "type": "Microsoft.Authorization/locks",
//@      "apiVersion": "2016-09-01",
//@      "scope": "[resourceId('Microsoft.Network/dnsZones', format('zone{0}', range(0, 4)[copyIndex()]))]",
//@      "name": "[format('another{0}', copyIndex())]",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Network/dnsZones', format('zone{0}', range(0, 4)[copyIndex()]))]"
//@      ]
//@    },
  name: 'another${i}'
  properties: {
//@      "properties": {
//@      },
    level: 'ReadOnly'
//@        "level": "ReadOnly"
  }
  scope: dnsZones[i]
}]

resource singleLockOnFirstZone 'Microsoft.Authorization/locks@2016-09-01' = {
//@    {
//@      "type": "Microsoft.Authorization/locks",
//@      "apiVersion": "2016-09-01",
//@      "scope": "[resourceId('Microsoft.Network/dnsZones', format('zone{0}', range(0, 4)[0]))]",
//@      "name": "single-lock",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Network/dnsZones', format('zone{0}', range(0, 4)[0]))]"
//@      ]
//@    },
  name: 'single-lock'
  properties: {
//@      "properties": {
//@      },
    level: 'ReadOnly'
//@        "level": "ReadOnly"
  }
  scope: dnsZones[0]
}


resource p1_vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@    {
//@      "type": "Microsoft.Network/virtualNetworks",
//@      "apiVersion": "2020-06-01",
//@      "name": "myVnet",
//@    },
  location: resourceGroup().location
//@      "location": "[resourceGroup().location]",
  name: 'myVnet'
  properties: {
//@      "properties": {
//@      }
    addressSpace: {
//@        "addressSpace": {
//@        }
      addressPrefixes: [
//@          "addressPrefixes": [
//@          ]
        '10.0.0.0/20'
//@            "10.0.0.0/20"
      ]
    }
  }
}

resource p1_subnet1 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@    {
//@      "type": "Microsoft.Network/virtualNetworks/subnets",
//@      "apiVersion": "2020-06-01",
//@      "name": "[format('{0}/{1}', 'myVnet', 'subnet1')]",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Network/virtualNetworks', 'myVnet')]"
//@      ]
//@    },
  parent: p1_vnet
  name: 'subnet1'
  properties: {
//@      "properties": {
//@      },
    addressPrefix: '10.0.0.0/24'
//@        "addressPrefix": "10.0.0.0/24"
  }
}

resource p1_subnet2 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@    {
//@      "type": "Microsoft.Network/virtualNetworks/subnets",
//@      "apiVersion": "2020-06-01",
//@      "name": "[format('{0}/{1}', 'myVnet', 'subnet2')]",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Network/virtualNetworks', 'myVnet')]"
//@      ]
//@    },
  parent: p1_vnet
  name: 'subnet2'
  properties: {
//@      "properties": {
//@      },
    addressPrefix: '10.0.1.0/24'
//@        "addressPrefix": "10.0.1.0/24"
  }
}

output p1_subnet1prefix string = p1_subnet1.properties.addressPrefix
//@    "p1_subnet1prefix": {
//@      "type": "string",
//@      "value": "[reference(resourceId('Microsoft.Network/virtualNetworks/subnets', 'myVnet', 'subnet1'), '2020-06-01').addressPrefix]"
//@    },
output p1_subnet1name string = p1_subnet1.name
//@    "p1_subnet1name": {
//@      "type": "string",
//@      "value": "subnet1"
//@    },
output p1_subnet1type string = p1_subnet1.type
//@    "p1_subnet1type": {
//@      "type": "string",
//@      "value": "Microsoft.Network/virtualNetworks/subnets"
//@    },
output p1_subnet1id string = p1_subnet1.id
//@    "p1_subnet1id": {
//@      "type": "string",
//@      "value": "[resourceId('Microsoft.Network/virtualNetworks/subnets', 'myVnet', 'subnet1')]"
//@    },

// parent property with extension resource
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@    {
//@      "type": "Microsoft.Rp1/resource1",
//@      "apiVersion": "2020-06-01",
//@      "name": "p2res1"
//@    },
  name: 'p2res1'
}

resource p2_res1child 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@    {
//@      "type": "Microsoft.Rp1/resource1/child1",
//@      "apiVersion": "2020-06-01",
//@      "name": "[format('{0}/{1}', 'p2res1', 'child1')]",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Rp1/resource1', 'p2res1')]"
//@      ]
//@    },
  parent: p2_res1
  name: 'child1'
}

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@    {
//@      "type": "Microsoft.Rp2/resource2",
//@      "apiVersion": "2020-06-01",
//@      "scope": "[resourceId('Microsoft.Rp1/resource1/child1', 'p2res1', 'child1')]",
//@      "name": "res2",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Rp1/resource1/child1', 'p2res1', 'child1')]"
//@      ]
//@    },
  scope: p2_res1child
  name: 'res2'
}

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@    {
//@      "type": "Microsoft.Rp2/resource2/child2",
//@      "apiVersion": "2020-06-01",
//@      "scope": "[resourceId('Microsoft.Rp1/resource1/child1', 'p2res1', 'child1')]",
//@      "name": "[format('{0}/{1}', 'res2', 'child2')]",
//@      "dependsOn": [
//@        "[extensionResourceId(resourceId('Microsoft.Rp1/resource1/child1', 'p2res1', 'child1'), 'Microsoft.Rp2/resource2', 'res2')]"
//@      ]
//@    },
  parent: p2_res2
  name: 'child2'
}

output p2_res2childprop string = p2_res2child.properties.someProp
//@    "p2_res2childprop": {
//@      "type": "string",
//@      "value": "[reference(extensionResourceId(resourceId('Microsoft.Rp1/resource1/child1', 'p2res1', 'child1'), 'Microsoft.Rp2/resource2/child2', 'res2', 'child2'), '2020-06-01').someProp]"
//@    },
output p2_res2childname string = p2_res2child.name
//@    "p2_res2childname": {
//@      "type": "string",
//@      "value": "child2"
//@    },
output p2_res2childtype string = p2_res2child.type
//@    "p2_res2childtype": {
//@      "type": "string",
//@      "value": "Microsoft.Rp2/resource2/child2"
//@    },
output p2_res2childid string = p2_res2child.id
//@    "p2_res2childid": {
//@      "type": "string",
//@      "value": "[extensionResourceId(resourceId('Microsoft.Rp1/resource1/child1', 'p2res1', 'child1'), 'Microsoft.Rp2/resource2/child2', 'res2', 'child2')]"
//@    },

// parent property with 'existing' resource
resource p3_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  name: 'p3res1'
}

resource p3_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@    {
//@      "type": "Microsoft.Rp1/resource1/child1",
//@      "apiVersion": "2020-06-01",
//@      "name": "[format('{0}/{1}', 'p3res1', 'child1')]"
//@    },
  parent: p3_res1
  name: 'child1'
}

output p3_res1childprop string = p3_child1.properties.someProp
//@    "p3_res1childprop": {
//@      "type": "string",
//@      "value": "[reference(resourceId('Microsoft.Rp1/resource1/child1', 'p3res1', 'child1'), '2020-06-01').someProp]"
//@    },
output p3_res1childname string = p3_child1.name
//@    "p3_res1childname": {
//@      "type": "string",
//@      "value": "child1"
//@    },
output p3_res1childtype string = p3_child1.type
//@    "p3_res1childtype": {
//@      "type": "string",
//@      "value": "Microsoft.Rp1/resource1/child1"
//@    },
output p3_res1childid string = p3_child1.id
//@    "p3_res1childid": {
//@      "type": "string",
//@      "value": "[resourceId('Microsoft.Rp1/resource1/child1', 'p3res1', 'child1')]"
//@    },

// parent & child with 'existing'
resource p4_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  scope: tenant()
  name: 'p4res1'
}

resource p4_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' existing = {
  parent: p4_res1
  name: 'child1'
}

output p4_res1childprop string = p4_child1.properties.someProp
//@    "p4_res1childprop": {
//@      "type": "string",
//@      "value": "[reference(tenantResourceId('Microsoft.Rp1/resource1/child1', 'p4res1', 'child1'), '2020-06-01').someProp]"
//@    },
output p4_res1childname string = p4_child1.name
//@    "p4_res1childname": {
//@      "type": "string",
//@      "value": "child1"
//@    },
output p4_res1childtype string = p4_child1.type
//@    "p4_res1childtype": {
//@      "type": "string",
//@      "value": "Microsoft.Rp1/resource1/child1"
//@    },
output p4_res1childid string = p4_child1.id
//@    "p4_res1childid": {
//@      "type": "string",
//@      "value": "[tenantResourceId('Microsoft.Rp1/resource1/child1', 'p4res1', 'child1')]"
//@    },

// parent & nested child with decorators https://github.com/Azure/bicep/issues/10970
var dbs = ['db1', 'db2','db3']
//@    "dbs": [
//@      "db1",
//@      "db2",
//@      "db3"
//@    ],
resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
//@    {
//@      "type": "Microsoft.Sql/servers",
//@      "apiVersion": "2021-11-01",
//@      "name": "sql-server-name",
//@    },
  name: 'sql-server-name'
  location: 'polandcentral'
//@      "location": "polandcentral"

  @batchSize(1)
  @description('Sql Databases')
//@        "description": "Sql Databases"
  resource sqlDatabases 'databases' = [for db in dbs: {
//@    {
//@      "copy": {
//@        "name": "sqlServer::sqlDatabases",
//@        "count": "[length(variables('dbs'))]",
//@        "mode": "serial",
//@        "batchSize": 1
//@      },
//@      "type": "Microsoft.Sql/servers/databases",
//@      "apiVersion": "2021-11-01",
//@      "name": "[format('{0}/{1}', 'sql-server-name', variables('dbs')[copyIndex()])]",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Sql/servers', 'sql-server-name')]"
//@      ],
//@      "metadata": {
//@      }
//@    },
    name: db
    location: 'polandcentral'
//@      "location": "polandcentral",
  }]

  @description('Primary Sql Database')
//@        "description": "Primary Sql Database"
  resource primaryDb 'databases' = {
//@    {
//@      "type": "Microsoft.Sql/servers/databases",
//@      "apiVersion": "2021-11-01",
//@      "name": "[format('{0}/{1}', 'sql-server-name', 'primary-db')]",
//@      "dependsOn": [
//@        "[resourceId('Microsoft.Sql/servers', 'sql-server-name')]"
//@      ],
//@      "metadata": {
//@      }
//@    },
    name: 'primary-db'
    location: 'polandcentral'
//@      "location": "polandcentral",

    resource threatProtection 'advancedThreatProtectionSettings' existing = {
      name: 'default'
    }
  }
}

//nameof
output nameof_sqlServer string = nameof(sqlServer)
//@    "nameof_sqlServer": {
//@      "type": "string",
//@      "value": "sqlServer"
//@    },
output nameof_location string = nameof(sqlServer.location)
//@    "nameof_location": {
//@      "type": "string",
//@      "value": "location"
//@    },
output nameof_minCapacity string = nameof(sqlServer::primaryDb.properties.minCapacity)
//@    "nameof_minCapacity": {
//@      "type": "string",
//@      "value": "minCapacity"
//@    },
output nameof_creationTime string = nameof(sqlServer::primaryDb::threatProtection.properties.creationTime)
//@    "nameof_creationTime": {
//@      "type": "string",
//@      "value": "creationTime"
//@    },
output nameof_id string = nameof(sqlServer::sqlDatabases[0].id)
//@    "nameof_id": {
//@      "type": "string",
//@      "value": "id"
//@    }

var sqlConfig = {
//@    "sqlConfig": {
//@    }
  westus: {}
//@      "westus": {},
  'server-name': {}
//@      "server-name": {}
}

resource sqlServerWithNameof 'Microsoft.Sql/servers@2021-11-01' = {
//@    {
//@      "type": "Microsoft.Sql/servers",
//@      "apiVersion": "2021-11-01",
//@      "name": "[format('sql-server-nameof-{0}', 'server-name')]",
//@    }
  name: 'sql-server-nameof-${nameof(sqlConfig['server-name'])}'
  location: nameof(sqlConfig.westus)
//@      "location": "westus"
}

