// Key Vault Secret
resource ${1:keyVaultSecret} 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: 'KeyVaultName/name'
  properties: {
    value: ${2:'value'}
  }
}
