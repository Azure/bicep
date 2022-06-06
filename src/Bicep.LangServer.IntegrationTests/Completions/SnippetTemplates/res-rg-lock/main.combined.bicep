// $1 = resourceGroupLock
// $2 = 'name'
// $3 = 'NotSpecified'

resource resourceGroupLock 'Microsoft.Authorization/locks@2020-05-01' = {
  name: 'name'
  properties: {
    level: 'NotSpecified'
  }
}
// Insert snippet here

