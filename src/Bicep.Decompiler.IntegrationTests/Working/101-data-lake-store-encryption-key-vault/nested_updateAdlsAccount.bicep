@description('The name of the Data Lake Store account to create.')
param dataLakeStoreName string

@description('The location in which to create the Data Lake Store account.')
param location string

@description('The Azure Key Vault resource group name.')
param keyVaultResourceGroupName string

@description('The Azure Key Vault name.')
param keyVaultName string

@description('The Azure Key Vault encryption key name.')
param keyName string

@description('The Azure Key Vault encryption key version.')
param keyVersion string

resource dataLakeStoreName_resource 'Microsoft.DataLakeStore/accounts@2016-11-01' = {
  name: dataLakeStoreName
  location: location
  properties: {
    encryptionConfig: {
//@[4:20) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "type". |encryptionConfig|
      keyVaultMetaInfo: {
        keyVaultResourceId: resourceId(keyVaultResourceGroupName, 'Microsoft.KeyVault/vaults', keyVaultName)
        encryptionKeyName: keyName
        encryptionKeyVersion: keyVersion
      }
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}
