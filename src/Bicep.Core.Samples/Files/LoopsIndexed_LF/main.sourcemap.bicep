param name string
//@[12:14]     "name": {
param accounts array
//@[15:17]     "accounts": {
param index int
//@[18:20]     "index": {

// single resource
resource singleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[44:53]       "type": "Microsoft.Storage/storageAccounts",
  name: '${name}single-resource-name'
  location: resourceGroup().location
//@[48:48]       "location": "[resourceGroup().location]",
  kind: 'StorageV2'
//@[49:49]       "kind": "StorageV2",
  sku: {
//@[50:52]       "sku": {
    name: 'Standard_LRS'
//@[51:51]         "name": "Standard_LRS"
  }
}

// extension of single resource
resource singleResourceExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[54:65]       "type": "Microsoft.Authorization/locks",
  scope: singleResource
  name: 'single-resource-lock'
  properties: {
//@[59:61]       "properties": {
    level: 'CanNotDelete'
//@[60:60]         "level": "CanNotDelete"
  }
}

// single resource cascade extension
resource singleResourceCascadeExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[66:77]       "type": "Microsoft.Authorization/locks",
  scope: singleResourceExtension
  name: 'single-resource-cascade-extension'
  properties: {
//@[71:73]       "properties": {
    level: 'CanNotDelete'
//@[72:72]         "level": "CanNotDelete"
  }
}

// resource collection
@batchSize(42)
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, index) in accounts: {
//@[78:96]       "copy": {
  name: '${name}-collection-${account.name}-${index}'
  location: account.location
//@[88:88]       "location": "[parameters('accounts')[copyIndex()].location]",
  kind: 'StorageV2'
//@[89:89]       "kind": "StorageV2",
  sku: {
//@[90:92]       "sku": {
    name: 'Standard_LRS'
//@[91:91]         "name": "Standard_LRS"
  }
  dependsOn: [
    singleResource
  ]
}]

// extension of a single resource in a collection
resource extendSingleResourceInCollection 'Microsoft.Authorization/locks@2016-09-01' = {
//@[97:108]       "type": "Microsoft.Authorization/locks",
  name: 'one-resource-collection-item-lock'
  properties: {
//@[102:104]       "properties": {
    level: 'ReadOnly'
//@[103:103]         "level": "ReadOnly"
  }
  scope: storageAccounts[index % 2]
}

// collection of extensions
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[109:124]       "copy": {
  name: 'lock-${i}-${i2}'
  properties: {
//@[118:120]       "properties": {
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[119:119]         "level": "[if(and(equals(range(0, 1)[copyIndex()], 0), equals(copyIndex(), 0)), 'CanNotDelete', 'ReadOnly')]"
  }
  scope: singleResource
}]

// cascade extend the extension
@batchSize(1)
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[125:142]       "copy": {
  name: 'lock-the-lock-${i}-${i2}'
  properties: {
//@[136:138]       "properties": {
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[137:137]         "level": "[if(and(equals(range(0, 1)[copyIndex()], 0), equals(copyIndex(), 0)), 'CanNotDelete', 'ReadOnly')]"
  }
  scope: extensionCollection[i2]
}]

// special case property access
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
//@[674:677]     "indexedCollectionBlobEndpoint": {
output indexedCollectionName string = storageAccounts[index].name
//@[678:681]     "indexedCollectionName": {
output indexedCollectionId string = storageAccounts[index].id
//@[682:685]     "indexedCollectionId": {
output indexedCollectionType string = storageAccounts[index].type
//@[686:689]     "indexedCollectionType": {
output indexedCollectionVersion string = storageAccounts[index].apiVersion
//@[690:693]     "indexedCollectionVersion": {

// general case property access
output indexedCollectionIdentity object = storageAccounts[index].identity
//@[694:697]     "indexedCollectionIdentity": {

// indexed access of two properties
output indexedEndpointPair object = {
//@[698:704]     "indexedEndpointPair": {
  primary: storageAccounts[index].properties.primaryEndpoints.blob
//@[701:701]         "primary": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}-{2}', parameters('name'), parameters('accounts')[parameters('index')].name, parameters('index')))).primaryEndpoints.blob]",
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
//@[702:702]         "secondary": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}-{2}', parameters('name'), parameters('accounts')[add(parameters('index'), 1)].name, add(parameters('index'), 1)))).secondaryEndpoints.blob]"
}

// nested indexer?
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier
//@[705:708]     "indexViaReference": {

// dependency on a resource collection
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, idx) in accounts: {
//@[143:159]       "copy": {
  name: '${name}-collection-${account.name}-${idx}'
  location: account.location
//@[151:151]       "location": "[parameters('accounts')[copyIndex()].location]",
  kind: 'StorageV2'
//@[152:152]       "kind": "StorageV2",
  sku: {
//@[153:155]       "sku": {
    name: 'Standard_LRS'
//@[154:154]         "name": "Standard_LRS"
  }
  dependsOn: [
    storageAccounts
  ]
}]

// one-to-one paired dependencies
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,ii) in range(0, length(accounts)): {
//@[160:173]       "copy": {
  name: '${name}-set1-${i}-${ii}'
  location: resourceGroup().location
//@[168:168]       "location": "[resourceGroup().location]",
  kind: 'StorageV2'
//@[169:169]       "kind": "StorageV2",
  sku: {
//@[170:172]       "sku": {
    name: 'Standard_LRS'
//@[171:171]         "name": "Standard_LRS"
  }
}]

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,iii) in range(0, length(accounts)): {
//@[174:190]       "copy": {
  name: '${name}-set2-${i}-${iii}'
  location: resourceGroup().location
//@[182:182]       "location": "[resourceGroup().location]",
  kind: 'StorageV2'
//@[183:183]       "kind": "StorageV2",
  sku: {
//@[184:186]       "sku": {
    name: 'Standard_LRS'
//@[185:185]         "name": "Standard_LRS"
  }
  dependsOn: [
    firstSet[iii]
  ]
}]

// depending on collection and one resource in the collection optimizes the latter part away
resource anotherSingleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[191:203]       "type": "Microsoft.Storage/storageAccounts",
  name: '${name}single-resource-name'
  location: resourceGroup().location
//@[195:195]       "location": "[resourceGroup().location]",
  kind: 'StorageV2'
//@[196:196]       "kind": "StorageV2",
  sku: {
//@[197:199]       "sku": {
    name: 'Standard_LRS'
//@[198:198]         "name": "Standard_LRS"
  }
  dependsOn: [
    secondSet
    secondSet[0]
  ]
}

// vnets
var vnetConfigurations = [
//@[23:32]     "vnetConfigurations": [
  {
    name: 'one'
//@[25:25]         "name": "one",
    location: resourceGroup().location
//@[26:26]         "location": "[resourceGroup().location]"
  }
  {
    name: 'two'
//@[29:29]         "name": "two",
    location: 'westus'
//@[30:30]         "location": "westus"
  }
]

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (vnetConfig, index) in vnetConfigurations: {
//@[204:213]       "copy": {
  name: '${vnetConfig.name}-${index}'
  location: vnetConfig.location
//@[212:212]       "location": "[variables('vnetConfigurations')[copyIndex()].location]"
}]

// implicit dependency on single resource from a resource collection
resource implicitDependencyOnSingleResourceByIndex 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[214:229]       "type": "Microsoft.Network/dnsZones",
  name: 'test'
  location: 'global'
//@[218:218]       "location": "global",
  properties: {
//@[219:225]       "properties": {
    resolutionVirtualNetworks: [
//@[220:224]         "resolutionVirtualNetworks": [
      {
        id: vnets[index+1].id
//@[222:222]             "id": "[resourceId('Microsoft.Network/virtualNetworks', format('{0}-{1}', variables('vnetConfigurations')[add(parameters('index'), 1)].name, add(parameters('index'), 1)))]"
      }
    ]
  }
}

// implicit and explicit dependency combined
resource combinedDependencies 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[230:248]       "type": "Microsoft.Network/dnsZones",
  name: 'test2'
  location: 'global'
//@[234:234]       "location": "global",
  properties: {
//@[235:244]       "properties": {
    resolutionVirtualNetworks: [
//@[236:243]         "resolutionVirtualNetworks": [
      {
        id: vnets[index-1].id
//@[238:238]             "id": "[resourceId('Microsoft.Network/virtualNetworks', format('{0}-{1}', variables('vnetConfigurations')[sub(parameters('index'), 1)].name, sub(parameters('index'), 1)))]"
      }
      {
        id: vnets[index * 2].id
//@[241:241]             "id": "[resourceId('Microsoft.Network/virtualNetworks', format('{0}-{1}', variables('vnetConfigurations')[mul(parameters('index'), 2)].name, mul(parameters('index'), 2)))]"
      }
    ]
  }
  dependsOn: [
    vnets
  ]
}

// single module
module singleModule 'passthrough.bicep' = {
//@[402:440]       "type": "Microsoft.Resources/deployments",
  name: 'test'
//@[405:405]       "name": "test",
  params: {
    myInput: 'hello'
  }
}

var moduleSetup = [
//@[33:37]     "moduleSetup": [
  'one'
//@[34:34]       "one",
  'two'
//@[35:35]       "two",
  'three'
//@[36:36]       "three"
]

// module collection plus explicit dependency on single module
@sys.batchSize(3)
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[441:489]       "copy": {
  name: concat(moduleName, moduleIndex)
//@[450:450]       "name": "[concat(variables('moduleSetup')[copyIndex()], copyIndex())]",
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
//@[490:536]       "copy": {
  name: concat(moduleName, moduleIndex)
//@[497:497]       "name": "[concat(variables('moduleSetup')[copyIndex()], copyIndex())]",
  params: {
    myInput: 'in-${moduleName}-${moduleIndex}'
  }
  dependsOn: [
    storageAccounts
    moduleCollectionWithSingleDependency
  ]
}]

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
//@[537:580]       "type": "Microsoft.Resources/deployments",
  name: 'hello'
//@[540:540]       "name": "hello",
  params: {
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
  }
  dependsOn: [
    storageAccounts2[index - 10]
  ]
}

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[581:628]       "copy": {
  name: concat(moduleName, moduleIndex)
//@[588:588]       "name": "[concat(variables('moduleSetup')[copyIndex()], copyIndex())]",
  params: {
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName} - ${moduleIndex}'
  }
  dependsOn: [
    storageAccounts2[index - 9]
  ]
}]

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
//@[709:712]     "indexedModulesName": {
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput
//@[713:716]     "indexedModuleOutput": {

// resource collection
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for (account, i) in accounts: {
  name: '${name}-existing-${account.name}-${i}'
}]

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
//@[717:720]     "existingIndexedResourceName": {
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
//@[721:724]     "existingIndexedResourceId": {
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
//@[725:728]     "existingIndexedResourceType": {
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
//@[729:732]     "existingIndexedResourceApiVersion": {
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
//@[733:736]     "existingIndexedResourceLocation": {
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier
//@[737:740]     "existingIndexedResourceAccessTier": {

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[249:258]       "copy": {
  name: 'no loop variable'
  location: 'eastus'
//@[257:257]       "location": "eastus"
}]

// reference to a resource collection whose name expression does not reference any loop variables
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[259:271]       "copy": {
  name: 'no loop variable 2'
  location: 'eastus'
//@[267:267]       "location": "eastus",
  dependsOn: [
    duplicatedNames[index]
  ]
}]

var regions = [
//@[38:41]     "regions": [
  'eastus'
//@[39:39]       "eastus",
  'westus'
//@[40:40]       "westus"
]

module apim 'passthrough.bicep' = [for (region, i) in regions: {
//@[629:671]       "copy": {
  name: 'apim-${region}-${name}-${i}'
//@[636:636]       "name": "[format('apim-{0}-{1}-{2}', variables('regions')[copyIndex()], parameters('name'), copyIndex())]",
  params: {
    myInput: region
  }
}]

resource propertyLoopDependencyOnModuleCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[272:303]       "type": "Microsoft.Network/frontDoors",
  name: name
  location: 'Global'
//@[276:276]       "location": "Global",
  properties: {
//@[277:299]       "properties": {
    backendPools: [
//@[278:298]         "backendPools": [
      {
        name: 'BackendAPIMs'
//@[280:280]             "name": "BackendAPIMs",
        properties: {
//@[281:296]             "properties": {
          backends: [for (index,i) in range(0, length(regions)): {
//@[283:294]                   "name": "backends",
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: apim[index + i].outputs.myOutput
//@[287:287]                     "address": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}-{2}', variables('regions')[add(range(0, length(variables('regions')))[copyIndex('backends')], copyIndex('backends'))], parameters('name'), add(range(0, length(variables('regions')))[copyIndex('backends')], copyIndex('backends'))))).outputs.myOutput.value]",
            backendHostHeader: apim[index + i].outputs.myOutput
//@[288:288]                     "backendHostHeader": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}-{2}', variables('regions')[add(range(0, length(variables('regions')))[copyIndex('backends')], copyIndex('backends'))], parameters('name'), add(range(0, length(variables('regions')))[copyIndex('backends')], copyIndex('backends'))))).outputs.myOutput.value]",
            httpPort: 80
//@[289:289]                     "httpPort": 80,
            httpsPort: 443
//@[290:290]                     "httpsPort": 443,
            priority: 1
//@[291:291]                     "priority": 1,
            weight: 50
//@[292:292]                     "weight": 50
          }]
        }
      }
    ]
  }
}

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index, i) in range(0, length(regions)): {
//@[304:336]       "copy": {
  name: '${name}-${index}-${i}'
  location: 'Global'
//@[312:312]       "location": "Global",
  properties: {
//@[313:331]       "properties": {
    backendPools: [
//@[314:330]         "backendPools": [
      {
        name: 'BackendAPIMs'
//@[316:316]             "name": "BackendAPIMs",
        properties: {
//@[317:328]             "properties": {
          backends: [
//@[318:327]               "backends": [
            {
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: apim[index+i].outputs.myOutput
//@[320:320]                   "address": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}-{2}', variables('regions')[add(range(0, length(variables('regions')))[copyIndex()], copyIndex())], parameters('name'), add(range(0, length(variables('regions')))[copyIndex()], copyIndex())))).outputs.myOutput.value]",
              backendHostHeader: apim[index+i].outputs.myOutput
//@[321:321]                   "backendHostHeader": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}-{2}', variables('regions')[add(range(0, length(variables('regions')))[copyIndex()], copyIndex())], parameters('name'), add(range(0, length(variables('regions')))[copyIndex()], copyIndex())))).outputs.myOutput.value]",
              httpPort: 80
//@[322:322]                   "httpPort": 80,
              httpsPort: 443
//@[323:323]                   "httpsPort": 443,
              priority: 1
//@[324:324]                   "priority": 1,
              weight: 50
//@[325:325]                   "weight": 50
            }
          ]
        }
      }
    ]
  }
}]

resource propertyLoopDependencyOnResourceCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[337:368]       "type": "Microsoft.Network/frontDoors",
  name: name
  location: 'Global'
//@[341:341]       "location": "Global",
  properties: {
//@[342:364]       "properties": {
    backendPools: [
//@[343:363]         "backendPools": [
      {
        name: 'BackendAPIMs'
//@[345:345]             "name": "BackendAPIMs",
        properties: {
//@[346:361]             "properties": {
          backends: [for index in range(0, length(accounts)): {
//@[348:359]                   "name": "backends",
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[352:352]                     "address": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}-{2}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex('backends')]].name, range(0, length(parameters('accounts')))[copyIndex('backends')]))).primaryEndpoints.internetEndpoints.web]",
            backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[353:353]                     "backendHostHeader": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}-{2}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex('backends')]].name, range(0, length(parameters('accounts')))[copyIndex('backends')]))).primaryEndpoints.internetEndpoints.web]",
            httpPort: 80
//@[354:354]                     "httpPort": 80,
            httpsPort: 443
//@[355:355]                     "httpsPort": 443,
            priority: 1
//@[356:356]                     "priority": 1,
            weight: 50
//@[357:357]                     "weight": 50
          }]
        }
      }
    ]
  }
}

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index,i) in range(0, length(accounts)): {
//@[369:401]       "copy": {
  name: '${name}-${index}-${i}'
  location: 'Global'
//@[377:377]       "location": "Global",
  properties: {
//@[378:396]       "properties": {
    backendPools: [
//@[379:395]         "backendPools": [
      {
        name: 'BackendAPIMs'
//@[381:381]             "name": "BackendAPIMs",
        properties: {
//@[382:393]             "properties": {
          backends: [
//@[383:392]               "backends": [
            {
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
//@[385:385]                   "address": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}-{2}', parameters('name'), parameters('accounts')[add(range(0, length(parameters('accounts')))[copyIndex()], copyIndex())].name, add(range(0, length(parameters('accounts')))[copyIndex()], copyIndex())))).primaryEndpoints.internetEndpoints.web]",
              backendHostHeader: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
//@[386:386]                   "backendHostHeader": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}-{2}', parameters('name'), parameters('accounts')[add(range(0, length(parameters('accounts')))[copyIndex()], copyIndex())].name, add(range(0, length(parameters('accounts')))[copyIndex()], copyIndex())))).primaryEndpoints.internetEndpoints.web]",
              httpPort: 80
//@[387:387]                   "httpPort": 80,
              httpsPort: 443
//@[388:388]                   "httpsPort": 443,
              priority: 1
//@[389:389]                   "priority": 1,
              weight: 50
//@[390:390]                   "weight": 50
            }
          ]
        }
      }
    ]
  }
}]

