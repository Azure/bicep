// $1 = lock
// $2 = 'name'
// $3 = 'NotSpecified'

resource lock 'Microsoft.Authorization/locks@2017-04-01' = {
  name: 'name'
  properties: {
    level: 'NotSpecified'
  }
}
// Insert snippet here

