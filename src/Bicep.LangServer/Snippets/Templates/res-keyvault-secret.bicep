// Key Vault Secret
resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: /*${1:'keyVaultName'}*/'keyVaultName'
}

resource /*${2:keyVaultSecret}*/keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  parent: keyVault
  name: /*${3:'name'}*/'name'
  properties: {
    value: /*${4:'value'}*/'value'
  }
}
