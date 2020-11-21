// The name of the Azure Redis Cache to create.
param redisCacheName string
// The location of the Redis Cache. For best performance, use the same location as the app to be used with the cache.
param location string = resourceGroup().location
// The size of the new Azure Redis Cache instance. Valid family and capacity combinations are (C0..C6, P1..P4).
param redisCacheCapacity int {
  allowed: [
    1
    2
    3
    4
  ]
  default: 1
}
// Name for the resource group containing the storage accounts
param storageRgName string = resourceGroup().name
// Set to true to allow access to redis on port 6379, without SSL tunneling (less secure).
param enableNonSslPort bool = false

module prereqs './prereqs.bicep' = {
  scope: resourceGroup(storageRgName)
  name: 'prereqs'
  params: {
    location: location
  }
}

var cacheAccountKey = listKeys(prereqs.outputs.cacheAccountId, '2019-06-01').keys[0].value
var cacheAccountName = prereqs.outputs.cacheAccountName

resource cache 'Microsoft.Cache/redis@2019-07-01' = {
  name: redisCacheName
  location: location
  properties: {
    enableNonSslPort: enableNonSslPort
    sku: {
      capacity: redisCacheCapacity
      family: 'P'
      name: 'Premium'
    }
    redisConfiguration: {
      'rdb-backup-enabled': 'true'
      'rdb-backup-frequency': '60'
      'rdb-storage-connection-string': 'DefaultEndpointsProtocol=https;AccountName=${cacheAccountName};AccountKey=${cacheAccountKey}'
    }
  }
}

resource diagSettings 'Microsoft.Insights/diagnosticSettings@2015-07-01' = {
  scope: cache
  name: 'service'
  location: location
  properties: {
    storageAccountId: prereqs.outputs.diagAccountId
  }
}
