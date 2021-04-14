resource keyVault_keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2016-10-01' = {
  name: 'testKeyVault/testKeyVaultSecret'
  properties: {
    value: 'testSecret'
  }
}
