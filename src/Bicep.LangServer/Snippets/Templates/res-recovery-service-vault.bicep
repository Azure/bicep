// Recovery Service Vault
resource /*${1:recoveryServiceVault}*/recoveryServiceVault 'Microsoft.RecoveryServices/vaults@2021-01-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  sku: {
    name: /*${4|'RS0','Standard'|}*/'RS0'
    tier: /*${5:'Standard'}*/'Standard'
  }
}
