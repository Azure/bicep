param keyVaultName string
param tenantId string
param location string
param secretName string
param objectId string
param functionName string

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: keyVaultName
  location: location
  properties: {
    tenantId: tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    accessPolicies: [
      {
        tenantId: tenantId
        objectId: objectId
        permissions: {
          secrets: [
            'get'
          ]
        }
      }
    ]
    enabledForDeployment: true
    enabledForDiskEncryption: true
    enabledForTemplateDeployment: true
  }
}

resource keyVaultName_secret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  parent: keyVault
  name: '${secretName}'
  location: location
  properties: {
    value: listkeys(resourceId('Microsoft.Web/sites/functions', functionName, 'HttpTrigger1'), '2020-06-01').default
  }
}
