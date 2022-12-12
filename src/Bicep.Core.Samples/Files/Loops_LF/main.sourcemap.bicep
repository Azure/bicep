param name string
//@[line000->line011]     "name": {
//@[line000->line012]       "type": "string"
//@[line000->line013]     },
param accounts array
//@[line001->line014]     "accounts": {
//@[line001->line015]       "type": "array"
//@[line001->line016]     },
param index int
//@[line002->line017]     "index": {
//@[line002->line018]       "type": "int"
//@[line002->line019]     }

// single resource
resource singleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[line005->line043]     {
//@[line005->line044]       "type": "Microsoft.Storage/storageAccounts",
//@[line005->line045]       "apiVersion": "2019-06-01",
//@[line005->line046]       "name": "[format('{0}single-resource-name', parameters('name'))]",
//@[line005->line052]     },
  name: '${name}single-resource-name'
  location: resourceGroup().location
//@[line007->line047]       "location": "[resourceGroup().location]",
  kind: 'StorageV2'
//@[line008->line048]       "kind": "StorageV2",
  sku: {
//@[line009->line049]       "sku": {
//@[line009->line051]       }
    name: 'Standard_LRS'
//@[line010->line050]         "name": "Standard_LRS"
  }
}

// extension of single resource
resource singleResourceExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[line015->line053]     {
//@[line015->line054]       "type": "Microsoft.Authorization/locks",
//@[line015->line055]       "apiVersion": "2016-09-01",
//@[line015->line056]       "scope": "[format('Microsoft.Storage/storageAccounts/{0}', format('{0}single-resource-name', parameters('name')))]",
//@[line015->line057]       "name": "single-resource-lock",
//@[line015->line061]       "dependsOn": [
//@[line015->line062]         "[resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name')))]"
//@[line015->line063]       ]
//@[line015->line064]     },
  scope: singleResource
  name: 'single-resource-lock'
  properties: {
//@[line018->line058]       "properties": {
//@[line018->line060]       },
    level: 'CanNotDelete'
//@[line019->line059]         "level": "CanNotDelete"
  }
}

// single resource cascade extension
resource singleResourceCascadeExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[line024->line065]     {
//@[line024->line066]       "type": "Microsoft.Authorization/locks",
//@[line024->line067]       "apiVersion": "2016-09-01",
//@[line024->line068]       "scope": "[extensionResourceId(format('Microsoft.Storage/storageAccounts/{0}', format('{0}single-resource-name', parameters('name'))), 'Microsoft.Authorization/locks', 'single-resource-lock')]",
//@[line024->line069]       "name": "single-resource-cascade-extension",
//@[line024->line073]       "dependsOn": [
//@[line024->line074]         "[extensionResourceId(resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name'))), 'Microsoft.Authorization/locks', 'single-resource-lock')]"
//@[line024->line075]       ]
//@[line024->line076]     },
  scope: singleResourceExtension
  name: 'single-resource-cascade-extension'
  properties: {
//@[line027->line070]       "properties": {
//@[line027->line072]       },
    level: 'CanNotDelete'
//@[line028->line071]         "level": "CanNotDelete"
  }
}

// resource collection
@batchSize(42)
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[line034->line077]     {
//@[line034->line078]       "copy": {
//@[line034->line079]         "name": "storageAccounts",
//@[line034->line080]         "count": "[length(parameters('accounts'))]",
//@[line034->line081]         "mode": "serial",
//@[line034->line082]         "batchSize": 42
//@[line034->line083]       },
//@[line034->line084]       "type": "Microsoft.Storage/storageAccounts",
//@[line034->line085]       "apiVersion": "2019-06-01",
//@[line034->line086]       "name": "[format('{0}-collection-{1}', parameters('name'), parameters('accounts')[copyIndex()].name)]",
//@[line034->line092]       "dependsOn": [
//@[line034->line093]         "[resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name')))]"
//@[line034->line094]       ]
//@[line034->line095]     },
  name: '${name}-collection-${account.name}'
  location: account.location
//@[line036->line087]       "location": "[parameters('accounts')[copyIndex()].location]",
  kind: 'StorageV2'
//@[line037->line088]       "kind": "StorageV2",
  sku: {
//@[line038->line089]       "sku": {
//@[line038->line091]       },
    name: 'Standard_LRS'
//@[line039->line090]         "name": "Standard_LRS"
  }
  dependsOn: [
    singleResource
  ]
}]

// extension of a single resource in a collection
resource extendSingleResourceInCollection 'Microsoft.Authorization/locks@2016-09-01' = {
//@[line047->line096]     {
//@[line047->line097]       "type": "Microsoft.Authorization/locks",
//@[line047->line098]       "apiVersion": "2016-09-01",
//@[line047->line099]       "scope": "[format('Microsoft.Storage/storageAccounts/{0}', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[mod(parameters('index'), 2)].name))]",
//@[line047->line100]       "name": "one-resource-collection-item-lock",
//@[line047->line104]       "dependsOn": [
//@[line047->line105]         "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[mod(parameters('index'), 2)].name))]"
//@[line047->line106]       ]
//@[line047->line107]     },
  name: 'one-resource-collection-item-lock'
  properties: {
//@[line049->line101]       "properties": {
//@[line049->line103]       },
    level: 'ReadOnly'
//@[line050->line102]         "level": "ReadOnly"
  }
  scope: storageAccounts[index % 2]
}

// collection of extensions
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[line056->line108]     {
//@[line056->line109]       "copy": {
//@[line056->line110]         "name": "extensionCollection",
//@[line056->line111]         "count": "[length(range(0, 1))]"
//@[line056->line112]       },
//@[line056->line113]       "type": "Microsoft.Authorization/locks",
//@[line056->line114]       "apiVersion": "2016-09-01",
//@[line056->line115]       "scope": "[format('Microsoft.Storage/storageAccounts/{0}', format('{0}single-resource-name', parameters('name')))]",
//@[line056->line116]       "name": "[format('lock-{0}', range(0, 1)[copyIndex()])]",
//@[line056->line120]       "dependsOn": [
//@[line056->line121]         "[resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name')))]"
//@[line056->line122]       ]
//@[line056->line123]     },
  name: 'lock-${i}'
  properties: {
//@[line058->line117]       "properties": {
//@[line058->line119]       },
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[line059->line118]         "level": "[if(equals(range(0, 1)[copyIndex()], 0), 'CanNotDelete', 'ReadOnly')]"
  }
  scope: singleResource
}]

// cascade extend the extension
@batchSize(1)
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[line066->line124]     {
//@[line066->line125]       "copy": {
//@[line066->line126]         "name": "lockTheLocks",
//@[line066->line127]         "count": "[length(range(0, 1))]",
//@[line066->line128]         "mode": "serial",
//@[line066->line129]         "batchSize": 1
//@[line066->line130]       },
//@[line066->line131]       "type": "Microsoft.Authorization/locks",
//@[line066->line132]       "apiVersion": "2016-09-01",
//@[line066->line133]       "scope": "[extensionResourceId(format('Microsoft.Storage/storageAccounts/{0}', format('{0}single-resource-name', parameters('name'))), 'Microsoft.Authorization/locks', format('lock-{0}', range(0, 1)[range(0, 1)[copyIndex()]]))]",
//@[line066->line134]       "name": "[format('lock-the-lock-{0}', range(0, 1)[copyIndex()])]",
//@[line066->line138]       "dependsOn": [
//@[line066->line139]         "[extensionResourceId(resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name'))), 'Microsoft.Authorization/locks', format('lock-{0}', range(0, 1)[range(0, 1)[copyIndex()]]))]"
//@[line066->line140]       ]
//@[line066->line141]     },
  name: 'lock-the-lock-${i}'
  properties: {
//@[line068->line135]       "properties": {
//@[line068->line137]       },
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[line069->line136]         "level": "[if(equals(range(0, 1)[copyIndex()], 0), 'CanNotDelete', 'ReadOnly')]"
  }
  scope: extensionCollection[i]
}]

// special case property access
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
//@[line075->line783]     "indexedCollectionBlobEndpoint": {
//@[line075->line784]       "type": "string",
//@[line075->line785]       "value": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name)), '2019-06-01').primaryEndpoints.blob]"
//@[line075->line786]     },
output indexedCollectionName string = storageAccounts[index].name
//@[line076->line787]     "indexedCollectionName": {
//@[line076->line788]       "type": "string",
//@[line076->line789]       "value": "[format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name)]"
//@[line076->line790]     },
output indexedCollectionId string = storageAccounts[index].id
//@[line077->line791]     "indexedCollectionId": {
//@[line077->line792]       "type": "string",
//@[line077->line793]       "value": "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name))]"
//@[line077->line794]     },
output indexedCollectionType string = storageAccounts[index].type
//@[line078->line795]     "indexedCollectionType": {
//@[line078->line796]       "type": "string",
//@[line078->line797]       "value": "Microsoft.Storage/storageAccounts"
//@[line078->line798]     },
output indexedCollectionVersion string = storageAccounts[index].apiVersion
//@[line079->line799]     "indexedCollectionVersion": {
//@[line079->line800]       "type": "string",
//@[line079->line801]       "value": "2019-06-01"
//@[line079->line802]     },

// general case property access
output indexedCollectionIdentity object = storageAccounts[index].identity
//@[line082->line803]     "indexedCollectionIdentity": {
//@[line082->line804]       "type": "object",
//@[line082->line805]       "value": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name)), '2019-06-01', 'full').identity]"
//@[line082->line806]     },

// indexed access of two properties
output indexedEndpointPair object = {
//@[line085->line807]     "indexedEndpointPair": {
//@[line085->line808]       "type": "object",
//@[line085->line809]       "value": {
//@[line085->line812]       }
//@[line085->line813]     },
  primary: storageAccounts[index].properties.primaryEndpoints.blob
//@[line086->line810]         "primary": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name)), '2019-06-01').primaryEndpoints.blob]",
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
//@[line087->line811]         "secondary": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[add(parameters('index'), 1)].name)), '2019-06-01').secondaryEndpoints.blob]"
}

// nested indexer?
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier
//@[line091->line814]     "indexViaReference": {
//@[line091->line815]       "type": "string",
//@[line091->line816]       "value": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[int(reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name)), '2019-06-01').creationTime)].name)), '2019-06-01').accessTier]"
//@[line091->line817]     },

// dependency on a resource collection
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[line094->line142]     {
//@[line094->line143]       "copy": {
//@[line094->line144]         "name": "storageAccounts2",
//@[line094->line145]         "count": "[length(parameters('accounts'))]"
//@[line094->line146]       },
//@[line094->line147]       "type": "Microsoft.Storage/storageAccounts",
//@[line094->line148]       "apiVersion": "2019-06-01",
//@[line094->line149]       "name": "[format('{0}-collection-{1}', parameters('name'), parameters('accounts')[copyIndex()].name)]",
//@[line094->line155]       "dependsOn": [
//@[line094->line156]         "storageAccounts"
//@[line094->line157]       ]
//@[line094->line158]     },
  name: '${name}-collection-${account.name}'
  location: account.location
//@[line096->line150]       "location": "[parameters('accounts')[copyIndex()].location]",
  kind: 'StorageV2'
//@[line097->line151]       "kind": "StorageV2",
  sku: {
//@[line098->line152]       "sku": {
//@[line098->line154]       },
    name: 'Standard_LRS'
//@[line099->line153]         "name": "Standard_LRS"
  }
  dependsOn: [
    storageAccounts
  ]
}]

// one-to-one paired dependencies
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[line107->line159]     {
//@[line107->line160]       "copy": {
//@[line107->line161]         "name": "firstSet",
//@[line107->line162]         "count": "[length(range(0, length(parameters('accounts'))))]"
//@[line107->line163]       },
//@[line107->line164]       "type": "Microsoft.Storage/storageAccounts",
//@[line107->line165]       "apiVersion": "2019-06-01",
//@[line107->line166]       "name": "[format('{0}-set1-{1}', parameters('name'), range(0, length(parameters('accounts')))[copyIndex()])]",
//@[line107->line172]     },
  name: '${name}-set1-${i}'
  location: resourceGroup().location
//@[line109->line167]       "location": "[resourceGroup().location]",
  kind: 'StorageV2'
//@[line110->line168]       "kind": "StorageV2",
  sku: {
//@[line111->line169]       "sku": {
//@[line111->line171]       }
    name: 'Standard_LRS'
//@[line112->line170]         "name": "Standard_LRS"
  }
}]

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[line116->line173]     {
//@[line116->line174]       "copy": {
//@[line116->line175]         "name": "secondSet",
//@[line116->line176]         "count": "[length(range(0, length(parameters('accounts'))))]"
//@[line116->line177]       },
//@[line116->line178]       "type": "Microsoft.Storage/storageAccounts",
//@[line116->line179]       "apiVersion": "2019-06-01",
//@[line116->line180]       "name": "[format('{0}-set2-{1}', parameters('name'), range(0, length(parameters('accounts')))[copyIndex()])]",
//@[line116->line186]       "dependsOn": [
//@[line116->line187]         "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-set1-{1}', parameters('name'), range(0, length(parameters('accounts')))[range(0, length(parameters('accounts')))[copyIndex()]]))]"
//@[line116->line188]       ]
//@[line116->line189]     },
  name: '${name}-set2-${i}'
  location: resourceGroup().location
//@[line118->line181]       "location": "[resourceGroup().location]",
  kind: 'StorageV2'
//@[line119->line182]       "kind": "StorageV2",
  sku: {
//@[line120->line183]       "sku": {
//@[line120->line185]       },
    name: 'Standard_LRS'
//@[line121->line184]         "name": "Standard_LRS"
  }
  dependsOn: [
    firstSet[i]
  ]
}]

// depending on collection and one resource in the collection optimizes the latter part away
resource anotherSingleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[line129->line190]     {
//@[line129->line191]       "type": "Microsoft.Storage/storageAccounts",
//@[line129->line192]       "apiVersion": "2019-06-01",
//@[line129->line193]       "name": "[format('{0}single-resource-name', parameters('name'))]",
//@[line129->line199]       "dependsOn": [
//@[line129->line200]         "secondSet"
//@[line129->line201]       ]
//@[line129->line202]     },
  name: '${name}single-resource-name'
  location: resourceGroup().location
//@[line131->line194]       "location": "[resourceGroup().location]",
  kind: 'StorageV2'
//@[line132->line195]       "kind": "StorageV2",
  sku: {
//@[line133->line196]       "sku": {
//@[line133->line198]       },
    name: 'Standard_LRS'
//@[line134->line197]         "name": "Standard_LRS"
  }
  dependsOn: [
    secondSet
    secondSet[0]
  ]
}

// vnets
var vnetConfigurations = [
//@[line143->line022]     "vnetConfigurations": [
//@[line143->line031]     ],
  {
//@[line144->line023]       {
//@[line144->line026]       },
    name: 'one'
//@[line145->line024]         "name": "one",
    location: resourceGroup().location
//@[line146->line025]         "location": "[resourceGroup().location]"
  }
  {
//@[line148->line027]       {
//@[line148->line030]       }
    name: 'two'
//@[line149->line028]         "name": "two",
    location: 'westus'
//@[line150->line029]         "location": "westus"
  }
]

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for vnetConfig in vnetConfigurations: {
//@[line154->line203]     {
//@[line154->line204]       "copy": {
//@[line154->line205]         "name": "vnets",
//@[line154->line206]         "count": "[length(variables('vnetConfigurations'))]"
//@[line154->line207]       },
//@[line154->line208]       "type": "Microsoft.Network/virtualNetworks",
//@[line154->line209]       "apiVersion": "2020-06-01",
//@[line154->line210]       "name": "[variables('vnetConfigurations')[copyIndex()].name]",
//@[line154->line212]     },
  name: vnetConfig.name
  location: vnetConfig.location
//@[line156->line211]       "location": "[variables('vnetConfigurations')[copyIndex()].location]"
}]

// implicit dependency on single resource from a resource collection
resource implicitDependencyOnSingleResourceByIndex 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[line160->line213]     {
//@[line160->line214]       "type": "Microsoft.Network/dnsZones",
//@[line160->line215]       "apiVersion": "2018-05-01",
//@[line160->line216]       "name": "test",
//@[line160->line225]       "dependsOn": [
//@[line160->line226]         "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[add(parameters('index'), 1)].name)]"
//@[line160->line227]       ]
//@[line160->line228]     },
  name: 'test'
  location: 'global'
//@[line162->line217]       "location": "global",
  properties: {
//@[line163->line218]       "properties": {
//@[line163->line224]       },
    resolutionVirtualNetworks: [
//@[line164->line219]         "resolutionVirtualNetworks": [
//@[line164->line223]         ]
      {
//@[line165->line220]           {
//@[line165->line222]           }
        id: vnets[index+1].id
//@[line166->line221]             "id": "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[add(parameters('index'), 1)].name)]"
      }
    ]
  }
}

// implicit and explicit dependency combined
resource combinedDependencies 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[line173->line229]     {
//@[line173->line230]       "type": "Microsoft.Network/dnsZones",
//@[line173->line231]       "apiVersion": "2018-05-01",
//@[line173->line232]       "name": "test2",
//@[line173->line244]       "dependsOn": [
//@[line173->line245]         "vnets"
//@[line173->line246]       ]
//@[line173->line247]     },
  name: 'test2'
  location: 'global'
//@[line175->line233]       "location": "global",
  properties: {
//@[line176->line234]       "properties": {
//@[line176->line243]       },
    resolutionVirtualNetworks: [
//@[line177->line235]         "resolutionVirtualNetworks": [
//@[line177->line242]         ]
      {
//@[line178->line236]           {
//@[line178->line238]           },
        id: vnets[index-1].id
//@[line179->line237]             "id": "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[sub(parameters('index'), 1)].name)]"
      }
      {
//@[line181->line239]           {
//@[line181->line241]           }
        id: vnets[index * 2].id
//@[line182->line240]             "id": "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[mul(parameters('index'), 2)].name)]"
      }
    ]
  }
  dependsOn: [
    vnets
  ]
}

// single module
module singleModule 'passthrough.bicep' = {
//@[line192->line423]     {
//@[line192->line424]       "type": "Microsoft.Resources/deployments",
//@[line192->line425]       "apiVersion": "2020-10-01",
//@[line192->line427]       "properties": {
//@[line192->line428]         "expressionEvaluationOptions": {
//@[line192->line429]           "scope": "inner"
//@[line192->line430]         },
//@[line192->line431]         "mode": "Incremental",
//@[line192->line432]         "parameters": {
//@[line192->line433]           "myInput": {
//@[line192->line435]           }
//@[line192->line436]         },
//@[line192->line437]         "template": {
//@[line192->line438]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line192->line439]           "contentVersion": "1.0.0.0",
//@[line192->line440]           "metadata": {
//@[line192->line441]             "_generator": {
//@[line192->line442]               "name": "bicep",
//@[line192->line443]               "version": "dev",
//@[line192->line444]               "templateHash": "14375999048727010492"
//@[line192->line445]             }
//@[line192->line446]           },
//@[line192->line447]           "parameters": {
//@[line192->line448]             "myInput": {
//@[line192->line449]               "type": "string"
//@[line192->line450]             }
//@[line192->line451]           },
//@[line192->line452]           "resources": [],
//@[line192->line453]           "outputs": {
//@[line192->line454]             "myOutput": {
//@[line192->line455]               "type": "string",
//@[line192->line456]               "value": "[parameters('myInput')]"
//@[line192->line457]             }
//@[line192->line458]           }
//@[line192->line459]         }
//@[line192->line460]       }
//@[line192->line461]     },
  name: 'test'
//@[line193->line426]       "name": "test",
  params: {
    myInput: 'hello'
//@[line195->line434]             "value": "hello"
  }
}

var moduleSetup = [
//@[line199->line032]     "moduleSetup": [
//@[line199->line036]     ],
  'one'
//@[line200->line033]       "one",
  'two'
//@[line201->line034]       "two",
  'three'
//@[line202->line035]       "three"
]

// module collection plus explicit dependency on single module
@sys.batchSize(3)
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[line207->line462]     {
//@[line207->line463]       "copy": {
//@[line207->line464]         "name": "moduleCollectionWithSingleDependency",
//@[line207->line465]         "count": "[length(variables('moduleSetup'))]",
//@[line207->line466]         "mode": "serial",
//@[line207->line467]         "batchSize": 3
//@[line207->line468]       },
//@[line207->line469]       "type": "Microsoft.Resources/deployments",
//@[line207->line470]       "apiVersion": "2020-10-01",
//@[line207->line472]       "properties": {
//@[line207->line473]         "expressionEvaluationOptions": {
//@[line207->line474]           "scope": "inner"
//@[line207->line475]         },
//@[line207->line476]         "mode": "Incremental",
//@[line207->line477]         "parameters": {
//@[line207->line478]           "myInput": {
//@[line207->line480]           }
//@[line207->line481]         },
//@[line207->line482]         "template": {
//@[line207->line483]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line207->line484]           "contentVersion": "1.0.0.0",
//@[line207->line485]           "metadata": {
//@[line207->line486]             "_generator": {
//@[line207->line487]               "name": "bicep",
//@[line207->line488]               "version": "dev",
//@[line207->line489]               "templateHash": "14375999048727010492"
//@[line207->line490]             }
//@[line207->line491]           },
//@[line207->line492]           "parameters": {
//@[line207->line493]             "myInput": {
//@[line207->line494]               "type": "string"
//@[line207->line495]             }
//@[line207->line496]           },
//@[line207->line497]           "resources": [],
//@[line207->line498]           "outputs": {
//@[line207->line499]             "myOutput": {
//@[line207->line500]               "type": "string",
//@[line207->line501]               "value": "[parameters('myInput')]"
//@[line207->line502]             }
//@[line207->line503]           }
//@[line207->line504]         }
//@[line207->line505]       },
//@[line207->line506]       "dependsOn": [
//@[line207->line507]         "[resourceId('Microsoft.Resources/deployments', 'test')]",
//@[line207->line508]         "[resourceId('Microsoft.Storage/storageAccounts', format('{0}single-resource-name', parameters('name')))]"
//@[line207->line509]       ]
//@[line207->line510]     },
  name: moduleName
//@[line208->line471]       "name": "[variables('moduleSetup')[copyIndex()]]",
  params: {
    myInput: 'in-${moduleName}'
//@[line210->line479]             "value": "[format('in-{0}', variables('moduleSetup')[copyIndex()])]"
  }
  dependsOn: [
    singleModule
    singleResource
  ]
}]

// another module collection with dependency on another module collection
module moduleCollectionWithCollectionDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[line219->line511]     {
//@[line219->line512]       "copy": {
//@[line219->line513]         "name": "moduleCollectionWithCollectionDependencies",
//@[line219->line514]         "count": "[length(variables('moduleSetup'))]"
//@[line219->line515]       },
//@[line219->line516]       "type": "Microsoft.Resources/deployments",
//@[line219->line517]       "apiVersion": "2020-10-01",
//@[line219->line519]       "properties": {
//@[line219->line520]         "expressionEvaluationOptions": {
//@[line219->line521]           "scope": "inner"
//@[line219->line522]         },
//@[line219->line523]         "mode": "Incremental",
//@[line219->line524]         "parameters": {
//@[line219->line525]           "myInput": {
//@[line219->line527]           }
//@[line219->line528]         },
//@[line219->line529]         "template": {
//@[line219->line530]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line219->line531]           "contentVersion": "1.0.0.0",
//@[line219->line532]           "metadata": {
//@[line219->line533]             "_generator": {
//@[line219->line534]               "name": "bicep",
//@[line219->line535]               "version": "dev",
//@[line219->line536]               "templateHash": "14375999048727010492"
//@[line219->line537]             }
//@[line219->line538]           },
//@[line219->line539]           "parameters": {
//@[line219->line540]             "myInput": {
//@[line219->line541]               "type": "string"
//@[line219->line542]             }
//@[line219->line543]           },
//@[line219->line544]           "resources": [],
//@[line219->line545]           "outputs": {
//@[line219->line546]             "myOutput": {
//@[line219->line547]               "type": "string",
//@[line219->line548]               "value": "[parameters('myInput')]"
//@[line219->line549]             }
//@[line219->line550]           }
//@[line219->line551]         }
//@[line219->line552]       },
//@[line219->line553]       "dependsOn": [
//@[line219->line554]         "moduleCollectionWithSingleDependency",
//@[line219->line555]         "storageAccounts"
//@[line219->line556]       ]
//@[line219->line557]     },
  name: moduleName
//@[line220->line518]       "name": "[variables('moduleSetup')[copyIndex()]]",
  params: {
    myInput: 'in-${moduleName}'
//@[line222->line526]             "value": "[format('in-{0}', variables('moduleSetup')[copyIndex()])]"
  }
  dependsOn: [
    storageAccounts
    moduleCollectionWithSingleDependency
  ]
}]

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
//@[line230->line558]     {
//@[line230->line559]       "type": "Microsoft.Resources/deployments",
//@[line230->line560]       "apiVersion": "2020-10-01",
//@[line230->line562]       "properties": {
//@[line230->line563]         "expressionEvaluationOptions": {
//@[line230->line564]           "scope": "inner"
//@[line230->line565]         },
//@[line230->line566]         "mode": "Incremental",
//@[line230->line567]         "parameters": {
//@[line230->line568]           "myInput": {
//@[line230->line570]           }
//@[line230->line571]         },
//@[line230->line572]         "template": {
//@[line230->line573]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line230->line574]           "contentVersion": "1.0.0.0",
//@[line230->line575]           "metadata": {
//@[line230->line576]             "_generator": {
//@[line230->line577]               "name": "bicep",
//@[line230->line578]               "version": "dev",
//@[line230->line579]               "templateHash": "14375999048727010492"
//@[line230->line580]             }
//@[line230->line581]           },
//@[line230->line582]           "parameters": {
//@[line230->line583]             "myInput": {
//@[line230->line584]               "type": "string"
//@[line230->line585]             }
//@[line230->line586]           },
//@[line230->line587]           "resources": [],
//@[line230->line588]           "outputs": {
//@[line230->line589]             "myOutput": {
//@[line230->line590]               "type": "string",
//@[line230->line591]               "value": "[parameters('myInput')]"
//@[line230->line592]             }
//@[line230->line593]           }
//@[line230->line594]         }
//@[line230->line595]       },
//@[line230->line596]       "dependsOn": [
//@[line230->line597]         "[resourceId('Microsoft.Resources/deployments', variables('moduleSetup')[parameters('index')])]",
//@[line230->line598]         "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[mul(parameters('index'), 3)].name))]",
//@[line230->line599]         "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[sub(parameters('index'), 10)].name))]"
//@[line230->line600]       ]
//@[line230->line601]     },
  name: 'hello'
//@[line231->line561]       "name": "hello",
  params: {
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
//@[line233->line569]             "value": "[concat(reference(resourceId('Microsoft.Resources/deployments', variables('moduleSetup')[parameters('index')]), '2020-10-01').outputs.myOutput.value, reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[mul(parameters('index'), 3)].name)), '2019-06-01').accessTier)]"
  }
  dependsOn: [
    storageAccounts2[index - 10]
  ]
}

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[line240->line602]     {
//@[line240->line603]       "copy": {
//@[line240->line604]         "name": "moduleCollectionWithIndexedDependencies",
//@[line240->line605]         "count": "[length(variables('moduleSetup'))]"
//@[line240->line606]       },
//@[line240->line607]       "type": "Microsoft.Resources/deployments",
//@[line240->line608]       "apiVersion": "2020-10-01",
//@[line240->line610]       "properties": {
//@[line240->line611]         "expressionEvaluationOptions": {
//@[line240->line612]           "scope": "inner"
//@[line240->line613]         },
//@[line240->line614]         "mode": "Incremental",
//@[line240->line615]         "parameters": {
//@[line240->line616]           "myInput": {
//@[line240->line618]           }
//@[line240->line619]         },
//@[line240->line620]         "template": {
//@[line240->line621]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line240->line622]           "contentVersion": "1.0.0.0",
//@[line240->line623]           "metadata": {
//@[line240->line624]             "_generator": {
//@[line240->line625]               "name": "bicep",
//@[line240->line626]               "version": "dev",
//@[line240->line627]               "templateHash": "14375999048727010492"
//@[line240->line628]             }
//@[line240->line629]           },
//@[line240->line630]           "parameters": {
//@[line240->line631]             "myInput": {
//@[line240->line632]               "type": "string"
//@[line240->line633]             }
//@[line240->line634]           },
//@[line240->line635]           "resources": [],
//@[line240->line636]           "outputs": {
//@[line240->line637]             "myOutput": {
//@[line240->line638]               "type": "string",
//@[line240->line639]               "value": "[parameters('myInput')]"
//@[line240->line640]             }
//@[line240->line641]           }
//@[line240->line642]         }
//@[line240->line643]       },
//@[line240->line644]       "dependsOn": [
//@[line240->line645]         "[resourceId('Microsoft.Resources/deployments', variables('moduleSetup')[parameters('index')])]",
//@[line240->line646]         "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[mul(parameters('index'), 3)].name))]",
//@[line240->line647]         "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[sub(parameters('index'), 9)].name))]"
//@[line240->line648]       ]
//@[line240->line649]     },
  name: moduleName
//@[line241->line609]       "name": "[variables('moduleSetup')[copyIndex()]]",
  params: {
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName}'
//@[line243->line617]             "value": "[format('{0} - {1} - {2}', reference(resourceId('Microsoft.Resources/deployments', variables('moduleSetup')[parameters('index')]), '2020-10-01').outputs.myOutput.value, reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[mul(parameters('index'), 3)].name)), '2019-06-01').accessTier, variables('moduleSetup')[copyIndex()])]"
  }
  dependsOn: [
    storageAccounts2[index - 9]
  ]
}]

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
//@[line250->line818]     "indexedModulesName": {
//@[line250->line819]       "type": "string",
//@[line250->line820]       "value": "[variables('moduleSetup')[parameters('index')]]"
//@[line250->line821]     },
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput
//@[line251->line822]     "indexedModuleOutput": {
//@[line251->line823]       "type": "string",
//@[line251->line824]       "value": "[reference(resourceId('Microsoft.Resources/deployments', variables('moduleSetup')[mul(parameters('index'), 1)]), '2020-10-01').outputs.myOutput.value]"
//@[line251->line825]     },

// resource collection
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for account in accounts: {
  name: '${name}-existing-${account.name}'
}]

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
//@[line258->line826]     "existingIndexedResourceName": {
//@[line258->line827]       "type": "string",
//@[line258->line828]       "value": "[format('{0}-existing-{1}', parameters('name'), parameters('accounts')[mul(parameters('index'), 0)].name)]"
//@[line258->line829]     },
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
//@[line259->line830]     "existingIndexedResourceId": {
//@[line259->line831]       "type": "string",
//@[line259->line832]       "value": "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-existing-{1}', parameters('name'), parameters('accounts')[mul(parameters('index'), 1)].name))]"
//@[line259->line833]     },
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
//@[line260->line834]     "existingIndexedResourceType": {
//@[line260->line835]       "type": "string",
//@[line260->line836]       "value": "Microsoft.Storage/storageAccounts"
//@[line260->line837]     },
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
//@[line261->line838]     "existingIndexedResourceApiVersion": {
//@[line261->line839]       "type": "string",
//@[line261->line840]       "value": "2019-06-01"
//@[line261->line841]     },
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
//@[line262->line842]     "existingIndexedResourceLocation": {
//@[line262->line843]       "type": "string",
//@[line262->line844]       "value": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-existing-{1}', parameters('name'), parameters('accounts')[div(parameters('index'), 2)].name)), '2019-06-01', 'full').location]"
//@[line262->line845]     },
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier
//@[line263->line846]     "existingIndexedResourceAccessTier": {
//@[line263->line847]       "type": "string",
//@[line263->line848]       "value": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-existing-{1}', parameters('name'), parameters('accounts')[mod(parameters('index'), 3)].name)), '2019-06-01').accessTier]"
//@[line263->line849]     },

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[line265->line248]     {
//@[line265->line249]       "copy": {
//@[line265->line250]         "name": "duplicatedNames",
//@[line265->line251]         "count": "[length(createArray())]"
//@[line265->line252]       },
//@[line265->line253]       "type": "Microsoft.Network/dnsZones",
//@[line265->line254]       "apiVersion": "2018-05-01",
//@[line265->line255]       "name": "no loop variable",
//@[line265->line257]     },
  name: 'no loop variable'
  location: 'eastus'
//@[line267->line256]       "location": "eastus"
}]

// reference to a resource collection whose name expression does not reference any loop variables
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[line271->line258]     {
//@[line271->line259]       "copy": {
//@[line271->line260]         "name": "referenceToDuplicateNames",
//@[line271->line261]         "count": "[length(createArray())]"
//@[line271->line262]       },
//@[line271->line263]       "type": "Microsoft.Network/dnsZones",
//@[line271->line264]       "apiVersion": "2018-05-01",
//@[line271->line265]       "name": "no loop variable 2",
//@[line271->line267]       "dependsOn": [
//@[line271->line268]         "[resourceId('Microsoft.Network/dnsZones', 'no loop variable')]"
//@[line271->line269]       ]
//@[line271->line270]     },
  name: 'no loop variable 2'
  location: 'eastus'
//@[line273->line266]       "location": "eastus",
  dependsOn: [
    duplicatedNames[index]
  ]
}]

var regions = [
//@[line279->line037]     "regions": [
//@[line279->line040]     ]
  'eastus'
//@[line280->line038]       "eastus",
  'westus'
//@[line281->line039]       "westus"
]

module apim 'passthrough.bicep' = [for region in regions: {
//@[line284->line650]     {
//@[line284->line651]       "copy": {
//@[line284->line652]         "name": "apim",
//@[line284->line653]         "count": "[length(variables('regions'))]"
//@[line284->line654]       },
//@[line284->line655]       "type": "Microsoft.Resources/deployments",
//@[line284->line656]       "apiVersion": "2020-10-01",
//@[line284->line658]       "properties": {
//@[line284->line659]         "expressionEvaluationOptions": {
//@[line284->line660]           "scope": "inner"
//@[line284->line661]         },
//@[line284->line662]         "mode": "Incremental",
//@[line284->line663]         "parameters": {
//@[line284->line664]           "myInput": {
//@[line284->line666]           }
//@[line284->line667]         },
//@[line284->line668]         "template": {
//@[line284->line669]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line284->line670]           "contentVersion": "1.0.0.0",
//@[line284->line671]           "metadata": {
//@[line284->line672]             "_generator": {
//@[line284->line673]               "name": "bicep",
//@[line284->line674]               "version": "dev",
//@[line284->line675]               "templateHash": "14375999048727010492"
//@[line284->line676]             }
//@[line284->line677]           },
//@[line284->line678]           "parameters": {
//@[line284->line679]             "myInput": {
//@[line284->line680]               "type": "string"
//@[line284->line681]             }
//@[line284->line682]           },
//@[line284->line683]           "resources": [],
//@[line284->line684]           "outputs": {
//@[line284->line685]             "myOutput": {
//@[line284->line686]               "type": "string",
//@[line284->line687]               "value": "[parameters('myInput')]"
//@[line284->line688]             }
//@[line284->line689]           }
//@[line284->line690]         }
//@[line284->line691]       }
//@[line284->line692]     },
  name: 'apim-${region}-${name}'
//@[line285->line657]       "name": "[format('apim-{0}-{1}', variables('regions')[copyIndex()], parameters('name'))]",
  params: {
    myInput: region
//@[line287->line665]             "value": "[variables('regions')[copyIndex()]]"
  }
}]

resource propertyLoopDependencyOnModuleCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[line291->line271]     {
//@[line291->line272]       "type": "Microsoft.Network/frontDoors",
//@[line291->line273]       "apiVersion": "2020-05-01",
//@[line291->line274]       "name": "[parameters('name')]",
//@[line291->line299]       "dependsOn": [
//@[line291->line300]         "apim"
//@[line291->line301]       ]
//@[line291->line302]     },
  name: name
  location: 'Global'
//@[line293->line275]       "location": "Global",
  properties: {
//@[line294->line276]       "properties": {
//@[line294->line298]       },
    backendPools: [
//@[line295->line277]         "backendPools": [
//@[line295->line297]         ]
      {
//@[line296->line278]           {
//@[line296->line296]           }
        name: 'BackendAPIMs'
//@[line297->line279]             "name": "BackendAPIMs",
        properties: {
//@[line298->line280]             "properties": {
//@[line298->line281]               "copy": [
//@[line298->line294]               ]
//@[line298->line295]             }
          backends: [for index in range(0, length(regions)): {
//@[line299->line282]                 {
//@[line299->line283]                   "name": "backends",
//@[line299->line284]                   "count": "[length(range(0, length(variables('regions'))))]",
//@[line299->line285]                   "input": {
//@[line299->line292]                   }
//@[line299->line293]                 }
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: apim[index].outputs.myOutput
//@[line303->line286]                     "address": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex('backends')]], parameters('name'))), '2020-10-01').outputs.myOutput.value]",
            backendHostHeader: apim[index].outputs.myOutput
//@[line304->line287]                     "backendHostHeader": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex('backends')]], parameters('name'))), '2020-10-01').outputs.myOutput.value]",
            httpPort: 80
//@[line305->line288]                     "httpPort": 80,
            httpsPort: 443
//@[line306->line289]                     "httpsPort": 443,
            priority: 1
//@[line307->line290]                     "priority": 1,
            weight: 50
//@[line308->line291]                     "weight": 50
          }]
        }
      }
    ]
  }
}

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(regions)): {
//@[line316->line303]     {
//@[line316->line304]       "copy": {
//@[line316->line305]         "name": "indexedModuleCollectionDependency",
//@[line316->line306]         "count": "[length(range(0, length(variables('regions'))))]"
//@[line316->line307]       },
//@[line316->line308]       "type": "Microsoft.Network/frontDoors",
//@[line316->line309]       "apiVersion": "2020-05-01",
//@[line316->line310]       "name": "[format('{0}-{1}', parameters('name'), range(0, length(variables('regions')))[copyIndex()])]",
//@[line316->line331]       "dependsOn": [
//@[line316->line332]         "[resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex()]], parameters('name')))]",
//@[line316->line333]         "[resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex()]], parameters('name')))]"
//@[line316->line334]       ]
//@[line316->line335]     },
  name: '${name}-${index}'
  location: 'Global'
//@[line318->line311]       "location": "Global",
  properties: {
//@[line319->line312]       "properties": {
//@[line319->line330]       },
    backendPools: [
//@[line320->line313]         "backendPools": [
//@[line320->line329]         ]
      {
//@[line321->line314]           {
//@[line321->line328]           }
        name: 'BackendAPIMs'
//@[line322->line315]             "name": "BackendAPIMs",
        properties: {
//@[line323->line316]             "properties": {
//@[line323->line327]             }
          backends: [
//@[line324->line317]               "backends": [
//@[line324->line326]               ]
            {
//@[line325->line318]                 {
//@[line325->line325]                 }
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: apim[index].outputs.myOutput
//@[line328->line319]                   "address": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex()]], parameters('name'))), '2020-10-01').outputs.myOutput.value]",
              backendHostHeader: apim[index].outputs.myOutput
//@[line329->line320]                   "backendHostHeader": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex()]], parameters('name'))), '2020-10-01').outputs.myOutput.value]",
              httpPort: 80
//@[line330->line321]                   "httpPort": 80,
              httpsPort: 443
//@[line331->line322]                   "httpsPort": 443,
              priority: 1
//@[line332->line323]                   "priority": 1,
              weight: 50
//@[line333->line324]                   "weight": 50
            }
          ]
        }
      }
    ]
  }
}]

resource propertyLoopDependencyOnResourceCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[line342->line336]     {
//@[line342->line337]       "type": "Microsoft.Network/frontDoors",
//@[line342->line338]       "apiVersion": "2020-05-01",
//@[line342->line339]       "name": "[parameters('name')]",
//@[line342->line364]       "dependsOn": [
//@[line342->line365]         "storageAccounts"
//@[line342->line366]       ]
//@[line342->line367]     },
  name: name
  location: 'Global'
//@[line344->line340]       "location": "Global",
  properties: {
//@[line345->line341]       "properties": {
//@[line345->line363]       },
    backendPools: [
//@[line346->line342]         "backendPools": [
//@[line346->line362]         ]
      {
//@[line347->line343]           {
//@[line347->line361]           }
        name: 'BackendAPIMs'
//@[line348->line344]             "name": "BackendAPIMs",
        properties: {
//@[line349->line345]             "properties": {
//@[line349->line346]               "copy": [
//@[line349->line359]               ]
//@[line349->line360]             }
          backends: [for index in range(0, length(accounts)): {
//@[line350->line347]                 {
//@[line350->line348]                   "name": "backends",
//@[line350->line349]                   "count": "[length(range(0, length(parameters('accounts'))))]",
//@[line350->line350]                   "input": {
//@[line350->line357]                   }
//@[line350->line358]                 }
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[line354->line351]                     "address": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex('backends')]].name)), '2019-06-01').primaryEndpoints.internetEndpoints.web]",
            backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[line355->line352]                     "backendHostHeader": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex('backends')]].name)), '2019-06-01').primaryEndpoints.internetEndpoints.web]",
            httpPort: 80
//@[line356->line353]                     "httpPort": 80,
            httpsPort: 443
//@[line357->line354]                     "httpsPort": 443,
            priority: 1
//@[line358->line355]                     "priority": 1,
            weight: 50
//@[line359->line356]                     "weight": 50
          }]
        }
      }
    ]
  }
}

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(accounts)): {
//@[line367->line368]     {
//@[line367->line369]       "copy": {
//@[line367->line370]         "name": "indexedResourceCollectionDependency",
//@[line367->line371]         "count": "[length(range(0, length(parameters('accounts'))))]"
//@[line367->line372]       },
//@[line367->line373]       "type": "Microsoft.Network/frontDoors",
//@[line367->line374]       "apiVersion": "2020-05-01",
//@[line367->line375]       "name": "[format('{0}-{1}', parameters('name'), range(0, length(parameters('accounts')))[copyIndex()])]",
//@[line367->line396]       "dependsOn": [
//@[line367->line397]         "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex()]].name))]",
//@[line367->line398]         "[resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex()]].name))]"
//@[line367->line399]       ]
//@[line367->line400]     },
  name: '${name}-${index}'
  location: 'Global'
//@[line369->line376]       "location": "Global",
  properties: {
//@[line370->line377]       "properties": {
//@[line370->line395]       },
    backendPools: [
//@[line371->line378]         "backendPools": [
//@[line371->line394]         ]
      {
//@[line372->line379]           {
//@[line372->line393]           }
        name: 'BackendAPIMs'
//@[line373->line380]             "name": "BackendAPIMs",
        properties: {
//@[line374->line381]             "properties": {
//@[line374->line392]             }
          backends: [
//@[line375->line382]               "backends": [
//@[line375->line391]               ]
            {
//@[line376->line383]                 {
//@[line376->line390]                 }
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[line379->line384]                   "address": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex()]].name)), '2019-06-01').primaryEndpoints.internetEndpoints.web]",
              backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[line380->line385]                   "backendHostHeader": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex()]].name)), '2019-06-01').primaryEndpoints.internetEndpoints.web]",
              httpPort: 80
//@[line381->line386]                   "httpPort": 80,
              httpsPort: 443
//@[line382->line387]                   "httpsPort": 443,
              priority: 1
//@[line383->line388]                   "priority": 1,
              weight: 50
//@[line384->line389]                   "weight": 50
            }
          ]
        }
      }
    ]
  }
}]

resource filteredZones 'Microsoft.Network/dnsZones@2018-05-01' = [for i in range(0,10): if(i % 3 == 0) {
//@[line393->line401]     {
//@[line393->line402]       "condition": "[equals(mod(range(0, 10)[copyIndex()], 3), 0)]",
//@[line393->line403]       "copy": {
//@[line393->line404]         "name": "filteredZones",
//@[line393->line405]         "count": "[length(range(0, 10))]"
//@[line393->line406]       },
//@[line393->line407]       "type": "Microsoft.Network/dnsZones",
//@[line393->line408]       "apiVersion": "2018-05-01",
//@[line393->line409]       "name": "[format('zone{0}', range(0, 10)[copyIndex()])]",
//@[line393->line411]     },
  name: 'zone${i}'
  location: resourceGroup().location
//@[line395->line410]       "location": "[resourceGroup().location]"
}]

module filteredModules 'passthrough.bicep' = [for i in range(0,6): if(i % 2 == 0) {
//@[line398->line693]     {
//@[line398->line694]       "condition": "[equals(mod(range(0, 6)[copyIndex()], 2), 0)]",
//@[line398->line695]       "copy": {
//@[line398->line696]         "name": "filteredModules",
//@[line398->line697]         "count": "[length(range(0, 6))]"
//@[line398->line698]       },
//@[line398->line699]       "type": "Microsoft.Resources/deployments",
//@[line398->line700]       "apiVersion": "2020-10-01",
//@[line398->line702]       "properties": {
//@[line398->line703]         "expressionEvaluationOptions": {
//@[line398->line704]           "scope": "inner"
//@[line398->line705]         },
//@[line398->line706]         "mode": "Incremental",
//@[line398->line707]         "parameters": {
//@[line398->line708]           "myInput": {
//@[line398->line710]           }
//@[line398->line711]         },
//@[line398->line712]         "template": {
//@[line398->line713]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line398->line714]           "contentVersion": "1.0.0.0",
//@[line398->line715]           "metadata": {
//@[line398->line716]             "_generator": {
//@[line398->line717]               "name": "bicep",
//@[line398->line718]               "version": "dev",
//@[line398->line719]               "templateHash": "14375999048727010492"
//@[line398->line720]             }
//@[line398->line721]           },
//@[line398->line722]           "parameters": {
//@[line398->line723]             "myInput": {
//@[line398->line724]               "type": "string"
//@[line398->line725]             }
//@[line398->line726]           },
//@[line398->line727]           "resources": [],
//@[line398->line728]           "outputs": {
//@[line398->line729]             "myOutput": {
//@[line398->line730]               "type": "string",
//@[line398->line731]               "value": "[parameters('myInput')]"
//@[line398->line732]             }
//@[line398->line733]           }
//@[line398->line734]         }
//@[line398->line735]       }
//@[line398->line736]     },
  name: 'stuff${i}'
//@[line399->line701]       "name": "[format('stuff{0}', range(0, 6)[copyIndex()])]",
  params: {
    myInput: 'script-${i}'
//@[line401->line709]             "value": "[format('script-{0}', range(0, 6)[copyIndex()])]"
  }
}]

resource filteredIndexedZones 'Microsoft.Network/dnsZones@2018-05-01' = [for (account, i) in accounts: if(account.enabled) {
//@[line405->line412]     {
//@[line405->line413]       "condition": "[parameters('accounts')[copyIndex()].enabled]",
//@[line405->line414]       "copy": {
//@[line405->line415]         "name": "filteredIndexedZones",
//@[line405->line416]         "count": "[length(parameters('accounts'))]"
//@[line405->line417]       },
//@[line405->line418]       "type": "Microsoft.Network/dnsZones",
//@[line405->line419]       "apiVersion": "2018-05-01",
//@[line405->line420]       "name": "[format('indexedZone-{0}-{1}', parameters('accounts')[copyIndex()].name, copyIndex())]",
//@[line405->line422]     },
  name: 'indexedZone-${account.name}-${i}'
  location: account.location
//@[line407->line421]       "location": "[parameters('accounts')[copyIndex()].location]"
}]

output lastNameServers array = filteredIndexedZones[length(accounts) - 1].properties.nameServers
//@[line410->line850]     "lastNameServers": {
//@[line410->line851]       "type": "array",
//@[line410->line852]       "value": "[reference(resourceId('Microsoft.Network/dnsZones', format('indexedZone-{0}-{1}', parameters('accounts')[sub(length(parameters('accounts')), 1)].name, sub(length(parameters('accounts')), 1))), '2018-05-01').nameServers]"
//@[line410->line853]     },

module filteredIndexedModules 'passthrough.bicep' = [for (account, i) in accounts: if(account.enabled) {
//@[line412->line737]     {
//@[line412->line738]       "condition": "[parameters('accounts')[copyIndex()].enabled]",
//@[line412->line739]       "copy": {
//@[line412->line740]         "name": "filteredIndexedModules",
//@[line412->line741]         "count": "[length(parameters('accounts'))]"
//@[line412->line742]       },
//@[line412->line743]       "type": "Microsoft.Resources/deployments",
//@[line412->line744]       "apiVersion": "2020-10-01",
//@[line412->line746]       "properties": {
//@[line412->line747]         "expressionEvaluationOptions": {
//@[line412->line748]           "scope": "inner"
//@[line412->line749]         },
//@[line412->line750]         "mode": "Incremental",
//@[line412->line751]         "parameters": {
//@[line412->line752]           "myInput": {
//@[line412->line754]           }
//@[line412->line755]         },
//@[line412->line756]         "template": {
//@[line412->line757]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line412->line758]           "contentVersion": "1.0.0.0",
//@[line412->line759]           "metadata": {
//@[line412->line760]             "_generator": {
//@[line412->line761]               "name": "bicep",
//@[line412->line762]               "version": "dev",
//@[line412->line763]               "templateHash": "14375999048727010492"
//@[line412->line764]             }
//@[line412->line765]           },
//@[line412->line766]           "parameters": {
//@[line412->line767]             "myInput": {
//@[line412->line768]               "type": "string"
//@[line412->line769]             }
//@[line412->line770]           },
//@[line412->line771]           "resources": [],
//@[line412->line772]           "outputs": {
//@[line412->line773]             "myOutput": {
//@[line412->line774]               "type": "string",
//@[line412->line775]               "value": "[parameters('myInput')]"
//@[line412->line776]             }
//@[line412->line777]           }
//@[line412->line778]         }
//@[line412->line779]       }
//@[line412->line780]     }
  name: 'stuff-${i}'
//@[line413->line745]       "name": "[format('stuff-{0}', copyIndex())]",
  params: {
    myInput: 'script-${account.name}-${i}'
//@[line415->line753]             "value": "[format('script-{0}-{1}', parameters('accounts')[copyIndex()].name, copyIndex())]"
  }
}]

output lastModuleOutput string = filteredIndexedModules[length(accounts) - 1].outputs.myOutput
//@[line419->line854]     "lastModuleOutput": {
//@[line419->line855]       "type": "string",
//@[line419->line856]       "value": "[reference(resourceId('Microsoft.Resources/deployments', format('stuff-{0}', sub(length(parameters('accounts')), 1))), '2020-10-01').outputs.myOutput.value]"
//@[line419->line857]     }

