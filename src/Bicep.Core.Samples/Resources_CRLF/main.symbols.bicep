
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[9:21) Resource basicStorage. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 119
  name: 'basicblobs'
  location: 'westus'
}

resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
//@[9:16) Resource dnsZone. Type: Microsoft.Network/dnszones@2018-05-01. Declaration start char: 0, length: 103
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
//@[9:24) Resource withExpressions. Type: Microsoft.Storage/storageAccounts@2017-10-01. Declaration start char: 0, length: 538
  name: 'myencryptedone'
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
//@[4:12) Variable location. Type: any. Declaration start char: 0, length: 39

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
//@[9:13) Resource farm. Type: Microsoft.Web/serverFarms@2019-08-01. Declaration start char: 0, length: 371
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
