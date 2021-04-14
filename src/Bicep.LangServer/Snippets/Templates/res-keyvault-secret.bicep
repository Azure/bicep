// KeyVault Secret
resource keyVault_keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2016-10-01' = {
  name: '${1:keyVault}/${2:keyVaultSecret}'
  properties: {
    value: '${3:secretValue}'
  }
}