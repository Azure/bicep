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
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[78:96]       "copy": {
  name: '${name}-collection-${account.name}'
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
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[109:124]       "copy": {
  name: 'lock-${i}'
  properties: {
//@[118:120]       "properties": {
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[119:119]         "level": "[if(equals(range(0, 1)[copyIndex()], 0), 'CanNotDelete', 'ReadOnly')]"
  }
  scope: singleResource
}]

// cascade extend the extension
@batchSize(1)
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[125:142]       "copy": {
  name: 'lock-the-lock-${i}'
  properties: {
//@[136:138]       "properties": {
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[137:137]         "level": "[if(equals(range(0, 1)[copyIndex()], 0), 'CanNotDelete', 'ReadOnly')]"
  }
  scope: extensionCollection[i]
}]

// special case property access
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
//@[784:787]     "indexedCollectionBlobEndpoint": {
output indexedCollectionName string = storageAccounts[index].name
//@[788:791]     "indexedCollectionName": {
output indexedCollectionId string = storageAccounts[index].id
//@[792:795]     "indexedCollectionId": {
output indexedCollectionType string = storageAccounts[index].type
//@[796:799]     "indexedCollectionType": {
output indexedCollectionVersion string = storageAccounts[index].apiVersion
//@[800:803]     "indexedCollectionVersion": {

// general case property access
output indexedCollectionIdentity object = storageAccounts[index].identity
//@[804:807]     "indexedCollectionIdentity": {

// indexed access of two properties
output indexedEndpointPair object = {
//@[808:814]     "indexedEndpointPair": {
  primary: storageAccounts[index].properties.primaryEndpoints.blob
//@[811:811]         "primary": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[parameters('index')].name))).primaryEndpoints.blob]",
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
//@[812:812]         "secondary": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[add(parameters('index'), 1)].name))).secondaryEndpoints.blob]"
}

// nested indexer?
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier
//@[815:818]     "indexViaReference": {

// dependency on a resource collection
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[143:159]       "copy": {
  name: '${name}-collection-${account.name}'
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
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[160:173]       "copy": {
  name: '${name}-set1-${i}'
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

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[174:190]       "copy": {
  name: '${name}-set2-${i}'
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
    firstSet[i]
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

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for vnetConfig in vnetConfigurations: {
//@[204:213]       "copy": {
  name: vnetConfig.name
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
//@[222:222]             "id": "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[add(parameters('index'), 1)].name)]"
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
//@[238:238]             "id": "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[sub(parameters('index'), 1)].name)]"
      }
      {
        id: vnets[index * 2].id
//@[241:241]             "id": "[resourceId('Microsoft.Network/virtualNetworks', variables('vnetConfigurations')[mul(parameters('index'), 2)].name)]"
      }
    ]
  }
  dependsOn: [
    vnets
  ]
}

// single module
module singleModule 'passthrough.bicep' = {
//@[424:462]       "type": "Microsoft.Resources/deployments",
  name: 'test'
//@[427:427]       "name": "test",
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
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[463:511]       "copy": {
  name: moduleName
//@[472:472]       "name": "[variables('moduleSetup')[copyIndex()]]",
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
//@[512:558]       "copy": {
  name: moduleName
//@[519:519]       "name": "[variables('moduleSetup')[copyIndex()]]",
  params: {
    myInput: 'in-${moduleName}'
  }
  dependsOn: [
    storageAccounts
    moduleCollectionWithSingleDependency
  ]
}]

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
//@[559:602]       "type": "Microsoft.Resources/deployments",
  name: 'hello'
//@[562:562]       "name": "hello",
  params: {
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
  }
  dependsOn: [
    storageAccounts2[index - 10]
  ]
}

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[603:650]       "copy": {
  name: moduleName
//@[610:610]       "name": "[variables('moduleSetup')[copyIndex()]]",
  params: {
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName}'
  }
  dependsOn: [
    storageAccounts2[index - 9]
  ]
}]

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
//@[819:822]     "indexedModulesName": {
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput
//@[823:826]     "indexedModuleOutput": {

// resource collection
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for account in accounts: {
  name: '${name}-existing-${account.name}'
}]

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
//@[827:830]     "existingIndexedResourceName": {
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
//@[831:834]     "existingIndexedResourceId": {
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
//@[835:838]     "existingIndexedResourceType": {
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
//@[839:842]     "existingIndexedResourceApiVersion": {
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
//@[843:846]     "existingIndexedResourceLocation": {
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier
//@[847:850]     "existingIndexedResourceAccessTier": {

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[249:258]       "copy": {
  name: 'no loop variable'
  location: 'eastus'
//@[257:257]       "location": "eastus"
}]

// reference to a resource collection whose name expression does not reference any loop variables
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
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

module apim 'passthrough.bicep' = [for region in regions: {
//@[651:693]       "copy": {
  name: 'apim-${region}-${name}'
//@[658:658]       "name": "[format('apim-{0}-{1}', variables('regions')[copyIndex()], parameters('name'))]",
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
          backends: [for index in range(0, length(regions)): {
//@[283:294]                   "name": "backends",
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: apim[index].outputs.myOutput
//@[287:287]                     "address": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex('backends')]], parameters('name')))).outputs.myOutput.value]",
            backendHostHeader: apim[index].outputs.myOutput
//@[288:288]                     "backendHostHeader": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex('backends')]], parameters('name')))).outputs.myOutput.value]",
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

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(regions)): {
//@[304:336]       "copy": {
  name: '${name}-${index}'
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
              address: apim[index].outputs.myOutput
//@[320:320]                   "address": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex()]], parameters('name')))).outputs.myOutput.value]",
              backendHostHeader: apim[index].outputs.myOutput
//@[321:321]                   "backendHostHeader": "[reference(resourceId('Microsoft.Resources/deployments', format('apim-{0}-{1}', variables('regions')[range(0, length(variables('regions')))[copyIndex()]], parameters('name')))).outputs.myOutput.value]",
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
//@[352:352]                     "address": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex('backends')]].name))).primaryEndpoints.internetEndpoints.web]",
            backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[353:353]                     "backendHostHeader": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex('backends')]].name))).primaryEndpoints.internetEndpoints.web]",
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

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(accounts)): {
//@[369:401]       "copy": {
  name: '${name}-${index}'
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
              address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[385:385]                   "address": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex()]].name))).primaryEndpoints.internetEndpoints.web]",
              backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[386:386]                   "backendHostHeader": "[reference(resourceId('Microsoft.Storage/storageAccounts', format('{0}-collection-{1}', parameters('name'), parameters('accounts')[range(0, length(parameters('accounts')))[copyIndex()]].name))).primaryEndpoints.internetEndpoints.web]",
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

resource filteredZones 'Microsoft.Network/dnsZones@2018-05-01' = [for i in range(0,10): if(i % 3 == 0) {
//@[402:412]       "condition": "[equals(mod(range(0, 10)[copyIndex()], 3), 0)]",
  name: 'zone${i}'
  location: resourceGroup().location
//@[411:411]       "location": "[resourceGroup().location]"
}]

module filteredModules 'passthrough.bicep' = [for i in range(0,6): if(i % 2 == 0) {
//@[694:737]       "condition": "[equals(mod(range(0, 6)[copyIndex()], 2), 0)]",
  name: 'stuff${i}'
//@[702:702]       "name": "[format('stuff{0}', range(0, 6)[copyIndex()])]",
  params: {
    myInput: 'script-${i}'
  }
}]

resource filteredIndexedZones 'Microsoft.Network/dnsZones@2018-05-01' = [for (account, i) in accounts: if(account.enabled) {
//@[413:423]       "condition": "[parameters('accounts')[copyIndex()].enabled]",
  name: 'indexedZone-${account.name}-${i}'
  location: account.location
//@[422:422]       "location": "[parameters('accounts')[copyIndex()].location]"
}]

output lastNameServers array = filteredIndexedZones[length(accounts) - 1].properties.nameServers
//@[851:854]     "lastNameServers": {

module filteredIndexedModules 'passthrough.bicep' = [for (account, i) in accounts: if(account.enabled) {
//@[738:781]       "condition": "[parameters('accounts')[copyIndex()].enabled]",
  name: 'stuff-${i}'
//@[746:746]       "name": "[format('stuff-{0}', copyIndex())]",
  params: {
    myInput: 'script-${account.name}-${i}'
  }
}]

output lastModuleOutput string = filteredIndexedModules[length(accounts) - 1].outputs.myOutput
//@[855:858]     "lastModuleOutput": {

