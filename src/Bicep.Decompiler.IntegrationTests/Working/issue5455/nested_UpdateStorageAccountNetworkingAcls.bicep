param storageLocation string
param defaultDataLakeStorageAccountName string
param workspaceStorageAccountProperties object

resource defaultDataLakeStorageAccountName_resource 'Microsoft.Storage/storageAccounts@2021-01-01' = {
//@[9:51) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "kind", "sku". (CodeDescription: none) |defaultDataLakeStorageAccountName_resource|
  location: storageLocation
  name: defaultDataLakeStorageAccountName
  properties: workspaceStorageAccountProperties
}
