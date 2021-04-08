// assumes key vault is in same subscription and rg as deployment
param existingKeyVaultName string
param secretName string = 'superSecretPassword'

@secure()
param secretValue string

resource secret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${existingKeyVaultName}/${secretName}'
  properties: {
    value: secretValue
  }
}
