resource keyVault_keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: 'testKeyVault/testKeyVaultSecret'
  properties: {
    value: 'testSecret'
  }
}

