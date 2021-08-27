
@sys.description('this is basicStorage')
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'basicblobs'
  location: 'westus'
  kind: 'BlobStorage'
  sku: {
    name: 'Standard_GRS'
  }
}

@sys.description('this is dnsZone')
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
  name: 'myZone'
  location: 'global'
}

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
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
  name: 'myencryptedone2'
  location: 'eastus2'
  properties: {
    supportsHttpsTrafficOnly: !false
    accessTier: true ? 'Hot' : 'Cold'
//@[16:37) [BCP036 (Warning)] The property "accessTier" expected a value of type "'Cool' | 'Hot' | null" but the provided value is of type "'Cold' | 'Hot'". (CodeDescription: none) |true ? 'Hot' : 'Cold'|
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
var hostingPlanName = applicationName // why not just use the param directly?

param appServicePlanTier string
param appServicePlanInstances int

var location = resourceGroup().location

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
  // dependsOn: resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName)
  name: hostingPlanName
  location: location
  sku: {
    name: appServicePlanTier
    capacity: appServicePlanInstances
  }
  properties: {
    name: hostingPlanName // just hostingPlanName results in an error
//@[4:8) [BCP037 (Warning)] The property "name" is not allowed on objects of type "AppServicePlanProperties". Permissible properties include "freeOfferExpirationTime", "hostingEnvironmentProfile", "hyperV", "isSpot", "isXenon", "maximumElasticWorkerCount", "perSiteScaling", "reserved", "spotExpirationTime", "targetWorkerCount", "targetWorkerSizeId", "workerTierName". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |name|
  }
}

var cosmosDbResourceId = resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosDb.account)
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint

// this variable is not accessed anywhere in this template and depends on a run-time reference
// it should not be present at all in the template output as there is nowhere logical to put it
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint
//@[4:20) [no-unused-vars (Warning)] Variable "cosmosDbEndpoint" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |cosmosDbEndpoint|

param webSiteName string
param cosmosDb object
resource site 'Microsoft.Web/sites@2019-08-01' = {
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
//@[4:19) [no-unused-vars (Warning)] Variable "_siteApiVersion" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |_siteApiVersion|
var _siteType = site.type
//@[4:13) [no-unused-vars (Warning)] Variable "_siteType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |_siteType|

output siteApiVersion string = site.apiVersion
output siteType string = site.type

resource nested 'Microsoft.Resources/deployments@2019-10-01' = {
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
//@[37:68) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2019-10-01" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2019-10-01'|
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
//@[19:43) [BCP081 (Warning)] Resource type "My.Rp/typeA@2020-01-01" does not have types available. (CodeDescription: none) |'My.Rp/typeA@2020-01-01'|
  name: 'resourceA'
}

resource resourceB 'My.Rp/typeA/typeB@2020-01-01' = {
//@[19:49) [BCP081 (Warning)] Resource type "My.Rp/typeA/typeB@2020-01-01" does not have types available. (CodeDescription: none) |'My.Rp/typeA/typeB@2020-01-01'|
  name: '${resourceA.name}/myName'
}

resource resourceC 'My.Rp/typeA/typeB@2020-01-01' = {
//@[19:49) [BCP081 (Warning)] Resource type "My.Rp/typeA/typeB@2020-01-01" does not have types available. (CodeDescription: none) |'My.Rp/typeA/typeB@2020-01-01'|
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
  bId: resourceB.id
  bType: resourceB.type
  bName: resourceB.name
  bApiVersion: resourceB.apiVersion
  aKind: resourceA.kind
}

var varBRuntime = [
  varARuntime
]

var resourceCRef = {
  id: resourceC.id
}
var setResourceCRef = true

resource resourceD 'My.Rp/typeD@2020-01-01' = {
//@[19:43) [BCP081 (Warning)] Resource type "My.Rp/typeD@2020-01-01" does not have types available. (CodeDescription: none) |'My.Rp/typeD@2020-01-01'|
  name: 'constant'
  properties: {
    runtime: varBRuntime
    // repro for https://github.com/Azure/bicep/issues/316
    repro316: setResourceCRef ? resourceCRef : null
  }
}

var myInterpKey = 'abc'
resource resourceWithInterp 'My.Rp/interp@2020-01-01' = {
//@[28:53) [BCP081 (Warning)] Resource type "My.Rp/interp@2020-01-01" does not have types available. (CodeDescription: none) |'My.Rp/interp@2020-01-01'|
  name: 'interpTest'
  properties: {
    '${myInterpKey}': 1
    'abc${myInterpKey}def': 2
    '${myInterpKey}abc${myInterpKey}': 3
  }
}

resource resourceWithEscaping 'My.Rp/mockResource@2020-01-01' = {
//@[30:61) [BCP081 (Warning)] Resource type "My.Rp/mockResource@2020-01-01" does not have types available. (CodeDescription: none) |'My.Rp/mockResource@2020-01-01'|
  name: 'test'
  properties: {
    // both key and value should be escaped in template output
    '[resourceGroup().location]': '[resourceGroup().location]'
  }
}

param shouldDeployVm bool = true

@sys.description('this is vmWithCondition')
resource vmWithCondition 'Microsoft.Compute/virtualMachines@2020-06-01' = if (shouldDeployVm) {
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
//@[20:56) [BCP081 (Warning)] Resource type "My.Rp/extensionResource@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/extensionResource@2020-12-01'|
  name: 'extension'
  scope: vmWithCondition
}

resource extension2 'My.Rp/extensionResource@2020-12-01' = {
//@[20:56) [BCP081 (Warning)] Resource type "My.Rp/extensionResource@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/extensionResource@2020-12-01'|
  name: 'extension'
  scope: extension1
}

resource extensionDependencies 'My.Rp/mockResource@2020-01-01' = {
//@[31:62) [BCP081 (Warning)] Resource type "My.Rp/mockResource@2020-01-01" does not have types available. (CodeDescription: none) |'My.Rp/mockResource@2020-01-01'|
  name: 'extensionDependencies'
  properties: {
    res1: vmWithCondition.id
    res1runtime: vmWithCondition.properties.something
//@[44:53) [BCP053 (Warning)] The type "VirtualMachineProperties" does not contain property "something". Available properties include "additionalCapabilities", "availabilitySet", "billingProfile", "diagnosticsProfile", "evictionPolicy", "extensionsTimeBudget", "hardwareProfile", "host", "hostGroup", "instanceView", "licenseType", "networkProfile", "osProfile", "priority", "provisioningState", "proximityPlacementGroup", "securityProfile", "storageProfile", "virtualMachineScaleSet", "vmId". (CodeDescription: none) |something|
    res2: extension1.id
    res2runtime: extension1.properties.something
    res3: extension2.id
    res3runtime: extension2.properties.something
  }
}

@sys.description('this is existing1')
resource existing1 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[19:65) [BCP081 (Warning)] Resource type "Mock.Rp/existingExtensionResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/existingExtensionResource@2020-01-01'|
  name: 'existing1'
  scope: extension1
}

resource existing2 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[19:65) [BCP081 (Warning)] Resource type "Mock.Rp/existingExtensionResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/existingExtensionResource@2020-01-01'|
  name: 'existing2'
  scope: existing1
}

resource extension3 'My.Rp/extensionResource@2020-12-01' = {
//@[20:56) [BCP081 (Warning)] Resource type "My.Rp/extensionResource@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/extensionResource@2020-12-01'|
  name: 'extension3'
  scope: existing1
}

/*
  valid loop cases
*/ 
var storageAccounts = [
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
  name: 'vnet-${i}'
  properties: {
    subnets: [for j in range(0, 4): {
      // #completionTest(0,1,2,3,4,5) -> subnetIdAndProperties
     
      // #completionTest(6) -> subnetIdAndPropertiesNoColon
      name: 'subnet-${i}-${j}'
    }]
  }
}]

// duplicate identifiers within the loop are allowed
resource duplicateIdentifiersWithinLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
  name: 'vnet-${i}'
  properties: {
    subnets: [for i in range(0, 4): {
      name: 'subnet-${i}-${i}'
    }]
  }
}]

// duplicate identifers in global and single loop scope are allowed (inner variable hides the outer)
var canHaveDuplicatesAcrossScopes = 'hello'
//@[4:33) [no-unused-vars (Warning)] Variable "canHaveDuplicatesAcrossScopes" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |canHaveDuplicatesAcrossScopes|
resource duplicateInGlobalAndOneLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for canHaveDuplicatesAcrossScopes in range(0, 3): {
  name: 'vnet-${canHaveDuplicatesAcrossScopes}'
  properties: {
    subnets: [for i in range(0, 4): {
      name: 'subnet-${i}-${i}'
    }]
  }
}]

// duplicate in global and multiple loop scopes are allowed (inner hides the outer)
var duplicatesEverywhere = 'hello'
//@[4:24) [no-unused-vars (Warning)] Variable "duplicatesEverywhere" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |duplicatesEverywhere|
resource duplicateInGlobalAndTwoLoops 'Microsoft.Network/virtualNetworks@2020-06-01' = [for duplicatesEverywhere in range(0, 3): {
  name: 'vnet-${duplicatesEverywhere}'
  properties: {
    subnets: [for duplicatesEverywhere in range(0, 4): {
      name: 'subnet-${duplicatesEverywhere}'
    }]
  }
}]

/*
  Scope values created via array access on a resource collection
*/
resource dnsZones 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in range(0,4): {
  name: 'zone${zone}'
  location: 'global'
}]

resource locksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for lock in range(0,2): {
  name: 'lock${lock}'
  properties: {
    level: 'CanNotDelete'
  }
  scope: dnsZones[lock]
}]

resource moreLocksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for (lock, i) in range(0,3): {
  name: 'another${i}'
  properties: {
    level: 'ReadOnly'
  }
  scope: dnsZones[i]
}]

resource singleLockOnFirstZone 'Microsoft.Authorization/locks@2016-09-01' = {
  name: 'single-lock'
  properties: {
    level: 'ReadOnly'
  }
  scope: dnsZones[0]
}


resource p1_vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
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
  parent: p1_vnet
  name: 'subnet1'
  properties: {
    addressPrefix: '10.0.0.0/24'
  }
}

resource p1_subnet2 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  parent: p1_vnet
  name: 'subnet2'
  properties: {
    addressPrefix: '10.0.1.0/24'
  }
}

output p1_subnet1prefix string = p1_subnet1.properties.addressPrefix
output p1_subnet1name string = p1_subnet1.name
output p1_subnet1type string = p1_subnet1.type
output p1_subnet1id string = p1_subnet1.id

// parent property with extension resource
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[17:53) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1@2020-06-01'|
  name: 'res1'
}

resource p2_res1child 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[22:65) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1/child1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1/child1@2020-06-01'|
  parent: p2_res1
  name: 'child1'
}

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[17:53) [BCP081 (Warning)] Resource type "Microsoft.Rp2/resource2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp2/resource2@2020-06-01'|
  scope: p2_res1child
  name: 'res2'
}

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[22:65) [BCP081 (Warning)] Resource type "Microsoft.Rp2/resource2/child2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp2/resource2/child2@2020-06-01'|
  parent: p2_res2
  name: 'child2'
}

output p2_res2childprop string = p2_res2child.properties.someProp
output p2_res2childname string = p2_res2child.name
output p2_res2childtype string = p2_res2child.type
output p2_res2childid string = p2_res2child.id

// parent property with 'existing' resource
resource p3_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[17:53) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1@2020-06-01'|
  name: 'res1'
}

resource p3_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[19:62) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1/child1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1/child1@2020-06-01'|
  parent: p3_res1
  name: 'child1'
}

output p3_res1childprop string = p3_child1.properties.someProp
output p3_res1childname string = p3_child1.name
output p3_res1childtype string = p3_child1.type
output p3_res1childid string = p3_child1.id

// parent & child with 'existing'
resource p4_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[17:53) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1@2020-06-01'|
  scope: tenant()
  name: 'res1'
}

resource p4_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' existing = {
//@[19:62) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1/child1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1/child1@2020-06-01'|
  parent: p4_res1
  name: 'child1'
}

output p4_res1childprop string = p4_child1.properties.someProp
output p4_res1childname string = p4_child1.name
output p4_res1childtype string = p4_child1.type
output p4_res1childid string = p4_child1.id

