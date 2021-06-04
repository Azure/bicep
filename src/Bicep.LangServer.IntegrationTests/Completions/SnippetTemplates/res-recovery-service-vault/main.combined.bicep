// $1 = recoveryServiceVault
// $2 = 'name'
// $3 = 'RS0'
// $4 = 'Standard'

resource recoveryServiceVault 'Microsoft.RecoveryServices/vaults@2021-01-01' = {
  name: 'name'
  location: resourceGroup().location
  sku: {
    name: 'RS0'
    tier: 'Standard'
  }
}
// Insert snippet here

