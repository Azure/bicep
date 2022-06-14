param name string
//@[11:659]     "name": {\r
param accounts array
//@[14:16]     "accounts": {\r
param index int
//@[17:666]     "index": {\r

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
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, index) in accounts: {
//@[77:95]       "copy": {\r
  name: '${name}-collection-${account.name}-${index}'
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
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[108:123]       "copy": {\r
  name: 'lock-${i}-${i2}'
  properties: {
//@[117:119]       "properties": {\r
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[118:118]         "level": "[if(and(equals(range(0, 1)[copyIndex()], 0), equals(copyIndex(), 0)), 'CanNotDelete', 'ReadOnly')]"\r
  }
  scope: singleResource
}]

// cascade extend the extension
@batchSize(1)
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[124:141]       "copy": {\r
  name: 'lock-the-lock-${i}-${i2}'
  properties: {
//@[135:137]       "properties": {\r
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[136:136]         "level": "[if(and(equals(range(0, 1)[copyIndex()], 0), equals(copyIndex(), 0)), 'CanNotDelete', 'ReadOnly')]"\r
  }
  scope: extensionCollection[i2]
}]

// special case property access
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
//@[673:676]     "indexedCollectionBlobEndpoint": {\r
output indexedCollectionName string = storageAccounts[index].name
//@[677:680]     "indexedCollectionName": {\r
output indexedCollectionId string = storageAccounts[index].id
//@[681:684]     "indexedCollectionId": {\r
output indexedCollectionType string = storageAccounts[index].type
//@[685:688]     "indexedCollectionType": {\r
output indexedCollectionVersion string = storageAccounts[index].apiVersion
//@[689:692]     "indexedCollectionVersion": {\r

// general case property access
output indexedCollectionIdentity object = storageAccounts[index].identity
//@[693:696]     "indexedCollectionIdentity": {\r

// indexed access of two properties
output indexedEndpointPair object = {
//@[697:703]     "indexedEndpointPair": {\r
  primary: storageAccounts[index].properties.primaryEndpoints.blob
//@[700:700]         "primary": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}-{2}', parameters('name'), parameters('accounts')[parameters('index')].name, parameters('index')))).primaryEndpoints.blob]",\r
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
//@[701:701]         "secondary": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}-{2}', parameters('name'), parameters('accounts')[add(parameters('index'), 1)].name, add(parameters('index'), 1)))).secondaryEndpoints.blob]"\r
}

// nested indexer?
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier
//@[704:707]     "indexViaReference": {\r

// dependency on a resource collection
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, idx) in accounts: {
//@[142:158]       "copy": {\r
  name: '${name}-collection-${account.name}-${idx}'
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
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,ii) in range(0, length(accounts)): {
//@[159:172]       "copy": {\r
  name: '${name}-set1-${i}-${ii}'
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

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,iii) in range(0, length(accounts)): {
//@[173:189]       "copy": {\r
  name: '${name}-set2-${i}-${iii}'
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
    firstSet[iii]
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

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (vnetConfig, index) in vnetConfigurations: {
//@[203:212]       "copy": {\r
  name: '${vnetConfig.name}-${index}'
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
//@[221:221]             "id": "[resourceId('Microsoft.Network/virtualNetworks', format('{0}-{1}', variables('vnetConfigurations')[add(parameters('index'), 1)].name, add(parameters('index'), 1)))]"\r
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
//@[237:237]             "id": "[resourceId('Microsoft.Network/virtualNetworks', format('{0}-{1}', variables('vnetConfigurations')[sub(parameters('index'), 1)].name, sub(parameters('index'), 1)))]"\r
      }
      {
        id: vnets[index * 2].id
//@[240:240]             "id": "[resourceId('Microsoft.Network/virtualNetworks', format('{0}-{1}', variables('vnetConfigurations')[mul(parameters('index'), 2)].name, mul(parameters('index'), 2)))]"\r
      }
    ]
  }
  dependsOn: [
    vnets
  ]
}

// single module
module singleModule 'passthrough.bicep' = {
//@[401:439]       "type": "Microsoft.Resources/deployments",\r
  name: 'test'
//@[404:404]       "name": "test",\r
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
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[440:488]       "copy": {\r
  name: concat(moduleName, moduleIndex)
//@[449:449]       "name": "[concat(variables('moduleSetup')[copyIndex()], copyIndex())]",\r
  params: {
    myInput: 'in-${moduleName}-${moduleIndex}'
  }
  dependsOn: [
    singleModule
    singleResource
  ]
}]

// another module collection with dependency on another module collection
module moduleCollectionWithCollectionDependencies 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[489:535]       "copy": {\r
  name: concat(moduleName, moduleIndex)
//@[496:496]       "name": "[concat(variables('moduleSetup')[copyIndex()], copyIndex())]",\r
  params: {
    myInput: 'in-${moduleName}-${moduleIndex}'
  }
  dependsOn: [
    storageAccounts
    moduleCollectionWithSingleDependency
  ]
}]

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
//@[536:579]       "type": "Microsoft.Resources/deployments",\r
  name: 'hello'
//@[539:539]       "name": "hello",\r
  params: {
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
  }
  dependsOn: [
    storageAccounts2[index - 10]
  ]
}

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[580:627]       "copy": {\r
  name: concat(moduleName, moduleIndex)
//@[587:587]       "name": "[concat(variables('moduleSetup')[copyIndex()], copyIndex())]",\r
  params: {
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName} - ${moduleIndex}'
  }
  dependsOn: [
    storageAccounts2[index - 9]
  ]
}]

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
//@[708:711]     "indexedModulesName": {\r
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput
//@[712:715]     "indexedModuleOutput": {\r

// resource collection
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for (account, i) in accounts: {
  name: '${name}-existing-${account.name}-${i}'
}]

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
//@[716:719]     "existingIndexedResourceName": {\r
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
//@[720:723]     "existingIndexedResourceId": {\r
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
//@[724:727]     "existingIndexedResourceType": {\r
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
//@[728:731]     "existingIndexedResourceApiVersion": {\r
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
//@[732:735]     "existingIndexedResourceLocation": {\r
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier
//@[736:739]     "existingIndexedResourceAccessTier": {\r

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[248:257]       "copy": {\r
  name: 'no loop variable'
  location: 'eastus'
//@[256:256]       "location": "eastus"\r
}]

// reference to a resource collection whose name expression does not reference any loop variables
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
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

module apim 'passthrough.bicep' = [for (region, i) in regions: {
//@[628:670]       "copy": {\r
  name: 'apim-${region}-${name}-${i}'
//@[635:635]       "name": "[format('apim-{0}-{1}-{2}', variables('regions')[copyIndex()], parameters('name'), copyIndex())]",\r
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
          backends: [for (index,i) in range(0, length(regions)): {
//@[282:293]                   "name": "backends",\r
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: apim[index + i].outputs.myOutput
//@[286:286]                     "address": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}-{2}', variables('regions')[add(range(0, length(variables('regions')))[copyIndex('backends')], copyIndex('backends'))], parameters('name'), add(range(0, length(variables('regions')))[copyIndex('backends')], copyIndex('backends'))))).outputs.myOutput.value]",\r
            backendHostHeader: apim[index + i].outputs.myOutput
//@[287:287]                     "backendHostHeader": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}-{2}', variables('regions')[add(range(0, length(variables('regions')))[copyIndex('backends')], copyIndex('backends'))], parameters('name'), add(range(0, length(variables('regions')))[copyIndex('backends')], copyIndex('backends'))))).outputs.myOutput.value]",\r
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

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index, i) in range(0, length(regions)): {
//@[303:335]       "copy": {\r
  name: '${name}-${index}-${i}'
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
              address: apim[index+i].outputs.myOutput
//@[319:319]                   "address": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}-{2}', variables('regions')[add(range(0, length(variables('regions')))[copyIndex()], copyIndex())], parameters('name'), add(range(0, length(variables('regions')))[copyIndex()], copyIndex())))).outputs.myOutput.value]",\r
              backendHostHeader: apim[index+i].outputs.myOutput
//@[320:320]                   "backendHostHeader": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}-{2}', variables('regions')[add(range(0, length(variables('regions')))[copyIndex()], copyIndex())], parameters('name'), add(range(0, length(variables('regions')))[copyIndex()], copyIndex())))).outputs.myOutput.value]",\r
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
//@[351:351]                     "address": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}-{2}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex('backends')]].name, range(0, length(parameters('accounts')))[copyIndex('backends')]))).primaryEndpoints.internetEndpoints.web]",\r
            backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[352:352]                     "backendHostHeader": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}-{2}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex('backends')]].name, range(0, length(parameters('accounts')))[copyIndex('backends')]))).primaryEndpoints.internetEndpoints.web]",\r
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

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index,i) in range(0, length(accounts)): {
//@[368:400]       "copy": {\r
  name: '${name}-${index}-${i}'
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
              address: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
//@[384:384]                   "address": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}-{2}', parameters('name'), parameters('accounts')[add(range(0, length(parameters('accounts')))[copyIndex()], copyIndex())].name, add(range(0, length(parameters('accounts')))[copyIndex()], copyIndex())))).primaryEndpoints.internetEndpoints.web]",\r
              backendHostHeader: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
//@[385:385]                   "backendHostHeader": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}-{2}', parameters('name'), parameters('accounts')[add(range(0, length(parameters('accounts')))[copyIndex()], copyIndex())].name, add(range(0, length(parameters('accounts')))[copyIndex()], copyIndex())))).primaryEndpoints.internetEndpoints.web]",\r
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

