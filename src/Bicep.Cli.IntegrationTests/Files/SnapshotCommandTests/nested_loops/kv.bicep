param permissions object[]

var vaults = [
  {
    name: 'myVault'
    permissions: permissions
    keyVaultSku: 'standard'
  }
]

@batchSize(1)
resource keyVaults 'Microsoft.KeyVault/vaults@2019-09-01' = [for vault in vaults: {
  name: vault.name
  location: resourceGroup().location
  properties: {
    sku: {
      family: 'A'
      name: vault.keyVaultSku
    }
    tenantId: subscription().tenantId
    accessPolicies: [for permission in vault.permissions: {
      tenantId: subscription().tenantId
      objectId: '00000000-0000-0000-0000-000000000000'
      permissions: permission.permissions
    }]
  }
}]
