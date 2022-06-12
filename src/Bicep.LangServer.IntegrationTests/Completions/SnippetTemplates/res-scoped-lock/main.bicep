// $1 = lock
// $2 = 'name'
// $3 = scopeResource
// $4 = 'NotSpecified'

resource scopeResource 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
  name: 'foo'
}

// Insert snippet here

