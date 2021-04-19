@description('The Azure Key Vault name.')
param keyVaultName string

@description('The name of the Data Lake Store account to create.')
param dataLakeStoreName string

resource keyVaultName_add 'Microsoft.KeyVault/vaults/accessPolicies@2019-09-01' = {
  name: '${keyVaultName}/add'
  properties: {
    accessPolicies: [
      {
        objectId: reference(resourceId('Microsoft.DataLakeStore/accounts', dataLakeStoreName), '2016-11-01', 'Full').identity.principalId
        tenantId: reference(resourceId('Microsoft.DataLakeStore/accounts', dataLakeStoreName), '2016-11-01', 'Full').identity.tenantId
        permissions: {
          keys: [
            'encrypt'
            'decrypt'
          ]
        }
      }
    ]
  }
}
