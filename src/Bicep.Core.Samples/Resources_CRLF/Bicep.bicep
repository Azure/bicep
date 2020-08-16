
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'basicblobs'
  location: 'westus'
}

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
  name: 'myencryptedone'
  location: 'eastus2'
  properties: {
    supportsHttpsTrafficOnly: !false
    accessTier: true ? 'Hot' : 'Cold'
    encryption: {
      keySource: myStorageAccount.id
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
  }
}