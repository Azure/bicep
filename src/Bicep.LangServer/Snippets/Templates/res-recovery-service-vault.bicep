// Recovery Service Vault
resource recoveryServiceVault 'Microsoft.RecoveryServices/vaults@2021-01-01' = {
  name: ${1:'recoveryServiceVault'}
  location: resourceGroup().location
  sku: {
    name: '${2|RS0,Standard|}'
    tier: 'Standard'
  }
  properties:{}
}
