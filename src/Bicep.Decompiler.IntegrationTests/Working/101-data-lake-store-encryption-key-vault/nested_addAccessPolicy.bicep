param resourceId_Microsoft_DataLakeStore_accounts_parameters_dataLakeStoreName object

@description('The Azure Key Vault name.')
param keyVaultName string

resource keyVaultName_add 'Microsoft.KeyVault/vaults/accessPolicies@2019-09-01' = {
  name: '${keyVaultName}/add'
  properties: {
    accessPolicies: [
      {
        objectId: resourceId_Microsoft_DataLakeStore_accounts_parameters_dataLakeStoreName.identity.principalId
        tenantId: resourceId_Microsoft_DataLakeStore_accounts_parameters_dataLakeStoreName.identity.tenantId
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
