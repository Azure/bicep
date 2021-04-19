// Redis Cache
resource ${1:'redisCache'} 'Microsoft.Cache/Redis@2019-07-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 0
    }
  }
}
