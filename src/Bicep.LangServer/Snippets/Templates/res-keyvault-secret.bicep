// Key Vault Secret
resource keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${1:keyVault}/${2:secret}'
  properties: {
    value: ${3:'value'}
  }
}
