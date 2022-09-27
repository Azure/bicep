param storageLocation string
param defaultDataLakeStorageAccountName string
param workspaceStorageAccountProperties object

resource defaultDataLakeStorageAccount 'Microsoft.Storage/storageAccounts@2021-01-01' = {
//@[9:38) [BCP035 (Warning)] The specified "resource" declaration is missing the following required properties: "kind", "sku". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |defaultDataLakeStorageAccount|
  location: storageLocation
  name: defaultDataLakeStorageAccountName
  properties: workspaceStorageAccountProperties
}
