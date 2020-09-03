// assumes key vault is in same subscription and rg as deployment

param location string = 'eastus'

param existingKeyVaultName string
param secretName string = 'superSecretPassword'
param secretValue string {
  secure: true
}

resource secret 'Microsoft.KeyVault/vaults/secrets@2018-02-14' = {
  name: '${existingKeyVaultName}/${secretName}'
  location: location
  properties: {
    value: secretValue
  }
}