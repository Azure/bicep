param name string
//@[11:769]     "name": {\r
param accounts array
//@[14:16]     "accounts": {\r
param index int
//@[17:776]     "index": {\r

// single resource
resource singleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[43:52]       "type": "Microsoft.Storage/storageAccounts",\r
  name: '${name}single-resource-name'
  location: resourceGroup().location
//@[47:47]       "location": "[resourceGroup().location]",\r
  kind: 'StorageV2'
//@[48:48]       "kind": "StorageV2",\r
  sku: {
//@[49:51]       "sku": {\r
    name: 'Standard_LRS'
//@[50:50]         "name": "Standard_LRS"\r
  }
}

// extension of single resource
resource singleResourceExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[53:64]       "type": "Microsoft.Authorization/locks",\r
  scope: singleResource
  name: 'single-resource-lock'
  properties: {
//@[58:60]       "properties": {\r
    level: 'CanNotDelete'
//@[59:59]         "level": "CanNotDelete"\r
  }
}

// single resource cascade extension
resource singleResourceCascadeExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[65:76]       "type": "Microsoft.Authorization/locks",\r
  scope: singleResourceExtension
  name: 'single-resource-cascade-extension'
  properties: {
//@[70:72]       "properties": {\r
    level: 'CanNotDelete'
//@[71:71]         "level": "CanNotDelete"\r
  }
}

// resource collection
@batchSize(42)
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[77:95]       "copy": {\r
  name: '${name}-collection-${account.name}'
  location: account.location
//@[87:87]       "location": "[parameters('accounts')[copyIndex()].location]",\r
  kind: 'StorageV2'
//@[88:88]       "kind": "StorageV2",\r
  sku: {
//@[89:91]       "sku": {\r
    name: 'Standard_LRS'
//@[90:90]         "name": "Standard_LRS"\r
  }
  dependsOn: [
    singleResource
  ]
}]

// extension of a single resource in a collection
resource extendSingleResourceInCollection 'Microsoft.Authorization/locks@2016-09-01' = {
//@[96:107]       "type": "Microsoft.Authorization/locks",\r
  name: 'one-resource-collection-item-lock'
  properties: {
//@[101:103]       "properties": {\r
    level: 'ReadOnly'
//@[102:102]         "level": "ReadOnly"\r
  }
  scope: storageAccounts[index % 2]
}

// collection of extensions
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[108:123]       "copy": {\r
  name: 'lock-${i}'
  properties: {
//@[117:119]       "properties": {\r
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[118:118]         "level": "[if(equals(range(0, 1)[copyIndex()], 0), 'CanNotDelete', 'ReadOnly')]"\r
  }
  scope: singleResource
}]

// cascade extend the extension
@batchSize(1)
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[124:141]       "copy": {\r
  name: 'lock-the-lock-${i}'
  properties: {
//@[135:137]       "properties": {\r
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[136:136]         "level": "[if(equals(range(0, 1)[copyIndex()], 0), 'CanNotDelete', 'ReadOnly')]"\r
  }
  scope: extensionCollection[i]
}]

// special case property access
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
//@[783:786]     "indexedCollectionBlobEndpoint": {\r
output indexedCollectionName string = storageAccounts[index].name
//@[787:790]     "indexedCollectionName": {\r
output indexedCollectionId string = storageAccounts[index].id
//@[791:794]     "indexedCollectionId": {\r
output indexedCollectionType string = storageAccounts[index].type
//@[795:798]     "indexedCollectionType": {\r
output indexedCollectionVersion string = storageAccounts[index].apiVersion
//@[799:802]     "indexedCollectionVersion": {\r

// general case property access
output indexedCollectionIdentity object = storageAccounts[index].identity
//@[803:806]     "indexedCollectionIdentity": {\r

// indexed access of two properties
output indexedEndpointPair object = {
//@[807:813]     "indexedEndpointPair": {\r
  primary: storageAccounts[index].properties.primaryEndpoints.blob
//@[810:810]         "primary": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name))).primaryEndpoints.blob]",\r
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
//@[811:811]         "secondary": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[add(parameters('index'), 1)].name))).secondaryEndpoints.blob]"\r
}

// nested indexer?
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier
//@[814:817]     "indexViaReference": {\r

// dependency on a resource collection
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[142:158]       "copy": {\r
  name: '${name}-collection-${account.name}'
  location: account.location
//@[150:150]       "location": "[parameters('accounts')[copyIndex()].location]",\r
  kind: 'StorageV2'
//@[151:151]       "kind": "StorageV2",\r
  sku: {
//@[152:154]       "sku": {\r
    name: 'Standard_LRS'
//@[153:153]         "name": "Standard_LRS"\r
  }
  dependsOn: [
    storageAccounts
  ]
}]

// one-to-one paired dependencies
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[159:172]       "copy": {\r
  name: '${name}-set1-${i}'
  location: resourceGroup().location
//@[167:167]       "location": "[resourceGroup().location]",\r
  kind: 'StorageV2'
//@[168:168]       "kind": "StorageV2",\r
  sku: {
//@[169:171]       "sku": {\r
    name: 'Standard_LRS'
//@[170:170]         "name": "Standard_LRS"\r
  }
}]

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[173:189]       "copy": {\r
  name: '${name}-set2-${i}'
  location: resourceGroup().location
//@[181:181]       "location": "[resourceGroup().location]",\r
  kind: 'StorageV2'
//@[182:182]       "kind": "StorageV2",\r
  sku: {
//@[183:185]       "sku": {\r
    name: 'Standard_LRS'
//@[184:184]         "name": "Standard_LRS"\r
  }
  dependsOn: [
    firstSet[i]
  ]
}]

// depending on collection and one resource in the collection optimizes the latter part away
resource anotherSingleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[190:202]       "type": "Microsoft.Storage/storageAccounts",\r
  name: '${name}single-resource-name'
  location: resourceGroup().location
//@[194:194]       "location": "[resourceGroup().location]",\r
  kind: 'StorageV2'
//@[195:195]       "kind": "StorageV2",\r
  sku: {
//@[196:198]       "sku": {\r
    name: 'Standard_LRS'
//@[197:197]         "name": "Standard_LRS"\r
  }
  dependsOn: [
    secondSet
    secondSet[0]
  ]
}

// vnets
var vnetConfigurations = [
//@[22:31]     "vnetConfigurations": [\r
  {
    name: 'one'
//@[24:24]         "name": "one",\r
    location: resourceGroup().location
//@[25:25]         "location": "[resourceGroup().location]"\r
  }
  {
    name: 'two'
//@[28:28]         "name": "two",\r
    location: 'westus'
//@[29:29]         "location": "westus"\r
  }
]

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for vnetConfig in vnetConfigurations: {
//@[203:212]       "copy": {\r
  name: vnetConfig.name
  location: vnetConfig.location
//@[211:211]       "location": "[variables('vnetConfigurations')[copyIndex()].location]"\r
}]

// implicit dependency on single resource from a resource collection
resource implicitDependencyOnSingleResourceByIndex 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[213:228]       "type": "Microsoft.Network/dnsZones",\r
  name: 'test'
  location: 'global'
//@[217:217]       "location": "global",\r
  properties: {
//@[218:224]       "properties": {\r
    resolutionVirtualNetworks: [
//@[219:223]         "resolutionVirtualNetworks": [\r
      {
        id: vnets[index+1].id
//@[221:221]             "id": "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[add(parameters('index'), 1)].name)]"\r
      }
    ]
  }
}

// implicit and explicit dependency combined
resource combinedDependencies 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[229:247]       "type": "Microsoft.Network/dnsZones",\r
  name: 'test2'
  location: 'global'
//@[233:233]       "location": "global",\r
  properties: {
//@[234:243]       "properties": {\r
    resolutionVirtualNetworks: [
//@[235:242]         "resolutionVirtualNetworks": [\r
      {
        id: vnets[index-1].id
//@[237:237]             "id": "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[sub(parameters('index'), 1)].name)]"\r
      }
      {
        id: vnets[index * 2].id
//@[240:240]             "id": "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[mul(parameters('index'), 2)].name)]"\r
      }
    ]
  }
  dependsOn: [
    vnets
  ]
}

// single module
module singleModule 'passthrough.bicep' = {
//@[423:461]       "type": "Microsoft.Resources/deployments",\r
  name: 'test'
//@[426:426]       "name": "test",\r
  params: {
    myInput: 'hello'
  }
}

var moduleSetup = [
//@[32:36]     "moduleSetup": [\r
  'one'
//@[33:33]       "one",\r
  'two'
//@[34:34]       "two",\r
  'three'
//@[35:35]       "three"\r
]

// module collection plus explicit dependency on single module
@sys.batchSize(3)
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[462:510]       "copy": {\r
  name: moduleName
//@[471:471]       "name": "[variables('moduleSetup')[copyIndex()]]",\r
  params: {
    myInput: 'in-${moduleName}'
  }
  dependsOn: [
    singleModule
    singleResource
  ]
}]

// another module collection with dependency on another module collection
module moduleCollectionWithCollectionDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[511:557]       "copy": {\r
  name: moduleName
//@[518:518]       "name": "[variables('moduleSetup')[copyIndex()]]",\r
  params: {
    myInput: 'in-${moduleName}'
  }
  dependsOn: [
    storageAccounts
    moduleCollectionWithSingleDependency
  ]
}]

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
//@[558:601]       "type": "Microsoft.Resources/deployments",\r
  name: 'hello'
//@[561:561]       "name": "hello",\r
  params: {
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
  }
  dependsOn: [
    storageAccounts2[index - 10]
  ]
}

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[602:649]       "copy": {\r
  name: moduleName
//@[609:609]       "name": "[variables('moduleSetup')[copyIndex()]]",\r
  params: {
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName}'
  }
  dependsOn: [
    storageAccounts2[index - 9]
  ]
}]

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
//@[818:821]     "indexedModulesName": {\r
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput
//@[822:825]     "indexedModuleOutput": {\r

// resource collection
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for account in accounts: {
  name: '${name}-existing-${account.name}'
}]

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
//@[826:829]     "existingIndexedResourceName": {\r
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
//@[830:833]     "existingIndexedResourceId": {\r
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
//@[834:837]     "existingIndexedResourceType": {\r
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
//@[838:841]     "existingIndexedResourceApiVersion": {\r
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
//@[842:845]     "existingIndexedResourceLocation": {\r
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier
//@[846:849]     "existingIndexedResourceAccessTier": {\r

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[248:257]       "copy": {\r
  name: 'no loop variable'
  location: 'eastus'
//@[256:256]       "location": "eastus"\r
}]

// reference to a resource collection whose name expression does not reference any loop variables
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[258:270]       "copy": {\r
  name: 'no loop variable 2'
  location: 'eastus'
//@[266:266]       "location": "eastus",\r
  dependsOn: [
    duplicatedNames[index]
  ]
}]

var regions = [
//@[37:40]     "regions": [\r
  'eastus'
//@[38:38]       "eastus",\r
  'westus'
//@[39:39]       "westus"\r
]

module apim 'passthrough.bicep' = [for region in regions: {
//@[650:692]       "copy": {\r
  name: 'apim-${region}-${name}'
//@[657:657]       "name": "[format('apim-{0}-{1}', variables('regions')[copyIndex()], parameters('name'))]",\r
  params: {
    myInput: region
  }
}]

resource propertyLoopDependencyOnModuleCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[271:302]       "type": "Microsoft.Network/frontDoors",\r
  name: name
  location: 'Global'
//@[275:275]       "location": "Global",\r
  properties: {
//@[276:298]       "properties": {\r
    backendPools: [
//@[277:297]         "backendPools": [\r
      {
        name: 'BackendAPIMs'
//@[279:279]             "name": "BackendAPIMs",\r
        properties: {
//@[280:295]             "properties": {\r
          backends: [for index in range(0, length(regions)): {
//@[282:293]                   "name": "backends",\r
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: apim[index].outputs.myOutput
//@[286:286]                     "address": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex('backends')]], parameters('name')))).outputs.myOutput.value]",\r
            backendHostHeader: apim[index].outputs.myOutput
//@[287:287]                     "backendHostHeader": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex('backends')]], parameters('name')))).outputs.myOutput.value]",\r
            httpPort: 80
//@[288:288]                     "httpPort": 80,\r
            httpsPort: 443
//@[289:289]                     "httpsPort": 443,\r
            priority: 1
//@[290:290]                     "priority": 1,\r
            weight: 50
//@[291:291]                     "weight": 50\r
          }]
        }
      }
    ]
  }
}

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(regions)): {
//@[303:335]       "copy": {\r
  name: '${name}-${index}'
  location: 'Global'
//@[311:311]       "location": "Global",\r
  properties: {
//@[312:330]       "properties": {\r
    backendPools: [
//@[313:329]         "backendPools": [\r
      {
        name: 'BackendAPIMs'
//@[315:315]             "name": "BackendAPIMs",\r
        properties: {
//@[316:327]             "properties": {\r
          backends: [
//@[317:326]               "backends": [\r
            {
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: apim[index].outputs.myOutput
//@[319:319]                   "address": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex()]], parameters('name')))).outputs.myOutput.value]",\r
              backendHostHeader: apim[index].outputs.myOutput
//@[320:320]                   "backendHostHeader": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex()]], parameters('name')))).outputs.myOutput.value]",\r
              httpPort: 80
//@[321:321]                   "httpPort": 80,\r
              httpsPort: 443
//@[322:322]                   "httpsPort": 443,\r
              priority: 1
//@[323:323]                   "priority": 1,\r
              weight: 50
//@[324:324]                   "weight": 50\r
            }
          ]
        }
      }
    ]
  }
}]

resource propertyLoopDependencyOnResourceCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[336:367]       "type": "Microsoft.Network/frontDoors",\r
  name: name
  location: 'Global'
//@[340:340]       "location": "Global",\r
  properties: {
//@[341:363]       "properties": {\r
    backendPools: [
//@[342:362]         "backendPools": [\r
      {
        name: 'BackendAPIMs'
//@[344:344]             "name": "BackendAPIMs",\r
        properties: {
//@[345:360]             "properties": {\r
          backends: [for index in range(0, length(accounts)): {
//@[347:358]                   "name": "backends",\r
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[351:351]                     "address": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex('backends')]].name))).primaryEndpoints.internetEndpoints.web]",\r
            backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[352:352]                     "backendHostHeader": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex('backends')]].name))).primaryEndpoints.internetEndpoints.web]",\r
            httpPort: 80
//@[353:353]                     "httpPort": 80,\r
            httpsPort: 443
//@[354:354]                     "httpsPort": 443,\r
            priority: 1
//@[355:355]                     "priority": 1,\r
            weight: 50
//@[356:356]                     "weight": 50\r
          }]
        }
      }
    ]
  }
}

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(accounts)): {
//@[368:400]       "copy": {\r
  name: '${name}-${index}'
  location: 'Global'
//@[376:376]       "location": "Global",\r
  properties: {
//@[377:395]       "properties": {\r
    backendPools: [
//@[378:394]         "backendPools": [\r
      {
        name: 'BackendAPIMs'
//@[380:380]             "name": "BackendAPIMs",\r
        properties: {
//@[381:392]             "properties": {\r
          backends: [
//@[382:391]               "backends": [\r
            {
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[384:384]                   "address": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex()]].name))).primaryEndpoints.internetEndpoints.web]",\r
              backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[385:385]                   "backendHostHeader": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex()]].name))).primaryEndpoints.internetEndpoints.web]",\r
              httpPort: 80
//@[386:386]                   "httpPort": 80,\r
              httpsPort: 443
//@[387:387]                   "httpsPort": 443,\r
              priority: 1
//@[388:388]                   "priority": 1,\r
              weight: 50
//@[389:389]                   "weight": 50\r
            }
          ]
        }
      }
    ]
  }
}]

resource filteredZones 'Microsoft.Network/dnsZones@2018-05-01' = [for i in range(0,10): if(i % 3 == 0) {
//@[401:411]       "condition": "[equals(mod(range(0, 10)[copyIndex()], 3), 0)]",\r
  name: 'zone${i}'
  location: resourceGroup().location
//@[410:410]       "location": "[resourceGroup().location]"\r
}]

module filteredModules 'passthrough.bicep' = [for i in range(0,6): if(i % 2 == 0) {
//@[693:736]       "condition": "[equals(mod(range(0, 6)[copyIndex()], 2), 0)]",\r
  name: 'stuff${i}'
//@[701:701]       "name": "[format('stuff{0}', range(0, 6)[copyIndex()])]",\r
  params: {
    myInput: 'script-${i}'
  }
}]

resource filteredIndexedZones 'Microsoft.Network/dnsZones@2018-05-01' = [for (account, i) in accounts: if(account.enabled) {
//@[412:422]       "condition": "[parameters('accounts')[copyIndex()].enabled]",\r
  name: 'indexedZone-${account.name}-${i}'
  location: account.location
//@[421:421]       "location": "[parameters('accounts')[copyIndex()].location]"\r
}]

output lastNameServers array = filteredIndexedZones[length(accounts) - 1].properties.nameServers
//@[850:853]     "lastNameServers": {\r

module filteredIndexedModules 'passthrough.bicep' = [for (account, i) in accounts: if(account.enabled) {
//@[737:780]       "condition": "[parameters('accounts')[copyIndex()].enabled]",\r
  name: 'stuff-${i}'
//@[745:745]       "name": "[format('stuff-{0}', copyIndex())]",\r
  params: {
    myInput: 'script-${account.name}-${i}'
  }
}]

output lastModuleOutput string = filteredIndexedModules[length(accounts) - 1].outputs.myOutput
//@[854:857]     "lastModuleOutput": {\r

