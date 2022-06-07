@description('The name of the Data Lake Store account to create.')
param dataLakeStoreName string = uniqueString(resourceGroup().id)

@description('The location in which to create the Data Lake Store account.')
param location string = resourceGroup().location

@description('The Azure Key Vault name.')
param keyVaultName string

@description('The Azure Key Vault resource group name.')
param keyVaultResourceGroupName string

@description('The Azure Key Vault encryption key name.')
param keyName string

@description('The Azure Key Vault encryption key version.')
param keyVersion string

resource dataLakeStoreName_resource 'Microsoft.DataLakeStore/accounts@2016-11-01' = {
  name: dataLakeStoreName
  location: location
  properties: {
    encryptionState: 'Enabled'
    encryptionConfig: {
      type: 'UserManaged'
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

module addAccessPolicy './nested_addAccessPolicy.bicep' = {
  name: 'addAccessPolicy'
  scope: resourceGroup(keyVaultResourceGroupName)
  params: {
    resourceId_Microsoft_DataLakeStore_accounts_parameters_dataLakeStoreName: reference(dataLakeStoreName_resource.id, '2016-11-01', 'Full')
    keyVaultName: keyVaultName
  }
}

module updateAdlsAccount './nested_updateAdlsAccount.bicep' = {
  name: 'updateAdlsAccount'
  params: {
    resourceId_parameters_keyVaultResourceGroupName_Microsoft_KeyVault_vaults_parameters_keyVaultName: resourceId(keyVaultResourceGroupName, 'Microsoft.KeyVault/vaults', keyVaultName)
    dataLakeStoreName: dataLakeStoreName
    location: location
    keyName: keyName
    keyVersion: keyVersion
  }
  dependsOn: [
    addAccessPolicy
  ]
}
