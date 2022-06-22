
@sys.description('this is basicStorage')
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[57:69]       "type": "Microsoft.Storage/storageAccounts",
  name: 'basicblobs'
  location: 'westus'
  kind: 'BlobStorage'
  sku: {
    name: 'Standard_GRS'
  }
}

@sys.description('this is dnsZone')
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
//@[70:78]       "type": "Microsoft.Network/dnsZones",
  name: 'myZone'
  location: 'global'
}

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[79:103]       "type": "Microsoft.Storage/storageAccounts",
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
//@[104:131]       "type": "Microsoft.Storage/storageAccounts",
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
//@[11:14]     "applicationName": {
var hostingPlanName = applicationName // why not just use the param directly?
//@[33:33]     "hostingPlanName": "[parameters('applicationName')]",

param appServicePlanTier string
//@[15:17]     "appServicePlanTier": {
param appServicePlanInstances int
//@[18:20]     "appServicePlanInstances": {

var location = resourceGroup().location
//@[34:34]     "location": "[resourceGroup().location]",

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
//@[132:144]       "type": "Microsoft.Web/serverfarms",
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
//@[35:35]     "cosmosDbResourceId": "[resourceId('Microsoft.DocumentDB/databaseAccounts', parameters('cosmosDb').account)]",
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint

// this variable is not accessed anywhere in this template and depends on a run-time reference
// it should not be present at all in the template output as there is nowhere logical to put it
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint

param webSiteName string
//@[21:23]     "webSiteName": {
param cosmosDb object
//@[24:26]     "cosmosDb": {
resource site 'Microsoft.Web/sites@2019-08-01' = {
//@[145:172]       "type": "Microsoft.Web/sites",
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
//@[36:36]     "_siteApiVersion": "2019-08-01",
var _siteType = site.type
//@[37:37]     "_siteType": "Microsoft.Web/sites",

output siteApiVersion string = site.apiVersion
//@[576:579]     "siteApiVersion": {
output siteType string = site.type
//@[580:583]     "siteType": {

resource nested 'Microsoft.Resources/deployments@2019-10-01' = {
//@[173:185]       "type": "Microsoft.Resources/deployments",
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
//@[186:200]       "type": "Microsoft.Foo/foos",
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
//@[201:205]       "type": "My.Rp/typeA",
  name: 'resourceA'
}

resource resourceB 'My.Rp/typeA/typeB@2020-01-01' = {
//@[206:213]       "type": "My.Rp/typeA/typeB",
  name: '${resourceA.name}/myName'
}

resource resourceC 'My.Rp/typeA/typeB@2020-01-01' = {
//@[214:229]       "type": "My.Rp/typeA/typeB",
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
//@[236:242]             "bId": "[resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1])]",
]

var resourceCRef = {
//@[38:40]     "resourceCRef": {
  id: resourceC.id
}
var setResourceCRef = true
//@[41:41]     "setResourceCRef": true,

resource resourceD 'My.Rp/typeD@2020-01-01' = {
//@[230:251]       "type": "My.Rp/typeD",
  name: 'constant'
  properties: {
    runtime: varBRuntime
    // repro for https://github.com/Azure/bicep/issues/316
    repro316: setResourceCRef ? resourceCRef : null
  }
}

var myInterpKey = 'abc'
//@[42:42]     "myInterpKey": "abc",
resource resourceWithInterp 'My.Rp/interp@2020-01-01' = {
//@[252:261]       "type": "My.Rp/interp",
  name: 'interpTest'
  properties: {
    '${myInterpKey}': 1
    'abc${myInterpKey}def': 2
    '${myInterpKey}abc${myInterpKey}': 3
  }
}

resource resourceWithEscaping 'My.Rp/mockResource@2020-01-01' = {
//@[262:269]       "type": "My.Rp/mockResource",
  name: 'test'
  properties: {
    // both key and value should be escaped in template output
    '[resourceGroup().location]': '[resourceGroup().location]'
  }
}

param shouldDeployVm bool = true
//@[27:30]     "shouldDeployVm": {

@sys.description('this is vmWithCondition')
resource vmWithCondition 'Microsoft.Compute/virtualMachines@2020-06-01' = if (shouldDeployVm) {
//@[270:286]       "condition": "[parameters('shouldDeployVm')]",
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
//@[287:295]       "type": "My.Rp/extensionResource",
  name: 'extension'
  scope: vmWithCondition
}

resource extension2 'My.Rp/extensionResource@2020-12-01' = {
//@[296:304]       "type": "My.Rp/extensionResource",
  name: 'extension'
  scope: extension1
}

resource extensionDependencies 'My.Rp/mockResource@2020-01-01' = {
//@[305:322]       "type": "My.Rp/mockResource",
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
  name: 'existing1'
  scope: extension1
}

resource existing2 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
  name: 'existing2'
  scope: existing1
}

resource extension3 'My.Rp/extensionResource@2020-12-01' = {
//@[323:331]       "type": "My.Rp/extensionResource",
  name: 'extension3'
  scope: existing1
}

/*
  valid loop cases
*/ 
var storageAccounts = [
//@[43:52]     "storageAccounts": [
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
//@[332:348]       "copy": {
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
//@[349:365]       "copy": {
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
//@[366:388]       "copy": {
  name: 'vnet-${i}'
  properties: {
    subnets: [for j in range(0, 4): {
//@[376:382]             "name": "subnets",
      // #completionTest(0,1,2,3,4,5) -> subnetIdAndProperties
     
      // #completionTest(6) -> subnetIdAndPropertiesNoColon
      name: 'subnet-${i}-${j}'
    }]
  }
}]

// duplicate identifiers within the loop are allowed
resource duplicateIdentifiersWithinLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[389:408]       "copy": {
  name: 'vnet-${i}'
  properties: {
    subnets: [for i in range(0, 4): {
//@[399:405]             "name": "subnets",
      name: 'subnet-${i}-${i}'
    }]
  }
}]

// duplicate identifers in global and single loop scope are allowed (inner variable hides the outer)
var canHaveDuplicatesAcrossScopes = 'hello'
//@[53:53]     "canHaveDuplicatesAcrossScopes": "hello",
resource duplicateInGlobalAndOneLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for canHaveDuplicatesAcrossScopes in range(0, 3): {
//@[409:428]       "copy": {
  name: 'vnet-${canHaveDuplicatesAcrossScopes}'
  properties: {
    subnets: [for i in range(0, 4): {
//@[419:425]             "name": "subnets",
      name: 'subnet-${i}-${i}'
    }]
  }
}]

// duplicate in global and multiple loop scopes are allowed (inner hides the outer)
var duplicatesEverywhere = 'hello'
//@[54:54]     "duplicatesEverywhere": "hello"
resource duplicateInGlobalAndTwoLoops 'Microsoft.Network/virtualNetworks@2020-06-01' = [for duplicatesEverywhere in range(0, 3): {
//@[429:448]       "copy": {
  name: 'vnet-${duplicatesEverywhere}'
  properties: {
    subnets: [for duplicatesEverywhere in range(0, 4): {
//@[439:445]             "name": "subnets",
      name: 'subnet-${duplicatesEverywhere}'
    }]
  }
}]

/*
  Scope values created via array access on a resource collection
*/
resource dnsZones 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in range(0,4): {
//@[449:458]       "copy": {
  name: 'zone${zone}'
  location: 'global'
}]

resource locksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for lock in range(0,2): {
//@[459:474]       "copy": {
  name: 'lock${lock}'
  properties: {
    level: 'CanNotDelete'
  }
  scope: dnsZones[lock]
}]

resource moreLocksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for (lock, i) in range(0,3): {
//@[475:490]       "copy": {
  name: 'another${i}'
  properties: {
    level: 'ReadOnly'
  }
  scope: dnsZones[i]
}]

resource singleLockOnFirstZone 'Microsoft.Authorization/locks@2016-09-01' = {
//@[491:502]       "type": "Microsoft.Authorization/locks",
  name: 'single-lock'
  properties: {
    level: 'ReadOnly'
  }
  scope: dnsZones[0]
}


resource p1_vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[503:515]       "type": "Microsoft.Network/virtualNetworks",
  location: resourceGroup().location
  name: 'myVnet'
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/20'
//@[511:511]             "10.0.0.0/20"
      ]
    }
  }
}

resource p1_subnet1 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[516:526]       "type": "Microsoft.Network/virtualNetworks/subnets",
  parent: p1_vnet
  name: 'subnet1'
  properties: {
    addressPrefix: '10.0.0.0/24'
  }
}

resource p1_subnet2 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[527:537]       "type": "Microsoft.Network/virtualNetworks/subnets",
  parent: p1_vnet
  name: 'subnet2'
  properties: {
    addressPrefix: '10.0.1.0/24'
  }
}

output p1_subnet1prefix string = p1_subnet1.properties.addressPrefix
//@[584:587]     "p1_subnet1prefix": {
output p1_subnet1name string = p1_subnet1.name
//@[588:591]     "p1_subnet1name": {
output p1_subnet1type string = p1_subnet1.type
//@[592:595]     "p1_subnet1type": {
output p1_subnet1id string = p1_subnet1.id
//@[596:599]     "p1_subnet1id": {

// parent property with extension resource
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[538:542]       "type": "Microsoft.Rp1/resource1",
  name: 'res1'
}

resource p2_res1child 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[543:550]       "type": "Microsoft.Rp1/resource1/child1",
  parent: p2_res1
  name: 'child1'
}

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[551:559]       "type": "Microsoft.Rp2/resource2",
  scope: p2_res1child
  name: 'res2'
}

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[560:568]       "type": "Microsoft.Rp2/resource2/child2",
  parent: p2_res2
  name: 'child2'
}

output p2_res2childprop string = p2_res2child.properties.someProp
//@[600:603]     "p2_res2childprop": {
output p2_res2childname string = p2_res2child.name
//@[604:607]     "p2_res2childname": {
output p2_res2childtype string = p2_res2child.type
//@[608:611]     "p2_res2childtype": {
output p2_res2childid string = p2_res2child.id
//@[612:615]     "p2_res2childid": {

// parent property with 'existing' resource
resource p3_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  name: 'res1'
}

resource p3_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[569:573]       "type": "Microsoft.Rp1/resource1/child1",
  parent: p3_res1
  name: 'child1'
}

output p3_res1childprop string = p3_child1.properties.someProp
//@[616:619]     "p3_res1childprop": {
output p3_res1childname string = p3_child1.name
//@[620:623]     "p3_res1childname": {
output p3_res1childtype string = p3_child1.type
//@[624:627]     "p3_res1childtype": {
output p3_res1childid string = p3_child1.id
//@[628:631]     "p3_res1childid": {

// parent & child with 'existing'
resource p4_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  scope: tenant()
  name: 'res1'
}

resource p4_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' existing = {
  parent: p4_res1
  name: 'child1'
}

output p4_res1childprop string = p4_child1.properties.someProp
//@[632:635]     "p4_res1childprop": {
output p4_res1childname string = p4_child1.name
//@[636:639]     "p4_res1childname": {
output p4_res1childtype string = p4_child1.type
//@[640:643]     "p4_res1childtype": {
output p4_res1childid string = p4_child1.id
//@[644:647]     "p4_res1childid": {

