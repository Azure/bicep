// $1 = 'keyVaultName'
// $2 = keyVaultSecret
// $3 = 'name'
// $4 = 'value'

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'keyVaultName'
}

resource keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  parent: keyVault
  name: 'name'
  properties: {
    value: 'value'
  }
}
// Insert snippet here

