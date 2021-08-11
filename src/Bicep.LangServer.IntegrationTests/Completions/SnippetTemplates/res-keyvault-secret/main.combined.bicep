// $1 = keyVaultSecret
// $2 = 'keyVaultName/name'
// $3 = 'value'

resource keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: 'keyVaultName/name'
  properties: {
    value: 'value'
  }
}
// Insert snippet here

