// $1 = lock
// $2 = 'name'
// $3 = 'scopeResource'
// $4 = 'NotSpecified'

resource lock 'Microsoft.Authorization/locks@2017-04-01' = {
  name: 'name'
  scope: 'scopeResource'
  properties: {
    level: 'NotSpecified'
  }
}
// Insert snippet here


