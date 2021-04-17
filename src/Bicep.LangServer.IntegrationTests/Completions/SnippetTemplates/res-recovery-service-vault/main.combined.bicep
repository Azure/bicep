resource recoveryServiceVault 'Microsoft.RecoveryServices/vaults@2021-01-01' = {
  name: 'recoveryServiceVault'
  location: resourceGroup().location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}
