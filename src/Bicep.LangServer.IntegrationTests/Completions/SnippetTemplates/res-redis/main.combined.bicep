// $1 = redisCache
// $2 = 'name'

resource redisCache 'Microsoft.Cache/Redis@2019-07-01' = {
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
// Insert snippet here

