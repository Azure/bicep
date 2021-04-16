// KeyVault Secret
resource keyVault_keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${1:keyVault}/${2:keyVaultSecret}'
  properties: {
    value: ${3:secretValue}
  }
}
