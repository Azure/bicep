// KeyVault
resource /*${1:keyVault}*/keyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
    tenantId: /*${3:'tenantId'}*/'tenantId'
    accessPolicies: [
      {
        tenantId: /*${3:'tenantId'}*/'tenantId'
        objectId: /*${4:'objectId'}*/'objectId'
        permissions: {
          keys: [
            'get'
          ]
          secrets: [
            'list'
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
