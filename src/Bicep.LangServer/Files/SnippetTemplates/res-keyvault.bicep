// KeyVault
resource /*${1:keyVault}*/keyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
    tenantId: /*${4:'tenantId'}*/'tenantId'
    accessPolicies: [
      {
        tenantId: /*${4:'tenantId'}*/'tenantId'
        objectId: /*${5:'objectId'}*/'objectId'
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
