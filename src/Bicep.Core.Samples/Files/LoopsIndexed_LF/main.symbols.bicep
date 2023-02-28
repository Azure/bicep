param name string
//@[006:010) Parameter name. Type: string. Declaration start char: 0, length: 17
param accounts array
//@[006:014) Parameter accounts. Type: array. Declaration start char: 0, length: 20
param index int
//@[006:011) Parameter index. Type: int. Declaration start char: 0, length: 15

// single resource
resource singleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[009:023) Resource singleResource. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 209
  name: '${name}single-resource-name'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

// extension of single resource
resource singleResourceExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[009:032) Resource singleResourceExtension. Type: Microsoft.Authorization/locks@2016-09-01. Declaration start char: 0, length: 182
  scope: singleResource
  name: 'single-resource-lock'
  properties: {
    level: 'CanNotDelete'
  }
}

// single resource cascade extension
resource singleResourceCascadeExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[009:039) Resource singleResourceCascadeExtension. Type: Microsoft.Authorization/locks@2016-09-01. Declaration start char: 0, length: 211
  scope: singleResourceExtension
  name: 'single-resource-cascade-extension'
  properties: {
    level: 'CanNotDelete'
  }
}

// resource collection
@batchSize(42)
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, index) in accounts: {
//@[080:087) Local account. Type: any. Declaration start char: 80, length: 7
//@[089:094) Local index. Type: int. Declaration start char: 89, length: 5
//@[009:024) Resource storageAccounts. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 307
  name: '${name}-collection-${account.name}-${index}'
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
//@[009:041) Resource extendSingleResourceInCollection. Type: Microsoft.Authorization/locks@2016-09-01. Declaration start char: 0, length: 212
  name: 'one-resource-collection-item-lock'
  properties: {
    level: 'ReadOnly'
  }
  scope: storageAccounts[index % 2]
}

// collection of extensions
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[080:081) Local i. Type: int. Declaration start char: 80, length: 1
//@[083:085) Local i2. Type: int. Declaration start char: 83, length: 2
//@[009:028) Resource extensionCollection. Type: Microsoft.Authorization/locks@2016-09-01[]. Declaration start char: 0, length: 235
  name: 'lock-${i}-${i2}'
  properties: {
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
  }
  scope: singleResource
}]

// cascade extend the extension
@batchSize(1)
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[073:074) Local i. Type: int. Declaration start char: 73, length: 1
//@[076:078) Local i2. Type: int. Declaration start char: 76, length: 2
//@[009:021) Resource lockTheLocks. Type: Microsoft.Authorization/locks@2016-09-01[]. Declaration start char: 0, length: 260
  name: 'lock-the-lock-${i}-${i2}'
  properties: {
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
  }
  scope: extensionCollection[i2]
}]

// special case property access
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
//@[007:036) Output indexedCollectionBlobEndpoint. Type: string. Declaration start char: 0, length: 101
output indexedCollectionName string = storageAccounts[index].name
//@[007:028) Output indexedCollectionName. Type: string. Declaration start char: 0, length: 65
output indexedCollectionId string = storageAccounts[index].id
//@[007:026) Output indexedCollectionId. Type: string. Declaration start char: 0, length: 61
output indexedCollectionType string = storageAccounts[index].type
//@[007:028) Output indexedCollectionType. Type: string. Declaration start char: 0, length: 65
output indexedCollectionVersion string = storageAccounts[index].apiVersion
//@[007:031) Output indexedCollectionVersion. Type: string. Declaration start char: 0, length: 74

// general case property access
output indexedCollectionIdentity object = storageAccounts[index].identity
//@[007:032) Output indexedCollectionIdentity. Type: object. Declaration start char: 0, length: 73

// indexed access of two properties
output indexedEndpointPair object = {
//@[007:026) Output indexedEndpointPair. Type: object. Declaration start char: 0, length: 181
  primary: storageAccounts[index].properties.primaryEndpoints.blob
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
}

// nested indexer?
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier
//@[007:024) Output indexViaReference. Type: string. Declaration start char: 0, length: 124

// dependency on a resource collection
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, idx) in accounts: {
//@[081:088) Local account. Type: any. Declaration start char: 81, length: 7
//@[090:093) Local idx. Type: int. Declaration start char: 90, length: 3
//@[009:025) Resource storageAccounts2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 290
  name: '${name}-collection-${account.name}-${idx}'
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
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,ii) in range(0, length(accounts)): {
//@[073:074) Local i. Type: int. Declaration start char: 73, length: 1
//@[075:077) Local ii. Type: int. Declaration start char: 75, length: 2
//@[009:017) Resource firstSet. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 243
  name: '${name}-set1-${i}-${ii}'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}]

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,iii) in range(0, length(accounts)): {
//@[074:075) Local i. Type: int. Declaration start char: 74, length: 1
//@[076:079) Local iii. Type: int. Declaration start char: 76, length: 3
//@[009:018) Resource secondSet. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 283
  name: '${name}-set2-${i}-${iii}'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  dependsOn: [
    firstSet[iii]
  ]
}]

// depending on collection and one resource in the collection optimizes the latter part away
resource anotherSingleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[009:030) Resource anotherSingleResource. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 266
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
//@[004:022) Variable vnetConfigurations. Type: [object, object]. Declaration start char: 0, length: 138
  {
    name: 'one'
    location: resourceGroup().location
  }
  {
    name: 'two'
    location: 'westus'
  }
]

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (vnetConfig, index) in vnetConfigurations: {
//@[070:080) Local vnetConfig. Type: object | object. Declaration start char: 70, length: 10
//@[082:087) Local index. Type: int. Declaration start char: 82, length: 5
//@[009:014) Resource vnets. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 186
  name: '${vnetConfig.name}-${index}'
  location: vnetConfig.location
}]

// implicit dependency on single resource from a resource collection
resource implicitDependencyOnSingleResourceByIndex 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[009:050) Resource implicitDependencyOnSingleResourceByIndex. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 237
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
//@[009:029) Resource combinedDependencies. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 294
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
//@[007:019) Module singleModule. Type: module. Declaration start char: 0, length: 97
  name: 'test'
  params: {
    myInput: 'hello'
  }
}

var moduleSetup = [
//@[004:015) Variable moduleSetup. Type: ['one', 'two', 'three']. Declaration start char: 0, length: 47
  'one'
  'two'
  'three'
]

// module collection plus explicit dependency on single module
@sys.batchSize(3)
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[072:082) Local moduleName. Type: 'one' | 'three' | 'two'. Declaration start char: 72, length: 10
//@[084:095) Local moduleIndex. Type: int. Declaration start char: 84, length: 11
//@[007:043) Module moduleCollectionWithSingleDependency. Type: module[]. Declaration start char: 0, length: 293
  name: concat(moduleName, moduleIndex)
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
//@[078:088) Local moduleName. Type: 'one' | 'three' | 'two'. Declaration start char: 78, length: 10
//@[090:101) Local moduleIndex. Type: int. Declaration start char: 90, length: 11
//@[007:049) Module moduleCollectionWithCollectionDependencies. Type: module[]. Declaration start char: 0, length: 306
  name: concat(moduleName, moduleIndex)
  params: {
    myInput: 'in-${moduleName}-${moduleIndex}'
  }
  dependsOn: [
    storageAccounts
    moduleCollectionWithSingleDependency
  ]
}]

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
//@[007:042) Module singleModuleWithIndexedDependencies. Type: module. Declaration start char: 0, length: 290
  name: 'hello'
  params: {
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
  }
  dependsOn: [
    storageAccounts2[index - 10]
  ]
}

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[075:085) Local moduleName. Type: 'one' | 'three' | 'two'. Declaration start char: 75, length: 10
//@[087:098) Local moduleIndex. Type: int. Declaration start char: 87, length: 11
//@[007:046) Module moduleCollectionWithIndexedDependencies. Type: module[]. Declaration start char: 0, length: 399
  name: concat(moduleName, moduleIndex)
  params: {
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName} - ${moduleIndex}'
  }
  dependsOn: [
    storageAccounts2[index - 9]
  ]
}]

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
//@[007:025) Output indexedModulesName. Type: string. Declaration start char: 0, length: 83
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput
//@[007:026) Output indexedModuleOutput. Type: string. Declaration start char: 0, length: 100

// resource collection
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for (account, i) in accounts: {
//@[097:104) Local account. Type: any. Declaration start char: 97, length: 7
//@[106:107) Local i. Type: int. Declaration start char: 106, length: 1
//@[009:032) Resource existingStorageAccounts. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 174
  name: '${name}-existing-${account.name}-${i}'
}]

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
//@[007:034) Output existingIndexedResourceName. Type: string. Declaration start char: 0, length: 83
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
//@[007:032) Output existingIndexedResourceId. Type: string. Declaration start char: 0, length: 79
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
//@[007:034) Output existingIndexedResourceType. Type: string. Declaration start char: 0, length: 81
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
//@[007:040) Output existingIndexedResourceApiVersion. Type: string. Declaration start char: 0, length: 93
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
//@[007:038) Output existingIndexedResourceLocation. Type: string. Declaration start char: 0, length: 89
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier
//@[007:040) Output existingIndexedResourceAccessTier. Type: string. Declaration start char: 0, length: 104

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[073:077) Local zone. Type: never. Declaration start char: 73, length: 4
//@[078:079) Local i. Type: int. Declaration start char: 78, length: 1
//@[009:024) Resource duplicatedNames. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 140
  name: 'no loop variable'
  location: 'eastus'
}]

// reference to a resource collection whose name expression does not reference any loop variables
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[083:087) Local zone. Type: never. Declaration start char: 83, length: 4
//@[088:089) Local i. Type: int. Declaration start char: 88, length: 1
//@[009:034) Resource referenceToDuplicateNames. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 198
  name: 'no loop variable 2'
  location: 'eastus'
  dependsOn: [
    duplicatedNames[index]
  ]
}]

var regions = [
//@[004:011) Variable regions. Type: ['eastus', 'westus']. Declaration start char: 0, length: 39
  'eastus'
  'westus'
]

module apim 'passthrough.bicep' = [for (region, i) in regions: {
//@[040:046) Local region. Type: 'eastus' | 'westus'. Declaration start char: 40, length: 6
//@[048:049) Local i. Type: int. Declaration start char: 48, length: 1
//@[007:011) Module apim. Type: module[]. Declaration start char: 0, length: 141
  name: 'apim-${region}-${name}-${i}'
  params: {
    myInput: region
  }
}]

resource propertyLoopDependencyOnModuleCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[009:049) Resource propertyLoopDependencyOnModuleCollection. Type: Microsoft.Network/frontDoors@2020-05-01. Declaration start char: 0, length: 792
  name: name
  location: 'Global'
  properties: {
    backendPools: [
      {
        name: 'BackendAPIMs'
        properties: {
          backends: [for (index,i) in range(0, length(regions)): {
//@[026:031) Local index. Type: int. Declaration start char: 26, length: 5
//@[032:033) Local i. Type: int. Declaration start char: 32, length: 1
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: apim[index + i].outputs.myOutput
            backendHostHeader: apim[index + i].outputs.myOutput
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

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index, i) in range(0, length(regions)): {
//@[093:098) Local index. Type: int. Declaration start char: 93, length: 5
//@[100:101) Local i. Type: int. Declaration start char: 100, length: 1
//@[009:042) Resource indexedModuleCollectionDependency. Type: Microsoft.Network/frontDoors@2020-05-01[]. Declaration start char: 0, length: 771
  name: '${name}-${index}-${i}'
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
              address: apim[index+i].outputs.myOutput
              backendHostHeader: apim[index+i].outputs.myOutput
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
//@[009:051) Resource propertyLoopDependencyOnResourceCollection. Type: Microsoft.Network/frontDoors@2020-05-01. Declaration start char: 0, length: 871
  name: name
  location: 'Global'
  properties: {
    backendPools: [
      {
        name: 'BackendAPIMs'
        properties: {
          backends: [for index in range(0, length(accounts)): {
//@[025:030) Local index. Type: int. Declaration start char: 25, length: 5
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

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index,i) in range(0, length(accounts)): {
//@[095:100) Local index. Type: int. Declaration start char: 95, length: 5
//@[101:102) Local i. Type: int. Declaration start char: 101, length: 1
//@[009:044) Resource indexedResourceCollectionDependency. Type: Microsoft.Network/frontDoors@2020-05-01[]. Declaration start char: 0, length: 861
  name: '${name}-${index}-${i}'
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
              address: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
              backendHostHeader: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
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

