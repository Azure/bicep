param name string
//@[6:10) Parameter name. Type: string. Declaration start char: 0, length: 17
param accounts array
//@[6:14) Parameter accounts. Type: array. Declaration start char: 0, length: 20
param index int
//@[6:11) Parameter index. Type: int. Declaration start char: 0, length: 15

// single resource
resource singleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[9:23) Resource singleResource. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 209
  name: '${name}single-resource-name'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

// extension of single resource
resource singleResourceExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[9:32) Resource singleResourceExtension. Type: Microsoft.Authorization/locks@2016-09-01. Declaration start char: 0, length: 182
  scope: singleResource
  name: 'single-resource-lock'
  properties: {
    level: 'CanNotDelete'
  }
}

// single resource cascade extension
resource singleResourceCascadeExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[9:39) Resource singleResourceCascadeExtension. Type: Microsoft.Authorization/locks@2016-09-01. Declaration start char: 0, length: 211
  scope: singleResourceExtension
  name: 'single-resource-cascade-extension'
  properties: {
    level: 'CanNotDelete'
  }
}

// resource collection
@batchSize(42)
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[79:86) Local account. Type: any. Declaration start char: 79, length: 7
//@[9:24) Resource storageAccounts. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 289
  name: '${name}-collection-${account.name}'
  location: account.location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  dependsOn: [
    singleResource
  ]
}]

// extension of a single resource in a collection
resource extendSingleResourceInCollection 'Microsoft.Authorization/locks@2016-09-01' = {
//@[9:41) Resource extendSingleResourceInCollection. Type: Microsoft.Authorization/locks@2016-09-01. Declaration start char: 0, length: 212
  name: 'one-resource-collection-item-lock'
  properties: {
    level: 'ReadOnly'
  }
  scope: storageAccounts[index % 2]
}

// collection of extensions
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[79:80) Local i. Type: int. Declaration start char: 79, length: 1
//@[9:28) Resource extensionCollection. Type: Microsoft.Authorization/locks@2016-09-01[]. Declaration start char: 0, length: 212
  name: 'lock-${i}'
  properties: {
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
  }
  scope: singleResource
}]

// cascade extend the extension
@batchSize(1)
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[72:73) Local i. Type: int. Declaration start char: 72, length: 1
//@[9:21) Resource lockTheLocks. Type: Microsoft.Authorization/locks@2016-09-01[]. Declaration start char: 0, length: 236
  name: 'lock-the-lock-${i}'
  properties: {
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
  }
  scope: extensionCollection[i]
}]

// special case property access
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
//@[7:36) Output indexedCollectionBlobEndpoint. Type: string. Declaration start char: 0, length: 101
output indexedCollectionName string = storageAccounts[index].name
//@[7:28) Output indexedCollectionName. Type: string. Declaration start char: 0, length: 65
output indexedCollectionId string = storageAccounts[index].id
//@[7:26) Output indexedCollectionId. Type: string. Declaration start char: 0, length: 61
output indexedCollectionType string = storageAccounts[index].type
//@[7:28) Output indexedCollectionType. Type: string. Declaration start char: 0, length: 65
output indexedCollectionVersion string = storageAccounts[index].apiVersion
//@[7:31) Output indexedCollectionVersion. Type: string. Declaration start char: 0, length: 74

// general case property access
output indexedCollectionIdentity object = storageAccounts[index].identity
//@[7:32) Output indexedCollectionIdentity. Type: object. Declaration start char: 0, length: 73

// indexed access of two properties
output indexedEndpointPair object = {
//@[7:26) Output indexedEndpointPair. Type: object. Declaration start char: 0, length: 181
  primary: storageAccounts[index].properties.primaryEndpoints.blob
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
}

// nested indexer?
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier
//@[7:24) Output indexViaReference. Type: string. Declaration start char: 0, length: 124

// dependency on a resource collection
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[80:87) Local account. Type: any. Declaration start char: 80, length: 7
//@[9:25) Resource storageAccounts2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 276
  name: '${name}-collection-${account.name}'
  location: account.location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  dependsOn: [
    storageAccounts
  ]
}]

// one-to-one paired dependencies
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[72:73) Local i. Type: int. Declaration start char: 72, length: 1
//@[9:17) Resource firstSet. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 232
  name: '${name}-set1-${i}'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}]

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[73:74) Local i. Type: int. Declaration start char: 73, length: 1
//@[9:18) Resource secondSet. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 268
  name: '${name}-set2-${i}'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  dependsOn: [
    firstSet[i]
  ]
}]

// depending on collection and one resource in the collection optimizes the latter part away
resource anotherSingleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[9:30) Resource anotherSingleResource. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 266
  name: '${name}single-resource-name'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  dependsOn: [
    secondSet
    secondSet[0]
  ]
}

// vnets
var vnetConfigurations = [
//@[4:22) Variable vnetConfigurations. Type: array. Declaration start char: 0, length: 138
  {
    name: 'one'
    location: resourceGroup().location
  }
  {
    name: 'two'
    location: 'westus'
  }
]

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for vnetConfig in vnetConfigurations: {
//@[69:79) Local vnetConfig. Type: any. Declaration start char: 69, length: 10
//@[9:14) Resource vnets. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 163
  name: vnetConfig.name
  location: vnetConfig.location
}]

// implicit dependency on single resource from a resource collection
resource implicitDependencyOnSingleResourceByIndex 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[9:50) Resource implicitDependencyOnSingleResourceByIndex. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 237
  name: 'test'
  location: 'global'
  properties: {
    resolutionVirtualNetworks: [
      {
        id: vnets[index+1].id
      }
    ]
  }
}

// implicit and explicit dependency combined
resource combinedDependencies 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[9:29) Resource combinedDependencies. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 294
  name: 'test2'
  location: 'global'
  properties: {
    resolutionVirtualNetworks: [
      {
        id: vnets[index-1].id
      }
      {
        id: vnets[index * 2].id
      }
    ]
  }
  dependsOn: [
    vnets
  ]
}

// single module
module singleModule 'passthrough.bicep' = {
//@[7:19) Module singleModule. Type: module. Declaration start char: 0, length: 97
  name: 'test'
  params: {
    myInput: 'hello'
  }
}

var moduleSetup = [
//@[4:15) Variable moduleSetup. Type: array. Declaration start char: 0, length: 47
  'one'
  'two'
  'three'
]

// module collection plus explicit dependency on single module
@sys.batchSize(3)
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[71:81) Local moduleName. Type: any. Declaration start char: 71, length: 10
//@[7:43) Module moduleCollectionWithSingleDependency. Type: module[]. Declaration start char: 0, length: 242
  name: moduleName
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
//@[77:87) Local moduleName. Type: any. Declaration start char: 77, length: 10
//@[7:49) Module moduleCollectionWithCollectionDependencies. Type: module[]. Declaration start char: 0, length: 255
  name: moduleName
  params: {
    myInput: 'in-${moduleName}'
  }
  dependsOn: [
    storageAccounts
    moduleCollectionWithSingleDependency
  ]
}]

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
//@[7:42) Module singleModuleWithIndexedDependencies. Type: module. Declaration start char: 0, length: 290
  name: 'hello'
  params: {
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
  }
  dependsOn: [
    storageAccounts2[index - 10]
  ]
}

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[74:84) Local moduleName. Type: any. Declaration start char: 74, length: 10
//@[7:46) Module moduleCollectionWithIndexedDependencies. Type: module[]. Declaration start char: 0, length: 346
  name: moduleName
  params: {
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName}'
  }
  dependsOn: [
    storageAccounts2[index - 9]
  ]
}]

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
//@[7:25) Output indexedModulesName. Type: string. Declaration start char: 0, length: 83
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput
//@[7:26) Output indexedModuleOutput. Type: string. Declaration start char: 0, length: 100

// resource collection
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for account in accounts: {
//@[96:103) Local account. Type: any. Declaration start char: 96, length: 7
//@[9:32) Resource existingStorageAccounts. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 164
  name: '${name}-existing-${account.name}'
}]

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
//@[7:34) Output existingIndexedResourceName. Type: string. Declaration start char: 0, length: 83
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
//@[7:32) Output existingIndexedResourceId. Type: string. Declaration start char: 0, length: 79
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
//@[7:34) Output existingIndexedResourceType. Type: string. Declaration start char: 0, length: 81
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
//@[7:40) Output existingIndexedResourceApiVersion. Type: string. Declaration start char: 0, length: 93
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
//@[7:38) Output existingIndexedResourceLocation. Type: string. Declaration start char: 0, length: 89
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier
//@[7:40) Output existingIndexedResourceAccessTier. Type: string. Declaration start char: 0, length: 104

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[72:76) Local zone. Type: any. Declaration start char: 72, length: 4
//@[9:24) Resource duplicatedNames. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 136
  name: 'no loop variable'
  location: 'eastus'
}]

// reference to a resource collection whose name expression does not reference any loop variables
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[82:86) Local zone. Type: any. Declaration start char: 82, length: 4
//@[9:34) Resource referenceToDuplicateNames. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 194
  name: 'no loop variable 2'
  location: 'eastus'
  dependsOn: [
    duplicatedNames[index]
  ]
}]

var regions = [
//@[4:11) Variable regions. Type: array. Declaration start char: 0, length: 39
  'eastus'
  'westus'
]

module apim 'passthrough.bicep' = [for region in regions: {
//@[39:45) Local region. Type: any. Declaration start char: 39, length: 6
//@[7:11) Module apim. Type: module[]. Declaration start char: 0, length: 131
  name: 'apim-${region}-${name}'
  params: {
    myInput: region
  }
}]

resource propertyLoopDependencyOnModuleCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[9:49) Resource propertyLoopDependencyOnModuleCollection. Type: Microsoft.Network/frontDoors@2020-05-01. Declaration start char: 0, length: 780
  name: name
  location: 'Global'
  properties: {
    backendPools: [
      {
        name: 'BackendAPIMs'
        properties: {
          backends: [for index in range(0, length(regions)): {
//@[25:30) Local index. Type: int. Declaration start char: 25, length: 5
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: apim[index].outputs.myOutput
            backendHostHeader: apim[index].outputs.myOutput
            httpPort: 80
            httpsPort: 443
            priority: 1
            weight: 50
          }]
        }
      }
    ]
  }
}

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(regions)): {
//@[92:97) Local index. Type: int. Declaration start char: 92, length: 5
//@[9:42) Resource indexedModuleCollectionDependency. Type: Microsoft.Network/frontDoors@2020-05-01[]. Declaration start char: 0, length: 757
  name: '${name}-${index}'
  location: 'Global'
  properties: {
    backendPools: [
      {
        name: 'BackendAPIMs'
        properties: {
          backends: [
            {
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: apim[index].outputs.myOutput
              backendHostHeader: apim[index].outputs.myOutput
              httpPort: 80
              httpsPort: 443
              priority: 1
              weight: 50
            }
          ]
        }
      }
    ]
  }
}]

resource propertyLoopDependencyOnResourceCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[9:51) Resource propertyLoopDependencyOnResourceCollection. Type: Microsoft.Network/frontDoors@2020-05-01. Declaration start char: 0, length: 871
  name: name
  location: 'Global'
  properties: {
    backendPools: [
      {
        name: 'BackendAPIMs'
        properties: {
          backends: [for index in range(0, length(accounts)): {
//@[25:30) Local index. Type: int. Declaration start char: 25, length: 5
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
            backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
            httpPort: 80
            httpsPort: 443
            priority: 1
            weight: 50
          }]
        }
      }
    ]
  }
}

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(accounts)): {
//@[94:99) Local index. Type: int. Declaration start char: 94, length: 5
//@[9:44) Resource indexedResourceCollectionDependency. Type: Microsoft.Network/frontDoors@2020-05-01[]. Declaration start char: 0, length: 848
  name: '${name}-${index}'
  location: 'Global'
  properties: {
    backendPools: [
      {
        name: 'BackendAPIMs'
        properties: {
          backends: [
            {
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
              backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
              httpPort: 80
              httpsPort: 443
              priority: 1
              weight: 50
            }
          ]
        }
      }
    ]
  }
}]

resource filteredZones 'Microsoft.Network/dnsZones@2018-05-01' = [for i in range(0,10): if(i % 3 == 0) {
//@[70:71) Local i. Type: int. Declaration start char: 70, length: 1
//@[9:22) Resource filteredZones. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 163
  name: 'zone${i}'
  location: resourceGroup().location
}]

module filteredModules 'passthrough.bicep' = [for i in range(0,6): if(i % 2 == 0) {
//@[50:51) Local i. Type: int. Declaration start char: 50, length: 1
//@[7:22) Module filteredModules. Type: module[]. Declaration start char: 0, length: 149
  name: 'stuff${i}'
  params: {
    myInput: 'script-${i}'
  }
}]

resource filteredIndexedZones 'Microsoft.Network/dnsZones@2018-05-01' = [for (account, i) in accounts: if(account.enabled) {
//@[78:85) Local account. Type: any. Declaration start char: 78, length: 7
//@[87:88) Local i. Type: int. Declaration start char: 87, length: 1
//@[9:29) Resource filteredIndexedZones. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 199
  name: 'indexedZone-${account.name}-${i}'
  location: account.location
}]

output lastNameServers array = filteredIndexedZones[length(accounts) - 1].properties.nameServers
//@[7:22) Output lastNameServers. Type: array. Declaration start char: 0, length: 96

module filteredIndexedModules 'passthrough.bicep' = [for (account, i) in accounts: if(account.enabled) {
//@[58:65) Local account. Type: any. Declaration start char: 58, length: 7
//@[67:68) Local i. Type: int. Declaration start char: 67, length: 1
//@[7:29) Module filteredIndexedModules. Type: module[]. Declaration start char: 0, length: 187
  name: 'stuff-${i}'
  params: {
    myInput: 'script-${account.name}-${i}'
  }
}]

output lastModuleOutput string = filteredIndexedModules[length(accounts) - 1].outputs.myOutput
//@[7:23) Output lastModuleOutput. Type: string. Declaration start char: 0, length: 94

