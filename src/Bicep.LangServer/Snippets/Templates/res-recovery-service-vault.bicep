// Recovery Service Vault
resource /*${1:recoveryServiceVault}*/recoveryServiceVault 'Microsoft.RecoveryServices/vaults@2021-01-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  sku: {
    name: /*${3|'RS0','Standard'|}*/'RS0'
    tier: /*${4:'Standard'}*/'Standard'
  }
}
