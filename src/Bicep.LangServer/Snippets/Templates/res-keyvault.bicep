// KeyVault
resource ${1:keyVault} 'Microsoft.KeyVault/vaults@2016-10-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
    tenantId: ${3:'tenantId'}
    accessPolicies: [
      {
        tenantId: ${3:'tenantId'}
        objectId: ${4:'objectId'}
        permissions: {
          keys: [
            'get'
          ]
          secrets: [
            'list'
            'get'
            'get'
          ]
        }
      }
    ]
    sku: {
      name: 'standard'
      family: 'A'
    }
  }
}
