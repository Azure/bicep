// Key Vault Secret
resource /*${1:keyVaultSecret}*/keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: /*${2:'keyVaultName/name'}*/'keyVaultName/name'
  properties: {
    value: /*${3:'value'}*/'value'
  }
}
