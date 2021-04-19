// Recovery Service Vault
resource ${1:'recoveryServiceVault'} 'Microsoft.RecoveryServices/vaults@2021-01-01' = {
  name: 'name'
  location: resourceGroup().location
  sku: {
    name: '${2|RS0,Standard|}'
    tier: 'Standard'
  }
}
