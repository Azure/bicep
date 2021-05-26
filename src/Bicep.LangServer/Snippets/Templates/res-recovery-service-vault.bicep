// Recovery Service Vault
resource ${1:recoveryServiceVault} 'Microsoft.RecoveryServices/vaults@2021-01-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  sku: {
    name: ${3|'RS0','Standard'|}
    tier: ${4:'Standard'}
  }
}
