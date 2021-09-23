
@sys.description('this is basicStorage')
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[9:21) Resource basicStorage. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 225
  name: 'basicblobs'
  location: 'westus'
  kind: 'BlobStorage'
  sku: {
    name: 'Standard_GRS'
  }
}

@sys.description('this is dnsZone')
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
//@[9:16) Resource dnsZone. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 140
  name: 'myZone'
  location: 'global'
}

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[9:25) Resource myStorageAccount. Type: Microsoft.Storage/storageAccounts@2017-10-01. Declaration start char: 0, length: 469
  name: 'myencryptedone'
  location: 'eastus2'
  properties: {
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: true
        }
        file: {
          enabled: true
        }
      }
    }
  }
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource withExpressions 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[9:24) Resource withExpressions. Type: Microsoft.Storage/storageAccounts@2017-10-01. Declaration start char: 0, length: 539
  name: 'myencryptedone2'
  location: 'eastus2'
  properties: {
    supportsHttpsTrafficOnly: !false
    accessTier: true ? 'Hot' : 'Cold'
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: true || false
        }
        file: {
          enabled: true
        }
      }
    }
  }
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  dependsOn: [
    myStorageAccount
  ]
}

param applicationName string = 'to-do-app${uniqueString(resourceGroup().id)}'
//@[6:21) Parameter applicationName. Type: string. Declaration start char: 0, length: 77
var hostingPlanName = applicationName // why not just use the param directly?
//@[4:19) Variable hostingPlanName. Type: string. Declaration start char: 0, length: 37

param appServicePlanTier string
//@[6:24) Parameter appServicePlanTier. Type: string. Declaration start char: 0, length: 31
param appServicePlanInstances int
//@[6:29) Parameter appServicePlanInstances. Type: int. Declaration start char: 0, length: 33

var location = resourceGroup().location
//@[4:12) Variable location. Type: string. Declaration start char: 0, length: 39

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
//@[9:13) Resource farm. Type: Microsoft.Web/serverfarms@2019-08-01. Declaration start char: 0, length: 371
  // dependsOn: resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName)
  name: hostingPlanName
  location: location
  sku: {
    name: appServicePlanTier
    capacity: appServicePlanInstances
  }
  properties: {
    name: hostingPlanName // just hostingPlanName results in an error
  }
}

var cosmosDbResourceId = resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosDb.account)
//@[4:22) Variable cosmosDbResourceId. Type: string. Declaration start char: 0, length: 94
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint
//@[4:15) Variable cosmosDbRef. Type: any. Declaration start char: 0, length: 64

// this variable is not accessed anywhere in this template and depends on a run-time reference
// it should not be present at all in the template output as there is nowhere logical to put it
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint
//@[4:20) Variable cosmosDbEndpoint. Type: any. Declaration start char: 0, length: 51

param webSiteName string
//@[6:17) Parameter webSiteName. Type: string. Declaration start char: 0, length: 24
param cosmosDb object
//@[6:14) Parameter cosmosDb. Type: object. Declaration start char: 0, length: 21
resource site 'Microsoft.Web/sites@2019-08-01' = {
//@[9:13) Resource site. Type: Microsoft.Web/sites@2019-08-01. Declaration start char: 0, length: 689
  name: webSiteName
  location: location
  properties: {
    // not yet supported // serverFarmId: farm.id
    siteConfig: {
      appSettings: [
        {
          name: 'CosmosDb:Account'
          value: reference(cosmosDbResourceId).documentEndpoint
        }
        {
          name: 'CosmosDb:Key'
          value: listKeys(cosmosDbResourceId, '2020-04-01').primaryMasterKey
        }
        {
          name: 'CosmosDb:DatabaseName'
          value: cosmosDb.databaseName
        }
        {
          name: 'CosmosDb:ContainerName'
          value: cosmosDb.containerName
        }
      ]
    }
  }
}

var _siteApiVersion = site.apiVersion
//@[4:19) Variable _siteApiVersion. Type: '2019-08-01'. Declaration start char: 0, length: 37
var _siteType = site.type
//@[4:13) Variable _siteType. Type: 'Microsoft.Web/sites'. Declaration start char: 0, length: 25

output siteApiVersion string = site.apiVersion
//@[7:21) Output siteApiVersion. Type: string. Declaration start char: 0, length: 46
output siteType string = site.type
//@[7:15) Output siteType. Type: string. Declaration start char: 0, length: 34

resource nested 'Microsoft.Resources/deployments@2019-10-01' = {
//@[9:15) Resource nested. Type: Microsoft.Resources/deployments@2019-10-01. Declaration start char: 0, length: 354
  name: 'nestedTemplate1'
  properties: {
    mode: 'Incremental'
    template: {
      // string key value
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
      contentVersion: '1.0.0.0'
      resources: [
      ]
    }
  }
}

// should be able to access the read only properties
resource accessingReadOnlyProperties 'Microsoft.Foo/foos@2019-10-01' = {
//@[9:36) Resource accessingReadOnlyProperties. Type: Microsoft.Foo/foos@2019-10-01. Declaration start char: 0, length: 284
  name: 'nestedTemplate1'
  properties: {
    otherId: nested.id
    otherName: nested.name
    otherVersion: nested.apiVersion
    otherType: nested.type

    otherThings: nested.properties.mode
  }
}

resource resourceA 'My.Rp/typeA@2020-01-01' = {
//@[9:18) Resource resourceA. Type: My.Rp/typeA@2020-01-01. Declaration start char: 0, length: 71
  name: 'resourceA'
}

resource resourceB 'My.Rp/typeA/typeB@2020-01-01' = {
//@[9:18) Resource resourceB. Type: My.Rp/typeA/typeB@2020-01-01. Declaration start char: 0, length: 92
  name: '${resourceA.name}/myName'
}

resource resourceC 'My.Rp/typeA/typeB@2020-01-01' = {
//@[9:18) Resource resourceC. Type: My.Rp/typeA/typeB@2020-01-01. Declaration start char: 0, length: 269
  name: '${resourceA.name}/myName'
  properties: {
    aId: resourceA.id
    aType: resourceA.type
    aName: resourceA.name
    aApiVersion: resourceA.apiVersion
    bProperties: resourceB.properties
  }
}

var varARuntime = {
//@[4:15) Variable varARuntime. Type: object. Declaration start char: 0, length: 155
  bId: resourceB.id
  bType: resourceB.type
  bName: resourceB.name
  bApiVersion: resourceB.apiVersion
  aKind: resourceA.kind
}

var varBRuntime = [
//@[4:15) Variable varBRuntime. Type: object[]. Declaration start char: 0, length: 37
  varARuntime
]

var resourceCRef = {
//@[4:16) Variable resourceCRef. Type: object. Declaration start char: 0, length: 43
  id: resourceC.id
}
var setResourceCRef = true
//@[4:19) Variable setResourceCRef. Type: bool. Declaration start char: 0, length: 26

resource resourceD 'My.Rp/typeD@2020-01-01' = {
//@[9:18) Resource resourceD. Type: My.Rp/typeD@2020-01-01. Declaration start char: 0, length: 231
  name: 'constant'
  properties: {
    runtime: varBRuntime
    // repro for https://github.com/Azure/bicep/issues/316
    repro316: setResourceCRef ? resourceCRef : null
  }
}

var myInterpKey = 'abc'
//@[4:15) Variable myInterpKey. Type: 'abc'. Declaration start char: 0, length: 23
resource resourceWithInterp 'My.Rp/interp@2020-01-01' = {
//@[9:27) Resource resourceWithInterp. Type: My.Rp/interp@2020-01-01. Declaration start char: 0, length: 202
  name: 'interpTest'
  properties: {
    '${myInterpKey}': 1
    'abc${myInterpKey}def': 2
    '${myInterpKey}abc${myInterpKey}': 3
  }
}

resource resourceWithEscaping 'My.Rp/mockResource@2020-01-01' = {
//@[9:29) Resource resourceWithEscaping. Type: My.Rp/mockResource@2020-01-01. Declaration start char: 0, length: 234
  name: 'test'
  properties: {
    // both key and value should be escaped in template output
    '[resourceGroup().location]': '[resourceGroup().location]'
  }
}

param shouldDeployVm bool = true
//@[6:20) Parameter shouldDeployVm. Type: bool. Declaration start char: 0, length: 32

@sys.description('this is vmWithCondition')
resource vmWithCondition 'Microsoft.Compute/virtualMachines@2020-06-01' = if (shouldDeployVm) {
//@[9:24) Resource vmWithCondition. Type: Microsoft.Compute/virtualMachines@2020-06-01. Declaration start char: 0, length: 308
  name: 'vmName'
  location: 'westus'
  properties: {
    osProfile: {
      windowsConfiguration: {
        enableAutomaticUpdates: true
      }
    }
  }
}

resource extension1 'My.Rp/extensionResource@2020-12-01' = {
//@[9:19) Resource extension1. Type: My.Rp/extensionResource@2020-12-01. Declaration start char: 0, length: 110
  name: 'extension'
  scope: vmWithCondition
}

resource extension2 'My.Rp/extensionResource@2020-12-01' = {
//@[9:19) Resource extension2. Type: My.Rp/extensionResource@2020-12-01. Declaration start char: 0, length: 105
  name: 'extension'
  scope: extension1
}

resource extensionDependencies 'My.Rp/mockResource@2020-01-01' = {
//@[9:30) Resource extensionDependencies. Type: My.Rp/mockResource@2020-01-01. Declaration start char: 0, length: 359
  name: 'extensionDependencies'
  properties: {
    res1: vmWithCondition.id
    res1runtime: vmWithCondition.properties.something
    res2: extension1.id
    res2runtime: extension1.properties.something
    res3: extension2.id
    res3runtime: extension2.properties.something
  }
}

@sys.description('this is existing1')
resource existing1 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[9:18) Resource existing1. Type: Mock.Rp/existingExtensionResource@2020-01-01. Declaration start char: 0, length: 162
  name: 'existing1'
  scope: extension1
}

resource existing2 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[9:18) Resource existing2. Type: Mock.Rp/existingExtensionResource@2020-01-01. Declaration start char: 0, length: 122
  name: 'existing2'
  scope: existing1
}

resource extension3 'My.Rp/extensionResource@2020-12-01' = {
//@[9:19) Resource extension3. Type: My.Rp/extensionResource@2020-12-01. Declaration start char: 0, length: 105
  name: 'extension3'
  scope: existing1
}

/*
  valid loop cases
*/ 
var storageAccounts = [
//@[4:19) Variable storageAccounts. Type: array. Declaration start char: 0, length: 129
  {
    name: 'one'
    location: 'eastus2'
  }
  {
    name: 'two'
    location: 'westus'
  }
]

// just a storage account loop
@sys.description('this is just a storage account loop')
resource storageResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[80:87) Local account. Type: any. Declaration start char: 80, length: 7
//@[9:25) Resource storageResources. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 284
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}]

// storage account loop with index
@sys.description('this is just a storage account loop with index')
resource storageResourcesWithIndex 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, i) in storageAccounts: {
//@[90:97) Local account. Type: any. Declaration start char: 90, length: 7
//@[99:100) Local i. Type: int. Declaration start char: 99, length: 1
//@[9:34) Resource storageResourcesWithIndex. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 318
  name: '${account.name}${i}'
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}]

// basic nested loop
@sys.description('this is just a basic nested loop')
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[68:69) Local i. Type: int. Declaration start char: 68, length: 1
//@[9:13) Resource vnet. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 399
  name: 'vnet-${i}'
  properties: {
    subnets: [for j in range(0, 4): {
//@[18:19) Local j. Type: int. Declaration start char: 18, length: 1
      // #completionTest(0,1,2,3,4,5) -> subnetIdAndProperties
     
      // #completionTest(6) -> subnetIdAndPropertiesNoColon
      name: 'subnet-${i}-${j}'
    }]
  }
}]

// duplicate identifiers within the loop are allowed
resource duplicateIdentifiersWithinLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[94:95) Local i. Type: int. Declaration start char: 94, length: 1
//@[9:39) Resource duplicateIdentifiersWithinLoop. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 239
  name: 'vnet-${i}'
  properties: {
    subnets: [for i in range(0, 4): {
//@[18:19) Local i. Type: int. Declaration start char: 18, length: 1
      name: 'subnet-${i}-${i}'
    }]
  }
}]

// duplicate identifers in global and single loop scope are allowed (inner variable hides the outer)
var canHaveDuplicatesAcrossScopes = 'hello'
//@[4:33) Variable canHaveDuplicatesAcrossScopes. Type: 'hello'. Declaration start char: 0, length: 43
resource duplicateInGlobalAndOneLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for canHaveDuplicatesAcrossScopes in range(0, 3): {
//@[91:120) Local canHaveDuplicatesAcrossScopes. Type: int. Declaration start char: 91, length: 29
//@[9:36) Resource duplicateInGlobalAndOneLoop. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 292
  name: 'vnet-${canHaveDuplicatesAcrossScopes}'
  properties: {
    subnets: [for i in range(0, 4): {
//@[18:19) Local i. Type: int. Declaration start char: 18, length: 1
      name: 'subnet-${i}-${i}'
    }]
  }
}]

// duplicate in global and multiple loop scopes are allowed (inner hides the outer)
var duplicatesEverywhere = 'hello'
//@[4:24) Variable duplicatesEverywhere. Type: 'hello'. Declaration start char: 0, length: 34
resource duplicateInGlobalAndTwoLoops 'Microsoft.Network/virtualNetworks@2020-06-01' = [for duplicatesEverywhere in range(0, 3): {
//@[92:112) Local duplicatesEverywhere. Type: int. Declaration start char: 92, length: 20
//@[9:37) Resource duplicateInGlobalAndTwoLoops. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 308
  name: 'vnet-${duplicatesEverywhere}'
  properties: {
    subnets: [for duplicatesEverywhere in range(0, 4): {
//@[18:38) Local duplicatesEverywhere. Type: int. Declaration start char: 18, length: 20
      name: 'subnet-${duplicatesEverywhere}'
    }]
  }
}]

/*
  Scope values created via array access on a resource collection
*/
resource dnsZones 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in range(0,4): {
//@[65:69) Local zone. Type: int. Declaration start char: 65, length: 4
//@[9:17) Resource dnsZones. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 135
  name: 'zone${zone}'
  location: 'global'
}]

resource locksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for lock in range(0,2): {
//@[72:76) Local lock. Type: int. Declaration start char: 72, length: 4
//@[9:21) Resource locksOnZones. Type: Microsoft.Authorization/locks@2016-09-01[]. Declaration start char: 0, length: 194
  name: 'lock${lock}'
  properties: {
    level: 'CanNotDelete'
  }
  scope: dnsZones[lock]
}]

resource moreLocksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for (lock, i) in range(0,3): {
//@[77:81) Local lock. Type: int. Declaration start char: 77, length: 4
//@[83:84) Local i. Type: int. Declaration start char: 83, length: 1
//@[9:25) Resource moreLocksOnZones. Type: Microsoft.Authorization/locks@2016-09-01[]. Declaration start char: 0, length: 196
  name: 'another${i}'
  properties: {
    level: 'ReadOnly'
  }
  scope: dnsZones[i]
}]

resource singleLockOnFirstZone 'Microsoft.Authorization/locks@2016-09-01' = {
//@[9:30) Resource singleLockOnFirstZone. Type: Microsoft.Authorization/locks@2016-09-01. Declaration start char: 0, length: 170
  name: 'single-lock'
  properties: {
    level: 'ReadOnly'
  }
  scope: dnsZones[0]
}


resource p1_vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[9:16) Resource p1_vnet. Type: Microsoft.Network/virtualNetworks@2020-06-01. Declaration start char: 0, length: 234
  location: resourceGroup().location
  name: 'myVnet'
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/20'
      ]
    }
  }
}

resource p1_subnet1 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[9:19) Resource p1_subnet1. Type: Microsoft.Network/virtualNetworks/subnets@2020-06-01. Declaration start char: 0, length: 175
  parent: p1_vnet
  name: 'subnet1'
  properties: {
    addressPrefix: '10.0.0.0/24'
  }
}

resource p1_subnet2 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[9:19) Resource p1_subnet2. Type: Microsoft.Network/virtualNetworks/subnets@2020-06-01. Declaration start char: 0, length: 175
  parent: p1_vnet
  name: 'subnet2'
  properties: {
    addressPrefix: '10.0.1.0/24'
  }
}

output p1_subnet1prefix string = p1_subnet1.properties.addressPrefix
//@[7:23) Output p1_subnet1prefix. Type: string. Declaration start char: 0, length: 68
output p1_subnet1name string = p1_subnet1.name
//@[7:21) Output p1_subnet1name. Type: string. Declaration start char: 0, length: 46
output p1_subnet1type string = p1_subnet1.type
//@[7:21) Output p1_subnet1type. Type: string. Declaration start char: 0, length: 46
output p1_subnet1id string = p1_subnet1.id
//@[7:19) Output p1_subnet1id. Type: string. Declaration start char: 0, length: 42

// parent property with extension resource
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[9:16) Resource p2_res1. Type: Microsoft.Rp1/resource1@2020-06-01. Declaration start char: 0, length: 76
  name: 'res1'
}

resource p2_res1child 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[9:21) Resource p2_res1child. Type: Microsoft.Rp1/resource1/child1@2020-06-01. Declaration start char: 0, length: 109
  parent: p2_res1
  name: 'child1'
}

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[9:16) Resource p2_res2. Type: Microsoft.Rp2/resource2@2020-06-01. Declaration start char: 0, length: 99
  scope: p2_res1child
  name: 'res2'
}

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[9:21) Resource p2_res2child. Type: Microsoft.Rp2/resource2/child2@2020-06-01. Declaration start char: 0, length: 109
  parent: p2_res2
  name: 'child2'
}

output p2_res2childprop string = p2_res2child.properties.someProp
//@[7:23) Output p2_res2childprop. Type: string. Declaration start char: 0, length: 65
output p2_res2childname string = p2_res2child.name
//@[7:23) Output p2_res2childname. Type: string. Declaration start char: 0, length: 50
output p2_res2childtype string = p2_res2child.type
//@[7:23) Output p2_res2childtype. Type: string. Declaration start char: 0, length: 50
output p2_res2childid string = p2_res2child.id
//@[7:21) Output p2_res2childid. Type: string. Declaration start char: 0, length: 46

// parent property with 'existing' resource
resource p3_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[9:16) Resource p3_res1. Type: Microsoft.Rp1/resource1@2020-06-01. Declaration start char: 0, length: 85
  name: 'res1'
}

resource p3_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[9:18) Resource p3_child1. Type: Microsoft.Rp1/resource1/child1@2020-06-01. Declaration start char: 0, length: 106
  parent: p3_res1
  name: 'child1'
}

output p3_res1childprop string = p3_child1.properties.someProp
//@[7:23) Output p3_res1childprop. Type: string. Declaration start char: 0, length: 62
output p3_res1childname string = p3_child1.name
//@[7:23) Output p3_res1childname. Type: string. Declaration start char: 0, length: 47
output p3_res1childtype string = p3_child1.type
//@[7:23) Output p3_res1childtype. Type: string. Declaration start char: 0, length: 47
output p3_res1childid string = p3_child1.id
//@[7:21) Output p3_res1childid. Type: string. Declaration start char: 0, length: 43

// parent & child with 'existing'
resource p4_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[9:16) Resource p4_res1. Type: Microsoft.Rp1/resource1@2020-06-01. Declaration start char: 0, length: 104
  scope: tenant()
  name: 'res1'
}

resource p4_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' existing = {
//@[9:18) Resource p4_child1. Type: Microsoft.Rp1/resource1/child1@2020-06-01. Declaration start char: 0, length: 115
  parent: p4_res1
  name: 'child1'
}

output p4_res1childprop string = p4_child1.properties.someProp
//@[7:23) Output p4_res1childprop. Type: string. Declaration start char: 0, length: 62
output p4_res1childname string = p4_child1.name
//@[7:23) Output p4_res1childname. Type: string. Declaration start char: 0, length: 47
output p4_res1childtype string = p4_child1.type
//@[7:23) Output p4_res1childtype. Type: string. Declaration start char: 0, length: 47
output p4_res1childid string = p4_child1.id
//@[7:21) Output p4_res1childid. Type: string. Declaration start char: 0, length: 43

