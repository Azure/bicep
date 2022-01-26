param defaultDataLakeStorageAccountName string
param defaultDataLakeStorageFilesystemName string

resource defaultDataLakeStorageAccountName_default_defaultDataLakeStorageFilesystemName 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-01-01' = {
  name: '${defaultDataLakeStorageAccountName}/default/${defaultDataLakeStorageFilesystemName}'
  properties: {
    publicAccess: 'None'
  }
}
