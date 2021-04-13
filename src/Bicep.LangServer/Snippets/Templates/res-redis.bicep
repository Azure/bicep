// Redis Cache
resource redisCache 'Microsoft.Cache/Redis@2019-07-01' = {
  name: ${1:redisCache}
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 0
    }
  }
}