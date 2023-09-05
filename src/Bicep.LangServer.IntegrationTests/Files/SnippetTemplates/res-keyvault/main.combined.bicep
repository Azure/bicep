// $1 = keyVault
// $2 = 'name'
// $3 = location
// $4 = 'tenantId'
// $5 = 'objectId'

param location string

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: 'name'
  location: location
  properties: {
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
    tenantId: 'tenantId'
//@[14:24) [BCP333 (Warning)] The provided value (whose length will always be less than or equal to 8) is too short to assign to a target for which the minimum allowable length is 36. (CodeDescription: none) |'tenantId'|
    accessPolicies: [
      {
        tenantId: 'tenantId'
//@[18:28) [BCP333 (Warning)] The provided value (whose length will always be less than or equal to 8) is too short to assign to a target for which the minimum allowable length is 36. (CodeDescription: none) |'tenantId'|
        objectId: 'objectId'
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


