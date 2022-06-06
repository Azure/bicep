// $1 = resourceGroupLock
// $2 = 'name'
// $3 = 'NotSpecified'

resource resourceGroupLock 'Microsoft.Authorization/locks@2017-04-01' = {
  name: 'name'
  properties: {
    level: 'NotSpecified'
  }
}
// Insert snippet here

