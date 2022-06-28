param accountName string

resource storageAcc 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: accountName
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  location: resourceGroup().location
}

resource lockResource 'Microsoft.Authorization/locks@2016-09-01' = {
  name: 'DontDelete'
  scope: storageAcc
  properties: {
    level: 'CanNotDelete'
  }
}
