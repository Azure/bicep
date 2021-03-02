// The name of the Azure Redis Cache to create.
param redisCacheName string
// The location of the Redis Cache. For best performance, use the same location as the app to be used with the cache.
param location string = resourceGroup().location
// Number of highly available shards to create in the cluster. Requires Premium SKU.
param redisShardCount int
// The size of the new Azure Redis Cache instance. Valid family and capacity combinations are (C0..C6, P1..P4).

@allowed([
  1
  2
  3
  4
])
param redisCacheCapacity int = 1

// ResourceId of an existing storage account for diagnostics. Must be in the same subscription.
param diagsStorageAccountId string
// Set to true to allow access to redis on port 6379, without SSL tunneling (less secure).
param enableNonSslPort bool = false

resource cache 'Microsoft.Cache/redis@2015-08-01' = {
  name: redisCacheName
  location: location
  properties: {
    enableNonSslPort: enableNonSslPort
    shardCount: redisShardCount
    sku: {
      capacity: redisCacheCapacity
      family: 'P'
      name: 'Premium'
    }
  }
}

resource diagSettings 'Microsoft.Insights/diagnosticsettings@2015-07-01' = {
  scope: cache
  name: 'service'
  location: location
  properties: {
    storageAccountId: diagsStorageAccountId
  }
}
