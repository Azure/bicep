// Redis Cache
resource /*${1:redisCache}*/redisCache 'Microsoft.Cache/Redis@2019-07-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 0
    }
  }
}
