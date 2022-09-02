// $1 = recoveryServiceVault
// $2 = 'name'
// $3 = location
// $4 = 'RS0'
// $5 = 'Standard'

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

