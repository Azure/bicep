@description('Storage Account type')
param storageAccountType string = 'Standard_LRS'

@description('The storage account location.')
param location string = resourceGroup().location

@description('The name of the storage account')
param storageAccountName string = 'store${uniqueString(resourceGroup().id)}'

resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'StorageV2'
  properties: {}
}


output storageAccountType string = storageAccountType
output blobUri string = sa.properties.primaryEndpoints.blob
output objectWithExpressions object = {
  foo: 'bar'
  baz: 123
  qux: sa.properties.primaryEndpoints.blob
}
output objectLiteral object = {
  storageAccountType: storageAccountType
  baz: 123
}
