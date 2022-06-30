param accountName string

resource storageAcc 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: accountName
}

resource lockResource 'Microsoft.Authorization/locks@2016-09-01' = {
  name: 'DontDelete'
  scope: storageAcc
  properties: {
    level: 'CanNotDelete'
  }
}
