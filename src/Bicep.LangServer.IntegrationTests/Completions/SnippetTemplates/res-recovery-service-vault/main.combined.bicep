// $1 = recoveryServiceVault
// $2 = 'name'
// $3 = 'RS0'
// $4 = 'Standard'

param location string

resource recoveryServiceVault 'Microsoft.RecoveryServices/vaults@2021-01-01' = {
  name: 'name'
  location: location
  sku: {
    name: 'RS0'
    tier: 'Standard'
  }
}
// Insert snippet here

