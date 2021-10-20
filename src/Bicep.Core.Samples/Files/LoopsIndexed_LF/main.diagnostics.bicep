param name string
param accounts array
param index int

// single resource
resource singleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: '${name}single-resource-name'
  location: resourceGroup().location
//@[12:36) [no-hardcoded-location (Warning)] Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |resourceGroup().location|
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

// extension of single resource
resource singleResourceExtension 'Microsoft.Authorization/locks@2016-09-01' = {
  scope: singleResource
  name: 'single-resource-lock'
  properties: {
    level: 'CanNotDelete'
  }
}

// single resource cascade extension
resource singleResourceCascadeExtension 'Microsoft.Authorization/locks@2016-09-01' = {
  scope: singleResourceExtension
  name: 'single-resource-cascade-extension'
  properties: {
    level: 'CanNotDelete'
  }
}

// resource collection
@batchSize(42)
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, index) in accounts: {
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
  name: 'one-resource-collection-item-lock'
  properties: {
    level: 'ReadOnly'
  }
  scope: storageAccounts[index % 2]
}

// collection of extensions
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
  name: 'lock-${i}-${i2}'
  properties: {
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
  }
  scope: singleResource
}]

// cascade extend the extension
@batchSize(1)
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
  name: 'lock-the-lock-${i}-${i2}'
  properties: {
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
  }
  scope: extensionCollection[i2]
}]

// special case property access
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
output indexedCollectionName string = storageAccounts[index].name
output indexedCollectionId string = storageAccounts[index].id
output indexedCollectionType string = storageAccounts[index].type
output indexedCollectionVersion string = storageAccounts[index].apiVersion

// general case property access
output indexedCollectionIdentity object = storageAccounts[index].identity

// indexed access of two properties
output indexedEndpointPair object = {
  primary: storageAccounts[index].properties.primaryEndpoints.blob
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
}

// nested indexer?
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier

// dependency on a resource collection
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, idx) in accounts: {
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
  name: '${name}-set1-${i}-${ii}'
  location: resourceGroup().location
//@[12:36) [no-hardcoded-location (Warning)] Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |resourceGroup().location|
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}]

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,iii) in range(0, length(accounts)): {
  name: '${name}-set2-${i}-${iii}'
  location: resourceGroup().location
//@[12:36) [no-hardcoded-location (Warning)] Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |resourceGroup().location|
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
  name: '${name}single-resource-name'
  location: resourceGroup().location
//@[12:36) [no-hardcoded-location (Warning)] Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |resourceGroup().location|
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
  {
    name: 'one'
    location: resourceGroup().location
//@[14:38) [no-hardcoded-location (Warning)] Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |resourceGroup().location|
  }
  {
    name: 'two'
    location: 'westus'
  }
]

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (vnetConfig, index) in vnetConfigurations: {
  name: '${vnetConfig.name}-${index}'
  location: vnetConfig.location
}]

// implicit dependency on single resource from a resource collection
resource implicitDependencyOnSingleResourceByIndex 'Microsoft.Network/dnsZones@2018-05-01' = {
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
  name: 'test'
  params: {
    myInput: 'hello'
  }
}

var moduleSetup = [
  'one'
  'two'
  'three'
]

// module collection plus explicit dependency on single module
@sys.batchSize(3)
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
  name: concat(moduleName, moduleIndex)
//@[8:39) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(moduleName, moduleIndex)|
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
  name: concat(moduleName, moduleIndex)
//@[8:39) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(moduleName, moduleIndex)|
  params: {
    myInput: 'in-${moduleName}-${moduleIndex}'
  }
  dependsOn: [
    storageAccounts
    moduleCollectionWithSingleDependency
  ]
}]

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
  name: 'hello'
  params: {
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
//@[13:137) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)|
  }
  dependsOn: [
    storageAccounts2[index - 10]
  ]
}

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
  name: concat(moduleName, moduleIndex)
//@[8:39) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(moduleName, moduleIndex)|
  params: {
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName} - ${moduleIndex}'
  }
  dependsOn: [
    storageAccounts2[index - 9]
  ]
}]

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput

// resource collection
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for (account, i) in accounts: {
  name: '${name}-existing-${account.name}-${i}'
}]

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[9:24) [BCP179 (Warning)] The loop item variable "zone" or the index variable "i" must be referenced in at least one of the value expressions of the following properties in the loop body: "name" (CodeDescription: none) |duplicatedNames|
  name: 'no loop variable'
  location: 'eastus'
//@[12:20) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
}]

// reference to a resource collection whose name expression does not reference any loop variables
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[9:34) [BCP179 (Warning)] The loop item variable "zone" or the index variable "i" must be referenced in at least one of the value expressions of the following properties in the loop body: "name" (CodeDescription: none) |referenceToDuplicateNames|
  name: 'no loop variable 2'
  location: 'eastus'
//@[12:20) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
  dependsOn: [
    duplicatedNames[index]
  ]
}]

var regions = [
  'eastus'
  'westus'
]

module apim 'passthrough.bicep' = [for (region, i) in regions: {
  name: 'apim-${region}-${name}-${i}'
  params: {
    myInput: region
  }
}]

resource propertyLoopDependencyOnModuleCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
  name: name
  location: 'Global'
  properties: {
    backendPools: [
      {
        name: 'BackendAPIMs'
        properties: {
          backends: [for (index,i) in range(0, length(regions)): {
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
  name: name
  location: 'Global'
  properties: {
    backendPools: [
      {
        name: 'BackendAPIMs'
        properties: {
          backends: [for index in range(0, length(accounts)): {
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

