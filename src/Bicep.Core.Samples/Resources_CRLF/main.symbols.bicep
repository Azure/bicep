
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[9:21] Resource basicStorage
  name: 'basicblobs'
  location: 'westus'
}

resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
//@[9:16] Resource dnsZone
  name: 'myZone'
  location: 'global'
}

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[9:25] Resource myStorageAccount
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
//@[9:24] Resource withExpressions
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
}

param applicationName string = 'to-do-app${uniqueString(resourceGroup().id)}'
//@[6:21] Parameter applicationName
var hostingPlanName = applicationName // why not just use the param directly?
//@[4:19] Variable hostingPlanName

param appServicePlanTier string
//@[6:24] Parameter appServicePlanTier
param appServicePlanInstances int
//@[6:29] Parameter appServicePlanInstances

var location = resourceGroup().location
//@[4:12] Variable location

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
//@[9:13] Resource farm
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
//@[4:22] Variable cosmosDbResourceId
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint
//@[4:15] Variable cosmosDbRef

// this variable is not accessed anywhere in this template and depends on a run-time reference
// it should not be present at all in the template output as there is nowhere logical to put it
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint
//@[4:20] Variable cosmosDbEndpoint

param webSiteName string
//@[6:17] Parameter webSiteName
param cosmosDb object
//@[6:14] Parameter cosmosDb
resource site 'Microsoft.Web/sites@2019-08-01' = {
//@[9:13] Resource site
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
