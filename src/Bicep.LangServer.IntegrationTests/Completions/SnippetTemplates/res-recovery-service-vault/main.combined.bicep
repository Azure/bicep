resource recoveryServiceVault 'Microsoft.RecoveryServices/vaults@2021-01-01' = {
  name: 'name'
  location: resourceGroup().location
  sku: {
    name: 'RS0'
    tier: 'Standard'
  }
}

