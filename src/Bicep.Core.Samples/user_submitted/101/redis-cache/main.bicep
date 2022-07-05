param environmentName string
param redisCacheSku string
param redisCacheFamily string
param redisCacheCapacity int
param location string

var redisCacheName = '${environmentName}${uniqueString(resourceGroup().id)}'

resource redisCache 'Microsoft.Cache/redis@2019-07-01' = {
  name: redisCacheName
  location: location
  properties: {
    enableNonSslPort: false
    minimumTlsVersion: '1.2'
    sku: {
      family: redisCacheFamily
      capacity: redisCacheCapacity
      name: redisCacheSku
    }
  }
}
