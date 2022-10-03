param resourceId_parameters_keyVaultResourceGroupName_Microsoft_KeyVault_vaults_parameters_keyVaultName string

@description('The name of the Data Lake Store account to create.')
param dataLakeStoreName string

@description('The location in which to create the Data Lake Store account.')
param location string

@description('The Azure Key Vault encryption key name.')
param keyName string

@description('The Azure Key Vault encryption key version.')
param keyVersion string

resource dataLakeStore 'Microsoft.DataLakeStore/accounts@2016-11-01' = {
  name: dataLakeStoreName
  location: location
  properties: {
    encryptionConfig: {
//@[4:20) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "type". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |encryptionConfig|
      keyVaultMetaInfo: {
        keyVaultResourceId: resourceId_parameters_keyVaultResourceGroupName_Microsoft_KeyVault_vaults_parameters_keyVaultName
        encryptionKeyName: keyName
        encryptionKeyVersion: keyVersion
      }
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}
